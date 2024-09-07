using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace ZeepSDK.Level;

/// <summary>
/// A block in a Zeeplevel
/// </summary>
public class ZeepBlock
{
    private static readonly CultureInfo _culture = new("en-US");

    internal ZeepBlock()
    {
        Paints = new List<int>();
        Options = new List<float>();
    }

    /// <summary>
    /// THe block ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The position of the block
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The rotation of the block in euler angles
    /// </summary>
    public Vector3 Euler { get; set; }

    /// <summary>
    /// The scale of the block
    /// </summary>
    public Vector3 Scale { get; set; }

    /// <summary>
    /// The paints of the block
    /// </summary>
    public List<int> Paints { get; private set; }

    /// <summary>
    /// The options of the block
    /// </summary>
    public List<float> Options { get; private set; }

    public override string ToString()
    {
        return
            $"Id: {Id.ToString(_culture)}, Position: {Position.ToString(string.Empty, _culture)}, Euler: {Euler.ToString(string.Empty, _culture)}, Scale: {Scale.ToString(string.Empty, _culture)}, Paints: {string.Join(", ", Paints.Select(x => x.ToString(_culture)))}, Options: {string.Join(", ", Options.Select(x => x.ToString(_culture)))}";
    }
}