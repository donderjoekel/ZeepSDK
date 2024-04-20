using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.LevelEditor;

/// <summary>
/// A builder that is used to define a custom folder
/// </summary>
[PublicAPI]
public interface ICustomFolderBuilder
{
    /// <summary>
    /// Allows you to add a folder to the current folder
    /// </summary>
    /// <param name="builder"><see cref="ICustomFolderBuilder"/></param>
    /// <returns>The builder</returns>
    ICustomFolderBuilder AddFolder(Action<ICustomFolderBuilder> builder);

    /// <summary>
    /// Sets the thumbnail of the folder
    /// </summary>
    /// <param name="thumbnail">The thumbnail you wish to use</param>
    /// <returns>The builder</returns>
    ICustomFolderBuilder WithThumbnail(Sprite thumbnail);

    /// <summary>
    /// Sets the name of the builder
    /// </summary>
    /// <param name="name">The name you wish to use</param>
    /// <returns>The builder</returns>
    ICustomFolderBuilder WithName(string name);

    /// <summary>
    /// Allows you to add a block to the current folder
    /// </summary>
    /// <param name="builder"><see cref="ICustomBlockBuilder"/></param>
    /// <returns>The builder</returns>
    ICustomFolderBuilder AddBlock(Action<ICustomBlockBuilder> builder);
}
