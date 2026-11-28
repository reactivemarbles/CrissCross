// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot.Statistics;

/// <summary>Provides descriptive statistics and array operations.</summary>
public static class Descriptive
{
    /// <summary>The denominator used to express percentages.</summary>
    private const double Percent = 100;

    /// <summary>The integer value one.</summary>
    private const int One = 1;

    /// <summary>The integer value two.</summary>
    private const int Two = 2;

    /* NOTES:
     *   The core functions in this class should support double[].
     *   Overloads may support <T> but they just convert to double[] then call the other function.
     *   IReadOnlyList is favored to IEnumerable to avoid Count().
     *   Code here may benefit from benchmarking to confirm which strategies are most performant.
     */

    /// <summary>Return the sample sum.</summary>
    /// <param name="values">The values to sum.</param>
    /// <returns>The sum of the values.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is empty.</exception>
    public static double Sum(double[] values)
    {
        if (values.Length == 0)
        {
            throw new ArgumentException($"{nameof(values)} cannot be empty");
        }

        double sum = 0;

        for (int i = 0; i < values.Length; i++)
        {
            sum += values[i];
        }

        return sum;
    }

    /// <summary>Return the sample sum.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values to sum.</param>
    /// <returns>The sum of the values.</returns>
    public static double Sum<T>(IReadOnlyList<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        return Sum(values2);
    }

    /// <summary>Return the sample mean.</summary>
    /// <param name="values">The values used to calculate the mean.</param>
    /// <returns>The arithmetic mean of the values.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is empty.</exception>
    public static double Mean(double[] values)
    {
        if (values.Length == 0)
        {
            throw new ArgumentException($"{nameof(values)} cannot be empty");
        }

        return Sum(values) / values.Length;
    }

    /// <summary>Return the sample mean.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the mean.</param>
    /// <returns>The arithmetic mean of the values.</returns>
    public static double Mean<T>(IEnumerable<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        return Mean(values2);
    }

    /// <summary>Return the sample median.</summary>
    /// <param name="values">The values used to calculate the median.</param>
    /// <returns>The median of the values.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is empty.</exception>
    public static double Median(double[] values)
    {
        if (values.Length == 0)
        {
            throw new ArgumentException($"{nameof(values)} cannot be empty");
        }

        double[] sorted = (double[])values.Clone();
        Array.Sort(sorted);

        return SortedMedian(sorted);
    }

    /// <summary>Return the median of a sorted sample.</summary>
    /// <param name="sortedValues">The sorted values.</param>
    /// <returns>The median of the sorted values.</returns>
    public static double SortedMedian(IReadOnlyList<double> sortedValues)
    {
        if (sortedValues.Count % Two == One)
        {
            return sortedValues[sortedValues.Count / Two];
        }

        double left = sortedValues[(sortedValues.Count / Two) - One];
        double right = sortedValues[sortedValues.Count / Two];
        return (left + right) / Two;
    }

    /// <summary>Return the percentile of a sorted sample.</summary>
    /// <param name="sortedValues">The sorted values.</param>
    /// <param name="percentile">The percentile to return.</param>
    /// <returns>The value at the requested percentile.</returns>
    public static double SortedPercentile(IReadOnlyList<double> sortedValues, double percentile)
    {
        int index = (int)(percentile * sortedValues.Count / Percent);
        index = Math.Max(0, index);
        index = Math.Min(sortedValues.Count - One, index);
        return sortedValues[index];
    }

    /// <summary>Return the percentile of a sample.</summary>
    /// <param name="values">The values from which to calculate the percentile.</param>
    /// <param name="percentile">The percentile to return.</param>
    /// <returns>The value at the requested percentile.</returns>
    public static double Percentile(IReadOnlyList<double> values, double percentile)
    {
        double[] sorted = new double[values.Count];
        for (int i = 0; i < values.Count; i++)
        {
            sorted[i] = values[i];
        }

        Array.Sort(sorted);
        return SortedPercentile(sorted, percentile);
    }

    /// <summary>
    /// Return the sample variance (second moment about the mean) of data.
    /// Input must contain at least two values. 
    /// Use this function when your data is a sample from a population.
    /// To calculate the variance from the entire population use <see cref="VarianceP(double[])"/>.
    /// </summary>
    /// <param name="values">The values used to calculate the variance.</param>
    /// <returns>The sample variance.</returns>
    public static double Variance(double[] values)
    {
        if (values.Length == 0)
        {
            throw new ArgumentException($"{nameof(values)} must not be empty");
        }

        if (values.Length == One)
        {
            return 0;
        }

        double mean = Mean(values);
        double variance = 0;
        for (int i = 0; i < values.Length; i++)
        {
            double difference = values[i] - mean;
            variance += difference * difference;
        }

        return variance / (values.Length - One);
    }

    /// <summary>
    /// Return the sample variance (second moment about the mean) of data.
    /// Input must contain at least two values. 
    /// Use this function to calculate the variance from the entire population. 
    /// To estimate the variance from a sample, use <see cref="VarianceP{T}(IReadOnlyList{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the variance.</param>
    /// <returns>The sample variance.</returns>
    public static double Variance<T>(IReadOnlyList<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        return Variance(values2);
    }

    /// <summary>
    /// Return the sample variance (second moment about the mean) of data.
    /// Input must contain at least two values. 
    /// Use this function to calculate the variance from the entire population. 
    /// To estimate the variance from a sample, use <see cref="Variance(double[])"/>.
    /// </summary>
    /// <param name="values">The values used to calculate the population variance.</param>
    /// <returns>The population variance.</returns>
    public static double VarianceP(double[] values)
    {
        if (values.Length == 0)
        {
            throw new ArgumentException($"{nameof(values)} must not be empty");
        }

        if (values.Length == One)
        {
            return 0;
        }

        double mean = Mean(values);
        double variance = 0;
        for (int i = 0; i < values.Length; i++)
        {
            double difference = values[i] - mean;
            variance += difference * difference;
        }

        return variance / values.Length;
    }

    /// <summary>
    /// Return the sample variance (second moment about the mean) of data.
    /// Input must contain at least two values. 
    /// Use this function to calculate the variance from the entire population. 
    /// To estimate the variance from a sample, use <see cref="Variance{T}(IReadOnlyList{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the population variance.</param>
    /// <returns>The population variance.</returns>
    public static double VarianceP<T>(IReadOnlyList<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        return VarianceP(values2);
    }

    /// <summary>Return the sample standard deviation (the square root of the sample variance).</summary>
    /// <param name="values">The values used to calculate the standard deviation.</param>
    /// <returns>The sample standard deviation.</returns>
    public static double StandardDeviation(double[] values) => Math.Sqrt(Variance(values));

    /// <summary>Return the sample standard deviation (the square root of the sample variance).</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the standard deviation.</param>
    /// <returns>The sample standard deviation.</returns>
    public static double StandardDeviation<T>(IEnumerable<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        return StandardDeviation(values2);
    }

    /// <summary>
    /// Return the population standard deviation (the square root of the population variance).
    /// See VarianceP() for more information.
    /// </summary>
    /// <param name="values">The values used to calculate the population standard deviation.</param>
    /// <returns>The population standard deviation.</returns>
    public static double StandardDeviationP(double[] values) => Math.Sqrt(VarianceP(values));

    /// <summary>
    /// Return the population standard deviation (the square root of the population variance).
    /// See VarianceP() for more information.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the standard deviation.</param>
    /// <returns>The population standard deviation.</returns>
    public static double StandardDeviationP<T>(IEnumerable<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        return StandardDeviationP(values2);
    }

    /// <summary>Standard error of the mean.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the standard error.</param>
    /// <returns>The standard error of the mean.</returns>
    public static double StandardError<T>(IReadOnlyList<T> values) => StandardDeviation(values) / Math.Sqrt(values.Count);

    /// <summary>Removes NaN values from a collection.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values to filter.</param>
    /// <returns>A read-only list containing only non-NaN values.</returns>
    public static IReadOnlyList<double> RemoveNaN<T>(IReadOnlyList<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        List<double> values3 = new();

        foreach (double value in values2)
        {
            if (!double.IsNaN(value))
            {
                values3.Add(value);
            }
        }

        return values3;
    }

    /// <summary>Removes NaN values from an array.</summary>
    /// <param name="values">The values to filter.</param>
    /// <returns>A new array containing only non-NaN values.</returns>
    public static double[] RemoveNaN(double[] values)
    {
        List<double> values2 = new();

        foreach (double value in values)
        {
            if (!double.IsNaN(value))
            {
                values2.Add(value);
            }
        }

        return values2.ToArray();
    }

    /// <summary>Return the sample mean. NaN values are ignored. Returns NaN if all values are NaN.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the mean.</param>
    /// <returns>The NaN-aware arithmetic mean.</returns>
    public static double NanMean<T>(IReadOnlyList<T> values)
    {
        IReadOnlyList<double> real = RemoveNaN(values);

        return (real.Count == 0) ? double.NaN : Mean(real);
    }

    /// <summary>Returns the mean of a row or column, ignoring NaN values.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="row">The row to use, or <see langword="null"/> to use <paramref name="column"/>.</param>
    /// <param name="column">The column to use, or <see langword="null"/> to use <paramref name="row"/>.</param>
    /// <returns>The mean, or NaN when no non-NaN values remain.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional row and column defaults are part of the established public API; removing them would break source compatibility.")]
    public static double NanMean(double[, ] values, uint row = 0, uint? column = null)
    {
        double[] vector = ArrayToVector(values, row, column);
        return NanMean(vector);
    }

    /// <summary>
    /// Return the sample variance (second moment about the mean) of data.
    /// Input must contain at least two values. NaN values are ignored.
    /// Use this function when your data is a sample from a population.
    /// To calculate the variance from the entire population use <see cref="VarianceP(double[])"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the variance.</param>
    /// <returns>The NaN-aware sample variance.</returns>
    public static double NanVariance<T>(IReadOnlyList<T> values)
    {
        IReadOnlyList<double> real = RemoveNaN(values);

        return real.Count < Two ? double.NaN : Variance(real);
    }

    /// <summary>Returns the sample variance of a row or column, ignoring NaN values.</summary>
    /// <typeparam name="T">The unused generic type placeholder retained for overload compatibility.</typeparam>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="row">The row to use, or <see langword="null"/> to use <paramref name="column"/>.</param>
    /// <param name="column">The column to use, or <see langword="null"/> to use <paramref name="row"/>.</param>
    /// <returns>The sample variance, or NaN when fewer than two values remain.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST1452", Justification = "The generic parameter is retained for compatibility with the overload family.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The generic parameter is retained for compatibility and cannot be inferred from a double[,] overload.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional row and column defaults are part of the established public API; removing them would break source compatibility.")]
    public static double NanVariance<T>(double[, ] values, uint row = 0, uint? column = null)
    {
        double[] vector = ArrayToVector(values, row, column);
        return Variance(RemoveNaN(vector));
    }

    /// <summary>
    /// Return the sample variance (second moment about the mean) of data.
    /// Input must contain at least two values. NaN values are ignored.
    /// Use this function when your data is a sample from a population.
    /// To calculate the variance from the entire population use <see cref="VarianceP(double[])"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the population variance.</param>
    /// <returns>The NaN-aware population variance.</returns>
    public static double NanVarianceP<T>(IReadOnlyList<T> values)
    {
        IReadOnlyList<double> real = RemoveNaN(values);

        return real.Count < Two ? double.NaN : VarianceP(real);
    }

    /// <summary>Returns the population variance of a row or column, ignoring NaN values.</summary>
    /// <typeparam name="T">The unused generic type placeholder retained for overload compatibility.</typeparam>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="row">The row to use, or <see langword="null"/> to use <paramref name="column"/>.</param>
    /// <param name="column">The column to use, or <see langword="null"/> to use <paramref name="row"/>.</param>
    /// <returns>The population variance, or NaN when fewer than two values remain.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST1452", Justification = "The generic parameter is retained for compatibility with the overload family.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The generic parameter is retained for compatibility and cannot be inferred from a double[,] overload.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional row and column defaults are part of the established public API; removing them would break source compatibility.")]
    public static double NanVarianceP<T>(double[, ] values, uint row = 0, uint? column = null)
    {
        double[] vector = ArrayToVector(values, row, column);
        return VarianceP(RemoveNaN(vector));
    }

    /// <summary>Return the sample standard deviation (the square root of the sample variance). NaN values are ignored.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the standard deviation.</param>
    /// <returns>The NaN-aware sample standard deviation.</returns>
    public static double NanStandardDeviation<T>(IReadOnlyList<T> values)
    {
        IReadOnlyList<double> real = RemoveNaN(values);

        return real.Count < Two ? double.NaN : StandardDeviation(real);
    }

    /// <summary>Returns the sample standard deviation of a row or column, ignoring NaN values.</summary>
    /// <typeparam name="T">The unused generic type placeholder retained for overload compatibility.</typeparam>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="row">The row to use, or <see langword="null"/> to use <paramref name="column"/>.</param>
    /// <param name="column">The column to use, or <see langword="null"/> to use <paramref name="row"/>.</param>
    /// <returns>The sample standard deviation.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST1452", Justification = "The generic parameter is retained for compatibility with the overload family.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The generic parameter is retained for compatibility and cannot be inferred from a double[,] overload.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional row and column defaults are part of the established public API; removing them would break source compatibility.")]
    public static double NanStandardDeviation<T>(double[, ] values, uint row = 0, uint? column = null)
    {
        double[] vector = ArrayToVector(values, row, column);
        return StandardDeviation(RemoveNaN(vector));
    }

    /// <summary>Return the population standard deviation (the square root of the sample variance). NaN values are ignored.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the standard deviation.</param>
    /// <returns>The NaN-aware population standard deviation.</returns>
    public static double NanStandardDeviationP<T>(IReadOnlyList<T> values)
    {
        IReadOnlyList<double> real = RemoveNaN(values);

        return real.Count < Two ? double.NaN : StandardDeviationP(real);
    }

    /// <summary>Returns the population standard deviation of a row or column, ignoring NaN values.</summary>
    /// <typeparam name="T">The unused generic type placeholder retained for overload compatibility.</typeparam>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="row">The row to use, or <see langword="null"/> to use <paramref name="column"/>.</param>
    /// <param name="column">The column to use, or <see langword="null"/> to use <paramref name="row"/>.</param>
    /// <returns>The population standard deviation.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST1452", Justification = "The generic parameter is retained for compatibility with the overload family.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The generic parameter is retained for compatibility and cannot be inferred from a double[,] overload.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional row and column defaults are part of the established public API; removing them would break source compatibility.")]
    public static double NanStandardDeviationP<T>(double[, ] values, uint row = 0, uint? column = null)
    {
        double[] vector = ArrayToVector(values, row, column);
        return StandardDeviationP(RemoveNaN(vector));
    }

    /// <summary>Standard error of the mean. NaN values are ignored.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="values">The values used to calculate the standard error.</param>
    /// <returns>The NaN-aware standard error of the mean.</returns>
    public static double NanStandardError<T>(IReadOnlyList<T> values)
    {
        IReadOnlyList<double> real = RemoveNaN(values);

        return (real.Count == 0) ? double.NaN : StandardError(real);
    }

    /// <summary>Returns the NaN-aware standard error of a row or column.</summary>
    /// <typeparam name="T">The unused generic type placeholder retained for overload compatibility.</typeparam>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="row">The row to use, or <see langword="null"/> to use <paramref name="column"/>.</param>
    /// <param name="column">The column to use, or <see langword="null"/> to use <paramref name="row"/>.</param>
    /// <returns>The standard error, or NaN when no values remain.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST1452", Justification = "The generic parameter is retained for compatibility with the overload family.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The generic parameter is retained for compatibility and cannot be inferred from a double[,] overload.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional row and column defaults are part of the established public API; removing them would break source compatibility.")]
    public static double NanStandardError<T>(double[, ] values, uint row = 0, uint? column = null)
    {
        double[] vector = ArrayToVector(values, row, column);
        return NanStandardError(RemoveNaN(vector));
    }

    /// <summary>Transpose a multidimensional (not jagged) array.</summary>
    /// <param name="matrix">The matrix to transpose.</param>
    /// <returns>The transposed matrix.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[, ] ArrayTranspose(double[, ] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(One);
        double[, ] result = new double[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }

        return result;
    }

    /// <summary>Extracts a row or column from a 2D array.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="row">The row to extract, or <see langword="null"/> to extract <paramref name="column"/>.</param>
    /// <param name="column">The column to extract, or <see langword="null"/> to extract <paramref name="row"/>.</param>
    /// <returns>The extracted row or column.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is empty.</exception>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2309", Justification = "Optional row and column defaults are part of the established public API; removing them would break source compatibility.")]
    public static double[] ArrayToVector(double[, ] values, uint? row = 0, uint? column = null)
    {
        if (values.GetLength(0) == 0)
        {
            throw new ArgumentException($"Array {nameof(values)} cannot be empty");
        }

        double[] vector = [];
        if (row is not null)
        {
            vector = new double[values.GetLength(One)];
            Array.Copy(values, (int)row * values.GetLength(One), vector, 0, values.GetLength(One));
        }
        else if (column is not null)
        {
            vector = ArrayToVector(ArrayTranspose(values), column);
        }

        return vector;
    }

    /// <summary>Extracts a vertical slice from a 2D array.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <param name="columnIndex">The zero-based column index.</param>
    /// <returns>The requested column.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[] VerticalSlice(double[, ] values, int columnIndex)
    {
        double[] slice = new double[values.GetLength(0)];

        for (int y = 0; y < slice.Length; y++)
        {
            slice[y] = values[y, columnIndex];
        }

        return slice;
    }

    /// <summary>Calculates the mean of each column.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <returns>The mean for each column.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[] VerticalMean(double[, ] values)
    {
        double[] result = new double[values.GetLength(1)];
        for (int column = 0; column < result.Length; column++)
        {
            result[column] = Mean(VerticalSlice(values, column));
        }

        return result;
    }

    /// <summary>Calculates the mean of each column while ignoring NaN values.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <returns>The NaN-aware mean for each column.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[] VerticalNanMean(double[, ] values)
    {
        double[] result = new double[values.GetLength(1)];
        for (int column = 0; column < result.Length; column++)
        {
            result[column] = NanMean(VerticalSlice(values, column));
        }

        return result;
    }

    /// <summary>Calculates the sample standard deviation of each column.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <returns>The sample standard deviation for each column.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[] VerticalStandardDeviation(double[, ] values)
    {
        double[] result = new double[values.GetLength(1)];
        for (int column = 0; column < result.Length; column++)
        {
            result[column] = StandardDeviation(VerticalSlice(values, column));
        }

        return result;
    }

    /// <summary>Calculates the NaN-aware sample standard deviation of each column.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <returns>The NaN-aware sample standard deviation for each column.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[] VerticalNanStandardDeviation(double[, ] values)
    {
        double[] result = new double[values.GetLength(1)];
        for (int column = 0; column < result.Length; column++)
        {
            result[column] = NanStandardDeviation(VerticalSlice(values, column));
        }

        return result;
    }

    /// <summary>Calculates the standard error of each column.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <returns>The standard error for each column.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[] VerticalStandardError(double[, ] values)
    {
        double[] result = new double[values.GetLength(1)];
        for (int column = 0; column < result.Length; column++)
        {
            result[column] = StandardError(VerticalSlice(values, column));
        }

        return result;
    }

    /// <summary>Calculates the NaN-aware standard error of each column.</summary>
    /// <param name="values">The two-dimensional values.</param>
    /// <returns>The NaN-aware standard error for each column.</returns>
    [SuppressMessage("Performance", "PSH1020", Justification = "The existing public API uses a rectangular double[,] and changing it would break callers.")]
    public static double[] VerticalNanStandardError(double[, ] values)
    {
        double[] result = new double[values.GetLength(1)];
        for (int column = 0; column < result.Length; column++)
        {
            result[column] = NanStandardError(VerticalSlice(values, column));
        }

        return result;
    }
}
