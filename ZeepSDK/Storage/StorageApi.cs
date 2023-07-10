using BepInEx;

namespace ZeepSDK.Storage;

/// <summary>
/// An API for creating storage objects for your mod
/// </summary>
public static class StorageApi
{
    /// <summary>
    /// Creates a new instance of <see cref="IModStorage"/> for the given plugin
    /// </summary>
    /// <param name="plugin">The instance to create the storage for</param>
    /// <returns>A new instance</returns>
    public static IModStorage CreateModStorage(BaseUnityPlugin plugin)
    {
        return new ModStorage(plugin);
    }
}
