using System.Collections.Generic;
using ZeepSDK.Numerics;

namespace ZeepSDK.Level;

/// <summary>
/// A representation of a Zeeplevel
/// </summary>
public class ZeepLevel
{
    internal ZeepLevel()
    {
        Blocks = new List<ZeepBlock>();
    }

    /// <summary>
    /// The scene name this level was made in
    /// </summary>
    public string SceneName { get; set; } = null!;

    /// <summary>
    /// The player name
    /// </summary>
    public string PlayerName { get; set; } = null!;

    /// <summary>
    /// The unique ID of the level
    /// </summary>
    public string UniqueId { get; set; } = null!;

    /// <summary>
    /// The position of the camera
    /// </summary>
    public Vector3 CameraPosition { get; set; }

    /// <summary>
    /// The euler angles of the camera
    /// </summary>
    public Vector3 CameraEuler { get; set; }

    /// <summary>
    /// The rotation of the camera
    /// </summary>
    public Vector2 CameraRotation { get; set; }

    /// <summary>
    /// Is the level validated
    /// </summary>
    public bool IsValidated { get; set; }

    /// <summary>
    /// The validation time of the level
    /// </summary>
    public float ValidationTime { get; set; }

    /// <summary>
    /// The gold time of the level
    /// </summary>
    public float GoldTime { get; set; }

    /// <summary>
    /// The silver time of the level
    /// </summary>
    public float SilverTime { get; set; }

    /// <summary>
    /// The bronze time of the level
    /// </summary>
    public float BronzeTime { get; set; }

    /// <summary>
    /// The skybox ID
    /// </summary>
    public int Skybox { get; set; }

    /// <summary>
    /// The ground ID
    /// </summary>
    public int Ground { get; set; }

    /// <summary>
    /// The blocks in the level
    /// </summary>
    public List<ZeepBlock> Blocks { get; set; }
}
