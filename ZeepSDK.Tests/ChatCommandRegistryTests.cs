using System;
using System.Collections.Generic;
using Xunit;
using ZeepSDK.ChatCommands;

namespace ZeepSDK.Tests;

public class ChatCommandRegistryTests
{
    [Fact]
    public void ExposesStableReadOnlySnapshots()
    {
        TestRemoteCommand first = NewCommand();
        TestRemoteCommand second = NewCommand();

        try
        {
            ChatCommandRegistry.RegisterRemoteChatCommand(first);
            IReadOnlyList<IRemoteChatCommand> snapshot = ChatCommandRegistry.RemoteChatCommands;
            int originalCount = snapshot.Count;

            ChatCommandRegistry.RegisterRemoteChatCommand(second);

            Assert.Equal(originalCount, snapshot.Count);
            Assert.False(snapshot is List<IRemoteChatCommand>);
            Assert.Throws<NotSupportedException>(() =>
                ((IList<IRemoteChatCommand>)snapshot).Add(NewCommand()));
        }
        finally
        {
            ChatCommandRegistry.UnregisterRemoteChatCommand(first);
            ChatCommandRegistry.UnregisterRemoteChatCommand(second);
        }
    }

    [Fact]
    public void RejectsCaseInsensitiveDuplicateCommands()
    {
        string keyword = $"unique{Guid.NewGuid():N}";
        TestRemoteCommand first = new("!", keyword, "first");
        TestRemoteCommand duplicate = new("!", keyword.ToUpperInvariant(), "second");

        try
        {
            ChatCommandRegistry.RegisterRemoteChatCommand(first);

            Assert.Throws<InvalidOperationException>(() =>
                ChatCommandRegistry.RegisterRemoteChatCommand(duplicate));
        }
        finally
        {
            ChatCommandRegistry.UnregisterRemoteChatCommand(first);
            ChatCommandRegistry.UnregisterRemoteChatCommand(duplicate);
        }
    }

    [Theory]
    [InlineData("", "command")]
    [InlineData("!", "")]
    [InlineData("!", "two words")]
    public void RejectsInvalidCommandNames(string prefix, string keyword)
    {
        TestRemoteCommand command = new(prefix, keyword, "description");

        Assert.Throws<ArgumentException>(() =>
            ChatCommandRegistry.RegisterRemoteChatCommand(command));
    }

    [Fact]
    public void RejectsNullWrapperCallbacks()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RemoteChatCommandWrapper("!", "command", "description", null));
    }

    private static TestRemoteCommand NewCommand()
        => new("!", $"unique{Guid.NewGuid():N}", "description");

    private sealed class TestRemoteCommand : IRemoteChatCommand
    {
        public TestRemoteCommand(string prefix, string command, string description)
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
