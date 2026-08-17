using System;
using ZeepSDK.Communication;
using Xunit;

namespace ZeepSDK.Tests;

public class ComReceiverTests
{
    [Fact]
    public void ObjectEqualityUsesReceiverIdentifier()
    {
        Guid id = Guid.NewGuid();
        object left = new ComReceiver(id, "mod-a");
        object right = new ComReceiver(id, "mod-b");

        Assert.True(left.Equals(right));
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void DifferentIdentifiersAreNotEqual()
    {
        object left = new ComReceiver(Guid.NewGuid(), "mod");
        object right = new ComReceiver(Guid.NewGuid(), "mod");

        Assert.False(left.Equals(right));
    }
}
