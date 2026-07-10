using System;

namespace ZeepSDK.ChatCommands;

internal static class ChatCommandUtilities
{
    public static bool MatchesCommand(string input, IChatCommand chatCommand)
    {
        if (string.IsNullOrEmpty(input) || chatCommand == null ||
            string.IsNullOrEmpty(chatCommand.Prefix) || chatCommand.Command == null)
            return false;

        int commandLength = chatCommand.Prefix.Length + chatCommand.Command.Length;
        if (input.Length < commandLength ||
            string.Compare(input, 0, chatCommand.Prefix, 0, chatCommand.Prefix.Length,
                StringComparison.OrdinalIgnoreCase) != 0 ||
            string.Compare(input, chatCommand.Prefix.Length, chatCommand.Command, 0, chatCommand.Command.Length,
                StringComparison.OrdinalIgnoreCase) != 0)
            return false;

        return input.Length == commandLength || char.IsWhiteSpace(input[commandLength]);
    }

    public static string GetArguments(string input, IChatCommand chatCommand)
    {
        string args = input[chatCommand.Prefix.Length..];
        return args[chatCommand.Command.Length..].Trim();
    }
}
