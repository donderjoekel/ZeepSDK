using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.LevelEditor.Builders;

public class CustomFolderBuilder : ICustomFolderBuilder
{
    private readonly GameObject gameObject;
    private readonly List<CustomFolderBuilder> folders = new();
    private readonly List<CustomBlockBuilder> blocks = new();

    private Sprite thumbnail;
    private string name;

    internal CustomFolderBuilder(GameObject gameObject)
    {
        this.gameObject = new GameObject("CustomFolderBuilder");
        this.gameObject.transform.SetParent(gameObject.transform);
    }

    public CustomFolderBuilder AddFolder(Action<ICustomFolderBuilder> builder)
    {
        CustomFolderBuilder customFolderBuilder = new(gameObject);
        builder.Invoke(customFolderBuilder);
        folders.Add(customFolderBuilder);
        return this;
    }

    public CustomFolderBuilder WithThumbnail(Sprite thumbnail)
    {
        this.thumbnail = thumbnail;
        return this;
    }

    public CustomFolderBuilder WithName(string name)
    {
        this.name = name;
        return this;
    }

    public CustomFolderBuilder AddBlock(Action<ICustomBlockBuilder> builder)
    {
        CustomBlockBuilder customBlockBuilder = new();
        builder.Invoke(customBlockBuilder);
        blocks.Add(customBlockBuilder);
        return this;
    }

    internal BlocksFolder Build()
    {
        BlocksFolder blocksFolder = gameObject.AddComponent<BlocksFolder>();

        blocksFolder.name = name;
        blocksFolder.folderThumb = thumbnail;
        blocksFolder.folders = new List<BlocksFolder>();
        foreach (CustomFolderBuilder customFolderBuilder in folders)
        {
            BlocksFolder folder = customFolderBuilder.Build();
            folder.hasParent = true;
            folder.parent = blocksFolder;
            blocksFolder.folders.Add(folder);
        }

        blocksFolder.blocks = new List<BlockProperties>();
        foreach (CustomBlockBuilder customBlockBuilder in blocks)
        {
            BlockProperties blockProperties = customBlockBuilder.Build(gameObject);
            blocksFolder.blocks.Add(blockProperties);
        }

        return blocksFolder;
    }
}
