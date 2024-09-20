using ZeepSDK.Chat;

namespace ZeepSDK.ChatCommands.Commands;

internal class ClearChatLocalChatCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "clear";
    public string Description => "Clears the chat";
    public string[] Aliases => [];
    public string[] Arguments => [];

    public void Handle(string arguments)
    {
        ChatApi.ClearChat();
    }
}
