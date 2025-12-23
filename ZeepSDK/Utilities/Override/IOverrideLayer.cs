using System;

namespace ZeepSDK.Utilities.Override;

/// <summary>
/// Represents a layer in an override stack that can apply a value override.
/// Implementations include <see cref="OverrideLayer{T}"/> for immediate overrides
/// and <see cref="ConditionalOverrideLayer{T}"/> for conditionally active overrides.
/// </summary>
/// <typeparam name="T">The type of value being overridden.</typeparam>
public interface IOverrideLayer<T> : IDisposable, IEquatable<IOverrideLayer<T>>
{
    /// <summary>
    /// Gets the unique identifier for this override layer.
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// Gets the value that this override layer applies.
    /// </summary>
    T Value { get; }
}
