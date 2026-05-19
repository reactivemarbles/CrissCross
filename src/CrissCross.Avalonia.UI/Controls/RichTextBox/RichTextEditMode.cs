// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Defines whether <see cref="RichTextBox"/> behaves as an editor, a display surface, or switches on focus.
/// </summary>
public enum RichTextEditMode
{
    /// <summary>
    /// Editing surface is enabled whenever the control is enabled.
    /// </summary>
    Edit,

    /// <summary>
    /// Display surface is used and mutation commands are disabled.
    /// </summary>
    Display,

    /// <summary>
    /// Display surface is shown until the control receives focus.
    /// </summary>
    EditOnFocus
}
