using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepSDK.Leaderboard;
using ZeepSDK.LevelEditor;
using ZeepSDK.Racing;

namespace ZeepSDK
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        public static ManualLogSource CreateLogger(string sourceName)
        {
            return BepInEx.Logging.Logger.CreateLogSource(Instance.Info.Metadata.Name + "." + sourceName);
        }

        private Harmony harmony;

        private void Awake()
        {
            Instance = this;

            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            ChatApi.Initialize(gameObject);
            ChatCommandApi.Initialize(gameObject);
            LeaderboardApi.Initialize(gameObject);
            LevelEditorApi.Initialize(gameObject);
            RacingApi.Initialize(gameObject);

            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            harmony = null;
        }
    }
}
