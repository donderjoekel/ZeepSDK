# Creating Local Chat Commands

Local chat commands are commands that can be executed by the local player (the person playing the game) in the chat. They are useful for providing quick access to mod functionality without needing to open menus or use keyboard shortcuts.

## Overview

ZeepSDK provides two ways to create local chat commands:

1. **Callback Delegate Method** - Simple and quick for basic commands
2. **Interface Implementation Method** - Better for more complex commands or when you need to reuse command logic

Both methods are registered using the `ChatCommandApi.RegisterLocalChatCommand` method.

## Method 1: Using Callback Delegates

The simplest way to create a local chat command is using a callback delegate. This is perfect for simple commands that don't require complex logic.

### Basic Example

```csharp
using BepInEx;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private void Awake()
    {
        // Register a simple command
        ChatCommandApi.RegisterLocalChatCommand(
            prefix: "/",
            command: "hello",
            description: "Says hello to the player",
            callback: OnHelloCommand
        );
    }

    private void OnHelloCommand(string arguments)
    {
        ChatApi.AddLocalMessage("Hello! You typed: " + arguments);
    }
}
```

When a player types `/hello` or `/hello world` in the chat, the `OnHelloCommand` method will be called with the remaining text as the `arguments` parameter.

### Command with Arguments

You can parse arguments to create more sophisticated commands:

```csharp
using BepInEx;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private void Awake()
    {
        ChatCommandApi.RegisterLocalChatCommand(
            prefix: "/",
            command: "teleport",
            description: "Teleports to coordinates: /teleport x y z",
            callback: OnTeleportCommand
        );
    }

    private void OnTeleportCommand(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            ChatApi.AddLocalMessage("Usage: /teleport x y z");
            return;
        }

        string[] parts = arguments.Split(' ');
        if (parts.Length != 3)
        {
            ChatApi.AddLocalMessage("Invalid arguments. Usage: /teleport x y z");
            return;
        }

        if (float.TryParse(parts[0], out float x) &&
            float.TryParse(parts[1], out float y) &&
            float.TryParse(parts[2], out float z))
        {
            // Perform teleportation logic here
            ChatApi.AddLocalMessage($"Teleporting to ({x}, {y}, {z})");
        }
        else
        {
            ChatApi.AddLocalMessage("Invalid coordinates. Please use numbers.");
        }
    }
}
```

## Method 2: Using Interface Implementation

For more complex commands or when you want to organize your code better, you can implement the `ILocalChatCommand` interface. This approach is also useful when you need to create multiple commands or want to reuse command logic.

### Basic Interface Implementation

```csharp
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

public class HelloCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "hello";
    public string Description => "Says hello to the player";

    public void Handle(string arguments)
    {
        ChatApi.AddLocalMessage("Hello! You typed: " + arguments);
    }
}
```

Then register it in your plugin:

```csharp
using BepInEx;
using ZeepSDK.ChatCommands;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private void Awake()
    {
        // Register using the generic method
        ChatCommandApi.RegisterLocalChatCommand<HelloCommand>();
        
        // Or register an instance
        ChatCommandApi.RegisterLocalChatCommand(new HelloCommand());
    }
}
```

### Advanced Example: Command with State

Interface implementation is particularly useful when your command needs to maintain state or access plugin data:

```csharp
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

public class ToggleFeatureCommand : ILocalChatCommand
{
    private readonly MyMod plugin;
    private bool featureEnabled = false;

    public ToggleFeatureCommand(MyMod plugin)
    {
        this.plugin = plugin;
    }

    public string Prefix => "/";
    public string Command => "toggle";
    public string Description => "Toggles a feature on/off";

    public void Handle(string arguments)
    {
        featureEnabled = !featureEnabled;
        ChatApi.AddLocalMessage($"Feature is now {(featureEnabled ? "enabled" : "disabled")}");
    }
}
```

Register it with an instance:

```csharp
private void Awake()
{
    ChatCommandApi.RegisterLocalChatCommand(new ToggleFeatureCommand(this));
}
```

## Command Prefixes

The prefix is what appears before the command keyword. Common prefixes include:

- `/` - Most common, e.g., `/help`, `/clear`
- `!` - Sometimes used for special commands
- Custom prefixes - You can use any string, but `/` is the standard

**Note:** The prefix is combined with the command to form the full command. For example, prefix `/` + command `help` = `/help`.

## Best Practices

### 1. Provide Clear Descriptions

Always provide a clear, concise description. This description appears when users run the built-in `/help` command:

```csharp
public string Description => "Teleports to the specified coordinates";
```

### 2. Validate Arguments

Always validate user input to provide helpful error messages:

```csharp
public void Handle(string arguments)
{
    if (string.IsNullOrWhiteSpace(arguments))
    {
        ChatApi.AddLocalMessage("Usage: /command <required_argument>");
        return;
    }
    
    // Process arguments...
}
```

### 3. Use Appropriate Command Names

Choose command names that are:
- Short and memorable
- Descriptive of their function
- Not conflicting with existing commands

### 4. Handle Errors Gracefully

Wrap your command logic in try-catch blocks if it might throw exceptions:

```csharp
public void Handle(string arguments)
{
    try
    {
        // Your command logic
    }
    catch (Exception ex)
    {
        ChatApi.AddLocalMessage($"Error: {ex.Message}");
        Logger.LogError($"Command error: {ex}");
    }
}
```

### 5. Provide Usage Information

For commands that take arguments, show usage information when called incorrectly:

```csharp
public void Handle(string arguments)
{
    if (string.IsNullOrWhiteSpace(arguments))
    {
        ChatApi.AddLocalMessage("Usage: /mycommand <option1> <option2>");
        ChatApi.AddLocalMessage("Options: list, add, remove");
        return;
    }
}
```

## Complete Example

Here's a complete example that demonstrates a practical use case:

```csharp
using BepInEx;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using UnityEngine;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private void Awake()
    {
        // Register multiple commands
        ChatCommandApi.RegisterLocalChatCommand(
            "/", "speed", "Sets player speed multiplier: /speed <multiplier>",
            OnSpeedCommand
        );
        
        ChatCommandApi.RegisterLocalChatCommand(
            "/", "reset", "Resets player speed to normal",
            OnResetCommand
        );
    }

    private void OnSpeedCommand(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            ChatApi.AddLocalMessage("Usage: /speed <multiplier>");
            ChatApi.AddLocalMessage("Example: /speed 2.5");
            return;
        }

        if (float.TryParse(arguments, out float multiplier) && multiplier > 0)
        {
            // Apply speed multiplier logic here
            ChatApi.AddLocalMessage($"Speed set to {multiplier}x");
        }
        else
        {
            ChatApi.AddLocalMessage("Invalid multiplier. Please use a positive number.");
        }
    }

    private void OnResetCommand(string arguments)
    {
        // Reset speed logic here
        ChatApi.AddLocalMessage("Speed reset to normal");
    }
}
```

## Unregistering Commands

If you need to unregister a command (e.g., when your plugin is disabled), you can use the unregister method:

```csharp
private ILocalChatCommand myCommand;

private void Awake()
{
    myCommand = new MyCommand();
    ChatCommandApi.RegisterLocalChatCommand(myCommand);
}

private void OnDestroy()
{
    ChatCommandApi.UnregisterLocalChatCommand(myCommand);
}
```

## Summary

- **Callback Delegate Method**: Best for simple, one-off commands
- **Interface Implementation Method**: Best for complex commands, reusable logic, or when you need to maintain state
- Always provide clear descriptions and validate user input
- Use appropriate prefixes (typically `/`)
- Handle errors gracefully and provide helpful usage messages

Local chat commands are a powerful way to make your mod's functionality easily accessible to users. Choose the method that best fits your needs and follow the best practices to create a great user experience.
