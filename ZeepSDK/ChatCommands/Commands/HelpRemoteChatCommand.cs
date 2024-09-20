using ZeepSDK.Chat;

namespace ZeepSDK.ChatCommands.Commands;

internal class HelpRemoteChatCommand : IRemoteChatCommand
{
    public string Prefix => "!";
    public string Command => "help";
    public string Description => "Shows all remotely available commands";
    public string[] Aliases => [];
    public string[] Arguments => [];

    public void Handle(ulong playerId, string arguments)
    {
        ChatApi.SendMessage("Available commands:");

        foreach (IRemoteChatCommand remoteChatCommand in ChatCommandRegistry.RemoteChatCommands)
        {
            ChatApi.SendMessage(
                $"- {remoteChatCommand.Prefix}{remoteChatCommand.Command} - {remoteChatCommand.Description}");
        }
    }
}
