using System;
using BepInEx.Logging;
using ZeepSDK.Utilities;

namespace ZeepSDK.Extensions;

internal static class DelegateExtensions
{
    public static void InvokeSafe(this Delegate d, params object[] args)
    {
        if (d == null)
        {
            return;
        }

        Delegate[] invocationList = d.GetInvocationList();
        foreach (Delegate invocation in invocationList)
        {
            if (invocation == null)
            {
                continue;
            }

            try
            {
                _ = invocation.DynamicInvoke(args);
            }
            catch (Exception e)
            {
                ManualLogSource manualLogSource = LoggerFactory.GetLogger(invocation.Method.DeclaringType);
                manualLogSource.LogError($"Unhandled exception in {invocation.Method.Name}: " + e);
            }
        }
    }
}
