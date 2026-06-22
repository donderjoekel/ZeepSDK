using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ZeepSDK.External.FluentResults
{
    public interface IError : IReason
    {
        /// <summary>
        /// Reasons of the error
        /// </summary>
        List<IError> Reasons { get; }
    }
}
