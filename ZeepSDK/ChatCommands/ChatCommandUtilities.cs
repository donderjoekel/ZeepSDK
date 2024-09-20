using System.Text.RegularExpressions;

namespace ZeepSDK.ChatCommands;

internal static class ChatCommandUtilities
{
    public static bool MatchesCommand(string input, IChatCommand chatCommand)
    {
        string escapedPrefix = Regex.Escape(chatCommand.Prefix);
        string escapedCommand = Regex.Escape(chatCommand.Command);
        string pattern = $"^{escapedPrefix}{escapedCommand}(\\W|$).*";
        if (Regex.IsMatch(input, pattern)) {
            return true;
        }
        else
        {
            for (int aliasIndex = 0; aliasIndex < chatCommand.Aliases.Length; ++aliasIndex)
            {
                escapedCommand = Regex.Escape(chatCommand.Aliases[aliasIndex]);
                pattern = $"^{escapedPrefix}{escapedCommand}(\\W|$).*";
                if (Regex.IsMatch(input, pattern)) return true;
            }
            return false;
        }
    }

    public static string GetArguments(string input, IChatCommand chatCommand)
    {
        string args = input[chatCommand.Prefix.Length..];
        for (int aliasIndex = 0; aliasIndex < chatCommand.Aliases.Length; ++aliasIndex)
        {
            if (args.Length >= chatCommand.Aliases[aliasIndex].Length && args[..chatCommand.Aliases[aliasIndex].Length] == chatCommand.Aliases[aliasIndex]) {
                return args[chatCommand.Aliases[aliasIndex].Length..].Trim();
            }
        }
        return args[chatCommand.Command.Length..].Trim();
    }


	public static string GetHelpString(IChatCommand chatCommand)
	{
		string commandsString = $"{chatCommand.Prefix}{chatCommand.Command}";

		foreach (string arg in chatCommand.Arguments)
		{
			commandsString += $" [{arg}]";
		}
		commandsString += $" <color=#FF8800>-- {chatCommand.Description}</color>";

		for (int aliasIdx = 0; aliasIdx < chatCommand.Aliases.Length; ++aliasIdx)
		{
			if (aliasIdx == 0)
			{
				commandsString += " <color=#407AFF>(aliases;";
			}

			commandsString += $" {chatCommand.Aliases[aliasIdx]}";

			if (aliasIdx == chatCommand.Aliases.Length - 1)
			{
				commandsString += ")</color>";
			}
			else
			{
				commandsString += ",";
			}
		}
		return commandsString;
	}
}
