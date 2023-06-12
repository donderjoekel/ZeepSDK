using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.Racing.Patches;

namespace ZeepSDK.Racing;

[PublicAPI]
public static class RacingApi
{
    private static readonly ManualLogSource logger = Plugin.CreateLogger(nameof(RacingApi));

    public static event CrossedFinishLineDelegate CrossedFinishLine;
    public static event PassedCheckpointDelegate PassedCheckpoint;
    public static event CrashedDelegate Crashed;
    public static event EnteredFirstPersonDelegate EnteredFirstPerson;
    public static event EnteredThirdPersonDelegate EnteredThirdPerson;
    public static event PlayerSpawnedDelegate PlayerSpawned;
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
