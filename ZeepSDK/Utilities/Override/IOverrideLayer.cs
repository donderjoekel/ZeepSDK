using System;

namespace ZeepSDK.Utilities.Override;

public interface IOverrideLayer<T> : IDisposable, IEquatable<IOverrideLayer<T>>
{
    Guid Id { get; }
    T Value { get; }
}
