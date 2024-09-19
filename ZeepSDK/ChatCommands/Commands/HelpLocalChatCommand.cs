using System.Collections.Generic;
using System.Linq;
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
        List<IEnumerable<ILocalChatCommand>> chunks = ChatCommandRegistry.LocalChatCommands.Chunk(5).ToList();

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
                $"- {localChatCommand.Prefix}{localChatCommand.Command} - {localChatCommand.Description}");
        }

        ChatApi.AddLocalMessage($"Page {page + 1}/{chunks.Count}");
    }
}
