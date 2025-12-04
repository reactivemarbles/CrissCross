// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Used to highlight an item, attract attention or flag status.
/// </summary>
public class Badge : global::Avalonia.Controls.ContentControl, IAppearanceControl
{
    /// <summary>
    /// Property for <see cref="Appearance"/>.
    /// </summary>
    public static readonly StyledProperty<ControlAppearance> AppearanceProperty = AvaloniaProperty.Register<Badge, ControlAppearance>(
        nameof(Appearance), ControlAppearance.Primary);

    /// <inheritdoc />
    public ControlAppearance Appearance
    {
        get => GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }
}
