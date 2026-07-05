using System.Collections.Generic;

namespace ZeepSDK.Terminal.Parsing;

internal sealed class ParsedCommandLine
{
    public string CommandName { get; }
    public IReadOnlyList<string> CommandPath { get; }
    public IReadOnlyList<string> Arguments { get; }
    public IReadOnlyDictionary<string, string> Flags { get; }

    public ParsedCommandLine(
        string commandName,
        IReadOnlyList<string> commandPath,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string> flags)
    {
        CommandName = commandName;
        CommandPath = commandPath;
        Arguments = arguments;
        Flags = flags;
    }
}
