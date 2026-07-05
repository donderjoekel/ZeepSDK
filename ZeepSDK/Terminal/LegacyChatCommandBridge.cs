using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepSDK.Utilities;

namespace ZeepSDK.Terminal;

internal static class LegacyChatCommandBridge
{
    private static readonly ManualLogSource Logger = LoggerFactory.GetLogger(typeof(LegacyChatCommandBridge));

    public static bool TryExecute(string input, TerminalOutputCollector output)
    {
        List<ILocalChatCommand> matchingCommands = ChatCommandRegistry.LocalChatCommands
            .Where(command => CommandLineMatching.MatchesCommand(input, command.Prefix, command.Command))
            .OrderByDescending(command => command.Command.Length)
            .ToList();

        if (matchingCommands.Count == 0)
            return false;

        ILocalChatCommand command = matchingCommands[0];
        string arguments = CommandLineMatching.GetArguments(input, command.Prefix, command.Command);

        try
        {
            using (ChatOutputRedirectScope.Redirect(output.WriteLine))
            {
                command.Handle(arguments);
            }
        }
        catch (Exception exception)
        {
            Logger.LogError($"Unhandled exception in legacy chat command '{command.Prefix}{command.Command}': {exception}");
            output.WriteError($"Command failed: {exception.Message}");
        }

        return true;
    }
}

internal sealed class ChatOutputRedirectScope : IDisposable
{
    public static IDisposable Redirect(Action<string> writeLine)
    {
        ChatApi.LocalMessageRedirect = writeLine;
        return new ChatOutputRedirectScope();
    }

    public void Dispose()
    {
        ChatApi.LocalMessageRedirect = null;
    }
}
