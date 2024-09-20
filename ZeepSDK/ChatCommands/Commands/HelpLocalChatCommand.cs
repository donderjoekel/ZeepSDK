using System.Collections.Generic;
using System.Linq;
using ZeepSDK.Chat;
using ZeepSDK.Extensions;

namespace ZeepSDK.ChatCommands.Commands;

internal class HelpLocalChatCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "help";
    public string Description => "Shows all locally available commands";
    public string[] Aliases => [];
    public string[] Arguments => [];

    public void Handle(string arguments)
    {
        ChatApi.AddLocalMessage("For available commands use F1, then spacebar to toggle through command pages");
    }
}
