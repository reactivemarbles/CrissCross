// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.BBCode;
#else
namespace CrissCross.Avalonia.UI.Controls.BBCode;
#endif

/// <summary>Represents one text or BBCode tag node.</summary>
internal sealed class BbCodeNode
{
    /// <summary>Initializes a new instance of the <see cref="BbCodeNode"/> class for text.</summary>
    /// <param name="text">The literal text.</param>
    internal BbCodeNode(string text)
    {
        Text = text;
        Name = string.Empty;
        RawOpeningTag = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="BbCodeNode"/> class for a tag.</summary>
    /// <param name="name">The normalized tag name.</param>
    /// <param name="value">The optional shorthand value.</param>
    /// <param name="rawOpeningTag">The opening tag as supplied.</param>
    internal BbCodeNode(string name, string? value, string rawOpeningTag)
    {
        Name = name;
        Value = value;
        RawOpeningTag = rawOpeningTag;
    }

    /// <summary>Gets the child nodes.</summary>
    internal IList<BbCodeNode> Children { get; } = [];

    /// <summary>Gets the normalized tag name, or an empty value for text.</summary>
    internal string Name { get; }

    /// <summary>Gets the opening tag as supplied.</summary>
    internal string RawOpeningTag { get; }

    /// <summary>Gets the literal text.</summary>
    internal string? Text { get; }

    /// <summary>Gets the optional shorthand value.</summary>
    internal string? Value { get; }

    /// <summary>Gets a value indicating whether this node is literal text.</summary>
    internal bool IsText => Name.Length == 0;

    /// <summary>Gets the unformatted descendant text.</summary>
    /// <returns>The concatenated text.</returns>
    internal string GetText()
    {
        if (IsText)
        {
            return Text ?? string.Empty;
        }

        var text = new StringBuilder();
        foreach (var child in Children)
        {
            _ = text.Append(child.GetText());
        }

        return text.ToString();
    }
}
