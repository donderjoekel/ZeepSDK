using System.Collections.Generic;

namespace ZeepSDK.ChatCommands;

internal static class ChatCommandRegistry
{
    private static readonly List<ILocalChatCommand> localChatCommands = new();
    private static readonly List<IRemoteChatCommand> remoteChatCommands = new();

    public static IReadOnlyList<ILocalChatCommand> LocalChatCommands => localChatCommands;
    public static IReadOnlyList<IRemoteChatCommand> RemoteChatCommands => remoteChatCommands;

    public static void RegisterLocalChatCommand(ILocalChatCommand chatCommand)
    {
        localChatCommands.Add(chatCommand);
    }

    public static void RegisterRemoteChatCommand(IRemoteChatCommand chatCommand)
    {
        remoteChatCommands.Add(chatCommand);
    }

    public static void RegisterMixedChatCommand(IMixedChatCommand chatCommand)
    {
        localChatCommands.Add(chatCommand);
        remoteChatCommands.Add(chatCommand);
    }
}
