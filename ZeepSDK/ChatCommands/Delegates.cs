using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

/// <summary>
/// The callback definition for local chat commands
/// </summary>
[PublicAPI]
public delegate void LocalChatCommandCallback(string arguments);

/// <summary>
/// The callback definition for remote chat commands
/// </summary>
[PublicAPI]
public delegate void RemoteChatCommandCallback(ulong playerId, string arguments);

/// <summary>
/// The callback definition for mixed chat commands
/// </summary>
[PublicAPI]
public delegate void MixedChatCommandCallback(bool isLocal, ulong playerId, string arguments);
