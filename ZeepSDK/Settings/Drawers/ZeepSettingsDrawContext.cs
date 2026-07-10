using System;
using BepInEx.Configuration;
using UnityEngine;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Shared services available to settings drawers while drawing.
/// </summary>
public sealed class ZeepSettingsDrawContext
{
    internal Action<ConfigEntry<KeyCode>> OpenKeyCodePopupInternal { get; init; }

    /// <summary>
    /// Opens the KeyCode capture popup for the given config entry.
    /// </summary>
    /// <param name="entry">The config entry to edit.</param>
    public void OpenKeyCodePopup(ConfigEntry<KeyCode> entry) => OpenKeyCodePopupInternal(entry);
}
