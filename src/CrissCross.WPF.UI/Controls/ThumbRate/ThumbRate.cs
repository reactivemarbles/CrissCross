// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Allows to rate positively or negatively by clicking on one of the thumbs.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(ThumbRate), "ThumbRate.bmp")]
public partial class ThumbRate : System.Windows.Controls.Control
{
    /// <summary>
    /// Property for <see cref="State"/>.
    /// </summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(ThumbRateState),
        typeof(ThumbRate),
        new PropertyMetadata(ThumbRateState.None, OnStateChanged));

    /// <summary>
    /// Event property for <see cref="StateChanged"/>.
    /// </summary>
    public static readonly RoutedEvent StateChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(StateChanged),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<ThumbRate, RoutedEventArgs>),
        typeof(ThumbRate));

    /// <summary>
    /// Property for <see cref="TemplateButtonCommand"/>.
    /// </summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty = DependencyProperty.Register(
        nameof(TemplateButtonCommand),
        typeof(IReactiveCommand),
        typeof(ThumbRate),
        new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="ThumbRate"/> class.
    /// </summary>
    public ThumbRate() => SetValue(TemplateButtonCommandProperty, OnTemplateButtonClickCommand);

    /// <summary>
    /// Occurs when <see cref="State"/> is changed.
    /// </summary>
    public event TypedEventHandler<ThumbRate, RoutedEventArgs> StateChanged
    {
        add => AddHandler(StateChangedEvent, value);
        remove => RemoveHandler(StateChangedEvent, value);
    }

    /// <summary>
    /// Gets or sets the value determining the current state of the control.
    /// </summary>
    public ThumbRateState State
    {
        get => (ThumbRateState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// Gets command triggered after clicking the button.
    /// </summary>
    public IReactiveCommand TemplateButtonCommand => (IReactiveCommand)GetValue(TemplateButtonCommandProperty);

    /// <summary>
    /// Triggered by clicking a button in the control template.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    [ReactiveCommand]
    protected virtual void OnTemplateButtonClick(ThumbRateState parameter)
    {
        if (State == parameter)
        {
            State = ThumbRateState.None;
            return;
        }

        State = parameter;
    }

    /// <summary>
    /// This virtual method is called when <see cref="State" /> is changed.
    /// </summary>
    /// <param name="previousState">State of the previous.</param>
    /// <param name="currentState">State of the current.</param>
    protected virtual void OnStateChanged(ThumbRateState previousState, ThumbRateState currentState) =>
        RaiseEvent(new RoutedEventArgs(StateChangedEvent, this));

    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ThumbRate thumbRate)
        {
            return;
        }

        thumbRate.OnStateChanged((ThumbRateState)e.OldValue, (ThumbRateState)e.NewValue);
    }
}
