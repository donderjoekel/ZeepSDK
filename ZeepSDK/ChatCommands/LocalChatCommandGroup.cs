using System.Collections.Generic;
using ZeepSDK.ChatCommands;

public class LocalChatCommandGroup : ILocalChatCommand
{
    private LocalChatCommandCallbackDelegate callback;

    public string Prefix { get; }
    public string Command { get; }
    public string Description { get; }
    public string[] Aliases { get; }
    public string[] Arguments { get; }

	public LocalChatCommandGroup(string prefix, string command, string description, LocalChatCommandCallbackDelegate defaultCallback = null, string[] aliases = null, string[] arguments = null)
	{
		Prefix = prefix;
		Command = command;
		Description = description;
		callback = defaultCallback;
		Aliases = aliases ?? [];
		Arguments = arguments ?? [];
		subCommands = new List<LocalChatCommandWrapper>();
	}

	public void registerSubcommand(string function, string description, LocalChatCommandCallbackDelegate handle = null, string[] subAliases = null, params string[] arguments)
	{
		LocalChatCommandWrapper subcommand = new LocalChatCommandWrapper("", function, description, handle, subAliases, arguments);
		subCommands.Add(subcommand);
	}

	public string helpString()
	{
		string commandsString = "";
		if (subCommands.Count > 0 || Aliases.Length > 0)
		{
			commandsString += $"<color=#FFDD00>{Prefix}{Command}";
			foreach (string alias in Aliases)
			{
				commandsString += $" | {Prefix}{alias}";
			}
			commandsString += "</color>";

			if (subCommands.Count == 0 && Arguments.Length == 0)
			{
				commandsString += $" <color=#FF8800>-- {Description}</color>";
			}

			foreach (LocalChatCommandWrapper subcommand in subCommands)
			{
				commandsString += $"\n{Prefix}{Command} {ChatCommandUtilities.GetHelpString(subcommand)}";
			}

			if (Arguments.Length > 0)
			{
				commandsString += $"\n{Prefix}{Command}";
				foreach (string arg in Arguments)
				{
					commandsString += $" [{arg}]";
				}
				commandsString += $" <color=#FF8800>-- {Description}</color>";
			}
		}
		else
		{
			commandsString += ChatCommandUtilities.GetHelpString(this);
		}
		return commandsString;
	}

	public void Handle(string arguments)
	{
		foreach (LocalChatCommandWrapper subcommand in subCommands)
		{
			if (ChatCommandUtilities.MatchesCommand(arguments, subcommand))
			{
				subcommand.Handle(ChatCommandUtilities.GetArguments(arguments, subcommand));
				return;
			}
		}
		callback?.Invoke(arguments);
	}

	private List<LocalChatCommandWrapper> subCommands;
}
