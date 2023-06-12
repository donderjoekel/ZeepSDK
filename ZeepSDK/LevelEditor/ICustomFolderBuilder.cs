using System;
using UnityEngine;
using ZeepSDK.LevelEditor.Builders;

namespace ZeepSDK.LevelEditor;

public interface ICustomFolderBuilder
{
    CustomFolderBuilder AddFolder(Action<ICustomFolderBuilder> builder);
    CustomFolderBuilder WithThumbnail(Sprite thumbnail);
    CustomFolderBuilder WithName(string name);
    CustomFolderBuilder AddBlock(Action<ICustomBlockBuilder> builder);
}
