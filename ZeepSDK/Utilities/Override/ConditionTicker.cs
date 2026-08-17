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
    private static readonly BepInEx.Logging.ManualLogSource logger = LoggerFactory.GetLogger<ConditionTicker>();
    private readonly ConditionTickScheduler scheduler = new((tickable, exception) =>
        logger.LogError($"Condition tick failed for '{tickable.GetType().FullName}': {exception}"));
    
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
        scheduler.Add(tickable);
    }

    /// <summary>
    /// Removes a condition tickable from the appropriate tick list based on its <see cref="ConditionTickType"/>.
    /// </summary>
    /// <param name="tickable">The condition tickable to remove.</param>
    public void Remove(IConditionTickable tickable)
    {
        scheduler.Remove(tickable);
    }

    private void Update()
    {
        scheduler.Tick(ConditionTickType.Update);
    }
    
    private void FixedUpdate()
    {
        scheduler.Tick(ConditionTickType.FixedUpdate);
    }
    
    private void LateUpdate()
    {
        scheduler.Tick(ConditionTickType.LateUpdate);
    }
    
    private void OnGUI()
    {
        scheduler.Tick(ConditionTickType.OnGUI);
    }
}
