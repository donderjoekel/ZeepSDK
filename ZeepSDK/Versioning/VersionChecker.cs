using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
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

    public static async UniTask CheckVersions(CancellationToken cancellationToken)
    {
        Result<int> result = await CalculateOutdatedMods(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return;

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

        try
        {
            await WaitForScene(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        MessengerApi.LogWarning(outdatedMods == 1
            ? "You have 1 outdated mod"
            : $"You have {outdatedMods} outdated mods");
    }

    private static async UniTask WaitForScene(CancellationToken cancellationToken)
    {
        const string sceneName = "IntroScene";

        if (!string.Equals(sceneName, SceneManager.GetActiveScene().name))
        {
            UniTaskCompletionSource sceneLoaded = new();

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name == sceneName)
                    sceneLoaded.TrySetResult();
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            using CancellationTokenRegistration registration = cancellationToken.Register(
                () => sceneLoaded.TrySetCanceled(cancellationToken));
            try
            {
                await sceneLoaded.Task;
                await UniTask.Yield(cancellationToken);
            }
            finally
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }
    }

    private static async UniTask<Result<int>> CalculateOutdatedMods(CancellationToken cancellationToken)
    {
        Result<ModioResponse<ModResponseData>> result = await GetData<ModioResponse<ModResponseData>>(
            "https://g-3213.modapi.io/v1/games/3213/mods?api_key=188efbb7446e6d527b0991c3672b3e31&tags-in=Plugin",
            cancellationToken);

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

    private static async UniTask<Result<TData>> GetData<TData>(string url, CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            string json = await BoundedHttpContent.ReadAsUtf8StringAsync(
                response.Content,
                MaximumResponseBytes,
                cancellationToken);
            TData data = JsonConvert.DeserializeObject<TData>(json, new JsonSerializerSettings
            {
                MaxDepth = 64
            });

            return data == null
                ? Result.Fail<TData>("HTTP response contained no JSON value")
                : data;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Fail<TData>("Version check was cancelled");
        }
        catch (Exception e)
        {
            return Result.Fail<TData>(new ExceptionalError(e));
        }
    }
}
