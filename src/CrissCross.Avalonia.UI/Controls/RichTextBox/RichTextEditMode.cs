// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Defines whether RichTextBox behaves as an editor, a display surface, or switches on focus.</summary>
public enum RichTextEditMode
{
    /// <summary>Editing surface is enabled whenever the control is enabled.</summary>
    Edit,

    /// <summary>Display surface is used and mutation commands are disabled.</summary>
    Display,

    /// <summary>Display surface is shown until the control receives focus.</summary>
    EditOnFocus,
}
