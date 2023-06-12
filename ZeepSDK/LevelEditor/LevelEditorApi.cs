using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepSDK.LevelEditor.Builders;
using ZeepSDK.LevelEditor.Patches;
using Object = UnityEngine.Object;

namespace ZeepSDK.LevelEditor;

[PublicAPI]
public static class LevelEditorApi
{
    private static readonly ManualLogSource logger = Plugin.CreateLogger(nameof(LevelEditorApi));
    private static readonly List<object> mouseInputBlockers = new();
    private static readonly List<object> keyboardInputBlockers = new();
    private static readonly List<CustomFolderBuilder> scheduledCustomFolderBuilders = new();

    private static GameObject gameObject;
    private static LEV_Inspector inspector;
    private static LEV_GizmoHandler gizmoHandler;

    public static event EnteredTestModeDelegate EnteredTestMode;
    public static event EnteredLevelEditorDelegate EnteredLevelEditor;

    public static bool IsMouseInputBlocked => mouseInputBlockers.Count > 0;
    public static bool IsKeyboardInputBlocked => keyboardInputBlockers.Count > 0;

    internal static void Initialize(GameObject gameObject)
    {
        LevelEditorApi.gameObject = gameObject;

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name != "GameScene")
                return;

            // Not particularly performant but I think this should be alright since this is a reaction to a scene change
            SetupGame setupGame = Object.FindObjectOfType<SetupGame>();
            if (setupGame.GlobalLevel.IsTestLevel)
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
    }

    public static void BlockMouseInput(object blocker)
    {
        if (mouseInputBlockers.Contains(blocker))
            return;
        mouseInputBlockers.Add(blocker);
    }

    public static void UnblockMouseInput(object blocker)
    {
        mouseInputBlockers.Remove(blocker);
    }

    public static void BlockKeyboardInput(object blocker)
    {
        if (keyboardInputBlockers.Contains(blocker))
            return;
        keyboardInputBlockers.Add(blocker);
    }

    public static void UnblockKeyboardInput(object blocker)
    {
        keyboardInputBlockers.Remove(blocker);
    }

    public static BlockProperties CreateNewBlock(
        BlockProperties blockProperties,
        Vector3? position = null,
        Quaternion? rotation = null,
        Vector3? scale = null
    )
    {
        return CreateNewBlock(blockProperties.blockID, position, rotation, scale);
    }

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

    public static void AddToSelection(BlockProperties blockProperties)
    {
        inspector.central.selection.AddThisBlock(blockProperties);
    }
    
    public static void RemoveFromSelection(BlockProperties blockProperties)
    {
        int index = inspector.central.selection.list.IndexOf(blockProperties);
        if (index == -1)
            return;

        inspector.central.selection.RemoveBlockAt(index, false, false);
    }
    
    public static void ClearSelection()
    {
        inspector.central.selection.ClickNothing();
    }

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
