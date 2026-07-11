// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays grouped validation messages for a form or editor surface.</summary>
public class ValidationSummary : ContentView
{
    /// <summary>Bindable property for <see cref="SummaryState"/>.</summary>
    public static readonly BindableProperty SummaryStateProperty = BindableProperty.Create(
        nameof(SummaryState),
        typeof(ValidationSummaryState),
        typeof(ValidationSummary));

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public ValidationSummaryState? SummaryState
    {
        get => (ValidationSummaryState?)GetValue(SummaryStateProperty);
        set => SetValue(SummaryStateProperty, value);
    }
}
