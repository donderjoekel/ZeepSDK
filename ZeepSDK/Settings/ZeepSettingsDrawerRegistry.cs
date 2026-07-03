using System.Collections.Generic;

namespace ZeepSDK.Settings;

/// <summary>
/// Stores custom settings drawer providers registered by mods.
/// </summary>
internal static class ZeepSettingsDrawerRegistry
{
    private static readonly Dictionary<string, ModSettingsDrawersDelegate> Providers = new();

    /// <summary>
    /// Registers a settings drawer provider for the given plugin GUID.
    /// </summary>
    public static void Register(string pluginGuid, ModSettingsDrawersDelegate provider)
        => Providers[pluginGuid] = provider;

    /// <summary>
    /// Attempts to get the settings drawer provider registered for the given plugin GUID.
    /// </summary>
    public static bool TryGetProvider(string pluginGuid, out ModSettingsDrawersDelegate provider)
        => Providers.TryGetValue(pluginGuid, out provider);
}
