// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Used to highlight an item, attract attention or flag status.</summary>
public class Badge : global::Avalonia.Controls.ContentControl, IAppearanceControl
{
    /// <summary>Property for <see cref="Appearance"/>.</summary>
    public static readonly StyledProperty<ControlAppearance> AppearanceProperty = AvaloniaProperty.Register<
        Badge,
        ControlAppearance
    >(nameof(Appearance), ControlAppearance.Primary);

    /// <inheritdoc />
    public ControlAppearance Appearance
    {
        get => GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }
}
