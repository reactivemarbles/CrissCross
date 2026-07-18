// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents the GroupBox type.</summary>
/// <remarks>Use a GroupBox to visually group related controls with a header. The header typically describes the
/// contents or purpose of the group. Only one child element can be added to the GroupBox; to contain multiple elements,
/// use a layout panel such as StackPanel or Grid as the child.</remarks>
public class GroupBox : global::Avalonia.Controls.Primitives.HeaderedContentControl;
