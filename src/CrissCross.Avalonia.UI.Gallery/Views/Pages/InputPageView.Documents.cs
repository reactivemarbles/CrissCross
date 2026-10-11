// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI.Gallery.Views.Pages;

/// <summary>Provides native Word document verification actions.</summary>
public partial class InputPageView
{
    /// <summary>The native Word package filter.</summary>
    private static readonly FilePickerFileType WordDocumentType = new("Word document") { Patterns = ["*.docx"] };

    /// <summary>Opens a native Word package in the paged editor.</summary>
    /// <param name="sender">The button.</param>
    /// <param name="e">The click event.</param>
    private async void OnRichTextOpenDocxClick(object? sender, RoutedEventArgs e) => await OpenWordDocumentAsync();

    /// <summary>Exports the edited document as a native Word package.</summary>
    /// <param name="sender">The button.</param>
    /// <param name="e">The click event.</param>
    private async void OnRichTextSaveDocxClick(object? sender, RoutedEventArgs e) => await SaveWordDocumentAsync();

    /// <summary>Reads a package chosen by the user.</summary>
    /// <returns>The asynchronous operation.</returns>
    private async Task OpenWordDocumentAsync()
    {
        if (TopLevel.GetTopLevel(this)?.StorageProvider is not { } storage || DemoRichTextBox is not { } editor)
        {
            return;
        }

        try
        {
            var files = await storage.OpenFilePickerAsync(new() { Title = "Open Word document", FileTypeFilter = [WordDocumentType] });
            if (files.Count == 0)
            {
                return;
            }

            await using var stream = await files[0].OpenReadAsync();
            editor.Load(stream, RichTextDataFormat.Docx);
            UpdateDisplayPreview($"Opened {files[0].Name}.");
        }
        catch (Exception exception) when (exception is IOException or NotSupportedException or ArgumentException or XmlException)
        {
            UpdateStatus($"Could not open Word document: {exception.Message}");
        }
    }

    /// <summary>Writes a package to a destination chosen by the user.</summary>
    /// <returns>The asynchronous operation.</returns>
    private async Task SaveWordDocumentAsync()
    {
        if (TopLevel.GetTopLevel(this)?.StorageProvider is not { } storage || DemoRichTextBox is not { } editor)
        {
            return;
        }

        try
        {
            var file = await storage.SaveFilePickerAsync(new() { Title = "Save Word document", SuggestedFileName = "RichTextDocument.docx", FileTypeChoices = [WordDocumentType] });
            if (file is null)
            {
                return;
            }

            await using var package = new MemoryStream();
            editor.Save(package, RichTextDataFormat.Docx);
            package.Position = 0;
            await using var stream = await file.OpenWriteAsync();
            if (stream.CanSeek)
            {
                stream.SetLength(0);
            }

            await package.CopyToAsync(stream);
            UpdateStatus($"Saved {file.Name}.");
        }
        catch (Exception exception) when (exception is IOException or NotSupportedException or ArgumentException or XmlException)
        {
            UpdateStatus($"Could not save Word document: {exception.Message}");
        }
    }
}
