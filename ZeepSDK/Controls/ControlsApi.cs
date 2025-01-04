using System;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using ZeepSDK.Controls.Patches;
using ZeepSDK.Utilities;
using ZeepSDK.Utilities.Override;

namespace ZeepSDK.Controls;

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

    public static OverrideStack<bool> EventSystemOverride { get; } = new(
        () => EventSystem.current != null && EventSystem.current.enabled,
        value=>
        {
            if (EventSystem.current != null)
                EventSystem.current.enabled = value;
        },
        true);
    public static InputOverrideStack DefaultInputOverride { get; } = new(DefaultMapKey);
    public static InputOverrideStack AdventureInputOverride { get; } = new(AdventureMapKey);
    public static InputOverrideStack GameplayInputOverride { get; } = new(GameplayMapKey);
    public static InputOverrideStack LevelEditorInputOverride { get; } = new(LevelEditorMapKey);
    public static InputOverrideStack MenuInputOverride { get; } = new(MenuMapKey);
    public static InputOverrideStack OnlineInputOverride { get; } = new(OnlineMapKey);
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
