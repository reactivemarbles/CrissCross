// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extended <see cref="System.Windows.Controls.TreeView"/>.
/// </summary>
/// <seealso cref="System.Windows.Controls.TreeView" />
public class TreeView : System.Windows.Controls.TreeView
{
    /// <summary>
    /// Identifies the SelectedItem dependency property for the TreeView control.
    /// </summary>
    /// <remarks>This field is used when referencing the SelectedItem property in property system operations,
    /// such as data binding or property metadata registration.</remarks>
    public static new readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(ReactiveTreeItem), typeof(TreeView), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the currently selected item in the tree.
    /// </summary>
    /// <remarks>If no item is selected, the property value is <see langword="null"/>. Changing this property
    /// updates the selection state of the tree and may trigger related events or bindings.</remarks>
    public new ReactiveTreeItem? SelectedItem
    {
        get => (ReactiveTreeItem?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
}
