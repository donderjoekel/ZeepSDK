using System;
using System.Collections.Generic;

namespace ZeepSDK.Utilities.Override;

/// <summary>
/// Manages a stack of override layers that can modify a value.
/// Override layers are applied in a stack-based manner, with the most recent active layer taking precedence.
/// Supports both immediate overrides and conditionally active overrides.
/// </summary>
/// <typeparam name="T">The type of value being managed.</typeparam>
public class OverrideStack<T>
{
    private readonly Func<T> _getValue;
    private readonly Action<T> _setValue;
    private readonly List<IOverrideLayer<T>> _layers = [];
    
    private T _baseValue;

    /// <summary>
    /// Gets the current effective value.
    /// Returns the base value if no override layers are active, otherwise returns the value from the getter function.
    /// </summary>
    public T Value => _layers.Count == 0 ? _baseValue : _getValue();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OverrideStack{T}"/> class.
    /// </summary>
    /// <param name="getValue">A function that retrieves the current value from the underlying system.</param>
    /// <param name="setValue">An action that sets the value in the underlying system.</param>
    /// <param name="baseValue">The initial base value to use when no override layers are active.</param>
    public OverrideStack(Func<T> getValue, Action<T> setValue, T baseValue)
    {
        _getValue = getValue;
        _setValue = setValue;
        _baseValue = baseValue;
    }

    internal void UpdateBaseValue(T value)
    {
        _baseValue = value;
        _setValue(_layers.Count == 0 ? _baseValue : _layers[^1].Value);
    }

    /// <summary>
    /// Creates a conditional override layer that is only active when the specified condition is true.
    /// The condition is evaluated periodically according to the specified <see cref="ConditionTickType"/>.
    /// </summary>
    /// <param name="value">The value to apply when the condition is true.</param>
    /// <param name="condition">The condition function that determines when this override is active.</param>
    /// <param name="conditionTickType">The tick type that determines when the condition is evaluated. Defaults to <see cref="ConditionTickType.Update"/>.</param>
    /// <returns>A conditional override layer that can be disposed to remove the override.</returns>
    public ConditionalOverrideLayer<T> Override(T value, Func<bool> condition, ConditionTickType conditionTickType = ConditionTickType.Update)
    {
        return new ConditionalOverrideLayer<T>(this, value, condition, conditionTickType);
    }
    
    /// <summary>
    /// Creates an immediate override layer that is active as soon as it is created.
    /// The override remains active until the returned layer is disposed.
    /// </summary>
    /// <param name="value">The value to apply as an override.</param>
    /// <returns>An override layer that can be disposed to remove the override.</returns>
    public OverrideLayer<T> Override(T value)
    {
        return new OverrideLayer<T>(this, value);
    }

    internal void AddLayer(IOverrideLayer<T> layer)
    {
        if (_layers.Contains(layer))
            throw new Exception();
        
        _layers.Add(layer);
        _setValue(layer.Value);
    }

    internal void RemoveLayer(IOverrideLayer<T> layer)
    {
        if (!_layers.Contains(layer))
            throw new Exception();

        _layers.Remove(layer);
        _setValue(_layers.Count == 0 ? _baseValue : _layers[^1].Value);
    }
}