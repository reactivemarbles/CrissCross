// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a toggle button control with a customizable chevron size for use in TreeView items.</summary>
public class ToggleButton : global::Avalonia.Controls.Primitives.ToggleButton
{
    /// <summary>The TreeView item chevron size property.</summary>
    public static readonly StyledProperty<double> ChevronSizeProperty = AvaloniaProperty.Register<ToggleButton, double>(
        nameof(ChevronSize),
        10D);

    /// <summary>Gets or sets the size of the TreeView item chevron.</summary>
    public double ChevronSize
    {
        get => GetValue(ChevronSizeProperty);
        set => SetValue(ChevronSizeProperty, value);
    }
}
