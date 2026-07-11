// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents FontIconFallback.</summary>
/// <seealso cref="Control" />
[EditorBrowsable(EditorBrowsableState.Never)]
public class FontIconFallback : Control
{
    /// <summary>The data property.</summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(
            nameof(Data),
            typeof(Geometry),
            typeof(FontIconFallback),
            null);

    /// <summary>Provides the FontIconFallback member.</summary>
    static FontIconFallback()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIconFallback), new FrameworkPropertyMetadata(typeof(FontIconFallback)));
        FocusableProperty.OverrideMetadata(typeof(FontIconFallback), new FrameworkPropertyMetadata(false));
    }

    /// <summary>Gets or sets the data.</summary>
    /// <value>
    /// The data.
    /// </value>
    public Geometry Data
    {
        get => (Geometry)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
}
