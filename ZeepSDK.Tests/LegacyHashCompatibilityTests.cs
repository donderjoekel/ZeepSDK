using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;
using ZeepSDK.Level;

namespace ZeepSDK.Tests;

/**
 * LegacyHashCompatibilityTests is an unbreakable contract between ZeepSDK and
 * the ZeepCentraal API for legacy SHA1/zeepHash and XXH128 hash generation.
 *
 * NEVER modify recorded hashes in `vectors.csv`.
 *
 * If a test fails to generate the expected hash, it is a breaking change and
 * must be corrected before merging. Breaking this contract will result in
 * ZeepSDK/GTR being unable to associate record submissions to the correct level
 * in ZeepCentraal.
 */

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
                return new object[] { values[0], values[1], values[2], values[3], values[4] };
            });
    }

    [Theory]
    [MemberData(nameof(Vectors))]
    public void HashesMatchGoldenVector(
        string fileName,
        string format,
        string expectedZeepHash,
        string expectedSha256,
        string expectedXxh128)
    {
        string path = Path.Combine(FixtureDirectory, fileName);
        byte[] bytes = File.ReadAllBytes(path);
        string content = File.ReadAllText(path);

        string actualSha256;
        using (SHA256 sha256 = SHA256.Create())
        {
            actualSha256 = string.Concat(sha256.ComputeHash(bytes).Select(value => value.ToString("X2")));
        }

        Assert.Equal(expectedSha256, actualSha256);

        if (format == "csv")
        {
            CsvZeepLevel level = CsvZeepLevelParser.Parse(File.ReadAllLines(path));
            Assert.NotNull(level);
            Assert.Equal(expectedZeepHash, level.CalculateHash());
            Assert.Equal(expectedXxh128, level.CalculateXxHash());
            return;
        }
        LevelHashV2 hash = LevelHashV2Calculator.Calculate(content, expectedZeepHash);
        Assert.Equal(expectedZeepHash, hash.ZeepHash);
        Assert.Equal(expectedXxh128, hash.Hash);
    }

}




