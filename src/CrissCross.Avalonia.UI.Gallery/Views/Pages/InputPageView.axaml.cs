// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Controls;
using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;
using RichTextBoxControl = CrissCross.Avalonia.UI.Controls.RichTextBox;

namespace CrissCross.Avalonia.UI.Gallery.Views.Pages;

/// <summary>Page demonstrating input controls.</summary>
public partial class InputPageView : ReactiveUserControl<InputPageViewModel>
{
    /// <summary>Default number of characters selected in the RichTextBox demos.</summary>
    private const int DefaultSelectionLength = 12;

    /// <summary>Maximum status text length before truncation.</summary>
    private const int StatusPreviewMaxLength = 96;

    /// <summary>Font size used by the RichTextBox formatting demo.</summary>
    private const int FormattingFontSize = 20;

    /// <summary>Inline image width used by the RichTextBox sample.</summary>
    private const int InlineImageWidth = 48;

    /// <summary>Inline image height used by the RichTextBox sample.</summary>
    private const int InlineImageHeight = 48;

    /// <summary>Provides the DemoImageDataUri member.</summary>
    private const string DemoImageDataUri = "data:image/png;base64,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";

    /// <summary>Provides the SampleMarkdown member.</summary>
    private const string SampleMarkdown = "# RichTextBox Markdown\n\n- **Bold** command sample\n- *Italic* command sample\n- <u>Underline</u> command sample\n\nThis sample mirrors WPF-style rich content workflows.";

    /// <summary>Provides the ClipboardHtmlSample member.</summary>
    private const string ClipboardHtmlSample = "<p><strong>Clipboard rich text</strong> with <em>italic</em>, <u>underline</u>, and <span style='color:#FF8C00'>foreground color</span>.</p>";

    /// <summary>Provides the RepresentativeHtml member.</summary>
    private const string RepresentativeHtml = "<h2>RichTextBox capabilities</h2><p><strong>Bold</strong>, <em>italic</em>, <u>underline</u>, <s>strikethrough</s>, <span style='font-family:Consolas;font-size:18;color:#00BFFF;background-color:#202020'>font family, size, foreground, and highlight</span>.</p><p>Use the toolbar, keyboard shortcuts, context menu, clipboard, drag/drop, and serialization buttons below.</p><ul><li>Drop plain text, HTML, image files, or data:image URIs.</li><li>Copy/cut preserves selected rich HTML and plain-text fallback.</li><li>Save/load supports HTML, Markdown, and plain-text streams.</li></ul>";

    /// <summary>Provides the _demoClipboard member.</summary>
    private readonly RichTextMemoryClipboardAdapter _demoClipboard = new();

    /// <summary>Provides the _displayModeEnabled member.</summary>
    private bool _displayModeEnabled;

    /// <summary>Initializes a new instance of the <see cref="InputPageView"/> class.</summary>
    public InputPageView()
    {
        InitializeComponent();
        _ = this.WhenActivated((CompositeDisposable _) => ViewModel ??= AppLocator.Current.GetService<InputPageViewModel>());
        RegisterXamlEventHandlersForAnalyzer();

        if (DemoRichTextBox is not { } richTextBox)
        {
            return;
        }

        LoadRepresentativeRichTextContent(richTextBox);
    }

    /// <summary>Gets the demo rich text box.</summary>
    private RichTextBoxControl? DemoRichTextBox => this.FindControl<RichTextBoxControl>(nameof(FormattingRichTextBox));

    /// <summary>Provides the EnsureDemoSelection member.</summary>
    /// <param name="richTextBox">The richTextBox value.</param>
    private static void EnsureDemoSelection(RichTextBoxControl richTextBox)
    {
        if (richTextBox.HasSelection || richTextBox.PlainText.Length == 0)
        {
            return;
        }

        richTextBox.Select(0, Math.Min(DefaultSelectionLength, richTextBox.PlainText.Length));
    }

    /// <summary>Provides the TruncateForStatus member.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private static string TruncateForStatus(string value)
    {
        var normalized = value.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal);
        return normalized.Length <= StatusPreviewMaxLength ? normalized : normalized[..StatusPreviewMaxLength] + "...";
    }

    /// <summary>References XAML click handlers so code analysis can see their generated markup usage.</summary>
    private void RegisterXamlEventHandlersForAnalyzer()
    {
        _ = (EventHandler<RoutedEventArgs>)OnRichTextResetClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextAppendPlainClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextAppendHtmlClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextMarkdownClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextInsertImageClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextReplaceSelectionClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextSelectAllClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextBoldClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextItalicClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextUnderlineClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextStrikeClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextClearFormattingClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextFontFamilyClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextFontSizeClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextForegroundClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextHighlightClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextUndoClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextRedoClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextCopyClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextCutClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextPasteSampleClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextPasteImageSampleClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextDropTextSampleClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextDropImageSampleClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextExportHtmlClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextExportMarkdownClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextSaveLoadStreamClick;
        _ = (EventHandler<RoutedEventArgs>)OnRichTextToggleDisplayModeClick;
    }

    /// <summary>Provides the LoadRepresentativeRichTextContent member.</summary>
    /// <param name="richTextBox">The richTextBox value.</param>
    private void LoadRepresentativeRichTextContent(RichTextBoxControl richTextBox)
    {
        richTextBox.ClipboardAdapter = _demoClipboard;
        richTextBox.EditMode = _displayModeEnabled ? RichTextEditMode.Display : RichTextEditMode.EditOnFocus;
        richTextBox.SetHtml(RepresentativeHtml);
        richTextBox.Select(0, Math.Min(DefaultSelectionLength, richTextBox.PlainText.Length));
        UpdateDisplayPreview("Representative sample content loaded.");
    }

    /// <summary>Provides the OnRichTextResetClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextResetClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        LoadRepresentativeRichTextContent(richTextBox);
    }

    /// <summary>Provides the OnRichTextAppendPlainClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextAppendPlainClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        richTextBox.AppendText("\nPlain text appended via AppendText() for clipboard and drag/drop comparison.");
        UpdateDisplayPreview("Plain text appended.");
    }

    /// <summary>Provides the OnRichTextAppendHtmlClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextAppendHtmlClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        richTextBox.AppendHtml("<p><strong>Bold</strong>, <em>Italic</em>, <u>Underline</u>, <s>Strikethrough</s>, <span style='font-family:Consolas;font-size:20;color:#00BFFF;background-color:#222'>font/color/highlight</span>.</p>");
        UpdateDisplayPreview("HTML fragment appended.");
    }

    /// <summary>Provides the OnRichTextMarkdownClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextMarkdownClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        richTextBox.SetMarkdown(SampleMarkdown);
        UpdateDisplayPreview("Markdown imported through SetMarkdown().");
    }

    /// <summary>Provides the OnRichTextInsertImageClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextInsertImageClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        richTextBox.AppendHtml($"<p>Inline image inserted from a data URI:</p><img src='{DemoImageDataUri}' width='{InlineImageWidth}' height='{InlineImageHeight}' align='left' /><p></p>");
        UpdateDisplayPreview("Inline image inserted from data URI.");
    }

    /// <summary>Provides the OnRichTextReplaceSelectionClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextReplaceSelectionClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        EnsureDemoSelection(richTextBox);
        richTextBox.ReplaceSelectionWithHtml("<strong>[Selection Replaced]</strong>");
        UpdateDisplayPreview("Selection replaced with HTML.");
    }

    /// <summary>Provides the OnRichTextSelectAllClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextSelectAllClick(object? sender, RoutedEventArgs e)
    {
        DemoRichTextBox?.SelectAll();
        UpdateStatus("All editor content selected.");
    }

    /// <summary>Provides the OnRichTextBoldClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextBoldClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.ToggleBold(), "Bold toggled for the current selection.");

    /// <summary>Provides the OnRichTextItalicClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextItalicClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.ToggleItalic(), "Italic toggled for the current selection.");

    /// <summary>Provides the OnRichTextUnderlineClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextUnderlineClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.ToggleUnderline(), "Underline toggled for the current selection.");

    /// <summary>Provides the OnRichTextStrikeClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextStrikeClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.ToggleStrikethrough(), "Strikethrough toggled for the current selection.");

    /// <summary>Provides the OnRichTextClearFormattingClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextClearFormattingClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.ClearFormatting(), "Formatting cleared for the current selection.");

    /// <summary>Provides the OnRichTextFontFamilyClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextFontFamilyClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.SetSelectionFontFamily("Consolas"), "Font Family changed to Consolas.");

    /// <summary>Provides the OnRichTextFontSizeClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextFontSizeClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.SetSelectionFontSize(FormattingFontSize), $"Font Size changed to {FormattingFontSize}.");

    /// <summary>Provides the OnRichTextForegroundClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextForegroundClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.SetSelectionForeground(Colors.DeepSkyBlue), "Foreground changed to DeepSkyBlue.");

    /// <summary>Provides the OnRichTextHighlightClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextHighlightClick(object? sender, RoutedEventArgs e) => ApplySelectionAction(richTextBox => richTextBox.SetSelectionHighlight(Colors.DarkSlateBlue), "Highlight changed to DarkSlateBlue.");

    /// <summary>Provides the OnRichTextUndoClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextUndoClick(object? sender, RoutedEventArgs e)
    {
        DemoRichTextBox?.Undo();
        UpdateDisplayPreview("Undo executed.");
    }

    /// <summary>Provides the OnRichTextRedoClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextRedoClick(object? sender, RoutedEventArgs e)
    {
        DemoRichTextBox?.Redo();
        UpdateDisplayPreview("Redo executed.");
    }

    /// <summary>Provides the OnRichTextCopyClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextCopyClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        EnsureDemoSelection(richTextBox);
        richTextBox.Copy();
        UpdateStatus($"Copied selection. Plain={_demoClipboard.PlainText?.Length ?? 0} chars; HTML={_demoClipboard.HtmlText?.Length ?? 0} chars.");
    }

    /// <summary>Provides the OnRichTextCutClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextCutClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        EnsureDemoSelection(richTextBox);
        richTextBox.Cut();
        UpdateDisplayPreview("Cut selection to rich clipboard adapter.");
    }

    /// <summary>Provides the OnRichTextPasteSampleClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextPasteSampleClick(object? sender, RoutedEventArgs e)
    {
        _demoClipboard.SetPlainText("Clipboard plain-text fallback");
        _demoClipboard.SetHtml(ClipboardHtmlSample);
        DemoRichTextBox?.Paste();
        UpdateDisplayPreview("Rich clipboard HTML sample pasted with plain-text fallback.");
    }

    /// <summary>Provides the OnRichTextPasteImageSampleClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextPasteImageSampleClick(object? sender, RoutedEventArgs e)
    {
        _demoClipboard.SetImage(DemoImageDataUri);
        DemoRichTextBox?.Paste();
        UpdateDisplayPreview("Image clipboard sample pasted from data URI.");
    }

    /// <summary>Provides the OnRichTextDropTextSampleClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextDropTextSampleClick(object? sender, RoutedEventArgs e)
    {
        var inserted = DemoRichTextBox?.TryDropText("<strong>Dropped HTML text</strong> with plain-text fallback instructions.") == true;
        UpdateDisplayPreview(inserted ? "Text/HTML drop sample accepted." : "Text/HTML drop sample rejected by current mode.");
    }

    /// <summary>Provides the OnRichTextDropImageSampleClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextDropImageSampleClick(object? sender, RoutedEventArgs e)
    {
        var inserted = DemoRichTextBox?.TryDropImage(DemoImageDataUri) == true;
        UpdateDisplayPreview(inserted ? "Image drop sample accepted." : "Image drop sample rejected by current mode.");
    }

    /// <summary>Provides the OnRichTextExportHtmlClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextExportHtmlClick(object? sender, RoutedEventArgs e)
    {
        var html = DemoRichTextBox?.Html ?? string.Empty;
        UpdateStatus($"Export HTML: {html.Length} chars. Preview: {TruncateForStatus(html)}");
    }

    /// <summary>Provides the OnRichTextExportMarkdownClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextExportMarkdownClick(object? sender, RoutedEventArgs e)
    {
        var markdown = DemoRichTextBox?.Markdown ?? string.Empty;
        UpdateStatus($"Export Markdown: {markdown.Length} chars. Preview: {TruncateForStatus(markdown)}");
    }

    /// <summary>Provides the OnRichTextSaveLoadStreamClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextSaveLoadStreamClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        using var memoryStream = new MemoryStream();
        richTextBox.Save(memoryStream, RichTextDataFormat.Html);
        memoryStream.Position = 0;
        richTextBox.Load(memoryStream, RichTextDataFormat.Html);
        UpdateDisplayPreview($"Save/Load stream round-trip completed ({memoryStream.Length} bytes).");
    }

    /// <summary>Provides the OnRichTextToggleDisplayModeClick member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnRichTextToggleDisplayModeClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        _displayModeEnabled = !_displayModeEnabled;
        richTextBox.EditMode = _displayModeEnabled ? RichTextEditMode.Display : RichTextEditMode.EditOnFocus;
        UpdateDisplayPreview(_displayModeEnabled ? "Main editor switched to Display mode." : "Main editor switched to EditOnFocus mode.");
    }

    /// <summary>Provides the ApplySelectionAction member.</summary>
    /// <param name="action">The action value.</param>
    /// <param name="status">The status value.</param>
    private void ApplySelectionAction(Action<RichTextBoxControl> action, string status)
    {
        var richTextBox = DemoRichTextBox;
        if (richTextBox is null)
        {
            return;
        }

        EnsureDemoSelection(richTextBox);
        action(richTextBox);
        UpdateDisplayPreview(status);
    }

    /// <summary>Provides the UpdateDisplayPreview member.</summary>
    /// <param name="status">The status value.</param>
    private void UpdateDisplayPreview(string status)
    {
        var source = DemoRichTextBox;
        var display = DisplayRichTextBox;
        if (source is null || display is null)
        {
            UpdateStatus(status);
            return;
        }

        display.SetHtml(source.Html);
        display.Select(0, 0);
        UpdateStatus(status);
    }

    /// <summary>Provides the UpdateStatus member.</summary>
    /// <param name="status">The status value.</param>
    private void UpdateStatus(string status)
    {
        var richTextBox = DemoRichTextBox;
        var plainLength = richTextBox?.PlainText.Length ?? 0;
        var htmlLength = richTextBox?.Html.Length ?? 0;
        var mode = richTextBox?.EditMode.ToString() ?? "Unavailable";
        var statusTextBlock = RichTextStatusTextBlock;
        if (statusTextBlock is null)
        {
            return;
        }

        statusTextBlock.Text = $"Status: {status} Mode={mode}; Plain={plainLength} chars; HTML={htmlLength} chars.";
    }
}
