// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Statistics;

/// <summary>Describes a least-squares linear regression model.</summary>
public readonly struct LinearRegression : IEquatable<LinearRegression>
{
    /// <summary>Gets the slope of the regression line.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly double Slope;

    /// <summary>Gets the vertical offset of the regression line.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly double Offset;

    /// <summary>Gets the coefficient of determination.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public readonly double Rsquared;

    /// <summary>Initializes a new instance of the <see cref="LinearRegression"/> struct from coordinates.</summary>
    /// <param name="coordinates">The coordinates used to calculate the regression.</param>
    public LinearRegression(Coordinates[] coordinates)
    {
        if (coordinates.Length < 2)
        {
            throw new ArgumentException($"{nameof(coordinates)} must have at least 2 points");
        }

        double[] xs = new double[coordinates.Length];
        double[] ys = new double[coordinates.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            xs[i] = coordinates[i].X;
            ys[i] = coordinates[i].Y;
        }

        (Slope, Offset, Rsquared) = GetCoefficients(xs, ys);
    }

    /// <summary>Initializes a new instance of the <see cref="LinearRegression"/> struct from a coordinate sequence.</summary>
    /// <param name="coordinates">The coordinates used to calculate the regression.</param>
    public LinearRegression(IEnumerable<Coordinates> coordinates)
    {
        if (coordinates is null)
        {
            throw new ArgumentException($"{nameof(coordinates)} must have at least 2 points");
        }

        List<Coordinates> points = new(coordinates);
        if (points.Count < 2)
        {
            throw new ArgumentException($"{nameof(coordinates)} must have at least 2 points");
        }

        double[] xs = new double[points.Count];
        double[] ys = new double[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            xs[i] = points[i].X;
            ys[i] = points[i].Y;
        }

        (Slope, Offset, Rsquared) = GetCoefficients(xs, ys);
    }

    /// <summary>Initializes a new instance of the <see cref="LinearRegression"/> struct from paired X and Y values.</summary>
    /// <param name="xs">The X coordinates.</param>
    /// <param name="ys">The corresponding Y coordinates.</param>
    public LinearRegression(double[] xs, double[] ys)
    {
        if (xs.Length != ys.Length)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must be the same length");
        }

        if (ys.Length < 2)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have at least 2 points");
        }

        (Slope, Offset, Rsquared) = GetCoefficients(xs, ys);
    }

    /// <summary>Initializes a new instance of the <see cref="LinearRegression"/> struct using evenly spaced X values.</summary>
    /// <param name="ys">The Y values.</param>
    /// <param name="firstX">The X coordinate of the first value.</param>
    /// <param name="xSpacing">The spacing between X values.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional defaults are retained for source and binary compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for named-argument compatibility.")]
    public LinearRegression(double[] ys, double firstX = 0, double xSpacing = 1)
    {
        if (xSpacing == 0)
        {
            throw new ArgumentException($"{nameof(xSpacing)} cannot be zero");
        }

        if (ys.Length < 2)
        {
            throw new ArgumentException($"{nameof(ys)} must have at least 2 points");
        }

        double[] xs = Generate.Consecutive(count: ys.Length, delta: xSpacing, first: firstX);
        (Slope, Offset, Rsquared) = GetCoefficients(xs, ys);
    }

    /// <summary>Gets the formula for the regression line.</summary>
    public string Formula => $"y = {Slope:0.###}x + {Offset:0.###}";

    /// <summary>Gets the formula and coefficient of determination.</summary>
    public string FormulaWithRSquared => $"{Formula} (r²={Rsquared:0.###})";

    /// <summary>Determines whether two regression models are equal.</summary>
    /// <param name="left">The first model.</param>
    /// <param name="right">The second model.</param>
    /// <returns><see langword="true"/> when all model coefficients match.</returns>
    public static bool operator ==(LinearRegression left, LinearRegression right) => left.Equals(right);

    /// <summary>Determines whether two regression models differ.</summary>
    /// <param name="left">The first model.</param>
    /// <param name="right">The second model.</param>
    /// <returns><see langword="true"/> when any model coefficient differs.</returns>
    public static bool operator !=(LinearRegression left, LinearRegression right) => !left.Equals(right);

    /// <summary>Returns the regression line's Y value at the specified X position.</summary>
    /// <param name="x">The X position.</param>
    /// <returns>The corresponding Y value.</returns>
    public double GetValue(double x) => Offset + (Slope * x);

    /// <summary>Calculates the regression line's Y values for the specified X positions.</summary>
    /// <param name="xs">The X positions.</param>
    /// <returns>The corresponding Y values.</returns>
    public double[] GetValues(double[] xs)
    {
        double[] ys = new double[xs.Length];

        for (int i = 0; i < ys.Length; i++)
        {
            ys[i] = GetValue(xs[i]);
        }

        return ys;
    }

    /// <summary>Determines whether this model equals another model.</summary>
    /// <param name="other">The model to compare.</param>
    /// <returns><see langword="true"/> when all model coefficients match.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact equality is required for value equality to remain consistent with the hash code.")]
    public bool Equals(LinearRegression other) => Slope.Equals(other.Slope) && Offset.Equals(other.Offset) && Rsquared.Equals(other.Rsquared);

    /// <summary>Determines whether this model equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal model.</returns>
    public override bool Equals(object? obj) => obj is LinearRegression other && Equals(other);

    /// <summary>Gets a hash code for this model.</summary>
    /// <returns>A hash code based on all model coefficients.</returns>
    public override int GetHashCode() => Slope.GetHashCode() ^ Offset.GetHashCode() ^ Rsquared.GetHashCode();

    /// <summary>Calculates the regression coefficients for paired X and Y values.</summary>
    /// <param name="xs">The X values.</param>
    /// <param name="ys">The corresponding Y values.</param>
    /// <returns>The slope, offset, and coefficient of determination.</returns>
    private static (double slope, double offset, double coefficientOfDetermination) GetCoefficients(double[] xs, double[] ys)
    {
        double sumX = 0;
        double sumY = 0;
        for (int i = 0; i < xs.Length; i++)
        {
            sumX += xs[i];
            sumY += ys[i];
        }

        double meanX = sumX / xs.Length;
        double meanY = sumY / ys.Length;
        double sumXYResidual = 0;
        double sumXSquareResidual = 0;
        for (int i = 0; i < xs.Length; i++)
        {
            double centeredXValue = xs[i] - meanX;
            sumXYResidual += centeredXValue * (ys[i] - meanY);
            sumXSquareResidual += centeredXValue * centeredXValue;
        }

        double slope = sumXYResidual / sumXSquareResidual;
        double offset = meanY - (slope * meanX);

        double sumSquaredTotal = 0;
        double sumSquaredResidual = 0;
        for (int i = 0; i < ys.Length; i++)
        {
            double meanResidual = ys[i] - meanY;
            sumSquaredTotal += meanResidual * meanResidual;

            double modelY = (slope * xs[i]) + offset;
            double modelResidual = ys[i] - modelY;
            sumSquaredResidual += modelResidual * modelResidual;
        }

        double coefficientOfDetermination = 1 - (sumSquaredResidual / sumSquaredTotal);
        return (slope, offset, coefficientOfDetermination);
    }
}
