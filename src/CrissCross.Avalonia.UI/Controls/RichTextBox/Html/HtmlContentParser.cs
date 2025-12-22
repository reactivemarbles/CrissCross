// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaHorizontalAlignment = Avalonia.Layout.HorizontalAlignment;

namespace CrissCross.Avalonia.UI.Controls;

internal static class HtmlContentParser
{
    private static readonly IBrowsingContext BrowsingContext = AngleSharp.BrowsingContext.New(Configuration.Default);

    public static IReadOnlyList<TextSegment> Parse(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return [];
        }

        var document = BrowsingContext.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();
        var container = document.Body ?? document.DocumentElement;
        if (container is null)
        {
            return [];
        }

        var writer = new SegmentWriter();
        foreach (var node in container.ChildNodes)
        {
            ProcessNode(node, FormattingContext.Default, writer);
        }

        return writer.Build();
    }

    public static string ToPlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var document = BrowsingContext.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();
        return (document.Body ?? document.DocumentElement)?.TextContent?.Trim() ?? string.Empty;
    }

    private static void ProcessNode(INode node, FormattingContext context, SegmentWriter writer)
    {
        switch (node)
        {
            case IComment:
                return;
            case IText textNode:
                writer.AppendText(NormalizeWhitespace(textNode.Data), context);
                return;
            case IHtmlBreakRowElement:
                writer.AppendLineBreak();
                return;
            case IHtmlImageElement imageElement:
                var imageAlignmentContext = context.WithImageAlignment(ParseAlignment(imageElement));
                writer.AppendImage(
                    imageElement.Source ?? string.Empty,
                    imageAlignmentContext.ImageAlignment,
                    ParseLengthAttribute(imageElement, "width"),
                    ParseLengthAttribute(imageElement, "height"));
                return;
        }

        if (node is not IElement element)
        {
            foreach (var child in node.ChildNodes)
            {
                ProcessNode(child, context, writer);
            }

            return;
        }

        var scopedContext = context.WithElement(element);

        if (element is IHtmlListItemElement)
        {
            writer.AppendText("• ", scopedContext);
        }

        foreach (var child in element.ChildNodes)
        {
            ProcessNode(child, scopedContext, writer);
        }

        if (IsParagraphElement(element))
        {
            writer.AppendParagraphBreak(scopedContext.ParagraphAlignment);
        }
    }

    private static string NormalizeWhitespace(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);
        var previousWasWhitespace = false;

        foreach (var ch in value)
        {
            if (char.IsWhiteSpace(ch))
            {
                if (!previousWasWhitespace)
                {
                    builder.Append(' ');
                    previousWasWhitespace = true;
                }
            }
            else
            {
                builder.Append(ch);
                previousWasWhitespace = false;
            }
        }

        return builder.ToString();
    }

    private static AvaloniaHorizontalAlignment ParseAlignment(IElement element)
    {
        var alignAttribute = element.GetAttribute("align");
        if (!string.IsNullOrWhiteSpace(alignAttribute) && Enum.TryParse(alignAttribute, true, out AvaloniaHorizontalAlignment alignment))
        {
            return alignment;
        }

        foreach (var declaration in ParseStyles(element.GetAttribute("style")))
        {
            if (declaration.Name.Equals("text-align", StringComparison.OrdinalIgnoreCase) &&
                Enum.TryParse(declaration.Value, true, out AvaloniaHorizontalAlignment styleAlignment))
            {
                return styleAlignment;
            }
        }

        return AvaloniaHorizontalAlignment.Left;
    }

    private static double? ParseLengthAttribute(IElement element, string attributeName)
    {
        var attributeValue = element.GetAttribute(attributeName);
        if (string.IsNullOrWhiteSpace(attributeValue))
        {
            return null;
        }

        var trimmed = attributeValue.Trim().ToLowerInvariant();
        if (trimmed.EndsWith("px", StringComparison.Ordinal))
        {
            trimmed = trimmed[..^2];
        }
        else if (trimmed.EndsWith("pt", StringComparison.Ordinal))
        {
            trimmed = trimmed[..^2];
        }

        return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : null;
    }

    private static bool IsParagraphElement(IElement element)
    {
        var tag = element.TagName?.ToUpperInvariant();
        return tag is "P" or "DIV" or "LI" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6";
    }

    private static bool TryParseColor(string? input, out IBrush? brush)
    {
        brush = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var value = input.Trim();
        if (global::Avalonia.Media.Color.TryParse(value, out var color))
        {
            brush = new SolidColorBrush(color);
            return true;
        }

        return false;
    }

    private static IEnumerable<(string Name, string Value)> ParseStyles(string? declaration)
    {
        if (string.IsNullOrWhiteSpace(declaration))
        {
            yield break;
        }

        var parts = declaration.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            var tokens = part.Split(':', 2, StringSplitOptions.TrimEntries);
            if (tokens.Length == 2)
            {
                yield return (tokens[0], tokens[1]);
            }
        }
    }

    private readonly record struct FormattingContext(
        bool Bold,
        bool Italic,
        bool Underline,
        bool Strikethrough,
        IBrush? Foreground,
        IBrush? Background,
        double? FontSize,
        FontFamily? FontFamily,
        AvaloniaHorizontalAlignment ImageAlignment,
        TextAlignment? ParagraphAlignment)
    {
        public static FormattingContext Default => new(false, false, false, false, null, null, null, null, AvaloniaHorizontalAlignment.Left, null);

        public FormattingContext WithElement(IElement element)
        {
            var result = this;
            var tagName = element.TagName?.ToUpperInvariant();

            switch (tagName)
            {
                case "B" or "STRONG":
                    result = result with { Bold = true };
                    break;
                case "I" or "EM":
                    result = result with { Italic = true };
                    break;
                case "U":
                    result = result with { Underline = true };
                    break;
                case "S" or "DEL" or "STRIKE":
                    result = result with { Strikethrough = true };
                    break;
                case "A":
                    result = result with { Underline = true };
                    break;
                case "P" or "DIV":
                    result = result with { ParagraphAlignment = ParseParagraphAlignment(element) };
                    break;
            }

            if (tagName is not null && tagName.StartsWith("H", StringComparison.Ordinal) && tagName.Length == 2 && char.IsDigit(tagName[1]))
            {
                result = result with { Bold = true, FontSize = GetHeadingSize(tagName) };
            }

            foreach (var style in ParseStyles(element.GetAttribute("style")))
            {
                result = style.Name.ToLowerInvariant() switch
                {
                    "font-weight" when style.Value.Equals("bold", StringComparison.OrdinalIgnoreCase) => result with { Bold = true },
                    "font-style" when style.Value.Equals("italic", StringComparison.OrdinalIgnoreCase) => result with { Italic = true },
                    "text-decoration" when style.Value.Contains("underline", StringComparison.OrdinalIgnoreCase) => result with { Underline = true },
                    "text-decoration" when style.Value.Contains("line-through", StringComparison.OrdinalIgnoreCase) => result with { Strikethrough = true },
                    "color" when TryParseColor(style.Value, out var fg) => result with { Foreground = fg },
                    "background-color" when TryParseColor(style.Value, out var bg) => result with { Background = bg },
                    "font-size" => result with { FontSize = ParseFontSize(style.Value, result.FontSize) },
                    "font-family" => result with { FontFamily = new FontFamily(style.Value) },
                    "text-align" when Enum.TryParse<TextAlignment>(style.Value, true, out var align) => result with { ParagraphAlignment = align },
                    _ => result
                };
            }

            return result;
        }

        public FormattingContext WithImageAlignment(AvaloniaHorizontalAlignment alignment) => this with { ImageAlignment = alignment };

        private static TextAlignment? ParseParagraphAlignment(IElement element)
        {
            var alignAttr = element.GetAttribute("align");
            if (!string.IsNullOrWhiteSpace(alignAttr) && Enum.TryParse<TextAlignment>(alignAttr, true, out var fromAttribute))
            {
                return fromAttribute;
            }

            foreach (var declaration in ParseStyles(element.GetAttribute("style")))
            {
                if (declaration.Name.Equals("text-align", StringComparison.OrdinalIgnoreCase) &&
                    Enum.TryParse<TextAlignment>(declaration.Value, true, out var fromStyle))
                {
                    return fromStyle;
                }
            }

            return null;
        }

        private static double? ParseFontSize(string value, double? current)
        {
            var trimmed = value.Trim().ToLowerInvariant();
            if (trimmed.EndsWith("px", StringComparison.Ordinal))
            {
                trimmed = trimmed[..^2];
            }
            else if (trimmed.EndsWith("pt", StringComparison.Ordinal))
            {
                trimmed = trimmed[..^2];
            }
            else if (trimmed.EndsWith("%", StringComparison.Ordinal))
            {
                if (double.TryParse(trimmed[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var pct))
                {
                    var baseSize = current ?? 14d;
                    return baseSize * (pct / 100d);
                }

                return current;
            }

            return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var px) ? px : current;
        }

        private static double? GetHeadingSize(string tag) => tag switch
        {
            "H1" => 32,
            "H2" => 28,
            "H3" => 24,
            "H4" => 20,
            "H5" => 18,
            "H6" => 16,
            _ => null,
        };
    }

    private sealed class SegmentWriter
    {
        private readonly List<TextSegment> _segments = new();
        private int _offset;

        public void AppendText(string? text, FormattingContext context)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var segment = new TextSegment(text, _offset)
            {
                IsBold = context.Bold,
                IsItalic = context.Italic,
                IsUnderline = context.Underline,
                IsStrikethrough = context.Strikethrough,
                Foreground = context.Foreground,
                Background = context.Background,
                FontSize = context.FontSize,
                FontFamily = context.FontFamily,
            };

            _segments.Add(segment);
            _offset += text.Length;
        }

        public void AppendLineBreak()
        {
            _segments.Add(TextSegment.CreateLineBreak(_offset));
            _offset += Environment.NewLine.Length;
        }

        public void AppendParagraphBreak(TextAlignment? alignment) => _segments.Add(TextSegment.CreateParagraphBreak(_offset, alignment));

        public void AppendImage(string source, AvaloniaHorizontalAlignment alignment, double? width, double? height)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return;
            }

            _segments.Add(TextSegment.CreateImage(_offset, source, alignment, width, height));
        }

        public IReadOnlyList<TextSegment> Build() => _segments;
    }
}
