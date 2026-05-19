// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Defines explicit rich text import and export formats.
/// </summary>
public enum RichTextDataFormat
{
    /// <summary>
    /// Plain text content with formatting removed.
    /// </summary>
    PlainText,

    /// <summary>
    /// HTML fragment content.
    /// </summary>
    Html,

    /// <summary>
    /// Markdown content.
    /// </summary>
    Markdown,

    /// <summary>
    /// Rich Text Format content.
    /// </summary>
    Rtf
}
