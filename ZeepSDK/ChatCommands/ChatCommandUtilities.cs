using System.Text.RegularExpressions;

namespace ZeepSDK.ChatCommands;

internal static class ChatCommandUtilities
{
    public static bool MatchesCommand(string input, IChatCommand chatCommand)
    {
        if (chatCommand == null) return false;
        string escapedPrefix = Regex.Escape(chatCommand.Prefix);
        string escapedCommand = Regex.Escape(chatCommand.Command);
        string pattern = $"^{escapedPrefix}{escapedCommand}(\\W|$).*";
        return Regex.IsMatch(input, pattern);
    }

    public static string GetArguments(string input, IChatCommand chatCommand)
    {
        string args = input[chatCommand.Prefix.Length..];
        return args[chatCommand.Command.Length..].Trim();
    }
}
