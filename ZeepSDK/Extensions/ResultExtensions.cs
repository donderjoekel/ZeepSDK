using System.Linq;
using ZeepSDK.Crashlytics;
using ZeepSDK.External.FluentResults;

namespace ZeepSDK.Extensions;

/// <summary>
/// Contains extensions for Result
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Notifies all ExceptionalErrors to Bugsnag
    /// </summary>
    /// <param name="result"></param>
    public static void NotifyErrors(this ResultBase result)
    {
        foreach (ExceptionalError error in result.Errors.Where(x => x is ExceptionalError).Cast<ExceptionalError>())
        {
            CrashlyticsApi.Notify(error.Exception);
        }
    }
}
