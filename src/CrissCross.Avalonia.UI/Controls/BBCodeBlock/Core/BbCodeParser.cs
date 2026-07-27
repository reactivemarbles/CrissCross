// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.BBCode;
#else
namespace CrissCross.Avalonia.UI.Controls.BBCode;
#endif

/// <summary>Parses a forgiving BBCode subset into a nested document tree.</summary>
internal sealed class BbCodeParser
{
    /// <summary>The synthetic document root name.</summary>
    private const string RootTagName = "root";

    /// <summary>The source to parse.</summary>
    private readonly string _source;

    /// <summary>Initializes a new instance of the <see cref="BbCodeParser"/> class.</summary>
    /// <param name="source">The BBCode source.</param>
    internal BbCodeParser(string source) => _source = source ?? throw new ArgumentNullException(nameof(source));

    /// <summary>Parses the configured source.</summary>
    /// <returns>A synthetic document root.</returns>
    internal BbCodeNode Parse()
    {
        var root = new BbCodeNode(RootTagName, null, string.Empty);
        var stack = new Stack<BbCodeNode>();
        stack.Push(root);
        var text = new StringBuilder();

        for (var index = 0; index < _source.Length;)
        {
            if (_source[index] != '[' || !TryReadTag(index, out var tag, out var nextIndex))
            {
                _ = text.Append(_source[index]);
                index++;
                continue;
            }

            FlushText(stack.Peek(), text);
            if (tag.IsClosing)
            {
                CloseTag(stack, in tag);
            }
            else
            {
                var node = new BbCodeNode(tag.Name, tag.Value, tag.Raw);
                stack.Peek().Children.Add(node);
                if (!tag.IsSelfClosing && tag.Name != "br")
                {
                    stack.Push(node);
                }
            }

            index = nextIndex;
        }

        FlushText(stack.Peek(), text);
        return root;
    }

    /// <summary>Preserves unmatched closing tags and closes a matching scope.</summary>
    /// <param name="stack">The open tag stack.</param>
    /// <param name="tag">The closing tag.</param>
    private static void CloseTag(Stack<BbCodeNode> stack, in ParsedTag tag)
    {
        BbCodeNode? matchingNode = null;
        var scopesAboveMatch = 0;
        foreach (var node in stack)
        {
            if (string.Equals(node.Name, tag.Name, StringComparison.OrdinalIgnoreCase))
            {
                matchingNode = node;
                break;
            }

            scopesAboveMatch++;
        }

        if (matchingNode is null || scopesAboveMatch == stack.Count - 1)
        {
            stack.Peek().Children.Add(new(tag.Raw));
            return;
        }

        for (var scopesToClose = scopesAboveMatch + 1; scopesToClose > 0; scopesToClose--)
        {
            _ = stack.Pop();
        }
    }

    /// <summary>Adds buffered text to a parent node.</summary>
    /// <param name="parent">The text parent.</param>
    /// <param name="text">The text buffer.</param>
    private static void FlushText(BbCodeNode parent, StringBuilder text)
    {
        if (text.Length <= 0)
        {
            return;
        }

        parent.Children.Add(new(text.ToString()));
        _ = text.Clear();
    }

    /// <summary>Reads a tag at the supplied source offset.</summary>
    /// <param name="index">The opening bracket offset.</param>
    /// <param name="tag">The parsed tag.</param>
    /// <param name="nextIndex">The offset following the tag.</param>
    /// <returns>Whether a valid tag was found.</returns>
    private bool TryReadTag(int index, out ParsedTag tag, out int nextIndex)
    {
        tag = default;
        nextIndex = index;
        var closingBracket = _source.IndexOf(']', index + 1);
        if (closingBracket < 0)
        {
            return false;
        }

        var raw = _source[index..(closingBracket + 1)];
        if (!TagParser.TryParse(raw, out tag))
        {
            return false;
        }

        nextIndex = closingBracket + 1;
        return true;
    }

    /// <summary>Represents a parsed tag header.</summary>
    /// <param name="Name">The normalized tag name.</param>
    /// <param name="Value">The optional shorthand value.</param>
    /// <param name="Raw">The literal tag text.</param>
    /// <param name="IsClosing">Whether the tag closes a scope.</param>
    /// <param name="IsSelfClosing">Whether the tag is self-closing.</param>
    private readonly record struct ParsedTag(string Name, string? Value, string Raw, bool IsClosing, bool IsSelfClosing);

    /// <summary>Parses BBCode tag literals.</summary>
    private static class TagParser
    {
        /// <summary>Parses a BBCode tag literal.</summary>
        /// <param name="raw">The literal tag source.</param>
        /// <param name="tag">The parsed tag.</param>
        /// <returns>Whether the source has a valid tag name.</returns>
        internal static bool TryParse(string raw, out ParsedTag tag)
        {
            tag = default;
            var body = raw[1..^1].Trim();
            if (body.Length == 0)
            {
                return false;
            }

            var isClosing = body[0] == '/';
            body = isClosing ? body[1..].Trim() : body;
            var isSelfClosing = body.EndsWith('/');
            body = isSelfClosing ? body[..^1].TrimEnd() : body;
            var nameLength = 0;
            while (nameLength < body.Length && (char.IsLetterOrDigit(body[nameLength]) || body[nameLength] is '*' or '_'))
            {
                nameLength++;
            }

            if (nameLength == 0)
            {
                return false;
            }

            var name = body[..nameLength].ToLowerInvariant();
            var remainder = body[nameLength..].Trim();
            tag = new(name, remainder.StartsWith('=') ? Unquote(remainder[1..].Trim()) : null, raw, isClosing, isSelfClosing);
            return true;
        }

        /// <summary>Removes matching quote delimiters.</summary>
        /// <param name="value">The source value.</param>
        /// <returns>The unquoted value.</returns>
        private static string Unquote(string value) =>
            value.Length >= 2 && ((value[0] == '\"' && value[^1] == '\"') || (value[0] == '\'' && value[^1] == '\''))
                ? value[1..^1]
                : value;
    }
}
