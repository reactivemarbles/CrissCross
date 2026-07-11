// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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

/// <summary>Provides the HtmlContentParser member.</summary>
internal static class HtmlContentParser
{
    /// <summary>Provides the UnitSuffixLength member.</summary>
    private const int UnitSuffixLength = 2;

    /// <summary>Provides the HeadingTagLength member.</summary>
    private const int HeadingTagLength = 2;

    /// <summary>Provides the StyleDeclarationTokenCount member.</summary>
    private const int StyleDeclarationTokenCount = 2;

    /// <summary>Provides the DefaultFontSize member.</summary>
    private const double DefaultFontSize = 14d;

    /// <summary>Provides the PercentDivisor member.</summary>
    private const double PercentDivisor = 100d;

    /// <summary>Provides the HeadingOneFontSize member.</summary>
    private const double HeadingOneFontSize = 32d;

    /// <summary>Provides the HeadingTwoFontSize member.</summary>
    private const double HeadingTwoFontSize = 28d;

    /// <summary>Provides the HeadingThreeFontSize member.</summary>
    private const double HeadingThreeFontSize = 24d;

    /// <summary>Provides the HeadingFourFontSize member.</summary>
    private const double HeadingFourFontSize = 20d;

    /// <summary>Provides the HeadingFiveFontSize member.</summary>
    private const double HeadingFiveFontSize = 18d;

    /// <summary>Provides the HeadingSixFontSize member.</summary>
    private const double HeadingSixFontSize = 16d;

    /// <summary>Provides the BrowsingContext member.</summary>
    private static readonly IBrowsingContext BrowsingContext = AngleSharp.BrowsingContext.New(Configuration.Default);

    /// <summary>Provides the Parse member.</summary>
    /// <param name="html">The html value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the ToPlainText member.</summary>
    /// <param name="html">The html value.</param>
    /// <returns>The result.</returns>
    public static string ToPlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var document = BrowsingContext.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();
        return (document.Body ?? document.DocumentElement)?.TextContent?.Trim() ?? string.Empty;
    }

    /// <summary>Provides the ProcessNode member.</summary>
    /// <param name="node">The node value.</param>
    /// <param name="context">The context value.</param>
    /// <param name="writer">The writer value.</param>
    private static void ProcessNode(INode node, FormattingContext context, SegmentWriter writer)
    {
        switch (node)
        {
            case IComment:
                return;
            case IText textNode:
                {
                    writer.AppendText(NormalizeWhitespace(textNode.Data), context);
                    return;
                }

            case IHtmlBreakRowElement:
                {
                    writer.AppendLineBreak();
                    return;
                }

            case IHtmlImageElement imageElement:
                {
                    var imageAlignmentContext = context.WithImageAlignment(ParseAlignment(imageElement));
                    var imageSource = imageElement.GetAttribute("src") ?? imageElement.Source ?? string.Empty;
                    writer.AppendImage(
                        imageSource,
                        imageAlignmentContext.ImageAlignment,
                        ParseLengthAttribute(imageElement, "width"),
                        ParseLengthAttribute(imageElement, "height"));
                    return;
                }
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
            writer.AppendText("� ", scopedContext);
        }

        foreach (var child in element.ChildNodes)
        {
            ProcessNode(child, scopedContext, writer);
        }

        if (!IsParagraphElement(element))
        {
            return;
        }

        writer.AppendParagraphBreak(scopedContext.ParagraphAlignment);
    }

    /// <summary>Provides the NormalizeWhitespace member.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
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
                    _ = builder.Append(' ');
                    previousWasWhitespace = true;
                }
            }
            else
            {
                _ = builder.Append(ch);
                previousWasWhitespace = false;
            }
        }

        return builder.ToString();
    }

    /// <summary>Provides the ParseAlignment member.</summary>
    /// <param name="element">The element value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the ParseLengthAttribute member.</summary>
    /// <param name="element">The element value.</param>
    /// <param name="attributeName">The attributeName value.</param>
    /// <returns>The result.</returns>
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
            trimmed = trimmed[..^UnitSuffixLength];
        }
        else if (trimmed.EndsWith("pt", StringComparison.Ordinal))
        {
            trimmed = trimmed[..^UnitSuffixLength];
        }

        return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : null;
    }

    /// <summary>Provides the IsParagraphElement member.</summary>
    /// <param name="element">The element value.</param>
    /// <returns>The result.</returns>
    private static bool IsParagraphElement(IElement element)
    {
        var tag = element.TagName?.ToUpperInvariant();
        return tag is "P" or "DIV" or "LI" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6";
    }

    /// <summary>Provides the TryParseColor member.</summary>
    /// <param name="input">The input value.</param>
    /// <param name="brush">The brush value.</param>
    /// <returns>The result.</returns>
    private static bool TryParseColor(string? input, out IBrush? brush)
    {
        brush = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var value = input.Trim();
        if (!global::Avalonia.Media.Color.TryParse(value, out var color))
        {
            return false;
        }

        brush = new SolidColorBrush(color);
        return true;
    }

    /// <summary>Provides the ParseStyles member.</summary>
    /// <param name="declaration">The declaration value.</param>
    /// <returns>The result.</returns>
    private static IEnumerable<(string Name, string Value)> ParseStyles(string? declaration)
    {
        if (string.IsNullOrWhiteSpace(declaration))
        {
            yield break;
        }

        foreach (var part in declaration.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var tokens = part.Split(':', StyleDeclarationTokenCount, StringSplitOptions.TrimEntries);
            if (tokens.Length == StyleDeclarationTokenCount)
            {
                yield return (tokens[0], tokens[1]);
            }
        }
    }

    /// <summary>Gets the value.</summary>
    /// <param name="Bold">The Bold value.</param>
    /// <param name="Italic">The Italic value.</param>
    /// <param name="Underline">The Underline value.</param>
    /// <param name="Strikethrough">The Strikethrough value.</param>
    /// <param name="Foreground">The foreground brush.</param>
    /// <param name="Background">The background brush.</param>
    /// <param name="FontSize">The font size.</param>
    /// <param name="FontFamily">The font family.</param>
    /// <param name="ImageAlignment">The image alignment.</param>
    /// <param name="ParagraphAlignment">The paragraph alignment.</param>
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
        /// <summary>Gets the default formatting context.</summary>
        public static FormattingContext Default => new(false, false, false, false, null, null, null, null, AvaloniaHorizontalAlignment.Left, null);

        /// <summary>Provides the WithElement member.</summary>
        /// <param name="element">The element value.</param>
        /// <returns>The result.</returns>
        public FormattingContext WithElement(IElement element)
        {
            var tagName = element.TagName?.ToUpperInvariant();
            var result = ApplyTag(this, element, tagName);
            result = ApplyHeading(result, tagName);

            foreach (var style in ParseStyles(element.GetAttribute("style")))
            {
                result = ApplyStyle(result, style);
            }

            return result;
        }

        /// <summary>Provides the WithImageAlignment member.</summary>
        /// <param name="alignment">The alignment value.</param>
        /// <returns>The result.</returns>
        public FormattingContext WithImageAlignment(AvaloniaHorizontalAlignment alignment) => this with { ImageAlignment = alignment };

        /// <summary>Applies formatting implied by an element tag.</summary>
        /// <param name="context">The current context.</param>
        /// <param name="element">The element.</param>
        /// <param name="tagName">The normalized tag name.</param>
        /// <returns>The updated context.</returns>
        private static FormattingContext ApplyTag(FormattingContext context, IElement element, string? tagName) => tagName switch
        {
            "B" or "STRONG" => context with { Bold = true },
            "I" or "EM" => context with { Italic = true },
            "U" => context with { Underline = true },
            "S" or "DEL" or "STRIKE" => context with { Strikethrough = true },
            "A" => context with { Underline = true },
            "P" or "DIV" => context with { ParagraphAlignment = ParseParagraphAlignment(element) },
            _ => context
        };

        /// <summary>Applies heading formatting.</summary>
        /// <param name="context">The current context.</param>
        /// <param name="tagName">The normalized tag name.</param>
        /// <returns>The updated context.</returns>
        private static FormattingContext ApplyHeading(FormattingContext context, string? tagName)
        {
            return IsHeadingTag(tagName) ? context with { Bold = true, FontSize = GetHeadingSize(tagName!) } : context;
        }

        /// <summary>Determines whether a tag is a heading tag.</summary>
        /// <param name="tagName">The normalized tag name.</param>
        /// <returns><see langword="true"/> when the tag is a heading.</returns>
        private static bool IsHeadingTag(string? tagName) =>
            tagName?.StartsWith("H", StringComparison.Ordinal) == true &&
            tagName.Length == HeadingTagLength &&
            char.IsDigit(tagName[1]);

        /// <summary>Applies one style declaration.</summary>
        /// <param name="context">The current context.</param>
        /// <param name="style">The style declaration.</param>
        /// <returns>The updated context.</returns>
        private static FormattingContext ApplyStyle(FormattingContext context, (string Name, string Value) style) => style.Name.ToLowerInvariant() switch
        {
            "font-weight" when style.Value.Equals("bold", StringComparison.OrdinalIgnoreCase) => context with { Bold = true },
            "font-style" when style.Value.Equals("italic", StringComparison.OrdinalIgnoreCase) => context with { Italic = true },
            "text-decoration" when style.Value.Contains("underline", StringComparison.OrdinalIgnoreCase) => context with { Underline = true },
            "text-decoration" when style.Value.Contains("line-through", StringComparison.OrdinalIgnoreCase) => context with { Strikethrough = true },
            "color" when TryParseColor(style.Value, out var foreground) => context with { Foreground = foreground },
            "background-color" when TryParseColor(style.Value, out var background) => context with { Background = background },
            "font-size" => context with { FontSize = ParseFontSize(style.Value, context.FontSize) },
            "font-family" => context with { FontFamily = new(style.Value) },
            "text-align" when Enum.TryParse<TextAlignment>(style.Value, true, out var alignment) => context with { ParagraphAlignment = alignment },
            _ => context
        };

        /// <summary>Provides the ParseParagraphAlignment member.</summary>
        /// <param name="element">The element value.</param>
        /// <returns>The result.</returns>
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

        /// <summary>Provides the ParseFontSize member.</summary>
        /// <param name="value">The value.</param>
        /// <param name="current">The current value.</param>
        /// <returns>The result.</returns>
        private static double? ParseFontSize(string value, double? current)
        {
            var trimmed = value.Trim().ToLowerInvariant();
            if (trimmed.EndsWith("px", StringComparison.Ordinal))
            {
                trimmed = trimmed[..^UnitSuffixLength];
            }
            else if (trimmed.EndsWith("pt", StringComparison.Ordinal))
            {
                trimmed = trimmed[..^UnitSuffixLength];
            }
            else if (trimmed.EndsWith("%", StringComparison.Ordinal))
            {
                if (double.TryParse(trimmed[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var pct))
                {
                    var baseSize = current ?? DefaultFontSize;
                    return baseSize * (pct / PercentDivisor);
                }

                return current;
            }

            return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var px) ? px : current;
        }

        /// <summary>Provides the GetHeadingSize member.</summary>
        /// <param name="tag">The tag value.</param>
        /// <returns>The result.</returns>
        private static double? GetHeadingSize(string tag) => tag switch
        {
            "H1" => HeadingOneFontSize,
            "H2" => HeadingTwoFontSize,
            "H3" => HeadingThreeFontSize,
            "H4" => HeadingFourFontSize,
            "H5" => HeadingFiveFontSize,
            "H6" => HeadingSixFontSize,
            _ => null,
        };
    }

    /// <summary>Provides the SegmentWriter member.</summary>
    private sealed class SegmentWriter
    {
        /// <summary>Provides the _segments member.</summary>
        private readonly List<TextSegment> _segments = new();

        /// <summary>Provides the _offset member.</summary>
        private int _offset;

        /// <summary>Provides the AppendText member.</summary>
        /// <param name="text">The text value.</param>
        /// <param name="context">The context value.</param>
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

        /// <summary>Provides the AppendLineBreak member.</summary>
        public void AppendLineBreak()
        {
            _segments.Add(TextSegment.CreateLineBreak(_offset));
            _offset += Environment.NewLine.Length;
        }

        /// <summary>Provides the AppendParagraphBreak member.</summary>
        /// <param name="alignment">The alignment value.</param>
        public void AppendParagraphBreak(TextAlignment? alignment) => _segments.Add(TextSegment.CreateParagraphBreak(_offset, alignment));

        /// <summary>Provides the AppendImage member.</summary>
        /// <param name="source">The source value.</param>
        /// <param name="alignment">The alignment value.</param>
        /// <param name="width">The width value.</param>
        /// <param name="height">The height value.</param>
        public void AppendImage(string source, AvaloniaHorizontalAlignment alignment, double? width, double? height)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return;
            }

            _segments.Add(TextSegment.CreateImage(_offset, source, alignment, width, height));
            _offset++;
        }

        /// <summary>Provides the Build member.</summary>
        /// <returns>The result.</returns>
        public List<TextSegment> Build() => _segments;
    }
}
