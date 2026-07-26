// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode;
#else
namespace CrissCross.WPF.UI.Controls.BBCode;
#endif

/// <summary>One text or tag node in a parsed BBCode document.</summary>
internal sealed class BbCodeNode
{
    /// <summary>Initializes a new instance of the <see cref="BbCodeNode"/> class as a text node.</summary>
    /// <param name="text">The text content.</param>
    internal BbCodeNode(string text)
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
    internal BbCodeNode(string name, string? value, IDictionary<string, string> attributes, string rawOpeningTag)
    {
        Name = name;
        Value = value;
        RawOpeningTag = rawOpeningTag;
        Attributes = new Dictionary<string, string>(attributes, StringComparer.OrdinalIgnoreCase);
        Children = [];
    }

    /// <summary>Gets the normalized tag name, or an empty string for text.</summary>
    internal string Name { get; }

    /// <summary>Gets the text content.</summary>
    internal string? Text { get; }

    /// <summary>Gets the shorthand attribute value.</summary>
    internal string? Value { get; }

    /// <summary>Gets the named attributes.</summary>
    internal IDictionary<string, string> Attributes { get; }

    /// <summary>Gets the child nodes.</summary>
    internal IList<BbCodeNode> Children { get; }

    /// <summary>Gets the opening tag as written.</summary>
    internal string RawOpeningTag { get; }

    /// <summary>Gets a value indicating whether this is a text node.</summary>
    internal bool IsText => Name.Length == 0;

    /// <summary>Returns all descendant text without formatting.</summary>
    /// <returns>The concatenated text.</returns>
    internal string GetText()
    {
        if (IsText)
        {
            return Text ?? string.Empty;
        }

        StringBuilder text = new();
        foreach (var child in Children)
        {
            _ = text.Append(child.GetText());
        }

        return text.ToString();
    }
}
