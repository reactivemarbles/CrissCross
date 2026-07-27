// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents a command-aware button that can project execution state, progress, and error content.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CommandButton : Button
{
    /// <summary>Property for <see cref="State"/>.</summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(CommandButtonState),
        typeof(CommandButton),
        new(CommandButtonState.Idle));

    /// <summary>Property for <see cref="IsExecuting"/>.</summary>
    public static readonly DependencyProperty IsExecutingProperty = DependencyProperty.Register(
        nameof(IsExecuting),
        typeof(bool),
        typeof(CommandButton),
        new(false, OnIsExecutingChanged));

    /// <summary>Property for <see cref="Progress"/>.</summary>
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress),
        typeof(double?),
        typeof(CommandButton),
        new(null));

    /// <summary>Property for <see cref="ExecutingContent"/>.</summary>
    public static readonly DependencyProperty ExecutingContentProperty = DependencyProperty.Register(
        nameof(ExecutingContent),
        typeof(object),
        typeof(CommandButton),
        new("Working..."));

    /// <summary>Property for <see cref="ErrorContent"/>.</summary>
    public static readonly DependencyProperty ErrorContentProperty = DependencyProperty.Register(
        nameof(ErrorContent),
        typeof(object),
        typeof(CommandButton),
        new(null));

    /// <summary>Gets or sets the command execution state displayed by the button.</summary>
    public CommandButtonState State
    {
        get => (CommandButtonState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the command is executing.</summary>
    public bool IsExecuting
    {
        get => (bool)GetValue(IsExecutingProperty);
        set => SetValue(IsExecutingProperty, value);
    }

    /// <summary>Gets or sets optional normalized progress from 0.0 to 1.0.</summary>
    public double? Progress
    {
        get => (double?)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>Gets or sets the content shown while the command is executing.</summary>
    public object? ExecutingContent
    {
        get => GetValue(ExecutingContentProperty);
        set => SetValue(ExecutingContentProperty, value);
    }

    /// <summary>Gets or sets optional content shown for a failed command.</summary>
    public object? ErrorContent
    {
        get => GetValue(ErrorContentProperty);
        set => SetValue(ErrorContentProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Provides the OnIsExecutingChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnIsExecutingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not CommandButton button || args.NewValue is false)
        {
            return;
        }

        button.SetCurrentValue(StateProperty, CommandButtonState.Executing);
    }
}
