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

    [Fact]
    public void JsonPresentBlocksDoNotAffectXxHash()
    {
        string content = @"{""level"":{""UID"":""uid-json"",""zeepHash"":""legacy-json-hash""},""author"":{""name"":""Author"",""StmID"":""76561198000000000""},""medals"":{""author"":10,""gold"":11,""silver"":12,""bronze"":13},""enviro"":{""skybox"":2,""groundMat"":-1},""blox"":[{""z"":1,""i"":1609,""d"":{""n"":{""ch5"":1}}}]}";
        string withPresentBlock = @"{""level"":{""UID"":""uid-json"",""zeepHash"":""legacy-json-hash""},""author"":{""name"":""Author"",""StmID"":""76561198000000000""},""medals"":{""author"":10,""gold"":11,""silver"":12,""bronze"":13},""enviro"":{""skybox"":2,""groundMat"":-1},""blox"":[{""z"":1,""i"":1609,""d"":{""n"":{""ch5"":1}}},{""i"":2264,""u"":""present"",""z"":999}]}]}";

        Assert.Equal(
            LevelHashV2Calculator.Calculate(content, "legacy-json-hash").Hash,
            LevelHashV2Calculator.Calculate(withPresentBlock, "legacy-json-hash").Hash);
    }

    [Fact]
    public void CsvNaNBlockIdsHashAsZero()
    {
        string zeroBlock = "0,1,2,3,4,5,6,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
        string nanBlock = zeroBlock.Replace("0,1,2", "NaN,1,2");
        string zeroContent = string.Join(
            "\n",
            "LevelEditor2,Author,uid-zero",
            "0,0,0,0,0,0,0,0",
            "1,2,3,4,1,-1",
            zeroBlock);
        string nanContent = zeroContent.Replace(zeroBlock, nanBlock);

        CsvZeepLevel nanLevel = CsvZeepLevelParser.Parse(nanContent.Split('\n'));
        CsvZeepLevel zeroLevel = CsvZeepLevelParser.Parse(zeroContent.Split('\n'));

        Assert.Equal(0, nanLevel.Blocks.Last().Id);
        Assert.Equal(zeroLevel.CalculateHash(), nanLevel.CalculateHash());
        Assert.Equal(zeroLevel.CalculateXxHash(), nanLevel.CalculateXxHash());
    }

    [Fact]
    public void CsvExtraColumnsStayInCanonicalHash()
    {
        string block = "1,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
        string longBlock = $"{block},1,2,3,4,5,6";
        string trimmedContent = string.Join(
            "\n",
            "LevelEditor2,Author,uid-extra",
            "0,0,0,0,0,0,0,0",
            "1,2,3,4,1,-1",
            block);
        string longContent = trimmedContent.Replace(block, longBlock);

        CsvZeepLevel trimmedLevel = CsvZeepLevelParser.Parse(trimmedContent.Split('\n'));
        CsvZeepLevel longLevel = CsvZeepLevelParser.Parse(longContent.Split('\n'));

        Assert.Equal(17, longLevel.Blocks.Last().Options.Count);
        Assert.NotEqual(trimmedLevel.CalculateHash(), longLevel.CalculateHash());
        Assert.NotEqual(trimmedLevel.CalculateXxHash(), longLevel.CalculateXxHash());
    }

    [Fact]
    public void CsvMetadataRowsWithExtraColumnsUseThirdColumnAsUid()
    {
        string content = string.Join(
            "\n",
            "LevelEditor2,Test, Level,discarded",
            "0,0,0,0,0,0,0,0",
            "1,2,3,4,1,-1");

        CsvZeepLevel level = CsvZeepLevelParser.Parse(content.Split('\n'));

        Assert.Equal("Test", level.PlayerName);
        Assert.Equal(" Level", level.UniqueId);
    }

    [Fact]
    public void CsvBlockShapedValidationRowDefaultsValidationAndKeepsFirstBlock()
    {
        string block = "22,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
        string content = string.Join(
            "\n",
            "LevelEditor2,Author,uid-block-validation",
            "0,0,0,0,0,0,0,0",
            block);

        CsvZeepLevel level = CsvZeepLevelParser.Parse(content.Split('\n'));

        Assert.Equal(0, level.ValidationTime);
        Assert.Equal(0, level.GoldTime);
        Assert.Equal(0, level.SilverTime);
        Assert.Equal(0, level.BronzeTime);
        Assert.Equal(0, level.Skybox);
        Assert.Equal(0, level.Ground);
        Assert.Single(level.Blocks);
        Assert.Equal(22, level.Blocks[0].Id);
    }

    [Fact]
    public void CsvShortValidationRowsPadMissingGround()
    {
        string content = string.Join(
            "\n",
            "LevelEditor2,Author,uid-short-validation",
            "0,0,0,0,0,0,0,0",
            "12.5,20,25,30,1");

        CsvZeepLevel level = CsvZeepLevelParser.Parse(content.Split('\n'));

        Assert.Equal(12.5f, level.ValidationTime);
        Assert.Equal(20, level.GoldTime);
        Assert.Equal(25, level.SilverTime);
        Assert.Equal(30, level.BronzeTime);
        Assert.Equal(1, level.Skybox);
        Assert.Equal(0, level.Ground);
    }

    [Fact]
    public void JsonBareNaNMedalsHashAsZero()
    {
        string nonFiniteContent = @"{""level"":{""UID"":""uid-json"",""zeepHash"":""legacy-json-hash""},""author"":{""name"":""Author"",""StmID"":""1""},""medals"":{""author"":NaN,""gold"":Infinity,""silver"":-Infinity,""bronze"":10},""enviro"":{""skybox"":1,""groundMat"":-1},""blox"":[]}";
        string zeroContent = @"{""level"":{""UID"":""uid-json"",""zeepHash"":""legacy-json-hash""},""author"":{""name"":""Author"",""StmID"":""1""},""medals"":{""author"":0,""gold"":0,""silver"":0,""bronze"":10},""enviro"":{""skybox"":1,""groundMat"":-1},""blox"":[]}";

        Assert.Equal(
            LevelHashV2Calculator.Calculate(zeroContent, "legacy-json-hash").Hash,
            LevelHashV2Calculator.Calculate(nonFiniteContent, "legacy-json-hash").Hash);
    }
}
