// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Displays progress through a sequence of steps.
/// </summary>
public class Stepper : ContentView
{
    /// <summary>
    /// Bindable property for <see cref="StepperState"/>.
    /// </summary>
    public static readonly BindableProperty StepperStateProperty = BindableProperty.Create(
        nameof(StepperState),
        typeof(StepperState),
        typeof(Stepper));

    /// <summary>
    /// Bindable property for <see cref="StepCommand"/>.
    /// </summary>
    public static readonly BindableProperty StepCommandProperty = BindableProperty.Create(
        nameof(StepCommand),
        typeof(ICommand),
        typeof(Stepper));

    /// <summary>
    /// Gets or sets the shared CrissCross state projected by this control.
    /// </summary>
    public StepperState? StepperState
    {
        get => (StepperState?)GetValue(StepperStateProperty);
        set => SetValue(StepperStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked by the control surface.
    /// </summary>
    public ICommand? StepCommand
    {
        get => (ICommand?)GetValue(StepCommandProperty);
        set => SetValue(StepCommandProperty, value);
    }
}
