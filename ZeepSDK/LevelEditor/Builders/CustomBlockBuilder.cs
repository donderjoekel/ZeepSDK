using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.LevelEditor.Builders;

internal class CustomBlockBuilder : ICustomBlockBuilder
{
    private static int customBlockIdCounter = -100;

    private readonly int blockId;
    private string name;
    private Sprite thumbnail;
    private CustomBlockCallback callback;

    internal CustomBlockBuilder()
    {
        // We're doing negative IDs here because I noticed Zeepkist only checks for positive ones.
        blockId = customBlockIdCounter--;
    }

    public ICustomBlockBuilder WithName(string name)
    {
        this.name = name;
        return this;
    }

    public ICustomBlockBuilder WithThumbnail(Sprite thumbnail)
    {
        this.thumbnail = thumbnail;
        return this;
    }

    public ICustomBlockBuilder WithCallback(Action callback)
    {
        this.callback = new CustomBlockCallbackWithoutData(callback);
        return this;
    }

    public ICustomBlockBuilder WithCallback(Action<object> callback, object userData)
    {
        this.callback = new CustomBlockCallbackWithData(callback, userData);
        return this;
    }

    internal BlockProperties Build(GameObject gameObject)
    {
        BlockProperties blockProperties = gameObject.AddComponent<BlockProperties>();
        blockProperties.name = name;
        blockProperties.thumbnail = thumbnail;
        blockProperties.blockID = blockId;

        CustomBlockCallbackRegistry.Register(blockId, callback);

        return blockProperties;
    }
}
