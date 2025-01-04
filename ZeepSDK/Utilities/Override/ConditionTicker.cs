using System.Collections.Generic;
using UnityEngine;

namespace ZeepSDK.Utilities.Override;

public class ConditionTicker : MonoBehaviour
{
    private readonly List<IConditionTickable> _update = [];
    private readonly List<IConditionTickable> _fixedUpdate = [];
    private readonly List<IConditionTickable> _lateUpdate = [];
    private readonly List<IConditionTickable> _onGUI = [];
    
    private static ConditionTicker _instance;

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
