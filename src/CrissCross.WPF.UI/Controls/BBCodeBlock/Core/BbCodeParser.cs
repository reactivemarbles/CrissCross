// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode;
#else
namespace CrissCross.WPF.UI.Controls.BBCode;
#endif

/// <summary>Parses BBCode into a nested document tree.</summary>
internal sealed class BbCodeParser
{
    /// <summary>The synthetic root node name.</summary>
    private const string RootTagName = "root";

    /// <summary>Tags whose contents are treated as literal text.</summary>
    private static readonly HashSet<string> RawTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "c",
        "code",
        "nfo",
        "noparse",
        "pre",
    };

    /// <summary>Tags that do not create a nested scope.</summary>
    private static readonly HashSet<string> SelfClosingTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "*",
        "br",
        "hr",
        "line",
    };

    /// <summary>The BBCode source.</summary>
    private readonly string _value;

    /// <summary>Initializes a new instance of the <see cref="BbCodeParser"/> class.</summary>
    /// <param name="value">The BBCode source.</param>
    public BbCodeParser(string? value) => _value = value ?? throw new ArgumentNullException(nameof(value));

    /// <summary>Parses the source into a document root.</summary>
    /// <returns>The document root.</returns>
    public BbCodeNode Parse()
    {
        Dictionary<string, string> noAttributes = [];
        BbCodeNode root = new(RootTagName, null, noAttributes, string.Empty);
        Stack<BbCodeNode> stack = new();
        stack.Push(root);
        StringBuilder text = new();

        for (var index = 0; index < _value.Length;)
        {
            if (_value[index] != '[' || !TryReadTag(_value, index, out var tag, out var nextIndex))
            {
                _ = text.Append(_value[index]);
                index++;
                continue;
            }

            FlushText(stack.Peek(), text);
            if (tag.IsClosing)
            {
                CloseTag(stack, tag);
                index = nextIndex;
                continue;
            }

            BbCodeNode node = new(tag.Name, tag.Value, tag.Attributes, tag.Raw);
            stack.Peek().Children.Add(node);
            index = RawTags.Contains(tag.Name) ? ReadRawNode(_value, node, nextIndex) : nextIndex;
            if (!RawTags.Contains(tag.Name) && !SelfClosingTags.Contains(tag.Name) && !tag.IsSelfClosing)
            {
                stack.Push(node);
            }
        }

        FlushText(stack.Peek(), text);
        return root;
    }

    /// <summary>Closes a matching node while keeping malformed closing tags visible.</summary>
    /// <param name="stack">The open node stack.</param>
    /// <param name="tag">The closing tag.</param>
    private static void CloseTag(Stack<BbCodeNode> stack, ParsedTag tag)
    {
        if (stack.Count <= 1)
        {
            stack.Peek().Children.Add(new BbCodeNode(tag.Raw));
            return;
        }

        var matchingNode = stack.FirstOrDefault(node =>
            string.Equals(node.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
        if (matchingNode is null)
        {
            stack.Peek().Children.Add(new BbCodeNode(tag.Raw));
            return;
        }

        while (stack.Count > 1)
        {
            if (ReferenceEquals(stack.Pop(), matchingNode))
            {
                return;
            }
        }
    }

    /// <summary>Copies buffered plain text into a parent node.</summary>
    /// <param name="parent">The parent node.</param>
    /// <param name="text">The text buffer.</param>
    private static void FlushText(BbCodeNode parent, StringBuilder text)
    {
        if (text.Length == 0)
        {
            return;
        }

        parent.Children.Add(new BbCodeNode(text.ToString()));
        _ = text.Clear();
    }

    /// <summary>Reads the name at the beginning of a tag body.</summary>
    /// <param name="body">The tag body.</param>
    /// <returns>The tag-name length.</returns>
    private static int GetNameLength(string body)
    {
        var length = 0;
        while (length < body.Length && IsNameCharacter(body[length]))
        {
            length++;
        }

        return length;
    }

    /// <summary>Gets a value indicating whether a character can occur in a tag name.</summary>
    /// <param name="value">The character.</param>
    /// <returns><see langword="true"/> when the character is valid.</returns>
    private static bool IsNameCharacter(char value) => char.IsLetterOrDigit(value) || value is '*' or '_';

    /// <summary>Reads named attributes from a tag.</summary>
    /// <param name="source">The attribute source.</param>
    /// <returns>The parsed attributes.</returns>
    private static Dictionary<string, string> ParseAttributes(string source)
    {
        var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var index = 0;
        while (TryReadAttribute(source, ref index, out var name, out var value))
        {
            attributes[name] = value;
        }

        return attributes;
    }

    /// <summary>Reads a raw-code node and advances beyond its closing tag.</summary>
    /// <param name="source">The complete BBCode source.</param>
    /// <param name="node">The raw tag node.</param>
    /// <param name="index">The current source index.</param>
    /// <returns>The index after the raw node.</returns>
    private static int ReadRawNode(string source, BbCodeNode node, int index)
    {
        var closingTag = "[/" + node.Name + "]";
        var closingIndex = source.IndexOf(closingTag, index, StringComparison.OrdinalIgnoreCase);
        var text = closingIndex < 0 ? source[index..] : source[index..closingIndex];
        node.Children.Add(new BbCodeNode(text));
        return closingIndex < 0 ? source.Length : closingIndex + closingTag.Length;
    }

    /// <summary>Reads an attribute name and advances the index.</summary>
    /// <param name="source">The attribute source.</param>
    /// <param name="index">The current index.</param>
    /// <returns>The attribute name.</returns>
    private static string ReadAttributeName(string source, ref int index)
    {
        var start = index;
        while (index < source.Length && (char.IsLetterOrDigit(source[index]) || source[index] is '_' or '-'))
        {
            index++;
        }

        return source[start..index];
    }

    /// <summary>Reads one named attribute.</summary>
    /// <param name="source">The remaining attribute source.</param>
    /// <param name="index">The current index.</param>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The attribute value.</param>
    /// <returns><see langword="true"/> when an attribute was read.</returns>
    private static bool TryReadAttribute(string source, ref int index, out string name, out string value)
    {
        SkipWhitespace(source, ref index);
        name = ReadAttributeName(source, ref index);
        value = string.Empty;
        if (name.Length == 0)
        {
            return false;
        }

        SkipWhitespace(source, ref index);
        if (index >= source.Length || source[index] != '=')
        {
            return false;
        }

        index++;
        SkipWhitespace(source, ref index);
        value = ReadAttributeValue(source, ref index);
        return true;
    }

    /// <summary>Reads a quoted or unquoted attribute value.</summary>
    /// <param name="source">The attribute source.</param>
    /// <param name="index">The current index.</param>
    /// <returns>The attribute value.</returns>
    private static string ReadAttributeValue(string source, ref int index)
    {
        if (index >= source.Length)
        {
            return string.Empty;
        }

        var quote = source[index] is '\'' or '"' ? source[index] : '\0';
        if (quote != '\0')
        {
            index++;
        }

        var start = index;
        index = FindAttributeValueEnd(source, index, quote);

        var value = source[start..index];
        if (index < source.Length && quote != '\0')
        {
            index++;
        }

        return value;
    }

    /// <summary>Finds the end of an attribute value.</summary>
    /// <param name="source">The attribute source.</param>
    /// <param name="index">The first value character.</param>
    /// <param name="quote">The quote delimiter, or a null character.</param>
    /// <returns>The exclusive end position.</returns>
    private static int FindAttributeValueEnd(string source, int index, char quote)
    {
        while (index < source.Length && (quote == '\0' ? !char.IsWhiteSpace(source[index]) : source[index] != quote))
        {
            index++;
        }

        return index;
    }

    /// <summary>Advances past whitespace.</summary>
    /// <param name="source">The source text.</param>
    /// <param name="index">The current index.</param>
    private static void SkipWhitespace(string source, ref int index)
    {
        while (index < source.Length && char.IsWhiteSpace(source[index]))
        {
            index++;
        }
    }

    /// <summary>Reads a tag at a source position.</summary>
    /// <param name="source">The complete source.</param>
    /// <param name="index">The opening-bracket position.</param>
    /// <param name="tag">The parsed tag.</param>
    /// <param name="nextIndex">The first position after the tag.</param>
    /// <returns><see langword="true"/> when a valid tag was found.</returns>
    private static bool TryReadTag(string source, int index, out ParsedTag tag, out int nextIndex)
    {
        tag = default;
        nextIndex = index;
        var closingBracket = source.IndexOf(']', index + 1);
        if (closingBracket < 0)
        {
            return false;
        }

        var raw = source[index..(closingBracket + 1)];
        var body = raw[1..^1].Trim();
        if (body.Length == 0)
        {
            return false;
        }

        var isClosing = body[0] == '/';
        body = isClosing ? body[1..].Trim() : body;
        var isSelfClosing = body.EndsWith("/", StringComparison.Ordinal);
        body = isSelfClosing ? body[..^1].TrimEnd() : body;
        var nameLength = GetNameLength(body);
        if (nameLength == 0)
        {
            return false;
        }

        var name = body[..nameLength].ToLowerInvariant();
        var remainder = body[nameLength..].Trim();
        var value = remainder.StartsWith("=", StringComparison.Ordinal) ? Unquote(remainder[1..].Trim()) : null;
        var attributes = value is null && remainder.Length > 0 ? ParseAttributes(remainder) : [];
        tag = new(name, value, attributes, raw, isClosing, isSelfClosing);
        nextIndex = closingBracket + 1;
        return true;
    }

    /// <summary>Removes matching quote characters around a shorthand value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The unquoted value.</returns>
    private static string Unquote(string value) =>
        value.Length >= 2 && ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\''))
            ? value[1..^1]
            : value;

    /// <summary>Represents a tag header read from the source.</summary>
    private readonly struct ParsedTag
    {
        /// <summary>Initializes a new instance of the <see cref="ParsedTag"/> struct.</summary>
        /// <param name="name">The tag name.</param>
        /// <param name="value">The shorthand value.</param>
        /// <param name="attributes">The named attributes.</param>
        /// <param name="raw">The raw tag.</param>
        /// <param name="isClosing">Whether the tag closes a scope.</param>
        /// <param name="isSelfClosing">Whether the tag is self-closing.</param>
        public ParsedTag(
            string name,
            string? value,
            IDictionary<string, string> attributes,
            string raw,
            bool isClosing,
            bool isSelfClosing)
        {
            Name = name;
            Value = value;
            Attributes = attributes;
            Raw = raw;
            IsClosing = isClosing;
            IsSelfClosing = isSelfClosing;
        }

        /// <summary>Gets the tag name.</summary>
        public string Name { get; }

        /// <summary>Gets the shorthand value.</summary>
        public string? Value { get; }

        /// <summary>Gets the named attributes.</summary>
        public IDictionary<string, string> Attributes { get; }

        /// <summary>Gets the raw tag.</summary>
        public string Raw { get; }

        /// <summary>Gets a value indicating whether the tag closes a scope.</summary>
        public bool IsClosing { get; }

        /// <summary>Gets a value indicating whether the tag is self-closing.</summary>
        public bool IsSelfClosing { get; }
    }
}
