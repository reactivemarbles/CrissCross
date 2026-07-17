// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Configures editing, values, commands, and validation for a property descriptor.</summary>
public sealed class PropertyDescriptorOptions
{
    /// <summary>Gets or sets the category used for grouping.</summary>
    public string? Category { get; set; }

    /// <summary>Gets or sets the editor kind used by platform presenters.</summary>
    public PropertyEditorKind EditorKind { get; set; } = PropertyEditorKind.Text;

    /// <summary>Gets or sets the current property value snapshot.</summary>
    public object? Value { get; set; }

    /// <summary>Gets or sets the original value snapshot used for modified-state calculation.</summary>
    public object? OriginalValue { get; set; }

    /// <summary>Gets or sets a value indicating whether the property is read-only.</summary>
    public bool IsReadOnly { get; set; }

    /// <summary>Gets or sets explicit choices for enum-like editors.</summary>
    public IReadOnlyList<object?>? Choices { get; set; }

    /// <summary>Gets or sets an optional command used to set a new value.</summary>
    public ICommand? SetValueCommand { get; set; }

    /// <summary>Gets or sets an optional command used to reset the property.</summary>
    public ICommand? ResetCommand { get; set; }

    /// <summary>Gets or sets validation messages for the property.</summary>
    public IReadOnlyList<ValidationMessage>? ValidationMessages { get; set; }

    /// <summary>Gets or sets an optional platform template key for custom editors.</summary>
    public string? TemplateKey { get; set; }
}
