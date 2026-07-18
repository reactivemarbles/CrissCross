// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Documents;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode;
#else
namespace CrissCross.WPF.UI.Controls.BBCode;
#endif

/// <summary>Creates WPF inline content from a parsed BBCode document.</summary>
internal sealed partial class BbCodeRenderer
{
    /// <summary>The shared corner radius for generated content.</summary>
    private const double GeneratedCornerRadius = 4D;

    /// <summary>The outer table spacing.</summary>
    private const double TableSpacing = 4D;

    /// <summary>The horizontal table-cell padding.</summary>
    private const double TableCellHorizontalPadding = 8D;

    /// <summary>The vertical table-cell padding.</summary>
    private const double TableCellVerticalPadding = 5D;

    /// <summary>Tags supported by the renderer.</summary>
    private static readonly HashSet<string> KnownTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "*",
        "align",
        "b",
        "bbvideo",
        "blur",
        "br",
        "c",
        "center",
        "code",
        "color",
        "del",
        "email",
        "em",
        "font",
        "gvideo",
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6",
        "header",
        "heading",
        "hide",
        "hr",
        "i",
        "image",
        "img",
        "ins",
        "justify",
        "left",
        "li",
        "line",
        "link",
        "list",
        "mail",
        "nfo",
        "noparse",
        "ol",
        "p",
        "paragraph",
        "pipes",
        "pre",
        "q",
        "quote",
        "rate",
        "rating",
        "right",
        "row",
        "s",
        "size",
        "spoil",
        "spoiler",
        "strike",
        "strong",
        "style",
        "sub",
        "sup",
        "table",
        "td",
        "th",
        "tr",
        "u",
        "ul",
        "url",
        "video",
        "youtube",
    };

    /// <summary>The control that owns rendered inline content.</summary>
    private readonly BBCodeBlock _source;

    /// <summary>Initializes a new instance of the <see cref="BbCodeRenderer"/> class.</summary>
    /// <param name="source">The owning control.</param>
    public BbCodeRenderer(BBCodeBlock source) => _source = source ?? throw new ArgumentNullException(nameof(source));

    /// <summary>Renders a parsed document.</summary>
    /// <param name="root">The document root.</param>
    /// <returns>The rendered span.</returns>
    public Span Render(BbCodeNode root)
    {
        var span = new Span();
        AddChildren(span.Inlines, root.Children);
        return span;
    }

    /// <summary>Adds rendered child nodes to an inline collection.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="nodes">The child nodes.</param>
    private void AddChildren(InlineCollection target, IEnumerable<BbCodeNode> nodes)
    {
        foreach (var node in nodes)
        {
            AddNode(target, node);
        }
    }

    /// <summary>Adds one rendered node to an inline collection.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The node to render.</param>
    private void AddNode(InlineCollection target, BbCodeNode node)
    {
        if (node.IsText)
        {
            target.Add(new Run(node.Text ?? string.Empty));
            return;
        }

        if (TryAddFormatting(target, node))
        {
            return;
        }

        if (TryAddInlineElement(target, node))
        {
            return;
        }

        if (TryAddDocumentBlock(target, node))
        {
            return;
        }

        if (TryAddLayoutBlock(target, node))
        {
            return;
        }

        if (TryAddHeading(target, node))
        {
            return;
        }

        if (TryAddMediaBlock(target, node))
        {
            return;
        }

        if (TryAddTransparentNode(target, node))
        {
            return;
        }

        AddUnknown(target, node);
    }

    /// <summary>Attempts to render a formatting node.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The formatting node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddFormatting(InlineCollection target, BbCodeNode node)
    {
        switch (node.Name)
        {
            case "b" or "strong":
            {
                AddStyledSpan(target, node, span => span.FontWeight = FontWeights.Bold);
                return true;
            }

            case "i"
            or "em":
            {
                AddStyledSpan(target, node, span => span.FontStyle = FontStyles.Italic);
                return true;
            }

            case "u"
            or "ins":
            {
                AddStyledSpan(target, node, span => span.TextDecorations = TextDecorations.Underline);
                return true;
            }

            case "s"
            or "strike"
            or "del":
            {
                AddStyledSpan(target, node, span => span.TextDecorations = TextDecorations.Strikethrough);
                return true;
            }

            case "sup":
            {
                AddStyledSpan(target, node, span => span.BaselineAlignment = BaselineAlignment.Superscript);
                return true;
            }

            case "sub":
            {
                AddStyledSpan(target, node, span => span.BaselineAlignment = BaselineAlignment.Subscript);
                return true;
            }

            default:
            {
                return TryAddParameterizedFormatting(target, node);
            }
        }
    }

    /// <summary>Attempts to render formatting that accepts a parameter.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The formatting node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddParameterizedFormatting(InlineCollection target, BbCodeNode node)
    {
        switch (node.Name)
        {
            case "color":
            {
                AddStyledSpan(target, node, span => BbCodeRenderHelpers.ApplyColor(span, node.Value));
                return true;
            }

            case "font":
            {
                AddStyledSpan(target, node, span => BbCodeRenderHelpers.ApplyFont(span, node.Value));
                return true;
            }

            case "size":
            {
                AddStyledSpan(target, node, span => BbCodeRenderHelpers.ApplySize(span, node.Value));
                return true;
            }

            case "style":
            {
                AddStyledSpan(target, node, span => BbCodeRenderHelpers.ApplyStyle(span, node));
                return true;
            }

            default:
            {
                return false;
            }
        }
    }

    /// <summary>Attempts to render an inline or inline-hosted node.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddInlineElement(InlineCollection target, BbCodeNode node)
    {
        switch (node.Name)
        {
            case "br":
            {
                target.Add(new LineBreak());
                return true;
            }

            case "hr"
            or "line":
            {
                target.Add(CreateSeparator());
                return true;
            }

            case "url"
            or "link"
            or "email"
            or "mail":
            {
                AddHyperlink(target, node);
                return true;
            }

            case "img"
            or "image":
            {
                target.Add(CreateImage(node));
                return true;
            }

            case "quote"
            or "q":
            {
                target.Add(CreateQuote(node));
                return true;
            }

            case "spoiler"
            or "spoil"
            or "hide":
            {
                target.Add(CreateSpoiler(node));
                return true;
            }

            case "blur":
            {
                target.Add(CreateBlur(node));
                return true;
            }

            case "noparse":
            {
                target.Add(new Run(node.GetText()));
                return true;
            }

            default:
            {
                return false;
            }
        }
    }

    /// <summary>Attempts to render a block-like node.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddDocumentBlock(InlineCollection target, BbCodeNode node)
    {
        switch (node.Name)
        {
            case "code" or "c" or "pre" or "nfo":
            {
                target.Add(CreateCode(node));
                return true;
            }

            case "ul"
            or "ol"
            or "list":
            {
                target.Add(CreateList(node));
                return true;
            }

            case "table":
            {
                target.Add(CreateTable(node));
                return true;
            }

            case "pipes":
            {
                target.Add(CreatePipeTable(node));
                return true;
            }

            default:
            {
                return false;
            }
        }
    }

    /// <summary>Attempts to render alignment and paragraph blocks.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddLayoutBlock(InlineCollection target, BbCodeNode node)
    {
        if (node.Name is "left" or "center" or "right" or "justify" or "align")
        {
            target.Add(CreateAlignedBlock(node));
            return true;
        }

        if (node.Name is not ("p" or "paragraph"))
        {
            return false;
        }

        target.Add(CreateParagraph(node));
        return true;
    }

    /// <summary>Attempts to render a heading.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddHeading(InlineCollection target, BbCodeNode node)
    {
        if (node.Name is not ("h1" or "h2" or "h3" or "h4" or "h5" or "h6" or "header" or "heading"))
        {
            return false;
        }

        target.Add(CreateHeading(node));
        return true;
    }

    /// <summary>Attempts to render ratings and media links.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddMediaBlock(InlineCollection target, BbCodeNode node)
    {
        if (node.Name is "rate" or "rating")
        {
            target.Add(CreateRating(node));
            return true;
        }

        if (node.Name is not ("youtube" or "gvideo" or "video" or "bbvideo"))
        {
            return false;
        }

        target.Add(CreateMediaLink(node));
        return true;
    }

    /// <summary>Renders structural nodes that are transparent outside their parent control.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when the node was handled.</returns>
    private bool TryAddTransparentNode(InlineCollection target, BbCodeNode node)
    {
        if (node.Name is not ("*" or "li" or "row" or "tr" or "td" or "th"))
        {
            return false;
        }

        AddChildren(target, node.Children);
        return true;
    }

    /// <summary>Renders a styled inline span.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The source node.</param>
    /// <param name="configure">The style operation.</param>
    private void AddStyledSpan(InlineCollection target, BbCodeNode node, Action<Span> configure)
    {
        var span = new Span();
        configure(span);
        AddChildren(span.Inlines, node.Children);
        target.Add(span);
    }

    /// <summary>Renders a safe hyperlink.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The link node.</param>
    private void AddHyperlink(InlineCollection target, BbCodeNode node)
    {
        var address = node.Value ?? node.GetText();
        if (node.Name is "email" or "mail" && !address.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
        {
            address = "mailto:" + address;
        }

        if (!BbCodeRenderHelpers.TryCreateAllowedUri(address, out var uri))
        {
            AddChildren(target, node.Children);
            return;
        }

        Hyperlink hyperlink = new() { NavigateUri = uri };
        hyperlink.SetResourceReference(TextElement.ForegroundProperty, "HyperlinkButtonForeground");
        AddChildren(hyperlink.Inlines, node.Children);
        if (hyperlink.Inlines.Count == 0)
        {
            hyperlink.Inlines.Add(address);
        }

        target.Add(hyperlink);
    }

    /// <summary>Creates a themed text block containing rendered child nodes.</summary>
    /// <param name="nodes">The child nodes.</param>
    /// <returns>The text block.</returns>
    private TextBlock CreateTextBlock(IEnumerable<BbCodeNode> nodes)
    {
        var textBlock = CreateThemedTextBlock();
        AddChildren(textBlock.Inlines, nodes);
        return textBlock;
    }

    /// <summary>Creates a text block linked to the current CrissCross theme.</summary>
    /// <returns>The themed text block.</returns>
    private TextBlock CreateThemedTextBlock()
    {
        TextBlock textBlock = new()
        {
            FontFamily = _source.FontFamily,
            FontSize = _source.FontSize,
            TextWrapping = TextWrapping.Wrap,
        };
        textBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextFillColorPrimaryBrush");
        return textBlock;
    }

    /// <summary>Creates a border linked to CrissCross theme resources.</summary>
    /// <param name="backgroundKey">The background brush resource key.</param>
    /// <returns>The themed border.</returns>
    private Border CreateThemedBorder(string backgroundKey)
    {
        Border border = new()
        {
            BorderThickness = new(1D),
            CornerRadius = new(GeneratedCornerRadius),
            FlowDirection = _source.FlowDirection,
        };
        border.SetResourceReference(Border.BackgroundProperty, backgroundKey);
        border.SetResourceReference(Border.BorderBrushProperty, "ControlStrokeColorDefaultBrush");
        return border;
    }

    /// <summary>Creates a themed grid from table rows.</summary>
    /// <param name="rows">The table rows.</param>
    /// <returns>The table grid.</returns>
    private Grid CreateTableGrid(IReadOnlyList<IReadOnlyList<BbCodeNode>> rows)
    {
        Grid grid = new() { Margin = new(0D, TableSpacing, 0D, TableSpacing) };
        var columnCount = rows.Count == 0 ? 0 : rows.Max(row => row.Count);
        for (var column = 0; column < columnCount; column++)
        {
            grid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        }

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            grid.RowDefinitions.Add(new() { Height = GridLength.Auto });
            for (var columnIndex = 0; columnIndex < rows[rowIndex].Count; columnIndex++)
            {
                var cellNode = rows[rowIndex][columnIndex];
                var backgroundKey =
                    cellNode.Name == "th" ? "ControlFillColorDefaultBrush" : "CardBackgroundFillColorDefaultBrush";
                var border = CreateThemedBorder(backgroundKey);
                border.CornerRadius = default;
                border.Padding = new(
                    TableCellHorizontalPadding,
                    TableCellVerticalPadding,
                    TableCellHorizontalPadding,
                    TableCellVerticalPadding);
                var cell = CreateTextBlock(cellNode.Children.Count > 0 ? cellNode.Children : [cellNode]);
                cell.FontWeight = cellNode.Name == "th" ? FontWeights.SemiBold : FontWeights.Normal;
                border.Child = cell;
                Grid.SetRow(border, rowIndex);
                Grid.SetColumn(border, columnIndex);
                _ = grid.Children.Add(border);
            }
        }

        return grid;
    }

    /// <summary>Renders a known alias transparently or preserves an unknown tag literally.</summary>
    /// <param name="target">The target inline collection.</param>
    /// <param name="node">The unknown node.</param>
    private void AddUnknown(InlineCollection target, BbCodeNode node)
    {
        if (KnownTags.Contains(node.Name))
        {
            AddChildren(target, node.Children);
            return;
        }

        target.Add(new Run(node.RawOpeningTag));
        AddChildren(target, node.Children);
        target.Add(new Run("[/" + node.Name + "]"));
    }
}
