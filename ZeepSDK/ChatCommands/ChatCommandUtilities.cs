using System;

namespace ZeepSDK.ChatCommands;

internal static class ChatCommandUtilities
{
    public static bool MatchesCommand(string input, IChatCommand chatCommand)
    {
        string prefix = input[..chatCommand.Prefix.Length];

        if (!string.Equals(prefix, chatCommand.Prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        string command = input[chatCommand.Prefix.Length..];

        if (command.Length == chatCommand.Command.Length)
            return string.Equals(command, chatCommand.Command, StringComparison.OrdinalIgnoreCase);

        if (command.Length < chatCommand.Command.Length)
            return false;

        command = command[..chatCommand.Command.Length];
        return string.Equals(command, chatCommand.Command, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetArguments(string input, IChatCommand chatCommand)
    {
        string args = input[chatCommand.Prefix.Length..];
        return args[chatCommand.Command.Length..].Trim();
    }
}
