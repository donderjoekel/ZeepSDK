using System;

namespace ZeepSDK.Racing;

/// <summary>
/// The arguments for when a player passes a checkpoint
/// </summary>
public class PassedCheckpointEventArgs : EventArgs
{
    /// <inheritdoc />
    public PassedCheckpointEventArgs(float time)
    {
        Time = time;
    }

    /// <summary>
    /// The time the player passed the checkpoint
    /// </summary>
    public float Time
    {
        get;
    }
}
