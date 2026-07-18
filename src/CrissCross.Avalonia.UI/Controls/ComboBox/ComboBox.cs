// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>
/// Represents a combo box control that allows users to select an item from a drop-down list, with support for
/// customizing the corner radius of the control.
/// </summary>
public class ComboBox : global::Avalonia.Controls.ComboBox
{
    /// <summary>
    /// Identifies the CornerRadius styled property, which determines the degree to which the corners of the ComboBox
    /// are rounded.
    /// </summary>
    /// <remarks>This property can be used in styles and templates to customize the appearance of the
    /// ComboBox's border. The default value is typically zero, resulting in square corners.</remarks>
    public static new readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<ComboBox, CornerRadius>(nameof(CornerRadius));

    /// <summary>Gets or sets the corner radius.</summary>
    public new CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}
