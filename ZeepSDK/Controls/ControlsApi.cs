using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using ZeepSDK.Controls.Patches;
using ZeepSDK.Utilities;
using ZeepSDK.Utilities.Override;

namespace ZeepSDK.Controls;

/// <summary>
/// Provides APIs for controlling input and event system states across different game contexts.
/// Allows mods to override input maps and the Unity EventSystem for various game modes.
/// </summary>
[PublicAPI]
public static class ControlsApi
{
    private const string DefaultMapKey = "Default";
    private const string AdventureMapKey = "Adventure";
    private const string GameplayMapKey = "Gameplay";
    private const string LevelEditorMapKey = "LevelEditor";
    private const string MenuMapKey = "Menu";
    private const string OnlineMapKey = "Online";
    private const string SpectateMapKey = "Spectate";

    /// <summary>
    /// Gets the override stack for the Unity EventSystem.
    /// Allows mods to enable or disable the EventSystem, which controls UI input handling.
    /// </summary>
    public static OverrideStack<bool> EventSystemOverride { get; } = new(
        () =>
        {
            EventSystem eventSystem = ComponentCache.Get<EventSystem>(true);
            return eventSystem != null && eventSystem.enabled;

        },
        value =>
        {
            EventSystem eventSystem = ComponentCache.Get<EventSystem>(true);
            if (eventSystem != null)
                eventSystem.enabled = value;
        },
        true);
    
    /// <summary>
    /// Gets the override stack for the default input map.
    /// Allows mods to enable or disable default input handling.
    /// </summary>
    public static InputOverrideStack DefaultInputOverride { get; } = new(DefaultMapKey);
    
    /// <summary>
    /// Gets the override stack for the adventure input map.
    /// Allows mods to enable or disable adventure mode input handling.
    /// </summary>
    public static InputOverrideStack AdventureInputOverride { get; } = new(AdventureMapKey);
    
    /// <summary>
    /// Gets the override stack for the gameplay input map.
    /// Allows mods to enable or disable gameplay input handling.
    /// </summary>
    public static InputOverrideStack GameplayInputOverride { get; } = new(GameplayMapKey);
    
    /// <summary>
    /// Gets the override stack for the level editor input map.
    /// Allows mods to enable or disable level editor input handling.
    /// </summary>
    public static InputOverrideStack LevelEditorInputOverride { get; } = new(LevelEditorMapKey);
    
    /// <summary>
    /// Gets the override stack for the menu input map.
    /// Allows mods to enable or disable menu input handling.
    /// </summary>
    public static InputOverrideStack MenuInputOverride { get; } = new(MenuMapKey);
    
    /// <summary>
    /// Gets the override stack for the online input map.
    /// Allows mods to enable or disable online multiplayer input handling.
    /// </summary>
    public static InputOverrideStack OnlineInputOverride { get; } = new(OnlineMapKey);
    
    /// <summary>
    /// Gets the override stack for the spectate input map.
    /// Allows mods to enable or disable spectate mode input handling.
    /// </summary>
    public static InputOverrideStack SpectateInputOverride { get; } = new(SpectateMapKey);

    internal static void Initialize()
    {
        DefaultInputOverride.UpdateBaseValue(true);
        AdventureInputOverride.UpdateBaseValue(true);
        GameplayInputOverride.UpdateBaseValue(true);
        LevelEditorInputOverride.UpdateBaseValue(true);
        MenuInputOverride.UpdateBaseValue(true);
        OnlineInputOverride.UpdateBaseValue(true);
        SpectateInputOverride.UpdateBaseValue(true);
        EventSystemOverride.UpdateBaseValue(true);

        Input_DisableAllInput.Invoked += () =>
        {
            DefaultInputOverride.UpdateBaseValue(false);
            MenuInputOverride.UpdateBaseValue(false);
            OnlineInputOverride.UpdateBaseValue(false);
        };
        Input_EnableAllInput.Invoked += () =>
        {
            DefaultInputOverride.UpdateBaseValue(true);
            MenuInputOverride.UpdateBaseValue(true);
            OnlineInputOverride.UpdateBaseValue(true);
        };
        
        Input_DisableAdventureInput.Invoked += () =>
        {
            AdventureInputOverride.UpdateBaseValue(false);
        };
        Input_EnableAdventureInput.Invoked += () =>
        {
            AdventureInputOverride.UpdateBaseValue(true);
        };
        
        Input_DisableGameplayInput.Invoked += () =>
        {
            GameplayInputOverride.UpdateBaseValue(false);
        };
        Input_EnableGameplayInput.Invoked += () =>
        {
            GameplayInputOverride.UpdateBaseValue(true);
        };
        
        Input_DisableLevelEditorInput.Invoked += () =>
        {
            LevelEditorInputOverride.UpdateBaseValue(false);
        };
        Input_EnableLevelEditorInput.Invoked += () =>
        {
            LevelEditorInputOverride.UpdateBaseValue(true);
        };
        
        Input_DisableSpectateInput.Invoked += () =>
        {
            SpectateInputOverride.UpdateBaseValue(false);
        };
        Input_EnableSpectateInput.Invoked += () =>
        {
            SpectateInputOverride.UpdateBaseValue(true);
        };
    }

    /// <summary>
    /// Disables all input maps and the EventSystem.
    /// Returns a <see cref="DisposableBag"/> that, when disposed, restores the previous state.
    /// </summary>
    /// <returns>A disposable bag that restores input when disposed.</returns>
    public static DisposableBag DisableAllInput()
    {
        return new DisposableBag(
            AdventureInputOverride.Override(false),
            DefaultInputOverride.Override(false),
            GameplayInputOverride.Override(false),
            LevelEditorInputOverride.Override(false),
            MenuInputOverride.Override(false),
            OnlineInputOverride.Override(false),
            SpectateInputOverride.Override(false),
            EventSystemOverride.Override(false)
        );
    }
    
    /// <summary>
    /// Enables all input maps and the EventSystem.
    /// Returns a <see cref="DisposableBag"/> that, when disposed, restores the previous state.
    /// </summary>
    /// <returns>A disposable bag that restores input when disposed.</returns>
    public static DisposableBag EnableAllInput()
    {
        return new DisposableBag(
            AdventureInputOverride.Override(true),
            DefaultInputOverride.Override(true),
            GameplayInputOverride.Override(true),
            LevelEditorInputOverride.Override(true),
            MenuInputOverride.Override(true),
            OnlineInputOverride.Override(true),
            SpectateInputOverride.Override(true),
            EventSystemOverride.Override(true)
        );
    }

    /// <summary>
    /// Disables all input maps and the EventSystem while the specified condition is true.
    /// The condition is evaluated during OnGUI updates.
    /// Returns a <see cref="DisposableBag"/> that, when disposed, removes the conditional override.
    /// </summary>
    /// <param name="condition">The condition that determines when input should be disabled. Input is disabled when this returns true.</param>
    /// <returns>A disposable bag that removes the conditional override when disposed.</returns>
    public static DisposableBag DisableAllInput(Func<bool> condition)
    {
        return new DisposableBag(
            AdventureInputOverride.Override(false, condition, ConditionTickType.OnGUI),
            DefaultInputOverride.Override(false, condition, ConditionTickType.OnGUI),
            GameplayInputOverride.Override(false, condition, ConditionTickType.OnGUI),
            LevelEditorInputOverride.Override(false, condition, ConditionTickType.OnGUI),
            MenuInputOverride.Override(false, condition, ConditionTickType.OnGUI),
            OnlineInputOverride.Override(false, condition, ConditionTickType.OnGUI),
            SpectateInputOverride.Override(false, condition, ConditionTickType.OnGUI),
            EventSystemOverride.Override(false, condition, ConditionTickType.OnGUI)
        );
    }
    
    /// <summary>
    /// Enables all input maps and the EventSystem while the specified condition is true.
    /// The condition is evaluated during OnGUI updates.
    /// Returns a <see cref="DisposableBag"/> that, when disposed, removes the conditional override.
    /// </summary>
    /// <param name="condition">The condition that determines when input should be enabled. Input is enabled when this returns true.</param>
    /// <returns>A disposable bag that removes the conditional override when disposed.</returns>
    public static DisposableBag EnableAllInput(Func<bool> condition)
    {
        return new DisposableBag(
            AdventureInputOverride.Override(true, condition, ConditionTickType.OnGUI),
            DefaultInputOverride.Override(true, condition, ConditionTickType.OnGUI),
            GameplayInputOverride.Override(true, condition, ConditionTickType.OnGUI),
            LevelEditorInputOverride.Override(true, condition, ConditionTickType.OnGUI),
            MenuInputOverride.Override(true, condition, ConditionTickType.OnGUI),
            OnlineInputOverride.Override(true, condition, ConditionTickType.OnGUI),
            SpectateInputOverride.Override(true, condition, ConditionTickType.OnGUI),
            EventSystemOverride.Override(true, condition, ConditionTickType.OnGUI)
        );
    }
}
