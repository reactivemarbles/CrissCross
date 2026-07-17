// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Presents aggregate validation messages and exposes a field navigation command hook.</summary>
public class ValidationSummary : ItemsControl
{
    /// <summary>Property for <see cref="SummaryState"/>.</summary>
    public static readonly DependencyProperty SummaryStateProperty = DependencyProperty.Register(
        nameof(SummaryState),
        typeof(ValidationSummaryState),
        typeof(ValidationSummary),
        new PropertyMetadata(null, OnSummaryStateChanged));

    /// <summary>Property for <see cref="NavigateToFieldCommand"/>.</summary>
    public static readonly DependencyProperty NavigateToFieldCommandProperty = DependencyProperty.Register(
        nameof(NavigateToFieldCommand),
        typeof(ICommand),
        typeof(ValidationSummary),
        new PropertyMetadata(null));

    /// <summary>Gets or sets the aggregate validation state shown by this summary.</summary>
    public ValidationSummaryState? SummaryState
    {
        get => (ValidationSummaryState?)GetValue(SummaryStateProperty);
        set => SetValue(SummaryStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked to navigate to the selected field message.</summary>
    public ICommand? NavigateToFieldCommand
    {
        get => (ICommand?)GetValue(NavigateToFieldCommandProperty);
        set => SetValue(NavigateToFieldCommandProperty, value);
    }

    /// <summary>Provides the OnSummaryStateChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnSummaryStateChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not ValidationSummary summary)
        {
            return;
        }

        summary.SetCurrentValue(
            ItemsSourceProperty,
            args.NewValue is ValidationSummaryState state ? state.Messages : null);
    }
}
