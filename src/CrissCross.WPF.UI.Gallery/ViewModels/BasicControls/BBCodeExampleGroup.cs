// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>Groups related BBCode examples.</summary>
public sealed class BBCodeExampleGroup
{
    /// <summary>Initializes a new instance of the <see cref="BBCodeExampleGroup"/> class.</summary>
    /// <param name="title">The group title.</param>
    /// <param name="examples">The examples in the group.</param>
    public BBCodeExampleGroup(string title, IReadOnlyList<BBCodeExample> examples)
    {
        Title = title;
        Examples = examples;
    }

    /// <summary>Gets the group title.</summary>
    public string Title { get; }

    /// <summary>Gets the examples in the group.</summary>
    public IReadOnlyList<BBCodeExample> Examples { get; }
}
