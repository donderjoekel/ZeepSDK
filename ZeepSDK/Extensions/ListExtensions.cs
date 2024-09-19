using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ZeepSDK.Extensions;

/// <summary>
/// Extensions for lists
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Chunks a list into chunks of the given size
    /// </summary>
    [PublicAPI]
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
    {
        List<T> sourceList = source as List<T> ?? source.ToList();
        for (int index = 0; index < sourceList.Count; index += size)
        {
            yield return sourceList.Skip(index).Take(size);
        }
    }
}