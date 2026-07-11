// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Presents aggregate validation messages and exposes a field navigation command hook.</summary>
public class ValidationSummary : ItemsControl
{
    /// <summary>Property for <see cref="SummaryState"/>.</summary>
    public static readonly StyledProperty<ValidationSummaryState?> SummaryStateProperty = AvaloniaProperty.Register<ValidationSummary, ValidationSummaryState?>(nameof(SummaryState));

    /// <summary>Property for <see cref="NavigateToFieldCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> NavigateToFieldCommandProperty = AvaloniaProperty.Register<ValidationSummary, ICommand?>(nameof(NavigateToFieldCommand));

    /// <summary>Gets or sets the aggregate validation state shown by this summary.</summary>
    public ValidationSummaryState? SummaryState
    {
        get => GetValue(SummaryStateProperty);
        set => SetValue(SummaryStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked to navigate to the selected field message.</summary>
    public ICommand? NavigateToFieldCommand
    {
        get => GetValue(NavigateToFieldCommandProperty);
        set => SetValue(NavigateToFieldCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property != SummaryStateProperty)
        {
            return;
        }

        ItemsSource = change.GetNewValue<ValidationSummaryState?>()?.Messages;
    }
}
