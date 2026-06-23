namespace BepInEx.Logging
{
    internal sealed class ManualLogSource
    {
        public void LogWarning(object value)
        {
        }

        public void LogError(object value)
        {
        }
    }
}

namespace ZeepSDK.Utilities
{
    using BepInEx.Logging;

    internal static class LoggerFactory
    {
        public static ManualLogSource GetLogger(System.Type type)
        {
            return new ManualLogSource();
        }
    }
}

namespace JetBrains.Annotations
{
    [System.AttributeUsage(System.AttributeTargets.All)]
    internal sealed class PublicAPIAttribute : System.Attribute
    {
    }
}
