using ZeepSDK.ChatCommands;
using Xunit;

namespace ZeepSDK.Tests;

public class ChatCommandUtilitiesTests
{
    private static readonly IChatCommand Command = new TestCommand("!", "kick");

    [Theory]
    [InlineData("!kick")]
    [InlineData("!KICK player")]
    [InlineData("!kick\tplayer")]
    [InlineData("!kick player")]
    public void MatchesExactCommandWithOptionalWhitespaceArguments(string input)
    {
        Assert.True(ChatCommandUtilities.MatchesCommand(input, Command));
    }

    [Theory]
    [InlineData("!kick.me")]
    [InlineData("!kick-player")]
    [InlineData("!kicker")]
    [InlineData("prefix !kick")]
    public void RejectsPunctuationAndPartialKeywordMatches(string input)
    {
        Assert.False(ChatCommandUtilities.MatchesCommand(input, Command));
    }

    [Fact]
    public void GetsArgumentsAfterValidatedCommand()
    {
        Assert.Equal("player reason", ChatCommandUtilities.GetArguments("!kick  player reason ", Command));
    }

    private sealed class TestCommand(string prefix, string command) : IChatCommand
    {
        public string Prefix { get; } = prefix;
        public string Command { get; } = command;
        public string Description => string.Empty;
    }
}
