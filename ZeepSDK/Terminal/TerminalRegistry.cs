using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeepSDK.Terminal;

internal static class TerminalRegistry
{
    private static readonly List<ITerminalCommand> commands = new();
    private static readonly Dictionary<string, ITerminalCommand> commandsByName =
        new(StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<ITerminalCommand> Commands => commands;

    public static void Register(ITerminalCommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Command name cannot be null or whitespace.", nameof(command));

        if (commandsByName.ContainsKey(command.Name))
            throw new InvalidOperationException($"A terminal command named '{command.Name}' is already registered.");

        commands.Add(command);
        commandsByName.Add(command.Name, command);
    }

    public static void Unregister(ITerminalCommand command)
    {
        if (command == null)
            return;

        commands.Remove(command);
        commandsByName.Remove(command.Name);
    }

    public static bool TryFindCommand(IReadOnlyList<string> tokens, out ITerminalCommand command, out int consumedTokens)
    {
        command = null;
        consumedTokens = 0;

        IReadOnlyList<string> names = commands.Select(x => x.Name).ToList();
        if (!Parsing.ShellLineParser.TryFindLongestCommandMatch(tokens, names, out string commandName, out consumedTokens))
            return false;

        return commandsByName.TryGetValue(commandName, out command);
    }

    public static IEnumerable<ITerminalCommand> GetCommandsWithPrefix(string prefix)
    {
        if (prefix == null)
            prefix = string.Empty;

        return commands.Where(command =>
            command.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<string> GetNextTokens(string typedPrefix)
    {
        if (string.IsNullOrWhiteSpace(typedPrefix))
        {
            return commands
                .Select(command => command.Name.Split(' ')[0])
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        string trimmed = typedPrefix.TrimEnd();
        bool endsWithSpace = typedPrefix.EndsWith(" ", StringComparison.Ordinal);
        string[] typedParts = trimmed.Split(' ');

        if (!endsWithSpace && typedParts.Length > 0)
        {
            string partial = typedParts[^1];
            string parentPrefix = typedParts.Length > 1
                ? string.Join(" ", typedParts, 0, typedParts.Length - 1) + " "
                : string.Empty;

            return commands
                .Where(command => command.Name.StartsWith(parentPrefix, StringComparison.OrdinalIgnoreCase))
                .Select(command =>
                {
                    string remainder = command.Name[parentPrefix.Length..];
                    string[] remainderParts = remainder.Split(' ');
                    return remainderParts[0];
                })
                .Where(token => token.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        string requiredPrefix = trimmed.Length == 0 ? string.Empty : trimmed + " ";
        return commands
            .Where(command => command.Name.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase))
            .Select(command =>
            {
                string remainder = command.Name[requiredPrefix.Length..];
                return remainder.Split(' ')[0];
            })
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
