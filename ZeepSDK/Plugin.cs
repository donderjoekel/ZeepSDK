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

    private Harmony _harmony;

    public void Awake()
    {
        Instance = this;

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll();

        ChatApi.Initialize();
        ChatCommandApi.Initialize(gameObject);
        LeaderboardApi.Initialize(gameObject);
        LevelEditorApi.Initialize(gameObject);
        RacingApi.Initialize();
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

    public void OnDestroy()
    {
        _harmony?.UnpatchSelf();
        _harmony = null;
    }
}
