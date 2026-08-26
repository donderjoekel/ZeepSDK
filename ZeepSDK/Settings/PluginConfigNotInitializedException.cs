using System;
using JetBrains.Annotations;

namespace ZeepSDK.Settings;

/// <summary>
/// Thrown when a BepInEx plugin instance has no config.
/// </summary>
[PublicAPI]
public sealed class PluginConfigNotInitializedException : InvalidOperationException
{
    /// <summary>
    /// The BepInEx GUID of the plugin whose config has not been initialized.
    /// </summary>
    public string PluginGuid { get; }

    /// <summary>
    /// Creates a new exception for a plugin whose config has not been initialized.
    /// </summary>
    /// <param name="pluginGuid">The BepInEx GUID of the plugin.</param>
    public PluginConfigNotInitializedException(string pluginGuid)
        : base($"The config for plugin '{pluginGuid}' has not been initialized.")
    {
        PluginGuid = pluginGuid;
    }
}
