using JetBrains.Annotations;
using ZeepSDK.PhotoMode.Patches;

namespace ZeepSDK.PhotoMode;

/// <summary>
/// An API for interacting with Photo Mode
/// </summary>
[PublicAPI]
public static class PhotoModeApi
{
    /// <summary>
    /// An event that fires whenever you enter Photo Mode
    /// </summary>
    public static event PhotoModeEnteredDelegate PhotoModeEntered;

    /// <summary>
    /// An event that fires whenever you exit Photo Mode
    /// </summary>
    public static event PhotoModeExitedDelegate PhotoModeExited;

    internal static void Initialize()
    {
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeEntered += () => PhotoModeEntered?.Invoke();
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeExited += () => PhotoModeExited?.Invoke();
    }
}
