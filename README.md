# ZeepSDK

A community-built Software Development Kit that simplifies plugin development for **Zeepkist**. ZeepSDK provides a comprehensive set of high-level APIs that abstract away complex game internals, allowing developers to focus on creating innovative features and enhancements.

## Overview

ZeepSDK is built on [BepInEx](https://github.com/BepInEx/BepInEx), a Unity modding framework, and uses Harmony for runtime patching. By exposing well-designed APIs, ZeepSDK acts as a translation layer between mods and Zeepkist, making it easier to maintain compatibility and fix issues without requiring updates to individual mods.

## Features

ZeepSDK provides APIs for a wide range of functionality:

### Core APIs

- **Chat API** - Send messages to chat and interact with the chat system
- **Chat Commands API** - Create local and remote chat commands with built-in help system
- **Messaging API** - Display messages to users using the game's messenger system (info, success, warning, error)
- **Storage API** - Persistent data storage for your mods
- **Settings API** - Manage mod settings windows and configuration
- **UI API** - Create custom user interfaces, toolbars, and GUI elements
- **Controls API** - Override and manage input handling (keyboard and mouse)

### Gameplay APIs

- **Racing API** - Hook into racing events (finish line, crashes, camera changes, etc.)
- **Level API** - Interact with levels, including CSV level parsing and current level information
- **Multiplayer API** - Interact with multiplayer lobbies, playlists, and player events
- **Leaderboard API** - Create custom leaderboard pages that integrate seamlessly
- **Playlist API** - Manage and edit playlists saved to disk

### Editor APIs

- **Level Editor API** - Extend the level editor with custom blocks, folders, input blocking, and events
- **Photo Mode API** - Integrate with the game's photo mode system

### Advanced APIs

- **Scripting API** - Advanced Lua scripting capabilities (ZUA) for complex mods
- **Communication API** - Enable inter-mod communication through senders and receivers
- **Workshop API** - Subscribe to and unsubscribe from Steam Workshop items
- **Cosmetics API** - Access and manage player cosmetics (soapboxes, hats, etc.)
- **Crashlytics API** - Error reporting and crash tracking (optional, user consent required)

## Documentation

Comprehensive API documentation is available at:  
**https://donderjoekel.github.io/ZeepSDK/api/index.html**

## Built-in Features

### Chat Commands

ZeepSDK includes several built-in chat commands:

**Local Commands:**
- `/help` - Shows available local commands with descriptions
- `/clear` - Clears the chat

**Remote Commands:**
- `!help` - Shows available remote commands with descriptions (host only)

Remote commands can be used by other players in the same lobby, but only if you are currently the host. This is useful for features like vote skipping or lobby management.

## Getting Started

### Prerequisites

- Zeepkist game installed
- BepInEx installed and configured for Zeepkist
- .NET Framework 4.7.2 or compatible

### Installation

#### Installing BepInEx

Before installing ZeepSDK, you need to have BepInEx installed. The easiest way to install BepInEx for Zeepkist is using one of these community tools:

- **Windows**: [Modkist Revamped](https://github.com/donderjoekel/ModkistRevamped) - A modloader for Zeepkist using mod.io with a user-friendly interface
- **Linux**: [zeeper](https://codeberg.org/Vulpesx/zeeper) - A command-line tool for managing Zeepkist mods and installing BepInEx

Alternatively, you can manually install BepInEx by following the [official BepInEx documentation](https://docs.bepinex.dev/articles/user_guide/installation/index.html).

#### Installing ZeepSDK

You can install ZeepSDK using the same tools mentioned above for installing BepInEx:

- **Windows**: Use [Modkist Revamped](https://github.com/donderjoekel/ModkistRevamped) to install ZeepSDK through its mod browser
- **Linux**: Use [zeeper](https://codeberg.org/Vulpesx/zeeper) to install ZeepSDK via command line

Alternatively, you can install ZeepSDK manually:

1. Download the latest ZeepSDK release
2. Place the `ZeepSDK.dll` file in your `BepInEx/plugins` directory
3. Launch Zeepkist - ZeepSDK will load automatically

### Creating Your First Plugin

1. Create a new BepInEx plugin project
2. Install the `ZeepSDK` NuGet package in your project (recommended) instead of directly referencing `ZeepSDK.dll`
3. Use the ZeepSDK APIs in your plugin code

Example:

```csharp
using BepInEx;
using ZeepSDK.Chat;

[BepInPlugin("com.yourname.yourmod", "Your Mod", "1.0.0")]
public class YourMod : BaseUnityPlugin
{
    private void Awake()
    {
        ChatApi.SendMessage("Hello from my mod!");
    }
}
```

## Contributing

Contributions are welcome! ZeepSDK is a community project that benefits from the contributions of developers like you.

Please read the [Contributing Guide](CONTRIBUTING.md) for guidelines on:
- Feature development
- Bug fixes
- Code style
- Pull request process

### Key Contribution Guidelines

- Features should benefit multiple mods, not just one
- Use `Api` classes for outward-facing functionality
- Return interfaces instead of concrete implementations
- Include comprehensive XML documentation
- Test thoroughly and handle exceptions appropriately
- Follow semantic versioning for version bumps

## License

See [LICENSE](LICENSE) file for details.

## Powered By

[![Bugsnag](https://images.typeform.com/images/QKuaAssrFCq7/image/default-firstframe.png)](https://www.bugsnag.com)

ZeepSDK uses Bugsnag for error reporting and crash tracking to help improve stability and fix bugs. Users can opt-out of crashlytics in the configuration.
