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

        public static ManualLogSource GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }
    }
}

namespace ZeepSDK.Communication
{
    public delegate void MessageReceivedDelegate(IComReceiver receiver, string message);
}

namespace JetBrains.Annotations
{
    [System.AttributeUsage(System.AttributeTargets.All)]
    internal sealed class PublicAPIAttribute : System.Attribute
    {
    }
}
