// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Interactivity;
using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views.Pages;

/// <summary>
/// Page demonstrating input controls.
/// </summary>
public partial class InputPageView : ReactiveUserControl<InputPageViewModel>
{
    private const string DemoImageDataUri = "data:image/png;base64,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";
    private const string SampleMarkdown = "# RichTextBox Markdown\n\n- **Bold**\n- *Italic*\n- <u>Underline</u>\n\nThis sample mirrors WPF-style rich content workflows.";

    /// <summary>
    /// Initializes a new instance of the <see cref="InputPageView"/> class.
    /// </summary>
    public InputPageView()
    {
        InitializeComponent();
        this.WhenActivated(_ => ViewModel ??= AppLocator.Current.GetService<InputPageViewModel>());
    }

    private CrissCross.Avalonia.UI.Controls.RichTextBox? DemoRichTextBox => this.FindControl<CrissCross.Avalonia.UI.Controls.RichTextBox>("FormattingRichTextBox");

    private void OnRichTextResetClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        richTextBox?.SetPlainText("Welcome to RichTextBox!\n\nUse the demo buttons to insert WPF-compatible content.");
    }

    private void OnRichTextAppendPlainClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        richTextBox?.AppendText("\nPlain text appended via AppendText().");
    }

    private void OnRichTextAppendHtmlClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        richTextBox?.AppendHtml("<p><strong>Bold</strong>, <em>Italic</em>, <u>Underline</u>, <s>Strikethrough</s>, <span style='color:#00BFFF;background-color:#222'>Color</span>.</p>");
    }

    private void OnRichTextMarkdownClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        richTextBox?.SetMarkdown(SampleMarkdown);
    }

    private void OnRichTextInsertImageClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        richTextBox?.AppendHtml($"<p>Inline image:</p><img src='{DemoImageDataUri}' width='24' height='24' align='left' /><p></p>");
    }

    private void OnRichTextReplaceSelectionClick(object? sender, RoutedEventArgs e)
    {
        var richTextBox = DemoRichTextBox;
        richTextBox?.ReplaceSelectionWithHtml("<strong>[Selection Replaced]</strong>");
    }

    private void OnRichTextSelectAllClick(object? sender, RoutedEventArgs e) => DemoRichTextBox?.SelectAll();

    private void OnRichTextBoldClick(object? sender, RoutedEventArgs e) => DemoRichTextBox?.ToggleBold();

    private void OnRichTextItalicClick(object? sender, RoutedEventArgs e) => DemoRichTextBox?.ToggleItalic();

    private void OnRichTextUnderlineClick(object? sender, RoutedEventArgs e) => DemoRichTextBox?.ToggleUnderline();

    private void OnRichTextStrikeClick(object? sender, RoutedEventArgs e) => DemoRichTextBox?.ToggleStrikethrough();

    private void OnRichTextClearFormattingClick(object? sender, RoutedEventArgs e) => DemoRichTextBox?.ClearFormatting();
}
