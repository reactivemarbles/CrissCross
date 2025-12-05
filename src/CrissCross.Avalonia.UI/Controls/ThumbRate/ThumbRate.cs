// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Allows to rate positively or negatively by clicking on one of the thumbs.
/// </summary>
public class ThumbRate : global::Avalonia.Controls.Control
{
    /// <summary>
    /// Property for <see cref="State"/>.
    /// </summary>
    public static readonly StyledProperty<ThumbRateState> StateProperty = AvaloniaProperty.Register<ThumbRate, ThumbRateState>(
        nameof(State), ThumbRateState.None);

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    public ThumbRateState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }
}
