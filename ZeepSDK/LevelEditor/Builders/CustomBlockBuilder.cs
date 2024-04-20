using System;
using UnityEngine;

namespace ZeepSDK.LevelEditor.Builders;

internal class CustomBlockBuilder : ICustomBlockBuilder
{
    private static int _customBlockIdCounter = -100;

    private readonly int _blockId;
    private string _name;
    private Sprite _thumbnail;
    private CustomBlockCallback _callback;

    internal CustomBlockBuilder()
    {
        // We're doing negative IDs here because I noticed Zeepkist only checks for positive ones.
        _blockId = _customBlockIdCounter--;
    }

    public ICustomBlockBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ICustomBlockBuilder WithThumbnail(Sprite thumbnail)
    {
        _thumbnail = thumbnail;
        return this;
    }

    public ICustomBlockBuilder WithCallback(Action callback)
    {
        _callback = new CustomBlockCallbackWithoutData(callback);
        return this;
    }

    public ICustomBlockBuilder WithCallback(Action<object> callback, object userData)
    {
        _callback = new CustomBlockCallbackWithData(callback, userData);
        return this;
    }

    internal BlockProperties Build(GameObject gameObject)
    {
        BlockProperties blockProperties = gameObject.AddComponent<BlockProperties>();
        blockProperties.name = _name;
        blockProperties.thumbnail = _thumbnail;
        blockProperties.blockID = _blockId;

        CustomBlockCallbackRegistry.Register(_blockId, _callback);

        return blockProperties;
    }
}
