using System;
using System.Collections.Generic;
using System.Linq;

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
    /// All registered local chat commands, including alias entries.
    /// </summary>
    public static IReadOnlyList<ILocalChatCommand> LocalChatCommands => localChatCommands;

    /// <summary>
    /// All registered remote chat commands.
    /// </summary>
    public static IReadOnlyList<IRemoteChatCommand> RemoteChatCommands => remoteChatCommands;

    /// <summary>
    /// Returns whether the given command is an alias entry.
    /// </summary>
    /// <param name="command">The command to check.</param>
    public static bool IsAlias(ILocalChatCommand command) => command is ILocalChatCommandAlias;

    /// <summary>
    /// Returns the primary command for the given entry, or the entry itself when it is not an alias.
    /// </summary>
    /// <param name="command">The command to resolve.</param>
    public static ILocalChatCommand ResolvePrimaryCommand(ILocalChatCommand command)
        => command is ILocalChatCommandAlias alias ? alias.Target : command;

    /// <summary>
    /// Returns all primary local chat commands, excluding alias entries.
    /// </summary>
    public static IEnumerable<ILocalChatCommand> GetPrimaryLocalChatCommands()
        => localChatCommands.Where(command => command is not ILocalChatCommandAlias);

    /// <summary>
    /// Returns all alias entries registered for the given primary command.
    /// </summary>
    /// <param name="command">The primary command to get aliases for.</param>
    public static IEnumerable<ILocalChatCommandAlias> GetAliasesFor(ILocalChatCommand command)
        => localChatCommands
            .OfType<ILocalChatCommandAlias>()
            .Where(alias => alias.Target == command);

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

    internal static bool TryFindPrimaryLocalCommand(string prefix, string command, out ILocalChatCommand target)
    {
        target = GetPrimaryLocalChatCommands()
            .FirstOrDefault(entry =>
                string.Equals(entry.Prefix, prefix, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(entry.Command, command, StringComparison.OrdinalIgnoreCase));

        return target != null;
    }

    internal static bool TryRegisterAlias(ILocalChatCommand target, string alias, out string error)
    {
        if (target == null)
        {
            error = "Target command cannot be null.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(alias))
        {
            error = "Alias cannot be null or whitespace.";
            return false;
        }

        if (target is ILocalChatCommandAlias)
        {
            error = "Cannot register an alias for another alias.";
            return false;
        }

        if (!localChatCommands.Contains(target))
        {
            error = "Target command is not registered.";
            return false;
        }

        if (string.Equals(target.Command, alias, StringComparison.OrdinalIgnoreCase))
        {
            error = $"Alias '{alias}' is the same as the primary command keyword.";
            return false;
        }

        if (HasConflictingKeyword(target.Prefix, alias))
        {
            error = $"Alias '{target.Prefix}{alias}' conflicts with an existing command or alias.";
            return false;
        }

        localChatCommands.Add(new LocalChatCommandAlias(target, alias));
        error = null;
        return true;
    }

    internal static void UnregisterAliasesFor(ILocalChatCommand target)
    {
        for (var i = localChatCommands.Count - 1; i >= 0; i--)
        {
            if (localChatCommands[i] is ILocalChatCommandAlias alias && alias.Target == target)
                localChatCommands.RemoveAt(i);
        }
    }

    internal static bool UnregisterAlias(ILocalChatCommand target, string alias)
    {
        for (var i = localChatCommands.Count - 1; i >= 0; i--)
        {
            if (localChatCommands[i] is ILocalChatCommandAlias entry &&
                entry.Target == target &&
                string.Equals(entry.Command, alias, StringComparison.OrdinalIgnoreCase))
            {
                localChatCommands.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    private static bool HasConflictingKeyword(string prefix, string keyword)
    {
        foreach (ILocalChatCommand entry in localChatCommands)
        {
            if (string.Equals(entry.Prefix, prefix, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(entry.Command, keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
