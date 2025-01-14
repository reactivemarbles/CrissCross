// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CP.Reactive;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Reactive Tree Item.
/// </summary>
/// <seealso cref="RxObject" />
public abstract class ReactiveTreeItem : RxObject
{
    private ReactiveTreeItem? _parent;
    private bool _isExpanded;
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTreeItem"/> class.
    /// </summary>
    /// <param name="children">The children.</param>
    protected ReactiveTreeItem(IEnumerable<ReactiveTreeItem>? children = null)
    {
        Children = new();
        if (children == null)
        {
            return;
        }

        foreach (var child in children)
        {
            AddChild(child);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is expanded.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
    /// </value>
    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    /// <summary>
    /// Gets the view model.
    /// </summary>
    /// <value>
    /// The view model.
    /// </value>
    public abstract object ViewModel { get; }

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>
    /// The children.
    /// </value>
    public ReactiveList<ReactiveTreeItem> Children { get; }

    /// <summary>
    /// Adds the child.
    /// </summary>
    /// <param name="child">The child.</param>
    public void AddChild(ReactiveTreeItem child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        child._parent = this;
        Children.Add(child);
    }

    /// <summary>
    /// Removes the selected child and its children.
    /// </summary>
    public void RemoveChild() => _parent?.Children.Remove(this);

    /// <summary>
    /// Expands the path.
    /// </summary>
    public void ExpandPath()
    {
        IsExpanded = true;
        _parent?.ExpandPath();
    }

    /// <summary>
    /// Collapses the path.
    /// </summary>
    public void CollapsePath()
    {
        IsExpanded = false;
        _parent?.CollapsePath();
    }
}
