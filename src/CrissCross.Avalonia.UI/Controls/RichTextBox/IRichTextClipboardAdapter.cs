// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides deterministic clipboard access for <see cref="RichTextBox"/> plain-text and HTML operations.
/// </summary>
public interface IRichTextClipboardAdapter
{
    /// <summary>
    /// Gets a value indicating whether plain text is available.
    /// </summary>
    bool ContainsPlainText { get; }

    /// <summary>
    /// Gets a value indicating whether HTML text is available.
    /// </summary>
    bool ContainsHtml { get; }

    /// <summary>
    /// Gets a value indicating whether an image payload is available.
    /// </summary>
    bool ContainsImage { get; }

    /// <summary>
    /// Gets or sets the plain text clipboard payload.
    /// </summary>
    string? PlainText { get; set; }

    /// <summary>
    /// Gets or sets the HTML clipboard payload.
    /// </summary>
    string? HtmlText { get; set; }

    /// <summary>
    /// Gets or sets an image source payload, such as a file URI or data URI.
    /// </summary>
    string? ImageSource { get; set; }

    /// <summary>
    /// Writes a plain text clipboard payload.
    /// </summary>
    /// <param name="text">The plain text to write.</param>
    void SetPlainText(string? text);

    /// <summary>
    /// Writes an HTML clipboard payload.
    /// </summary>
    /// <param name="html">The HTML text to write.</param>
    void SetHtml(string? html);

    /// <summary>
    /// Writes an image source clipboard payload.
    /// </summary>
    /// <param name="imageSource">The image source to write.</param>
    void SetImage(string? imageSource);
}
