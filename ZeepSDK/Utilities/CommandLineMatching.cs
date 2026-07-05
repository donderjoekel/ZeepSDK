using System.Text.RegularExpressions;

namespace ZeepSDK.Utilities;

internal static class CommandLineMatching
{
    public static bool MatchesCommand(string input, string prefix, string command)
    {
        if (string.IsNullOrEmpty(command))
            return false;

        string escapedPrefix = Regex.Escape(prefix ?? string.Empty);
        string escapedCommand = Regex.Escape(command);
        string pattern = $"^{escapedPrefix}{escapedCommand}(\\W|$).*";
        return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
    }

    public static string GetArguments(string input, string prefix, string command)
    {
        string args = input[(prefix ?? string.Empty).Length..];
        return args[command.Length..].Trim();
    }
}
