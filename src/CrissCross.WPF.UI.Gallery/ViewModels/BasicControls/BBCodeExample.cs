// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>Describes one BBCode syntax and rendering example.</summary>
public sealed class BBCodeExample
{
    /// <summary>Initializes a new instance of the <see cref="BBCodeExample"/> class.</summary>
    /// <param name="title">The example title.</param>
    /// <param name="description">The example description.</param>
    /// <param name="markup">The BBCode source.</param>
    public BBCodeExample(string title, string description, string markup)
    {
        Title = title;
        Description = description;
        Markup = markup;
    }

    /// <summary>Gets the example title.</summary>
    public string Title { get; }

    /// <summary>Gets the example description.</summary>
    public string Description { get; }

    /// <summary>Gets the BBCode source.</summary>
    public string Markup { get; }
}
