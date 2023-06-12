using JetBrains.Annotations;

namespace ZeepSDK.Racing;

[PublicAPI]
public enum CrashReason
{
    Unknown = -1,
    Crashed,
    Eye,
    Ghost,
    Sticky,
    FoundFootage,
}
