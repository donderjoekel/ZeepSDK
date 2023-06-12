using System;
using UnityEngine;
using ZeepSDK.LevelEditor.Builders;

namespace ZeepSDK.LevelEditor;

public interface ICustomBlockBuilder
{
    CustomBlockBuilder WithName(string name);
    CustomBlockBuilder WithThumbnail(Sprite thumbnail);
    CustomBlockBuilder WithCallback(Action callback);
    CustomBlockBuilder WithCallback(Action<object> callback, object userData);
}
