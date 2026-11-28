// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary><see cref="IComparer{T}"/> for various types provided by CrissCross.Plot.</summary>
public sealed class BinarySearchComparer : IComparer<Coordinates>, IComparer<double>, IComparer<RootedCoordinateVector>, IComparer<RootedPixelVector>, IComparer
{
    /// <summary>Thread-Safe singleton</summary>
    public static readonly BinarySearchComparer Instance = new();

    /// <summary>Initializes a new instance of the <see cref="BinarySearchComparer"/> class.</summary>
    private BinarySearchComparer() { }

    /// <summary>Gets the default comparer for the specified type.</summary>
    /// <typeparam name="T">The type compared by the returned comparer.</typeparam>
    /// <returns>The default comparer for <typeparamref name="T"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2307",
        Justification = "A comparer factory has no value parameter from which the type argument could be inferred.")]
    public static IComparer<T> GetComparer<T>() => GenericComparer<T>.Default;

    /// <summary>Compares coordinate vectors by their horizontal values.</summary>
    /// <param name="x">The first coordinate vector.</param>
    /// <param name="y">The second coordinate vector.</param>
    /// <returns>A value indicating the relative horizontal order.</returns>
    public int Compare(Coordinates x, Coordinates y) => x.X.CompareTo(y.X);

    /// <summary>Compares two numeric values.</summary>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <returns>A value indicating the relative numeric order.</returns>
    public int Compare(double x, double y) => x.CompareTo(y);

    /// <summary>Compares rooted pixel vectors by their horizontal coordinates.</summary>
    /// <param name="x">The first vector.</param>
    /// <param name="y">The second vector.</param>
    /// <returns>A value indicating the relative horizontal order.</returns>
    public int Compare(RootedPixelVector x, RootedPixelVector y) => x.Point.X.CompareTo(y.Point.X);

    /// <summary>Compares rooted coordinate vectors by their horizontal coordinates.</summary>
    /// <param name="x">The first vector.</param>
    /// <param name="y">The second vector.</param>
    /// <returns>A value indicating the relative horizontal order.</returns>
    public int Compare(RootedCoordinateVector x, RootedCoordinateVector y) => x.Point.X.CompareTo(y.Point.X);

    /// <summary>Compares objects that contain supported coordinate values.</summary>
    /// <param name="x">The first object.</param>
    /// <param name="y">The second object.</param>
    /// <returns>A value indicating the relative order.</returns>
    public int Compare(object? x, object? y)
    {
        if (x == y)
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        if (x is Coordinates a
            && y is Coordinates b)
        {
            return Compare(a, b);
        }

        throw new ArgumentException(string.Empty, nameof(x));
    }
}
