using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Bootstrap;
using BugsnagUnity;
using BugsnagUnity.Payload;
using HarmonyLib;

namespace ZeepSDK.Crashlytics;

internal static class HarmonyPatchMetadata
{
    private const int MaxOriginalsPerEvent = 3;

    private static readonly Regex DmdPattern = new(
        @"DMD<(?<type>[^:>]+)::(?<method>[^>(]+)",
        RegexOptions.Compiled);

    public static void Enrich(IEvent evt)
    {
        try
        {
            List<(string TypeName, string MethodName)> originals = CollectDmdOriginals(evt);
            if (originals.Count == 0)
                return;

            Dictionary<string, object> metadata = new();
            int index = 0;

            foreach ((string typeName, string methodName) in originals)
            {
                if (!TryAddPatchMetadata(metadata, index, typeName, methodName))
                    continue;

                index++;
                if (index >= MaxOriginalsPerEvent)
                    break;
            }

            if (metadata.Count > 0)
                evt.AddMetadata("Harmony", metadata);
        }
        catch
        {
            // Never break Bugsnag delivery because of enrichment failures.
        }
    }

    private static List<(string TypeName, string MethodName)> CollectDmdOriginals(IEvent evt)
    {
        List<(string TypeName, string MethodName)> originals = [];
        HashSet<string> seen = new(StringComparer.Ordinal);

        foreach (IError error in evt.Errors)
        {
            foreach (IStackframe frame in error.Stacktrace)
            {
                string method = frame.Method;
                if (string.IsNullOrEmpty(method) || !method.Contains("DMD<"))
                    continue;

                Match match = DmdPattern.Match(method);
                if (!match.Success)
                    continue;

                string typeName = match.Groups["type"].Value.Trim();
                string methodName = match.Groups["method"].Value.Trim();
                if (typeName.Length == 0 || methodName.Length == 0)
                    continue;

                string key = typeName + "::" + methodName;
                if (!seen.Add(key))
                    continue;

                originals.Add((typeName, methodName));
            }
        }

        return originals;
    }

    private static bool TryAddPatchMetadata(
        Dictionary<string, object> metadata,
        int index,
        string typeName,
        string methodName)
    {
        Type type = AccessTools.TypeByName(typeName);
        if (type == null)
            return false;

        MethodBase original = AccessTools.DeclaredMethod(type, methodName)
                              ?? AccessTools.Method(type, methodName);
        if (original == null)
            return false;

        Patches patches = Harmony.GetPatchInfo(original);
        if (patches == null)
            return false;

        string prefix = index.ToString();
        metadata[$"{prefix}.original"] = $"{type.FullName}.{original.Name}";
        metadata[$"{prefix}.owners"] = string.Join(", ", patches.Owners);

        AddPatchEntries(metadata, prefix, "prefix", patches.Prefixes);
        AddPatchEntries(metadata, prefix, "postfix", patches.Postfixes);
        AddPatchEntries(metadata, prefix, "transpiler", patches.Transpilers);
        AddPatchEntries(metadata, prefix, "finalizer", patches.Finalizers);

        return true;
    }

    private static void AddPatchEntries(
        Dictionary<string, object> metadata,
        string originalPrefix,
        string kind,
        IEnumerable<Patch> patches)
    {
        int i = 0;
        foreach (Patch patch in patches)
        {
            metadata[$"{originalPrefix}.{kind}:{i}"] = FormatPatch(patch);
            i++;
        }
    }

    private static string FormatPatch(Patch patch)
    {
        MethodInfo patchMethod = patch.PatchMethod;
        Assembly assembly = patchMethod?.DeclaringType?.Assembly;
        string assemblyName = assembly?.GetName().Name ?? "unknown";
        string patchMethodName = patchMethod != null
            ? $"{patchMethod.DeclaringType?.FullName}.{patchMethod.Name}"
            : "unknown";

        PluginInfo plugin = FindPlugin(assembly);
        string pluginPart = plugin != null
            ? $"{plugin.Metadata.Name} ({plugin.Metadata.GUID}) v{plugin.Metadata.Version}"
            : "no matching plugin";

        return $"{patch.owner} | {pluginPart} | {assemblyName} | {patchMethodName}";
    }

    private static PluginInfo FindPlugin(Assembly assembly)
    {
        if (assembly == null)
            return null;

        return Chainloader.PluginInfos.Values.FirstOrDefault(plugin =>
            plugin.Instance != null && plugin.Instance.GetType().Assembly == assembly);
    }
}
