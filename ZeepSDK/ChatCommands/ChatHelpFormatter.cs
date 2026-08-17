using System;
using System.Collections.Generic;
using System.Text;

namespace ZeepSDK.ChatCommands;

internal static class ChatHelpFormatter
{
    public const int MaximumMessageLength = 900;
    public const int MaximumAdvertisedCommands = 32;
    private const string TruncatedSuffix = "\n- ... more commands omitted";

    public static string Format(IReadOnlyList<IRemoteChatCommand> commands)
    {
        if (commands == null)
            throw new ArgumentNullException(nameof(commands));

        StringBuilder message = new("Available commands:");
        bool truncated = commands.Count > MaximumAdvertisedCommands;
        int count = Math.Min(commands.Count, MaximumAdvertisedCommands);

        for (int index = 0; index < count; index++)
        {
            IRemoteChatCommand command = commands[index];
            if (command == null)
                continue;

            string line = $"\n- {Normalize(command.Prefix)}{Normalize(command.Command)} - {Normalize(command.Description)}";
            int reservedLength = truncated ? TruncatedSuffix.Length : 0;
            if (message.Length + line.Length + reservedLength > MaximumMessageLength)
            {
                truncated = true;
                break;
            }

            message.Append(line);
        }

        if (truncated)
        {
            int available = MaximumMessageLength - TruncatedSuffix.Length;
            if (message.Length > available)
                message.Length = available;
            message.Append(TruncatedSuffix);
        }

        return message.ToString();
    }

    private static string Normalize(string value)
    {
        return (value ?? string.Empty)
            .Replace('\r', ' ')
            .Replace('\n', ' ')
            .Replace('\t', ' ');
    }
}
