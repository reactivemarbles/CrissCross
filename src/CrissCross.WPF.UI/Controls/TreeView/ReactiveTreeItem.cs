// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CP.Reactive.Collections;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Reactive Tree Item.</summary>
/// <seealso cref="RxObject" />
public abstract partial class ReactiveTreeItem : RxObject
{
    /// <summary>Stores the _parent value.</summary>
    private ReactiveTreeItem? _parent;

    /// <summary>Gets or sets a value indicating whether this instance is expanded.</summary>
    /// <value>
    ///   <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _isExpanded;

    /// <summary>Gets or sets a value indicating whether this instance is selected.</summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _isSelected;

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    [Reactive]
    private IconElement? _icon;

    /// <summary>Initializes a new instance of the <see cref="ReactiveTreeItem"/> class.</summary>
    /// <param name="children">The children.</param>
    protected ReactiveTreeItem(IEnumerable<ReactiveTreeItem>? children = null)
    {
        Children = [];
        if (children is null)
        {
            return;
        }

        Children.Edit(a =>
        {
            foreach (var child in children)
            {
                child._parent = this;
                a.Add(child);
            }
        });
    }

    /// <summary>Gets the view model.</summary>
    /// <value>
    /// The view model.
    /// </value>
    public abstract object ViewModel { get; }

    /// <summary>Gets the children.</summary>
    /// <value>
    /// The children.
    /// </value>
    public ReactiveList<ReactiveTreeItem> Children { get; }

    /// <summary>Adds the child.</summary>
    /// <param name="child">The child.</param>
    public void AddChild(ReactiveTreeItem child)
    {
        if (child is null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        child._parent = this;
        Children.Add(child);
    }

    /// <summary>Removes the selected child and its children.</summary>
    public void RemoveChild() => _parent?.Children.Remove(this);

    /// <summary>Expands the path.</summary>
    public void ExpandPath()
    {
        IsExpanded = true;
        _parent?.ExpandPath();
    }

    /// <summary>Collapses the path.</summary>
    public void CollapsePath()
    {
        IsExpanded = false;
        _parent?.CollapsePath();
    }

    /// <summary>Releases the resources used by the current instance of the class.</summary>
    /// <param name="disposing">A value indicating whether to release managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Children.Dispose();
        }

        base.Dispose(disposing);
    }
}
