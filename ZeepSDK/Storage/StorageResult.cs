using System;

namespace ZeepSDK.Storage;

/// <summary>Describes whether a storage operation succeeded and preserves failure details.</summary>
public class StorageResult
{
    internal StorageResult(bool isSuccess, string error, Exception exception)
    {
        IsSuccess = isSuccess;
        Error = error;
        Exception = exception;
    }

    /// <summary>Whether the operation completed successfully.</summary>
    public bool IsSuccess { get; }

    /// <summary>Human-readable failure detail, or null on success.</summary>
    public string Error { get; }

    /// <summary>Original exception, or null when no exception was raised.</summary>
    public Exception Exception { get; }

    internal static StorageResult FromSuccess() => new(true, null, null);
    internal static StorageResult FromFailure(string error, Exception exception) => new(false, error, exception);
}

/// <summary>Describes a storage operation that returns a value.</summary>
public sealed class StorageResult<TValue> : StorageResult
{
    private StorageResult(bool isSuccess, TValue value, string error, Exception exception)
        : base(isSuccess, error, exception)
    {
        Value = value;
    }

    /// <summary>Operation value, or default on failure.</summary>
    public TValue Value { get; }

    internal static StorageResult<TValue> FromValue(TValue value) => new(true, value, null, null);
    internal static StorageResult<TValue> FromValueFailure(string error, Exception exception)
        => new(false, default, error, exception);
}
