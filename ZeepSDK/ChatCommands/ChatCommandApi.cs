using System;
using System.Linq;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.ChatCommands.Commands;
using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands;

/// <summary>
/// An API that allows you to register chat commands
/// </summary>
[PublicAPI]
public static class ChatCommandApi
{
    private static ManualLogSource logger = LoggerFactory.GetLogger(typeof(ChatCommandApi));
    private static RemoteChatMessageHandler remoteChatMessageHandler;

    internal static void Initialize(GameObject gameObject)
    {
        Shutdown();
        remoteChatMessageHandler = gameObject.AddComponent<RemoteChatMessageHandler>();

        RegisterLocalChatCommand<HelpLocalChatCommand>();
        RegisterLocalChatCommand<ClearChatLocalChatCommand>();

        RegisterRemoteChatCommand<HelpRemoteChatCommand>();
    }

    internal static void Shutdown()
    {
        remoteChatMessageHandler?.Dispose();
        remoteChatMessageHandler = null;
    }

    /// <summary>
    /// Allows you to register a local chat command
    /// </summary>
    /// <param name="prefix">The prefix to use for the command</param>
    /// <param name="command">The keyword for the command</param>
    /// <param name="description">The description for the command</param>
    /// <param name="callback">The callback to invoke whenever the command gets used</param>
    public static void RegisterLocalChatCommand(
        string prefix,
        string command,
        string description,
        LocalChatCommandCallbackDelegate callback
    )
    {
        try
        {
            ChatCommandRegistry.RegisterLocalChatCommand(
                new LocalChatCommandWrapper(
                    prefix,
                    command,
                    description,
                    callback));
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterLocalChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a local chat command
    /// </summary>
    /// <typeparam name="TChatCommand">The type of the chat command to create a new instance of</typeparam>
    public static void RegisterLocalChatCommand<TChatCommand>()
        where TChatCommand : ILocalChatCommand, new()
    {
        try
        {
            ChatCommandRegistry.RegisterLocalChatCommand(new TChatCommand());
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterLocalChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a local chat command
    /// </summary>
    /// <param name="command">The command to register</param>
    public static void RegisterLocalChatCommand<TChatCommand>(TChatCommand command)
        where TChatCommand : ILocalChatCommand, new()
    {
        try
        {
            ChatCommandRegistry.RegisterLocalChatCommand(command);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterLocalChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a remote chat command
    /// </summary>
    /// <param name="prefix">The prefix for the command. Note: this should not start with a / as this does not work</param>
    /// <param name="command">The keyword for the command</param>
    /// <param name="description">The description for the command</param>
    /// <param name="callback">The callback to invoke whenever the command gets used</param>
    public static void RegisterRemoteChatCommand(
        string prefix,
        string command,
        string description,
        RemoteChatCommandCallbackDelegate callback
    )
    {
        try
        {
            ChatCommandRegistry.RegisterRemoteChatCommand(
                new RemoteChatCommandWrapper(
                    prefix,
                    command,
                    description,
                    callback));
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterRemoteChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a remote chat command
    /// </summary>
    /// <typeparam name="TChatCommand">The type of the chat command to create a new instance of</typeparam>
    public static void RegisterRemoteChatCommand<TChatCommand>()
        where TChatCommand : IRemoteChatCommand, new()
    {
        try
        {
            ChatCommandRegistry.RegisterRemoteChatCommand(new TChatCommand());
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterRemoteChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a remote chat command
    /// </summary>
    /// <param name="command">The command to register</param>
    public static void RegisterRemoteChatCommand<TChatCommand>(TChatCommand command)
        where TChatCommand : IRemoteChatCommand, new()
    {
        try
        {
            ChatCommandRegistry.RegisterRemoteChatCommand(command);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterRemoteChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a mixed chat command
    /// </summary>
    /// <param name="prefix">The prefix for the command. Note: this should not start with a / as this does not work</param>
    /// <param name="command">The keyword for the command</param>
    /// <param name="description">The description for the command</param>
    /// <param name="callback">The callback to invoke whenever the command gets used</param>
    public static void RegisterMixedChatCommand(
        string prefix,
        string command,
        string description,
        MixedChatCommandCallbackDelegate callback
    )
    {
        try
        {
            ChatCommandRegistry.RegisterMixedChatCommand(
                new MixedChatCommandWrapper(
                    prefix,
                    command,
                    description,
                    callback));
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterMixedChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a mixed chat command
    /// </summary>
    /// <typeparam name="TChatCommand">The type of the chat command to create a new instance of</typeparam>
    public static void RegisterMixedChatCommand<TChatCommand>()
        where TChatCommand : IMixedChatCommand, new()
    {
        try
        {
            ChatCommandRegistry.RegisterMixedChatCommand(new TChatCommand());
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterMixedChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to register a mixed chat command
    /// </summary>
    /// <param name="command">The command to register</param>
    public static void RegisterMixedChatCommand<TChatCommand>(TChatCommand command)
        where TChatCommand : IMixedChatCommand, new()
    {
        try
        {
            ChatCommandRegistry.RegisterMixedChatCommand(command);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterMixedChatCommand)}: " + e);
        }
    }

    /// <summary>
    /// Unregisters a local chat command and any aliases registered for it.
    /// </summary>
    /// <param name="command">The command to unregister</param>
    public static void UnregisterLocalChatCommand(ILocalChatCommand command)
    {
        ChatCommandRegistry.UnregisterAliasesFor(command);
        ChatCommandRegistry.UnregisterLocalChatCommand(command);
    }

    /// <summary>
    /// Unregisters a remote chat command
    /// </summary>
    /// <param name="command">The command to unregister</param>
    public static void UnregisterRemoteChatCommand(IRemoteChatCommand command)
    {
        ChatCommandRegistry.UnregisterRemoteChatCommand(command);
    }

    /// <summary>
    /// Unregisters a mixed chat command and any local aliases registered for it.
    /// </summary>
    /// <param name="command">The command to unregister</param>
    public static void UnregisterMixedChatCommand(IMixedChatCommand command)
    {
        ChatCommandRegistry.UnregisterAliasesFor(command);
        ChatCommandRegistry.UnregisterMixedChatCommand(command);
    }

    /// <summary>
    /// Registers an alias for an existing local chat command.
    /// </summary>
    /// <param name="command">The primary command to alias.</param>
    /// <param name="alias">The alias keyword. Uses the same prefix as the primary command.</param>
    public static void RegisterLocalChatCommandAlias(ILocalChatCommand command, string alias)
    {
        try
        {
            if (!ChatCommandRegistry.TryRegisterAlias(command, alias, out string error))
                logger.LogWarning($"Could not register alias '{alias}': {error}");
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterLocalChatCommandAlias)}: " + e);
        }
    }

    /// <summary>
    /// Registers multiple aliases for an existing local chat command.
    /// </summary>
    /// <param name="command">The primary command to alias.</param>
    /// <param name="aliases">The alias keywords. Each uses the same prefix as the primary command.</param>
    public static void RegisterLocalChatCommandAliases(ILocalChatCommand command, params string[] aliases)
    {
        foreach (string alias in aliases)
            RegisterLocalChatCommandAlias(command, alias);
    }

    /// <summary>
    /// Registers an alias for a registered local chat command identified by prefix and keyword.
    /// </summary>
    /// <param name="prefix">The prefix of the primary command.</param>
    /// <param name="command">The keyword of the primary command.</param>
    /// <param name="alias">The alias keyword.</param>
    public static void RegisterLocalChatCommandAlias(string prefix, string command, string alias)
    {
        try
        {
            if (!ChatCommandRegistry.TryFindPrimaryLocalCommand(prefix, command, out ILocalChatCommand target))
            {
                logger.LogWarning(
                    $"Could not register alias '{alias}': primary command '{prefix}{command}' was not found.");
                return;
            }

            RegisterLocalChatCommandAlias(target, alias);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterLocalChatCommandAlias)}: " + e);
        }
    }

    /// <summary>
    /// Registers multiple aliases for a registered local chat command identified by prefix and keyword.
    /// </summary>
    /// <param name="prefix">The prefix of the primary command.</param>
    /// <param name="command">The keyword of the primary command.</param>
    /// <param name="aliases">The alias keywords.</param>
    public static void RegisterLocalChatCommandAliases(string prefix, string command, params string[] aliases)
    {
        foreach (string alias in aliases)
            RegisterLocalChatCommandAlias(prefix, command, alias);
    }

    /// <summary>
    /// Registers an alias for the first registered primary local chat command of the given type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the primary command.</typeparam>
    /// <param name="alias">The alias keyword.</param>
    public static void RegisterLocalChatCommandAlias<TCommand>(string alias)
        where TCommand : ILocalChatCommand
    {
        try
        {
            ILocalChatCommand target = ChatCommandRegistry.GetPrimaryLocalChatCommands()
                .FirstOrDefault(command => command is TCommand);

            if (target == null)
            {
                logger.LogWarning(
                    $"Could not register alias '{alias}': no registered primary command of type '{typeof(TCommand).Name}' was found.");
                return;
            }

            RegisterLocalChatCommandAlias(target, alias);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RegisterLocalChatCommandAlias)}: " + e);
        }
    }

    /// <summary>
    /// Registers multiple aliases for the first registered primary local chat command of the given type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the primary command.</typeparam>
    /// <param name="aliases">The alias keywords.</param>
    public static void RegisterLocalChatCommandAliases<TCommand>(params string[] aliases)
        where TCommand : ILocalChatCommand
    {
        foreach (string alias in aliases)
            RegisterLocalChatCommandAlias<TCommand>(alias);
    }

    /// <summary>
    /// Unregisters a specific alias for a local chat command.
    /// </summary>
    /// <param name="command">The primary command the alias belongs to.</param>
    /// <param name="alias">The alias keyword to remove.</param>
    public static void UnregisterLocalChatCommandAlias(ILocalChatCommand command, string alias)
    {
        if (!ChatCommandRegistry.UnregisterAlias(command, alias))
        {
            logger.LogWarning(
                $"Could not unregister alias '{alias}': no matching alias was found for '{command.Prefix}{command.Command}'.");
        }
    }

    /// <summary>
    /// Unregisters a specific alias for a registered local chat command identified by prefix and keyword.
    /// </summary>
    /// <param name="prefix">The prefix of the primary command.</param>
    /// <param name="command">The keyword of the primary command.</param>
    /// <param name="alias">The alias keyword to remove.</param>
    public static void UnregisterLocalChatCommandAlias(string prefix, string command, string alias)
    {
        if (!ChatCommandRegistry.TryFindPrimaryLocalCommand(prefix, command, out ILocalChatCommand target))
        {
            logger.LogWarning(
                $"Could not unregister alias '{alias}': primary command '{prefix}{command}' was not found.");
            return;
        }

        UnregisterLocalChatCommandAlias(target, alias);
    }
}
