// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Statistics;

/// <summary>Represents the probability density of a normally distributed population.</summary>
public class ProbabilityDensity
{
    /// <summary>Gets the mean of the distribution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly double Mean;

    /// <summary>Gets the standard deviation of the distribution.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly double StDev;

    /// <summary>The multiplier used by the Gaussian exponent.</summary>
    private const double GaussianExponentMultiplier = -0.5;

    /// <summary>Initializes a new instance of the <see cref="ProbabilityDensity"/> class from a sequence of values.</summary>
    /// <param name="values">The sample values.</param>
    public ProbabilityDensity(IEnumerable<double> values)
    {
        List<double> samples = new(values);
        Mean = Descriptive.Mean(samples);
        StDev = Descriptive.StandardDeviation(samples);
    }

    /// <summary>Initializes a new instance of the <see cref="ProbabilityDensity"/> class from its mean and standard deviation.</summary>
    /// <param name="mean">The distribution mean.</param>
    /// <param name="stDev">The distribution standard deviation.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for named-argument compatibility.")]
    public ProbabilityDensity(double mean, double stDev)
    {
        Mean = mean;
        StDev = stDev;
    }

    /// <summary>Calculates the density at an X coordinate.</summary>
    /// <param name="x">The coordinate at which to calculate the density.</param>
    /// <param name="scale">A scale factor applied to the density.</param>
    /// <returns>The scaled probability density.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional scale default is retained for source and binary compatibility.")]
    public double GetY(double x, double scale = 1)
    {
        double u = (x - Mean) / StDev;
        return Math.Exp(GaussianExponentMultiplier * u * u) * scale;
    }

    /// <summary>Calculates the density at each specified X coordinate.</summary>
    /// <param name="xs">The coordinates at which to calculate densities.</param>
    /// <param name="scale">A scale factor applied to each density.</param>
    /// <returns>The scaled densities.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional scale default is retained for source and binary compatibility.")]
    public double[] GetYs(double[] xs, double scale = 1)
    {
        double[] ys = new double[xs.Length];
        for (int i = 0; i < xs.Length; i++)
        {
            ys[i] = GetY(xs[i], scale);
        }

        return ys;
    }
}
