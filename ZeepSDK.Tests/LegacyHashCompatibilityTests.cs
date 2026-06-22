using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;
using ZeepSDK.Level;

namespace ZeepSDK.Tests;

public class LegacyHashCompatibilityTests
{
    private static readonly string FixtureDirectory =
        Path.Combine(AppContext.BaseDirectory, "testdata", "legacy-hash");

    public static IEnumerable<object[]> Vectors()
    {
        string manifestPath = Path.Combine(FixtureDirectory, "vectors.csv");
        return File.ReadAllLines(manifestPath)
            .Skip(1)
            .Select(line =>
            {
                string[] values = line.Split(',');
                return new object[] { values[0], values[1], values[2] };
            });
    }

    [Theory]
    [MemberData(nameof(Vectors))]
    public void CsvHashMatchesGoldenVector(string fileName, string expectedSha1, string expectedSha256)
    {
        string path = Path.Combine(FixtureDirectory, fileName);
        byte[] bytes = File.ReadAllBytes(path);

        string actualSha256;
        using (SHA256 sha256 = SHA256.Create())
        {
            actualSha256 = string.Concat(sha256.ComputeHash(bytes).Select(value => value.ToString("X2")));
        }

        CsvZeepLevel level = CsvZeepLevelParser.Parse(File.ReadAllLines(path));

        Assert.NotNull(level);
        Assert.Equal(expectedSha256, actualSha256);
        Assert.Equal(expectedSha1, level.CalculateHash());
    }
}
