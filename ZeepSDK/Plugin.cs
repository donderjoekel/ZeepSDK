using BepInEx;
using HarmonyLib;
using UnityEngine.LowLevel;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.Leaderboard;
using ZeepSDK.LevelEditor;
using ZeepSDK.Multiplayer;
using ZeepSDK.PhotoMode;
using ZeepSDK.Racing;
using ZeepSDK.Versioning;

namespace ZeepSDK;

#pragma warning disable CA2243
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
#pragma warning restore CA2243
internal class Plugin : BaseUnityPlugin
{
    public static Plugin Instance
    {
        get;
        private set;
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
        MultiplayerApi.Initialize();
        PhotoModeApi.Initialize();

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
