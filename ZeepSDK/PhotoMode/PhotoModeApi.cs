using System;
using JetBrains.Annotations;
using ZeepSDK.Extensions;
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
    public static event EventHandler PhotoModeEntered;

    /// <summary>
    /// An event that fires whenever you exit Photo Mode
    /// </summary>
    public static event EventHandler PhotoModeExited;

    internal static void Initialize()
    {
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeEntered += () => PhotoModeEntered.InvokeSafe();
        EnableFlyingCamera2_ToggleFlyingCamera.PhotoModeExited += () => PhotoModeExited.InvokeSafe();
    }
}
