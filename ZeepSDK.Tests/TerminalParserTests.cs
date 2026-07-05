using System.Collections.Generic;
using Xunit;
using ZeepSDK.Terminal.Parsing;

namespace ZeepSDK.Tests;

public class ShellLineParserTests
{
    [Fact]
    public void TryParse_SimpleCommandWithArgs()
    {
        bool parsed = ShellLineParser.TryParse(
            "teleport 1 2 3",
            new[] { "teleport" },
            out ParsedCommandLine result);

        Assert.True(parsed);
        Assert.Equal("teleport", result.CommandName);
        Assert.Equal(new[] { "1", "2", "3" }, result.Arguments);
    }

    [Fact]
    public void TryParse_SubcommandWithArgsAndFlag()
    {
        bool parsed = ShellLineParser.TryParse(
            "rtm start my-session --force",
            new[] { "rtm", "rtm start" },
            out ParsedCommandLine result);

        Assert.True(parsed);
        Assert.Equal("rtm start", result.CommandName);
        Assert.Equal(new[] { "my-session" }, result.Arguments);
        Assert.True(result.Flags.ContainsKey("force"));
    }

    [Fact]
    public void TryParse_DeepSubcommandUsesLongestMatch()
    {
        bool parsed = ShellLineParser.TryParse(
            "rtm start something else",
            new[] { "rtm start", "rtm start something" },
            out ParsedCommandLine result);

        Assert.True(parsed);
        Assert.Equal("rtm start something", result.CommandName);
        Assert.Equal(new[] { "else" }, result.Arguments);
    }

    [Fact]
    public void TryParse_QuotedArgument()
    {
        bool parsed = ShellLineParser.TryParse(
            "say \"hello world\"",
            new[] { "say" },
            out ParsedCommandLine result);

        Assert.True(parsed);
        Assert.Equal(new[] { "hello world" }, result.Arguments);
    }

    [Fact]
    public void TryParse_FlagWithValue()
    {
        bool parsed = ShellLineParser.TryParse(
            "teleport --x=1 --y=2",
            new[] { "teleport" },
            out ParsedCommandLine result);

        Assert.True(parsed);
        Assert.Equal("1", result.Flags["x"]);
        Assert.Equal("2", result.Flags["y"]);
    }

    [Fact]
    public void TryParse_UnknownCommandReturnsFalse()
    {
        bool parsed = ShellLineParser.TryParse(
            "unknown arg",
            new[] { "help" },
            out ParsedCommandLine result);

        Assert.False(parsed);
        Assert.Null(result);
    }
}

public class ShellLexerTests
{
    [Fact]
    public void Tokenize_SplitsQuotedStrings()
    {
        List<string> tokens = ShellLexer.Tokenize("rtm stop \"server restart\"");

        Assert.Equal(new[] { "rtm", "stop", "server restart" }, tokens);
    }
}
