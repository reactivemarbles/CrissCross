// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Displays validation and metadata for a reactive form field.
/// </summary>
public class ReactiveFormField : ContentView
{
    /// <summary>
    /// Bindable property for <see cref="FieldState"/>.
    /// </summary>
    public static readonly BindableProperty FieldStateProperty = BindableProperty.Create(
        nameof(FieldState),
        typeof(FormFieldState),
        typeof(ReactiveFormField));

    /// <summary>
    /// Gets or sets the shared CrissCross state projected by this control.
    /// </summary>
    public FormFieldState? FieldState
    {
        get => (FormFieldState?)GetValue(FieldStateProperty);
        set => SetValue(FieldStateProperty, value);
    }
}
