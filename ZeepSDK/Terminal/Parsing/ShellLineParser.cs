using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeepSDK.Terminal.Parsing;

internal static class ShellLineParser
{
    public static bool TryParse(string input, IReadOnlyList<string> registeredCommandNames, out ParsedCommandLine parsed)
    {
        parsed = null;
        List<string> tokens = ShellLexer.Tokenize(input);
        if (tokens.Count == 0)
            return false;

        if (!TryFindLongestCommandMatch(tokens, registeredCommandNames, out string commandName, out int consumedTokens))
            return false;

        string[] commandPath = commandName.Split(' ');
        var arguments = new List<string>();
        var flags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = consumedTokens; i < tokens.Count; i++)
        {
            string token = tokens[i];
            if (token.StartsWith("--", StringComparison.Ordinal))
            {
                ParseLongFlag(token, flags);
                continue;
            }

            if (token.StartsWith("-", StringComparison.Ordinal) && token.Length > 1)
            {
                ParseShortFlag(token, flags);
                continue;
            }

            arguments.Add(token);
        }

        parsed = new ParsedCommandLine(commandName, commandPath, arguments, flags);
        return true;
    }

    internal static bool TryFindLongestCommandMatch(
        IReadOnlyList<string> tokens,
        IReadOnlyList<string> registeredCommandNames,
        out string commandName,
        out int consumedTokens)
    {
        commandName = null;
        consumedTokens = 0;
        var bestTokenCount = 0;

        foreach (string registeredName in registeredCommandNames)
        {
            if (string.IsNullOrWhiteSpace(registeredName))
                continue;

            string[] nameParts = registeredName.Split(' ');
            if (tokens.Count < nameParts.Length)
                continue;

            bool matches = true;
            for (var i = 0; i < nameParts.Length; i++)
            {
                if (!string.Equals(tokens[i], nameParts[i], StringComparison.OrdinalIgnoreCase))
                {
                    matches = false;
                    break;
                }
            }

            if (!matches || nameParts.Length < bestTokenCount)
                continue;

            bestTokenCount = nameParts.Length;
            commandName = registeredName;
            consumedTokens = nameParts.Length;
        }

        return commandName != null;
    }

    private static void ParseLongFlag(string token, Dictionary<string, string> flags)
    {
        string body = token[2..];
        int equalsIndex = body.IndexOf('=');
        if (equalsIndex < 0)
        {
            flags[body] = null;
            return;
        }

        string name = body[..equalsIndex];
        string value = body[(equalsIndex + 1)..];
        flags[name] = value;
    }

    private static void ParseShortFlag(string token, Dictionary<string, string> flags)
    {
        string body = token[1..];
        if (body.Length == 1)
        {
            flags[body] = null;
            return;
        }

        flags[body] = null;
    }
}
