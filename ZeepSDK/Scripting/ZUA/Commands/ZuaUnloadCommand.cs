using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

namespace ZeepSDK.Scripting.ZUA.Commands;

internal class ZuaUnloadCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "zua unload";
    public string Description => "Unloads a script and unsubscribes from any events";
    public void Handle(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            ChatApi.AddLocalMessage("Please specify a script name");
            return;
        }
        
        ScriptingApi.UnloadLuaByName(arguments.Trim());
    }
}
