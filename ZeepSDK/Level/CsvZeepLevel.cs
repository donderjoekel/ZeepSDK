using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BepInEx.Logging;
using ZeepSDK.Extensions;
using ZeepSDK.Numerics;
using ZeepSDK.Utilities;

namespace ZeepSDK.Level;

internal class CsvZeepLevel
{
    private const int PresentBlockID = 2264;
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(CsvZeepLevel));
    private static readonly CsvSortVectorComparer _csvSortVectorComparer = new();
    private static readonly IntComparer _intSequenceComparer = new();
    private static readonly FloatComparer _floatSequenceComparer = new();

    internal CsvZeepLevel()
    {
        Blocks = new List<CsvZeepBlock>();
    }

    public string SceneName { get; set; } = null!;
    public string PlayerName { get; set; } = null!;
    public string UniqueId { get; set; } = null!;
    public Vector3 CameraPosition { get; set; }
    public Vector3 CameraEuler { get; set; }
    public Vector2 CameraRotation { get; set; }
    public bool IsValidated { get; set; }
    public float ValidationTime { get; set; }
    public float GoldTime { get; set; }
    public float SilverTime { get; set; }
    public float BronzeTime { get; set; }
    public int Skybox { get; set; }
    public int Ground { get; set; }
    public List<CsvZeepBlock> Blocks { get; set; }

    public string CalculateHash()
    {
        return Hash(this);
    }

    public string CalculateXxHash()
    {
        return XxHash(this);
    }

    internal static string Hash(CsvZeepLevel zeepLevel)
    {
        if (zeepLevel == null)
        {
            logger.LogWarning("Trying to hash a null level");
            return null;
        }

        byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(CanonicalContent(zeepLevel)));
        return ToUpperHex(hash);
    }

    internal static string XxHash(CsvZeepLevel zeepLevel)
    {
        if (zeepLevel == null)
        {
            logger.LogWarning("Trying to hash a null level");
            return null;
        }

        byte[] hash = XxHash128Reflection.Hash(Encoding.UTF8.GetBytes(CanonicalContent(zeepLevel)));
        return ToUpperHex(hash);
    }

    internal static string CanonicalContent(CsvZeepLevel zeepLevel)
    {
        StringBuilder inputBuilder = new();
        inputBuilder.AppendCLRF(zeepLevel.Skybox.ToString());
        inputBuilder.AppendCLRF(zeepLevel.Ground.ToString());

        List<CsvZeepBlock> orderedBlocks = zeepLevel.Blocks
            .Where(x => x.Id != PresentBlockID)
            .OrderBy(x => x.Id)
            .ThenBy(x => x.PositionSort, _csvSortVectorComparer)
            .ThenBy(x => x.EulerSort, _csvSortVectorComparer)
            .ThenBy(x => x.ScaleSort, _csvSortVectorComparer)
            .ThenBy(x => x.Paints, _intSequenceComparer)
            .ThenBy(x => x.Options, _floatSequenceComparer)
            .ToList();

        foreach (CsvZeepBlock block in orderedBlocks)
        {
            inputBuilder.AppendCLRF(block.ToString());
        }

        return inputBuilder.ToString();
    }

    private static string ToUpperHex(byte[] hash)
    {
        StringBuilder hashBuilder = new(hash.Length * 2);

        foreach (byte b in hash)
        {
            hashBuilder.Append(b.ToString("X2"));
        }

        return hashBuilder.ToString();
    }
}
