using System.Collections.Generic;
using Xunit;
using ZeepSDK.ChatCommands;

namespace ZeepSDK.Tests;

public class ChatHelpFormatterTests
{
    [Fact]
    public void FormatsCommandsIntoOneBoundedMessage()
    {
        List<IRemoteChatCommand> commands = new()
        {
            new TestCommand("!", "help", "Shows help"),
            new TestCommand("!", "ping", "Checks latency")
        };

        string result = ChatHelpFormatter.Format(commands);

        Assert.Contains("!help", result);
        Assert.Contains("!ping", result);
        Assert.True(result.Length <= ChatHelpFormatter.MaximumMessageLength);
    }

    [Fact]
    public void CapsAdvertisedCommandsAndNormalizesLineBreaks()
    {
        List<IRemoteChatCommand> commands = new();
        for (int index = 0; index < ChatHelpFormatter.MaximumAdvertisedCommands + 5; index++)
            commands.Add(new TestCommand("!", $"command{index}", "line\nbreak"));

        string result = ChatHelpFormatter.Format(commands);

        Assert.Contains("more commands omitted", result);
        Assert.DoesNotContain("line\nbreak", result);
        Assert.DoesNotContain($"command{ChatHelpFormatter.MaximumAdvertisedCommands}", result);
        Assert.True(result.Length <= ChatHelpFormatter.MaximumMessageLength);
    }

    private sealed class TestCommand : IRemoteChatCommand
    {
        public TestCommand(string prefix, string command, string description)
        {
            Prefix = prefix;
            Command = command;
            Description = description;
        }

        public string Prefix { get; }
        public string Command { get; }
        public string Description { get; }
        public void Handle(ulong playerId, string arguments) { }
    }
}
