// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot.Statistics;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies descriptive statistics over one-dimensional values.</summary>
public sealed class DescriptiveTests
{
    /// <summary>Checks sums and means for primitive and generic sequences.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SumAndMean_ComputeExpectedValues()
    {
        const int Two = 2;
        const int Three = 3;
        const int Four = 4;
        const int Ten = 10;
        const double ExpectedMean = 2.5;
        int[] values = [1, Two, Three, Four];
        double[] doubleValues = [1, Two, Three, Four];

        await Assert.That(Descriptive.Sum(values)).IsEqualTo(Ten);
        await Assert.That(Descriptive.Sum(doubleValues)).IsEqualTo(Ten);
        await Assert.That(Descriptive.Mean(values)).IsEqualTo(ExpectedMean);
        await Assert.That(Descriptive.Mean(doubleValues)).IsEqualTo(ExpectedMean);
        await Assert.That(static () => Descriptive.Sum([])).Throws<ArgumentException>();
        await Assert.That(static () => Descriptive.Mean([])).Throws<ArgumentException>();
    }

    /// <summary>Checks medians and percentile boundary selection.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MedianAndPercentile_HandleOddEvenAndBounds()
    {
        const int Two = 2;
        const int Three = 3;
        const int Four = 4;
        const int Ten = 10;
        const int Fifteen = 15;
        const int Twenty = 20;
        const int Thirty = 30;
        const int Fifty = 50;
        const int OneHundred = 100;
        const int TwoHundred = 200;
        const double ExpectedEvenMedian = 2.5;
        double[] evenValues = [Four, 1, Three, Two];
        double[] sortedValues = [0, Ten, Twenty, Thirty];

        await Assert.That(Descriptive.Median([Three, 1, Two])).IsEqualTo(Two);
        await Assert.That(Descriptive.Median(evenValues)).IsEqualTo(ExpectedEvenMedian);
        await Assert.That(Descriptive.SortedMedian(sortedValues)).IsEqualTo(Fifteen);
        await Assert.That(Descriptive.SortedPercentile(sortedValues, Fifty)).IsEqualTo(Twenty);
        await Assert.That(Descriptive.SortedPercentile(sortedValues, 0)).IsEqualTo(0);
        await Assert.That(Descriptive.SortedPercentile(sortedValues, OneHundred)).IsEqualTo(Thirty);
        await Assert.That(Descriptive.SortedPercentile(sortedValues, -Ten)).IsEqualTo(0);
        await Assert.That(Descriptive.SortedPercentile(sortedValues, TwoHundred)).IsEqualTo(Thirty);
        await Assert.That(Descriptive.Percentile([Thirty, 0, Twenty, Ten], Fifty)).IsEqualTo(Twenty);
        await AssertValues(evenValues, Four, 1, Three, Two);
    }

    /// <summary>Checks sample and population variance, including empty and single-item inputs.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Variance_UsesSampleAndPopulationDenominators()
    {
        const int Two = 2;
        const int Four = 4;
        double[] values = [Two, Four];
        int[] genericValues = [Two, Four];

        await Assert.That(Descriptive.Variance(values)).IsEqualTo(Two);
        await Assert.That(Descriptive.Variance(genericValues)).IsEqualTo(Two);
        await Assert.That(Descriptive.VarianceP(values)).IsEqualTo(1);
        await Assert.That(Descriptive.VarianceP(genericValues)).IsEqualTo(1);
        await Assert.That(Descriptive.Variance([Four])).IsEqualTo(0);
        await Assert.That(Descriptive.VarianceP([Four])).IsEqualTo(0);
        await Assert.That(static () => Descriptive.Variance([])).Throws<ArgumentException>();
        await Assert.That(static () => Descriptive.VarianceP([])).Throws<ArgumentException>();
    }

    /// <summary>Checks sample and population deviations and standard error.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DeviationsAndStandardError_UseExpectedFormulas()
    {
        const int Two = 2;
        const int Four = 4;
        double[] values = [Two, Four];
        int[] genericValues = [Two, Four];
        double sampleDeviation = Math.Sqrt(Two);

        await Assert.That(Descriptive.StandardDeviation(values)).IsEqualTo(sampleDeviation);
        await Assert.That(Descriptive.StandardDeviation(genericValues)).IsEqualTo(sampleDeviation);
        await Assert.That(Descriptive.StandardDeviationP(values)).IsEqualTo(1);
        await Assert.That(Descriptive.StandardDeviationP(genericValues)).IsEqualTo(1);
        await Assert.That(Descriptive.StandardError(genericValues)).IsEqualTo(1);
    }

    /// <summary>Checks that NaN filtering works for both primitive and generic lists.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RemoveNaN_FiltersNaNValues()
    {
        const float Three = 3;
        float[] genericValues = [float.NaN, 1, Three, float.NaN];
        double[] values = [double.NaN, 1, Three, double.NaN];
        IReadOnlyList<double> genericResult = Descriptive.RemoveNaN((IReadOnlyList<float>)genericValues);
        double[] result = Descriptive.RemoveNaN(values);

        await AssertValues(genericResult, 1, Three);
        await AssertValues(result, 1, Three);
    }

    /// <summary>Checks NaN-aware descriptive statistics for mixed and entirely NaN input.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NanStatistics_IgnoreNaNAndReturnNaNWhenEmpty()
    {
        const double Two = 2;
        const double Three = 3;
        const double Four = 4;
        double[] values = [double.NaN, Two, Four];
        double[] allNaN = [double.NaN, double.NaN];

        await Assert.That(Descriptive.NanMean(values)).IsEqualTo(Three);
        await Assert.That(double.IsNaN(Descriptive.NanMean(allNaN))).IsTrue();
        await Assert.That(Descriptive.NanVariance(values)).IsEqualTo(Two);
        await Assert.That(Descriptive.NanVarianceP(values)).IsEqualTo(1);
        await Assert.That(Descriptive.NanStandardDeviation(values)).IsEqualTo(Math.Sqrt(Two));
        await Assert.That(Descriptive.NanStandardDeviationP(values)).IsEqualTo(1);
        await Assert.That(Descriptive.NanStandardError(values)).IsEqualTo(1);
        await Assert.That(double.IsNaN(Descriptive.NanVariance([double.NaN]))).IsTrue();
        await Assert.That(double.IsNaN(Descriptive.NanVarianceP([double.NaN]))).IsTrue();
        await Assert.That(double.IsNaN(Descriptive.NanStandardDeviation([double.NaN]))).IsTrue();
        await Assert.That(double.IsNaN(Descriptive.NanStandardDeviationP([double.NaN]))).IsTrue();
        await Assert.That(double.IsNaN(Descriptive.NanStandardError(allNaN))).IsTrue();
    }

    /// <summary>Asserts that a list contains exactly the expected values in order.</summary>
    /// <param name="actual">The values returned by the operation.</param>
    /// <param name="expected">The expected ordered values.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    private static async Task AssertValues(IReadOnlyList<double> actual, params double[] expected)
    {
        await Assert.That(actual.Count).IsEqualTo(expected.Length);
        for (int index = 0; index < expected.Length; index++)
        {
            await Assert.That(actual[index]).IsEqualTo(expected[index]);
        }
    }
}
