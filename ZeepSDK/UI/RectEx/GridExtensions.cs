using System.Linq;
using Imui.Core;

namespace ZeepSDK.UI.RectEx;

/// <summary>Provides rectangular grid layout helpers.</summary>
public static class GridExtensions
{
    private const float SPACE = 2f;

    /// <summary>Splits a rectangle into a grid with uniform spacing.</summary>
    public static ImRect[,] Grid(this ImRect rect, int rows, int columns, float space = SPACE)
    {
        return Grid(rect, rows, columns, space, space);
    }

    /// <summary>Splits a rectangle into a grid with independent row and column spacing.</summary>
    public static ImRect[,] Grid(this ImRect rect, int rows, int columns, float spaceBetweenRows, float spaceBetweenColumns)
    {
        ImRect[][] grid = RowExtensions.Row(rect, rows, spaceBetweenRows)
            .Select(x => ColumnExtensions.Column(x, columns, spaceBetweenColumns))
            .ToArray();

        ImRect[,] result = new ImRect[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                result[row, column] = grid[row][column];
            }
        }

        return result;
    }

    /// <summary>Splits a rectangle into a square grid.</summary>
    public static ImRect[,] Grid(this ImRect rect, int size, float space = SPACE)
    {
        return Grid(rect, size, size, space, space);
    }
}
