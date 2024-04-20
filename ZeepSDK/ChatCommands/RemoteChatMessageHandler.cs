using System;
using BepInEx.Logging;
using ZeepkistClient;
using ZeepSDK.Chat;
using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands;

internal class RemoteChatMessageHandler : MonoBehaviourWithLogging
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger<RemoteChatMessageHandler>();

    private void Start()
    {
        ChatApi.ChatMessageReceived += OnChatMessageReceived;
    }

    private void OnDestroy()
    {
        ChatApi.ChatMessageReceived -= OnChatMessageReceived;
    }

    private void OnChatMessageReceived(object sender, ChatMessageReceivedEventArgs args)
    {
        try
        {
            if (!ZeepkistNetwork.IsMasterClient)
            {
                return;
            }

            if (string.IsNullOrEmpty(args.Message))
            {
                Logger.LogWarning("Received chat message with empty message");
                return;
            }

            string message = args.Message
                .Replace("<noparse>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("</noparse>", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (ZeepkistNetwork.LocalPlayer.SteamID == args.PlayerId)
            {
                return;
            }

            foreach (IRemoteChatCommand remoteChatCommand in ChatCommandRegistry.RemoteChatCommands)
            {
                ProcessRemoteChatCommand(remoteChatCommand, args.PlayerId, message);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(OnChatMessageReceived)}: " + e);
        }
    }

    private static void ProcessRemoteChatCommand(IRemoteChatCommand remoteChatCommand, ulong playerId, string message)
    {
        if (remoteChatCommand == null)
        {
            return;
        }

        if (!ChatCommandUtilities.MatchesCommand(message, remoteChatCommand))
        {
            return;
        }

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
