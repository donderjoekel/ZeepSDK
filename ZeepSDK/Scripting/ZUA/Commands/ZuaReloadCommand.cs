using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

namespace ZeepSDK.Scripting.ZUA.Commands;

internal class ZuaReloadCommand : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "zua reload";
    public string Description => "Reloads a script from the plugins folder by name";

    public void Handle(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            ChatApi.AddLocalMessage("Please specify a script name");
            return;
        }

        string args = arguments.Trim();
        ScriptingApi.UnloadLuaByName(args);
        ScriptingApi.LoadLuaByName(args);
    }
}
