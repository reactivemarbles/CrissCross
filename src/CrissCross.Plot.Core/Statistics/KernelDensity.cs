// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace CrissCross.Plot.Statistics;

/// <summary>Provides kernel density estimation methods.</summary>
public static class KernelDensity
{
    /// <summary>Coefficient used by the Epanechnikov kernel.</summary>
    private const double EpanechnikovCoefficient = 0.75;

    /// <summary>Exponent coefficient used by the Gaussian kernel.</summary>
    private const double GaussianExponentCoefficient = -0.5;

    /// <summary>Normalization factor used by the Gaussian kernel.</summary>
    private const double GaussianNormalizationFactor = 2;

    /// <summary>Density of the uniform kernel within its support.</summary>
    private const double UniformKernelDensity = 0.5;

    /// <summary>Absolute normalized distance beyond which the kernels return zero.</summary>
    private const double KernelSupportBoundary = 1;

    /// <summary>Multiplier used by Scott's rule bandwidth estimation.</summary>
    private const double ScottStandardDeviationMultiplier = 24;

    /// <summary>Cube-root exponent used by Scott's rule.</summary>
    private const double OneThird = 1.0 / 3.0;

    /// <summary>Estimates the density at a value using the specified kernel and bandwidth.</summary>
    /// <param name="x">The value at which to estimate density.</param>
    /// <param name="values">The observed values used for the estimate.</param>
    /// <param name="kernel">The kernel function.</param>
    /// <param name="bandwidth">The bandwidth applied to each observed value.</param>
    /// <returns>The estimated density.</returns>
    public static double Estimate(double x, IReadOnlyList<double> values, Func<double, double> kernel, double bandwidth)
    {
        if (values.Count == 0 || bandwidth == 0)
        {
            return 0;
        }

        double sum = 0;
        for (int i = 0; i < values.Count; i++)
        {
            sum += kernel((x - values[i]) / bandwidth);
        }

        return sum / (values.Count * bandwidth);
    }

    /// <summary>Estimates densities for each input value using the specified kernel and bandwidth.</summary>
    /// <param name="xs">The values at which to estimate density.</param>
    /// <param name="values">The observed values used for the estimates.</param>
    /// <param name="kernel">The kernel function.</param>
    /// <param name="bandwidth">The bandwidth applied to each observed value.</param>
    /// <returns>A lazily evaluated sequence of density estimates.</returns>
    public static IEnumerable<double> Estimate(
        IEnumerable<double> xs,
        IReadOnlyList<double> values,
        Func<double, double> kernel,
        double bandwidth)
    {
        ThrowIfNull(xs, nameof(xs));
        return EstimateEach(xs, values, kernel, bandwidth);
    }

    /// <summary>Estimates the density at a value using the selected kernel and bandwidth rule.</summary>
    /// <param name="x">The value at which to estimate density.</param>
    /// <param name="values">The observed values used for the estimate.</param>
    /// <param name="kernel">The kernel used to estimate density.</param>
    /// <param name="bandwidthRule">The rule used to select the bandwidth.</param>
    /// <returns>The estimated density.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional kernel and bandwidth defaults are part of the established public API.")]
    public static double Estimate(
        double x,
        IReadOnlyList<double> values,
        KdeKernel kernel = KdeKernel.Epanechnikov,
        KdeBandWidthRule bandwidthRule = KdeBandWidthRule.Scotts)
    {
        Func<double, double> kernelFunction = GetKernel(kernel);
        double bandwidth = GetBandWidth(bandwidthRule, values);
        return Estimate(x, values, kernelFunction, bandwidth);
    }

    /// <summary>Estimates densities for each input value using the selected kernel and bandwidth rule.</summary>
    /// <param name="xs">The values at which to estimate density.</param>
    /// <param name="values">The observed values used for the estimates.</param>
    /// <param name="kernel">The kernel used to estimate density.</param>
    /// <param name="bandwidthRule">The rule used to select the bandwidth.</param>
    /// <returns>A lazily evaluated sequence of density estimates.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional kernel and bandwidth defaults are part of the established public API.")]
    public static IEnumerable<double> Estimate(
        IEnumerable<double> xs,
        IReadOnlyList<double> values,
        KdeKernel kernel = KdeKernel.Epanechnikov,
        KdeBandWidthRule bandwidthRule = KdeBandWidthRule.Scotts)
    {
        Func<double, double> kernelFunction = GetKernel(kernel);
        double bandwidth = GetBandWidth(bandwidthRule, values);
        return Estimate(xs, values, kernelFunction, bandwidth);
    }

    /// <summary>Estimates the density for each value in a lazily evaluated sequence.</summary>
    /// <param name="xs">The values at which to estimate density.</param>
    /// <param name="values">The observed values used for the estimates.</param>
    /// <param name="kernel">The kernel function.</param>
    /// <param name="bandwidth">The bandwidth applied to each observed value.</param>
    /// <returns>A lazily evaluated sequence of density estimates.</returns>
    private static IEnumerable<double> EstimateEach(
        IEnumerable<double> xs,
        IReadOnlyList<double> values,
        Func<double, double> kernel,
        double bandwidth)
    {
        foreach (double x in xs)
        {
            yield return Estimate(x, values, kernel, bandwidth);
        }
    }

    /// <summary>Throws when a required reference parameter is null.</summary>
    /// <typeparam name="T">The reference type to validate.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1409", Justification = "ArgumentNullException.ThrowIfNull is unavailable for the supported .NET Framework targets.")]
    private static void ThrowIfNull<T>(T value, string parameterName)
        where T : class
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value, parameterName);
#else
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(parameterName);
#endif
    }

    /// <summary>Gets the function associated with a kernel selection.</summary>
    /// <param name="kernel">The kernel selection.</param>
    /// <returns>The kernel function.</returns>
    private static Func<double, double> GetKernel(KdeKernel kernel) => kernel switch
    {
        KdeKernel.Epanechnikov => EpanechnikovKernel,
        KdeKernel.Gaussian => GaussianKernel,
        KdeKernel.Uniform => UniformKernel,
        KdeKernel.Triangular => TriangularKernel,
        _ => throw new ArgumentOutOfRangeException(nameof(kernel), kernel, "Unsupported kernel."),
    };

    /// <summary>Gets the bandwidth selected by the specified rule.</summary>
    /// <param name="bandwidthRule">The bandwidth selection rule.</param>
    /// <param name="values">The observed values used to estimate bandwidth.</param>
    /// <returns>The estimated bandwidth.</returns>
    private static double GetBandWidth(KdeBandWidthRule bandwidthRule, IReadOnlyList<double> values) => bandwidthRule switch
    {
        KdeBandWidthRule.Scotts => ScottsRule(values),
        _ => throw new ArgumentOutOfRangeException(nameof(bandwidthRule), bandwidthRule, "Unsupported bandwidth rule."),
    };

    /// <summary>Calculates the Epanechnikov kernel density for a value.</summary>
    /// <param name="x">The normalized distance from the estimate point.</param>
    /// <returns>The Epanechnikov kernel density.</returns>
    private static double EpanechnikovKernel(double x) => Math.Abs(x) <= KernelSupportBoundary
        ? EpanechnikovCoefficient * (1 - (x * x))
        : 0;

    /// <summary>Calculates the Gaussian kernel density for a value.</summary>
    /// <param name="x">The normalized distance from the estimate point.</param>
    /// <returns>The Gaussian kernel density.</returns>
    private static double GaussianKernel(double x) =>
        Math.Exp(GaussianExponentCoefficient * x * x) / Math.Sqrt(GaussianNormalizationFactor * Math.PI);

    /// <summary>Calculates the uniform kernel density for a value.</summary>
    /// <param name="x">The normalized distance from the estimate point.</param>
    /// <returns>The uniform kernel density on the interval from -1 to 1.</returns>
    private static double UniformKernel(double x) => Math.Abs(x) <= KernelSupportBoundary
        ? UniformKernelDensity
        : 0;

    /// <summary>Calculates the triangular kernel density for a value.</summary>
    /// <param name="x">The normalized distance from the estimate point.</param>
    /// <returns>The triangular kernel density on the interval from -1 to 1.</returns>
    private static double TriangularKernel(double x)
    {
        if (double.IsNaN(x))
        {
            throw new ArgumentOutOfRangeException(nameof(x), x, "The normalized distance cannot be NaN.");
        }

        if (x <= -KernelSupportBoundary || x >= KernelSupportBoundary)
        {
            return 0;
        }

        return x <= 0 ? x + KernelSupportBoundary : KernelSupportBoundary - x;
    }

    /// <summary>Estimates bandwidth using Scott's rule.</summary>
    /// <param name="values">The observed values used for the estimate.</param>
    /// <returns>The estimated bandwidth, or zero when no values are available.</returns>
    private static double ScottsRule(IReadOnlyList<double> values)
    {
        if (values.Count == 0)
        {
            return 0;
        }

        double standardDeviation = Statistics.Descriptive.StandardDeviation(values);
        double sampleCount = values.Count;
        double scale = Math.Pow(ScottStandardDeviationMultiplier * Math.Sqrt(Math.PI), OneThird);

        return scale * standardDeviation * Math.Pow(sampleCount, -OneThird);
    }
}
