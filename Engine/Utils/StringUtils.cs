using System;
using System.Linq;
using System.Text;

namespace Engine.Utils;

public static class StringUtils
{
    public static string FormatGrid(string[,] arr, int width = 20) =>
        string.Join(
            Environment.NewLine,
            Enumerable
                .Range(0, arr.GetLength(0))
                .Select(i =>
                    string.Join(
                        " | ",
                        Enumerable
                            .Range(0, arr.GetLength(1))
                            .Select(j =>
                            {
                                var c = arr[i, j] ?? "";
                                return c.Length > width ? c[..width] : c.PadRight(width);
                            })
                    )
                )
        );
}
