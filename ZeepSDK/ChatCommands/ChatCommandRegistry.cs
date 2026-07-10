using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private static IReadOnlyList<ILocalChatCommand> localSnapshot = Array.Empty<ILocalChatCommand>();
    private static IReadOnlyList<IRemoteChatCommand> remoteSnapshot = Array.Empty<IRemoteChatCommand>();

    /// <summary>
    /// All registered local chat commands, including alias entries.
    /// </summary>
    public static IReadOnlyList<ILocalChatCommand> LocalChatCommands => localSnapshot;

    /// <summary>
    /// All registered remote chat commands.
    /// </summary>
    public static IReadOnlyList<IRemoteChatCommand> RemoteChatCommands => remoteSnapshot;

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
        => localSnapshot.Where(command => command is not ILocalChatCommandAlias);

    /// <summary>
    /// Returns all alias entries registered for the given primary command.
    /// </summary>
    /// <param name="command">The primary command to get aliases for.</param>
    public static IEnumerable<ILocalChatCommandAlias> GetAliasesFor(ILocalChatCommand command)
        => localSnapshot
            .OfType<ILocalChatCommandAlias>()
            .Where(alias => alias.Target == command);

    /// <summary>
    /// Adds a local chat command to the registry.
    /// </summary>
    /// <param name="chatCommand">The command to register.</param>
    internal static void RegisterLocalChatCommand(ILocalChatCommand chatCommand)
    {
        ValidateCommand(chatCommand);
        EnsureNoLocalConflict(chatCommand.Prefix, chatCommand.Command);
        localChatCommands.Add(chatCommand);
        RefreshSnapshots();
    }

    /// <summary>
    /// Adds a remote chat command to the registry.
    /// </summary>
    /// <param name="chatCommand">The command to register.</param>
    internal static void RegisterRemoteChatCommand(IRemoteChatCommand chatCommand)
    {
        ValidateCommand(chatCommand);
        EnsureNoRemoteConflict(chatCommand.Prefix, chatCommand.Command);
        remoteChatCommands.Add(chatCommand);
        RefreshSnapshots();
    }

    /// <summary>
    /// Adds a mixed chat command to both the local and remote registries.
    /// </summary>
    /// <param name="chatCommand">The command to register.</param>
    internal static void RegisterMixedChatCommand(IMixedChatCommand chatCommand)
    {
        ValidateCommand(chatCommand);
        EnsureNoLocalConflict(chatCommand.Prefix, chatCommand.Command);
        EnsureNoRemoteConflict(chatCommand.Prefix, chatCommand.Command);
        localChatCommands.Add(chatCommand);
        remoteChatCommands.Add(chatCommand);
        RefreshSnapshots();
    }

    /// <summary>
    /// Removes a local chat command from the registry.
    /// </summary>
    /// <param name="chatCommand">The command to unregister.</param>
    internal static void UnregisterLocalChatCommand(ILocalChatCommand chatCommand)
    {
        if (localChatCommands.Remove(chatCommand))
            RefreshSnapshots();
    }

    /// <summary>
    /// Removes a remote chat command from the registry.
    /// </summary>
    /// <param name="chatCommand">The command to unregister.</param>
    internal static void UnregisterRemoteChatCommand(IRemoteChatCommand chatCommand)
    {
        if (remoteChatCommands.Remove(chatCommand))
            RefreshSnapshots();
    }

    /// <summary>
    /// Removes a mixed chat command from both the local and remote registries.
    /// </summary>
    /// <param name="chatCommand">The command to unregister.</param>
    internal static void UnregisterMixedChatCommand(IMixedChatCommand chatCommand)
    {
        bool changed = localChatCommands.Remove(chatCommand);
        changed |= remoteChatCommands.Remove(chatCommand);
        if (changed)
            RefreshSnapshots();
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

        if (!IsValidKeyword(alias))
        {
            error = "Alias must be 1-64 non-whitespace characters.";
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
        RefreshSnapshots();
        error = null;
        return true;
    }

    internal static void UnregisterAliasesFor(ILocalChatCommand target)
    {
        bool changed = false;
        for (var i = localChatCommands.Count - 1; i >= 0; i--)
        {
            if (localChatCommands[i] is ILocalChatCommandAlias alias && alias.Target == target)
            {
                localChatCommands.RemoveAt(i);
                changed = true;
            }
        }

        if (changed)
            RefreshSnapshots();
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
                RefreshSnapshots();
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

    private static void ValidateCommand(IChatCommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));
        if (!IsValidPrefix(command.Prefix))
            throw new ArgumentException("Command prefix must be 1-16 non-whitespace characters.", nameof(command));
        if (!IsValidCommandText(command.Command))
            throw new ArgumentException(
                "Command text must be 1-64 characters, may contain internal spaces, and cannot contain edge whitespace or control characters.",
                nameof(command));
        if (command.Description == null)
            throw new ArgumentException("Command description cannot be null.", nameof(command));
        if (command.Description.Length > 512)
            throw new ArgumentException("Command description cannot exceed 512 characters.", nameof(command));
    }

    private static bool IsValidPrefix(string prefix)
        => !string.IsNullOrWhiteSpace(prefix) && prefix.Length <= 16 && !prefix.Any(char.IsWhiteSpace);

    private static bool IsValidKeyword(string keyword)
        => !string.IsNullOrWhiteSpace(keyword) && keyword.Length <= 64 && !keyword.Any(char.IsWhiteSpace);

    private static bool IsValidCommandText(string command)
    {
        if (string.IsNullOrWhiteSpace(command) || command.Length > 64 ||
            command[0] == ' ' || command[^1] == ' ')
        {
            return false;
        }

        return command.All(character => character == ' ' ||
            (!char.IsWhiteSpace(character) && !char.IsControl(character)));
    }

    private static void EnsureNoLocalConflict(string prefix, string keyword)
    {
        if (HasConflictingKeyword(prefix, keyword))
            throw new InvalidOperationException($"Local command '{prefix}{keyword}' is already registered.");
    }

    private static void EnsureNoRemoteConflict(string prefix, string keyword)
    {
        if (remoteChatCommands.Any(entry =>
                string.Equals(entry.Prefix, prefix, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(entry.Command, keyword, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Remote command '{prefix}{keyword}' is already registered.");
    }

    private static void RefreshSnapshots()
    {
        localSnapshot = new ReadOnlyCollection<ILocalChatCommand>(localChatCommands.ToArray());
        remoteSnapshot = new ReadOnlyCollection<IRemoteChatCommand>(remoteChatCommands.ToArray());
    }
}
