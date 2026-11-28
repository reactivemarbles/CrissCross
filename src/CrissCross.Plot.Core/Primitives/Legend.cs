// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Displays legend items for plottables on a plot.</summary>
/// <param name="plot">The plot whose plottables are displayed in the legend.</param>
public class Legend(Plot plot) : IPlottable, IHasOutline, IHasBackground, IHasShadow
{
    /// <summary>The default distance from the plot edge to the legend.</summary>
    private const int DefaultMargin = 7;

    /// <summary>The default horizontal padding inside the legend.</summary>
    private const int DefaultHorizontalPadding = 10;

    /// <summary>The default vertical padding inside the legend.</summary>
    private const int DefaultVerticalPadding = 5;

    /// <summary>The default width of a legend symbol.</summary>
    private const int DefaultSymbolWidth = 20;

    /// <summary>The default height of a legend symbol.</summary>
    private const int DefaultSymbolHeight = 10;

    /// <summary>The default padding between a symbol and its label.</summary>
    private const int DefaultSymbolPadding = 5;

    /// <summary>The default padding between legend items.</summary>
    private const int DefaultInterItemPadding = 10;

    /// <summary>The default cross-axis padding between legend items.</summary>
    private const int DefaultInterItemCrossPadding = 6;

    /// <summary>The default offset of the legend shadow.</summary>
    private const int DefaultShadowOffset = 3;

    /// <summary>The inset ratio used to draw a legend symbol's fill.</summary>
    private const float SymbolFillInsetRatio = 0.2F;

    /// <summary>The default opacity used for the legend shadow.</summary>
    private const float ShadowOpacity = 0.2F;

    /// <summary>The opacity used to draw legend debug rectangles.</summary>
    private const float DebugRectangleOpacity = 0.2F;

    /// <summary>Gets the plot that owns this legend.</summary>
    public Plot Plot { get; } = plot;

    /// <summary>Gets or sets a value indicating whether the legend is rendered.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets position of the legend relative to the data area.</summary>
    public Alignment Alignment { get; set; } = Alignment.LowerRight;

    /// <summary>Gets or sets the obsolete position alias for the legend.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility alias for the established public API.")]
    [Obsolete("use Alignment")]
    public Alignment Location { get => Alignment; set => Alignment = value; }

    /// <summary>Gets or sets stack items in the legend according to this preferred orientation.</summary>
    public Orientation Orientation { get; set; } = Orientation.Vertical;

    /// <summary>Gets or sets distance from the edge of the data area to the edge of the legend.</summary>
    public PixelPadding Margin { get; set; } = new(DefaultMargin, DefaultMargin);

    /// <summary>Gets or sets distance between the legend frame and the items within it.</summary>
    public PixelPadding Padding { get; set; } = new(DefaultHorizontalPadding, DefaultVerticalPadding);

    /// <summary>Gets or sets width of the symbol in a legend item.</summary>
    public float SymbolWidth { get; set; } = DefaultSymbolWidth;

    /// <summary>Gets or sets height of the symbol in a legend item.</summary>
    public float SymbolHeight { get; set; } = DefaultSymbolHeight;

    /// <summary>Gets or sets padding between a symbol and label within a legend item.</summary>
    public float SymbolPadding { get; set; } = DefaultSymbolPadding;

    /// <summary>Gets or sets space separating legend items.</summary>
    public PixelPadding InterItemPadding { get; set; } = new(DefaultInterItemPadding, DefaultInterItemPadding, DefaultInterItemCrossPadding, DefaultInterItemCrossPadding);

    /// <summary>Gets or sets items in this list will always be displayed in the legend.</summary>
    public List<LegendItem> ManualItems { get; } = [];

    /// <summary>
    /// Gets or sets if enabled, items in horizontal oriented legends will not
    /// be aligned in columns but instead resized tightly to fit their contents.
    /// </summary>
    public bool TightHorizontalWrapping { get; set; }

    /// <summary>Gets or sets a value indicating whether item rectangles are drawn for debugging.</summary>
    public bool ShowItemRectangles_DEBUG { get; set; }

    /// <summary>Gets or sets enabling this allows multi-language text in the figure legend, but may slow down the render loop.</summary>
    public bool SetBestFontOnEachRender { get; set; }

    /// <summary>Gets or sets if set, this overrides the value in the LegendItem's FontStyle.</summary>
    public float? FontSize { get; set; }

    /// <summary>Gets or sets if set, this overrides the value in the LegendItem's FontStyle.</summary>
    public string? FontName { get; set; }

    /// <summary>Gets or sets if set, this overrides the value in the LegendItem's FontStyle.</summary>
    public Color? FontColor { get; set; }

    /// <summary>Gets or sets the obsolete font style.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("Assign FontSize, FontName, or FontColor to control appearance of all legend items", true)]
    public FillStyle Font { get; set; } = new();

    /// <summary>Gets or sets the obsolete multiline setting.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("Multiline is now enabled by default.", true)]
    public bool AllowMultiline { get; set; }

    /// <summary>Gets or sets the layout used to arrange legend items.</summary>
    public ILegendLayout Layout { get; set; } = new LegendLayouts.Wrapping();

    /// <summary>Gets the size produced by the most recent render.</summary>
    public PixelSize LastRenderSize { get; private set; } = PixelSize.NaN;

    /// <summary>Gets or sets the style used to outline the legend.</summary>
    public LineStyle OutlineStyle { get; set; } = new() { Width = 1, Color = Colors.Black, };

    /// <summary>Gets or sets the outline width.</summary>
    public float OutlineWidth { get => OutlineStyle.Width; set => OutlineStyle.Width = value; }

    /// <summary>Gets or sets the outline pattern.</summary>
    public LinePattern OutlinePattern { get => OutlineStyle.Pattern; set => OutlineStyle.Pattern = value; }

    /// <summary>Gets or sets the outline color.</summary>
    public Color OutlineColor { get => OutlineStyle.Color; set => OutlineStyle.Color = value; }

    /// <summary>Gets or sets the obsolete background fill alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("Assign BackgroundColor or interact with BackgroundFillStyle")]
    public FillStyle BackgroundFill { get => BackgroundFillStyle; set { } }

    /// <summary>Gets the fill style used for the legend background.</summary>
    public FillStyle BackgroundFillStyle { get; } = new() { Color = Colors.White };

    /// <summary>Gets or sets the legend background color.</summary>
    public Color BackgroundColor { get => BackgroundFillStyle.Color; set => BackgroundFillStyle.Color = value; }

    /// <summary>Gets or sets the legend background hatch color.</summary>
    public Color BackgroundHatchColor { get => BackgroundFillStyle.HatchColor; set => BackgroundFillStyle.HatchColor = value; }

    /// <summary>Gets or sets the hatch used for the legend background.</summary>
    public IHatch? BackgroundHatch { get => BackgroundFillStyle.Hatch; set => BackgroundFillStyle.Hatch = value; }

    /// <summary>Gets or sets the obsolete shadow fill alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310:Deprecated public API",
        Justification = "Retained as an obsolete compatibility member for the established public API.")]
    [Obsolete("Assign ShadowColor or interact with ShadowFillStyle")]
    public FillStyle ShadowFill { get => ShadowFillStyle; set { } }

    /// <summary>Gets the fill style used for the legend shadow.</summary>
    public FillStyle ShadowFillStyle { get; } = new() { Color = Colors.Black.WithOpacity(ShadowOpacity) };

    /// <summary>Gets or sets the legend shadow color.</summary>
    public Color ShadowColor { get => ShadowFillStyle.Color; set => ShadowFillStyle.Color = value; }

    /// <summary>Gets or sets the offset applied to the legend shadow.</summary>
    public PixelOffset ShadowOffset { get; set; } = new(DefaultShadowOffset, DefaultShadowOffset);

    /// <summary>Gets or sets the alignment of the legend shadow.</summary>
    public Alignment ShadowAlignment { get; set; } = Alignment.LowerRight;

    /// <summary>Gets or sets the axes associated with this legend.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items exposed by this plottable.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets if supplied, always use this marker shape.</summary>
    public MarkerShape? MarkerShapeOverride { get; set; }

    /// <summary>Gets or sets a value indicating whether legend items are collected from plottables.</summary>
    public bool DisplayPlottableLegendItems { get; set; } = true;

    /// <summary>
    /// Gets or sets if enabled, the legend will include items from hidden plottables.
    /// They will be partially painted over using the background color to simulate semitransparency.
    /// </summary>
    public bool ShowItemsFromHiddenPlottables { get; set; }

    /// <summary>
    /// Gets or sets this property controls how visible legend items are when their parent control's visibility is disabled.
    /// This property is only used when <see cref="ShowItemsFromHiddenPlottables"/> is enabled.
    /// </summary>
    public double HiddenItemOpacity { get; set; } = 0.25;

    /// <summary>Gets the axis limits for this legend.</summary>
    /// <returns>This legend does not occupy space in the data area.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Gets the legend items in their display order.</summary>
    /// <returns>The visible legend items collected from manual items and plottables.</returns>
    public virtual LegendItem[] GetItems()
    {
        List<LegendItem> items = GetIndexedManualItems();

        if (DisplayPlottableLegendItems)
        {
            AddPlottableItems(items);
        }

        AddUnindexedManualItems(items);
        _ = items.RemoveAll(static item => !item.IsVisible);
        ApplyItemAppearance(items);

        return items.ToArray();
    }

    /// <summary>Gets the layout of the legend for the supplied size and paint.</summary>
    /// <param name="size">The available size for the legend.</param>
    /// <param name="paint">The paint used to measure text.</param>
    /// <returns>The calculated legend layout.</returns>
    public LegendLayout GetLayout(PixelSize size, Paint paint) => Layout.GetLayout(this, GetItems(), size, paint);

    /// <summary>Gets an image containing just the legend.</summary>
    /// <returns>An image rendered from the current legend items.</returns>
    public Image GetImage()
    {
        using Paint paint = Paint.NewDisposablePaint();
        LegendItem[] items = GetItems();

        LegendLayout lp = items.Length > 0
            ? Layout.GetLayout(this, items, PixelSize.Infinity, paint)
            : LegendLayout.NoLegend;

        SKImageInfo info = new(
            width: Math.Max(1, (int)Math.Ceiling(lp.LegendRect.Width)),
            height: Math.Max(1, (int)Math.Ceiling(lp.LegendRect.Height)),
            colorType: SKColorType.Rgba8888,
            alphaType: SKAlphaType.Premul);

        using SKSurface surface = SKSurface.Create(info);

        RenderLayout(surface.Canvas, paint, lp);

        return new(surface);
    }

    /// <summary>Gets the contents of an SVG image containing just the legend.</summary>
    /// <returns>The SVG document for the rendered legend.</returns>
    public string GetSvgXml()
    {
        LegendItem[] items = GetItems();

        using Paint paint = Paint.NewDisposablePaint();
        LegendLayout lp = items.Length > 0
            ? Layout.GetLayout(this, items, PixelSize.Infinity, paint)
            : LegendLayout.NoLegend;

        int width = (int)Math.Ceiling(lp.LegendRect.Width);
        int height = (int)Math.Ceiling(lp.LegendRect.Height);
        using SvgImage svg = new(width, height);
        RenderLayout(svg.Canvas, paint, lp);
        return svg.GetXml();
    }

    /// <summary>Renders this legend as part of a plot.</summary>
    /// <param name="rp">The render pack containing the canvas, paint, and data rectangle.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        LegendItem[] items = GetItems();
        if (items.Length == 0)
        {
            return;
        }

        PixelRect dataRectAfterMargin = rp.DataRect.Contract(Margin);
        LegendLayout tightLayout = Layout.GetLayout(this, items, dataRectAfterMargin.Size, rp.Paint);
        PixelRect standaloneLegendRect = tightLayout.LegendRect.AlignedInside(dataRectAfterMargin, Alignment);
        PixelOffset legendOffset = new(standaloneLegendRect.Left, standaloneLegendRect.Top);
        LegendLayout layout = new()
        {
            LegendItems = tightLayout.LegendItems,
            LegendRect = tightLayout.LegendRect.WithOffset(legendOffset),
            LabelRects = OffsetRects(tightLayout.LabelRects, legendOffset),
            SymbolRects = OffsetRects(tightLayout.SymbolRects, legendOffset),
        };

        RenderLayout(rp.Canvas, rp.Paint, layout);
    }

    /// <summary>Renders the legend within the supplied rectangle.</summary>
    /// <param name="canvas">The canvas on which to render.</param>
    /// <param name="paint">The paint used to measure and draw content.</param>
    /// <param name="rect">The rectangle available for the legend.</param>
    /// <param name="alignment">The alignment of the legend within the rectangle.</param>
    public void Render(SKCanvas canvas, Paint paint, PixelRect rect, Alignment alignment)
    {
        LegendItem[] items = GetItems();
        if (items.Length == 0)
        {
            return;
        }

        LegendLayout tightLayout = Layout.GetLayout(this, items, rect.Size, paint);
        PixelRect standaloneLegendRect = tightLayout.LegendRect.AlignedInside(rect, alignment);
        PixelOffset legendOffset = new(standaloneLegendRect.Left, standaloneLegendRect.Top);
        LegendLayout layout = new()
        {
            LegendItems = tightLayout.LegendItems,
            LegendRect = tightLayout.LegendRect.WithOffset(legendOffset),
            LabelRects = OffsetRects(tightLayout.LabelRects, legendOffset),
            SymbolRects = OffsetRects(tightLayout.SymbolRects, legendOffset),
        };

        RenderLayout(canvas, paint, layout);
    }

    /// <summary>Returns the supplied rectangles translated by an offset.</summary>
    /// <param name="rects">The rectangles to translate.</param>
    /// <param name="offset">The translation applied to each rectangle.</param>
    /// <returns>A new array containing the translated rectangles.</returns>
    private static PixelRect[] OffsetRects(PixelRect[] rects, PixelOffset offset)
    {
        PixelRect[] result = new PixelRect[rects.Length];
        for (int i = 0; i < rects.Length; i++)
        {
            result[i] = rects[i].WithOffset(offset);
        }

        return result;
    }

    /// <summary>Collects manually added legend items with an index in stable index order.</summary>
    /// <returns>The indexed items in their display order.</returns>
    private List<LegendItem> GetIndexedManualItems()
    {
        List<(LegendItem Item, int Index, int Order)> indexedItems = [];
        int manualItemOrder = 0;
        foreach (LegendItem item in ManualItems)
        {
            if (item.Index is int index)
            {
                indexedItems.Add((item, index, manualItemOrder));
            }

            manualItemOrder++;
        }

        indexedItems.Sort(static (left, right) =>
        {
            int indexComparison = left.Index.CompareTo(right.Index);
            return indexComparison != 0 ? indexComparison : left.Order.CompareTo(right.Order);
        });

        List<LegendItem> items = [];
        foreach (var indexedItem in indexedItems)
        {
            items.Add(indexedItem.Item);
        }

        return items;
    }

    /// <summary>Adds eligible plottable legend items to the destination list.</summary>
    /// <param name="items">The destination list.</param>
    private void AddPlottableItems(List<LegendItem> items)
    {
        foreach (IPlottable plottable in Plot.PlottableList)
        {
            if (!ShowItemsFromHiddenPlottables && !plottable.IsVisible)
            {
                continue;
            }

            foreach (LegendItem item in plottable.LegendItems)
            {
                if (!string.IsNullOrEmpty(item.LabelText))
                {
                    items.Add(item);
                }
            }
        }
    }

    /// <summary>Adds manually added legend items without an index.</summary>
    /// <param name="items">The destination list.</param>
    private void AddUnindexedManualItems(List<LegendItem> items)
    {
        foreach (LegendItem item in ManualItems)
        {
            if (item.Index is null)
            {
                items.Add(item);
            }
        }
    }

    /// <summary>Applies font settings and best-font selection to the legend items.</summary>
    /// <param name="items">The items to update.</param>
    private void ApplyItemAppearance(List<LegendItem> items)
    {
        foreach (LegendItem item in items)
        {
            if (SetBestFontOnEachRender)
            {
                item.LabelStyle.SetBestFont();
            }

            if (FontSize is not null)
            {
                item.LabelFontSize = FontSize.Value;
            }

            if (FontName is not null)
            {
                item.LabelFontName = FontName;
            }

            if (FontColor is not null)
            {
                item.LabelFontColor = FontColor.Value;
            }
        }
    }

    /// <summary>Renders the legend panel and its items using a precomputed layout.</summary>
    /// <param name="canvas">The canvas on which to render.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="layout">The calculated layout to render.</param>
    private void RenderLayout(SKCanvas canvas, Paint paint, in LegendLayout layout)
    {
        LastRenderSize = layout.LegendRect.Size;

        // render the legend panel
        PixelRect shadowRect = layout.LegendRect.WithOffset(ShadowOffset);
        Drawing.FillRectangle(canvas, shadowRect, paint, ShadowFillStyle);
        Drawing.FillRectangle(canvas, layout.LegendRect, paint, BackgroundFillStyle);
        Drawing.DrawRectangle(canvas, layout.LegendRect, paint, OutlineStyle);

        CanvasState canvasState = new(canvas);
        canvasState.Save();

        PixelRect clipRect = layout.LegendRect.Contract(Padding).Expand(1F);
        canvasState.Clip(clipRect);

        // render items inside the legend
        for (int i = 0; i < layout.LegendItems.Length; i++)
        {
            LegendItem item = layout.LegendItems[i];
            PixelRect labelRect = layout.LabelRects[i];
            PixelRect symbolRect = layout.SymbolRects[i];
            PixelRect symbolFillRect = symbolRect.Contract(0, symbolRect.Height * SymbolFillInsetRatio);
            PixelRect symbolFillOutlineRect = symbolFillRect.Expand(1 - item.OutlineWidth);
            PixelLine symbolLine = new(symbolRect.RightCenter, symbolRect.LeftCenter);

            if (ShowItemRectangles_DEBUG)
            {
                LineStyle ls = new() { Color = Colors.Magenta.WithAlpha(DebugRectangleOpacity) };
                Drawing.DrawRectangle(canvas, symbolRect, paint, ls);
                Drawing.DrawRectangle(canvas, labelRect, paint, ls);
            }

            // draw symbol components
            if (MarkerShapeOverride.HasValue)
            {
                item.MarkerShape = MarkerShapeOverride.Value;

                if (item.MarkerColor == Colors.Transparent)
                {
                    item.MarkerColor = item.FillColor;
                }

                // NOTE: Some plottables like signal plots have dynamically sized markers that can get very small
                item.MarkerSize = Math.Max(item.LabelFontSize, item.MarkerSize);

                // when custom marker shape is in use, only render that and the label
                item.LabelStyle.Render(canvas, labelRect.LeftCenter, paint);
                item.MarkerStyle.Render(canvas, symbolRect.Center, paint);
            }
            else
            {
                item.LabelStyle.Render(canvas, labelRect.LeftCenter, paint);
                item.LineStyle.Render(canvas, symbolLine, paint);
                item.FillStyle.Render(canvas, symbolFillRect, paint);
                item.OutlineStyle.Render(canvas, symbolFillOutlineRect, paint);
                item.MarkerStyle.Render(canvas, symbolRect.Center, paint);
                item.ArrowStyle.Render(canvas, symbolLine, paint);
            }

            // Partially hide legend item to reflect that plottable is invisible
            if (item.Plottable?.IsVisible == false)
            {
                PixelRect itemRect = new(symbolRect.TopLeft, labelRect.BottomRight);
                FillStyle fs = new() { Color = BackgroundColor.WithAlpha(1.0 - HiddenItemOpacity) };
                Drawing.FillRectangle(canvas, itemRect, paint, fs);
            }
        }

        canvasState.Restore();
    }
}
