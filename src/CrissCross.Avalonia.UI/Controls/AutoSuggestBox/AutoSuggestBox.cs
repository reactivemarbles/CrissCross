// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a text control that makes suggestions to users as they enter text.</summary>
public class AutoSuggestBox : global::Avalonia.Controls.AutoCompleteBox
{
    /// <inheritdoc/>
    protected override Type StyleKeyOverride => typeof(global::Avalonia.Controls.AutoCompleteBox);
}
