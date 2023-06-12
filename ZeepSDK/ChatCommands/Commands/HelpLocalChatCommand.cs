using UnityEngine;

namespace ZeepSDK.ChatCommands.Commands;

internal class HelpLocalChatCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "help";
    public string Description => "Shows all locally available commands";

    public void Handle(string arguments)
    {
        OnlineChatUI onlineChatUi = Object.FindObjectOfType<OnlineChatUI>(true);
        onlineChatUi.UpdateChatFields("Available commands:", 0);
        foreach (ILocalChatCommand localChatCommand in ChatCommandRegistry.LocalChatCommands)
        {
            onlineChatUi.UpdateChatFields(
                $"{localChatCommand.Prefix}{localChatCommand.Command} - {localChatCommand.Description}",
                0);
        }
    }
}
