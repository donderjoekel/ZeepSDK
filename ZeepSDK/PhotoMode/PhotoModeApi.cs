using JetBrains.Annotations;
using ZeepSDK.Extensions;
using ZeepSDK.PhotoMode.Patches;
using ZeepSDK.Scripting.Attributes;

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
    [GenerateEvent]
    public static event PhotoModeEnteredDelegate PhotoModeEntered;

    /// <summary>
    /// An event that fires whenever you exit Photo Mode
    /// </summary>
    [GenerateEvent]
    public static event PhotoModeExitedDelegate PhotoModeExited;

    internal static void Initialize()
    {
        Shutdown();
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeEntered += OnPhotoModeEntered;
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeExited += OnPhotoModeExited;
    }

    internal static void Shutdown()
    {
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeEntered -= OnPhotoModeEntered;
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeExited -= OnPhotoModeExited;
    }

    private static void OnPhotoModeEntered() => PhotoModeEntered.InvokeSafe();
    private static void OnPhotoModeExited() => PhotoModeExited.InvokeSafe();
}
