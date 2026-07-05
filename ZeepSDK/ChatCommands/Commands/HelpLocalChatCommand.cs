using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeepSDK.Chat;
using ZeepSDK.Extensions;

namespace ZeepSDK.ChatCommands.Commands;

internal class HelpLocalChatCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "help";
    public string Description => "Shows all locally available commands";

    public void Handle(string arguments)
    {
        List<IEnumerable<ILocalChatCommand>> chunks = ChatCommandRegistry.GetPrimaryLocalChatCommands()
            .Chunk(5)
            .ToList();

        ChatApi.AddLocalMessage("Available commands:");

        int page = 0;

        if (int.TryParse(arguments, out int parsedPage))
        {
            page = parsedPage - 1;
        }

        if (page >= chunks.Count)
        {
            page = chunks.Count - 1;
        }
        else if (page < 1)
        {
            page = 0;
        }

        IEnumerable<ILocalChatCommand> chunk = chunks[page];

        foreach (ILocalChatCommand localChatCommand in chunk)
        {
            ChatApi.AddLocalMessage(
                $"- {FormatCommandLine(localChatCommand)} - {localChatCommand.Description}");
        }

        ChatApi.AddLocalMessage($"Page {page + 1}/{chunks.Count}");
    }

    private static string FormatCommandLine(ILocalChatCommand command)
    {
        var line = new StringBuilder();
        line.Append(command.Prefix);
        line.Append(command.Command);

        List<ILocalChatCommandAlias> aliases = ChatCommandRegistry.GetAliasesFor(command).ToList();
        if (aliases.Count == 0)
            return line.ToString();

        line.Append(" (");
        for (var i = 0; i < aliases.Count; i++)
        {
            if (i > 0)
                line.Append(", ");

            line.Append(aliases[i].Prefix);
            line.Append(aliases[i].Command);
        }

        line.Append(')');
        return line.ToString();
    }
}
