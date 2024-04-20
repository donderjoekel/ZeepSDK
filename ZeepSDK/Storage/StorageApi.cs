using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepSDK.Utilities;

namespace ZeepSDK.Storage;

/// <summary>
/// An API for creating storage objects for your mod
/// </summary>
[PublicAPI]
public static class StorageApi
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(StorageApi));

    /// <summary>
    /// Creates a new instance of <see cref="IModStorage"/> for the given plugin
    /// </summary>
    /// <param name="plugin">The instance to create the storage for</param>
    /// <returns>A new instance</returns>
    public static IModStorage CreateModStorage(BaseUnityPlugin plugin)
    {
        if (plugin != null)
        {
            return new ModStorage(plugin);
        }

        _logger.LogError("StorageApi.CreateModStorage requires a non-null plugin parameters");
        return null;
    }
}
