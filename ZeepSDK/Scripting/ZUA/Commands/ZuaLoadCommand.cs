using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

namespace ZeepSDK.Scripting.ZUA.Commands;

internal class ZuaLoadCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "zua load";
    public string Description => "Loads a script from the plugins folder by name";
    public void Handle(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            ChatApi.AddLocalMessage("Please specify a script name");
            return;
        }

        ScriptingApi.LoadLuaByName(arguments.Trim());
    }
}
