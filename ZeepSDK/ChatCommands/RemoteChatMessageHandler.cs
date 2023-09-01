using System;
using BepInEx.Logging;
using ZeepkistClient;
using ZeepSDK.Chat;
using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands;

internal class RemoteChatMessageHandler : MonoBehaviourWithLogging
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger<RemoteChatMessageHandler>();

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
        try
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
                ProcessRemoteChatCommand(remoteChatCommand, playerId, message);
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(OnChatMessageReceived)}: " + e);
        }
    }

    private static void ProcessRemoteChatCommand(IRemoteChatCommand remoteChatCommand, ulong playerId, string message)
    {
        if (remoteChatCommand == null)
            return;

        if (!message.StartsWith(remoteChatCommand.Prefix))
            return;

        string messageWithoutPrefix = message[1..];

        if (!messageWithoutPrefix.StartsWith(remoteChatCommand.Command))
            return;

        string arguments = messageWithoutPrefix[remoteChatCommand.Command.Length..].Trim();

        try
        {
            remoteChatCommand.Handle(playerId, arguments);
        }
        catch (Exception e)
        {
            ManualLogSource manualLogSource = LoggerFactory.GetLogger(remoteChatCommand);
            manualLogSource.LogError($"Unhandled exception in {remoteChatCommand.GetType().Name}: " + e);
        }
    }
}
