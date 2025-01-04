namespace ZeepSDK.Utilities.Override;

public interface IConditionTickable
{
    ConditionTickType TickType { get; }
    void Tick();
}
