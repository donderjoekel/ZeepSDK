using System;

namespace ZeepSDK.Racing;

/// <summary>
/// The arguments for when a player crashes
/// </summary>
public class CrashedEventArgs : EventArgs
{
    /// <inheritdoc />
    public CrashedEventArgs(CrashReason reason)
    {
        Reason = reason;
    }

    /// <summary>
    /// The reason the player crashed
    /// </summary>
    public CrashReason Reason
    {
        get;
    }
}
