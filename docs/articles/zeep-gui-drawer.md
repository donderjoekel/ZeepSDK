# Using IZeepGUIDrawer

`IZeepGUIDrawer` is an interface that allows you to create custom GUI elements that are rendered using ZeepSDK's immediate mode GUI system. This is perfect for creating windows, overlays, debug panels, and other custom UI elements that integrate seamlessly with the game's UI system.

## Overview

`IZeepGUIDrawer` provides a way to draw custom UI elements using the ImGui (Immediate Mode GUI) system. Your drawer's `OnZeepGUI` method is called every frame during the GUI rendering phase, allowing you to create interactive UI elements.

### When to Use IZeepGUIDrawer

Use `IZeepGUIDrawer` when you need to:
- Create custom windows or overlays
- Display debug information
- Create interactive UI panels
- Build mod configuration interfaces
- Display real-time game information

## Basic Implementation

### Simple Example

Here's a basic example of implementing `IZeepGUIDrawer`:

```csharp
using BepInEx;
using Imui.Core;
using ZeepSDK.UI;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private MyGUIDrawer _drawer;

    private void Awake()
    {
        _drawer = new MyGUIDrawer();
        UIApi.AddZeepGUIDrawer(_drawer);
    }

    private void OnDestroy()
    {
        UIApi.RemoveZeepGUIDrawer(_drawer);
    }
}

public class MyGUIDrawer : IZeepGUIDrawer
{
    private bool _windowOpen = true;

    public void OnZeepGUI(ImGui gui)
    {
        if (_windowOpen && gui.BeginWindow("My Custom Window", ref _windowOpen, (400, 300)))
        {
            gui.Text("Hello from ZeepSDK!");
            
            if (gui.Button("Click Me"))
            {
                // Handle button click
            }
            
            gui.EndWindow();
        }
    }
}
```

## Common ImGui Controls

The `ImGui` instance provides many controls for building your UI. Here are the most commonly used ones:

### Windows

```csharp
// Create a window
bool windowOpen = true;
if (gui.BeginWindow("Window Title", ref windowOpen, (width, height)))
{
    // Window content
    gui.EndWindow();
}
```

### Text

```csharp
// Simple text
gui.Text("Hello World");

// Text with custom settings
gui.Text("Styled Text", new ImTextSettings(24, 0.5f, 0.5f, true));

// Text with tooltip
gui.Text("Hover me", "This is a tooltip");
```

### Buttons

```csharp
// Simple button
if (gui.Button("Click Me"))
{
    // Button was clicked
}

// Button with custom size
if (gui.Button("Click Me", (100, 30)))
{
    // Button was clicked
}
```

### Checkboxes

```csharp
bool value = true;
if (gui.Checkbox(ref value, "Enable Feature"))
{
    // Value changed
}
```

### Text Input

```csharp
string text = "Initial value";
string newText = gui.TextEdit(text, (200, 30));
if (newText != text)
{
    text = newText;
    // Text changed
}
```

### Numeric Input

```csharp
int number = 10;
if (gui.NumericEdit(ref number, (100, 30)))
{
    // Number changed
}

float floatValue = 1.5f;
if (gui.NumericEdit(ref floatValue, (100, 30)))
{
    // Float value changed
}
```

### Dropdowns

```csharp
int selectedIndex = 0;
string[] options = { "Option 1", "Option 2", "Option 3" };
if (gui.Dropdown(ref selectedIndex, options, (150, 30)))
{
    // Selection changed
}
```

### Color Picker

```csharp
Color color = Color.red;
if (gui.ColorEdit(ref color, (100, 30)))
{
    // Color changed
}
```

### Layout

```csharp
// Horizontal layout
gui.BeginHorizontal((400, 30));
gui.Text("Left");
gui.Text("Right");
gui.EndHorizontal();

// Vertical layout
gui.BeginVertical((400, 200));
gui.Text("Top");
gui.Text("Bottom");
gui.EndVertical();

// Using scopes (recommended)
using (gui.Horizontal((400, 30)))
{
    gui.Text("Left");
    gui.Text("Right");
}

using (gui.Vertical((400, 200)))
{
    gui.Text("Top");
    gui.Text("Bottom");
}
```

### Separators and Spacing

```csharp
gui.Separator();
gui.AddSpacing();
gui.AddSpacing(20); // Custom spacing
```

## Complete Examples

### Example 1: Simple Info Window

```csharp
using Imui.Core;
using UnityEngine;
using ZeepSDK.UI;

public class InfoWindowDrawer : IZeepGUIDrawer
{
    private bool _visible = true;
    private int _frameCount = 0;
    private float _fps = 0;
    private float _fpsUpdateTimer = 0;

    public void OnZeepGUI(ImGui gui)
    {
        // Update FPS
        _frameCount++;
        _fpsUpdateTimer += Time.deltaTime;
        if (_fpsUpdateTimer >= 1.0f)
        {
            _fps = _frameCount / _fpsUpdateTimer;
            _frameCount = 0;
            _fpsUpdateTimer = 0;
        }

        // Draw window
        if (_visible && gui.BeginWindow("Info Window", ref _visible, (300, 200)))
        {
            gui.Text($"FPS: {_fps:F1}");
            gui.Separator();
            gui.Text($"Time: {Time.time:F2}s");
            gui.Text($"Frame: {Time.frameCount}");
            
            gui.EndWindow();
        }
    }
}
```

### Example 2: Settings Panel

```csharp
using Imui.Core;
using UnityEngine;
using ZeepSDK.UI;

public class SettingsPanelDrawer : IZeepGUIDrawer
{
    private bool _visible = false;
    private bool _enableFeature = true;
    private float _volume = 0.5f;
    private string _playerName = "Player";
    private int _selectedOption = 0;
    private readonly string[] _options = { "Low", "Medium", "High" };

    public void OnZeepGUI(ImGui gui)
    {
        // Toggle visibility with a key
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _visible = !_visible;
        }

        if (!_visible)
            return;

        if (gui.BeginWindow("Settings", ref _visible, (400, 400)))
        {
            gui.Text("Mod Settings", new ImTextSettings(24, 0.5f));
            gui.Separator();

            // Checkbox
            if (gui.Checkbox(ref _enableFeature, "Enable Feature"))
            {
                // Feature toggled
            }

            gui.Separator();

            // Slider-like numeric input
            gui.Text("Volume:");
            if (gui.NumericEdit(ref _volume, (200, 30)))
            {
                _volume = Mathf.Clamp01(_volume);
            }

            gui.Separator();

            // Text input
            gui.Text("Player Name:");
            string newName = gui.TextEdit(_playerName, (200, 30));
            if (newName != _playerName)
            {
                _playerName = newName;
            }

            gui.Separator();

            // Dropdown
            gui.Text("Quality:");
            if (gui.Dropdown(ref _selectedOption, _options, (200, 30)))
            {
                // Quality changed
            }

            gui.Separator();

            // Buttons
            using (gui.Horizontal((400, 30)))
            {
                if (gui.Button("Save", (100, 30)))
                {
                    // Save settings
                }

                if (gui.Button("Reset", (100, 30)))
                {
                    _enableFeature = true;
                    _volume = 0.5f;
                    _playerName = "Player";
                    _selectedOption = 0;
                }
            }

            gui.EndWindow();
        }
    }
}
```

### Example 3: Debug Overlay

```csharp
using Imui.Core;
using Imui.Style;
using UnityEngine;
using ZeepSDK.UI;
using ZeepkistClient;

public class DebugOverlayDrawer : IZeepGUIDrawer
{
    private bool _enabled = false;

    public void OnZeepGUI(ImGui gui)
    {
        if (!_enabled)
            return;

        // Draw overlay in top-right corner
        var screenSize = new Vector2(Screen.width, Screen.height);
        var overlaySize = new Vector2(300, 200);
        var overlayPos = new Vector2(screenSize.x - overlaySize.x - 10, 10);

        gui.Layout.Push(ImAxis.Vertical, new ImRect(overlayPos, overlaySize));
        
        gui.Text("Debug Info", new ImTextSettings(18, 0.5f));
        gui.Separator();

        if (PlayerManager.Instance != null)
        {
            gui.Text($"Position: {PlayerManager.Instance.currentPlayer.transform.position}");
            gui.Text($"Velocity: {PlayerManager.Instance.currentPlayer.rigidbody.velocity.magnitude:F2}");
        }

        if (ZeepkistNetwork.IsConnectedToGame)
        {
            gui.Text($"Players: {ZeepkistNetwork.Players.Count}");
            gui.Text($"Host: {ZeepkistNetwork.IsMasterClient}");
        }

        gui.Layout.Pop();
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }
}
```

## Best Practices

### 1. Always Clean Up

Remove your drawer when your plugin is destroyed:

```csharp
private void OnDestroy()
{
    UIApi.RemoveZeepGUIDrawer(_drawer);
}
```

### 2. Use Scopes for Layouts

Prefer using scopes with `using` statements for layouts:

```csharp
// Good
using (gui.Horizontal((400, 30)))
{
    gui.Text("Left");
    gui.Text("Right");
}

// Also works, but scopes are safer
gui.BeginHorizontal((400, 30));
gui.Text("Left");
gui.Text("Right");
gui.EndHorizontal();
```

### 3. Check Window Visibility

Always check if a window should be drawn:

```csharp
if (_visible && gui.BeginWindow("Title", ref _visible, size))
{
    // Window content
    gui.EndWindow();
}
```

### 4. Handle Input Responsibly

Don't process input when windows are not visible or when the game is paused:

```csharp
public void OnZeepGUI(ImGui gui)
{
    if (!_visible || Time.timeScale == 0)
        return;
    
    // Draw UI
}
```

### 5. Use Appropriate Sizes

Make sure your windows and controls are appropriately sized:

```csharp
// Good - explicit size
gui.BeginWindow("Title", ref open, (400, 300));

// Good - use layout width/height
var width = gui.GetLayoutWidth();
var height = gui.GetLayoutHeight();
gui.BeginWindow("Title", ref open, (width, height));
```

### 6. Organize Complex UIs

Break down complex UIs into smaller methods:

```csharp
public void OnZeepGUI(ImGui gui)
{
    if (gui.BeginWindow("Complex UI", ref _visible, (600, 500)))
    {
        DrawHeader(gui);
        DrawSettings(gui);
        DrawButtons(gui);
        gui.EndWindow();
    }
}

private void DrawHeader(ImGui gui)
{
    gui.Text("Settings", new ImTextSettings(24));
    gui.Separator();
}

private void DrawSettings(ImGui gui)
{
    // Settings controls
}

private void DrawButtons(ImGui gui)
{
    // Buttons
}
```

### 7. Store State Appropriately

Store UI state in your drawer class:

```csharp
public class MyDrawer : IZeepGUIDrawer
{
    private bool _windowOpen = true;
    private string _inputText = "";
    private int _selectedIndex = 0;
    
    // UI state goes here
}
```

## Advanced Topics

### Conditional Rendering

You can conditionally render UI elements based on game state:

```csharp
public void OnZeepGUI(ImGui gui)
{
    if (!ZeepkistNetwork.IsConnectedToGame)
        return; // Don't show UI when not in a game
    
    // Draw UI
}
```

### Multiple Windows

You can create multiple windows in a single drawer:

```csharp
public void OnZeepGUI(ImGui gui)
{
    if (_window1Open && gui.BeginWindow("Window 1", ref _window1Open, (300, 200)))
    {
        gui.Text("Window 1 Content");
        gui.EndWindow();
    }

    if (_window2Open && gui.BeginWindow("Window 2", ref _window2Open, (300, 200)))
    {
        gui.Text("Window 2 Content");
        gui.EndWindow();
    }
}
```

### Tooltips

Add tooltips to provide additional information:

```csharp
gui.Text("Hover me", "This is a tooltip");
gui.TooltipAtLastControl("Additional information");
```

## Integration with Plugin

Here's a complete example of integrating a GUI drawer with your plugin:

```csharp
using BepInEx;
using ZeepSDK.UI;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private MyGUIDrawer _drawer;

    private void Awake()
    {
        _drawer = new MyGUIDrawer(this);
        UIApi.AddZeepGUIDrawer(_drawer);
        
        Logger.LogInfo("My Mod loaded!");
    }

    private void OnDestroy()
    {
        if (_drawer != null)
        {
            UIApi.RemoveZeepGUIDrawer(_drawer);
        }
    }
}
```

## Summary

- `IZeepGUIDrawer` allows you to create custom UI elements using ImGui
- Implement the interface and register your drawer with `UIApi.AddZeepGUIDrawer()`
- Always remove your drawer in `OnDestroy()`
- Use ImGui controls like windows, buttons, text inputs, and layouts
- Follow best practices for clean, maintainable UI code
- Organize complex UIs into smaller methods
- Handle state appropriately in your drawer class

With `IZeepGUIDrawer`, you can create rich, interactive UI elements that integrate seamlessly with Zeepkist's UI system!
