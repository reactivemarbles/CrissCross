// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.BBCode;
#else
namespace CrissCross.Avalonia.UI.Controls.BBCode;
#endif

/// <summary>Creates Avalonia inline content from a parsed BBCode document.</summary>
internal sealed class BbCodeRenderer
{
    /// <summary>The preferred monospaced font families for code formatting.</summary>
    private const string MonospaceFontFamily = "Consolas, Cascadia Mono, monospace";

    /// <summary>Configures spans for simple BBCode format tags.</summary>
    private static readonly Dictionary<string, Action<Span>> SimpleFormatters = new(StringComparer.Ordinal)
    {
        ["b"] = static span => span.FontWeight = FontWeight.Bold,
        ["strong"] = static span => span.FontWeight = FontWeight.Bold,
        ["i"] = static span => span.FontStyle = FontStyle.Italic,
        ["em"] = static span => span.FontStyle = FontStyle.Italic,
        ["u"] = static span => span.TextDecorations = TextDecorations.Underline,
        ["ins"] = static span => span.TextDecorations = TextDecorations.Underline,
        ["s"] = static span => span.TextDecorations = TextDecorations.Strikethrough,
        ["strike"] = static span => span.TextDecorations = TextDecorations.Strikethrough,
        ["del"] = static span => span.TextDecorations = TextDecorations.Strikethrough,
        ["code"] = static span => span.FontFamily = new(MonospaceFontFamily),
        ["c"] = static span => span.FontFamily = new(MonospaceFontFamily),
        ["pre"] = static span => span.FontFamily = new(MonospaceFontFamily),
        ["nfo"] = static span => span.FontFamily = new(MonospaceFontFamily),
    };

    /// <summary>The control that owns the rendered content.</summary>
    private readonly BBCodeBlock _source;

    /// <summary>Initializes a new instance of the <see cref="BbCodeRenderer"/> class.</summary>
    /// <param name="source">The owning control.</param>
    internal BbCodeRenderer(BBCodeBlock source) => _source = source ?? throw new ArgumentNullException(nameof(source));

    /// <summary>Renders a document root.</summary>
    /// <param name="root">The parsed document.</param>
    /// <returns>The rendered root span.</returns>
    internal Span Render(BbCodeNode root)
    {
        var span = new Span();
        AddChildren(span.Inlines, root.Children);
        return span;
    }

    /// <summary>Renders nodes into an inline collection.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="nodes">The source nodes.</param>
    private void AddChildren(InlineCollection target, IEnumerable<BbCodeNode> nodes)
    {
        foreach (var node in nodes)
        {
            AddNode(target, node);
        }
    }

    /// <summary>Renders one node.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddNode(InlineCollection target, BbCodeNode node)
    {
        if (node.IsText)
        {
            target.Add(new Run(node.Text ?? string.Empty));
            return;
        }

        AddMarkupNode(target, node);
    }

    /// <summary>Renders a non-text node.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddMarkupNode(InlineCollection target, BbCodeNode node)
    {
        if (TryAddSimpleFormatting(target, node) || TryAddParameterizedFormatting(target, node) || TryAddLayout(target, node))
        {
            return;
        }

        AddUnformattedMarkup(target, node);
    }

    /// <summary>Renders an unsupported or non-formatting markup node.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddUnformattedMarkup(InlineCollection target, BbCodeNode node)
    {
        if (node.Name is "url" or "link" or "email" or "mail")
        {
            AddLink(target, node);
            return;
        }

        if (node.Name == "br")
        {
            target.Add(new LineBreak());
            return;
        }

        if (node.Name is "root" or "list" or "ul" or "ol")
        {
            AddChildren(target, node.Children);
            return;
        }

        AddUnknown(target, node);
    }

    /// <summary>Adds basic style tags.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    /// <returns>Whether the node was handled.</returns>
    private bool TryAddSimpleFormatting(InlineCollection target, BbCodeNode node)
    {
        if (!SimpleFormatters.TryGetValue(node.Name, out var configure))
        {
            return false;
        }

        AddStyledSpan(target, node, configure);
        return true;
    }

    /// <summary>Adds formatting tags with a value.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    /// <returns>Whether the node was handled.</returns>
    private bool TryAddParameterizedFormatting(InlineCollection target, BbCodeNode node)
    {
        if (node.Name == "color")
        {
            AddColor(target, node);
            return true;
        }

        if (node.Name == "size")
        {
            AddSize(target, node);
            return true;
        }

        if (node.Name != "font")
        {
            return false;
        }

        AddFont(target, node);
        return true;
    }

    /// <summary>Adds simple layout tags.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    /// <returns>Whether the node was handled.</returns>
    private bool TryAddLayout(InlineCollection target, BbCodeNode node)
    {
        if (node.Name is "p" or "paragraph")
        {
            target.Add(new LineBreak());
            AddChildren(target, node.Children);
            target.Add(new LineBreak());
            return true;
        }

        if (node.Name is not ("*" or "li"))
        {
            return false;
        }

        target.Add(new Run("• "));
        AddChildren(target, node.Children);
        target.Add(new LineBreak());
        return true;
    }

    /// <summary>Adds a span configured by the supplied operation.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    /// <param name="configure">The span configuration.</param>
    private void AddStyledSpan(InlineCollection target, BbCodeNode node, Action<Span> configure)
    {
        var span = new Span();
        configure(span);
        AddChildren(span.Inlines, node.Children);
        target.Add(span);
    }

    /// <summary>Adds valid color formatting.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddColor(InlineCollection target, BbCodeNode node)
    {
        if (Color.TryParse(node.Value, out var color))
        {
            AddStyledSpan(target, node, span => span.Foreground = new SolidColorBrush(color));
            return;
        }

        AddChildren(target, node.Children);
    }

    /// <summary>Adds valid font-size formatting.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddSize(InlineCollection target, BbCodeNode node)
    {
        if (double.TryParse(node.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var size) && size > 0D)
        {
            AddStyledSpan(target, node, span => span.FontSize = size);
            return;
        }

        AddChildren(target, node.Children);
    }

    /// <summary>Adds font-family formatting.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddFont(InlineCollection target, BbCodeNode node)
    {
        if (string.IsNullOrWhiteSpace(node.Value))
        {
            AddChildren(target, node.Children);
            return;
        }

        AddStyledSpan(target, node, span => span.FontFamily = new(node.Value));
    }

    /// <summary>Adds a safe interactive link.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddLink(InlineCollection target, BbCodeNode node)
    {
        var address = LinkUtilities.GetAddress(node);
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri) || uri is null || !LinkUtilities.IsAllowedScheme(uri.Scheme))
        {
            AddChildren(target, node.Children);
            return;
        }

        var label = node.GetText();
        var button = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = default,
            Content = string.IsNullOrWhiteSpace(label) ? address : label,
            HorizontalAlignment = HorizontalAlignment.Left,
            Padding = default,
        };
        button.Classes.Add("hyperlink");
        button.Click += (_, _) => _source.Navigate(uri);
        target.Add(button);
    }

    /// <summary>Preserves unsupported markup visibly.</summary>
    /// <param name="target">The output collection.</param>
    /// <param name="node">The source node.</param>
    private void AddUnknown(InlineCollection target, BbCodeNode node)
    {
        target.Add(new Run(node.RawOpeningTag));
        AddChildren(target, node.Children);
        target.Add(new Run($"[/{node.Name}]"));
    }

    /// <summary>Provides link-specific formatting helpers.</summary>
    private static class LinkUtilities
    {
        /// <summary>Gets the URI source associated with a link node.</summary>
        /// <param name="node">The source node.</param>
        /// <returns>The URI text.</returns>
        internal static string GetAddress(BbCodeNode node)
        {
            var address = node.Value ?? node.GetText();
            return node.Name is "email" or "mail" && !address.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                ? $"mailto:{address}"
                : address;
        }

        /// <summary>Determines whether a URI scheme is supported.</summary>
        /// <param name="scheme">The URI scheme.</param>
        /// <returns>Whether the scheme is allowed.</returns>
        internal static bool IsAllowedScheme(string scheme) => scheme is "cmd" or "http" or "https" or "mailto";
    }
}
