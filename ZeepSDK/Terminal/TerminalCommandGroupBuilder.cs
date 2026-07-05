using System;
using System.Collections.Generic;
using System.Text;

namespace ZeepSDK.Terminal;

/// <summary>
/// Builds a nested terminal command group.
/// </summary>
[JetBrains.Annotations.PublicAPI]
public sealed class TerminalCommandGroupBuilder
{
    private readonly string name;
    private readonly string description;
    private readonly List<TerminalCommandGroupBuilder> childGroups = new();
    private readonly List<(string Name, string Description, string Usage, TerminalCommandCallbackDelegate Callback)> leafCommands = new();

    internal TerminalCommandGroupBuilder(string name, string description)
    {
        this.name = name;
        this.description = description;
    }

    /// <summary>
    /// Registers a leaf subcommand.
    /// </summary>
    public void Subcommand(
        string subcommandName,
        string subcommandDescription,
        TerminalCommandCallbackDelegate callback,
        string usage = null)
    {
        leafCommands.Add(($"{name} {subcommandName}", subcommandDescription, usage, callback));
    }

    /// <summary>
    /// Registers a nested subcommand group.
    /// </summary>
    public void SubcommandGroup(
        string subcommandName,
        string subcommandDescription,
        Action<TerminalCommandGroupBuilder> configure)
    {
        var childBuilder = new TerminalCommandGroupBuilder($"{name} {subcommandName}", subcommandDescription);
        configure(childBuilder);
        childGroups.Add(childBuilder);
    }

    internal void Register()
    {
        foreach ((string commandName, string commandDescription, string usage, TerminalCommandCallbackDelegate callback) in leafCommands)
        {
            TerminalApi.RegisterTerminalCommand(commandName, commandDescription, callback, usage);
        }

        foreach (TerminalCommandGroupBuilder childGroup in childGroups)
            childGroup.Register();

        TerminalApi.RegisterTerminalCommand(name, description, PrintUsage);
    }

    private void PrintUsage(TerminalCommandContext context)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Usage for '{name}':");
        foreach ((string commandName, string commandDescription, _, _) in leafCommands)
        {
            builder.Append("  ");
            builder.Append(commandName);
            builder.Append(" - ");
            builder.AppendLine(commandDescription);
        }

        foreach (TerminalCommandGroupBuilder childGroup in childGroups)
        {
            builder.Append("  ");
            builder.Append(childGroup.name);
            builder.Append(" - ");
            builder.AppendLine(childGroup.description);
        }

        context.WriteLine(builder.ToString().TrimEnd());
    }
}
