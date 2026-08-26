using System;
using JetBrains.Annotations;

namespace ZeepSDK.Settings;

/// <summary>
/// Thrown when a BepInEx plugin has no instance.
/// </summary>
[PublicAPI]
public sealed class PluginInstanceNotInitializedException : InvalidOperationException
{
    /// <summary>
    /// The BepInEx GUID of the plugin that has no instance.
    /// </summary>
    public string PluginGuid { get; }

    /// <summary>
    /// Creates a new exception for a plugin that has no instance.
    /// </summary>
    /// <param name="pluginGuid">The BepInEx GUID of the plugin.</param>
    public PluginInstanceNotInitializedException(string pluginGuid)
        : base($"The plugin instance for '{pluginGuid}' has not been initialized.")
    {
        PluginGuid = pluginGuid;
    }
}
