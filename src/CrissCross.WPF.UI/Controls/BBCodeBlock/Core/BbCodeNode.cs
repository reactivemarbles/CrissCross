// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.BBCode;

/// <summary>One text or tag node in a parsed BBCode document.</summary>
internal sealed class BbCodeNode
{
    /// <summary>Initializes a new instance of the <see cref="BbCodeNode"/> class as a text node.</summary>
    /// <param name="text">The text content.</param>
    public BbCodeNode(string text)
    {
        Text = text;
        Name = string.Empty;
        RawOpeningTag = string.Empty;
        Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Children = [];
    }

    /// <summary>Initializes a new instance of the <see cref="BbCodeNode"/> class as a tag node.</summary>
    /// <param name="name">The normalized tag name.</param>
    /// <param name="value">The shorthand attribute value.</param>
    /// <param name="attributes">The named attributes.</param>
    /// <param name="rawOpeningTag">The opening tag as written.</param>
    public BbCodeNode(string name, string? value, IDictionary<string, string> attributes, string rawOpeningTag)
    {
        Name = name;
        Value = value;
        RawOpeningTag = rawOpeningTag;
        Attributes = new Dictionary<string, string>(attributes, StringComparer.OrdinalIgnoreCase);
        Children = [];
    }

    /// <summary>Gets the normalized tag name, or an empty string for text.</summary>
    public string Name { get; }

    /// <summary>Gets the text content.</summary>
    public string? Text { get; }

    /// <summary>Gets the shorthand attribute value.</summary>
    public string? Value { get; }

    /// <summary>Gets the named attributes.</summary>
    public IDictionary<string, string> Attributes { get; }

    /// <summary>Gets the child nodes.</summary>
    public IList<BbCodeNode> Children { get; }

    /// <summary>Gets the opening tag as written.</summary>
    public string RawOpeningTag { get; }

    /// <summary>Gets a value indicating whether this is a text node.</summary>
    public bool IsText => Name.Length == 0;

    /// <summary>Returns all descendant text without formatting.</summary>
    /// <returns>The concatenated text.</returns>
    public string GetText() => IsText
        ? Text ?? string.Empty
        : string.Concat(Children.Select(static child => child.GetText()));
}
