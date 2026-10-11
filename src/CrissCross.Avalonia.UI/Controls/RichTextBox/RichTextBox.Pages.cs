// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides measured page layout and document-position input.</summary>
public partial class RichTextBox
{
    /// <summary>Identifies the measured sheet count.</summary>
    public static readonly DirectProperty<RichTextBox, int> PageCountProperty = AvaloniaProperty.RegisterDirect<RichTextBox, int>(
        nameof(PageCount),
        static editor => editor.PageCount);

    /// <summary>Identifies the paged layout option.</summary>
    public static readonly StyledProperty<bool> IsPageLayoutEnabledProperty = AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsPageLayoutEnabled));

    /// <summary>Identifies the paper dimensions.</summary>
    public static readonly StyledProperty<Size> PageSizeProperty = AvaloniaProperty.Register<RichTextBox, Size>(
        nameof(PageSize),
        new(794, 1123),
        validate: static value => double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0);

    /// <summary>Identifies the paper margins.</summary>
    public static readonly StyledProperty<Thickness> PagePaddingProperty = AvaloniaProperty.Register<RichTextBox, Thickness>(
        nameof(PagePadding),
        new(48),
        validate: static value => double.IsFinite(value.Left) && double.IsFinite(value.Top) && double.IsFinite(value.Right)
            && double.IsFinite(value.Bottom) && value.Left >= 0 && value.Top >= 0 && value.Right >= 0 && value.Bottom >= 0);

    /// <summary>Identifies the distance between sheets.</summary>
    public static readonly StyledProperty<double> PageSpacingProperty = AvaloniaProperty.Register<RichTextBox, double>(
        nameof(PageSpacing),
        20,
        validate: static value => double.IsFinite(value) && value >= 0);

    /// <summary>The paged template surface.</summary>
    private PagedDocumentPresenter? _pagedPresenter;

    /// <summary>The scroll container for paper sheets.</summary>
    private global::Avalonia.Controls.ScrollViewer? _pageScrollViewer;

    /// <summary>The pointer selection anchor.</summary>
    private int _pageSelectionAnchor;

    /// <summary>Whether pointer selection is active.</summary>
    private bool _isSelectingPage;

    /// <summary>Occurs when a hyperlink is activated. Hosts decide how to open its target.</summary>
    public event EventHandler<RichTextHyperlinkEventArgs>? HyperlinkActivated;

    /// <summary>Gets or sets whether the document is edited on paper sheets.</summary>
    public bool IsPageLayoutEnabled { get => GetValue(IsPageLayoutEnabledProperty); set => SetValue(IsPageLayoutEnabledProperty, value); }

    /// <summary>Gets or sets the paper size in device independent pixels.</summary>
    public Size PageSize { get => GetValue(PageSizeProperty); set => SetValue(PageSizeProperty, value); }

    /// <summary>Gets or sets the paper margins.</summary>
    public Thickness PagePadding { get => GetValue(PagePaddingProperty); set => SetValue(PagePaddingProperty, value); }

    /// <summary>Gets or sets the distance between paper sheets.</summary>
    public double PageSpacing { get => GetValue(PageSpacingProperty); set => SetValue(PageSpacingProperty, value); }

    /// <summary>Gets the measured number of paper sheets.</summary>
    public int PageCount => _pagedPresenter?.PageCount ?? 0;

    /// <inheritdoc/>
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        if (!IsPageLayoutEnabled || IsReadOnlyInternal || e.Handled || e.Text is not { Length: > 0 } text)
        {
            return;
        }

        ReplaceSelection(text);
        e.Handled = true;
        BringPageCaretIntoView();
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!IsPageLayoutEnabled || _pagedPresenter is null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        if ((e.KeyModifiers & KeyModifiers.Control) != 0)
        {
            return;
        }

        _ = Focus();
        var offset = _pagedPresenter.GetDocumentPosition(e.GetPosition(_pagedPresenter));
        _pageSelectionAnchor = (e.KeyModifiers & KeyModifiers.Shift) != 0 ? SelectionStart : offset;
        SetPageSelection(_pageSelectionAnchor, offset);
        _isSelectingPage = true;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isSelectingPage || _pagedPresenter is null)
        {
            return;
        }

        SetPageSelection(_pageSelectionAnchor, _pagedPresenter.GetDocumentPosition(e.GetPosition(_pagedPresenter)));
        e.Handled = true;
    }

    /// <summary>Routes page editing keys through document positions.</summary>
    /// <param name="e">The keyboard event.</param>
    /// <returns>Whether the key was handled.</returns>
    private bool HandlePageKey(KeyEventArgs e)
    {
        if (!IsPageLayoutEnabled || e.Handled)
        {
            return false;
        }

        var caret = Math.Clamp(CaretIndex, 0, Document.Length);
        if (HandlePageMutation(e, caret))
        {
            e.Handled = true;
            BringPageCaretIntoView();
            return true;
        }

        var next = GetPageNavigationOffset(e, caret);
        if (next is null)
        {
            return false;
        }

        SetPageSelection((e.KeyModifiers & KeyModifiers.Shift) != 0 ? SelectionStart : next.Value, next.Value);
        e.Handled = true;
        BringPageCaretIntoView();
        return true;
    }

    /// <summary>Gets the document offset for a navigation gesture.</summary>
    /// <param name="e">The key event.</param>
    /// <param name="caret">The current caret.</param>
    /// <returns>The destination or null for a non-navigation gesture.</returns>
    private int? GetPageNavigationOffset(KeyEventArgs e, int caret) => e.Key switch
    {
        Key.Left => GetAdjacentTextElement(caret, true),
        Key.Right => GetAdjacentTextElement(caret, false),
        Key.Home => (e.KeyModifiers & KeyModifiers.Control) != 0 ? 0 : GetPageLineBoundary(caret, false),
        Key.End => (e.KeyModifiers & KeyModifiers.Control) != 0 ? Document.Length : GetPageLineBoundary(caret, true),
        Key.Up or Key.Down => GetVerticalPageOffset(caret, e.Key == Key.Up),
        _ => null,
    };

    /// <summary>Gets the insertion position on the adjacent visual line.</summary>
    /// <param name="caret">The current caret.</param>
    /// <param name="up">Whether to move upward.</param>
    /// <returns>The destination position.</returns>
    private int GetVerticalPageOffset(int caret, bool up) => _pagedPresenter?.GetVerticalDocumentPosition(caret, up) ?? caret;

    /// <summary>Applies document mutations for a page editing gesture.</summary>
    /// <param name="e">The key event.</param>
    /// <param name="caret">The current caret.</param>
    /// <returns>Whether the gesture was recognized.</returns>
    private bool HandlePageMutation(KeyEventArgs e, int caret)
    {
        if (e.Key is Key.Back or Key.Delete)
        {
            DeletePageSelection(caret, e.Key == Key.Back);
            return true;
        }

        if (IsReadOnlyInternal)
        {
            return false;
        }

        if (e.Key == Key.Enter && AcceptsReturn)
        {
            InsertPageReturn((e.KeyModifiers & KeyModifiers.Control) != 0);
            return true;
        }

        if (e.Key != Key.Tab || !AcceptsTab)
        {
            return false;
        }

        ReplaceSelection("\t");
        return true;
    }

    /// <summary>Deletes a selection or the adjacent character.</summary>
    /// <param name="caret">The current caret.</param>
    /// <param name="back">Whether to delete backward.</param>
    private void DeletePageSelection(int caret, bool back)
    {
        if (IsReadOnlyInternal)
        {
            return;
        }

        if (!HasSelection)
        {
            var adjacent = GetAdjacentTextElement(caret, back);
            Select(Math.Min(caret, adjacent), Math.Abs(caret - adjacent));
        }

        ReplaceSelection(string.Empty);
    }

    /// <summary>Moves to a Unicode text element boundary without splitting a grapheme.</summary>
    /// <param name="caret">The current insertion position.</param>
    /// <param name="back">Whether to seek backward.</param>
    /// <returns>The adjacent text element boundary.</returns>
    private int GetAdjacentTextElement(int caret, bool back)
    {
        var boundaries = StringInfo.ParseCombiningCharacters(Document.PlainText);
        var index = Array.BinarySearch(boundaries, caret);
        if (index < 0)
        {
            index = ~index;
        }
        else if (!back)
        {
            index++;
        }

        if (back)
        {
            return index > 0 ? boundaries[index - 1] : 0;
        }

        return index < boundaries.Length ? boundaries[index] : Document.Length;
    }

    /// <summary>Inserts a line or page break.</summary>
    /// <param name="page">Whether to insert a page break.</param>
    private void InsertPageReturn(bool page)
    {
        if (page)
        {
            InsertPageBreak();
        }
        else
        {
            ReplaceSelection(Environment.NewLine);
        }
    }

    /// <summary>Finds a paragraph boundary for Home and End.</summary>
    /// <param name="caret">The caret position.</param>
    /// <param name="end">Whether to seek the paragraph end.</param>
    /// <returns>The boundary offset.</returns>
    private int GetPageLineBoundary(int caret, bool end)
    {
        var text = Document.PlainText;
        if (end)
        {
            var index = text.IndexOf('\n', caret);
            if (index < 0)
            {
                return text.Length;
            }

            return index > 0 && text[index - 1] == '\r' ? index - 1 : index;
        }

        return caret == 0 ? 0 : text.LastIndexOf('\n', caret - 1) + 1;
    }

    /// <summary>Updates selection without changing its direction.</summary>
    /// <param name="anchor">The anchor.</param>
    /// <param name="caret">The active endpoint.</param>
    private void SetPageSelection(int anchor, int caret)
    {
        SelectionStart = anchor;
        SelectionEnd = caret;
        CaretIndex = caret;
        SynchronizeSelectionFromIndexes(raiseEvent: true);
        NotifyCommandStateChanged();
        UpdatePageSelection();
    }

    /// <summary>Updates the paged selection and caret decoration.</summary>
    private void UpdatePageSelection()
    {
        if (_pagedPresenter is null)
        {
            return;
        }

        _pagedPresenter.SelectionStart = SelectionStart;
        _pagedPresenter.SelectionEnd = SelectionEnd;
        _pagedPresenter.CaretIndex = CaretIndex;
        _pagedPresenter.ShowCaret = IsFocused && !IsReadOnlyInternal;
    }

    /// <summary>Scrolls the paged caret into the visible document area.</summary>
    private void BringPageCaretIntoView()
    {
        _pagedPresenter?.UpdateLayout();
        if (_pagedPresenter is null)
        {
            return;
        }

        _pagedPresenter.BringIntoView(_pagedPresenter.GetDocumentCaretRect(CaretIndex));
    }

    /// <summary>Forwards document hyperlink activation to the host.</summary>
    /// <param name="sender">The document presenter.</param>
    /// <param name="e">The hyperlink target.</param>
    private void OnDocumentHyperlinkActivated(object? sender, RichTextHyperlinkEventArgs e) => HyperlinkActivated?.Invoke(this, e);

    /// <summary>Notifies bindings after pagination changes.</summary>
    /// <param name="sender">The paged presenter.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPaginationChanged(object? sender, EventArgs e) => RaisePropertyChanged(PageCountProperty, 0, PageCount);
}
