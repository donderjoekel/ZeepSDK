using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepkistClient;
using ZeepSDK.Extensions;
using ZeepSDK.LevelEditor.Builders;
using ZeepSDK.LevelEditor.Patches;
using ZeepSDK.Scripting.Attributes;
using ZeepSDK.Utilities;

namespace ZeepSDK.LevelEditor;

/// <summary>
/// An API for interacting with the level editor
/// </summary>
[PublicAPI]
public static class LevelEditorApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(LevelEditorApi));
    private static readonly List<object> mouseInputBlockers = new();
    private static readonly List<object> keyboardInputBlockers = new();
    private static readonly List<CustomFolderBuilder> scheduledCustomFolderBuilders = new();

    private static SetupGame SetupGame => ComponentCache.Get<SetupGame>();

    private static GameObject gameObject;
    private static LEV_Inspector inspector;

    /// <summary>
    /// An event that is fired when the user enters test mode
    /// </summary>
    [GenerateEvent]
    public static event EnteredTestModeDelegate EnteredTestMode;

    /// <summary>
    /// An event that is fired when the user enters the level editor
    /// </summary>
    [GenerateEvent]
    public static event EnteredLevelEditorDelegate EnteredLevelEditor;

    /// <summary>
    /// An event that is fired when the user leaves the level editor
    /// </summary>
    [GenerateEvent]
    public static event ExitedLevelEditorDelegate ExitedLevelEditor;

    /// <summary>
    /// An event that is fired when the user loads an existing level from a file in the level editor 
    /// </summary>
    [GenerateEvent]
    public static event LevelLoadedDelegate LevelLoaded;

    /// <summary>
    /// An event that is fired whenever the user saves a level in the level editor
    /// </summary>
    [GenerateEvent]
    public static event LevelSavedDelegate LevelSaved;

    /// <summary>
    /// An event that is fired whenever the block selection changes
    /// </summary>
    [GenerateEvent]
    public static event SelectionChangedDelegate SelectionChanged;

    /// <summary>
    /// Boolean indicating whether the mouse input is currently being blocked
    /// </summary>
    public static bool IsMouseInputBlocked => mouseInputBlockers.Count > 0;

    /// <summary>
    /// Boolean indicating whether the keyboard input is currently being blocked
    /// </summary>
    public static bool IsKeyboardInputBlocked => keyboardInputBlockers.Count > 0;
    
    /// <summary>
    /// Boolean indicating whether the user is in the level editor or not
    /// </summary>
    public static bool IsInLevelEditor { get; private set; }
    
    /// <summary>
    /// Boolean indicating whether the user is testing their custom-made level or not
    /// </summary>
    public static bool IsTestingLevel { get; private set; }

    internal static void Initialize(GameObject gameObject)
    {
        LevelEditorApi.gameObject = gameObject;

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name != "GameScene")
                return;

            if (ZeepkistNetwork.IsConnectedToGame)
                return;

            if (SetupGame.GlobalLevel.IsTestLevel)
            {
                EnteredTestMode.InvokeSafe();
                IsTestingLevel = true;
            }
        };

        LEV_Inspector_Awake.Awake += inspector =>
        {
            if (LevelEditorApi.inspector != inspector)
            {
                LevelEditorApi.inspector = inspector;
                AddScheduledCustomFolders();
            }

            ComponentCache.Get<LEV_Selection>().ThingsJustGotSelected.AddListener(HandleThingsJustGotSelected);
            ComponentCache.Get<LEV_Selection>().ThingsJustGotDeselected.AddListener(HandleThingsJustGotDeselected);

            EnteredLevelEditor.InvokeSafe();
            IsInLevelEditor = true;
            IsTestingLevel = false;
        };

        LEV_LevelEditorCentral_OnDestroy.PostfixEvent += () =>
        {
            ExitedLevelEditor.InvokeSafe();
            IsInLevelEditor = false;
        };
        LEV_SaveLoad_ExternalLoad.PostfixEvent += () => LevelLoaded.InvokeSafe();
        LEV_SaveLoad_ExternalSaveFile.PostfixEvent += () => LevelSaved.InvokeSafe();
    }

    private static void HandleThingsJustGotSelected()
    {
        SelectionChanged.InvokeSafe(ComponentCache.Get<LEV_Selection>().list);
    }

    private static void HandleThingsJustGotDeselected()
    {
        SelectionChanged.InvokeSafe(ComponentCache.Get<LEV_Selection>().list);
    }

    /// <summary>
    /// Method that can be used to block mouse input. The blocker object that is passed along is used for identification and ensuring that the caller only can hold one block at a time
    /// </summary>
    /// <param name="blocker">The blocker to use for identification</param>
    public static void BlockMouseInput(object blocker)
    {
        if (mouseInputBlockers.Contains(blocker))
            return;
        mouseInputBlockers.Add(blocker);
    }

    /// <summary>
    /// Method that can be used to unblock mouse input that has been blocked with <see cref="BlockMouseInput"/>
    /// </summary>
    /// <param name="blocker">The blocker to use for identification</param>
    public static void UnblockMouseInput(object blocker)
    {
        mouseInputBlockers.Remove(blocker);
    }

    /// <summary>
    /// Method that can be used to block keyboard input. The blocker object that is passed along is used for identification and ensuring that the caller only can hold one block at a time
    /// </summary>
    /// <param name="blocker">The blocker to use for identification</param>
    public static void BlockKeyboardInput(object blocker)
    {
        try
        {
            if (keyboardInputBlockers.Contains(blocker))
                return;

            int countBefore = keyboardInputBlockers.Count;
            keyboardInputBlockers.Add(blocker);

            if (countBefore != 0)
                return;

            InputRegister inputRegister = ComponentCache.Get<InputRegister>();
            foreach (InputPlayerScriptableObject input in inputRegister.Inputs)
            {
                input.DisableLevelEditorInput();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(BlockKeyboardInput)}: " + e);
        }
    }

    /// <summary>
    /// Method that can be used to unblock keyboard input that has been blocked with <see cref="BlockKeyboardInput"/>
    /// </summary>
    /// <param name="blocker">The blocker to use for identification</param>
    public static void UnblockKeyboardInput(object blocker)
    {
        try
        {
            int countBefore = keyboardInputBlockers.Count;
            keyboardInputBlockers.Remove(blocker);

            if (countBefore <= 0 || keyboardInputBlockers.Count != 0)
                return;

            InputRegister inputRegister = ComponentCache.Get<InputRegister>();
            foreach (InputPlayerScriptableObject input in inputRegister.Inputs)
            {
                input.EnableLevelEditorInput();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(UnblockKeyboardInput)}: " + e);
        }
    }

    /// <summary>
    /// Creates a new block in the level editor with the specified properties
    /// </summary>
    /// <param name="blockProperties">The block to create</param>
    /// <param name="position">The position to apply to the newly created block</param>
    /// <param name="rotation">The rotation to apply to the newly created block</param>
    /// <param name="scale">The scale to apply to the newly created block</param>
    /// <returns>The newly created block</returns>
    public static BlockProperties CreateNewBlock(
        BlockProperties blockProperties,
        Vector3? position = null,
        Quaternion? rotation = null,
        Vector3? scale = null
    )
    {
        return CreateNewBlock(blockProperties.blockID, position, rotation, scale);
    }

    /// <summary>
    /// Creates a new block in the level editor with the specified properties
    /// </summary>
    /// <param name="blockProperties">The block to create</param>
    /// <param name="position">The position to apply to the newly created block</param>
    /// <param name="rotation">The rotation to apply to the newly created block</param>
    /// <param name="scale">The scale to apply to the newly created block</param>
    /// <param name="removeFromSelection">Should the block be removed from selection</param>
    /// <returns>The newly created block</returns>
    public static BlockProperties CreateNewBlock(
        BlockProperties blockProperties,
        Vector3? position = null,
        Quaternion? rotation = null,
        Vector3? scale = null,
        bool removeFromSelection = false
    )
    {
        return CreateNewBlock(blockProperties.blockID, position, rotation, scale, removeFromSelection);
    }

    /// <summary>
    /// Creates a new block in the level editor with the specified properties
    /// </summary>
    /// <param name="blockId">The internal block id for the block you want to create</param>
    /// <param name="position">The position to apply to the newly created block</param>
    /// <param name="rotation">The rotation to apply to the newly created block</param>
    /// <param name="scale">The scale to apply to the newly created block</param>
    /// <returns>The newly created block</returns>
    public static BlockProperties CreateNewBlock(
        int blockId,
        Vector3? position = null,
        Quaternion? rotation = null,
        Vector3? scale = null
    )
    {
        try
        {
            inspector.central.gizmos.CreateNewBlock(blockId);

            BlockProperties createdBlock = inspector.central.gizmos.central.selection.list.Last();

            if (position.HasValue)
                createdBlock.transform.position = position.Value;

            if (rotation.HasValue)
                createdBlock.transform.rotation = rotation.Value;

            if (scale.HasValue)
                createdBlock.transform.localScale = scale.Value;

            return createdBlock;
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(CreateNewBlock)}: " + e);
            return null;
        }
    }

    /// <summary>
    /// Creates a new block in the level editor with the specified properties
    /// </summary>
    /// <param name="blockId">The internal block id for the block you want to create</param>
    /// <param name="position">The position to apply to the newly created block</param>
    /// <param name="rotation">The rotation to apply to the newly created block</param>
    /// <param name="scale">The scale to apply to the newly created block</param>
    /// <param name="removeFromSelection">Should the block be removed from selection</param>
    /// <returns>The newly created block</returns>
    public static BlockProperties CreateNewBlock(
        int blockId,
        Vector3? position = null,
        Quaternion? rotation = null,
        Vector3? scale = null,
        bool removeFromSelection = false
    )
    {
        try
        {
            BlockProperties blockProperties = CreateNewBlock(blockId, position, rotation, scale);

            if (removeFromSelection)
                RemoveFromSelection(blockProperties);

            return blockProperties;
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(CreateNewBlock)}: " + e);
            return null;
        }
    }

    /// <summary>
    /// Adds the specified block to the selection in the level editor
    /// </summary>
    /// <param name="blockProperties"></param>
    public static void AddToSelection(BlockProperties blockProperties)
    {
        try
        {
            inspector.central.selection.AddThisBlock(blockProperties);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddToSelection)}: " + e);
        }
    }

    /// <summary>
    /// Removes the specified block from the selection in the level editor
    /// </summary>
    /// <param name="blockProperties"></param>
    public static void RemoveFromSelection(BlockProperties blockProperties)
    {
        try
        {
            int index = inspector.central.selection.list.IndexOf(blockProperties);
            if (index == -1)
                return;

            inspector.central.selection.RemoveBlockAt(index, false, false);

            if (inspector.central.selection.list.Count == 0)
            {
                inspector.central.gizmos.GoOutOfGMode();
            }

            inspector.central.selection.ThingsJustGotDeselected.Invoke();
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RemoveFromSelection)}: " + e);
        }
    }

    /// <summary>
    /// Clears the selection in the level editor
    /// </summary>
    [GenerateFunction]
    public static void ClearSelection()
    {
        try
        {
            inspector.central.selection.ClickNothing();
            inspector.central.gizmos.GoOutOfGMode();
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(ClearSelection)}: " + e);
        }
    }

    /// <summary>
    /// Adds a custom folder to the block gui
    /// </summary>
    /// <param name="builder">A callback that is used to create/customize the folder</param>
    public static void AddCustomFolder(Action<ICustomFolderBuilder> builder)
    {
        try
        {
            CustomFolderBuilder customFolderBuilder = new(gameObject);
            builder(customFolderBuilder);

            if (inspector == null)
            {
                scheduledCustomFolderBuilders.Add(customFolderBuilder);
            }
            else
            {
                AddCustomFolderBuilder(customFolderBuilder);
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddCustomFolder)}: " + e);
        }
    }

    private static void AddScheduledCustomFolders()
    {
        foreach (CustomFolderBuilder scheduledCustomFolderBuilder in scheduledCustomFolderBuilders)
        {
            AddCustomFolderBuilder(scheduledCustomFolderBuilder);
        }
    }

    private static void AddCustomFolderBuilder(CustomFolderBuilder customFolderBuilder)
    {
        try
        {
            BlocksFolder blocksFolder = customFolderBuilder.Build();
            blocksFolder.hasParent = true;
            blocksFolder.parent = inspector.globalBlockList.globalBlocksFolder;
            inspector.globalBlockList.globalBlocksFolder.folders.Add(blocksFolder);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddCustomFolderBuilder)}: " + e);
        }
    }
}
