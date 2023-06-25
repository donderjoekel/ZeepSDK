using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepSDK.LevelEditor.Builders;
using ZeepSDK.LevelEditor.Patches;
using ZeepSDK.Utilities;
using Object = UnityEngine.Object;

namespace ZeepSDK.LevelEditor;

/// <summary>
/// An API for interacting with the level editor
/// </summary>
[PublicAPI]
public static class LevelEditorApi
{
    private static readonly ManualLogSource logger = Plugin.CreateLogger(nameof(LevelEditorApi));
    private static readonly List<object> mouseInputBlockers = new();
    private static readonly List<object> keyboardInputBlockers = new();
    private static readonly List<CustomFolderBuilder> scheduledCustomFolderBuilders = new();

    private static SetupGame SetupGame => ComponentCache.Get<SetupGame>();

    private static GameObject gameObject;
    private static LEV_Inspector inspector;
    private static LEV_GizmoHandler gizmoHandler;

    /// <summary>
    /// An event that is fired when the user enters test mode
    /// </summary>
    public static event EnteredTestModeDelegate EnteredTestMode;

    /// <summary>
    /// An event that is fired when the user enters the level editor
    /// </summary>
    public static event EnteredLevelEditorDelegate EnteredLevelEditor;

    /// <summary>
    /// An event that is fired when the user leaves the level editor
    /// </summary>
    public static event ExitedLevelEditorDelegate ExitedLevelEditor;

    /// <summary>
    /// Boolean indicating whether or not the mouse input is currently being blocked
    /// </summary>
    public static bool IsMouseInputBlocked => mouseInputBlockers.Count > 0;

    /// <summary>
    /// Boolean indicating whether or not the keyboard input is currently being blocked
    /// </summary>
    public static bool IsKeyboardInputBlocked => keyboardInputBlockers.Count > 0;

    internal static void Initialize(GameObject gameObject)
    {
        LevelEditorApi.gameObject = gameObject;

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name != "GameScene")
                return;

            if (SetupGame.GlobalLevel.IsTestLevel)
            {
                EnteredTestMode?.Invoke();
            }
        };

        LEV_Inspector_Awake.Awake += inspector =>
        {
            if (LevelEditorApi.inspector != inspector)
            {
                LevelEditorApi.inspector = inspector;
                AddScheduledCustomFolders();
            }

            EnteredLevelEditor?.Invoke();
        };
        
        LEV_LevelEditorCentral_OnDestroy.PostfixEvent += () =>
        {
            ExitedLevelEditor?.Invoke();
        };
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
        if (keyboardInputBlockers.Contains(blocker))
            return;
        keyboardInputBlockers.Add(blocker);
    }

    /// <summary>
    /// Method that can be used to unblock keyboard input that has been blocked with <see cref="BlockKeyboardInput"/>
    /// </summary>
    /// <param name="blocker">The blocker to use for identification</param>
    public static void UnblockKeyboardInput(object blocker)
    {
        keyboardInputBlockers.Remove(blocker);
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
        if (gizmoHandler == null)
        {
            gizmoHandler = Object.FindObjectOfType<LEV_GizmoHandler>(true);
        }

        if (gizmoHandler == null)
        {
            logger.LogWarning("Unable to create new block because there's no GizmoHandler available");
            return null;
        }

        gizmoHandler.CreateNewBlock(blockId);

        BlockProperties createdBlock = gizmoHandler.central.selection.list.Last();

        if (position.HasValue)
            createdBlock.transform.position = position.Value;

        if (rotation.HasValue)
            createdBlock.transform.rotation = rotation.Value;

        if (scale.HasValue)
            createdBlock.transform.localScale = scale.Value;

        return createdBlock;
    }

    /// <summary>
    /// Adds the specified block to the selection in the level editor
    /// </summary>
    /// <param name="blockProperties"></param>
    public static void AddToSelection(BlockProperties blockProperties)
    {
        inspector.central.selection.AddThisBlock(blockProperties);
    }

    /// <summary>
    /// Removes the specified block from the selection in the level editor
    /// </summary>
    /// <param name="blockProperties"></param>
    public static void RemoveFromSelection(BlockProperties blockProperties)
    {
        int index = inspector.central.selection.list.IndexOf(blockProperties);
        if (index == -1)
            return;

        inspector.central.selection.RemoveBlockAt(index, false, false);
    }

    /// <summary>
    /// Clears the selection in the level editor
    /// </summary>
    public static void ClearSelection()
    {
        inspector.central.selection.ClickNothing();
    }

    /// <summary>
    /// Adds a custom folder to the block gui
    /// </summary>
    /// <param name="builder">A callback that is used to create/customize the folder</param>
    public static void AddCustomFolder(Action<ICustomFolderBuilder> builder)
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

    private static void AddScheduledCustomFolders()
    {
        foreach (CustomFolderBuilder scheduledCustomFolderBuilder in scheduledCustomFolderBuilders)
        {
            AddCustomFolderBuilder(scheduledCustomFolderBuilder);
        }
    }

    private static void AddCustomFolderBuilder(CustomFolderBuilder customFolderBuilder)
    {
        BlocksFolder blocksFolder = customFolderBuilder.Build();
        blocksFolder.hasParent = true;
        blocksFolder.parent = inspector.globalBlockList.globalBlocksFolder;
        inspector.globalBlockList.globalBlocksFolder.folders.Add(blocksFolder);
    }
}
