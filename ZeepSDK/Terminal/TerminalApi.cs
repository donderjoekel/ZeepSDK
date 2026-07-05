using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.Terminal.BuiltIn;
using ZeepSDK.Terminal.UI;
using ZeepSDK.UI;
using ZeepSDK.Utilities;

namespace ZeepSDK.Terminal;

/// <summary>
/// Invoked whenever the terminal window opens.
/// </summary>
[PublicAPI]
public delegate void TerminalOpenedDelegate();

/// <summary>
/// Invoked whenever the terminal window closes.
/// </summary>
[PublicAPI]
public delegate void TerminalClosedDelegate();

/// <summary>
/// An API for registering and using the ZeepSDK terminal.
/// </summary>
[PublicAPI]
public static class TerminalApi
{
    private static readonly ManualLogSource Logger = LoggerFactory.GetLogger(typeof(TerminalApi));

    /// <summary>
    /// Invoked whenever the terminal window opens.
    /// </summary>
    public static event TerminalOpenedDelegate TerminalOpened;

    /// <summary>
    /// Invoked whenever the terminal window closes.
    /// </summary>
    public static event TerminalClosedDelegate TerminalClosed;

    private static ZeepTerminalDrawer terminalDrawer;
    private static ConfigEntry<KeyCode> toggleTerminalKey;
    private static ConfigEntry<bool> legacyChatCommandsEnabled;

    /// <summary>
    /// Whether legacy local chat commands can be executed from the terminal.
    /// </summary>
    public static bool LegacyChatCommandsEnabled => legacyChatCommandsEnabled?.Value ?? true;

    /// <summary>
    /// Whether the terminal window is currently open.
    /// </summary>
    public static bool IsOpen => terminalDrawer?.IsOpen ?? false;

    internal static void Initialize(GameObject gameObject)
    {
        toggleTerminalKey = Plugin.Instance.Config.Bind(
            "General",
            "Toggle Terminal Key",
            KeyCode.BackQuote,
            "The key to toggle the terminal window");

        legacyChatCommandsEnabled = Plugin.Instance.Config.Bind(
            "General",
            "Terminal Legacy Chat Commands",
            true,
            "Allow existing local chat commands to be executed from the terminal");

        RegisterTerminalCommand<HelpTerminalCommand>();
        RegisterTerminalCommand<ClearTerminalCommand>();

        terminalDrawer = new ZeepTerminalDrawer(toggleTerminalKey);
        UIApi.AddZeepGUIDrawer(terminalDrawer);
    }

    /// <summary>
    /// Registers a terminal command using a callback delegate.
    /// </summary>
    public static void RegisterTerminalCommand(
        string name,
        string description,
        TerminalCommandCallbackDelegate callback,
        string usage = null)
    {
        try
        {
            TerminalRegistry.Register(new TerminalCommandWrapper(name, description, usage, callback));
        }
        catch (Exception exception)
        {
            Logger.LogError($"Unhandled exception in {nameof(RegisterTerminalCommand)}: {exception}");
        }
    }

    /// <summary>
    /// Registers a terminal command by creating a new instance of the given type.
    /// </summary>
    public static void RegisterTerminalCommand<TTerminalCommand>()
        where TTerminalCommand : ITerminalCommand, new()
    {
        try
        {
            TerminalRegistry.Register(new TTerminalCommand());
        }
        catch (Exception exception)
        {
            Logger.LogError($"Unhandled exception in {nameof(RegisterTerminalCommand)}: {exception}");
        }
    }

    /// <summary>
    /// Registers a terminal command instance.
    /// </summary>
    public static void RegisterTerminalCommand<TTerminalCommand>(TTerminalCommand command)
        where TTerminalCommand : ITerminalCommand
    {
        try
        {
            TerminalRegistry.Register(command);
        }
        catch (Exception exception)
        {
            Logger.LogError($"Unhandled exception in {nameof(RegisterTerminalCommand)}: {exception}");
        }
    }

    /// <summary>
    /// Registers a nested terminal command group.
    /// </summary>
    public static void RegisterTerminalCommandGroup(
        string name,
        string description,
        Action<TerminalCommandGroupBuilder> configure)
    {
        try
        {
            var builder = new TerminalCommandGroupBuilder(name, description);
            configure(builder);
            builder.Register();
        }
        catch (Exception exception)
        {
            Logger.LogError($"Unhandled exception in {nameof(RegisterTerminalCommandGroup)}: {exception}");
        }
    }

    /// <summary>
    /// Unregisters a terminal command.
    /// </summary>
    public static void UnregisterTerminalCommand(ITerminalCommand command)
    {
        TerminalRegistry.Unregister(command);
    }

    /// <summary>
    /// Opens the terminal window.
    /// </summary>
    public static void Open() => terminalDrawer?.Open();

    /// <summary>
    /// Closes the terminal window.
    /// </summary>
    public static void Close() => terminalDrawer?.Close();

    /// <summary>
    /// Toggles the terminal window.
    /// </summary>
    public static void Toggle()
    {
        if (IsOpen)
            Close();
        else
            Open();
    }

    /// <summary>
    /// Executes a command line and returns the output lines produced.
    /// </summary>
    public static IReadOnlyList<TerminalOutputLine> Execute(string input)
    {
        var output = new TerminalOutputCollector();
        TerminalExecutor.TryExecute(input, output);
        return output.Lines.Select(TerminalOutputLine.FromInternal).ToList();
    }

    internal static void RequestClearOutput() => terminalDrawer?.ClearOutput();

    internal static void DispatchOpened() => TerminalOpened?.Invoke();

    internal static void DispatchClosed() => TerminalClosed?.Invoke();
}

/// <summary>
/// A line written to the terminal output.
/// </summary>
[PublicAPI]
public sealed class TerminalOutputLine
{
    /// <summary>
    /// The kind of output line.
    /// </summary>
    public TerminalOutputLineKind Kind { get; }

    /// <summary>
    /// The line text.
    /// </summary>
    public string Text { get; }

    internal TerminalOutputLine(TerminalOutputLineKind kind, string text)
    {
        Kind = kind;
        Text = text;
    }

    internal static TerminalOutputLine FromInternal(CollectedTerminalLine line) =>
        new(MapKind(line.Kind), line.Text);

    private static TerminalOutputLineKind MapKind(TerminalOutputKind kind) =>
        kind switch
        {
            TerminalOutputKind.Input => TerminalOutputLineKind.Input,
            TerminalOutputKind.Error => TerminalOutputLineKind.Error,
            _ => TerminalOutputLineKind.Output
        };
}

/// <summary>
/// The kind of terminal output line.
/// </summary>
[PublicAPI]
public enum TerminalOutputLineKind
{
    /// <summary>Input echo line.</summary>
    Input,
    /// <summary>Standard output line.</summary>
    Output,
    /// <summary>Error output line.</summary>
    Error
}
