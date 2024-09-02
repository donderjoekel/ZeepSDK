using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace ZeepSDK.Level;

internal class ZeepBlock
{
    private static readonly CultureInfo _culture = new("en-US");

    public ZeepBlock()
    {
        Paints = new List<int>();
        Options = new List<float>();
    }

    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Euler { get; set; }
    public Vector3 Scale { get; set; }
    public List<int> Paints { get; private set; }
    public List<float> Options { get; private set; }

    public override string ToString()
    {
        return
            $"Id: {Id.ToString(_culture)}, Position: {Position.ToString(string.Empty, _culture)}, Euler: {Euler.ToString(string.Empty, _culture)}, Scale: {Scale.ToString(string.Empty, _culture)}, Paints: {string.Join(", ", Paints.Select(x => x.ToString(_culture)))}, Options: {string.Join(", ", Options.Select(x => x.ToString(_culture)))}";
    }
}
