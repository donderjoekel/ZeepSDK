using System.Collections.Generic;
using UnityEngine;

namespace ZeepSDK.Extensions;

/// <summary>
/// A class containing extensions for <see cref="Transform"/>
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// Gets all components of type <typeparamref name="TComponent"/> in the direct descendants of the transform.
    /// </summary>
    /// <param name="transform"></param>
    /// <typeparam name="TComponent"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TComponent> GetComponentsInDirectDescendants<TComponent>(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent(out TComponent component))
                yield return component;
        }
    }
}
