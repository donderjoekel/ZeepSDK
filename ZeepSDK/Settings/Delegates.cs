using BepInEx;

namespace ZeepSDK.Settings;

public delegate void ModSettingsWindowOpenedDelegate();
public delegate void ModSettingsWindowClosedDelegate();
public delegate void ModSettingsTabSwitchedDelegate(PluginInfo from, PluginInfo to);