using System;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using ZeepkistClient;
using ZeepSDK.Extensions;
using ZeepSDK.External.Cysharp.Threading.Tasks;
using ZeepSDK.External.FluentResults;
using ZeepSDK.Racing.Patches;
using ZeepSDK.Utilities;

namespace ZeepSDK.Racing;

/// <summary>
/// 
/// </summary>
[PublicAPI]
public static class RacingApi
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(RacingApi));

    /// <summary>
    /// An event that is fired when the player crosses the finish line. The parameter is the time the player crossed the finish line
    /// </summary>
    public static event EventHandler<CrossedFinishLineEventArgs> CrossedFinishLine;

    /// <summary>
    /// An event that is fired whenever the player passes any checkpoint. The parameter is the time the player passed the checkpoint
    /// </summary>
    public static event EventHandler<PassedCheckpointEventArgs> PassedCheckpoint;

    /// <summary>
    /// An event that is fired whenever the player crashes. The parameter is the reason that the player crashed
    /// </summary>
    public static event EventHandler<CrashedEventArgs> Crashed;

    /// <summary>
    /// An event that is fired whenever the player enters first person mode
    /// </summary>
    public static event EventHandler EnteredFirstPerson;

    /// <summary>
    /// An event that is fired whenever the player enters third person mode
    /// </summary>
    public static event EventHandler EnteredThirdPerson;

    /// <summary>
    /// An event that is fired whenever the player spawns
    /// </summary>
    public static event EventHandler PlayerSpawned;

    /// <summary>
    /// An event that is fired whenever the round starts
    /// </summary>
    public static event EventHandler RoundStarted;

    /// <summary>
    /// An event that is fired whenever the round ends
    /// </summary>
    public static event EventHandler RoundEnded;

    /// <summary>
    /// An even that is fired whenever a wheel breaks
    /// </summary>
    public static event EventHandler WheelBroken;

    /// <summary>
    /// An event that is fired when the level you are about to play has been loaded
    /// </summary>
    public static event EventHandler LevelLoaded;

    internal static void Initialize()
    {
        ReadyToReset_HeyYouHitATrigger.TriggerCheckpoint += time => PassedCheckpoint.InvokeSafe(time);
        ReadyToReset_HeyYouHitATrigger.TriggerFinish += time => CrossedFinishLine.InvokeSafe(time);
        SwitchCamera_GoToFirstPerson.EnteredFirstPerson += () => EnteredFirstPerson.InvokeSafe();
        SwitchCamera_GoToThirdPerson.EnteredThirdPerson += () => EnteredThirdPerson.InvokeSafe();
        DamageCharacterScript_KillCharacter.CharacterKilled += reason => Crashed.InvokeSafe(reason);
        GameMaster_SpawnPlayers.SpawnPlayers += () => PlayerSpawned.InvokeSafe();
        GameMaster_ReleaseTheZeepkists.Released += () => RoundStarted.InvokeSafe();
        DamageWheel_KillWheel.KillWheel += () => WheelBroken.InvokeSafe();
        GameMaster_StartLevelFirstTime.StartLevelFirstTime += () => LevelLoaded.InvokeSafe();
        ZeepkistNetwork.LobbyGameStateChanged += () =>
        {
            if (ZeepkistNetwork.CurrentLobby != null && ZeepkistNetwork.CurrentLobby.GameState == 1)
            {
                RoundEnded.InvokeSafe();
            }
        };
    }

    /// <summary>
    /// Attempts to load a track in free play mode
    /// </summary>
    /// <param name="uid">The UID of the track</param>
    /// <returns>Ok if all went well, Fail if level was not found</returns>
    public static async UniTask<Result> LoadTrackInFreePlayAsync(string uid)
    {
        try
        {
            if (!LevelManager.Instance.TryGetLevel(uid, out LevelScriptableObject level))
            {
                return Result.Fail("Level not found");
            }

            PlayerManager.Instance.amountOfPlayers = 1;
            PlayerManager.Instance.singlePlayer = true;
            PlayerManager.Instance.loader.GlobalLevel.Copy(level);

            AnimateWhitePanel.AnimateTheCircle(true, 0.75f, 0.0f, true);
            await UniTask.Delay(800);

            SceneManager.LoadScene("GameScene");
            await UniTask.Yield();

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(LoadTrackInFreePlayAsync)}: " + e);
            return Result.Fail(new ExceptionalError(e));
        }
    }
}
