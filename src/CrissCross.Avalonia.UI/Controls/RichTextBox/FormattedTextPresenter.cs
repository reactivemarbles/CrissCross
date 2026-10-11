// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>A control that displays formatted text using Inlines.</summary>
public partial class FormattedTextPresenter : TextBlock
{
    /// <summary>Property for <see cref="Document"/>.</summary>
    public static readonly StyledProperty<FlowDocument?> DocumentProperty = AvaloniaProperty.Register<
        FormattedTextPresenter,
        FlowDocument?
    >(nameof(Document));

    /// <summary>Property for <see cref="DefaultForeground"/>.</summary>
    public static readonly StyledProperty<IBrush?> DefaultForegroundProperty = AvaloniaProperty.Register<
        FormattedTextPresenter,
        IBrush?
    >(nameof(DefaultForeground));

    /// <summary>Property for <see cref="DefaultFontSize"/>.</summary>
    public static readonly StyledProperty<double> DefaultFontSizeProperty = AvaloniaProperty.Register<
        FormattedTextPresenter,
        double
    >(nameof(DefaultFontSize), 14);

    /// <summary>Provides the DefaultImageMaxWidth member.</summary>
    private const double DefaultImageMaxWidth = 640;

    /// <summary>Provides the DefaultImageMaxHeight member.</summary>
    private const double DefaultImageMaxHeight = 480;

    /// <summary>Pixel tolerance used when comparing line geometry.</summary>
    private const double GeometryTolerance = 0.5;

    /// <summary>Default extent reserved for unavailable images.</summary>
    private const double MissingImageExtent = 32;

    /// <summary>Padding inside table cells.</summary>
    private const double CellPadding = 5;

    /// <summary>Maps document ranges to the text formatter's ranges.</summary>
    private readonly List<RichLayoutRange> _layoutRanges = [];

    /// <summary>Tracks the text formatter offset while constructing inlines.</summary>
    private int _layoutOffset;

    /// <summary>The current available content width.</summary>
    private double _contentWidth = DefaultImageMaxWidth;

    /// <summary>Rich content for a presenter embedded in a table cell.</summary>
    private IReadOnlyList<TextSegment>? _inlineSegments;

    /// <summary>Whether this presenter already owns one independently aligned paragraph.</summary>
    private bool _isParagraph;

    /// <summary>Raised when a hyperlink is activated.</summary>
    public event EventHandler<RichTextHyperlinkEventArgs>? HyperlinkActivated;

    /// <summary>Gets or sets whether HTTP/HTTPS image sources may be resolved by RemoteImageLoader.</summary>
    public bool IsRemoteImageLoadingEnabled { get; set; }

    /// <summary>Gets or sets the opt-in remote image loader used for HTTP/HTTPS image sources.</summary>
    public Func<Uri, IImage?>? RemoteImageLoader { get; set; }

    /// <summary>Gets or sets the document to display.</summary>
    public FlowDocument? Document
    {
        get => GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    /// <summary>Gets or sets the default foreground brush.</summary>
    public IBrush? DefaultForeground
    {
        get => GetValue(DefaultForegroundProperty);
        set => SetValue(DefaultForegroundProperty, value);
    }

    /// <summary>Gets or sets the default font size.</summary>
    public double DefaultFontSize
    {
        get => GetValue(DefaultFontSizeProperty);
        set => SetValue(DefaultFontSizeProperty, value);
    }

    /// <summary>Gets or sets a shared image resolver for composed page presenters.</summary>
    internal Func<string, IImage?>? ImageSourceLoader { get; set; }

    /// <summary>Updates the inline collection from the document.</summary>
    public void UpdateInlines()
    {
        Inlines?.Clear();
        _layoutRanges.Clear();
        _layoutOffset = 0;

        if (!_isParagraph)
        {
            TextAlignment = TextAlignment.Left;
        }

        var segments = Document?.Segments ?? _inlineSegments;
        if (segments is null || segments.Count == 0)
        {
            return;
        }

        if (!_isParagraph && AppendAlignedParagraphs(segments))
        {
            return;
        }

        foreach (var segment in segments)
        {
            AppendSegment(segment);
        }
    }

    /// <summary>Measures the formatted document, including inline images and tables.</summary>
    /// <param name="width">The available document width.</param>
    /// <returns>The measured document size.</returns>
    public Size MeasureDocument(double width)
    {
        Measure(new(Math.Max(1, width), double.PositiveInfinity));
        return DesiredSize;
    }

    /// <summary>Converts a formatter text index to a document offset.</summary>
    /// <param name="layoutIndex">The formatter index.</param>
    /// <returns>The document offset.</returns>
    public int GetDocumentOffset(int layoutIndex)
    {
        foreach (var range in _layoutRanges)
        {
            if (layoutIndex < range.LayoutStart + range.LayoutLength)
            {
                return range.DocumentStart + (range.LayoutLength == range.DocumentLength
                    ? Math.Clamp(layoutIndex - range.LayoutStart, 0, range.DocumentLength)
                    : 0);
            }
        }

        return _layoutRanges.Count > 0 ? _layoutRanges[^1].DocumentStart + _layoutRanges[^1].DocumentLength : 0;
    }

    /// <summary>Converts a document offset to a formatter text index.</summary>
    /// <param name="documentOffset">The document offset.</param>
    /// <returns>The formatter index.</returns>
    public int GetLayoutOffset(int documentOffset)
    {
        foreach (var range in _layoutRanges)
        {
            if (documentOffset < range.DocumentStart + range.DocumentLength)
            {
                return range.LayoutStart + (range.LayoutLength == range.DocumentLength
                    ? Math.Clamp(documentOffset - range.DocumentStart, 0, range.LayoutLength)
                    : 0);
            }
        }

        return _layoutOffset;
    }

    /// <summary>Finds the document position under a point in this presenter's coordinates.</summary>
    /// <param name="point">The local point.</param>
    /// <returns>The document offset.</returns>
    public int GetDocumentPosition(Point point)
    {
        foreach (var range in _layoutRanges)
        {
            if (range.Child is FormattedTextPresenter paragraph)
            {
                var local = this.TranslatePoint(point, paragraph);
                if (local is { } paragraphPoint && paragraphPoint.Y >= 0 && paragraphPoint.Y < paragraph.Bounds.Height)
                {
                    return range.DocumentStart + paragraph.GetDocumentPosition(paragraphPoint);
                }
            }

            if (range.Child is Grid grid && range.Table is { } table)
            {
                var position = HitTestTable(point, grid, table);
                if (position.HasValue)
                {
                    return range.DocumentStart + position.Value;
                }
            }
        }

        var hit = TextLayout.HitTestPoint(point);
        return GetDocumentOffset(hit.TextPosition + (hit.IsTrailing ? 1 : 0));
    }

    /// <summary>Gets the caret rectangle for a document offset.</summary>
    /// <param name="documentOffset">The document offset.</param>
    /// <returns>A rectangle in presenter coordinates.</returns>
    public Rect GetDocumentCaretRect(int documentOffset)
    {
        foreach (var range in _layoutRanges)
        {
            var paragraphCaret = GetParagraphCaretRect(range, documentOffset);
            if (paragraphCaret.HasValue)
            {
                return paragraphCaret.Value;
            }

            if (range.Table is { } table && range.Child is Grid grid)
            {
                var rectangle = GetTableCaretRect(documentOffset - range.DocumentStart, grid, table);
                if (rectangle.HasValue)
                {
                    return rectangle.Value;
                }
            }
        }

        var result = TextLayout.HitTestTextPosition(GetLayoutOffset(documentOffset));
        return new(result.X, result.Y, 1, Math.Max(result.Height, DefaultFontSize));
    }

    /// <summary>Gets selection rectangles for a document range.</summary>
    /// <param name="start">The document start offset.</param>
    /// <param name="length">The range length.</param>
    /// <returns>The rectangles in presenter coordinates.</returns>
    public IReadOnlyList<Rect> GetDocumentRangeRectangles(int start, int length)
    {
        var rectangles = new List<Rect>();
        var end = Math.Min(GetDocumentOffset(_layoutOffset), start + Math.Max(0, length));
        for (var index = Math.Max(0, start); index < end; index++)
        {
            var left = GetDocumentCaretRect(index);
            var right = GetDocumentCaretRect(index + 1);
            var width = Math.Abs(right.Y - left.Y) < GeometryTolerance ? Math.Max(1, right.X - left.X) : Math.Max(1, Bounds.Width - left.X);
            var next = new Rect(left.X, left.Y, width, left.Height);
            if (rectangles.Count > 0 && Math.Abs(rectangles[^1].Y - next.Y) < GeometryTolerance
                && Math.Abs(rectangles[^1].Right - next.X) < GeometryTolerance)
            {
                rectangles[^1] = rectangles[^1].Union(next);
            }
            else
            {
                rectangles.Add(next);
            }
        }

        return rectangles;
    }

    /// <summary>Creates a sized image element for a document image segment.</summary>
    /// <param name="segment">The image segment.</param>
    /// <returns>The image element, or <see langword="null"/> when the source cannot be loaded.</returns>
    internal Image? CreateImageElement(TextSegment segment)
    {
        if (string.IsNullOrWhiteSpace(segment.ImageSource))
        {
            return null;
        }

        var bitmap = ImageSourceLoader is null ? LoadUncachedImage(segment.ImageSource) : ImageSourceLoader(segment.ImageSource);
        if (bitmap is null)
        {
            return null;
        }

        var image = new Image
        {
            Source = bitmap,
            MaxWidth = Math.Min(DefaultImageMaxWidth, _contentWidth),
            MaxHeight = DefaultImageMaxHeight,
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.DownOnly,
            HorizontalAlignment = segment.ImageAlignment,
        };

        if (segment.ImageWidth.HasValue)
        {
            image.Width = segment.ImageWidth.Value;
        }

        if (segment.ImageHeight.HasValue)
        {
            image.Height = segment.ImageHeight.Value;
        }

        if (
            !segment.ImageWidth.HasValue
            && !segment.ImageHeight.HasValue
            && bitmap.Size is { Width: > 0, Height: > 0 } naturalSize)
        {
            var scale = Math.Min(
                1,
                Math.Min(DefaultImageMaxWidth / naturalSize.Width, DefaultImageMaxHeight / naturalSize.Height));
            image.Width = naturalSize.Width * scale;
            image.Height = naturalSize.Height * scale;
        }

        return image;
    }

    /// <summary>Loads an image through the built-in resolver without consulting a shared cache.</summary>
    /// <param name="source">The image source.</param>
    /// <returns>The decoded image or null when it cannot be loaded.</returns>
    internal IImage? LoadUncachedImage(string source) => TryLoadImage(source, out var image) ? image : null;

    /// <summary>Flattens independently formatted paragraphs into measured editable lines.</summary>
    /// <returns>The measured lines in document order.</returns>
    internal List<DocumentPageLayout.MeasuredLine> GetMeasuredDocumentLines()
    {
        var result = new List<DocumentPageLayout.MeasuredLine>();
        foreach (var line in TextLayout.TextLines)
        {
            var range = FindParagraphRange(line.FirstTextSourceIndex, line.Length);
            var end = GetDocumentOffset(line.FirstTextSourceIndex + line.Length);
            if (range is { Child: FormattedTextPresenter paragraph } paragraphRange)
            {
                AppendMeasuredParagraphLines(result, paragraph, paragraphRange.DocumentStart, end, line.Height);
            }
            else if (range is { Child: Grid grid, Table: { } table } tableRange)
            {
                AppendMeasuredTableRows(result, grid, table, tableRange.DocumentStart, end, line.Height);
            }
            else
            {
                result.Add(new(GetDocumentOffset(line.FirstTextSourceIndex), end, line.Height));
            }
        }

        return result;
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsFinite(availableSize.Width) ? Math.Max(1, availableSize.Width) : DefaultImageMaxWidth;
        if (Math.Abs(_contentWidth - width) > GeometryTolerance)
        {
            _contentWidth = width;
            UpdateInlines();
        }

        return base.MeasureOverride(availableSize);
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Handled || e.InitialPressMouseButton != MouseButton.Left)
        {
            return;
        }

        base.OnPointerReleased(e);
        var offset = GetDocumentPosition(e.GetPosition(this));
        var segments = Document?.Segments ?? _inlineSegments;
        if (segments is null)
        {
            return;
        }

        ActivateHyperlink(segments, offset);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null)
        {
            return;
        }

        if (
            change.Property != DocumentProperty
            && change.Property != DefaultForegroundProperty
            && change.Property != DefaultFontSizeProperty
            && change.Property != ForegroundProperty
            && change.Property != FontSizeProperty
            && change.Property != FontFamilyProperty)
        {
            return;
        }

        UpdateInlines();
    }

    /// <summary>Reserves a visible inline extent for an unavailable image.</summary>
    /// <param name="segment">The image metadata.</param>
    /// <returns>The placeholder control.</returns>
    private static Border CreateMissingImageElement(TextSegment segment)
    {
        var width = segment.ImageWidth ?? MissingImageExtent;
        var height = segment.ImageHeight ?? MissingImageExtent;
        return new Border { Width = width, Height = height, BorderBrush = Brushes.Gray, BorderThickness = new(1) };
    }

    /// <summary>Provides the TryLoadDataUri member.</summary>
    /// <param name="source">The image source.</param>
    /// <param name="bitmap">The loaded bitmap.</param>
    /// <returns><see langword="true"/> when the data URI was loaded.</returns>
    private static bool TryLoadDataUri(string source, out IImage? bitmap)
    {
        bitmap = null;
        if (!source.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var commaIndex = source.IndexOf(',');
        if (commaIndex < 0)
        {
            return false;
        }

        var metadata = source[..commaIndex];
        if (!metadata.Contains(";base64", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var payload = source[(commaIndex + 1)..].Trim();
        if (payload.Length == 0)
        {
            return false;
        }

        try
        {
            var normalizedPayload = Uri.UnescapeDataString(payload).Replace(" ", "+", StringComparison.Ordinal);
            var bytes = Convert.FromBase64String(normalizedPayload);
            if (bytes.Length == 0)
            {
                return false;
            }

            using var memory = new MemoryStream(bytes);
            bitmap = new Bitmap(memory);
            return true;
        }
        catch
        {
            bitmap = null;
            return false;
        }
    }

    /// <summary>Provides the CreateBitmapFromStream member.</summary>
    /// <param name="stream">The source stream.</param>
    /// <returns>The decoded bitmap.</returns>
    private static Bitmap CreateBitmapFromStream(Stream stream)
    {
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        buffer.Position = 0;
        return new(buffer);
    }

    /// <summary>Loads an image from a local file.</summary>
    /// <param name="path">The local image path.</param>
    /// <returns>The loaded image, or <see langword="null"/> when the file does not exist.</returns>
    private static Bitmap? LoadImageFile(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        return CreateBitmapFromStream(stream);
    }

    /// <summary>Replaces an atomic parent paragraph line with its actual wrapped text lines.</summary>
    /// <param name="result">The flattened measured lines.</param>
    /// <param name="paragraph">The independent paragraph presenter.</param>
    /// <param name="start">The paragraph document start.</param>
    /// <param name="end">The parent line document end including its paragraph break.</param>
    /// <param name="height">The parent formatter line height.</param>
    private static void AppendMeasuredParagraphLines(
        List<DocumentPageLayout.MeasuredLine> result,
        FormattedTextPresenter paragraph,
        int start,
        int end,
        double height)
    {
        var lines = paragraph.GetMeasuredDocumentLines();
        var remaining = height;
        var leading = Math.Max(0, height - paragraph.DesiredSize.Height);
        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var last = index == lines.Count - 1;
            var lineHeight = last ? Math.Max(0, remaining) : line.Height + (index == 0 ? leading : 0);
            result.Add(new(start + line.Start, last ? end : start + line.End, lineHeight));
            remaining -= lineHeight;
        }
    }

    /// <summary>Creates an image or its measured placeholder.</summary>
    /// <param name="segment">The image segment.</param>
    /// <returns>The inline image control.</returns>
    private Control CreateImageChild(TextSegment segment)
    {
        var image = CreateImageElement(segment);
        return image is not null ? image : CreateMissingImageElement(segment);
    }

    /// <summary>Raises navigation for a hyperlink containing the released pointer position.</summary>
    /// <param name="segments">The displayed segments.</param>
    /// <param name="offset">The document position.</param>
    private void ActivateHyperlink(IReadOnlyList<TextSegment> segments, int offset)
    {
        foreach (var segment in segments)
        {
            if (segment.HyperlinkNavigationUri is { } uri && offset >= segment.StartIndex && offset < segment.EndIndex)
            {
                HyperlinkActivated?.Invoke(this, new RichTextHyperlinkEventArgs(uri, segment.HyperlinkTarget));
                return;
            }
        }
    }

    /// <summary>Appends one segment to the inline collection.</summary>
    /// <param name="segment">The segment to append.</param>
    private void AppendSegment(TextSegment segment)
    {
        var start = _layoutOffset;
        if (segment.ListMarker is { } marker)
        {
            Inlines!.Add(new Run(marker));
            AddLayoutRange(segment, start, marker.Length);
            return;
        }

        if (segment.IsPageBreak)
        {
            Inlines!.Add(new Run("\n"));
            AddLayoutRange(segment, start, 1);
            return;
        }

        if (segment.Table is { } table)
        {
            var grid = CreateTableElement(table);
            Inlines!.Add(new InlineUIContainer(grid));
            _layoutRanges.Add(new(segment.StartIndex, segment.Length, start, 1, grid, table));
            _layoutOffset++;
            return;
        }

        if (segment.IsParagraphBreak)
        {
            AppendParagraphBreak();
            AddLayoutRange(segment, start, Environment.NewLine.Length);
            return;
        }

        if (segment.IsLineBreak)
        {
            AppendLineBreak();
            AddLayoutRange(segment, start, Environment.NewLine.Length);
            return;
        }

        if (segment.IsImage)
        {
            var child = CreateImageChild(segment);
            Inlines!.Add(new InlineUIContainer(child));
            _layoutRanges.Add(new(segment.StartIndex, 1, start, 1, child, null));
            _layoutOffset++;
            return;
        }

        if (!segment.HasRenderableText)
        {
            return;
        }

        AppendRunSegment(segment);
        AddLayoutRange(segment, start, segment.Text.Length);
    }

    /// <summary>Appends a paragraph break.</summary>
    private void AppendParagraphBreak() => Inlines!.Add(new Run(Environment.NewLine));

    /// <summary>Appends a line break.</summary>
    private void AppendLineBreak() => AppendParagraphBreak();

    /// <summary>Builds independently aligned paragraph children when their alignments differ.</summary>
    /// <param name="segments">The content segments.</param>
    /// <returns>Whether independent paragraph children were created.</returns>
    private bool AppendAlignedParagraphs(IReadOnlyList<TextSegment> segments)
    {
        var paragraphs = RichParagraphLayout.Create(segments);
        if (!RichParagraphLayout.HasMixedAlignment(paragraphs))
        {
            TextAlignment = paragraphs.Count > 0 ? paragraphs[0].Alignment : TextAlignment.Left;
            return false;
        }

        foreach (var paragraph in paragraphs)
        {
            var child = CreateParagraphPresenter(paragraph);
            Inlines!.Add(new InlineUIContainer(child) { BaselineAlignment = BaselineAlignment.Bottom });
            var end = paragraph.ParagraphBreak?.EndIndex ?? paragraph.End;
            _layoutRanges.Add(new(paragraph.Start, end - paragraph.Start, _layoutOffset, 1, child, null));
            _layoutOffset++;
            if (paragraph.ParagraphBreak is { } paragraphBreak)
            {
                AppendSegment(paragraphBreak);
            }
        }

        return true;
    }

    /// <summary>Creates a child that formats one paragraph at the available document width.</summary>
    /// <param name="paragraph">The paragraph content.</param>
    /// <returns>The configured child presenter.</returns>
    private FormattedTextPresenter CreateParagraphPresenter(RichParagraphLayout.ParagraphContent paragraph)
    {
        var presenter = new FormattedTextPresenter
        {
            _isParagraph = true,
            ImageSourceLoader = ImageSourceLoader,
            IsRemoteImageLoadingEnabled = IsRemoteImageLoadingEnabled,
            RemoteImageLoader = RemoteImageLoader,
            Width = _contentWidth,
            DefaultForeground = DefaultForeground ?? Foreground,
            Foreground = DefaultForeground ?? Foreground,
            DefaultFontSize = DefaultFontSize,
            FontSize = FontSize,
            FontFamily = FontFamily,
            TextWrapping = TextWrapping,
            TextAlignment = paragraph.Alignment,
            _inlineSegments = paragraph.CreateLocalSegments(),
        };
        presenter.UpdateInlines();
        presenter.HyperlinkActivated += OnCellHyperlinkActivated;
        return presenter;
    }

    /// <summary>Finds the paragraph child occupying one parent formatter line.</summary>
    /// <param name="start">The formatter line start.</param>
    /// <param name="length">The formatter line length.</param>
    /// <returns>The child range when this line contains an independent paragraph.</returns>
    private RichLayoutRange? FindParagraphRange(int start, int length)
    {
        foreach (var range in _layoutRanges)
        {
            if (range.Child is FormattedTextPresenter or Grid && range.LayoutStart >= start && range.LayoutStart < start + length)
            {
                return range;
            }
        }

        return null;
    }

    /// <summary>Gets an independently aligned paragraph caret in parent coordinates.</summary>
    /// <param name="range">The paragraph's parent mapping.</param>
    /// <param name="documentOffset">The requested document offset.</param>
    /// <returns>The mapped caret when this paragraph owns the offset.</returns>
    private Rect? GetParagraphCaretRect(in RichLayoutRange range, int documentOffset)
    {
        var end = range.DocumentStart + range.DocumentLength;
        if (range.Child is not FormattedTextPresenter paragraph || documentOffset < range.DocumentStart || documentOffset > end)
        {
            return null;
        }

        if (documentOffset == end && range.LayoutStart + range.LayoutLength != _layoutOffset)
        {
            return null;
        }

        var offset = Math.Min(documentOffset - range.DocumentStart, paragraph.GetDocumentOffset(paragraph._layoutOffset));
        var caret = paragraph.GetDocumentCaretRect(offset);
        var origin = paragraph.TranslatePoint(caret.Position, this);
        return origin.HasValue ? new(origin.Value, caret.Size) : null;
    }

    /// <summary>Records the relationship between one segment and its formatter range.</summary>
    /// <param name="segment">The document segment.</param>
    /// <param name="start">The formatter start.</param>
    /// <param name="length">The formatter length.</param>
    private void AddLayoutRange(TextSegment segment, int start, int length)
    {
        _layoutRanges.Add(new(segment.StartIndex, segment.Length, start, length, null, null));
        _layoutOffset += length;
    }

    /// <summary>Builds a real grid with rich presenters in its spanning cells.</summary>
    /// <param name="table">The table model.</param>
    /// <returns>The table grid.</returns>
    private Grid CreateTableElement(RichTextTable table)
    {
        var grid = new Grid { Width = Math.Min(table.Width ?? _contentWidth, _contentWidth) };
        for (var column = 0; column < table.ColumnCount; column++)
        {
            grid.ColumnDefinitions.Add(new(GridLength.Star));
        }

        for (var row = 0; row < table.RowCount; row++)
        {
            grid.RowDefinitions.Add(new(GridLength.Auto));
        }

        foreach (var cell in table.Cells)
        {
            var presenter = new FormattedTextPresenter
            {
                DefaultForeground = DefaultForeground ?? Foreground,
                DefaultFontSize = DefaultFontSize,
                FontFamily = FontFamily,
                TextWrapping = TextWrapping.Wrap,
                IsRemoteImageLoadingEnabled = IsRemoteImageLoadingEnabled,
                RemoteImageLoader = RemoteImageLoader,
                ImageSourceLoader = ImageSourceLoader,
                _inlineSegments = cell.Segments,
                VerticalAlignment = cell.ContentAlignment,
            };
            presenter.UpdateInlines();
            presenter.HyperlinkActivated += OnCellHyperlinkActivated;

            var border = new Border
            {
                Child = presenter,
                Padding = cell.Padding ?? new(CellPadding),
                BorderThickness = cell.BorderThickness ?? new(GeometryTolerance),
                BorderBrush = cell.BorderBrush ?? Brushes.Gray,
                Background = cell.Background,
                MinWidth = Math.Min(cell.Width ?? 0, grid.Width / Math.Max(1, table.ColumnCount)),
            };
            Grid.SetRow(border, cell.Row);
            Grid.SetColumn(border, cell.Column);
            Grid.SetRowSpan(border, cell.RowSpan);
            Grid.SetColumnSpan(border, cell.ColumnSpan);
            grid.Children.Add(border);
        }

        return grid;
    }

    /// <summary>Finds a table cell position under the pointer.</summary>
    /// <param name="point">The point in this presenter's coordinates.</param>
    /// <param name="grid">The table grid.</param>
    /// <param name="table">The table model.</param>
    /// <returns>The offset within the table when a cell is hit.</returns>
    private int? HitTestTable(Point point, Grid grid, RichTextTable table)
    {
        for (var index = 0; index < grid.Children.Count; index++)
        {
            if (grid.Children[index] is Border { Child: FormattedTextPresenter presenter } border)
            {
                var cellPoint = this.TranslatePoint(point, presenter);
                var borderPoint = this.TranslatePoint(point, border);
                if (cellPoint is { } cellLocal && borderPoint is { } borderLocal
                    && new Rect(border.Bounds.Size).Contains(borderLocal))
                {
                    return table.Cells[index].StartIndex + presenter.GetDocumentPosition(cellLocal);
                }
            }
        }

        return null;
    }

    /// <summary>Gets a caret rectangle from a nested table cell.</summary>
    /// <param name="offset">The offset relative to the table.</param>
    /// <param name="grid">The table grid.</param>
    /// <param name="table">The table model.</param>
    /// <returns>The caret rectangle when the offset is in a cell.</returns>
    private Rect? GetTableCaretRect(int offset, Grid grid, RichTextTable table)
    {
        for (var index = 0; index < table.Cells.Count; index++)
        {
            var cell = table.Cells[index];
            var relative = offset - cell.StartIndex;
            if (relative >= 0 && relative <= cell.Length
                && grid.Children[index] is Border { Child: FormattedTextPresenter presenter })
            {
                var rect = presenter.GetDocumentCaretRect(relative);
                var origin = presenter.TranslatePoint(rect.Position, this);
                if (origin.HasValue)
                {
                    return new(origin.Value, rect.Size);
                }
            }
        }

        return null;
    }

    /// <summary>Forwards link activation from a nested table cell.</summary>
    /// <param name="sender">The nested presenter.</param>
    /// <param name="args">The hyperlink activation.</param>
    private void OnCellHyperlinkActivated(object? sender, RichTextHyperlinkEventArgs args) => HyperlinkActivated?.Invoke(this, args);

    /// <summary>Appends a text run segment.</summary>
    /// <param name="segment">The text segment.</param>
    private void AppendRunSegment(TextSegment segment)
    {
        var run = new Run(segment.Text)
        {
            FontWeight = segment.FontWeight,
            FontStyle = segment.FontStyle,
            FontSize = segment.FontSize ?? DefaultFontSize,
            FontFamily = segment.FontFamily ?? FontFamily,
            Foreground = segment.Foreground ?? DefaultForeground ?? Foreground,
        };

        if (segment.TextDecorations is { } decorations)
        {
            run.TextDecorations = decorations;
        }

        if (segment.Background is not null)
        {
            run.Background = segment.Background;
        }

        Inlines!.Add(run);
    }

    /// <summary>Provides the TryLoadImage member.</summary>
    /// <param name="source">The source value.</param>
    /// <param name="bitmap">The bitmap value.</param>
    /// <returns>The result.</returns>
    private bool TryLoadImage(string source, out IImage? bitmap)
    {
        try
        {
            bitmap = LoadImage(source);
            return bitmap is not null;
        }
        catch (IOException)
        {
            bitmap = null;
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            bitmap = null;
            return false;
        }
        catch (InvalidOperationException)
        {
            bitmap = null;
            return false;
        }
        catch (NotSupportedException)
        {
            bitmap = null;
            return false;
        }
        catch (ArgumentException)
        {
            bitmap = null;
            return false;
        }
    }

    /// <summary>Loads one supported image source.</summary>
    /// <param name="source">The image source.</param>
    /// <returns>The loaded image, or <see langword="null"/> when no supported source exists.</returns>
    private IImage? LoadImage(string source)
    {
        if (TryLoadDataUri(source, out var dataImage))
        {
            return dataImage;
        }

        if (!Uri.TryCreate(source, UriKind.Absolute, out var absolute))
        {
            return LoadImageFile(source);
        }

        if (absolute.IsFile)
        {
            return LoadImageFile(absolute.LocalPath);
        }

        if (absolute.Scheme is "http" or "https")
        {
            return TryLoadRemoteImage(absolute, out var remoteImage) ? remoteImage : null;
        }

        using var assetStream = AssetLoader.Open(absolute);
        return CreateBitmapFromStream(assetStream);
    }

    /// <summary>Provides the TryLoadRemoteImage member.</summary>
    /// <param name="uri">The uri value.</param>
    /// <param name="bitmap">The bitmap value.</param>
    /// <returns>The result.</returns>
    private bool TryLoadRemoteImage(Uri uri, out IImage? bitmap)
    {
        bitmap = null;
        if (!IsRemoteImageLoadingEnabled || RemoteImageLoader is null)
        {
            return false;
        }

        try
        {
            bitmap = RemoteImageLoader(uri);
            return bitmap is not null;
        }
        catch
        {
            bitmap = null;
            return false;
        }
    }

    /// <summary>Maps one document range to its formatter range and optional inline child.</summary>
    /// <param name="DocumentStart">The document start.</param>
    /// <param name="DocumentLength">The document length.</param>
    /// <param name="LayoutStart">The formatter start.</param>
    /// <param name="LayoutLength">The formatter length.</param>
    /// <param name="Child">The optional inline control.</param>
    /// <param name="Table">The optional table model.</param>
    private readonly record struct RichLayoutRange(int DocumentStart, int DocumentLength, int LayoutStart, int LayoutLength, Control? Child, RichTextTable? Table);
}
