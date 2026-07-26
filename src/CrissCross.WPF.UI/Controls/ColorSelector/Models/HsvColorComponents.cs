// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Groups HSV color components.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class HsvColorComponents
{
    /// <summary>Initializes a new instance of the <see cref="HsvColorComponents"/> class.</summary>
    /// <param name="hue">The hue component.</param>
    /// <param name="saturation">The saturation component.</param>
    /// <param name="value">The value component.</param>
    public HsvColorComponents(double hue, double saturation, double value) =>
        (Hue, Saturation, Value) = (hue, saturation, value);

    /// <summary>Gets the hue component.</summary>
    public double Hue { get; }

    /// <summary>Gets the saturation component.</summary>
    public double Saturation { get; }

    /// <summary>Gets the value component.</summary>
    public double Value { get; }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
