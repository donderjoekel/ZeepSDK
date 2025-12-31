# Welcome to ZeepSDK

**ZeepSDK** is a community-built Software Development Kit designed to simplify plugin development for **Zeepkist**. By providing a comprehensive set of APIs that abstract away complex implementation details, ZeepSDK enables developers to focus on creating innovative features and enhancements for the game.

## What is ZeepSDK?

ZeepSDK is a [BepInEx](https://github.com/BepInEx/BepInEx)-based framework that exposes a collection of well-designed APIs covering various aspects of Zeepkist gameplay and functionality. Instead of dealing with low-level game internals, developers can use these high-level APIs to quickly implement features like custom chat commands, leaderboard pages, level editor enhancements, racing events, and much more.

By exposing well-designed APIs, ZeepSDK acts as a translation layer between mods and Zeepkist, making it easier to maintain compatibility and fix issues without requiring updates to individual mods.

## Available APIs

ZeepSDK provides APIs organized into several categories:

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

## Getting Started

### Installation

Before using ZeepSDK, you need to have BepInEx installed. The easiest way to install BepInEx for Zeepkist is using one of these community tools:

- **Windows**: [Modkist Revamped](https://github.com/donderjoekel/ModkistRevamped) - A modloader for Zeepkist using mod.io with a user-friendly interface
- **Linux**: [zeeper](https://codeberg.org/Vulpesx/zeeper) - A command-line tool for managing Zeepkist mods and installing BepInEx

You can install ZeepSDK using the same tools, or manually by placing `ZeepSDK.dll` in your `BepInEx/plugins` directory.

### Development

When creating plugins, we recommend using the `ZeepSDK` NuGet package instead of directly referencing `ZeepSDK.dll`. This ensures you get the latest version and proper dependency management.

### Documentation

- Check out the [Articles](articles/) section for tutorials and guides
- Browse the [API Documentation](api/) for detailed reference on all available APIs
- Explore the codebase to see examples of how other developers use ZeepSDK

## Built-in Features

ZeepSDK includes several built-in chat commands:

**Local Commands:**
- `/help` - Shows available local commands with descriptions
- `/clear` - Clears the chat

**Remote Commands:**
- `!help` - Shows available remote commands with descriptions (host only)

Remote commands can be used by other players in the same lobby, but only if you are currently the host. This is useful for features like vote skipping or lobby management.

## Contributing

ZeepSDK is a community project. Contributions, bug reports, and feature requests are welcome! See the [Contributing Guide](../CONTRIBUTING.md) for more information.

---

*Happy modding! 🚀*
