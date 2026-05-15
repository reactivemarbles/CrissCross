// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a workflow stepper or wizard-progress surface.
/// </summary>
public class Stepper : ItemsControl
{
    /// <summary>
    /// Property for <see cref="State"/>.
    /// </summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(StepperState),
        typeof(Stepper),
        new PropertyMetadata(null, OnStateChanged));

    /// <summary>
    /// Property for <see cref="CurrentKey"/>.
    /// </summary>
    public static readonly DependencyProperty CurrentKeyProperty = DependencyProperty.Register(
        nameof(CurrentKey),
        typeof(string),
        typeof(Stepper),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Property for <see cref="StepRequestedCommand"/>.
    /// </summary>
    public static readonly DependencyProperty StepRequestedCommandProperty = DependencyProperty.Register(
        nameof(StepRequestedCommand),
        typeof(ICommand),
        typeof(Stepper),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="FinishCommand"/>.
    /// </summary>
    public static readonly DependencyProperty FinishCommandProperty = DependencyProperty.Register(
        nameof(FinishCommand),
        typeof(ICommand),
        typeof(Stepper),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="CancelCommand"/>.
    /// </summary>
    public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
        nameof(CancelCommand),
        typeof(ICommand),
        typeof(Stepper),
        new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="Stepper"/> class.
    /// </summary>
    public Stepper()
    {
        PreviousStepCommand = new StepNavigationCommand(RequestPreviousStep, () => State?.CanGoPrevious == true);
        NextStepCommand = new StepNavigationCommand(RequestNextStep, () => State?.CanGoNext == true);
        JumpToStepCommand = new StepParameterCommand(RequestStep);
    }

    /// <summary>
    /// Gets or sets the shared stepper state projected by the control.
    /// </summary>
    public StepperState? State
    {
        get => (StepperState?)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently requested step key.
    /// </summary>
    public string? CurrentKey
    {
        get => (string?)GetValue(CurrentKeyProperty);
        set => SetValue(CurrentKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when the control requests a step transition.
    /// </summary>
    public ICommand? StepRequestedCommand
    {
        get => (ICommand?)GetValue(StepRequestedCommandProperty);
        set => SetValue(StepRequestedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked by consumers to finish the workflow.
    /// </summary>
    public ICommand? FinishCommand
    {
        get => (ICommand?)GetValue(FinishCommandProperty);
        set => SetValue(FinishCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked by consumers to cancel the workflow.
    /// </summary>
    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    /// <summary>
    /// Gets the command that requests the previous step.
    /// </summary>
    public ICommand PreviousStepCommand { get; }

    /// <summary>
    /// Gets the command that requests the next step.
    /// </summary>
    public ICommand NextStepCommand { get; }

    /// <summary>
    /// Gets the command that requests a specific step key or descriptor.
    /// </summary>
    public ICommand JumpToStepCommand { get; }

    /// <summary>
    /// Requests navigation to the specified step key.
    /// </summary>
    /// <param name="stepKey">The stable step key.</param>
    public void RequestStep(string? stepKey)
    {
        var normalizedKey = (stepKey ?? string.Empty).Trim();
        if (normalizedKey.Length == 0)
        {
            return;
        }

        var step = State?.GetStep(normalizedKey);
        if (step?.IsAvailable != true)
        {
            return;
        }

        var key = step.Key;
        SetCurrentValue(CurrentKeyProperty, key);

        if (step.EnterCommand?.CanExecute(step) == true)
        {
            step.EnterCommand.Execute(step);
        }

        if (StepRequestedCommand?.CanExecute(key) == true)
        {
            StepRequestedCommand.Execute(key);
        }
    }

    private static void OnStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is Stepper stepper && args.NewValue is StepperState state)
        {
            stepper.SetCurrentValue(CurrentKeyProperty, state.CurrentKey);
            stepper.SetCurrentValue(ItemsSourceProperty, state.Steps);
        }
    }

    private void RequestPreviousStep()
    {
        if (State is { CanGoPrevious: true } state)
        {
            RequestStep(state.Steps[state.CurrentIndex - 1].Key);
        }
    }

    private void RequestNextStep()
    {
        if (State is { CanGoNext: true } state)
        {
            RequestStep(state.Steps[state.CurrentIndex + 1].Key);
        }
    }

    private sealed class StepNavigationCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public StepNavigationCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute();
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private sealed class StepParameterCommand : ICommand
    {
        private readonly Action<string?> _execute;

        public StepParameterCommand(Action<string?> execute) => _execute = execute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => parameter is StepDescriptor { IsAvailable: true } or string;

        public void Execute(object? parameter)
        {
            var key = parameter switch
            {
                StepDescriptor step => step.Key,
                string value => value,
                _ => null
            };

            _execute(key);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
