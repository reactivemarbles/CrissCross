// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows.Controls;
#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Presents a form input with shared validation state, helper text, and required-field metadata.</summary>
public class ReactiveFormField : ContentControl
{
    /// <summary>Property for <see cref="Header"/>.</summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(ReactiveFormField),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="HelperText"/>.</summary>
    public static readonly DependencyProperty HelperTextProperty = DependencyProperty.Register(
        nameof(HelperText),
        typeof(string),
        typeof(ReactiveFormField),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="FieldKey"/>.</summary>
    public static readonly DependencyProperty FieldKeyProperty = DependencyProperty.Register(
        nameof(FieldKey),
        typeof(string),
        typeof(ReactiveFormField),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="State"/>.</summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(FormFieldState),
        typeof(ReactiveFormField),
        new PropertyMetadata(FormFieldState.Normal));

    /// <summary>Property for <see cref="Messages"/>.</summary>
    public static readonly DependencyProperty MessagesProperty = DependencyProperty.Register(
        nameof(Messages),
        typeof(IEnumerable<ValidationMessage>),
        typeof(ReactiveFormField),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="IsRequired"/>.</summary>
    public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
        nameof(IsRequired),
        typeof(bool),
        typeof(ReactiveFormField),
        new PropertyMetadata(false));

    /// <summary>Gets or sets the form field header content.</summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>Gets or sets optional helper text displayed below the field.</summary>
    public string? HelperText
    {
        get => (string?)GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    /// <summary>Gets or sets the stable validation field key represented by this field.</summary>
    public string? FieldKey
    {
        get => (string?)GetValue(FieldKeyProperty);
        set => SetValue(FieldKeyProperty, value);
    }

    /// <summary>Gets or sets the current form field validation state.</summary>
    public FormFieldState State
    {
        get => (FormFieldState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets or sets validation messages associated with this field.</summary>
    public IEnumerable<ValidationMessage>? Messages
    {
        get => (IEnumerable<ValidationMessage>?)GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the field should be presented as required.</summary>
    public bool IsRequired
    {
        get => (bool)GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }
}
