using ZeepSDK.ChatCommands;
using Xunit;

namespace ZeepSDK.Tests;

public class RemoteCommandRateLimiterTests
{
    [Fact]
    public void AllowsBurstThenRejectsUntilRefill()
    {
        RemoteCommandRateLimiter limiter = new();

        Assert.True(limiter.TryConsume(1, 10));
        Assert.True(limiter.TryConsume(1, 10));
        Assert.True(limiter.TryConsume(1, 10));
        Assert.False(limiter.TryConsume(1, 10));
        Assert.False(limiter.TryConsume(1, 10.5));
        Assert.True(limiter.TryConsume(1, 11));
    }

    [Fact]
    public void TracksPlayersIndependently()
    {
        RemoteCommandRateLimiter limiter = new();

        for (int i = 0; i < 3; i++)
            Assert.True(limiter.TryConsume(1, 0));

        Assert.False(limiter.TryConsume(1, 0));
        Assert.True(limiter.TryConsume(2, 0));
    }

    [Fact]
    public void BackwardsClockDoesNotRefillTokens()
    {
        RemoteCommandRateLimiter limiter = new();

        for (int i = 0; i < 3; i++)
            Assert.True(limiter.TryConsume(1, 10));

        Assert.False(limiter.TryConsume(1, 5));
    }
}
