namespace ZeepSDK.Terminal.BuiltIn;

internal sealed class ClearTerminalCommand : ITerminalCommand
{
    public string Name => "clear";
    public string Description => "Clears the terminal output";
    public string Usage => "clear";

    public void Execute(TerminalCommandContext context)
    {
        TerminalApi.RequestClearOutput();
        context.WriteLine("Terminal cleared.");
    }
}
