// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Layout;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a numeric keypad control for number input.
/// </summary>
public class NumberPad : Grid
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumberPad"/> class.
    /// </summary>
    public NumberPad()
    {
        // 4x3 grid for standard number pad layout
        RowDefinitions = new RowDefinitions("*,*,*,*");
        ColumnDefinitions = new ColumnDefinitions("*,*,*");
    }
}
