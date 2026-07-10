using System;

namespace ZeepSDK.Versioning;

internal static class ModioPagination
{
    public static bool TryGetNextOffset<TData>(
        ModioResponse<TData> response,
        int expectedOffset,
        int maximumItems,
        out int nextOffset,
        out bool complete,
        out string error)
    {
        nextOffset = expectedOffset;
        complete = false;
        error = null;

        if (response?.Data == null)
            return Fail("mod.io returned no page data", out error);
        if (expectedOffset < 0 || maximumItems <= 0)
            return Fail("Invalid pagination bounds", out error);
        if (response.ResultOffset != expectedOffset)
            return Fail("mod.io returned an unexpected or repeated offset", out error);
        if (response.ResultCount < 0 || response.ResultLimit < 0 || response.ResultTotal < 0)
            return Fail("mod.io returned negative pagination metadata", out error);
        if (response.ResultCount != response.Data.Length)
            return Fail("mod.io result_count did not match returned data", out error);
        if (response.ResultCount > 0 && response.ResultLimit <= 0)
            return Fail("mod.io returned data with no positive result_limit", out error);
        if (response.ResultCount > response.ResultLimit)
            return Fail("mod.io result_count exceeded result_limit", out error);
        if (response.ResultTotal > maximumItems)
            return Fail($"mod.io result_total exceeded the {maximumItems}-item limit", out error);

        if (response.ResultOffset > int.MaxValue - response.ResultCount)
            return Fail("mod.io pagination offset overflowed", out error);

        nextOffset = response.ResultOffset + response.ResultCount;
        if (nextOffset > response.ResultTotal || nextOffset > maximumItems)
            return Fail("mod.io pagination metadata exceeded declared bounds", out error);

        complete = response.ResultCount == 0 || nextOffset >= response.ResultTotal;
        if (!complete && nextOffset <= expectedOffset)
            return Fail("mod.io pagination made no progress", out error);

        return true;
    }

    private static bool Fail(string message, out string error)
    {
        error = message;
        return false;
    }
}
