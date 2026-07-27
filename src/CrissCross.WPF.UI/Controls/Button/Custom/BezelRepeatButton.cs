// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents GenericRepeatButton.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class BezelRepeatButton : CommonRepeatButtonBase
{
    /// <summary>The glare opacity mask property.</summary>
    public static readonly DependencyProperty GlareOpacityMaskProperty = DependencyProperty.Register(
        nameof(GlareOpacityMask),
        typeof(Brush),
        typeof(BezelRepeatButton),
        new(null));

    /// <summary>The pressed brush property.</summary>
    public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register(
        nameof(PressedBrush),
        typeof(Brush),
        typeof(BezelRepeatButton),
        new(Brushes.Green));

    /// <summary>Initializes a new instance of the <see cref="BezelRepeatButton"/> class.</summary>
    public BezelRepeatButton()
        : base(typeof(BezelRepeatButton).FullName!) { }

    /// <summary>Gets or sets the glare opacity mask.</summary>
    /// <value>
    /// The glare opacity mask.
    /// </value>
    public Brush GlareOpacityMask
    {
        get => (Brush)GetValue(GlareOpacityMaskProperty);
        set => SetValue(GlareOpacityMaskProperty, value);
    }

    /// <summary>Gets or sets the pressed brush.</summary>
    /// <value>
    /// The pressed brush.
    /// </value>
    public Brush PressedBrush
    {
        get => (Brush)GetValue(PressedBrushProperty);
        set => SetValue(PressedBrushProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
