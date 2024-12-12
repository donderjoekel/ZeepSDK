using System;

namespace ZeepSDK.Scripting.ZUA;

internal class ListenToLuaFunction : ILuaFunction
{
    private readonly Zua zua;
    
    public string Namespace => "Zua";
    public string Name => "ListenTo";

    public ListenToLuaFunction(Zua zua)
    {
        this.zua = zua;
    }
    
    public Delegate CreateFunction()
    {
        return new Action<string>(Implementation);
    }

    private void Implementation(string eventNameArg)
    {
        zua.ListenTo(eventNameArg);
    }
}
