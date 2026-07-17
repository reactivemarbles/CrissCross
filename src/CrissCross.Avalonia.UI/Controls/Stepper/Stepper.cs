// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a workflow stepper or wizard-progress surface.</summary>
public class Stepper : ItemsControl
{
    /// <summary>Property for <see cref="State"/>.</summary>
    public static readonly StyledProperty<StepperState?> StateProperty = AvaloniaProperty.Register<
        Stepper,
        StepperState?
    >(nameof(State));

    /// <summary>Property for <see cref="CurrentKey"/>.</summary>
    public static readonly StyledProperty<string?> CurrentKeyProperty = AvaloniaProperty.Register<Stepper, string?>(
        nameof(CurrentKey),
        defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Property for <see cref="StepRequestedCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> StepRequestedCommandProperty = AvaloniaProperty.Register<
        Stepper,
        ICommand?
    >(nameof(StepRequestedCommand));

    /// <summary>Property for <see cref="FinishCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> FinishCommandProperty = AvaloniaProperty.Register<
        Stepper,
        ICommand?
    >(nameof(FinishCommand));

    /// <summary>Property for <see cref="CancelCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> CancelCommandProperty = AvaloniaProperty.Register<
        Stepper,
        ICommand?
    >(nameof(CancelCommand));

    /// <summary>Initializes a new instance of the <see cref="Stepper"/> class.</summary>
    public Stepper()
    {
        PreviousStepCommand = new StepNavigationCommand(RequestPreviousStep, () => State?.CanGoPrevious == true);
        NextStepCommand = new StepNavigationCommand(RequestNextStep, () => State?.CanGoNext == true);
        JumpToStepCommand = new StepParameterCommand(RequestStep);
    }

    /// <summary>Gets or sets the shared stepper state projected by the control.</summary>
    public StepperState? State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets or sets the currently requested step key.</summary>
    public string? CurrentKey
    {
        get => GetValue(CurrentKeyProperty);
        set => SetValue(CurrentKeyProperty, value);
    }

    /// <summary>Gets or sets the command invoked when the control requests a step transition.</summary>
    public ICommand? StepRequestedCommand
    {
        get => GetValue(StepRequestedCommandProperty);
        set => SetValue(StepRequestedCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked by consumers to finish the workflow.</summary>
    public ICommand? FinishCommand
    {
        get => GetValue(FinishCommandProperty);
        set => SetValue(FinishCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked by consumers to cancel the workflow.</summary>
    public ICommand? CancelCommand
    {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    /// <summary>Gets the command that requests the previous step.</summary>
    public ICommand PreviousStepCommand { get; }

    /// <summary>Gets the command that requests the next step.</summary>
    public ICommand NextStepCommand { get; }

    /// <summary>Gets the command that requests a specific step key or descriptor.</summary>
    public ICommand JumpToStepCommand { get; }

    /// <summary>Requests navigation to the specified step key.</summary>
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
        CurrentKey = key;

        if (step.EnterCommand?.CanExecute(step) == true)
        {
            step.EnterCommand.Execute(step);
        }

        if (StepRequestedCommand?.CanExecute(key) != true)
        {
            return;
        }

        StepRequestedCommand.Execute(key);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property != StateProperty || change.GetNewValue<StepperState?>() is not { } state)
        {
            return;
        }

        CurrentKey = state.CurrentKey;
        ItemsSource = state.Steps;
    }

    /// <summary>Provides the RequestPreviousStep member.</summary>
    private void RequestPreviousStep()
    {
        if (State is not { CanGoPrevious: true } state)
        {
            return;
        }

        RequestStep(state.Steps[state.CurrentIndex - 1].Key);
    }

    /// <summary>Provides the RequestNextStep member.</summary>
    private void RequestNextStep()
    {
        if (State is not { CanGoNext: true } state)
        {
            return;
        }

        RequestStep(state.Steps[state.CurrentIndex + 1].Key);
    }

    /// <summary>Provides the StepNavigationCommand member.</summary>
    /// <param name="execute">The execute value.</param>
    /// <param name="canExecute">The canExecute value.</param>
    private sealed class StepNavigationCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        /// <summary>Provides the _execute member.</summary>
        private readonly Action _execute = execute;

        /// <summary>Provides the documented member.</summary>
        private readonly Func<bool> _canExecute = canExecute;

        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => _canExecute();

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            _execute();
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Provides the StepParameterCommand member.</summary>
    private sealed class StepParameterCommand : ICommand
    {
        /// <summary>Provides the documented member.</summary>
        private readonly Action<string?> _execute;

        /// <summary>Initializes a new instance of the <see cref="StepParameterCommand"/> class.</summary>
        /// <param name="execute">The execute value.</param>
        public StepParameterCommand(Action<string?> execute) => _execute = execute;

        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => parameter is StepDescriptor { IsAvailable: true } or string;

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        public void Execute(object? parameter)
        {
            var key = parameter switch
            {
                StepDescriptor step => step.Key,
                string value => value,
                _ => null,
            };

            _execute(key);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
