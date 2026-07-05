using System;
using System.Collections.Generic;
using System.Linq;
using ZeepSDK.Extensions;

namespace ZeepSDK.Terminal.BuiltIn;

internal sealed class HelpTerminalCommand : ITerminalCommand
{
    public string Name => "help";
    public string Description => "Shows all available terminal commands";
    public string Usage => "help [page]";

    public void Execute(TerminalCommandContext context)
    {
        List<ITerminalCommand> commands = TerminalRegistry.Commands
            .Where(command => command is not HelpTerminalCommand)
            .OrderBy(command => command.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (commands.Count == 0)
        {
            context.WriteLine("No terminal commands are registered.");
            return;
        }

        const int pageSize = 8;
        var chunks = commands.Chunk(pageSize).ToList();
        var page = 0;

        if (context.TryGetArgument(0, out string pageArgument) && int.TryParse(pageArgument, out int parsedPage))
            page = parsedPage - 1;

        if (page >= chunks.Count)
            page = chunks.Count - 1;
        else if (page < 0)
            page = 0;

        context.WriteLine("Available terminal commands:");
        foreach (ITerminalCommand command in chunks[page])
        {
            string usage = string.IsNullOrWhiteSpace(command.Usage) ? command.Name : command.Usage;
            context.WriteLine($"- {usage} - {command.Description}");
        }

        context.WriteLine($"Page {page + 1}/{chunks.Count}");
    }
}
