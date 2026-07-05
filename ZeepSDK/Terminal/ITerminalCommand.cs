using System.Collections.Generic;
using JetBrains.Annotations;

namespace ZeepSDK.Terminal;

/// <summary>
/// The interface to use for terminal commands.
/// </summary>
[PublicAPI]
public interface ITerminalCommand
{
    /// <summary>
    /// The command name. Multi-word names are supported (e.g. "rtm start").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A short description of the command.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Optional usage hint (e.g. "teleport &lt;x&gt; &lt;y&gt; &lt;z&gt; [--force]").
    /// </summary>
    string Usage => null;

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="context">The execution context.</param>
    void Execute(TerminalCommandContext context);

    /// <summary>
    /// Returns autocomplete suggestions for the current completion context.
    /// </summary>
    /// <param name="context">The completion context.</param>
    IEnumerable<string> GetCompletions(TerminalCompletionContext context) => null;
}
