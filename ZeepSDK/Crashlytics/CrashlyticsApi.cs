using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using BugsnagUnity;
using BugsnagUnity.Payload;
using JetBrains.Annotations;
using Rewired;
using Steamworks;
using TMPro;
using UnityEngine;
using ZeepSDK.BugReporting.Patches;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.External.Cysharp.Threading.Tasks.Triggers;
using ZeepSDK.Utilities;
using Object = UnityEngine.Object;

namespace ZeepSDK.Crashlytics;

/// <summary>
/// An API for dealing with crashlytics
/// </summary>
[PublicAPI]
public static class CrashlyticsApi
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(CrashlyticsApi));
    
    internal static void Initialize(GameObject gameObject)
    {
        OpenUIOnStart_Start.Postfixed += OnPostfix;
    }

    /// <summary>
    /// Allows you to leave an extra breadcrumb in the crashlytics report
    /// </summary>
    /// <param name="message"></param>
    /// <param name="metadata"></param>
    public static void LeaveBreadcrumb(string message, Dictionary<string,object> metadata = null)
    {
        Bugsnag.LeaveBreadcrumb(message, metadata, BreadcrumbType.Manual);
    }

    /// <summary>
    /// Allows you to notify crashlytics of an exception that happened
    /// </summary>
    /// <param name="exception"></param>
    public static void Notify(Exception exception)
    {
        Bugsnag.Notify(exception);
    }

    private static void OnPostfix(OpenUIOnStart instance)
    {
        if (instance.hasMod && PlayerPrefs.GetInt("ZeepSDK.HasGivenConsent", 0) == 0)
        {
            WaitForDestroy(instance).Forget();    
        }

        if (Plugin.Instance.ConsentToCrashlytics.Value)
        {
            _logger.LogInfo("Initializing bugsnag");
            InitializeBugsnag();
        }
    }

    private static void InitializeBugsnag()
    {
        if (!SteamClient.IsValid || !SteamClient.IsLoggedOn)
        {
            _logger.LogWarning("Steam isn't valid");
            return;   
        }

        try
        {
            VersionScriptableObject v = SteamManager.Instance.version;
            MainThreadDispatchBehaviour.InitializeLoop();
            Bugsnag.Start(Secrets.Bugsnag, configuration =>
            {
                configuration.AppVersion = $"{v.version}.{v.patch}.{v.build}";
                configuration.AddOnSendError(AddOnSend);

                Dictionary<string, object> modVersions = Chainloader.PluginInfos.ToDictionary(
                    x => x.Value.Metadata.Name,
                    object (y) => y.Value.Metadata.Version.ToString());
                configuration.AddMetadata("Mods", modVersions);
            });

            _logger.LogInfo("Bugsnag initialized");
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to initialize bugsnag");
            _logger.LogFatal(e);
        }

        Bugsnag.SetUser(SteamClient.SteamId.Value.ToString(), null, SteamClient.Name);
    }

    private static bool AddOnSend(IEvent evt)
    {
        HashSet<Assembly> assemblies = [];

        try
        {
            foreach (IError error in evt.Errors)
            {
                foreach (IStackframe frame in error.Stacktrace)
                {
                    string sub = frame.Method[..frame.Method.LastIndexOf('(')];
                    int index;
                    while ((index = sub.LastIndexOf('.')) != -1)
                    {
                        sub = sub[..index];
                        Type foundType = Type.GetType(sub);
                        if (foundType == null) continue;
                        assemblies.Add(foundType.Assembly);
                        break;
                    }
                }
            }
        }
        finally
        {
            if (assemblies.Count > 0)
            {
                evt.AddMetadata("Assemblies",
                    assemblies.ToDictionary(x => x.GetName().Name, object (y) => y.GetName().Version.ToString()));
            }
        }

        return true;
    }

    private static async UniTaskVoid WaitForDestroy(OpenUIOnStart instance)
    {
        await instance.modPanel.GetAsyncDisableTrigger().OnDisableAsync();

        GameObject newModPanel = Object.Instantiate(instance.modPanel, instance.modPanel.transform.parent);
        instance.UI.gameObject.SetActive(false);
        newModPanel.gameObject.SetActive(true);

        Transform child = newModPanel.transform.Find("You Have Mods Installed");
        TextMeshProUGUI tmp = child.GetComponent<TextMeshProUGUI>();
        tmp.text = "Wait, hol'up!<br>" +
                   "<br>" +
                   "Because making mods is tough and it is hard to figure things out when they go wrong, ZeepSDK now comes with an automatic exception/crash information collector.<br>" +
                   "<br>" +
                   "If you do NOT want this then you can disable this from within ZeepSettings!<br>";

        await UniTask.NextFrame(PlayerLoopTiming.Update);
        await UniTask.WaitUntil(() => ReInput.controllers.GetAnyButtonDown());

        instance.UI.gameObject.SetActive(true);
        newModPanel.gameObject.SetActive(false);

        PlayerPrefs.SetInt("ZeepSDK.HasGivenConsent", 1);
        PlayerPrefs.Save();
    }
}
