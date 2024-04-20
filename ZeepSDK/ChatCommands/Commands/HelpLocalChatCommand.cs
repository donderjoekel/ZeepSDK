using ZeepSDK.Chat;

namespace ZeepSDK.ChatCommands.Commands;

internal class HelpLocalChatCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "help";
    public string Description => "Shows all locally available commands";

    public void Handle(string arguments)
    {
        ChatApi.AddLocalMessage("Available commands:");
        foreach (ILocalChatCommand localChatCommand in ChatCommandRegistry.LocalChatCommands)
        {
            ChatApi.AddLocalMessage(
                $"- {localChatCommand.Prefix}{localChatCommand.Command} - {localChatCommand.Description}");
        }
    }
}
