// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Holds a collection of bars which are all styled the same and have a common label.</summary>
[Obsolete("temporarially not in use", true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2310",
    Justification = "This obsolete public type remains binary-visible to existing consumers; removing it would break already compiled callers.")]
public class BarSeries
{
    /// <summary>Gets or sets the bars in this series.</summary>
    public IList<Bar> Bars { get; set; } = Array.Empty<Bar>();

    /// <summary>Gets or sets the label shared by the bars.</summary>
    public string? Label { get; set; }

    /// <summary>Gets or sets the color shared by the bars.</summary>
    public Color Color { get; set; }
}
