using System;
using System.Collections.Generic;
using ZeepSDK.Terminal.Parsing;

namespace ZeepSDK.Terminal;

/// <summary>
/// Context passed to a terminal command when it is executed.
/// </summary>
[JetBrains.Annotations.PublicAPI]
public sealed class TerminalCommandContext
{
    private readonly Action<string> writeLine;
    private readonly Action<string> writeError;

    private readonly ParsedCommandLine parsed;

    internal TerminalCommandContext(
        ParsedCommandLine parsed,
        Action<string> writeLine,
        Action<string> writeError)
    {
        this.parsed = parsed;
        this.writeLine = writeLine;
        this.writeError = writeError;
    }

    /// <summary>
    /// The full matched command name.
    /// </summary>
    public string CommandName => parsed.CommandName;

    /// <summary>
    /// The matched command path split into segments.
    /// </summary>
    public IReadOnlyList<string> CommandPath => parsed.CommandPath;

    /// <summary>
    /// Positional arguments after the matched command name.
    /// </summary>
    public IReadOnlyList<string> Arguments => parsed.Arguments;

    /// <summary>
    /// Parsed flags from the command line.
    /// </summary>
    public IReadOnlyDictionary<string, string> Flags => parsed.Flags;

    /// <summary>
    /// Writes a line to the terminal output.
    /// </summary>
    public void WriteLine(string message) => writeLine(message);

    /// <summary>
    /// Writes an error line to the terminal output.
    /// </summary>
    public void WriteError(string message) => writeError(message);

    /// <summary>
    /// Tries to get a positional argument by index.
    /// </summary>
    public bool TryGetArgument(int index, out string value)
    {
        if (index < 0 || index >= parsed.Arguments.Count)
        {
            value = null;
            return false;
        }

        value = parsed.Arguments[index];
        return true;
    }

    /// <summary>
    /// Tries to get whether a boolean flag is present.
    /// </summary>
    public bool TryGetFlag(string name, out bool present)
    {
        present = parsed.Flags.ContainsKey(name);
        return present;
    }

    /// <summary>
    /// Tries to get a flag value.
    /// </summary>
    public bool TryGetFlag(string name, out string value)
    {
        if (!parsed.Flags.TryGetValue(name, out value))
            return false;

        return true;
    }
}
