// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a button with a drop-down list.</summary>
public class DropDownButton : global::Avalonia.Controls.DropDownButton
{
    /// <inheritdoc/>
    protected override Type StyleKeyOverride => typeof(global::Avalonia.Controls.DropDownButton);
}
