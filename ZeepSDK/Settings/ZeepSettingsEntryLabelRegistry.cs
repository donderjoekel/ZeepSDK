using System.Collections.Generic;
using BepInEx.Configuration;

namespace ZeepSDK.Settings;

/// <summary>
/// Stores custom display labels for config entries, keyed by plugin GUID and config definition.
/// </summary>
internal static class ZeepSettingsEntryLabelRegistry
{
    private static readonly Dictionary<string, Dictionary<ConfigDefinition, string>> LabelsByPlugin = new();

    public static void SetLabel(string pluginGuid, ConfigDefinition definition, string label)
    {
        if (!LabelsByPlugin.TryGetValue(pluginGuid, out var labels))
            LabelsByPlugin[pluginGuid] = labels = new Dictionary<ConfigDefinition, string>();

        labels[definition] = label;
    }

    public static void ClearLabel(string pluginGuid, ConfigDefinition definition)
    {
        if (!LabelsByPlugin.TryGetValue(pluginGuid, out var labels))
            return;

        labels.Remove(definition);

        if (labels.Count == 0)
            LabelsByPlugin.Remove(pluginGuid);
    }

    public static IReadOnlyDictionary<ConfigDefinition, string> GetLabels(string pluginGuid)
    {
        return LabelsByPlugin.GetValueOrDefault(pluginGuid);
    }
}
