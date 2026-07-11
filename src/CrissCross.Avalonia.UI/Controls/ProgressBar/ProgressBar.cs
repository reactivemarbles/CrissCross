// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a control that indicates progress by displaying a bar.</summary>
public class ProgressBar : global::Avalonia.Controls.ProgressBar
{
    /// <summary>Property for <see cref="CornerRadius"/>.</summary>
    public static new readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<ProgressBar, CornerRadius>(nameof(CornerRadius));

    /// <summary>Gets or sets the corner radius.</summary>
    public new CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}
