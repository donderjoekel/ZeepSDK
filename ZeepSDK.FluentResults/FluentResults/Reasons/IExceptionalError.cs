using System;

// ReSharper disable once CheckNamespace
namespace ZeepSDK.External.FluentResults
{
    public interface IExceptionalError : IError
    {
        Exception Exception { get; }

    }
}
