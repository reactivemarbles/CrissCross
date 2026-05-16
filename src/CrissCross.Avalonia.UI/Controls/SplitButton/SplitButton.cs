// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a button with two parts that can be invoked separately.
/// </summary>
public class SplitButton : global::Avalonia.Controls.SplitButton
{
    /// <inheritdoc/>
    protected override Type StyleKeyOverride => typeof(global::Avalonia.Controls.SplitButton);
}
