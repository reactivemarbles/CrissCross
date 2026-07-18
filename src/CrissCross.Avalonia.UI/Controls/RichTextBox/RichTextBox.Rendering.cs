// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the Rendering members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
    /// <summary>Provides the UpdateFormattedPresenter member.</summary>
    private void UpdateFormattedPresenter()
    {
        if (_formattedPresenter is null)
        {
            return;
        }

        Document.Refresh();
        _formattedPresenter.Document = Document;
        _formattedPresenter.UpdateInlines();
        UpdateImageOverlay();
        UpdateDisplayMode();
    }

    /// <summary>Updates the composed image layer from document image segments.</summary>
    private void UpdateImageOverlay()
    {
        if (_imageOverlay is null || _formattedPresenter is null)
        {
            return;
        }

        _imageOverlay.Children.Clear();
        var lineHeight = GetEstimatedLineHeight();
        var stackedImageOffset = 0D;
        var overlayHeight = 0D;

        foreach (var segment in Document.Segments.Where(segment => segment.IsImage))
        {
            var image = _formattedPresenter.CreateImageElement(segment);
            if (image is null)
            {
                continue;
            }

            var precedingLineCount =
                Document.Segments.Count(candidate => candidate.StartIndex < segment.StartIndex && candidate.IsLineBreak)
                + (
                    Document.Segments.Count(candidate =>
                        candidate.StartIndex < segment.StartIndex
                        && candidate.IsParagraphBreak) * ParagraphBreakLineCount)
                + 1;
            var top = (precedingLineCount * lineHeight) + stackedImageOffset;
            var imageHeight = double.IsNaN(image.Height) ? DefaultImageOverlayHeight : image.Height;

            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, top);
            _imageOverlay.Children.Add(image);

            stackedImageOffset += imageHeight + lineHeight;
            overlayHeight = Math.Max(overlayHeight, top + imageHeight);
        }

        _imageOverlay.Height = overlayHeight;
    }

    /// <summary>Provides the UpdateDisplayMode member.</summary>
    private void UpdateDisplayMode()
    {
        if (_editingTextBox is null || _formattedPresenter is null)
        {
            return;
        }

        var showEditing = ShouldShowEditingSurface();
        var showRichEditingSurface = showEditing && Document.Segments.Any(segment => segment.IsImage);
        UpdateEditingSurface(showEditing, showRichEditingSurface);
        UpdateFormattedSurface(showEditing, showRichEditingSurface);
    }

    /// <summary>Applies state to the editable text surface.</summary>
    /// <param name="showEditing">Whether editing is active.</param>
    /// <param name="showRichEditingSurface">Whether formatted content overlays the editor.</param>
    private void UpdateEditingSurface(bool showEditing, bool showRichEditingSurface)
    {
        if (_editingTextBox is null)
        {
            return;
        }

        _editingTextBox.IsVisible = showEditing;
        _editingTextBox.IsHitTestVisible = showEditing;
        _editingTextBox.Opacity = showEditing ? 1 : 0;
        _editingTextBox.Foreground = showRichEditingSurface ? Brushes.Transparent : Foreground;
    }

    /// <summary>Applies state to the formatted text and image surfaces.</summary>
    /// <param name="showEditing">Whether editing is active.</param>
    /// <param name="showRichEditingSurface">Whether formatted content overlays the editor.</param>
    private void UpdateFormattedSurface(bool showEditing, bool showRichEditingSurface)
    {
        if (_formattedPresenter is null)
        {
            return;
        }

        var showFormattedSurface = !showEditing || showRichEditingSurface;
        _formattedPresenter.IsVisible = showFormattedSurface;
        _formattedPresenter.IsHitTestVisible = !showEditing && IsDocumentEnabled;
        _imageOverlay?.SetCurrentValue(IsVisibleProperty, showFormattedSurface);
        _displayScrollViewer?.SetCurrentValue(IsHitTestVisibleProperty, !showEditing && IsDocumentEnabled);
    }

    /// <summary>Determines whether the composed text input surface should be active.</summary>
    /// <returns><see langword="true"/> when the editing surface should be shown.</returns>
    private bool ShouldShowEditingSurface() =>
        EditMode switch
        {
            RichTextEditMode.Edit => true,
            RichTextEditMode.Display => false,
            _ => _editingTextBox?.IsFocused == true || IsFocused,
        };

    /// <summary>Provides the OnDragOver member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        e.DragEffects = CanAcceptDropData(e.DataTransfer) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    /// <summary>Provides the OnDrop member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (!CanAcceptDropData(e.DataTransfer))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        MoveSelectionToDropPoint(e.GetPosition(this));
        var inserted = await TryInsertDropDataAsync(e.DataTransfer);
        e.DragEffects = inserted ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    /// <summary>Provides the CanAcceptDropData member.</summary>
    /// <param name="dataTransfer">The dataTransfer value.</param>
    /// <returns>The result.</returns>
    private bool CanAcceptDropData(IDataTransfer dataTransfer)
    {
        if (!IsDragDropEnabled || IsReadOnlyInternal)
        {
            return false;
        }

        var textPayload = RichTextHelpers.NormalizeClipboardText(dataTransfer.TryGetText());
        if (!string.IsNullOrWhiteSpace(textPayload))
        {
            return true;
        }

        var files = dataTransfer.TryGetFiles();
        if (files is null)
        {
            return false;
        }

        foreach (var file in files)
        {
            var path = RichTextHelpers.GetStorageItemPath(file);
            if (RichTextHelpers.IsSupportedImagePath(path))
            {
                return IsImageDropEnabled;
            }

            if (file is IStorageFile && RichTextHelpers.IsSupportedTextFilePath(path))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Provides the TryInsertDropDataAsync member.</summary>
    /// <param name="dataTransfer">The dataTransfer value.</param>
    /// <returns>The result.</returns>
    private async Task<bool> TryInsertDropDataAsync(IDataTransfer dataTransfer)
    {
        var files = dataTransfer.TryGetFiles();
        if (files is { Length: > 0 })
        {
            var inserted = false;
            foreach (var file in files)
            {
                if (file is IStorageFile storageFile)
                {
                    if (await TryDropStorageFileAsync(storageFile).ConfigureAwait(true))
                    {
                        inserted = true;
                    }
                }
                else if (
                    RichTextHelpers.IsSupportedImagePath(RichTextHelpers.GetStorageItemPath(file))
                    && TryDropImage(file.Path.AbsoluteUri))
                {
                    inserted = true;
                }
            }

            return inserted;
        }

        return TryDropText(dataTransfer.TryGetText());
    }

    /// <summary>Provides the TryDropStorageFileAsync member.</summary>
    /// <param name="file">The file value.</param>
    /// <returns>The result.</returns>
    private async Task<bool> TryDropStorageFileAsync(IStorageFile file)
    {
        var path = RichTextHelpers.GetStorageItemPath(file);
        if (RichTextHelpers.IsSupportedImagePath(path))
        {
            var imageSource = await RichTextHelpers.TryCreateImageSourceAsync(file).ConfigureAwait(true);
            return !string.IsNullOrWhiteSpace(imageSource) && TryDropImage(imageSource);
        }

        if (!RichTextHelpers.IsSupportedTextFilePath(path))
        {
            return false;
        }

        var properties = await file.GetBasicPropertiesAsync().ConfigureAwait(true);
        if (properties.Size is { } size && size > (ulong)MaxDroppedTextFileBytes)
        {
            return false;
        }

        await using var readStream = await file.OpenReadAsync().ConfigureAwait(true);
        if (readStream.CanSeek && readStream.Length > MaxDroppedTextFileBytes)
        {
            return false;
        }

        using var reader = new StreamReader(readStream, Encoding.UTF8, true);
        var text = await reader.ReadToEndAsync().ConfigureAwait(true);
        return Encoding.UTF8.GetByteCount(text) > MaxDroppedTextFileBytes ? false : TryDropText(text);
    }

    /// <summary>Provides the MoveSelectionToDropPoint member.</summary>
    /// <param name="point">The point value.</param>
    private void MoveSelectionToDropPoint(Point point)
    {
        var position = GetPositionFromPoint(point, snapToText: true);
        if (position is null)
        {
            return;
        }

        Select(position.Offset, 0);
    }

    /// <summary>Provides the TryGetRenderedDocumentOffsetFromPoint member.</summary>
    /// <param name="point">The point value.</param>
    /// <param name="offset">The offset value.</param>
    /// <returns>The result.</returns>
    private bool TryGetRenderedDocumentOffsetFromPoint(Point point, out int offset)
    {
        offset = 0;

        if (_editingTextBox is not null)
        {
            var textPresenter = _editingTextBox.GetVisualDescendants().OfType<TextPresenter>().FirstOrDefault();
            if (TryHitTestTextLayout(textPresenter, point, out offset))
            {
                return true;
            }
        }

        return TryHitTestTextLayout(_formattedPresenter, point, out offset);
    }

    /// <summary>Provides the TryHitTestTextLayout member.</summary>
    /// <param name="visual">The visual value.</param>
    /// <param name="point">The point value.</param>
    /// <param name="offset">The offset value.</param>
    /// <returns>The result.</returns>
    private bool TryHitTestTextLayout(Visual? visual, Point point, out int offset)
    {
        offset = 0;
        if (visual is null)
        {
            return false;
        }

        var textLayout = visual is TextPresenter presenter
            ? presenter.TextLayout
            : (visual as FormattedTextPresenter)?.TextLayout;

        if (textLayout is null)
        {
            return false;
        }

        var translatedPoint = this.TranslatePoint(point, visual);
        if (!translatedPoint.HasValue)
        {
            return false;
        }

        var hitTest = textLayout.HitTestPoint(translatedPoint.Value);
        offset = Math.Clamp(hitTest.TextPosition, 0, Document.Length);
        return hitTest.IsInside || new Rect(visual.Bounds.Size).Contains(translatedPoint.Value);
    }

    /// <summary>Provides the GetTextHitTestBounds member.</summary>
    /// <returns>The result.</returns>
    private Rect GetTextHitTestBounds()
    {
        if (_editingTextBox is not null)
        {
            return _editingTextBox.Bounds;
        }

        if (_formattedPresenter is not null)
        {
            return _formattedPresenter.Bounds;
        }

        var width =
            Bounds.Width > 0
                ? Bounds.Width
                : Math.Max(Document.Length * GetEstimatedCharacterWidth(), GetEstimatedCharacterWidth());
        var height = Bounds.Height > 0 ? Bounds.Height : GetEstimatedLineHeight();
        return new Rect(0, 0, width, height);
    }

    /// <summary>Provides the EstimateDocumentOffsetFromPoint member.</summary>
    /// <param name="point">The point value.</param>
    /// <param name="bounds">The bounds value.</param>
    /// <returns>The result.</returns>
    private int EstimateDocumentOffsetFromPoint(Point point, Rect bounds)
    {
        var text = Document.PlainText;
        if (text.Length == 0)
        {
            return 0;
        }

        var characterWidth = GetEstimatedCharacterWidth();
        var lineHeight = GetEstimatedLineHeight();
        var relativeX = Math.Max(0, point.X - bounds.X);
        var relativeY = Math.Max(0, point.Y - bounds.Y);
        var targetLine = Math.Max(0, (int)Math.Floor(relativeY / lineHeight));
        var targetColumn = Math.Max(0, (int)Math.Round(relativeX / characterWidth, MidpointRounding.AwayFromZero));
        var currentLine = 0;
        var currentColumn = 0;

        for (var offset = 0; offset < text.Length; offset++)
        {
            if (currentLine == targetLine && currentColumn >= targetColumn)
            {
                return Math.Clamp(offset, 0, Document.Length);
            }

            if (text[offset] == '\n')
            {
                if (currentLine == targetLine)
                {
                    return offset;
                }

                currentLine++;
                currentColumn = 0;
                continue;
            }

            currentColumn++;
        }

        return Document.Length;
    }

    /// <summary>Provides the GetEstimatedCharacterWidth member.</summary>
    /// <returns>The result.</returns>
    private double GetEstimatedCharacterWidth() =>
        Math.Max(1, FontSize > 0 ? FontSize * CharacterWidthFontScale : DefaultCharacterWidth);

    /// <summary>Provides the GetEstimatedLineHeight member.</summary>
    /// <returns>The result.</returns>
    private double GetEstimatedLineHeight() =>
        Math.Max(1, FontSize > 0 ? FontSize * LineHeightFontScale : DefaultLineHeight);
}
