using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

[PublicAPI]
public delegate void LocalChatCommandCallbackDelegate(string arguments);

[PublicAPI]
public delegate void RemoteChatCommandCallbackDelegate(ulong playerId, string arguments);
