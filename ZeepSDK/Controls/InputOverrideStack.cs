using System.Linq;
using ZeepSDK.Utilities;
using ZeepSDK.Utilities.Override;

namespace ZeepSDK.Controls;

public class InputOverrideStack : OverrideStack<bool>
{
    public InputOverrideStack(string key)
        : base(
            () => GetValue(key),
            value => SetValue(key, value),
            GetValue(key))
    {
    }

    private static bool GetValue(string key)
    {
        InputRegister inputRegister = ComponentCache.Get<InputRegister>();
        if (inputRegister == null)
            return false;

        return inputRegister.Inputs
            .First()
            .player.controllers.maps.GetAllMapsInCategory(key)
            .Any(x => x.enabled);
    }

    private static void SetValue(string key, bool value)
    {
        InputRegister inputRegister = ComponentCache.Get<InputRegister>();
        if (inputRegister == null)
            return;

        foreach (InputPlayerScriptableObject input in inputRegister.Inputs)
        {
            input.player.controllers.maps.SetMapsEnabled(value, key);
            input.ResetAllActions();
        }
    }
}