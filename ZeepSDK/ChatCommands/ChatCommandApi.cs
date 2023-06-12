using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.ChatCommands.Commands;

namespace ZeepSDK.ChatCommands;

[PublicAPI]
public static class ChatCommandApi
{
    internal static void Initialize(GameObject gameObject)
    {
        gameObject.AddComponent<RemoteChatMessageHandler>();

        RegisterLocalChatCommand<HelpLocalChatCommand>();
        RegisterLocalChatCommand<ClearChatLocalChatCommand>();

        RegisterRemoteChatCommand<HelpRemoteChatCommand>();
    }

    public static void RegisterLocalChatCommand(
        string prefix,
        string command,
        string description,
        LocalChatCommandCallbackDelegate callback
    )
    {
        ChatCommandRegistry.RegisterLocalChatCommand(new LocalChatCommandWrapper(prefix,
            command,
            description,
            callback));
    }

    public static void RegisterLocalChatCommand<TChatCommand>()
        where TChatCommand : ILocalChatCommand, new()
    {
        ChatCommandRegistry.RegisterLocalChatCommand(new TChatCommand());
    }

    public static void RegisterRemoteChatCommand(
        string prefix,
        string command,
        string description,
        RemoteChatCommandCallbackDelegate callback
    )
    {
        ChatCommandRegistry.RegisterRemoteChatCommand(new RemoteChatCommandWrapper(prefix,
            command,
            description,
            callback));
    }

    public static void RegisterRemoteChatCommand<TChatCommand>()
        where TChatCommand : IRemoteChatCommand, new()
    {
        ChatCommandRegistry.RegisterRemoteChatCommand(new TChatCommand());
    }
}
