using Xunit;
using ZeepSDK.Versioning;

namespace ZeepSDK.Tests;

public class ModioPaginationTests
{
    [Fact]
    public void AdvancesAcrossMultiplePages()
    {
        ModioResponse<int> response = Page(new[] { 1, 2 }, 0, 2, 3);

        bool valid = ModioPagination.TryGetNextOffset(
            response, 0, 10, out int nextOffset, out bool complete, out string error);

        Assert.True(valid, error);
        Assert.Equal(2, nextOffset);
        Assert.False(complete);
    }

    [Fact]
    public void CompletesOnLastPage()
    {
        ModioResponse<int> response = Page(new[] { 3 }, 2, 2, 3);

        bool valid = ModioPagination.TryGetNextOffset(
            response, 2, 10, out int nextOffset, out bool complete, out string error);

        Assert.True(valid, error);
        Assert.Equal(3, nextOffset);
        Assert.True(complete);
    }

    [Fact]
    public void RejectsRepeatedOffset()
    {
        ModioResponse<int> response = Page(new[] { 3 }, 0, 2, 4);

        bool valid = ModioPagination.TryGetNextOffset(
            response, 2, 10, out _, out _, out string error);

        Assert.False(valid);
        Assert.Contains("offset", error);
    }

    [Fact]
    public void RejectsItemLimitOverflow()
    {
        ModioResponse<int> response = Page(new[] { 1 }, 0, 1, 11);

        bool valid = ModioPagination.TryGetNextOffset(
            response, 0, 10, out _, out _, out string error);

        Assert.False(valid);
        Assert.Contains("limit", error);
    }

    [Fact]
    public void RejectsMismatchedResultCount()
    {
        ModioResponse<int> response = Page(new[] { 1 }, 0, 2, 2);
        response.ResultCount = 2;

        bool valid = ModioPagination.TryGetNextOffset(
            response, 0, 10, out _, out _, out string error);

        Assert.False(valid);
        Assert.Contains("result_count", error);
    }

    private static ModioResponse<int> Page(int[] data, int offset, int limit, int total)
    {
        return new ModioResponse<int>
        {
            Data = data,
            ResultCount = data.Length,
            ResultOffset = offset,
            ResultLimit = limit,
            ResultTotal = total
        };
    }
}
