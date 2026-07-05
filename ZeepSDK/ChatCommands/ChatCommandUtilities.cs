using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands;

internal static class ChatCommandUtilities
{
    public static bool MatchesCommand(string input, IChatCommand chatCommand)
    {
        if (chatCommand == null)
            return false;

        return CommandLineMatching.MatchesCommand(input, chatCommand.Prefix, chatCommand.Command);
    }

    public static string GetArguments(string input, IChatCommand chatCommand)
    {
        return CommandLineMatching.GetArguments(input, chatCommand.Prefix, chatCommand.Command);
    }
}
