using System;
using ZeepSDK.Extensions;

namespace ZeepSDK.LevelEditor;

internal abstract class CustomBlockCallback
{
    public abstract void Invoke();
}

internal class CustomBlockCallbackWithoutData : CustomBlockCallback
{
    private readonly Action _action;

    public CustomBlockCallbackWithoutData(Action action)
    {
        _action = action;
    }

    public override void Invoke()
    {
        _action.InvokeSafe();
    }
}

internal class CustomBlockCallbackWithData : CustomBlockCallback
{
    private readonly Action<object> _action;
    private readonly object _userData;

    public CustomBlockCallbackWithData(Action<object> action, object userData)
    {
        _action = action;
        _userData = userData;
    }

    public override void Invoke()
    {
        _action.InvokeSafe(_userData);
    }
}
