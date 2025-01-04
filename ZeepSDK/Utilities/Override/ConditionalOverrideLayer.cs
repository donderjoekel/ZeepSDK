using System;

namespace ZeepSDK.Utilities.Override;

public class ConditionalOverrideLayer<T> : IOverrideLayer<T>, IConditionTickable
{
    private readonly OverrideStack<T> _stack;
    private readonly Func<bool> _condition;
    private readonly ConditionTickType _conditionTickType;

    private bool _previousResult;

    public Guid Id { get; } = Guid.NewGuid();
    public T Value { get; }

    ConditionTickType IConditionTickable.TickType => _conditionTickType;

    public ConditionalOverrideLayer(OverrideStack<T> stack, T value, Func<bool> condition,
        ConditionTickType conditionTickType)
    {
        _stack = stack;
        _condition = condition;
        _conditionTickType = conditionTickType;
        Value = value;
        ConditionTicker.Instance.Add(this);
    }

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

    public void Dispose()
    {
        if (_previousResult)
        {
            _stack.RemoveLayer(this);
        }
        
        ConditionTicker.Instance.Remove(this);
    }

    public bool Equals(ConditionalOverrideLayer<T> other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public bool Equals(IOverrideLayer<T> other)
    {
        return other is ConditionalOverrideLayer<T> layer && Equals(layer);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ConditionalOverrideLayer<T>)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
