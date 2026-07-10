using System;
using BepInEx.Logging;
using UnityEngine;
using ZeepkistClient;
using ZeepSDK.Chat;
using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands;

internal class RemoteChatMessageHandler : MonoBehaviourWithLogging
{
    private const int MaximumMessageLength = 1024;
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger<RemoteChatMessageHandler>();
    private readonly RemoteCommandRateLimiter rateLimiter = new();

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

            if (message.Length > MaximumMessageLength)
                return;

            message = message
                .Replace("<noparse>", string.Empty)
                .Replace("</noparse>", string.Empty)
                .Trim();

            if (ZeepkistNetwork.LocalPlayer.SteamID == playerId)
                return;

            foreach (IRemoteChatCommand remoteChatCommand in ChatCommandRegistry.RemoteChatCommands)
            {
                if (remoteChatCommand == null ||
                    !ChatCommandUtilities.MatchesCommand(message, remoteChatCommand))
                    continue;

                if (!rateLimiter.TryConsume(playerId, Time.realtimeSinceStartup))
                    return;

                ProcessRemoteChatCommand(remoteChatCommand, playerId, message);
                return;
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(OnChatMessageReceived)}: " + e);
        }
    }

    private static void ProcessRemoteChatCommand(IRemoteChatCommand remoteChatCommand, ulong playerId, string message)
    {
        string arguments = ChatCommandUtilities.GetArguments(message, remoteChatCommand);

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
