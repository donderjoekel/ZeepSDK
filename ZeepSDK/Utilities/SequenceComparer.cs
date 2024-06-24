using System.Collections.Generic;

namespace ZeepSDK.Utilities;

internal class FloatComparer : IComparer<List<float>>
{
    public int Compare(List<float> x, List<float> y)
    {
        if (x == null && y == null)
            return 0;

        if (x == null)
            return -1;

        if (y == null)
            return 1;

        int countComparison = x.Count.CompareTo(y.Count);
        if (countComparison != 0)
            return countComparison;

        for (int i = 0; i < x.Count; i++)
        {
            int elementComparison = x[i].CompareTo(y[i]);
            if (elementComparison != 0)
                return elementComparison;
        }

        return 0;
    }
}

internal class IntComparer : IComparer<List<int>>
{
    public int Compare(List<int> x, List<int> y)
    {
        if (x == null && y == null)
            return 0;

        if (x == null)
            return -1;

        if (y == null)
            return 1;

        int countComparison = x.Count.CompareTo(y.Count);
        if (countComparison != 0)
            return countComparison;

        for (int i = 0; i < x.Count; i++)
        {
            int elementComparison = x[i].CompareTo(y[i]);
            if (elementComparison != 0)
                return elementComparison;
        }

        return 0;
    }
}
