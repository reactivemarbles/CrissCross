// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Statistics.Tests;

/// <summary>Calculates an unpaired two-sample Student's t-test.</summary>
public class UnpairedTTest
{
    /// <summary>Initializes a new instance of the <see cref="UnpairedTTest"/> class.</summary>
    /// <param name="pop1">The first sample population.</param>
    /// <param name="pop2">The second sample population.</param>
    public UnpairedTTest(Population pop1, Population pop2)
    {
        const int SampleCountAdjustment = 2;

        Population1 = pop1;
        Population2 = pop2;

        // Standard error on the difference between the samples.
        double standardErrorDifference = Math.Sqrt((pop1.StandardError * pop1.StandardError) + (pop2.StandardError * pop2.StandardError));
        T = (pop1.Mean - pop2.Mean) / standardErrorDifference;
        DF = pop1.Count + pop2.Count - SampleCountAdjustment;
        P = GetStudentP(T, DF);
    }

    /// <summary>Gets the first sample population.</summary>
    public Population Population1 { get; }

    /// <summary>Gets the second sample population.</summary>
    public Population Population2 { get; }

    /// <summary>Gets the t statistic.</summary>
    public double T { get; }

    /// <summary>Gets the degrees of freedom.</summary>
    public int DF { get; }

    /// <summary>Gets the two-tailed p-value.</summary>
    public double P { get; }

    /// <summary>Calculates the cumulative probability for the standard normal distribution.</summary>
    /// <param name="z">The z-score.</param>
    /// <returns>The probability under the normal curve from negative infinity to <paramref name="z"/>.</returns>
    public static double Gauss(double z)
    {
        const double Half = 0.5;
        const double Two = 2.0;
        const double Three = 3.0;

        // ACM Algorithm #209.
        double y = Math.Abs(z) / Two;
        if (z == 0.0)
        {
            return Half;
        }

        if (y >= Three)
        {
            return z > 0.0 ? 1.0 : 0.0;
        }

        double probability = y < 1.0 ? EvaluateSmallRange(y) : EvaluateLargeRange(y - Two);
        return z > 0.0 ? (probability + 1.0) / Two : (1.0 - probability) / Two;
    }

    /// <summary>Calculates the two-tailed p-value from Student's t distribution.</summary>
    /// <param name="t">The t statistic.</param>
    /// <param name="df">The degrees of freedom.</param>
    /// <returns>The two-tailed p-value.</returns>
    private static double GetStudentP(double t, double df)
    {
        const double Half = 0.5;
        const double Two = 2.0;
        const double Three = 3.0;
        const double LogarithmThreshold = 1.0E-6;
        const double DegreesOfFreedomScale = 48.0;
        const double PolynomialCoefficient1 = -0.4;
        const double PolynomialCoefficient2 = -3.3;
        const double PolynomialCoefficient3 = -24.0;
        const double PolynomialCoefficient4 = -85.5;
        const double PolynomialDenominatorScale = 0.8;
        const double PolynomialDenominatorConstant = 100.0;

        double n = df;
        double a;
        double b;
        double y;

        t *= t;
        y = t / n;
        b = y + 1.0;
        if (y > LogarithmThreshold)
        {
            y = Math.Log(b);
        }

        a = n - Half;
        b = DegreesOfFreedomScale * a * a;
        y = a * y;

        y = ((((((((((PolynomialCoefficient1 * y) + PolynomialCoefficient2) * y) + PolynomialCoefficient3) * y) + PolynomialCoefficient4)
          / ((PolynomialDenominatorScale * y * y) + PolynomialDenominatorConstant + b))
            + y + Three) / b) + 1.0) * Math.Sqrt(y);
        return Two * Gauss(-y);
    }

    /// <summary>Evaluates the standard normal approximation for values below one.</summary>
    /// <param name="y">The absolute z-score scaled by one half.</param>
    /// <returns>The approximation polynomial result.</returns>
    private static double EvaluateSmallRange(double y)
    {
        const double Two = 2.0;
        const double Coefficient1 = 0.000124818987;
        const double Coefficient2 = -0.001075204047;
        const double Coefficient3 = 0.005198775019;
        const double Coefficient4 = -0.019198292004;
        const double Coefficient5 = 0.059054035642;
        const double Coefficient6 = -0.151968751364;
        const double Coefficient7 = 0.319152932694;
        const double Coefficient8 = -0.531923007300;
        const double Coefficient9 = 0.797884560593;

        double squaredY = y * y;
        return ((((((((((((((((Coefficient1 * squaredY)
            + Coefficient2) * squaredY) + Coefficient3) * squaredY)
            + Coefficient4) * squaredY) + Coefficient5) * squaredY)
            + Coefficient6) * squaredY) + Coefficient7) * squaredY)
            + Coefficient8) * squaredY) + Coefficient9) * y * Two;
    }

    /// <summary>Evaluates the standard normal approximation for values from one to three.</summary>
    /// <param name="y">The absolute z-score offset by two.</param>
    /// <returns>The approximation polynomial result.</returns>
    private static double EvaluateLargeRange(double y)
    {
        const double Coefficient1 = -0.000045255659;
        const double Coefficient2 = 0.000152529290;
        const double Coefficient3 = -0.000019538132;
        const double Coefficient4 = -0.000676904986;
        const double Coefficient5 = 0.001390604284;
        const double Coefficient6 = -0.000794620820;
        const double Coefficient7 = -0.002034254874;
        const double Coefficient8 = 0.006549791214;
        const double Coefficient9 = -0.010557625006;
        const double Coefficient10 = 0.011630447319;
        const double Coefficient11 = -0.009279453341;
        const double Coefficient12 = 0.005353579108;
        const double Coefficient13 = -0.002141268741;
        const double Coefficient14 = 0.000535310849;
        const double Constant = 0.999936657524;

        return (((((((((((((((((((((((((((Coefficient1 * y)
            + Coefficient2) * y) + Coefficient3) * y)
            + Coefficient4) * y) + Coefficient5) * y)
            + Coefficient6) * y) + Coefficient7) * y)
            + Coefficient8) * y) + Coefficient9) * y)
            + Coefficient10) * y) + Coefficient11) * y)
            + Coefficient12) * y) + Coefficient13) * y)
            + Coefficient14) * y) + Constant;
    }
}
