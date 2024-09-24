using System;
using JetBrains.Annotations;

namespace ZeepSDK.Numerics;

/// <summary>
/// A decimal backed vector2
/// </summary>
[PublicAPI]
public struct Vector2 : IEquatable<Vector2>, IFormattable
{
    /// <summary>
    /// The x component of the vector
    /// </summary>
    public readonly decimal X;

    /// <summary>
    /// The y component of the vector
    /// </summary>
    public readonly decimal Y;

    /// <summary>
    /// Creates a new vector2
    /// </summary>
    public Vector2(decimal x, decimal y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Returns a string representation of the vector
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return
            $"<{X.ToString(format, formatProvider)},{Y.ToString(format, formatProvider)}>";
    }

    /// <inheritdoc />
    public bool Equals(Vector2 other)
    {
        return X == other.X && Y == other.Y;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Vector2 other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
