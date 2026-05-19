// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// In-memory clipboard adapter used as the default deterministic fallback for rich text operations.
/// </summary>
public sealed class RichTextMemoryClipboardAdapter : IRichTextClipboardAdapter
{
    /// <inheritdoc/>
    public bool ContainsPlainText => !string.IsNullOrEmpty(PlainText);

    /// <inheritdoc/>
    public bool ContainsHtml => !string.IsNullOrEmpty(HtmlText);

    /// <inheritdoc/>
    public bool ContainsImage => !string.IsNullOrEmpty(ImageSource);

    /// <inheritdoc/>
    public string? PlainText { get; set; }

    /// <inheritdoc/>
    public string? HtmlText { get; set; }

    /// <inheritdoc/>
    public string? ImageSource { get; set; }

    /// <inheritdoc/>
    public void SetPlainText(string? text) => PlainText = text;

    /// <inheritdoc/>
    public void SetHtml(string? html) => HtmlText = html;

    /// <inheritdoc/>
    public void SetImage(string? imageSource) => ImageSource = imageSource;
}
