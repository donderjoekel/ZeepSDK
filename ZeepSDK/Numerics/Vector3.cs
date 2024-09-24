using System;
using JetBrains.Annotations;

namespace ZeepSDK.Numerics;

/// <summary>
/// A decimal backed vector3
/// </summary>
[PublicAPI]
public struct Vector3 : IEquatable<Vector3>, IFormattable
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
    /// The z component of the vector
    /// </summary>
    public readonly decimal Z;

    /// <summary>
    /// Creates a new vector3
    /// </summary>
    public Vector3(decimal x, decimal y, decimal z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Returns a string representation of the vector
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return
            $"<{X.ToString(format, formatProvider)},{Y.ToString(format, formatProvider)},{Z.ToString(format, formatProvider)}>";
    }

    /// <inheritdoc />
    public bool Equals(Vector3 other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Vector3 other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}
