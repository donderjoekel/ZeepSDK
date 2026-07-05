using System.Collections.Generic;

namespace ZeepSDK.Terminal.UI;

internal sealed class TerminalInputController
{
    private readonly List<string> history = new();
    private int historyIndex = -1;

    public void AddToHistory(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        string trimmed = input.Trim();
        if (history.Count > 0 && history[^1] == trimmed)
            return;

        history.Add(trimmed);
        historyIndex = history.Count;
    }

    public string NavigateHistory(int direction, string currentInput)
    {
        if (history.Count == 0)
            return currentInput;

        if (historyIndex < 0 || historyIndex >= history.Count)
            historyIndex = history.Count;

        historyIndex += direction;
        if (historyIndex < 0)
            historyIndex = 0;
        if (historyIndex >= history.Count)
        {
            historyIndex = history.Count;
            return string.Empty;
        }

        return history[historyIndex];
    }

    public void ResetHistoryNavigation() => historyIndex = history.Count;
}
