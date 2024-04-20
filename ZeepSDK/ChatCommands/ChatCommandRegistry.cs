using System.Collections.Generic;

namespace ZeepSDK.ChatCommands;

internal static class ChatCommandRegistry
{
    private static readonly List<ILocalChatCommand> _localChatCommands = [];
    private static readonly List<IRemoteChatCommand> _remoteChatCommands = [];

    public static IEnumerable<ILocalChatCommand> LocalChatCommands => _localChatCommands;
    public static IEnumerable<IRemoteChatCommand> RemoteChatCommands => _remoteChatCommands;

    public static void RegisterLocalChatCommand(ILocalChatCommand chatCommand)
    {
        _localChatCommands.Add(chatCommand);
    }

    public static void RegisterRemoteChatCommand(IRemoteChatCommand chatCommand)
    {
        _remoteChatCommands.Add(chatCommand);
    }

    public static void RegisterMixedChatCommand(IMixedChatCommand chatCommand)
    {
        _localChatCommands.Add(chatCommand);
        _remoteChatCommands.Add(chatCommand);
    }
}
