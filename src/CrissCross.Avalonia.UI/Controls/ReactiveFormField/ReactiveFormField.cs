// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Presents a form input with shared validation state, helper text, and required-field metadata.</summary>
public class ReactiveFormField : ContentControl
{
    /// <summary>Property for <see cref="Header"/>.</summary>
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<ReactiveFormField, object?>(nameof(Header));

    /// <summary>Property for <see cref="HelperText"/>.</summary>
    public static readonly StyledProperty<string?> HelperTextProperty = AvaloniaProperty.Register<ReactiveFormField, string?>(nameof(HelperText));

    /// <summary>Property for <see cref="FieldKey"/>.</summary>
    public static readonly StyledProperty<string?> FieldKeyProperty = AvaloniaProperty.Register<ReactiveFormField, string?>(nameof(FieldKey));

    /// <summary>Property for <see cref="State"/>.</summary>
    public static readonly StyledProperty<FormFieldState> StateProperty = AvaloniaProperty.Register<ReactiveFormField, FormFieldState>(nameof(State), FormFieldState.Normal);

    /// <summary>Property for <see cref="Messages"/>.</summary>
    public static readonly StyledProperty<IEnumerable<ValidationMessage>?> MessagesProperty = AvaloniaProperty.Register<ReactiveFormField, IEnumerable<ValidationMessage>?>(nameof(Messages));

    /// <summary>Property for <see cref="IsRequired"/>.</summary>
    public static readonly StyledProperty<bool> IsRequiredProperty = AvaloniaProperty.Register<ReactiveFormField, bool>(nameof(IsRequired));

    /// <summary>Gets or sets the form field header content.</summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>Gets or sets optional helper text displayed below the field.</summary>
    public string? HelperText
    {
        get => GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    /// <summary>Gets or sets the stable validation field key represented by this field.</summary>
    public string? FieldKey
    {
        get => GetValue(FieldKeyProperty);
        set => SetValue(FieldKeyProperty, value);
    }

    /// <summary>Gets or sets the current form field validation state.</summary>
    public FormFieldState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets or sets validation messages associated with this field.</summary>
    public IEnumerable<ValidationMessage>? Messages
    {
        get => GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the field should be presented as required.</summary>
    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }
}
