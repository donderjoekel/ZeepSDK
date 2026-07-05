using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using ZeepSDK.Terminal.Parsing;
using ZeepSDK.Utilities;

namespace ZeepSDK.Terminal;

internal static class TerminalExecutor
{
    private static readonly ManualLogSource Logger = LoggerFactory.GetLogger(typeof(TerminalExecutor));

    public static bool TryExecute(string input, TerminalOutputCollector output)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        output.WriteInput($"> {input.Trim()}");

        if (TryExecuteNative(input, output))
            return true;

        if (TerminalApi.LegacyChatCommandsEnabled && LegacyChatCommandBridge.TryExecute(input, output))
            return true;

        output.WriteError($"Unknown command: {input.Trim()}");
        return false;
    }

    private static bool TryExecuteNative(string input, TerminalOutputCollector output)
    {
        IReadOnlyList<string> registeredNames = TerminalRegistry.Commands
            .Select(command => command.Name)
            .ToList();

        if (!ShellLineParser.TryParse(input, registeredNames, out ParsedCommandLine parsed))
            return false;

        if (!TerminalRegistry.Commands.Any(command =>
                string.Equals(command.Name, parsed.CommandName, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        ITerminalCommand command = TerminalRegistry.Commands.First(command =>
            string.Equals(command.Name, parsed.CommandName, StringComparison.OrdinalIgnoreCase));

        var context = new TerminalCommandContext(
            parsed,
            output.WriteLine,
            output.WriteError);

        try
        {
            command.Execute(context);
        }
        catch (Exception exception)
        {
            Logger.LogError($"Unhandled exception in terminal command '{command.Name}': {exception}");
            output.WriteError($"Command failed: {exception.Message}");
        }

        return true;
    }
}
