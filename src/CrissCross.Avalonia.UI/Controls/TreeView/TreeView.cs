// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a control that displays a hierarchical collection of items in a tree structure.</summary>
/// <remarks>Use the TreeView control to present data that has a parent-child relationship, such as file systems
/// or organizational charts. Items can be expanded or collapsed to show or hide their child elements. This control
/// supports data binding and selection of items.</remarks>
public class TreeView : global::Avalonia.Controls.TreeView;
