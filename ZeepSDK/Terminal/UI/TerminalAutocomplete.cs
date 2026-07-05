using System;
using System.Collections.Generic;
using System.Linq;
using ZeepSDK.ChatCommands;
using ZeepSDK.Terminal.Parsing;

namespace ZeepSDK.Terminal.UI;

internal sealed class TerminalAutocomplete
{
    public IReadOnlyList<string> GetSuggestions(string input)
    {
        if (input == null)
            input = string.Empty;

        List<string> suggestions = new();
        suggestions.AddRange(GetNativeSuggestions(input));

        if (TerminalApi.LegacyChatCommandsEnabled)
            suggestions.AddRange(GetLegacySuggestions(input));

        return suggestions
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToList();
    }

    private static IEnumerable<string> GetNativeSuggestions(string input)
    {
        string trimmed = input.TrimStart();
        if (trimmed.StartsWith("/", StringComparison.Ordinal))
            yield break;

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            foreach (ITerminalCommand command in TerminalRegistry.Commands)
                yield return command.Name;

            yield break;
        }

        bool endsWithSpace = trimmed.EndsWith(" ", StringComparison.Ordinal);
        string[] parts = trimmed.Split(' ');

        if (!endsWithSpace)
        {
            string parentPrefix = parts.Length > 1
                ? string.Join(" ", parts, 0, parts.Length - 1) + " "
                : string.Empty;
            string partial = parts[^1];

            foreach (string nextToken in TerminalRegistry.GetNextTokens(trimmed))
            {
                string suggestion = string.IsNullOrEmpty(parentPrefix)
                    ? nextToken
                    : parentPrefix + nextToken;
                yield return suggestion;
            }

            foreach (ITerminalCommand command in TerminalRegistry.GetCommandsWithPrefix(parentPrefix + partial))
                yield return command.Name;

            yield break;
        }

        string requiredPrefix = trimmed;
        foreach (ITerminalCommand command in TerminalRegistry.GetCommandsWithPrefix(requiredPrefix))
            yield return command.Name;
    }

    private static IEnumerable<string> GetLegacySuggestions(string input)
    {
        string trimmed = input.TrimStart();
        if (!trimmed.StartsWith("/", StringComparison.Ordinal) && !string.IsNullOrEmpty(trimmed))
            yield break;

        foreach (ILocalChatCommand command in ChatCommandRegistry.LocalChatCommands)
        {
            string fullName = command.Prefix + command.Command;
            if (fullName.StartsWith(trimmed, StringComparison.OrdinalIgnoreCase))
                yield return fullName;
        }
    }

    public string GetTabCompletion(string input, IReadOnlyList<string> suggestions)
    {
        if (suggestions.Count == 0)
            return input;

        if (suggestions.Count == 1)
            return suggestions[0] + " ";

        string commonPrefix = suggestions[0];
        foreach (string suggestion in suggestions.Skip(1))
        {
            int length = Math.Min(commonPrefix.Length, suggestion.Length);
            var index = 0;
            while (index < length &&
                   char.ToLowerInvariant(commonPrefix[index]) == char.ToLowerInvariant(suggestion[index]))
            {
                index++;
            }

            commonPrefix = commonPrefix[..index];
            if (commonPrefix.Length == 0)
                return input;
        }

        if (commonPrefix.Length <= input.TrimStart().Length)
            return input;

        return commonPrefix;
    }
}
