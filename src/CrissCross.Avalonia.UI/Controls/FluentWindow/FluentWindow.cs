// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A custom Fluent Window with more convenience methods.
/// </summary>
/// <remarks>
/// This is a simplified version for Avalonia. Platform-specific features
/// like Windows Mica backdrop are not available cross-platform.
/// </remarks>
public class FluentWindow : Window
{
    /// <summary>
    /// Property for <see cref="ExtendsContentIntoTitleBar"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ExtendsContentIntoTitleBarProperty =
        AvaloniaProperty.Register<FluentWindow, bool>(
            nameof(ExtendsContentIntoTitleBar), false);

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentWindow"/> class.
    /// </summary>
    public FluentWindow()
    {
        // Configure window for Fluent design appearance
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur };
    }

    /// <summary>
    /// Gets or sets a value indicating whether the content extends into the title bar area.
    /// </summary>
    public bool ExtendsContentIntoTitleBar
    {
        get => GetValue(ExtendsContentIntoTitleBarProperty);
        set => SetValue(ExtendsContentIntoTitleBarProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == ExtendsContentIntoTitleBarProperty)
        {
            ExtendClientAreaToDecorationsHint = (bool)change.NewValue!;
            ExtendClientAreaTitleBarHeightHint = (bool)change.NewValue! ? -1 : 0;
        }
    }
}
