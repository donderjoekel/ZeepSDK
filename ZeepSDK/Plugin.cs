using BepInEx;
using HarmonyLib;
using UnityEngine.LowLevel;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.Leaderboard;
using ZeepSDK.Level;
using ZeepSDK.LevelEditor;
using ZeepSDK.Multiplayer;
using ZeepSDK.PhotoMode;
using ZeepSDK.Racing;
using ZeepSDK.Storage;
using ZeepSDK.UI;
using ZeepSDK.Versioning;

namespace ZeepSDK
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static IModStorage Storage { get; private set; }

        private Harmony harmony;

        private void Awake()
        {
            Instance = this;

            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Storage = StorageApi.CreateModStorage(Instance);

            ChatApi.Initialize(gameObject);
            ChatCommandApi.Initialize(gameObject);
            LeaderboardApi.Initialize(gameObject);
            LevelApi.Initialize();
            LevelEditorApi.Initialize(gameObject);
            RacingApi.Initialize(gameObject);
            MultiplayerApi.Initialize();
            PhotoModeApi.Initialize();
            UIApi.Initialize(gameObject);

            // Initialize the player loop helper, this is to reduce issues with UniTask
            if (!PlayerLoopHelper.IsInjectedUniTaskPlayerLoop())
            {
                PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopHelper.Initialize(ref loop);
            }

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