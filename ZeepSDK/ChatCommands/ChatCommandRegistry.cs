using System.Collections.Generic;

namespace ZeepSDK.ChatCommands;

/// <summary>
/// Stores the registered local and remote chat commands.
/// Use <see cref="ChatCommandApi"/> to register or unregister commands.
/// </summary>
public static class ChatCommandRegistry
{
    private static readonly List<ILocalChatCommand> localChatCommands = new();
    private static readonly List<IRemoteChatCommand> remoteChatCommands = new();

    /// <summary>
    /// All registered local chat commands.
    /// </summary>
    public static IReadOnlyList<ILocalChatCommand> LocalChatCommands => localChatCommands;

    /// <summary>
    /// All registered remote chat commands.
    /// </summary>
    public static IReadOnlyList<IRemoteChatCommand> RemoteChatCommands => remoteChatCommands;

    /// <summary>
    /// Adds a local chat command to the registry.
    /// </summary>
    /// <param name="chatCommand">The command to register.</param>
    internal static void RegisterLocalChatCommand(ILocalChatCommand chatCommand)
    {
        localChatCommands.Add(chatCommand);
    }

    /// <summary>
    /// Adds a remote chat command to the registry.
    /// </summary>
    /// <param name="chatCommand">The command to register.</param>
    internal static void RegisterRemoteChatCommand(IRemoteChatCommand chatCommand)
    {
        remoteChatCommands.Add(chatCommand);
    }

    /// <summary>
    /// Adds a mixed chat command to both the local and remote registries.
    /// </summary>
    /// <param name="chatCommand">The command to register.</param>
    internal static void RegisterMixedChatCommand(IMixedChatCommand chatCommand)
    {
        localChatCommands.Add(chatCommand);
        remoteChatCommands.Add(chatCommand);
    }

    /// <summary>
    /// Removes a local chat command from the registry.
    /// </summary>
    /// <param name="chatCommand">The command to unregister.</param>
    internal static void UnregisterLocalChatCommand(ILocalChatCommand chatCommand)
    {
        localChatCommands.Remove(chatCommand);
    }

    /// <summary>
    /// Removes a remote chat command from the registry.
    /// </summary>
    /// <param name="chatCommand">The command to unregister.</param>
    internal static void UnregisterRemoteChatCommand(IRemoteChatCommand chatCommand)
    {
        remoteChatCommands.Remove(chatCommand);
    }

    /// <summary>
    /// Removes a mixed chat command from both the local and remote registries.
    /// </summary>
    /// <param name="chatCommand">The command to unregister.</param>
    internal static void UnregisterMixedChatCommand(IMixedChatCommand chatCommand)
    {
        localChatCommands.Remove(chatCommand);
        remoteChatCommands.Remove(chatCommand);
    }
}
