using ZeepkistClient;
using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands;

internal class RemoteChatMessageHandler : MonoBehaviourWithLogging
{
    private void Start()
    {
        ZeepkistNetwork.ChatMessageReceived += OnChatMessageReceived;
    }

    private void OnDestroy()
    {
        ZeepkistNetwork.ChatMessageReceived -= OnChatMessageReceived;
    }

    private static void OnChatMessageReceived(ZeepkistChatMessage zeepkistChatMessage)
    {
        if (!ZeepkistNetwork.IsMasterClient)
            return;
        
        if (zeepkistChatMessage == null)
        {
            Logger.LogWarning("Received null chat message");
            return;
        }

        if (string.IsNullOrEmpty(zeepkistChatMessage.Message))
        {
            Logger.LogWarning("Received chat message with empty message");
            return;
        }

        if (zeepkistChatMessage.Player == null)
        {
            Logger.LogWarning("Received chat message with null player");
            return;
        }

        string message = zeepkistChatMessage.Message
            .Replace("<noparse>", string.Empty)
            .Replace("</noparse>", string.Empty)
            .Trim();

        if (zeepkistChatMessage.Player.IsLocal)
            return;

        foreach (IRemoteChatCommand remoteChatCommand in ChatCommandRegistry.RemoteChatCommands)
        {
            if (!message.StartsWith(remoteChatCommand.Prefix))
                continue;

            string messageWithoutPrefix = message[1..];

            if (!messageWithoutPrefix.StartsWith(remoteChatCommand.Command))
                continue;

            string arguments = messageWithoutPrefix[remoteChatCommand.Command.Length..].Trim();

            remoteChatCommand.Handle(zeepkistChatMessage.Player.SteamID, arguments);
        }
    }
}
