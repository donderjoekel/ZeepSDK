using System.Collections.Generic;
using JetBrains.Annotations;

namespace ZeepSDK.Terminal;

/// <summary>
/// Context passed to <see cref="ITerminalCommand.GetCompletions"/> for autocomplete suggestions.
/// </summary>
[PublicAPI]
public sealed class TerminalCompletionContext
{
    /// <summary>
    /// The command path tokens matched so far.
    /// </summary>
    public IReadOnlyList<string> CommandPath { get; init; }

    /// <summary>
    /// The index of the positional argument being completed, or -1 when not completing an argument.
    /// </summary>
    public int ArgumentIndex { get; init; } = -1;

    /// <summary>
    /// The partial token currently being typed.
    /// </summary>
    public string CurrentToken { get; init; }

    /// <summary>
    /// Whether the cursor is on a flag token.
    /// </summary>
    public bool IsCompletingFlag { get; init; }
}
