using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using ZeepSDK.Chat;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.External.FluentResults;
using ZeepSDK.Messaging;
using ZeepSDK.Racing;
using ZeepSDK.Utilities;
using ZeepSDK.Versioning.Data;

namespace ZeepSDK.Versioning;

internal class VersionChecker
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(VersionChecker));

    public static async UniTaskVoid CheckVersions()
    {
        Result<int> result = await CalculateOutdatedMods();

        if (result.IsFailed)
        {
            logger.LogError("Failed to calculate outdated mods! " + result.ToString());
            return;
        }

        if (result.Value <= 0)
            return;

        int outdatedMods = result.Value;

        void LogOutdatedModsToChat()
        {
            const string workSprite = "<sprite=\"moremojis\" name=\"Work\">";
            ChatApi.AddLocalMessage(outdatedMods == 1
                ? $"{workSprite} <color=red>You have 1 outdated mod</color> {workSprite}"
                : $"{workSprite} <color=red>You have {outdatedMods} outdated mods</color> {workSprite}");
        }

        RacingApi.LevelLoaded += LogOutdatedModsToChat;

        await WaitForScene();

        MessengerApi.LogWarning(outdatedMods == 1
            ? "You have 1 outdated mod"
            : $"You have {outdatedMods} outdated mods");
    }

    private static async UniTask WaitForScene()
    {
        const string sceneName = "IntroScene";

        bool hasLoadedScene = false;

        if (!string.Equals(sceneName, SceneManager.GetActiveScene().name))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            while (!hasLoadedScene)
            {
                await UniTask.Yield();
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;

            await UniTask.Yield();
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }

        return;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == sceneName)
                hasLoadedScene = true;
        }
    }

    private static async UniTask<Result<int>> CalculateOutdatedMods()
    {
        Result<ModioResponse<ModResponseData>> result = await GetData<ModioResponse<ModResponseData>>(
            "https://g-3213.modapi.io/v1/games/3213/mods?api_key=188efbb7446e6d527b0991c3672b3e31&tags-in=Plugin");

        if (result.IsFailed)
            return result.ToResult();

        string[] directories = Directory.GetDirectories(Paths.PluginPath, "*", SearchOption.TopDirectoryOnly);

        int outdatedMods = 0;

        foreach (ModResponseData modResponseData in result.Value.Data)
        {
            string existingModPath = directories
                .FirstOrDefault(x => Path.GetFileNameWithoutExtension(x)!.StartsWith(modResponseData.Id + "_"));

            if (string.IsNullOrEmpty(existingModPath))
                continue;

            string directoryName = Path.GetFileNameWithoutExtension(existingModPath);
            string versionedModId = $"{modResponseData.Id}_{modResponseData.ModFile.Id}";

            if (!string.Equals(directoryName, versionedModId))
            {
                logger.LogInfo($"Mismatch found; Expected: {versionedModId} but got {directoryName}");
                outdatedMods++;
            }
        }

        return outdatedMods;
    }

    private static async UniTask<Result<TData>> GetData<TData>(string url)
    {
        HttpClient httpClient = new();

        HttpResponseMessage result = await httpClient.GetAsync(url);

        try
        {
            result.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }

        string json;

        try
        {
            json = await result.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }

        try
        {
            return JsonConvert.DeserializeObject<TData>(json);
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }
}
