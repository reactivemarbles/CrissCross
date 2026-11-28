// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides operations for two-dimensional plotting data.</summary>
public static class DataOperations
{
    /// <summary>The factor used when reducing dimensions by half.</summary>
    private const int ReductionFactor = 2;

    /// <summary>The number of samples in each reduction block.</summary>
    private const int ReductionSampleCount = 4;

    /// <summary>Resizes a rectangular data array to half its original dimensions by averaging each block.</summary>
    /// <param name="values">The source values.</param>
    /// <returns>A new array with each dimension reduced by half.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "PerformanceSharp",
        "PSH1020",
        Justification = "Changing this public rectangular-array API to jagged arrays would break compatibility.")]
    public static double[, ] ResizeHalf(double[, ] values)
    {
        int height = values.GetLength(0);
        int width = values.GetLength(1);

        int heightNew = height / ReductionFactor;
        int widthNew = width / ReductionFactor;
        double[, ] output = new double[heightNew, widthNew];

        for (int y = 0; y < heightNew; y++)
        {
            for (int x = 0; x < widthNew; x++)
            {
                int sourceY = y * ReductionFactor;
                int sourceX = x * ReductionFactor;
                double sum = values[sourceY, sourceX]
                    + values[sourceY + 1, sourceX]
                    + values[sourceY, sourceX + 1]
                    + values[sourceY + 1, sourceX + 1];
                output[y, x] = sum / ReductionSampleCount;
            }
        }

        return output;
    }

    /// <summary>Replaces null values with <see cref="double.NaN"/>.</summary>
    /// <param name="values">The nullable source values.</param>
    /// <returns>A new rectangular array with nulls represented as NaN.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "PerformanceSharp",
        "PSH1020",
        Justification = "Changing this public rectangular-array API to jagged arrays would break compatibility.")]
    public static double[, ] ReplaceNullWithNaN(double?[, ] values)
    {
        int height = values.GetLength(0);
        int width = values.GetLength(1);
        double[, ] output = new double[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                output[y, x] = values[y, x] ?? double.NaN;
            }
        }

        return output;
    }

    /// <summary>Replaces NaN values with null.</summary>
    /// <param name="values">The source values.</param>
    /// <returns>A new nullable rectangular array with NaN values represented as null.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "PerformanceSharp",
        "PSH1020",
        Justification = "Changing this public rectangular-array API to jagged arrays would break compatibility.")]
    public static double?[, ] ReplaceNaNWithNull(double[, ] values)
    {
        int height = values.GetLength(0);
        int width = values.GetLength(1);
        double?[, ] output = new double?[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                output[y, x] = double.IsNaN(values[y, x]) ? null : values[y, x];
            }
        }

        return output;
    }

    /// <summary>Multiplies every value in a rectangular array by the specified factor.</summary>
    /// <param name="values">The values to modify in place.</param>
    /// <param name="mult">The multiplier.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "PerformanceSharp",
        "PSH1020",
        Justification = "Changing this public rectangular-array API to jagged arrays would break compatibility.")]
    public static void Multiply2D(double[, ] values, double mult)
    {
        for (int i = 0; i < values.GetLength(0); i++)
        {
            for (int j = 0; j < values.GetLength(1); j++)
            {
                values[i, j] *= mult;
            }
        }
    }

    /// <summary>Sums arrays element by element.</summary>
    /// <param name="arrays">The arrays to sum.</param>
    /// <returns>The element-wise sum.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="arrays"/> is null.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="arrays"/> is empty.</exception>
    /// <exception cref="InvalidDataException">The arrays have different lengths.</exception>
    public static double[] SumVertically(IEnumerable<double[]> arrays)
    {
        ArgumentChecks.ThrowIfNull(arrays, nameof(arrays));

        using IEnumerator<double[]> enumerator = arrays.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        double[] firstArray = enumerator.Current;
        double[] result = new double[firstArray.Length];
        AddValues(firstArray, result);

        while (enumerator.MoveNext())
        {
            AddValues(enumerator.Current, result);
        }

        return result;
    }

    /// <summary>Adds one array to the running element-wise sum.</summary>
    /// <param name="array">The array to add.</param>
    /// <param name="result">The running sum.</param>
    private static void AddValues(double[] array, double[] result)
    {
        if (array.Length != result.Length)
        {
            throw new InvalidDataException("All arrays must have equal length");
        }

        for (int i = 0; i < result.Length; i++)
        {
            result[i] += array[i];
        }
    }
}
