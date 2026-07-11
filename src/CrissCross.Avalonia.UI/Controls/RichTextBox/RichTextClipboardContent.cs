// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Contains the supported representations read from a platform clipboard.</summary>
/// <param name="PlainText">The plain-text representation.</param>
/// <param name="HtmlText">The HTML representation.</param>
/// <param name="ImageSource">The image data URI representation.</param>
internal readonly record struct RichTextClipboardContent(string? PlainText, string? HtmlText, string? ImageSource);
