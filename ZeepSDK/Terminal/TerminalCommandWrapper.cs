using System.Collections.Generic;

namespace ZeepSDK.Terminal;

internal sealed class TerminalCommandWrapper : ITerminalCommand
{
    private readonly TerminalCommandCallbackDelegate callback;

    public string Name { get; }
    public string Description { get; }
    public string Usage { get; }

    public TerminalCommandWrapper(
        string name,
        string description,
        string usage,
        TerminalCommandCallbackDelegate callback)
    {
        Name = name;
        Description = description;
        Usage = usage;
        this.callback = callback;
    }

    public void Execute(TerminalCommandContext context)
    {
        callback?.Invoke(context);
    }
}
