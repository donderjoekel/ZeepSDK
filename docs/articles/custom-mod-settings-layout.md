# Custom Mod Settings Layout

When the default settings layout is not enough, ZeepSDK lets you build a custom drawer pipeline for your mod. Each element in the pipeline implements `IZeepSettingsDrawer` and draws one piece of the settings panel.

## Overview

The mod settings window renders a list of drawers for the selected plugin. By default, ZeepSDK builds that list automatically from your config entries: section header, entry row, separator, and trailing section spacing.

Register a custom provider with `RegisterModSettingsDrawers` to replace or extend that list. Your provider receives a `ModSettingsDrawerBuildContext` and returns the drawers to render.

### When to Use a Custom Layout

Use a custom layout when you need to:

- Insert non-config UI between settings rows
- Reorder or omit sections
- Group entries differently from BepInEx sections
- Add headers, help text, or action buttons that are not config entries
- Compose the default layout with additional custom drawers

For simpler changes — custom labels, one-off entry UI, or shared type renderers — see **[Mod Settings](mod-settings.md)** first.

## Basic Implementation

```csharp
using System.Collections.Generic;
using BepInEx;
using ZeepSDK.Settings;
using ZeepSDK.Settings.Drawers;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private void Awake()
    {
        SettingsApi.RegisterModSettingsDrawers(this, BuildSettingsDrawers);
    }

    private IEnumerable<IZeepSettingsDrawer> BuildSettingsDrawers(ModSettingsDrawerBuildContext context)
    {
        // Use the default layout unchanged
        return context.CreateDefaultDrawers();
    }
}
```

Call `RegisterModSettingsDrawers` once during plugin startup, typically in `Awake`.

## ModSettingsDrawerBuildContext

The build context gives your provider access to the plugin being drawn and its config entries:

| Member | Description |
|--------|-------------|
| `Plugin` | The `PluginInfo` for the mod being rendered |
| `EntriesBySection` | Config entries grouped by BepInEx section name |
| `CreateDefaultDrawers()` | Builds the default drawer sequence, including any labels and per-entry callbacks you registered |

`CreateDefaultDrawers()` automatically includes:

- Custom labels from `SetConfigEntryLabel`
- Per-entry callbacks from `SetConfigEntryDrawer`
- Type-based drawers from `RegisterConfigEntryTypeDrawer`

So you can register incremental customizations and still call `CreateDefaultDrawers()` inside a custom layout provider.

## Built-in Drawer Types

These public drawer classes are available in the `ZeepSDK.Settings.Drawers` namespace:

### `ZeepSettingsHeaderDrawer`

Draws a section header:

```csharp
yield return new ZeepSettingsHeaderDrawer("Audio");
```

### `ZeepSettingsEntryDrawer`

Draws a single config entry using the built-in row renderer:

```csharp
yield return new ZeepSettingsEntryDrawer(myConfigEntry, "Friendly Label");
```

When `label` is omitted, the config key is used.

### `ZeepSettingsSeparatorDrawer`

Draws a horizontal separator between entries:

```csharp
yield return new ZeepSettingsSeparatorDrawer();
```

The default layout inserts a separator after every visible entry.

## Composing Layouts

### Default Layout Plus Custom Content

Add content before or after the default drawers:

```csharp
private IEnumerable<IZeepSettingsDrawer> BuildSettingsDrawers(ModSettingsDrawerBuildContext context)
{
    yield return new ZeepSettingsHeaderDrawer("About");

    foreach (var drawer in context.CreateDefaultDrawers())
        yield return drawer;
}
```

### Manual Layout

Build the drawer list yourself when you need full control:

```csharp
private IEnumerable<IZeepSettingsDrawer> BuildSettingsDrawers(ModSettingsDrawerBuildContext context)
{
    yield return new ZeepSettingsHeaderDrawer("General");

    foreach (var entry in context.EntriesBySection["General"])
    {
        yield return new ZeepSettingsEntryDrawer(entry);
        yield return new ZeepSettingsSeparatorDrawer();
    }
}
```

Hidden entries (`[hide]` / `[hidden]` in the description) are filtered automatically only when using `CreateDefaultDrawers()`. When building manually, skip those entries yourself if needed.

## Custom Drawer Types

Implement `IZeepSettingsDrawer` to draw arbitrary content:

```csharp
using Imui.Core;
using ZeepSDK.Settings.Drawers;

public class HelpTextDrawer : IZeepSettingsDrawer
{
    private readonly string _text;

    public HelpTextDrawer(string text) => _text = text;

    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
            gui.Text(_text);
    }
}
```

Use it in your provider:

```csharp
yield return new HelpTextDrawer("Adjust these settings while in the main menu.");
yield return new ZeepSettingsSeparatorDrawer();

foreach (var drawer in context.CreateDefaultDrawers())
    yield return drawer;
```

Return your custom drawer from a type factory when registering with `RegisterConfigEntryTypeDrawer`:

```csharp
public class MyCustomTypeEntryDrawer : IZeepSettingsDrawer
{
    private readonly ConfigEntry<MyCustomType> _entry;
    private readonly string _label;

    public MyCustomTypeEntryDrawer(ConfigEntryBase entry, string label)
    {
        _entry = (ConfigEntry<MyCustomType>)entry;
        _label = label ?? entry.Definition.Key;
    }

    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
        {
            gui.Text(_label);
            // Draw custom controls for _entry.Value
        }
    }
}

SettingsApi.RegisterConfigEntryTypeDrawer<MyCustomType>(
    (entry, label) => new MyCustomTypeEntryDrawer(entry, label));
```

## ZeepSettingsDrawContext

While drawing, shared services are available through `ZeepSettingsDrawContext`:

```csharp
public void Draw(ImGui gui, ZeepSettingsDrawContext context)
{
    // For KeyCode config entries
    context.OpenKeyCodePopup(myKeyCodeEntry);
}
```

Use this when building custom KeyCode UI so the standard key capture popup is used.

## Per-Entry Callbacks in Custom Layouts

`SetConfigEntryDrawer` integrates with `CreateDefaultDrawers()`. When you call `CreateDefaultDrawers()`, any per-entry callbacks you registered are applied automatically.

If you build the layout manually with `ZeepSettingsEntryDrawer`, those callbacks are **not** used — manual drawers always render the built-in row renderer. For manual layouts, either:

- Draw the entry yourself inside a custom `IZeepSettingsDrawer`, or
- Prefer `CreateDefaultDrawers()` and use `SetConfigEntryDrawer` for entries that need custom UI

## Complete Example

```csharp
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using ZeepSDK.Settings;
using ZeepSDK.Settings.Drawers;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private ConfigEntry<bool> _enabled;
    private ConfigEntry<float> _volume;

    private void Awake()
    {
        _enabled = Config.Bind("General", "Enabled", true);
        _volume = Config.Bind(
            "General",
            "Volume",
            0.75f,
            new ConfigDescription(
                "Output volume",
                new AcceptableValueRange<float>(0f, 1f)));

        SettingsApi.SetConfigEntryLabel(this, _volume, "Master Volume");

        SettingsApi.RegisterModSettingsDrawers(this, context =>
        {
            yield return new ZeepSettingsHeaderDrawer("Quick Start");
            yield return new HelpTextDrawer("Enable the mod, then adjust volume below.");
            yield return new ZeepSettingsSeparatorDrawer();

            foreach (var drawer in context.CreateDefaultDrawers())
                yield return drawer;
        });
    }
}

public class HelpTextDrawer : IZeepSettingsDrawer
{
    private readonly string _text;

    public HelpTextDrawer(string text) => _text = text;

    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
            gui.Text(_text);
    }
}
```

Note: `HelpTextDrawer` is defined above as a reusable `IZeepSettingsDrawer` implementation.

## Best Practices

### Prefer Incremental Customization

Start with the default layout and add only what you need:

- Custom labels → `SetConfigEntryLabel`
- One special entry → `SetConfigEntryDrawer`
- Shared custom type → `RegisterConfigEntryTypeDrawer`
- Full layout control → `RegisterModSettingsDrawers`

### Use `CreateDefaultDrawers()` When Possible

Calling `CreateDefaultDrawers()` keeps your layout aligned with ZeepSDK defaults — hidden entry filtering, label overrides, per-entry callbacks, type drawers, separators, and section spacing.

### Match Existing Indentation

Built-in drawers wrap content in `gui.Indent()`. Do the same in custom drawers so rows align with the default layout.

### Register Once

Register your layout provider once during plugin initialization. The drawer list is rebuilt when the settings window needs it; your provider is called at that time.

## Summary

- Register a layout provider with `RegisterModSettingsDrawers`
- Return `IEnumerable<IZeepSettingsDrawer>` from your provider callback
- Use `ModSettingsDrawerBuildContext.CreateDefaultDrawers()` to compose with the default layout
- Use `ZeepSettingsHeaderDrawer`, `ZeepSettingsEntryDrawer`, and `ZeepSettingsSeparatorDrawer` to build layouts manually
- Implement `IZeepSettingsDrawer` for fully custom content
- Use `ZeepSettingsDrawContext.OpenKeyCodePopup` for KeyCode editing in custom UI
