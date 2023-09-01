using System;
using ZeepSDK.Extensions;

namespace ZeepSDK.LevelEditor;

internal abstract class CustomBlockCallback
{
    public abstract void Invoke();
}

internal class CustomBlockCallbackWithoutData : CustomBlockCallback
{
    private readonly Action action;

    public CustomBlockCallbackWithoutData(Action action)
    {
        this.action = action;
    }

    public override void Invoke()
    {
        action.InvokeSafe();
    }
}

internal class CustomBlockCallbackWithData : CustomBlockCallback
{
    private readonly Action<object> action;
    private readonly object userData;
    
    public CustomBlockCallbackWithData(Action<object> action, object userData)
    {
        this.action = action;
        this.userData = userData;
    }

    public override void Invoke()
    {
        action.InvokeSafe(userData);
    }
}
