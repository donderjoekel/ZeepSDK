using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ZeepSDK.Numerics;

namespace ZeepSDK.Level;

internal class CsvZeepBlock
{
    private static readonly CultureInfo _culture = new("en-US");

    internal CsvZeepBlock()
    {
        Paints = new List<int>();
        Options = new List<float>();
    }

    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Euler { get; set; }
    public Vector3 Scale { get; set; }
    public CsvSortVector PositionSort { get; set; }
    public CsvSortVector EulerSort { get; set; }
    public CsvSortVector ScaleSort { get; set; }
    public string[] RawPosition { get; set; } = new[] { "0", "0", "0" };
    public string[] RawEuler { get; set; } = new[] { "0", "0", "0" };
    public string[] RawScale { get; set; } = new[] { "0", "0", "0" };
    public List<int> Paints { get; private set; }
    public List<float> Options { get; private set; }

    public override string ToString()
    {
        return
            $"Id: {Id.ToString(_culture)}, Position: {FormatVector(RawPosition)}, Euler: {FormatVector(RawEuler)}, Scale: {FormatVector(RawScale)}, Paints: {string.Join(", ", Paints.Select(x => x.ToString(_culture)))}, Options: {string.Join(", ", Options.Select(FormatFloat))}";
    }

    private static string FormatVector(string[] values)
    {
        return $"<{string.Join(",", values)}>";
    }

    private static string FormatFloat(float value)
    {
        if (float.IsPositiveInfinity(value))
            return "Infinity";
        if (float.IsNegativeInfinity(value))
            return "-Infinity";
        if (value == 0f)
            return "0";

        string scientific = value.ToString("E6", _culture);
        int scientificExponentIndex = scientific.IndexOf('E');
        int exponentValue = int.Parse(scientific[(scientificExponentIndex + 1)..], _culture);
        bool useFixed = exponentValue >= -6 && exponentValue < 7;
        string formatted = useFixed
            ? TrimFractionalZeros(value.ToString($"F{System.Math.Max(0, 6 - exponentValue)}", _culture))
            : scientific;
        int exponentIndex = formatted.IndexOf('E');
        if (exponentIndex < 0)
            return formatted;

        string mantissa = formatted[..exponentIndex].TrimEnd('0').TrimEnd('.');
        string exponent = formatted[(exponentIndex + 1)..];
        char sign = exponent.StartsWith("-", System.StringComparison.Ordinal) ? '-' : '+';
        exponent = exponent.TrimStart('+', '-').TrimStart('0');
        if (exponent.Length == 0)
            exponent = "0";

        return $"{mantissa}e{sign}{exponent}";
    }

    private static string TrimFractionalZeros(string value)
    {
        return value.Contains(".") ? value.TrimEnd('0').TrimEnd('.') : value;
    }
}

internal readonly struct CsvSortVector
{
    public CsvSortVector(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double X { get; }
    public double Y { get; }
    public double Z { get; }
}

internal class CsvSortVectorComparer : IComparer<CsvSortVector>
{
    public int Compare(CsvSortVector v1, CsvSortVector v2)
    {
        int xComparison = v1.X.CompareTo(v2.X);
        if (xComparison != 0)
            return xComparison;

        int yComparison = v1.Y.CompareTo(v2.Y);
        if (yComparison != 0)
            return yComparison;

        return v1.Z.CompareTo(v2.Z);
    }
}
