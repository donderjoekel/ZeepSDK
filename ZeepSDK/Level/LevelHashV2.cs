using JetBrains.Annotations;

namespace ZeepSDK.Level;

/// <summary>
/// Represents the version 2 of a level hash.
/// </summary>
[PublicAPI]
public sealed class LevelHashV2
{
    /// <summary>
    /// The XXH128 hash of the level
    /// </summary>
    public string Hash { get; set; }
    /// <summary>
    /// The legacy SHA1 hash of CSV level data or the level.zeepHash of JSON level data
    /// </summary>
    public string ZeepHash { get; set; }
}
