using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using BugsnagUnity;
using Rewired;
using Steamworks;
using TMPro;
using UnityEngine;
using ZeepSDK.BugReporting.Patches;
using ZeepSDK.Chat;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.External.Cysharp.Threading.Tasks.Triggers;
using ZeepSDK.Utilities;
using Logger = UnityEngine.Logger;
using Object = UnityEngine.Object;

namespace ZeepSDK.Crashlytics;

internal static class CrashlyticsApi
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(CrashlyticsApi));
    
    public static void Initialize(GameObject gameObject)
    {
        OpenUIOnStart_Start.Postfixed += OnPostfix;
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
            Bugsnag.Start("", configuration =>
            {
                configuration.AppVersion = $"{v.version}.{v.patch}.{v.build}";

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
