using JetBrains.Annotations;

namespace ZeepSDK.Level;

[PublicAPI]
public sealed class LevelHashV2
{
    public string Hash { get; set; }
    public string ZeepHash { get; set; }
}
