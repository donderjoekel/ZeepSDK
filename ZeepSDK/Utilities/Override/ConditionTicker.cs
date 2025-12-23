using System.Collections.Generic;
using UnityEngine;

namespace ZeepSDK.Utilities.Override;

/// <summary>
/// A singleton MonoBehaviour that manages and ticks <see cref="IConditionTickable"/> objects
/// according to their specified <see cref="ConditionTickType"/>.
/// Ticks are performed during Unity's Update, FixedUpdate, LateUpdate, or OnGUI lifecycle methods.
/// </summary>
public class ConditionTicker : MonoBehaviour
{
    private readonly List<IConditionTickable> _update = [];
    private readonly List<IConditionTickable> _fixedUpdate = [];
    private readonly List<IConditionTickable> _lateUpdate = [];
    private readonly List<IConditionTickable> _onGUI = [];
    
    private static ConditionTicker _instance;

    /// <summary>
    /// Gets the singleton instance of the <see cref="ConditionTicker"/>.
    /// Creates a new instance if one doesn't exist.
    /// </summary>
    public static ConditionTicker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("ConditionTicker").AddComponent<ConditionTicker>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds a condition tickable to the appropriate tick list based on its <see cref="ConditionTickType"/>.
    /// </summary>
    /// <param name="tickable">The condition tickable to add.</param>
    public void Add(IConditionTickable tickable)
    {
        switch (tickable.TickType)
        {
            case ConditionTickType.Update:
                _update.Add(tickable);
                break;
            case ConditionTickType.FixedUpdate:
                _fixedUpdate.Add(tickable);
                break;
            case ConditionTickType.LateUpdate:
                _lateUpdate.Add(tickable);
                break;
            case ConditionTickType.OnGUI:
                _onGUI.Add(tickable);
                break;
        }
    }

    /// <summary>
    /// Removes a condition tickable from the appropriate tick list based on its <see cref="ConditionTickType"/>.
    /// </summary>
    /// <param name="tickable">The condition tickable to remove.</param>
    public void Remove(IConditionTickable tickable)
    {
        switch (tickable.TickType)
        {
            case ConditionTickType.Update:
                _update.Remove(tickable);
                break;
            case ConditionTickType.FixedUpdate:
                _fixedUpdate.Remove(tickable);
                break;
            case ConditionTickType.LateUpdate:
                _lateUpdate.Remove(tickable);
                break;
            case ConditionTickType.OnGUI:
                _onGUI.Remove(tickable);
                break;
        }
    }

    private void Update()
    {
        foreach (IConditionTickable tickable in _update)
        {
            tickable.Tick();
        }
    }
    
    private void FixedUpdate()
    {
        foreach (IConditionTickable tickable in _fixedUpdate)
        {
            tickable.Tick();
        }
    }
    
    private void LateUpdate()
    {
        foreach (IConditionTickable tickable in _lateUpdate)
        {
            tickable.Tick();
        }
    }
    
    private void OnGUI()
    {
        foreach (IConditionTickable tickable in _onGUI)
        {
            tickable.Tick();
        }
    }
}
