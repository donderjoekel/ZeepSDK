using System;
using System.Collections.Generic;

namespace ZeepSDK.Utilities.Override;

internal sealed class ConditionTickScheduler
{
    private readonly Dictionary<ConditionTickType, List<IConditionTickable>> tickables = new()
    {
        [ConditionTickType.Update] = new List<IConditionTickable>(),
        [ConditionTickType.FixedUpdate] = new List<IConditionTickable>(),
        [ConditionTickType.LateUpdate] = new List<IConditionTickable>(),
        [ConditionTickType.OnGUI] = new List<IConditionTickable>()
    };
    private readonly List<PendingMutation> pendingMutations = new();
    private readonly Action<IConditionTickable, Exception> onError;
    private bool ticking;

    public ConditionTickScheduler(Action<IConditionTickable, Exception> onError) => this.onError = onError;

    public void Add(IConditionTickable tickable)
    {
        if (tickable == null)
            throw new ArgumentNullException(nameof(tickable));
        Mutate(new PendingMutation(true, tickable));
    }

    public void Remove(IConditionTickable tickable)
    {
        if (tickable != null)
            Mutate(new PendingMutation(false, tickable));
    }

    public void Tick(ConditionTickType tickType)
    {
        List<IConditionTickable> phase = GetPhase(tickType);
        ticking = true;
        try
        {
            for (int index = 0; index < phase.Count; index++)
            {
                IConditionTickable tickable = phase[index];
                try
                {
                    tickable.Tick();
                }
                catch (Exception exception)
                {
                    onError?.Invoke(tickable, exception);
                }
            }
        }
        finally
        {
            ticking = false;
            foreach (PendingMutation mutation in pendingMutations)
                Apply(mutation);
            pendingMutations.Clear();
        }
    }

    private void Mutate(PendingMutation mutation)
    {
        if (ticking)
            pendingMutations.Add(mutation);
        else
            Apply(mutation);
    }

    private void Apply(PendingMutation mutation)
    {
        if (mutation.Add)
        {
            List<IConditionTickable> phase = GetPhase(mutation.Tickable.TickType);
            if (!phase.Contains(mutation.Tickable))
                phase.Add(mutation.Tickable);
            return;
        }

        foreach (List<IConditionTickable> phase in tickables.Values)
            phase.Remove(mutation.Tickable);
    }

    private List<IConditionTickable> GetPhase(ConditionTickType tickType)
    {
        if (!tickables.TryGetValue(tickType, out List<IConditionTickable> phase))
            throw new ArgumentOutOfRangeException(nameof(tickType), tickType, "Unknown condition tick type.");
        return phase;
    }

    private readonly struct PendingMutation
    {
        public PendingMutation(bool add, IConditionTickable tickable)
        {
            Add = add;
            Tickable = tickable;
        }

        public bool Add { get; }
        public IConditionTickable Tickable { get; }
    }
}
