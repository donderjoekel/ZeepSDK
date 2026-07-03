using System.Collections.Generic;
using ZeepSDK.Settings.Drawers;

namespace ZeepSDK.Settings;

/// <summary>
/// Represents a method that is called when the mod settings window is opened.
/// </summary>
public delegate void ModSettingsWindowOpenedDelegate();

/// <summary>
/// Represents a method that is called when the mod settings window is closed.
/// </summary>
public delegate void ModSettingsWindowClosedDelegate();

/// <summary>
/// Builds the list of settings drawers to render for a mod.
/// </summary>
/// <param name="context">Context containing the plugin and its config entries.</param>
/// <returns>The drawers to render when this mod is selected in the settings window.</returns>
public delegate IEnumerable<IZeepSettingsDrawer> ModSettingsDrawersDelegate(ModSettingsDrawerBuildContext context);
