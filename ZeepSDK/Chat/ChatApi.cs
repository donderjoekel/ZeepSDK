using System;
using System.Collections.Generic;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using ZeepkistClient;
using ZeepkistNetworking;
using ZeepSDK.Utilities;

namespace ZeepSDK.Chat;

/// <summary>
/// An API that allows you to send and receive chat messages
/// </summary>
[PublicAPI]
public static class ChatApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(ChatApi));

    private static OnlineChatUI OnlineChatUI => ComponentCache.Get<OnlineChatUI>();

    /// <summary>
    /// Event that is fired when a chat message is received
    /// </summary>
    public static event EventHandler<ChatMessageReceivedEventArgs> ChatMessageReceived;

    internal static void Initialize(GameObject gameObject)
    {
        ZeepkistNetwork.ChatMessageReceived += OnChatMessageReceived;
    }

    private static void OnChatMessageReceived(ZeepkistChatMessage zeepkistChatMessage)
    {
        try
        {
            if (zeepkistChatMessage?.Player == null)
            {
                return;
            }

            ChatMessageReceived?.Invoke(null,
                new ChatMessageReceivedEventArgs(zeepkistChatMessage.Player?.SteamID ?? 0,
                    zeepkistChatMessage.Player?.GetTaggedUsername() ?? string.Empty,
                    zeepkistChatMessage.Message));
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(OnChatMessageReceived)}: " + e);
        }
    }

    /// <summary>
    /// Adds a message to the local chat UI
    /// </summary>
    /// <param name="message">The message you want to add</param>
    public static void AddLocalMessage(string message)
    {
        try
        {
            OnlineChatUI onlineChatUi = OnlineChatUI;
            if (onlineChatUi != null)
            {
                onlineChatUi.UpdateChatFields(message, 0);
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddLocalMessage)}: " + e);
        }
    }

    /// <summary>
    /// Sends a message to the chat
    /// </summary>
    /// <param name="message">The message you wish to send</param>
    public static void SendMessage(string message)
    {
        SendChatMessagePacket(message);
    }

    /// <summary>
    /// Sends a server message
    /// </summary>
    /// <param name="message">The contents of the message</param>
    /// <param name="duration">The duration of the message in seconds</param>
    /// <param name="color">The color of the message</param>
    public static void SendServerMessage(string message, int duration, MessageColor color)
    {
        SendChatMessagePacket($"/servermessage {color.ToValidString()} {duration} {message}");
    }

    /// <summary>
    /// Sets the join message
    /// </summary>
    /// <param name="message">The contents of the message</param>
    /// <param name="color">The color of the message</param>
    public static void SetJoinMessage(string message, MessageColor color)
    {
        SendChatMessagePacket($"/joinmessage {color.ToValidString()} {message}");
    }

    /// <summary>
    /// Enables the join message
    /// </summary>
    public static void EnableJoinMessage()
    {
        SendChatMessagePacket("/joinmessage on");
    }

    /// <summary>
    /// Disables the join message
    /// </summary>
    public static void DisableJoinMessage()
    {
        SendChatMessagePacket("/joinmessage off");
    }

    private static void SendChatMessagePacket(string content)
    {
        try
        {
            if (ZeepkistNetwork.NetworkClient == null)
            {
                return;
            }

            ZeepkistNetwork.NetworkClient.SendPacket(new ChatMessagePacket() { Message = content, Badges = [] });
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(SendChatMessagePacket)}: " + e);
        }
    }

    /// <summary>
    /// Clears the chat window
    /// </summary>
    public static void ClearChat()
    {
        try
        {
            OnlineChatUI onlineChatUi = OnlineChatUI;
            if (onlineChatUi != null)
            {
                onlineChatUi.ClearChat();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(ClearChat)}: " + e);
        }
    }
}
