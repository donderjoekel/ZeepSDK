using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using BugsnagUnity;
using BugsnagUnity.Payload;
using JetBrains.Annotations;
using Steamworks;
using UnityEngine;
using ZeepSDK.BugReporting.Patches;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.External.Cysharp.Threading.Tasks.Triggers;
using ZeepSDK.UI;
using ZeepSDK.Utilities;

namespace ZeepSDK.Crashlytics;

/// <summary>
/// An API for dealing with crashlytics
/// </summary>
[PublicAPI]
public static class CrashlyticsApi
{
    private const string ExplicitConsentKey = "ZeepSDK.HasExplicitCrashlyticsConsent";
    private const string NoticeSeenKey = "ZeepSDK.HasSeenCrashlyticsNotice";
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(CrashlyticsApi));
    private static CrashlyticsConsentDrawer _consentDrawer;
    private static bool _canInitialize;
    private static bool _initialized;
    
    internal static void Initialize()
    {
        Shutdown();
        _initialized = true;
        OpenUIOnStart_Start.Postfixed += OnPostfix;
        Plugin.Instance.ConsentToCrashlytics.SettingChanged += OnConsentSettingChanged;
    }

    internal static void Shutdown()
    {
        OpenUIOnStart_Start.Postfixed -= OnPostfix;

        if (Plugin.Instance?.ConsentToCrashlytics != null)
            Plugin.Instance.ConsentToCrashlytics.SettingChanged -= OnConsentSettingChanged;

        _initialized = false;
        _canInitialize = false;
        if (_consentDrawer != null)
        {
            UIApi.RemoveZeepGUIDrawer(_consentDrawer);
            _consentDrawer.Dispose();
            _consentDrawer = null;
        }

        if (Bugsnag.IsStarted())
            Bugsnag.PauseSession();
    }

    /// <summary>
    /// Allows you to leave an extra breadcrumb in the crashlytics report
    /// </summary>
    /// <param name="message"></param>
    /// <param name="metadata"></param>
    public static void LeaveBreadcrumb(string message, Dictionary<string,object> metadata = null)
    {
        if (CanSend())
            Bugsnag.LeaveBreadcrumb(message, metadata, BreadcrumbType.Manual);
    }

    /// <summary>
    /// Allows you to notify crashlytics of an exception that happened
    /// </summary>
    /// <param name="exception"></param>
    public static void Notify(Exception exception)
    {
        if (CanSend())
            Bugsnag.Notify(exception);
    }

    private static void OnPostfix(OpenUIOnStart instance)
    {
        _canInitialize = true;

        if (PlayerPrefs.GetInt(ExplicitConsentKey, 0) != 1 && Plugin.Instance.ConsentToCrashlytics.Value)
            Plugin.Instance.ConsentToCrashlytics.Value = false;

        if (instance.hasMod && PlayerPrefs.GetInt(NoticeSeenKey, 0) == 0)
        {
            ShowCrashlyticsConsent(instance).Forget(exception =>
                _logger.LogError($"Failed to show crash reporting consent dialog: {exception}"));
        }

        if (HasConsent())
        {
            _logger.LogInfo("Initializing bugsnag");
            InitializeBugsnag();
        }
    }

    private static void OnConsentSettingChanged(object sender, EventArgs args)
    {
        if (Plugin.Instance.ConsentToCrashlytics.Value)
        {
            PlayerPrefs.SetInt(ExplicitConsentKey, 1);
            PlayerPrefs.Save();

            if (_canInitialize)
                InitializeBugsnag();
        }
        else if (Bugsnag.IsStarted())
        {
            Bugsnag.PauseSession();
        }
    }

    private static void ApplyConsentDecision(bool accepted)
    {
        PlayerPrefs.SetInt(NoticeSeenKey, 1);
        PlayerPrefs.SetInt(ExplicitConsentKey, accepted ? 1 : 0);
        PlayerPrefs.Save();

        bool settingChanged = Plugin.Instance.ConsentToCrashlytics.Value != accepted;
        Plugin.Instance.ConsentToCrashlytics.Value = accepted;

        if (accepted && !settingChanged && _canInitialize)
            InitializeBugsnag();
        else if (!accepted && Bugsnag.IsStarted())
            Bugsnag.PauseSession();
    }

    private static bool HasConsent()
    {
        return Plugin.Instance?.ConsentToCrashlytics?.Value == true &&
               PlayerPrefs.GetInt(ExplicitConsentKey, 0) == 1;
    }

    private static bool CanSend()
    {
        return HasConsent() && Bugsnag.IsStarted();
    }

    private static void InitializeBugsnag()
    {
        if (!HasConsent())
            return;

        if (Bugsnag.IsStarted())
        {
            Bugsnag.ResumeSession();
            return;
        }

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
                    x => x.Value.Metadata.GUID,
                    object (y) => $"{y.Value.Metadata.Name} {y.Value.Metadata.Version}");
                configuration.AddMetadata("Mods", modVersions);
            });

            Bugsnag.SetUser(SteamClient.SteamId.Value.ToString(), null, SteamClient.Name);
            _logger.LogInfo("Bugsnag initialized");
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to initialize bugsnag");
            _logger.LogFatal(e);
        }

    }

    private static bool AddOnSend(IEvent evt)
    {
        if (!HasConsent())
            return false;

        HashSet<Assembly> assemblies = [];

        try
        {
            foreach (IError error in evt.Errors)
            {
                foreach (IStackframe frame in error.Stacktrace)
                {
                    string method = frame.Method;
                    if (string.IsNullOrEmpty(method))
                        continue;

                    int parameterListIndex = method.LastIndexOf('(');
                    string sub = parameterListIndex > 0 ? method[..parameterListIndex] : method;
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

    private static async UniTask ShowCrashlyticsConsent(OpenUIOnStart instance)
    {
        await instance.modPanel.GetAsyncDisableTrigger().OnDisableAsync();

        if (!_initialized || PlayerPrefs.GetInt(NoticeSeenKey, 0) != 0)
            return;

        if (_consentDrawer != null)
        {
            UIApi.RemoveZeepGUIDrawer(_consentDrawer);
            _consentDrawer.Dispose();
        }

        _consentDrawer = new CrashlyticsConsentDrawer(ApplyConsentDecision);
        UIApi.AddZeepGUIDrawer(_consentDrawer);
    }
}
