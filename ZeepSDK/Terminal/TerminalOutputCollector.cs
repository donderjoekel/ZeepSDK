using System.Collections.Generic;

namespace ZeepSDK.Terminal;

internal enum TerminalOutputKind
{
    Input,
    Output,
    Error
}

internal readonly struct CollectedTerminalLine
{
    public TerminalOutputKind Kind { get; }
    public string Text { get; }

    public CollectedTerminalLine(TerminalOutputKind kind, string text)
    {
        Kind = kind;
        Text = text;
    }
}

internal sealed class TerminalOutputCollector
{
    private readonly List<CollectedTerminalLine> lines = new();

    public IReadOnlyList<CollectedTerminalLine> Lines => lines;

    public void WriteInput(string text) => lines.Add(new CollectedTerminalLine(TerminalOutputKind.Input, text));

    public void WriteLine(string text) => lines.Add(new CollectedTerminalLine(TerminalOutputKind.Output, text));

    public void WriteError(string text) => lines.Add(new CollectedTerminalLine(TerminalOutputKind.Error, text));
}
