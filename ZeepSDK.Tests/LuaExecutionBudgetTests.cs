using MoonSharp.Interpreter;
using Xunit;
using ZeepSDK.Scripting.ZUA;

namespace ZeepSDK.Tests;

public class LuaExecutionBudgetTests
{
    [Fact]
    public void CompletesNormalScript()
    {
        Script script = new(CoreModules.Preset_SoftSandbox);
        DynValue function = script.LoadString("return 40 + 2");

        DynValue result = LuaExecutionBudget.Execute(script, function);

        Assert.Equal(42, result.Number);
    }

    [Fact]
    public void TerminatesInfiniteLoop()
    {
        Script script = new(CoreModules.Preset_SoftSandbox);
        DynValue function = script.LoadString("while true do end");

        Assert.Throws<ScriptRuntimeException>(() => LuaExecutionBudget.Execute(script, function));
    }

    [Fact]
    public void TerminatesEndlessExplicitYields()
    {
        Script script = new(CoreModules.Preset_SoftSandbox);
        DynValue function = script.LoadString("while true do coroutine.yield() end");

        Assert.Throws<ScriptRuntimeException>(() => LuaExecutionBudget.Execute(script, function));
    }
}
