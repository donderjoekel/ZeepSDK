using System;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.LevelEditor.Builders;

namespace ZeepSDK.LevelEditor;

/// <summary>
/// A builder that is used to define a custom block
/// </summary>
[PublicAPI]
public interface ICustomBlockBuilder
{
    /// <summary>
    /// Sets the name of the block
    /// </summary>
    /// <param name="name">The name you wish to use</param>
    /// <returns>The builder</returns>
    ICustomBlockBuilder WithName(string name);

    /// <summary>
    /// Sets the thumbnail of the block
    /// </summary>
    /// <param name="thumbnail">The thumbnail you wish to use</param>
    /// <returns>The builder</returns>
    ICustomBlockBuilder WithThumbnail(Sprite thumbnail);

    /// <summary>
    /// Sets the callback that gets invoked whenever the user presses on the block
    /// </summary>
    /// <param name="callback">The callback you wish to use</param>
    /// <returns>The builder</returns>
    ICustomBlockBuilder WithCallback(Action callback);

    /// <summary>
    /// Sets the callback that gets invoked whenever the user presses on the block, with the ability to pass in user data
    /// </summary>
    /// <param name="callback">The callback you wish to use</param>
    /// <param name="userData">The data you wish to pass along</param>
    /// <returns>The builder</returns>
    ICustomBlockBuilder WithCallback(Action<object> callback, object userData);
}
