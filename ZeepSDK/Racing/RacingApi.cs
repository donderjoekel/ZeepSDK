using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.Racing.Patches;

namespace ZeepSDK.Racing;

[PublicAPI]
public static class RacingApi
{
    private static readonly ManualLogSource logger = Plugin.CreateLogger(nameof(RacingApi));

    /// <summary>
    /// An event that is fired when the player crosses the finish line. The parameter is the time the player crossed the finish line
    /// </summary>
    public static event CrossedFinishLineDelegate CrossedFinishLine;

    /// <summary>
    /// An event that is fired whenever the player passes any checkpoint. The parameter is the time the player passed the checkpoint
    /// </summary>
    public static event PassedCheckpointDelegate PassedCheckpoint;

    /// <summary>
    /// An event that is fired whenever the player crashes. The parameter is the reason that the player crashed
    /// </summary>
    public static event CrashedDelegate Crashed;

    /// <summary>
    /// An event that is fired whenever the player enters first person mode
    /// </summary>
    public static event EnteredFirstPersonDelegate EnteredFirstPerson;

    /// <summary>
    /// An event that is fired whenever the player enters third person mode
    /// </summary>
    public static event EnteredThirdPersonDelegate EnteredThirdPerson;

    /// <summary>
    /// An event that is fired whenever the player spawns
    /// </summary>
    public static event PlayerSpawnedDelegate PlayerSpawned;

    /// <summary>
    /// An event that is fired whenever the round starts
    /// </summary>
    public static event RoundStartedDelegate RoundStarted;

    internal static void Initialize(GameObject gameObject)
    {
        ReadyToReset_HeyYouHitATrigger.TriggerCheckpoint += time => PassedCheckpoint?.Invoke(time);
        ReadyToReset_HeyYouHitATrigger.TriggerFinish += time => CrossedFinishLine?.Invoke(time);
        SwitchCamera_GoToFirstPerson.EnteredFirstPerson += () => EnteredFirstPerson?.Invoke();
        SwitchCamera_GoToThirdPerson.EnteredThirdPerson += () => EnteredThirdPerson?.Invoke();
        DamageCharacterScript_KillCharacter.CharacterKilled += reason => Crashed?.Invoke(reason);
        GameMaster_SpawnPlayers.SpawnPlayers += () => PlayerSpawned?.Invoke();
        GameMaster_ReleaseTheZeepkists.Released += () => RoundStarted?.Invoke();
    }
}
