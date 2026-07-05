# Terminal Commands

ZeepSDK provides a local terminal window for prefix-free commands with shell-style arguments, flags, subcommands, autocomplete, and command history. It complements the existing [local chat commands](local-chat-commands.md) system and does not replace it.

## Opening the terminal

- Press the backtick key (`` ` ``) by default, or rebind it under **Zeep SDK → General → Toggle Terminal Key** in the mod settings window
- Use **File → Terminal** in the Zeep toolbar
- Call `TerminalApi.Open()`, `TerminalApi.Close()`, or `TerminalApi.Toggle()` from code

## Registering commands

### Callback delegate

```csharp
using ZeepSDK.Terminal;

TerminalApi.RegisterTerminalCommand(
    "teleport",
    "Teleport to coordinates",
    context =>
    {
        if (!context.TryGetArgument(0, out string x) ||
            !context.TryGetArgument(1, out string y) ||
            !context.TryGetArgument(2, out string z))
        {
            context.WriteError("Usage: teleport <x> <y> <z> [--force]");
            return;
        }

        context.WriteLine($"Teleporting to ({x}, {y}, {z})");
    },
    usage: "teleport <x> <y> <z> [--force]");
```

### Interface implementation

```csharp
public class TeleportCommand : ITerminalCommand
{
    public string Name => "teleport";
    public string Description => "Teleport to coordinates";
    public string Usage => "teleport <x> <y> <z> [--force]";

    public void Execute(TerminalCommandContext context)
    {
        // ...
    }
}

TerminalApi.RegisterTerminalCommand<TeleportCommand>();
```

## Subcommands

Register multi-word command names for nested commands:

```csharp
TerminalApi.RegisterTerminalCommand("rtm start", "Start RTM", ctx => { ... });
TerminalApi.RegisterTerminalCommand("rtm stop", "Stop RTM", ctx => { ... });
```

Or use command groups:

```csharp
TerminalApi.RegisterTerminalCommandGroup("rtm", "RTM control", group =>
{
    group.Subcommand("stop", "Stop RTM", ctx => { ... });
    group.SubcommandGroup("start", "Start RTM", start =>
    {
        start.Subcommand("something", "Start with something", ctx =>
        {
            ctx.TryGetArgument(0, out string value);
        });
    });
});
```

Example invocations:

```
rtm start my-session --force
rtm start something else
rtm session create my-room --private
```

## Legacy chat commands

Existing local chat commands (including aliases) can be run from the terminal using their original prefix and name:

```
/help
/clear
/zua load my-script
```

Output from legacy commands appears in the terminal scrollback. Disable this behavior with the `Terminal Legacy Chat Commands` config entry or `TerminalApi.LegacyChatCommandsEnabled`.

## Built-in commands

| Command | Description |
|---|---|
| `help [page]` | Lists registered terminal commands |
| `clear` | Clears the terminal output |

## Autocomplete and history

- **Tab** completes the current token
- **Up/Down** navigates command history
- **Enter** executes the current line

Implement `ITerminalCommand.GetCompletions` to provide custom suggestions for arguments and flags.
