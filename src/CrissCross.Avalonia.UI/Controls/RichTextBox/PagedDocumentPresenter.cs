// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Displays editable-document coordinates on sheets partitioned at measured line boundaries.</summary>
public sealed partial class PagedDocumentPresenter : Panel
{
    /// <summary>Identifies the document property.</summary>
    public static readonly StyledProperty<FlowDocument?> DocumentProperty = AvaloniaProperty.Register<PagedDocumentPresenter, FlowDocument?>(nameof(Document));

    /// <summary>Identifies the sheet size property.</summary>
    public static readonly StyledProperty<Size> PageSizeProperty = AvaloniaProperty.Register<PagedDocumentPresenter, Size>(
        nameof(PageSize),
        new(794, 1123),
        validate: IsValidSize);

    /// <summary>Identifies the sheet padding property.</summary>
    public static readonly StyledProperty<Thickness> PagePaddingProperty = AvaloniaProperty.Register<PagedDocumentPresenter, Thickness>(
        nameof(PagePadding),
        new(48),
        validate: IsValidPadding);

    /// <summary>Identifies the sheet spacing property.</summary>
    public static readonly StyledProperty<double> PageSpacingProperty = AvaloniaProperty.Register<PagedDocumentPresenter, double>(
        nameof(PageSpacing),
        20,
        validate: static value => double.IsFinite(value) && value >= 0);

    /// <summary>Identifies the default text size property.</summary>
    public static readonly StyledProperty<double> DefaultFontSizeProperty = AvaloniaProperty.Register<PagedDocumentPresenter, double>(
        nameof(DefaultFontSize),
        14,
        validate: static value => double.IsFinite(value) && value > 0);

    /// <summary>Identifies the default text brush property.</summary>
    public static readonly StyledProperty<IBrush?> DefaultForegroundProperty = AvaloniaProperty.Register<PagedDocumentPresenter, IBrush?>(nameof(DefaultForeground), Brushes.Black);

    /// <summary>Identifies the default font family property.</summary>
    public static readonly StyledProperty<FontFamily> DefaultFontFamilyProperty = AvaloniaProperty.Register<PagedDocumentPresenter, FontFamily>(nameof(DefaultFontFamily), FontFamily.Default);

    /// <summary>Identifies the selection start property.</summary>
    public static readonly StyledProperty<int> SelectionStartProperty = AvaloniaProperty.Register<PagedDocumentPresenter, int>(nameof(SelectionStart));

    /// <summary>Identifies the selection end property.</summary>
    public static readonly StyledProperty<int> SelectionEndProperty = AvaloniaProperty.Register<PagedDocumentPresenter, int>(nameof(SelectionEnd));

    /// <summary>Identifies the caret offset property.</summary>
    public static readonly StyledProperty<int> CaretIndexProperty = AvaloniaProperty.Register<PagedDocumentPresenter, int>(nameof(CaretIndex));

    /// <summary>Identifies the caret visibility property.</summary>
    public static readonly StyledProperty<bool> ShowCaretProperty = AvaloniaProperty.Register<PagedDocumentPresenter, bool>(nameof(ShowCaret));

    /// <summary>Identifies the selection brush property.</summary>
    public static readonly StyledProperty<IBrush> SelectionBrushProperty = AvaloniaProperty.Register<PagedDocumentPresenter, IBrush>(
        nameof(SelectionBrush),
        new SolidColorBrush(Color.Parse("#50286EDC")));

    /// <summary>Identifies the caret brush property.</summary>
    public static readonly StyledProperty<IBrush?> CaretBrushProperty = AvaloniaProperty.Register<PagedDocumentPresenter, IBrush?>(nameof(CaretBrush));

    /// <summary>The fraction used to center sheets and divide inter-sheet hit regions.</summary>
    private const double Half = 0.5;

    /// <summary>The fallback line height relative to the default text size.</summary>
    private const double EmptyLineHeightScale = 1.5;

    /// <summary>Measured page slices.</summary>
    private readonly List<PageEntry> _pages = new();

    /// <summary>Decoded images shared by every sheet of the current layout.</summary>
    private readonly Dictionary<string, IImage?> _imageCache = [with(StringComparer.Ordinal)];

    /// <summary>Whether the measured page tree must be rebuilt.</summary>
    private bool _dirty = true;

    /// <summary>The transient input method composition.</summary>
    private string? _preeditText;

    /// <summary>The composition insertion position.</summary>
    private int _preeditOffset;

    /// <summary>The cursor position within the transient composition.</summary>
    private int _preeditCursor;

    /// <summary>The committed document retained while previewing composition.</summary>
    private FlowDocument? _preeditSource;

    /// <summary>Raised after the measured sheet collection changes.</summary>
    public event EventHandler? PaginationChanged;

    /// <summary>Raised when a hyperlink on a sheet is activated.</summary>
    public event EventHandler<RichTextHyperlinkEventArgs>? HyperlinkActivated;

    /// <summary>Gets or sets the document rendered on the sheets.</summary>
    public FlowDocument? Document { get => GetValue(DocumentProperty); set => SetValue(DocumentProperty, value); }

    /// <summary>Gets or sets the nominal sheet size in device independent pixels.</summary>
    public Size PageSize { get => GetValue(PageSizeProperty); set => SetValue(PageSizeProperty, value); }

    /// <summary>Gets or sets the sheet margins.</summary>
    public Thickness PagePadding { get => GetValue(PagePaddingProperty); set => SetValue(PagePaddingProperty, value); }

    /// <summary>Gets or sets the vertical space between sheets.</summary>
    public double PageSpacing { get => GetValue(PageSpacingProperty); set => SetValue(PageSpacingProperty, value); }

    /// <summary>Gets or sets the default text size.</summary>
    public double DefaultFontSize { get => GetValue(DefaultFontSizeProperty); set => SetValue(DefaultFontSizeProperty, value); }

    /// <summary>Gets or sets the default text brush.</summary>
    public IBrush? DefaultForeground { get => GetValue(DefaultForegroundProperty); set => SetValue(DefaultForegroundProperty, value); }

    /// <summary>Gets or sets the default font family for unformatted text.</summary>
    public FontFamily DefaultFontFamily { get => GetValue(DefaultFontFamilyProperty); set => SetValue(DefaultFontFamilyProperty, value); }

    /// <summary>Gets or sets the selection anchor in rendered document coordinates.</summary>
    public int SelectionStart { get => GetValue(SelectionStartProperty); set => SetValue(SelectionStartProperty, value); }

    /// <summary>Gets or sets the selection endpoint in rendered document coordinates.</summary>
    public int SelectionEnd { get => GetValue(SelectionEndProperty); set => SetValue(SelectionEndProperty, value); }

    /// <summary>Gets or sets the caret in rendered document coordinates.</summary>
    public int CaretIndex { get => GetValue(CaretIndexProperty); set => SetValue(CaretIndexProperty, value); }

    /// <summary>Gets or sets whether to draw the caret.</summary>
    public bool ShowCaret { get => GetValue(ShowCaretProperty); set => SetValue(ShowCaretProperty, value); }

    /// <summary>Gets or sets the translucent selection brush.</summary>
    public IBrush SelectionBrush { get => GetValue(SelectionBrushProperty); set => SetValue(SelectionBrushProperty, value); }

    /// <summary>Gets or sets the caret brush, falling back to the document foreground.</summary>
    public IBrush? CaretBrush { get => GetValue(CaretBrushProperty); set => SetValue(CaretBrushProperty, value); }

    /// <summary>Gets the measured sheet count.</summary>
    public int PageCount => _pages.Count;

    /// <summary>Gets whether an indivisible line or object required an expanded sheet.</summary>
    public bool HasOverflow { get; private set; }

    /// <summary>Invalidates pagination after changing document content or image dimensions.</summary>
    public void RefreshDocument()
    {
        _dirty = true;
        InvalidateMeasure();
    }

    /// <summary>Maps a point on a sheet to an offset in the rendered document.</summary>
    /// <param name="point">A point relative to this control.</param>
    /// <returns>The nearest document insertion offset.</returns>
    public int GetDocumentPosition(Point point)
    {
        if (_pages.Count == 0)
        {
            return 0;
        }

        var page = _pages[^1];
        foreach (var candidate in _pages)
        {
            if (point.Y < candidate.Top + candidate.SheetHeight + (PageSpacing * Half))
            {
                page = candidate;
                break;
            }
        }

        var bodyTop = page.Top + page.Padding.Top;
        if (point.Y < bodyTop)
        {
            return page.Slice.Start;
        }

        if (point.Y >= bodyTop + page.Slice.Height)
        {
            return page.Slice.End;
        }

        var local = new Point(point.X - page.Left - page.Padding.Left, point.Y - bodyTop + page.Slice.DocumentTop);
        return Math.Clamp(page.Viewport.Presenter.GetDocumentPosition(local), page.Slice.Start, page.Slice.End);
    }

    /// <summary>Gets the caret rectangle relative to the paged surface.</summary>
    /// <param name="offset">The rendered document offset.</param>
    /// <returns>The caret rectangle on its sheet.</returns>
    public Rect GetDocumentCaretRect(int offset)
    {
        if (_pages.Count == 0)
        {
            return default;
        }

        var page = _pages[^1];
        foreach (var candidate in _pages)
        {
            if (offset < candidate.Slice.End)
            {
                page = candidate;
                break;
            }
        }

        var caret = page.Viewport.Presenter.GetDocumentCaretRect(offset);
        var caretTop = page.Slice.Height > 0 ? caret.Y : page.Slice.DocumentTop;
        return new(caret.X + page.Left + page.Padding.Left, caretTop + page.Top + page.Padding.Top - page.Slice.DocumentTop, caret.Width, caret.Height);
    }

    /// <summary>Gets measured range rectangles in paper-sheet coordinates.</summary>
    /// <param name="start">The first rendered document position.</param>
    /// <param name="length">The number of positions.</param>
    /// <returns>Rectangles clipped and translated to each containing sheet.</returns>
    public IReadOnlyList<Rect> GetDocumentRangeRectangles(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        var rectangles = new List<Rect>();
        var end = (long)start + length;
        foreach (var page in _pages)
        {
            var first = Math.Max(start, page.Slice.Start);
            var last = (int)Math.Min(end, page.Slice.End);
            if (last <= first)
            {
                continue;
            }

            foreach (var rectangle in page.Viewport.Presenter.GetDocumentRangeRectangles(first, last - first))
            {
                var clip = new Rect(0, page.Slice.DocumentTop, Math.Max(0, page.Size.Width - page.Padding.Left - page.Padding.Right), page.Slice.Height);
                var clipped = rectangle.Intersect(clip);
                if (clipped.Width > 0 && clipped.Height > 0)
                {
                    rectangles.Add(clipped.Translate(new(page.Left + page.Padding.Left, page.Top + page.Padding.Top - page.Slice.DocumentTop)));
                }
            }
        }

        return rectangles;
    }

    /// <summary>Moves vertically in document coordinates so paper margins cannot trap the caret.</summary>
    /// <param name="offset">The current caret offset.</param>
    /// <param name="previous">Whether to move to the previous line.</param>
    /// <returns>The adjacent line insertion position.</returns>
    public int GetVerticalDocumentPosition(int offset, bool previous)
    {
        if (_pages.Count == 0)
        {
            return 0;
        }

        var pageIndex = _pages.Count - 1;
        for (var index = 0; index < _pages.Count; index++)
        {
            if (offset < _pages[index].Slice.End)
            {
                pageIndex = index;
                break;
            }
        }

        var page = _pages[pageIndex];
        var presenter = page.Viewport.Presenter;
        var caret = presenter.GetDocumentCaretRect(offset);
        var target = new Point(caret.X, caret.Y + (caret.Height * (previous ? -Half : 1 + Half)));
        var next = presenter.GetDocumentPosition(target);
        return next >= page.Slice.Start && next < page.Slice.End
            ? next
            : GetAdjacentPagePosition(pageIndex, previous, caret.X + page.Left + page.Padding.Left);
    }

    /// <summary>Updates transient composition without editing the committed document.</summary>
    /// <param name="text">The uncommitted text.</param>
    /// <param name="cursor">The cursor within the composition.</param>
    /// <param name="offset">The document insertion position.</param>
    /// <param name="selectionLength">The replaced committed range.</param>
    /// <param name="html">The encoded composition with insertion formatting.</param>
    internal void SetPreedit(string? text, int? cursor, int offset, int selectionLength = 0, string? html = null)
    {
        _preeditText = text;
        var textLength = text?.Length ?? 0;
        _preeditCursor = Math.Clamp(cursor ?? textLength, 0, textLength);
        _preeditOffset = offset;
        if (string.IsNullOrEmpty(text))
        {
            if (_preeditSource is not { } source)
            {
                return;
            }

            _preeditSource = null;
            Document = source;
            RefreshDocument();
            return;
        }

        _preeditSource ??= Document;
        var preview = new FlowDocument();
        preview.SetText(_preeditSource?.Text);
        preview.Replace(offset, selectionLength, html ?? HtmlClipboardUtilities.EncodePlainText(text));
        Document = preview;
        RefreshDocument();
    }

    /// <summary>Gets the measured composition cursor in paged surface coordinates.</summary>
    /// <returns>The cursor rectangle.</returns>
    internal Rect GetInputMethodCursorRectangle() => GetDocumentCaretRect(string.IsNullOrEmpty(_preeditText) ? CaretIndex : _preeditOffset + _preeditCursor);

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        ArgumentNullException.ThrowIfNull(change);
        UpdateDocumentSubscription(change);

        if (IsAdornmentProperty(change.Property))
        {
            foreach (var page in _pages)
            {
                page.Viewport.Adornment.InvalidateVisual();
            }
        }
        else if (IsLayoutProperty(change.Property))
        {
            RefreshDocument();
        }
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (_dirty)
        {
            RebuildPages();
        }

        var height = 0.0;
        var width = PageSize.Width;
        foreach (var page in _pages)
        {
            page.Sheet.Measure(new(page.Size.Width, page.SheetHeight));
            height += page.SheetHeight;
            width = Math.Max(width, page.Size.Width);
        }

        return new(width, height + (Math.Max(0, _pages.Count - 1) * PageSpacing));
    }

    /// <inheritdoc/>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Children.Clear();
        _pages.Clear();
        _pageFieldRegions.Clear();
        _automationStories.Clear();
        ClearImageCache();
        RefreshDocument();
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var top = 0.0;
        foreach (var page in _pages)
        {
            var left = Math.Max(0, (finalSize.Width - page.Size.Width) * Half);
            page.Top = top;
            page.Left = left;
            page.Sheet.Arrange(new(left, top, page.Size.Width, page.SheetHeight));
            top += page.SheetHeight + PageSpacing;
        }

        return finalSize;
    }

    /// <summary>Identifies properties that change document measurement.</summary>
    /// <param name="property">The changed property.</param>
    /// <returns>Whether pagination must be refreshed.</returns>
    private static bool IsLayoutProperty(AvaloniaProperty property) =>
        property == DocumentProperty || property == PageSizeProperty || property == PagePaddingProperty
        || property == PageSpacingProperty || property == DefaultFontSizeProperty || property == DefaultForegroundProperty
        || property == DefaultFontFamilyProperty;

    /// <summary>Identifies properties that change selection rendering.</summary>
    /// <param name="property">The changed property.</param>
    /// <returns>Whether the selection overlay must be refreshed.</returns>
    private static bool IsAdornmentProperty(AvaloniaProperty property) =>
        property == SelectionStartProperty || property == SelectionEndProperty || property == CaretIndexProperty
        || property == ShowCaretProperty || property == SelectionBrushProperty || property == CaretBrushProperty;

    /// <summary>Validates sheet dimensions.</summary>
    /// <param name="value">The candidate sheet size.</param>
    /// <returns>Whether both dimensions are finite and positive.</returns>
    private static bool IsValidSize(Size value) => double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0;

    /// <summary>Validates sheet margins.</summary>
    /// <param name="value">The candidate margins.</param>
    /// <returns>Whether every margin is finite and nonnegative.</returns>
    private static bool IsValidPadding(Thickness value) =>
        double.IsFinite(value.Left) && double.IsFinite(value.Top) && double.IsFinite(value.Right) && double.IsFinite(value.Bottom)
        && value.Left >= 0 && value.Top >= 0 && value.Right >= 0 && value.Bottom >= 0;

    /// <summary>Recreates measured page viewports after a document layout change.</summary>
    private void RebuildPages()
    {
        if (PagePadding.Left + PagePadding.Right >= PageSize.Width || PagePadding.Top + PagePadding.Bottom >= PageSize.Height)
        {
            throw new InvalidOperationException("Page margins consume the printable paper area.");
        }

        Children.Clear();
        _pages.Clear();
        _pageFieldRegions.Clear();
        _automationStories.Clear();
        ClearImageCache();
        HasOverflow = false;
        foreach (var section in RichDocumentStructure.ReadSections(Document?.Text ?? string.Empty, PageSize, PagePadding))
        {
            AddSectionPages(section);
        }

        UpdatePageFields();
        _dirty = false;
        PaginationChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Finds block-level page breaks in a measured line.</summary>
    /// <param name="start">The line start.</param>
    /// <param name="end">The line end.</param>
    /// <returns>Whether the line starts a new sheet.</returns>
    private bool HasPageBreakBefore(int start, int end)
    {
        if (Document is null)
        {
            return false;
        }

        foreach (var segment in Document.Segments)
        {
            if (segment.BreakPageBefore && segment.StartIndex >= start && segment.StartIndex < end)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Creates an identically configured rich document presenter.</summary>
    /// <returns>The configured presenter.</returns>
    private FormattedTextPresenter CreatePresenter()
    {
        var presenter = new FormattedTextPresenter
        {
            ImageSourceLoader = LoadCachedImage,
            DefaultFontSize = DefaultFontSize,
            FontSize = DefaultFontSize,
            FontFamily = DefaultFontFamily,
            DefaultForeground = DefaultForeground,
            Foreground = DefaultForeground ?? Brushes.Black,
            TextWrapping = TextWrapping.Wrap,
            Document = Document,
        };
        presenter.HyperlinkActivated += OnHyperlinkActivated;
        return presenter;
    }

    /// <summary>Resolves each image once for all sheets in one document layout.</summary>
    /// <param name="source">The image source.</param>
    /// <returns>The shared decoded image, or null for an unavailable source.</returns>
    private IImage? LoadCachedImage(string source)
    {
        if (_imageCache.TryGetValue(source, out var image))
        {
            return image;
        }

        image = new FormattedTextPresenter().LoadUncachedImage(source);
        _imageCache.Add(source, image);
        return image;
    }

    /// <summary>Releases owned image decodes after their page presenters are removed.</summary>
    private void ClearImageCache()
    {
        foreach (var image in _imageCache.Values)
        {
            if (image is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        _imageCache.Clear();
    }

    /// <summary>Responds to an edit of the attached document.</summary>
    /// <param name="sender">The attached document.</param>
    /// <param name="args">The change notification.</param>
    private void OnDocumentChanged(object? sender, EventArgs args) => RefreshDocument();

    /// <summary>Forwards hyperlink activation from a sheet.</summary>
    /// <param name="sender">The sheet presenter.</param>
    /// <param name="args">The activation details.</param>
    private void OnHyperlinkActivated(object? sender, RichTextHyperlinkEventArgs args) => HyperlinkActivated?.Invoke(this, args);

    /// <summary>Updates the document's content subscription.</summary>
    /// <param name="change">The changed property.</param>
    private void UpdateDocumentSubscription(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property != DocumentProperty)
        {
            return;
        }

        if (change.OldValue is FlowDocument previous)
        {
            previous.TextChanged -= OnDocumentChanged;
        }

        if (Document is not { } current)
        {
            return;
        }

        current.TextChanged += OnDocumentChanged;
    }

    /// <summary>A sheet and its measured document slice.</summary>
    /// <param name="sheet">The paper border.</param>
    /// <param name="viewport">The clipped document.</param>
    /// <param name="slice">The measured document range.</param>
    /// <param name="sheetHeight">The actual sheet height.</param>
    /// <param name="size">The section paper size.</param>
    /// <param name="padding">The section margins.</param>
    private sealed class PageEntry(Border sheet, PageViewport viewport, DocumentPageLayout.PageSlice slice, double sheetHeight, Size size, Thickness padding)
    {
        /// <summary>Gets the paper border.</summary>
        public Border Sheet { get; } = sheet;

        /// <summary>Gets the clipped document.</summary>
        public PageViewport Viewport { get; } = viewport;

        /// <summary>Gets the measured document range.</summary>
        public DocumentPageLayout.PageSlice Slice { get; } = slice;

        /// <summary>Gets the actual sheet height.</summary>
        public double SheetHeight { get; } = sheetHeight;

        /// <summary>Gets the section paper size.</summary>
        public Size Size { get; } = size;

        /// <summary>Gets the section margins.</summary>
        public Thickness Padding { get; } = padding;

        /// <summary>Gets or sets the arranged sheet top.</summary>
        public double Top { get; set; }

        /// <summary>Gets or sets the arranged sheet left.</summary>
        public double Left { get; set; }
    }

    /// <summary>Clips the same measured document to one page's line range.</summary>
    private sealed class PageViewport : Panel
    {
        /// <summary>The measured slice top.</summary>
        private readonly double _documentTop;

        /// <summary>The full document height.</summary>
        private readonly double _documentHeight;

        /// <summary>Initializes a new instance of the <see cref="PageViewport"/> class.</summary>
        /// <param name="owner">The paged surface.</param>
        /// <param name="presenter">The full document presenter.</param>
        /// <param name="documentTop">The measured slice top.</param>
        /// <param name="documentHeight">The full document height.</param>
        /// <param name="isEmpty">Whether this sheet has no measured lines.</param>
        public PageViewport(PagedDocumentPresenter owner, FormattedTextPresenter presenter, double documentTop, double documentHeight, bool isEmpty)
        {
            _documentTop = documentTop;
            _documentHeight = documentHeight;
            Presenter = presenter;
            Adornment = new(owner, presenter, documentTop, isEmpty) { IsHitTestVisible = false };
            ClipToBounds = true;
            Children.Add(presenter);
            Children.Add(Adornment);
        }

        /// <summary>Gets the full document presenter.</summary>
        public FormattedTextPresenter Presenter { get; }

        /// <summary>Gets the selection overlay.</summary>
        public SelectionAdornment Adornment { get; }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            Presenter.Measure(new(availableSize.Width, double.PositiveInfinity));
            Adornment.Measure(new(availableSize.Width, Math.Max(_documentHeight, Presenter.DesiredSize.Height)));
            return new(availableSize.Width, availableSize.Height);
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var bounds = new Rect(0, -_documentTop, finalSize.Width, Math.Max(_documentHeight, Presenter.DesiredSize.Height));
            Presenter.Arrange(bounds);
            Adornment.Arrange(bounds);
            return finalSize;
        }
    }

    /// <summary>Draws document selection and caret using the rich presenter's measured geometry.</summary>
    /// <param name="owner">The paged surface.</param>
    /// <param name="presenter">The full document presenter.</param>
    /// <param name="documentTop">The measured slice top.</param>
    /// <param name="isEmpty">Whether this sheet has no measured lines.</param>
    private sealed class SelectionAdornment(PagedDocumentPresenter owner, FormattedTextPresenter presenter, double documentTop, bool isEmpty) : Control
    {
        /// <inheritdoc/>
        public override void Render(DrawingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            base.Render(context);
            if (!string.IsNullOrEmpty(owner._preeditText))
            {
                RenderComposition(context);
                return;
            }

            var start = Math.Min(owner.SelectionStart, owner.SelectionEnd);
            var length = Math.Abs(owner.SelectionEnd - owner.SelectionStart);
            if (length > 0)
            {
                foreach (var rectangle in presenter.GetDocumentRangeRectangles(start, length))
                {
                    context.DrawRectangle(owner.SelectionBrush, null, rectangle);
                }
            }
            else if (owner.ShowCaret)
            {
                var caret = presenter.GetDocumentCaretRect(owner.CaretIndex);
                var caretTop = isEmpty && owner.CaretIndex == owner.Document?.Length ? documentTop : caret.Y;
                context.DrawRectangle(owner.CaretBrush ?? owner.DefaultForeground ?? Brushes.Black, null, new(caret.X, caretTop, 1, Math.Max(1, caret.Height)));
            }
        }

        /// <summary>Decorates the actual measured composition across all preview lines.</summary>
        /// <param name="context">The drawing context.</param>
        private void RenderComposition(DrawingContext context)
        {
            var pen = new Pen(owner.CaretBrush ?? owner.DefaultForeground ?? Brushes.Black);
            foreach (var rectangle in presenter.GetDocumentRangeRectangles(owner._preeditOffset, owner._preeditText?.Length ?? 0))
            {
                context.DrawLine(pen, rectangle.BottomLeft, rectangle.BottomRight);
            }

            var caret = presenter.GetDocumentCaretRect(owner._preeditOffset + owner._preeditCursor);
            context.DrawRectangle(pen.Brush, null, new(caret.X, caret.Y, 1, Math.Max(1, caret.Height)));
        }
    }
}
