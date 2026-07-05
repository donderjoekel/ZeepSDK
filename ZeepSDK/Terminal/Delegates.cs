using JetBrains.Annotations;

namespace ZeepSDK.Terminal;

/// <summary>
/// Callback invoked when a terminal command is executed.
/// </summary>
/// <param name="context">The execution context for the command.</param>
[PublicAPI]
public delegate void TerminalCommandCallbackDelegate(TerminalCommandContext context);
