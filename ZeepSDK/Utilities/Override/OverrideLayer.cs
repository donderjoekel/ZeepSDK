using System;

namespace ZeepSDK.Utilities.Override;

/// <summary>
/// Represents a simple override layer that is immediately active when created.
/// Unlike <see cref="ConditionalOverrideLayer{T}"/>, this override is always active until disposed.
/// </summary>
/// <typeparam name="T">The type of value being overridden.</typeparam>
public readonly struct OverrideLayer<T> : IOverrideLayer<T>
{
    private readonly OverrideStack<T> _stack;

    /// <summary>
    /// Gets the unique identifier for this override layer.
    /// </summary>
    public Guid Id { get; }
    
    /// <summary>
    /// Gets the value that this override layer applies.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OverrideLayer{T}"/> struct.
    /// The override is immediately added to the stack and becomes active.
    /// </summary>
    /// <param name="stack">The override stack that this layer belongs to.</param>
    /// <param name="value">The value to apply as an override.</param>
    public OverrideLayer(OverrideStack<T> stack, T value)
    {
        _stack = stack;
        Id = Guid.NewGuid();
        Value = value;

        _stack.AddLayer(this);
    }

    /// <summary>
    /// Removes this override layer from the stack, restoring the previous override state.
    /// </summary>
    public void Dispose()
    {
        _stack.RemoveLayer(this);
    }

    /// <summary>
    /// Determines whether the specified <see cref="OverrideLayer{T}"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The other override layer to compare with.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(OverrideLayer<T> other)
    {
        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Determines whether the specified <see cref="IOverrideLayer{T}"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The other override layer to compare with.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(IOverrideLayer<T> other)
    {
        return other is OverrideLayer<T> layer && Equals(layer);
    }

    /// <summary>
    /// Determines whether the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return obj is OverrideLayer<T> other && Equals(other);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code based on the <see cref="Id"/>.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
