using System;
using System.Collections.Generic;
using Xunit;
using ZeepSDK.Utilities.Override;

namespace ZeepSDK.Tests;

public class ConditionTickSchedulerTests
{
    [Fact]
    public void DefersMutationUntilPhaseCompletes()
    {
        ConditionTickScheduler scheduler = new(null);
        List<string> calls = new();
        TestTickable added = new(() => calls.Add("added"));
        TestTickable removed = new(() => calls.Add("removed"));
        TestTickable mutator = new(() =>
        {
            calls.Add("mutator");
            scheduler.Remove(removed);
            scheduler.Add(added);
        });
        scheduler.Add(mutator);
        scheduler.Add(removed);

        scheduler.Tick(ConditionTickType.Update);
        Assert.Equal(new[] { "mutator", "removed" }, calls);

        calls.Clear();
        scheduler.Tick(ConditionTickType.Update);
        Assert.Equal(new[] { "mutator", "added" }, calls);
    }

    [Fact]
    public void IsolatesCallbackExceptions()
    {
        int errors = 0;
        int successfulTicks = 0;
        ConditionTickScheduler scheduler = new((_, _) => errors++);
        scheduler.Add(new TestTickable(() => throw new InvalidOperationException("failure")));
        scheduler.Add(new TestTickable(() => successfulTicks++));

        scheduler.Tick(ConditionTickType.Update);

        Assert.Equal(1, errors);
        Assert.Equal(1, successfulTicks);
    }

    private sealed class TestTickable : IConditionTickable
    {
        private readonly Action tick;

        public TestTickable(Action tick) => this.tick = tick;
        public ConditionTickType TickType => ConditionTickType.Update;
        public void Tick() => tick();
    }
}
