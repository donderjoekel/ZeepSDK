using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeepSDK.LevelEditor.Builders;

internal class CustomFolderBuilder : ICustomFolderBuilder
{
    private readonly GameObject _gameObject;
    private readonly List<CustomFolderBuilder> _folders = [];
    private readonly List<CustomBlockBuilder> _blocks = [];

    private Sprite _thumbnail;
    private string _name;

    internal CustomFolderBuilder(GameObject gameObject)
    {
        _gameObject = new GameObject("CustomFolderBuilder");
        _gameObject.transform.SetParent(gameObject.transform);
    }

    public ICustomFolderBuilder AddFolder(Action<ICustomFolderBuilder> builder)
    {
        CustomFolderBuilder customFolderBuilder = new(_gameObject);
        builder.Invoke(customFolderBuilder);
        _folders.Add(customFolderBuilder);
        return this;
    }

    public ICustomFolderBuilder WithThumbnail(Sprite thumbnail)
    {
        _thumbnail = thumbnail;
        return this;
    }

    public ICustomFolderBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ICustomFolderBuilder AddBlock(Action<ICustomBlockBuilder> builder)
    {
        CustomBlockBuilder customBlockBuilder = new();
        builder.Invoke(customBlockBuilder);
        _blocks.Add(customBlockBuilder);
        return this;
    }

    internal BlocksFolder Build()
    {
        BlocksFolder blocksFolder = _gameObject.AddComponent<BlocksFolder>();

        blocksFolder.name = _name;
        blocksFolder.folderThumb = _thumbnail;
        blocksFolder.folders = [];
        foreach (CustomFolderBuilder customFolderBuilder in _folders)
        {
            BlocksFolder folder = customFolderBuilder.Build();
            folder.hasParent = true;
            folder.parent = blocksFolder;
            blocksFolder.folders.Add(folder);
        }

        blocksFolder.blocks = [];
        foreach (CustomBlockBuilder customBlockBuilder in _blocks)
        {
            BlockProperties blockProperties = customBlockBuilder.Build(_gameObject);
            blocksFolder.blocks.Add(blockProperties);
        }

        return blocksFolder;
    }
}
