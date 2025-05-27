using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BugsnagUnity;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using UnityEngine.LowLevel;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepSDK.Controls;
using ZeepSDK.Crashlytics;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.Leaderboard;
using ZeepSDK.Level;
using ZeepSDK.LevelEditor;
using ZeepSDK.Multiplayer;
using ZeepSDK.PhotoMode;
using ZeepSDK.Racing;
using ZeepSDK.Scripting;
using ZeepSDK.Settings;
using ZeepSDK.Storage;
using ZeepSDK.UI;
using ZeepSDK.Versioning;

namespace ZeepSDK
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, "Zeep SDK", MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static IModStorage Storage { get; private set; }

        private Harmony harmony;
        
        public ConfigEntry<KeyCode> ToggleMenuBarKey { get; private set; }
        public ConfigEntry<bool> ConsentToCrashlytics { get; private set; }

        private void Awake()
        {
            Instance = this;

            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            ToggleMenuBarKey =
                Config.Bind("General", "Toggle Menu Bar Key", KeyCode.None, "The key to toggle the menu bar");
            ConsentToCrashlytics = Config.Bind("General", "Crashlytics Enabled", true,
                "Can ZeepSDK send crashlytics in order to help us fix bugs");

            Storage = StorageApi.CreateModStorage(Instance);

            // Initialize the player loop helper, this is to reduce issues with UniTask
            if (!PlayerLoopHelper.IsInjectedUniTaskPlayerLoop())
            {
                PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopHelper.Initialize(ref loop);
            }

            ChatApi.Initialize(gameObject);
            ChatCommandApi.Initialize(gameObject);
            CrashlyticsApi.Initialize(gameObject);
            LeaderboardApi.Initialize(gameObject);
            LevelApi.Initialize();
            LevelEditorApi.Initialize(gameObject);
            RacingApi.Initialize(gameObject);
            MultiplayerApi.Initialize();
            PhotoModeApi.Initialize();
            UIApi.Initialize(gameObject);
            ScriptingApi.Initialize();
            ControlsApi.Initialize();
            SettingsApi.Initialize();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            VersionChecker.CheckVersions().Forget();
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            harmony = null;
        }
    }
}