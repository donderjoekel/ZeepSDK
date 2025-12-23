using System.Linq;
using ZeepSDK.Utilities;
using ZeepSDK.Utilities.Override;

namespace ZeepSDK.Controls;

/// <summary>
/// Represents an override stack for managing input map states.
/// Allows multiple layers of overrides to control whether input maps in a specific category are enabled or disabled.
/// </summary>
public class InputOverrideStack : OverrideStack<bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputOverrideStack"/> class for the specified input map category.
    /// </summary>
    /// <param name="key">The category key that identifies which input maps to control (e.g., "Default", "Gameplay", "Menu").</param>
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