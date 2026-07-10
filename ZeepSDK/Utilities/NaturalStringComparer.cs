using System.Collections.Generic;

namespace ZeepSDK.Utilities;

internal sealed class NaturalStringComparer : IComparer<string>
{
    public static NaturalStringComparer Instance { get; } = new();

    private NaturalStringComparer()
    {
    }

    public int Compare(string x, string y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (x == null)
            return -1;

        if (y == null)
            return 1;

        int ix = 0;
        int iy = 0;

        while (ix < x.Length && iy < y.Length)
        {
            char cx = x[ix];
            char cy = y[iy];

            if (char.IsDigit(cx) && char.IsDigit(cy))
            {
                long nx = ReadNumber(x, ref ix);
                long ny = ReadNumber(y, ref iy);
                int numberComparison = nx.CompareTo(ny);
                if (numberComparison != 0)
                    return numberComparison;

                continue;
            }

            if (cx != cy)
                return cx.CompareTo(cy);

            ix++;
            iy++;
        }

        return x.Length.CompareTo(y.Length);
    }

    private static long ReadNumber(string value, ref int index)
    {
        long number = 0;

        while (index < value.Length && char.IsDigit(value[index]))
        {
            number = (number * 10) + (value[index] - '0');
            index++;
        }

        return number;
    }
}
