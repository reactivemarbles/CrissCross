// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents the base class for an icon source.</summary>
public abstract class IconSource : DependencyObject
{
    /// <summary>Property for <see cref="Foreground"/>.</summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        nameof(Foreground),
        typeof(Brush),
        typeof(IconSource),
        new FrameworkPropertyMetadata(SystemColors.ControlTextBrush));

    /// <summary>Gets or sets the foreground.</summary>
    /// <value>
    /// The foreground.
    /// </value>
    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>Creates the icon element.</summary>
    /// <returns>A IconElement.</returns>
    public abstract IconElement CreateIconElement();
}
