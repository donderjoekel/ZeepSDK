using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ZeepSDK.External.FluentResults
{
    public interface IReason
    {
        string Message { get; }

        Dictionary<string, object> Metadata { get; }
    }
}
