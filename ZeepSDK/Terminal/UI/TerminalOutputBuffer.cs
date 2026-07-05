using System.Collections.Generic;

namespace ZeepSDK.Terminal.UI;

internal sealed class TerminalOutputBuffer
{
    private readonly List<CollectedTerminalLine> lines = new();
    private int capacity = 512;

    public IReadOnlyList<CollectedTerminalLine> Lines => lines;

    public void SetCapacity(int value) => capacity = value;

    public void Append(IEnumerable<CollectedTerminalLine> newLines)
    {
        foreach (CollectedTerminalLine line in newLines)
            Append(line);
    }

    public void Append(CollectedTerminalLine line)
    {
        lines.Add(line);
        Trim();
    }

    public void Clear() => lines.Clear();

    private void Trim()
    {
        while (lines.Count > capacity)
            lines.RemoveAt(0);
    }
}
