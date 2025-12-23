using System;

namespace ZeepSDK.Utilities.Override;

/// <summary>
/// Represents an override layer that is conditionally active based on a boolean condition.
/// The condition is evaluated periodically according to the specified <see cref="ConditionTickType"/>,
/// and the override is automatically added or removed from the stack when the condition changes.
/// </summary>
/// <typeparam name="T">The type of value being overridden.</typeparam>
public class ConditionalOverrideLayer<T> : IOverrideLayer<T>, IConditionTickable
{
    private readonly OverrideStack<T> _stack;
    private readonly Func<bool> _condition;
    private readonly ConditionTickType _conditionTickType;

    private bool _previousResult;

    /// <summary>
    /// Gets the unique identifier for this override layer.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();
    
    /// <summary>
    /// Gets the value that this override layer applies when active.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets the tick type that determines when the condition is evaluated.
    /// </summary>
    ConditionTickType IConditionTickable.TickType => _conditionTickType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalOverrideLayer{T}"/> class.
    /// </summary>
    /// <param name="stack">The override stack that this layer belongs to.</param>
    /// <param name="value">The value to apply when the condition is true.</param>
    /// <param name="condition">The condition function that determines when this override is active.</param>
    /// <param name="conditionTickType">The tick type that determines when the condition is evaluated (Update, FixedUpdate, LateUpdate, or OnGUI).</param>
    public ConditionalOverrideLayer(OverrideStack<T> stack, T value, Func<bool> condition,
        ConditionTickType conditionTickType)
    {
        _stack = stack;
        _condition = condition;
        _conditionTickType = conditionTickType;
        Value = value;
        ConditionTicker.Instance.Add(this);
    }

    /// <summary>
    /// Evaluates the condition and adds or removes this layer from the stack based on the result.
    /// Called periodically by the <see cref="ConditionTicker"/> according to the <see cref="ConditionTickType"/>.
    /// </summary>
    void IConditionTickable.Tick()
    {
        bool result = _condition();
        if (result == _previousResult) return;
        if (result)
        {
            _stack.AddLayer(this);
        }
        else
        {
            _stack.RemoveLayer(this);
        }

        _previousResult = result;
    }

    /// <summary>
    /// Removes this override layer from the stack and unregisters it from the condition ticker.
    /// If the override is currently active, it is removed from the stack before cleanup.
    /// </summary>
    public void Dispose()
    {
        if (_previousResult)
        {
            _stack.RemoveLayer(this);
        }
        
        ConditionTicker.Instance.Remove(this);
    }

    /// <summary>
    /// Determines whether the specified <see cref="ConditionalOverrideLayer{T}"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The other conditional override layer to compare with.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(ConditionalOverrideLayer<T> other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Determines whether the specified <see cref="IOverrideLayer{T}"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The other override layer to compare with.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(IOverrideLayer<T> other)
    {
        return other is ConditionalOverrideLayer<T> layer && Equals(layer);
    }

    /// <summary>
    /// Determines whether the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ConditionalOverrideLayer<T>)obj);
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
