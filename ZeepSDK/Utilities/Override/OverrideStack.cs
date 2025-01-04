using System;
using System.Collections.Generic;

namespace ZeepSDK.Utilities.Override;

public class OverrideStack<T>
{
    private readonly Func<T> _getValue;
    private readonly Action<T> _setValue;
    private readonly List<IOverrideLayer<T>> _layers = [];
    
    private T _baseValue;

    public T Value => _layers.Count == 0 ? _baseValue : _getValue();
    
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

    public ConditionalOverrideLayer<T> Override(T value, Func<bool> condition, ConditionTickType conditionTickType = ConditionTickType.Update)
    {
        return new ConditionalOverrideLayer<T>(this, value, condition, conditionTickType);
    }
    
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