using System;
using System.Linq;
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

        string[] splits = message[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (splits.Length == 0)
            return; // This shouldn't really happen though

        string command = splits[0];

        if (!string.Equals(command, remoteChatCommand.Command, StringComparison.OrdinalIgnoreCase))
            return;

        string arguments = string.Join(' ', splits.Skip(1));

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
