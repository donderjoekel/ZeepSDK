# Mod Settings

ZeepSDK provides a built-in mod settings window that automatically renders your BepInEx config entries. You can use the default layout as-is, customize individual entries, or take full control of the layout when you need something more advanced.

## Overview

When a player opens the mod settings window, ZeepSDK lists every loaded plugin and draws that plugin's config entries grouped by BepInEx section. Each entry is rendered as a row with a label, an editor control, and a **Reset** button that restores the default value.

You do not need to register anything for basic config entries to appear. If your mod defines config entries through BepInEx's `ConfigFile`, they show up automatically when the mod is selected in the settings window.

### Opening and Closing the Window

Use `SettingsApi` to control the settings window programmatically:

```csharp
using ZeepSDK.Settings;

// Open the mod settings window
SettingsApi.OpenModSettings();

// Close the mod settings window
SettingsApi.CloseModSettings();
```

You can also subscribe to window lifecycle events:

```csharp
SettingsApi.ModSettingsWindowOpened += OnSettingsOpened;
SettingsApi.ModSettingsWindowClosed += OnSettingsClosed;
```

The settings window closes automatically when the lobby game state changes.

## Default Rendering

The default renderer supports common BepInEx config value types:

| Type | Control |
|------|---------|
| `bool` | Checkbox |
| `int`, `float`, `double` | Numeric input, or slider when an `AcceptableValueRange<T>` is set |
| `string` | Text input |
| `KeyCode` | Button that opens a key capture popup |
| Enums (except `KeyCode`) | Dropdown |
| `Vector2`, `Vector3`, `Vector4` | Multi-field numeric input |
| `Color` | Color picker |

### Range Sliders

When you define a config entry with an `AcceptableValueRange<int>`, `AcceptableValueRange<float>`, or `AcceptableValueRange<double>`, the settings UI renders a slider instead of a plain numeric field:

```csharp
_volume = Config.Bind(
    "Audio",
    "Volume",
    0.75f,
    new ConfigDescription(
        "Master volume",
        new AcceptableValueRange<float>(0f, 1f)));
```

### Description Metadata

You can influence how an entry appears using prefixes in the config entry's description string:

| Description prefix | Effect |
|--------------------|--------|
| `[hide]` or `[hidden]` | Entry is not shown in the settings UI |
| `[button]` (bool entries only) | Renders an **Execute** button instead of a checkbox |

Example:

```csharp
// Hidden internal setting
_internalFlag = Config.Bind("Debug", "InternalFlag", false, "[hide] Internal use only");

// Action button
_rebuildCache = Config.Bind("Tools", "RebuildCache", false, "[button] Rebuild the local cache");
```

The text after the prefix is still used as the tooltip shown when hovering the entry row.

## Customizing the Default Layout

For most mods, the default layout is enough. ZeepSDK provides three levels of customization that work together without replacing the entire settings panel.

### Custom Labels

Use `SetConfigEntryLabel` to show a friendly name instead of the raw config key:

```csharp
using BepInEx;
using BepInEx.Configuration;
using ZeepSDK.Settings;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private ConfigEntry<float> _volume;

    private void Awake()
    {
        _volume = Config.Bind("Audio", "Volume", 0.75f);

        SettingsApi.SetConfigEntryLabel(this, _volume, "Master Volume");
    }
}
```

Call `ClearConfigEntryLabel` to restore the default label.

### Custom Per-Entry UI

Use `SetConfigEntryDrawer` when one specific entry needs custom controls while keeping the rest of the default layout:

```csharp
SettingsApi.SetConfigEntryDrawer(this, _volume, (gui, context, entryContext) =>
{
    gui.Text(entryContext.Label);

    var entry = (ConfigEntry<float>)entryContext.Entry;
    var value = entry.Value;

    if (gui.NumericEdit(ref value, (200, gui.GetRowHeight())))
        entry.Value = value;
});
```

The `entryContext` passed to your callback provides:

- `Entry` — the `ConfigEntryBase` being drawn
- `Label` — the resolved label, including any override from `SetConfigEntryLabel`
- `DrawDefault(gui, context)` — draws the built-in row renderer for that entry

Mix custom and default rendering in the same callback:

```csharp
SettingsApi.SetConfigEntryDrawer(this, _volume, (gui, context, entryContext) =>
{
    gui.Text("Adjust volume with the slider below:");
    entryContext.DrawDefault(gui, context);
});
```

Call `ClearConfigEntryDrawer` to restore the default row renderer.

### Custom Type Drawers

Use `RegisterConfigEntryTypeDrawer` when you want every config entry of a given value type to use custom UI across all mods. This is useful for shared types such as custom structs or wrapper types used by a library:

```csharp
using ZeepSDK.Settings;
using ZeepSDK.Settings.Drawers;

SettingsApi.RegisterConfigEntryTypeDrawer<MyCustomType>((entry, label) =>
    new MyCustomTypeEntryDrawer(entry, label));
```

Your factory receives the `ConfigEntryBase` and the resolved label (if any) and returns an `IZeepSettingsDrawer` instance.

Call `ClearConfigEntryTypeDrawer<MyCustomType>()` to remove the registration.

### Resolution Order

When the default layout builds a row for a config entry, ZeepSDK resolves the drawer in this order:

1. **Per-entry callback** — registered with `SetConfigEntryDrawer`
2. **Per-type factory** — registered with `RegisterConfigEntryTypeDrawer`
3. **Default row renderer** — built-in control for the entry's value type

Per-entry customization always takes priority over type-based customization.

## Basic Plugin Example

```csharp
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using ZeepSDK.Settings;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private ConfigEntry<bool> _enabled;
    private ConfigEntry<float> _volume;
    private ConfigEntry<KeyCode> _hotkey;

    private void Awake()
    {
        _enabled = Config.Bind("General", "Enabled", true, "Enable the mod");
        _volume = Config.Bind(
            "General",
            "Volume",
            0.75f,
            new ConfigDescription(
                "Output volume",
                new AcceptableValueRange<float>(0f, 1f)));
        _hotkey = Config.Bind("General", "Hotkey", KeyCode.F6, "Toggle overlay");

        SettingsApi.SetConfigEntryLabel(this, _enabled, "Enable Mod");
        SettingsApi.SetConfigEntryLabel(this, _hotkey, "Toggle Overlay");
    }
}
```

No additional registration is required. Open the mod settings window in-game and select your plugin to edit these values.

## Next Steps

If you need to reorder sections, insert custom content between entries, or replace the layout entirely, see **[Custom Mod Settings Layout](custom-mod-settings-layout.md)**.

## Summary

- BepInEx config entries appear automatically in the mod settings window
- Common value types are rendered with appropriate controls out of the box
- `AcceptableValueRange<T>` values render as sliders
- Use description prefixes `[hide]`, `[hidden]`, and `[button]` to control visibility and behavior
- Customize labels with `SetConfigEntryLabel`
- Customize individual entries with `SetConfigEntryDrawer` and `DrawDefault()`
- Customize all entries of a type with `RegisterConfigEntryTypeDrawer`
- Per-entry customization overrides type-based customization
