# Creating Remote Chat Commands

Remote chat commands are commands that can be executed by other players in a multiplayer lobby. These commands are only processed when you are the host (master client) of the lobby, making them perfect for lobby management features like vote skipping, kicking players, or changing game settings.

## Overview

Remote chat commands differ from local commands in several important ways:

- **Host-only**: Only work when you are the host of the lobby
- **Player identification**: Receive the Steam ID of the player who executed the command
- **Public messages**: Use `ChatApi.SendMessage()` to send messages visible to all players
- **Different prefix**: Typically use `!` instead of `/` (and should NOT start with `/`)

ZeepSDK provides two ways to create remote chat commands:

1. **Callback Delegate Method** - Simple and quick for basic commands
2. **Interface Implementation Method** - Better for more complex commands or when you need to reuse command logic

## Important Considerations

Before creating remote chat commands, keep in mind:

- **Host requirement**: Commands only work when you are the host. The system automatically checks this, so you don't need to verify it yourself.
- **Prefix restriction**: The prefix should **NOT** start with `/` as this does not work for remote commands. Use `!` or another prefix instead.
- **Player ID**: You receive the Steam ID (`ulong`) of the player who executed the command, allowing you to identify and interact with specific players.

## Method 1: Using Callback Delegates

The simplest way to create a remote chat command is using a callback delegate. This is perfect for simple commands that don't require complex logic.

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
        // Register a simple remote command
        ChatCommandApi.RegisterRemoteChatCommand(
            prefix: "!",
            command: "hello",
            description: "Says hello to everyone",
            callback: OnHelloCommand
        );
    }

    private void OnHelloCommand(ulong playerId, string arguments)
    {
        ChatApi.SendMessage($"Player {playerId} says hello!");
    }
}
```

When a player types `!hello` or `!hello world` in the chat, the `OnHelloCommand` method will be called with the player's Steam ID and the remaining text as arguments.

### Command with Player Identification

You can use the `playerId` parameter to identify and respond to specific players:

```csharp
using BepInEx;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepkistClient;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private void Awake()
    {
        ChatCommandApi.RegisterRemoteChatCommand(
            prefix: "!",
            command: "whoami",
            description: "Shows your player information",
            callback: OnWhoAmICommand
        );
    }

    private void OnWhoAmICommand(ulong playerId, string arguments)
    {
        // Find the player by their Steam ID
        ZeepkistPlayer player = ZeepkistNetwork.GetPlayerBySteamID(playerId);
        if (player != null)
        {
            string username = player.GetTaggedUsername();
            ChatApi.SendMessage($"{username} (ID: {playerId}) executed the command!");
        }
        else
        {
            ChatApi.SendMessage($"Player with ID {playerId} not found.");
        }
    }
}
```

### Vote Skip Example

Here's a practical example for implementing a vote skip feature:

```csharp
using System.Collections.Generic;
using BepInEx;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private readonly HashSet<ulong> votes = new HashSet<ulong>();
    private int requiredVotes = 3;

    private void Awake()
    {
        ChatCommandApi.RegisterRemoteChatCommand(
            prefix: "!",
            command: "skip",
            description: "Vote to skip the current level",
            callback: OnSkipCommand
        );
    }

    private void OnSkipCommand(ulong playerId, string arguments)
    {
        if (votes.Contains(playerId))
        {
            ChatApi.SendMessage("You have already voted to skip!");
            return;
        }

        votes.Add(playerId);
        int currentVotes = votes.Count;
        
        ChatApi.SendMessage($"Skip vote: {currentVotes}/{requiredVotes}");

        if (currentVotes >= requiredVotes)
        {
            ChatApi.SendMessage("Skip vote passed! Skipping level...");
            votes.Clear();
            // Implement level skip logic here
        }
    }
}
```

## Method 2: Using Interface Implementation

For more complex commands or when you want to organize your code better, you can implement the `IRemoteChatCommand` interface. This approach is also useful when you need to create multiple commands or want to reuse command logic.

### Basic Interface Implementation

```csharp
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

public class HelloCommand : IRemoteChatCommand
{
    public string Prefix => "!";
    public string Command => "hello";
    public string Description => "Says hello to everyone";

    public void Handle(ulong playerId, string arguments)
    {
        ChatApi.SendMessage($"Player {playerId} says hello!");
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
        ChatCommandApi.RegisterRemoteChatCommand<HelloCommand>();
        
        // Or register an instance
        ChatCommandApi.RegisterRemoteChatCommand(new HelloCommand());
    }
}
```

### Advanced Example: Command with State and Player Management

Interface implementation is particularly useful when your command needs to maintain state or access plugin data:

```csharp
using System.Collections.Generic;
using UnityEngine;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepkistClient;

public class KickCommand : IRemoteChatCommand
{
    private readonly MyMod plugin;
    private readonly HashSet<ulong> kickVotes = new HashSet<ulong>();

    public KickCommand(MyMod plugin)
    {
        this.plugin = plugin;
    }

    public string Prefix => "!";
    public string Command => "kick";
    public string Description => "Vote to kick a player: !kick <player_id>";

    public void Handle(ulong playerId, string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            ChatApi.SendMessage("Usage: !kick <player_id>");
            return;
        }

        if (!ulong.TryParse(arguments, out ulong targetPlayerId))
        {
            ChatApi.SendMessage("Invalid player ID. Please provide a valid Steam ID.");
            return;
        }

        // Prevent self-kick
        if (playerId == targetPlayerId)
        {
            ChatApi.SendMessage("You cannot vote to kick yourself!");
            return;
        }

        // Check if target player exists
        ZeepkistPlayer targetPlayer = ZeepkistNetwork.GetPlayerBySteamID(targetPlayerId);
        if (targetPlayer == null)
        {
            ChatApi.SendMessage("Player not found in this lobby.");
            return;
        }

        // Add vote
        string voteKey = $"{playerId}_{targetPlayerId}";
        if (kickVotes.Contains(playerId))
        {
            ChatApi.SendMessage("You have already voted to kick this player.");
            return;
        }

        kickVotes.Add(playerId);
        ChatApi.SendMessage($"Kick vote registered. Votes needed: {GetRequiredVotes()}");
        
        // Implement kick logic when enough votes are reached
    }

    private int GetRequiredVotes()
    {
        // Calculate required votes based on lobby size
        int playerCount = ZeepkistNetwork.Players.Count;
        return Mathf.Max(2, playerCount / 2);
    }
}
```

## Command Prefixes

The prefix is what appears before the command keyword. For remote commands:

- `!` - Most common for remote commands, e.g., `!help`, `!skip`
- Custom prefixes - You can use any string, but `!` is the standard
- **Important**: The prefix should **NOT** start with `/` as this does not work for remote commands

**Note:** The prefix is combined with the command to form the full command. For example, prefix `!` + command `help` = `!help`.

## Best Practices

### 1. Provide Clear Descriptions

Always provide a clear, concise description. This description appears when users run the built-in `!help` command:

```csharp
public string Description => "Vote to skip the current level";
```

### 2. Validate Arguments

Always validate user input to provide helpful error messages:

```csharp
public void Handle(ulong playerId, string arguments)
{
    if (string.IsNullOrWhiteSpace(arguments))
    {
        ChatApi.SendMessage("Usage: !command <required_argument>");
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
public void Handle(ulong playerId, string arguments)
{
    try
    {
        // Your command logic
    }
    catch (Exception ex)
    {
        ChatApi.SendMessage($"Error: {ex.Message}");
        Logger.LogError($"Command error: {ex}");
    }
}
```

### 5. Provide Usage Information

For commands that take arguments, show usage information when called incorrectly:

```csharp
public void Handle(ulong playerId, string arguments)
{
    if (string.IsNullOrWhiteSpace(arguments))
    {
        ChatApi.SendMessage("Usage: !mycommand <option1> <option2>");
        ChatApi.SendMessage("Options: list, add, remove");
        return;
    }
}
```

### 6. Verify Player Existence

When working with player IDs, always verify the player exists:

```csharp
ZeepkistPlayer player = ZeepkistNetwork.GetPlayerBySteamID(playerId);
if (player == null)
{
    ChatApi.SendMessage("Player not found in this lobby.");
    return;
}
```

### 7. Prevent Abuse

Implement safeguards to prevent command abuse:

```csharp
private readonly Dictionary<ulong, DateTime> lastCommandTime = new Dictionary<ulong, DateTime>();
private readonly TimeSpan cooldown = TimeSpan.FromSeconds(5);

public void Handle(ulong playerId, string arguments)
{
    if (lastCommandTime.TryGetValue(playerId, out DateTime lastTime))
    {
        if (DateTime.Now - lastTime < cooldown)
        {
            ChatApi.SendMessage("Please wait before using this command again.");
            return;
        }
    }
    
    lastCommandTime[playerId] = DateTime.Now;
    // Process command...
}
```

## Complete Example

Here's a complete example that demonstrates a practical use case - a lobby management system:

```csharp
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;
using ZeepkistClient;

[BepInPlugin("com.example.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    private readonly HashSet<ulong> skipVotes = new HashSet<ulong>();

    private void Awake()
    {
        // Register multiple remote commands
        ChatCommandApi.RegisterRemoteChatCommand(
            "!", "skip", "Vote to skip the current level",
            OnSkipCommand
        );
        
        ChatCommandApi.RegisterRemoteChatCommand(
            "!", "players", "Lists all players in the lobby",
            OnPlayersCommand
        );
    }

    private void OnSkipCommand(ulong playerId, string arguments)
    {
        if (skipVotes.Contains(playerId))
        {
            ChatApi.SendMessage("You have already voted to skip!");
            return;
        }

        skipVotes.Add(playerId);
        int playerCount = ZeepkistNetwork.Players.Count;
        int requiredVotes = Mathf.Max(2, playerCount / 2);
        
        ChatApi.SendMessage($"Skip vote: {skipVotes.Count}/{requiredVotes}");

        if (skipVotes.Count >= requiredVotes)
        {
            ChatApi.SendMessage("Skip vote passed! Skipping level...");
            skipVotes.Clear();
            // Implement level skip logic here
        }
    }

    private void OnPlayersCommand(ulong playerId, string arguments)
    {
        ChatApi.SendMessage($"Players in lobby ({ZeepkistNetwork.Players.Count}):");
        foreach (ZeepkistPlayer player in ZeepkistNetwork.Players)
        {
            string username = player.GetTaggedUsername();
            string hostIndicator = ZeepkistNetwork.IsMasterClient ? " [HOST]" : "";
            ChatApi.SendMessage($"- {username} (ID: {player.SteamID}){hostIndicator}");
        }
    }
}
```

## Unregistering Commands

If you need to unregister a command (e.g., when your plugin is disabled), you can use the unregister method:

```csharp
private IRemoteChatCommand myCommand;

private void Awake()
{
    myCommand = new MyCommand();
    ChatCommandApi.RegisterRemoteChatCommand(myCommand);
}

private void OnDestroy()
{
    ChatCommandApi.UnregisterRemoteChatCommand(myCommand);
}
```

## Differences from Local Commands

| Feature | Local Commands | Remote Commands |
|---------|---------------|-----------------|
| **Prefix** | `/` (can use others) | `!` (should NOT use `/`) |
| **Who can use** | Only the local player | Other players in lobby |
| **Host requirement** | No | Yes (only host processes) |
| **Player ID** | Not provided | Provided as parameter |
| **Message method** | `ChatApi.AddLocalMessage()` | `ChatApi.SendMessage()` |
| **Use case** | Personal utilities | Lobby management, multiplayer features |

## Summary

- **Callback Delegate Method**: Best for simple, one-off commands
- **Interface Implementation Method**: Best for complex commands, reusable logic, or when you need to maintain state
- **Host-only**: Commands only work when you are the host
- **Player identification**: Always receive the Steam ID of the command sender
- **Prefix restriction**: Do not use `/` as prefix - use `!` or another prefix
- Always provide clear descriptions and validate user input
- Handle errors gracefully and provide helpful usage messages
- Implement safeguards to prevent command abuse

Remote chat commands are a powerful way to add multiplayer functionality to your mods, enabling features like voting systems, lobby management, and player interactions. Choose the method that best fits your needs and follow the best practices to create a great user experience.
