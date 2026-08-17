using System;
using Xunit;
using ZeepSDK.Storage;

namespace ZeepSDK.Tests;

public class StorageResultTests
{
    [Fact]
    public void PreservesSuccessfulValue()
    {
        StorageResult<int> result = StorageResult<int>.FromValue(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void PreservesFailureDetails()
    {
        InvalidOperationException exception = new("failure");
        StorageResult<byte[]> result = StorageResult<byte[]>.FromValueFailure("Could not read.", exception);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Could not read.", result.Error);
        Assert.Same(exception, result.Exception);
    }
}
