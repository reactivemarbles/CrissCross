// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Groups RGB color components.</summary>
public sealed class RgbColorComponents
{
    /// <summary>Initializes a new instance of the <see cref="RgbColorComponents"/> class.</summary>
    /// <param name="red">The red component.</param>
    /// <param name="green">The green component.</param>
    /// <param name="blue">The blue component.</param>
    public RgbColorComponents(double red, double green, double blue) => (Red, Green, Blue) = (red, green, blue);

    /// <summary>Gets the red component.</summary>
    public double Red { get; }

    /// <summary>Gets the green component.</summary>
    public double Green { get; }

    /// <summary>Gets the blue component.</summary>
    public double Blue { get; }
}
