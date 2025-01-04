using System;

namespace ZeepSDK.Utilities.Override;

public readonly struct OverrideLayer<T> : IOverrideLayer<T>
{
    private readonly OverrideStack<T> _stack;

    public Guid Id { get; }
    public T Value { get; }

    public OverrideLayer(OverrideStack<T> stack, T value)
    {
        _stack = stack;
        Id = Guid.NewGuid();
        Value = value;

        _stack.AddLayer(this);
    }

    public void Dispose()
    {
        _stack.RemoveLayer(this);
    }

    public bool Equals(OverrideLayer<T> other)
    {
        return Id.Equals(other.Id);
    }

    public bool Equals(IOverrideLayer<T> other)
    {
        return other is OverrideLayer<T> layer && Equals(layer);
    }

    public override bool Equals(object obj)
    {
        return obj is OverrideLayer<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
