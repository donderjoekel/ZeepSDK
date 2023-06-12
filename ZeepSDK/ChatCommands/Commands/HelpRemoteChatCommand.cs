using System.Collections.Generic;
using ZeepkistClient;
using ZeepkistNetworking;

namespace ZeepSDK.ChatCommands.Commands;

internal class HelpRemoteChatCommand : IRemoteChatCommand
{
    public string Prefix => "!";
    public string Command => "Help";
    public string Description => "Shows all remotely available commands";

    public void Handle(ulong playerId, string arguments)
    {
        ZeepkistNetwork.NetworkClient.SendPacket(new ChatMessagePacket()
        {
            Message = "Available commands:",
            Badges = new List<string>()
        });
        
        foreach (IRemoteChatCommand remoteChatCommand in ChatCommandRegistry.RemoteChatCommands)
        {
            ZeepkistNetwork.NetworkClient.SendPacket(new ChatMessagePacket()
            {
                Message = $"{remoteChatCommand.Prefix}{remoteChatCommand.Command} - {remoteChatCommand.Description}",
                Badges = new List<string>()
            });
        }
    }
}
