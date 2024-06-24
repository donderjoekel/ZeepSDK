using System.Collections.Generic;
using UnityEngine;

namespace ZeepSDK.Utilities;

internal class Vector3Comparer : IComparer<Vector3>
{
    public int Compare(Vector3 v1, Vector3 v2)
    {
        int xComparison = v1.x.CompareTo(v2.x);
        if (xComparison != 0)
            return xComparison;

        int yComparison = v1.y.CompareTo(v2.y);
        if (yComparison != 0)
            return yComparison;

        return v1.z.CompareTo(v2.z);
    }
}
