using System;
using MoonSharp.Interpreter;

namespace ZeepSDK.Scripting.ZUA;

internal static class LuaExecutionBudget
{
    private const int InstructionsPerSlice = 10_000;
    private const int MaximumSlices = 100;

    public static DynValue Execute(Script script, DynValue function, params DynValue[] arguments)
    {
        if (script == null)
            throw new ArgumentNullException(nameof(script));
        if (function == null)
            throw new ArgumentNullException(nameof(function));

        DynValue coroutineHandle = script.CreateCoroutine(function);
        Coroutine coroutine = coroutineHandle.Coroutine;
        coroutine.AutoYieldCounter = InstructionsPerSlice;

        DynValue result = DynValue.Nil;
        for (int slice = 0; slice < MaximumSlices; slice++)
        {
            result = slice == 0 ? coroutine.Resume(arguments) : coroutine.Resume();
            if (coroutine.State == CoroutineState.Dead)
                return result;
        }

        throw new ScriptRuntimeException(
            $"Lua execution exceeded {InstructionsPerSlice * MaximumSlices:N0} instructions or yields.");
    }
}
