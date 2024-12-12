using System;
using JetBrains.Annotations;

namespace ZeepSDK.Scripting.ZUA;

[UsedImplicitly]
internal class ZuaLogger
{
    [UsedImplicitly]
    public class LogInfo : ILuaFunction
    {
        public string Namespace => "Zua";
        public string Name => "LogInfo";

        public Delegate CreateFunction()
        {
            return Execute;
        }

        private void Execute(string message)
        {
            Zua.Logger.LogInfo(message);
        }
    }

    [UsedImplicitly]
    public class LogWarning : ILuaFunction
    {
        public string Namespace => "Zua";
        public string Name => "LogWarning";

        public Delegate CreateFunction()
        {
            return Execute;
        }

        private void Execute(string message)
        {
            Zua.Logger.LogWarning(message);
        }
    }

    [UsedImplicitly]
    public class LogError : ILuaFunction
    {
        public string Namespace => "Zua";
        public string Name => "LogError";

        public Delegate CreateFunction()
        {
            return Execute;
        }

        private void Execute(string message)
        {
            Zua.Logger.LogError(message);
        }
    }
}
