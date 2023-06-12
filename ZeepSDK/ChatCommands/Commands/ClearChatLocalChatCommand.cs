using UnityEngine;

namespace ZeepSDK.ChatCommands.Commands;

internal class ClearChatLocalChatCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "clear";
    public string Description => "Clears the chat";

    public void Handle(string arguments)
    {
        OnlineChatUI onlineChatUi = Object.FindObjectOfType<OnlineChatUI>(true);
        onlineChatUi.ClearChat();
    }
}
