using JetBrains.Annotations;
#pragma warning disable CS1591

namespace ZeepSDK.Racing;

[PublicAPI]
public delegate void CrossedFinishLineDelegate(float time);

[PublicAPI]
public delegate void PassedCheckpointDelegate(float time);

[PublicAPI]
public delegate void CrashedDelegate(CrashReason reason);

[PublicAPI]
public delegate void EnteredFirstPersonDelegate();

[PublicAPI]
public delegate void EnteredThirdPersonDelegate();

[PublicAPI]
public delegate void PlayerSpawnedDelegate();

[PublicAPI]
public delegate void RoundStartedDelegate();
