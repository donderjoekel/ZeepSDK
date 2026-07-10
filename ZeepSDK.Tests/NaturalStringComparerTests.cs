using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZeepSDK.Utilities;

namespace ZeepSDK.Tests;

public class NaturalStringComparerTests
{
    private static readonly NaturalStringComparer Comparer = NaturalStringComparer.Instance;

    [Fact]
    public void Compare_IntegerPrefixes_SortsNumerically()
    {
        var input = new[] { "10. Baz", "2. Bar", "1. Foo", "11. Qux" };
        var expected = new[] { "1. Foo", "2. Bar", "10. Baz", "11. Qux" };

        Assert.Equal(expected, Sort(input));
    }

    [Fact]
    public void Compare_DecimalStylePrefixes_SortsNumerically()
    {
        var input = new[] { "2.10 Delta", "2.1 Alpha", "2.3 Gamma", "2.2 Beta" };
        var expected = new[] { "2.1 Alpha", "2.2 Beta", "2.3 Gamma", "2.10 Delta" };

        Assert.Equal(expected, Sort(input));
    }

    [Fact]
    public void Compare_NonPrefixedStrings_SortsLexicographically()
    {
        var input = new[] { "Beta", "Alpha", "Charlie" };
        var expected = new[] { "Alpha", "Beta", "Charlie" };

        Assert.Equal(expected, Sort(input));
    }

    [Fact]
    public void Compare_MixedPrefixedAndPlainStrings_SortsNaturally()
    {
        var input = new[] { "10. Item", "Alpha", "2. Item", "Beta", "1. Item" };
        var expected = new[] { "1. Item", "2. Item", "10. Item", "Alpha", "Beta" };

        Assert.Equal(expected, Sort(input));
    }

    [Fact]
    public void Compare_NullAndEmpty_HandlesEdgeCases()
    {
        Assert.True(Comparer.Compare(null, "A") < 0);
        Assert.True(Comparer.Compare("A", null) > 0);
        Assert.Equal(0, Comparer.Compare(null, null));
        Assert.True(Comparer.Compare(string.Empty, "A") < 0);
        Assert.True(Comparer.Compare("A", string.Empty) > 0);
        Assert.Equal(0, Comparer.Compare(string.Empty, string.Empty));
        Assert.True(Comparer.Compare("A", "AB") < 0);
    }

    [Fact]
    public void Compare_EqualStrings_ReturnsZero()
    {
        Assert.Equal(0, Comparer.Compare("2.1 Alpha", "2.1 Alpha"));
    }

    [Fact]
    public void Compare_LeadingZeroPrefixes_TreatsDigitRunsNumerically()
    {
        var input = new[] { "01. Second", "1. First", "10. Third" };
        var expected = new[] { "1. First", "01. Second", "10. Third" };

        Assert.Equal(expected, Sort(input));
    }

    private static List<string> Sort(IEnumerable<string> values)
        => values.OrderBy(x => x, Comparer).ToList();
}
