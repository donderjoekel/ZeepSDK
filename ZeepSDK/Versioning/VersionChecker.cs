using System;
using System.Collections.Generic;
using System.IO;
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
    private const int MaximumResponseBytes = 2 * 1024 * 1024;
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(VersionChecker));
    private static readonly HttpClient httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

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
        if (result.Value?.Data == null)
            return Result.Fail<int>("mod.io returned no mod data");

        Dictionary<int, string> directoriesByModId = new();
        foreach (string directory in Directory.EnumerateDirectories(Paths.PluginPath, "*", SearchOption.TopDirectoryOnly))
        {
            string name = Path.GetFileName(directory);
            int separator = name.IndexOf('_');
            if (separator > 0 && int.TryParse(name[..separator], out int modId))
                directoriesByModId.TryAdd(modId, directory);
        }

        int outdatedMods = 0;

        foreach (ModResponseData modResponseData in result.Value.Data)
        {
            if (modResponseData?.ModFile == null ||
                !directoriesByModId.TryGetValue(modResponseData.Id, out string existingModPath))
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
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            string json = await BoundedHttpContent.ReadAsUtf8StringAsync(
                response.Content,
                MaximumResponseBytes);
            TData data = JsonConvert.DeserializeObject<TData>(json, new JsonSerializerSettings
            {
                MaxDepth = 64
            });

            return data == null
                ? Result.Fail<TData>("HTTP response contained no JSON value")
                : data;
        }
        catch (Exception e)
        {
            return Result.Fail<TData>(new ExceptionalError(e));
        }
    }
}
