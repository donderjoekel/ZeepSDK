namespace ZeepSDK.Chat;
#pragma warning disable CS1591

public delegate void ChatMessageReceivedDelegate(ulong playerId, string username, string message);

public delegate void ServerMessageReceivedDelegate(string message);