using System;
using ZeepkistClient;
using ZeepSDK.Chat;
using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands;

internal class RemoteChatMessageHandler : MonoBehaviourWithLogging
{
    private void Start()
    {
        ChatApi.ChatMessageReceived += OnChatMessageReceived;
    }

    private void OnDestroy()
    {
        ChatApi.ChatMessageReceived -= OnChatMessageReceived;
    }

    private void OnChatMessageReceived(ulong playerId, string username, string message)
    {
        if (!ZeepkistNetwork.IsMasterClient)
            return;

        if (string.IsNullOrEmpty(message))
        {
            Logger.LogWarning("Received chat message with empty message");
            return;
        }

        message = message
            .Replace("<noparse>", string.Empty)
            .Replace("</noparse>", string.Empty)
            .Trim();

        if (ZeepkistNetwork.LocalPlayer.SteamID == playerId)
            return;

        foreach (IRemoteChatCommand remoteChatCommand in ChatCommandRegistry.RemoteChatCommands)
        {
            if (!message.StartsWith(remoteChatCommand.Prefix))
                continue;

            string messageWithoutPrefix = message[1..];

            if (!messageWithoutPrefix.StartsWith(remoteChatCommand.Command))
                continue;

            string arguments = messageWithoutPrefix[remoteChatCommand.Command.Length..].Trim();

            remoteChatCommand.Handle(playerId, arguments);
        }
    }
}
