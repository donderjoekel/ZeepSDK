using System.Collections.Generic;
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
        if (zeepkistChatMessage == null)
            return;
        if (zeepkistChatMessage.Player == null)
            return;
        ChatMessageReceived?.Invoke(zeepkistChatMessage.Player.SteamID,
            zeepkistChatMessage.Player.Username,
            zeepkistChatMessage.Message);
    }

    /// <summary>
    /// Adds a message to the local chat UI
    /// </summary>
    /// <param name="message">The message you want to add</param>
    public static void AddLocalMessage(string message)
    {
        OnlineChatUI onlineChatUi = OnlineChatUI;
        if (onlineChatUi != null)
        {
            onlineChatUi.UpdateChatFields(message, 0);
        }
    }

    /// <summary>
    /// Sends a message to the chat
    /// </summary>
    /// <param name="message">The message you wish to send</param>
    public static void SendMessage(string message)
    {
        if (ZeepkistNetwork.NetworkClient == null)
            return;

        ZeepkistNetwork.NetworkClient.SendPacket(new ChatMessagePacket()
        {
            Message = message,
            Badges = new List<string>()
        });
    }

    /// <summary>
    /// Clears the chat window
    /// </summary>
    public static void ClearChat()
    {
        OnlineChatUI onlineChatUi = OnlineChatUI;
        if (onlineChatUi != null)
        {
            onlineChatUi.ClearChat();
        }
    }
}
