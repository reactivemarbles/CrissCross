// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Allows to rate positively or negatively by clicking on one of the thumbs.
/// </summary>
public class ThumbRate : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="State"/>.
    /// </summary>
    public static readonly StyledProperty<ThumbRateState> StateProperty = AvaloniaProperty.Register<ThumbRate, ThumbRateState>(
        nameof(State), ThumbRateState.None);

    /// <summary>
    /// Property for <see cref="ThumbUpCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> ThumbUpCommandProperty = AvaloniaProperty.Register<ThumbRate, ICommand?>(
        nameof(ThumbUpCommand));

    /// <summary>
    /// Property for <see cref="ThumbDownCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> ThumbDownCommandProperty = AvaloniaProperty.Register<ThumbRate, ICommand?>(
        nameof(ThumbDownCommand));

    /// <summary>
    /// Property for <see cref="ThumbSize"/>.
    /// </summary>
    public static readonly StyledProperty<double> ThumbSizeProperty = AvaloniaProperty.Register<ThumbRate, double>(
        nameof(ThumbSize), 24.0);

    /// <summary>
    /// Property for <see cref="SelectedBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> SelectedBrushProperty = AvaloniaProperty.Register<ThumbRate, IBrush>(
        nameof(SelectedBrush), Brushes.DodgerBlue);

    /// <summary>
    /// Property for <see cref="UnselectedBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> UnselectedBrushProperty = AvaloniaProperty.Register<ThumbRate, IBrush>(
        nameof(UnselectedBrush), Brushes.Gray);

    /// <summary>
    /// Defines the <see cref="StateChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> StateChangedEvent =
        RoutedEvent.Register<ThumbRate, RoutedEventArgs>(nameof(StateChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the state changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? StateChanged
    {
        add => AddHandler(StateChangedEvent, value);
        remove => RemoveHandler(StateChangedEvent, value);
    }

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    public ThumbRateState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// Gets or sets the thumb up command.
    /// </summary>
    public ICommand? ThumbUpCommand
    {
        get => GetValue(ThumbUpCommandProperty);
        set => SetValue(ThumbUpCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the thumb down command.
    /// </summary>
    public ICommand? ThumbDownCommand
    {
        get => GetValue(ThumbDownCommandProperty);
        set => SetValue(ThumbDownCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the thumb size.
    /// </summary>
    public double ThumbSize
    {
        get => GetValue(ThumbSizeProperty);
        set => SetValue(ThumbSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected brush.
    /// </summary>
    public IBrush SelectedBrush
    {
        get => GetValue(SelectedBrushProperty);
        set => SetValue(SelectedBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the unselected brush.
    /// </summary>
    public IBrush UnselectedBrush
    {
        get => GetValue(UnselectedBrushProperty);
        set => SetValue(UnselectedBrushProperty, value);
    }

    /// <summary>
    /// Sets the state to liked.
    /// </summary>
    public void ThumbUp()
    {
        State = State == ThumbRateState.Liked ? ThumbRateState.None : ThumbRateState.Liked;
        RaiseEvent(new RoutedEventArgs(StateChangedEvent));
        ThumbUpCommand?.Execute(State);
    }

    /// <summary>
    /// Sets the state to disliked.
    /// </summary>
    public void ThumbDown()
    {
        State = State == ThumbRateState.Disliked ? ThumbRateState.None : ThumbRateState.Disliked;
        RaiseEvent(new RoutedEventArgs(StateChangedEvent));
        ThumbDownCommand?.Execute(State);
    }
}
