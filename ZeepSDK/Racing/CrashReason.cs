using JetBrains.Annotations;

namespace ZeepSDK.Racing;

/// <summary>
/// The reason why a player crashed
/// </summary>
[PublicAPI]
public enum CrashReason
{
    /// <summary>
    /// This is used when the reason cannot be mapped to the in-game reason
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// A generic crash
    /// </summary>
    Crashed,
    
    /// <summary>
    /// When you drive into an eye
    /// </summary>
    Eye,

    /// <summary>
    /// When you get hit by a ghost
    /// </summary>
    Ghost,

    /// <summary>
    /// When you get stuck in a spider web
    /// </summary>
    Sticky,
    
    /// <summary>
    /// ???
    /// </summary>
    FoundFootage,
}
