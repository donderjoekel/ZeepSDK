using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeepSDK.LevelEditor.Builders;

internal class CustomFolderBuilder : ICustomFolderBuilder
{
    private readonly GameObject gameObject;
    private readonly List<CustomFolderBuilder> folders = [];
    private readonly List<CustomBlockBuilder> blocks = [];

    private Sprite thumbnail;
    private string name;

    internal CustomFolderBuilder(GameObject gameObject)
    {
        this.gameObject = new GameObject("CustomFolderBuilder");
        this.gameObject.transform.SetParent(gameObject.transform);
    }

    public ICustomFolderBuilder AddFolder(Action<ICustomFolderBuilder> builder)
    {
        CustomFolderBuilder customFolderBuilder = new(gameObject);
        builder.Invoke(customFolderBuilder);
        folders.Add(customFolderBuilder);
        return this;
    }

    public ICustomFolderBuilder WithThumbnail(Sprite thumbnail)
    {
        this.thumbnail = thumbnail;
        return this;
    }

    public ICustomFolderBuilder WithName(string name)
    {
        this.name = name;
        return this;
    }

    public ICustomFolderBuilder AddBlock(Action<ICustomBlockBuilder> builder)
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
        blocksFolder.folders = [];
        foreach (CustomFolderBuilder customFolderBuilder in folders)
        {
            BlocksFolder folder = customFolderBuilder.Build();
            folder.hasParent = true;
            folder.parent = blocksFolder;
            blocksFolder.folders.Add(folder);
        }

        blocksFolder.blocks = [];
        foreach (CustomBlockBuilder customBlockBuilder in blocks)
        {
            BlockProperties blockProperties = customBlockBuilder.Build(gameObject);
            blocksFolder.blocks.Add(blockProperties);
        }

        return blocksFolder;
    }
}
