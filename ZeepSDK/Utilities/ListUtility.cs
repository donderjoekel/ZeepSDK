using System;
using System.Collections.Generic;

namespace ZeepSDK.Utilities;

/// <summary>
/// A class containing utility methods for lists
/// </summary>
public static class ListUtility
{
    /// <summary>
    /// Finds the first item in the list that satisfies the predicate, starting from the specified index
    /// </summary>
    /// <param name="list">The list to iterate</param>
    /// <param name="start">The index to start from</param>
    /// <param name="predicate">The predicate to validate the item</param>
    /// <typeparam name="T">The type of items that the list contains</typeparam>
    /// <returns>Either the first item that satisfies the condition or default</returns>
    public static T FindFirst<T>(IList<T> list, int start, Func<T, bool> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int index = (start + i) % list.Count;
            T item = list[index];

            if (predicate(item))
                return item;
        }

        return default;
    }

    /// <summary>
    /// Finds the first item in the list that satisfies the predicate, starting from the specified index in reverse
    /// </summary>
    /// <param name="list">The list to iterate</param>
    /// <param name="start">The index to start from</param>
    /// <param name="predicate">The predicate to validate the item</param>
    /// <typeparam name="T">The type of items that the list contains</typeparam>
    /// <returns>Either the first item that satisfies the condition or default</returns>
    public static T FindFirstReverse<T>(IList<T> list, int start, Func<T, bool> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int index = (start - i + list.Count) % list.Count;
            T item = list[index];

            if (predicate(item))
                return item;
        }

        return default;
    }
}
