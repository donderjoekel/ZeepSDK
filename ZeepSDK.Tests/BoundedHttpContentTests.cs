using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ZeepSDK.Versioning;

namespace ZeepSDK.Tests;

public class BoundedHttpContentTests
{
    [Fact]
    public async Task ReadsContentAtLimit()
    {
        using ByteArrayContent content = new(Encoding.UTF8.GetBytes("12345"));

        string result = await BoundedHttpContent.ReadAsUtf8StringAsync(content, 5);

        Assert.Equal("12345", result);
    }

    [Fact]
    public async Task RejectsDeclaredOversizedContent()
    {
        using ByteArrayContent content = new(Encoding.UTF8.GetBytes("123456"));

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            BoundedHttpContent.ReadAsUtf8StringAsync(content, 5));
    }

    [Fact]
    public async Task RejectsChunkedOversizedContent()
    {
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes("123456"));
        using StreamContent content = new(stream);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            BoundedHttpContent.ReadAsUtf8StringAsync(content, 5));
    }

    [Fact]
    public async Task HonorsCancellationWhileReadingContent()
    {
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes("content"));
        using StreamContent content = new(stream);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            BoundedHttpContent.ReadAsUtf8StringAsync(content, 100, cancellation.Token));
    }
}
