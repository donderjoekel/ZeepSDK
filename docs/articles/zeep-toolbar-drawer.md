# Using IZeepToolbarDrawer

`IZeepToolbarDrawer` is an interface that allows you to add custom menu items to the Zeep toolbar menu bar. This provides a clean, integrated way to expose your mod's functionality through the game's menu system.

## Overview

The Zeep toolbar is a menu bar that appears at the top of the screen when toggled (by default, using the key configured in ZeepSDK settings). It includes a built-in "File" menu and allows mods to add their own menus with custom menu items.

### When to Use IZeepToolbarDrawer

Use `IZeepToolbarDrawer` when you want to:
- Add menu items to the toolbar for easy access to mod features
- Provide a consistent UI experience that matches the game's menu system
- Organize multiple mod features under a single menu
- Create shortcuts to open windows, toggle features, or execute commands

## Basic Implementation

### Simple Example

Here's a basic example of implementing `IZeepToolbarDrawer`:

```csharp
using BepInEx;
using Imui.Core;
using ZeepSDK.UI;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private MyToolbarDrawer _toolbarDrawer;

    private void Awake()
    {
        _toolbarDrawer = new MyToolbarDrawer();
        UIApi.AddToolbarDrawer(_toolbarDrawer);
    }

    private void OnDestroy()
    {
        UIApi.RemoveToolbarDrawer(_toolbarDrawer);
    }
}

public class MyToolbarDrawer : IZeepToolbarDrawer
{
    public string MenuTitle => "My Mod";

    public void DrawMenuItems(ImGui gui)
    {
        if (gui.Menu("Open Settings"))
        {
            // Open settings window
        }

        if (gui.Menu("Toggle Feature"))
        {
            // Toggle a feature
        }
    }
}
```

## Menu Items

### Basic Menu Items

The most common way to add menu items is using `gui.Menu()`:

```csharp
public void DrawMenuItems(ImGui gui)
{
    if (gui.Menu("My Menu Item"))
    {
        // This code runs when the menu item is clicked
        Logger.LogInfo("Menu item clicked!");
    }
}
```

### Menu Items with Actions

You can perform any action when a menu item is clicked:

```csharp
public class MyToolbarDrawer : IZeepToolbarDrawer
{
    private bool _featureEnabled = false;

    public string MenuTitle => "My Mod";

    public void DrawMenuItems(ImGui gui)
    {
        if (gui.Menu("Enable Feature"))
        {
            _featureEnabled = true;
            Logger.LogInfo("Feature enabled");
        }

        if (gui.Menu("Disable Feature"))
        {
            _featureEnabled = false;
            Logger.LogInfo("Feature disabled");
        }

        if (gui.Menu("Toggle Feature"))
        {
            _featureEnabled = !_featureEnabled;
            Logger.LogInfo($"Feature is now {(_featureEnabled ? "enabled" : "disabled")}");
        }
    }
}
```

### Separators

Use separators to group related menu items:

```csharp
public void DrawMenuItems(ImGui gui)
{
    if (gui.Menu("First Item"))
    {
        // Action 1
    }

    gui.Separator();

    if (gui.Menu("Second Item"))
    {
        // Action 2
    }

    if (gui.Menu("Third Item"))
    {
        // Action 3
    }

    gui.Separator();

    if (gui.Menu("Last Item"))
    {
        // Action 4
    }
}
```

### Checkable Menu Items

You can create checkable menu items to show the state of a feature:

```csharp
public class MyToolbarDrawer : IZeepToolbarDrawer
{
    private bool _featureEnabled = false;

    public string MenuTitle => "My Mod";

    public void DrawMenuItems(ImGui gui)
    {
        if (gui.Menu("Enable Feature", _featureEnabled))
        {
            _featureEnabled = !_featureEnabled;
        }
    }
}
```

## Complete Examples

### Example 1: Settings and Features Menu

```csharp
using BepInEx;
using Imui.Core;
using ZeepSDK.UI;
using ZeepSDK.Settings;

public class MyToolbarDrawer : IZeepToolbarDrawer
{
    private readonly MyMod _plugin;
    private bool _debugMode = false;
    private bool _showOverlay = true;

    public MyToolbarDrawer(MyMod plugin)
    {
        _plugin = plugin;
    }

    public string MenuTitle => "My Mod";

    public void DrawMenuItems(ImGui gui)
    {
        // Settings
        if (gui.Menu("Open Settings"))
        {
            SettingsApi.OpenModSettings();
        }

        gui.Separator();

        // Features
        if (gui.Menu("Toggle Debug Mode", _debugMode))
        {
            _debugMode = !_debugMode;
            _plugin.Logger.LogInfo($"Debug mode: {_debugMode}");
        }

        if (gui.Menu("Toggle Overlay", _showOverlay))
        {
            _showOverlay = !_showOverlay;
            _plugin.Logger.LogInfo($"Overlay: {_showOverlay}");
        }

        gui.Separator();

        // Actions
        if (gui.Menu("Reset All"))
        {
            _debugMode = false;
            _showOverlay = true;
            _plugin.Logger.LogInfo("All settings reset");
        }
    }
}
```

### Example 2: Window Management

```csharp
using BepInEx;
using Imui.Core;
using ZeepSDK.UI;

public class WindowManagerToolbarDrawer : IZeepToolbarDrawer
{
    private readonly MyMod _plugin;
    private bool _infoWindowOpen = false;
    private bool _debugWindowOpen = false;

    public WindowManagerToolbarDrawer(MyMod plugin)
    {
        _plugin = plugin;
    }

    public string MenuTitle => "Windows";

    public void DrawMenuItems(ImGui gui)
    {
        if (gui.Menu("Show Info Window", _infoWindowOpen))
        {
            _infoWindowOpen = !_infoWindowOpen;
        }

        if (gui.Menu("Show Debug Window", _debugWindowOpen))
        {
            _debugWindowOpen = !_debugWindowOpen;
        }

        gui.Separator();

        if (gui.Menu("Close All Windows"))
        {
            _infoWindowOpen = false;
            _debugWindowOpen = false;
        }
    }
}
```

### Example 3: Multi-Feature Menu

```csharp
using BepInEx;
using Imui.Core;
using ZeepSDK.UI;
using ZeepSDK.Chat;

public class FeatureToolbarDrawer : IZeepToolbarDrawer
{
    private readonly MyMod _plugin;

    public FeatureToolbarDrawer(MyMod plugin)
    {
        _plugin = plugin;
    }

    public string MenuTitle => "Features";

    public void DrawMenuItems(ImGui gui)
    {
        // Quick Actions
        if (gui.Menu("Send Test Message"))
        {
            ChatApi.SendMessage("Test message from toolbar!");
        }

        if (gui.Menu("Clear Chat"))
        {
            ChatApi.ClearChat();
        }

        gui.Separator();

        // Mod Features
        if (gui.Menu("Feature 1"))
        {
            _plugin.ExecuteFeature1();
        }

        if (gui.Menu("Feature 2"))
        {
            _plugin.ExecuteFeature2();
        }

        gui.Separator();

        // Help
        if (gui.Menu("About"))
        {
            ChatApi.AddLocalMessage("My Mod v1.0.0 - Created by You");
        }
    }
}
```

## Integration with Plugin

Here's a complete example showing how to integrate a toolbar drawer with your plugin:

```csharp
using BepInEx;
using Imui.Core;
using ZeepSDK.UI;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private MyToolbarDrawer _toolbarDrawer;

    private void Awake()
    {
        _toolbarDrawer = new MyToolbarDrawer(this);
        UIApi.AddToolbarDrawer(_toolbarDrawer);
        
        Logger.LogInfo("My Mod loaded!");
    }

    private void OnDestroy()
    {
        if (_toolbarDrawer != null)
        {
            UIApi.RemoveToolbarDrawer(_toolbarDrawer);
        }
    }

    public void ExecuteFeature1()
    {
        Logger.LogInfo("Feature 1 executed");
    }

    public void ExecuteFeature2()
    {
        Logger.LogInfo("Feature 2 executed");
    }
}

public class MyToolbarDrawer : IZeepToolbarDrawer
{
    private readonly MyMod _plugin;

    public MyToolbarDrawer(MyMod plugin)
    {
        _plugin = plugin;
    }

    public string MenuTitle => "My Mod";

    public void DrawMenuItems(ImGui gui)
    {
        if (gui.Menu("Feature 1"))
        {
            _plugin.ExecuteFeature1();
        }

        if (gui.Menu("Feature 2"))
        {
            _plugin.ExecuteFeature2();
        }
    }
}
```

## Accessing Plugin State

You can access your plugin's state and methods from the toolbar drawer:

```csharp
public class MyToolbarDrawer : IZeepToolbarDrawer
{
    private readonly MyMod _plugin;

    public MyToolbarDrawer(MyMod plugin)
    {
        _plugin = plugin;
    }

    public string MenuTitle => "My Mod";

    public void DrawMenuItems(ImGui gui)
    {
        // Access plugin configuration
        if (gui.Menu("Toggle Feature", _plugin.FeatureEnabled))
        {
            _plugin.FeatureEnabled = !_plugin.FeatureEnabled;
        }

        // Call plugin methods
        if (gui.Menu("Execute Action"))
        {
            _plugin.DoSomething();
        }

        // Access plugin state
        if (gui.Menu($"Status: {_plugin.GetStatus()}"))
        {
            // Menu item shows current status
        }
    }
}
```

## Best Practices

### 1. Always Clean Up

Remove your toolbar drawer when your plugin is destroyed:

```csharp
private void OnDestroy()
{
    if (_toolbarDrawer != null)
    {
        UIApi.RemoveToolbarDrawer(_toolbarDrawer);
    }
}
```

### 2. Use Descriptive Menu Titles

Choose clear, concise menu titles that identify your mod:

```csharp
// Good
public string MenuTitle => "My Mod";
public string MenuTitle => "Racing Tools";
public string MenuTitle => "Leaderboard Plus";

// Avoid
public string MenuTitle => "Mod";
public string MenuTitle => "Stuff";
```

### 3. Organize Menu Items Logically

Group related items together and use separators:

```csharp
public void DrawMenuItems(ImGui gui)
{
    // Settings group
    if (gui.Menu("Settings")) { }
    if (gui.Menu("Preferences")) { }
    
    gui.Separator();
    
    // Actions group
    if (gui.Menu("Action 1")) { }
    if (gui.Menu("Action 2")) { }
    
    gui.Separator();
    
    // Help group
    if (gui.Menu("About")) { }
    if (gui.Menu("Help")) { }
}
```

### 4. Use Checkable Items for Toggles

Use checkable menu items to show the state of toggleable features:

```csharp
if (gui.Menu("Enable Feature", _featureEnabled))
{
    _featureEnabled = !_featureEnabled;
}
```

### 5. Keep Menu Items Concise

Keep menu item labels short and clear:

```csharp
// Good
if (gui.Menu("Open Settings")) { }
if (gui.Menu("Toggle Debug")) { }

// Avoid
if (gui.Menu("Click here to open the settings window")) { }
if (gui.Menu("This will toggle the debug mode on or off")) { }
```

### 6. Handle Errors Gracefully

Wrap actions in try-catch blocks if they might throw exceptions:

```csharp
public void DrawMenuItems(ImGui gui)
{
    if (gui.Menu("Risky Action"))
    {
        try
        {
            // Potentially risky operation
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error executing action: {ex}");
        }
    }
}
```

### 7. Update State Appropriately

Update state immediately when menu items are clicked:

```csharp
public void DrawMenuItems(ImGui gui)
{
    if (gui.Menu("Toggle Feature", _enabled))
    {
        _enabled = !_enabled;
        SaveSettings(); // Persist state if needed
    }
}
```

## Advanced Usage

### Dynamic Menu Items

You can create dynamic menu items based on game state:

```csharp
public void DrawMenuItems(ImGui gui)
{
    if (ZeepkistNetwork.IsConnectedToGame)
    {
        if (gui.Menu("Disconnect"))
        {
            // Disconnect from game
        }
    }
    else
    {
        if (gui.Menu("Connect"))
        {
            // Connect to game
        }
    }
}
```

### Menu Items with Counters

Display information in menu items:

```csharp
public void DrawMenuItems(ImGui gui)
{
    int itemCount = GetItemCount();
    if (gui.Menu($"Items ({itemCount})"))
    {
        // Show items
    }
}
```

### Conditional Menu Items

Show or hide menu items based on conditions:

```csharp
public void DrawMenuItems(ImGui gui)
{
    if (gui.Menu("Always Visible"))
    {
        // Always shown
    }

    if (_debugMode)
    {
        gui.Separator();
        if (gui.Menu("Debug Only"))
        {
            // Only shown in debug mode
        }
    }
}
```

## Toggling the Toolbar

The toolbar is toggled using a configurable key. Users can:
1. Set the toggle key in ZeepSDK settings
2. Press the key to show/hide the toolbar
3. The toolbar appears at the top of the screen when visible

Your menu will automatically appear in the toolbar when it's visible, so you don't need to handle the visibility yourself.

## Summary

- `IZeepToolbarDrawer` allows you to add custom menus to the Zeep toolbar
- Implement the interface with `MenuTitle` and `DrawMenuItems` method
- Register your drawer with `UIApi.AddToolbarDrawer()`
- Always remove your drawer in `OnDestroy()`
- Use `gui.Menu()` to create menu items
- Use `gui.Separator()` to group related items
- Use checkable menu items to show toggle states
- Organize menu items logically with separators
- Keep menu titles and item labels concise and descriptive

With `IZeepToolbarDrawer`, you can provide users with easy access to your mod's features through the integrated toolbar menu system!
