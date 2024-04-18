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
    public static event ChatMessageReceivedDelegate ChatMessageReceived;

    internal static void Initialize(GameObject gameObject)
    {
        ZeepkistNetwork.ChatMessageReceived += OnChatMessageReceived;
    }

    private static void OnChatMessageReceived(ZeepkistChatMessage zeepkistChatMessage)
    {
        try
        {
            if (zeepkistChatMessage == null)
                return;
            if (zeepkistChatMessage.Player == null)
                return;
            ChatMessageReceived?.Invoke(zeepkistChatMessage.Player.SteamID,
                zeepkistChatMessage.Player.GetTaggedUsername(),
                zeepkistChatMessage.Message);
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

    private static void SendChatMessagePacket(string content)
    {
        try
        {
            if (ZeepkistNetwork.NetworkClient == null)
                return;

            ZeepkistNetwork.NetworkClient.SendPacket(new ChatMessagePacket()
            {
                Message = content,
                Badges = new List<string>()
            });
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(SendServerMessage)}: " + e);
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
