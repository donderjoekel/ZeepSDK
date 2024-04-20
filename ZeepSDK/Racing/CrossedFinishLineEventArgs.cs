using System;

namespace ZeepSDK.Racing;

/// <summary>
/// The arguments for when a player crosses the finish line
/// </summary>
public class CrossedFinishLineEventArgs : EventArgs
{
    /// <inheritdoc />
    public CrossedFinishLineEventArgs(float time)
    {
        Time = time;
    }

    /// <summary>
    /// The time the player crossed the finish line
    /// </summary>
    public float Time
    {
        get;
    }
}
