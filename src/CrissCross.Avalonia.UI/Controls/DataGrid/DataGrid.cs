// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A DataGrid control that displays data in rows and columns and allows
/// for the entering and editing of data.
/// </summary>
/// <remarks>
/// This control requires the Avalonia.Controls.DataGrid NuGet package to be installed.
/// Install it separately: dotnet add package Avalonia.Controls.DataGrid.
/// </remarks>
public class DataGrid : ItemsControl
{
    /// <summary>
    /// Property for <see cref="CheckBoxColumnElementStyle"/>.
    /// </summary>
    public static readonly StyledProperty<Style?> CheckBoxColumnElementStyleProperty =
        AvaloniaProperty.Register<DataGrid, Style?>(
            nameof(CheckBoxColumnElementStyle), null);

    /// <summary>
    /// Property for <see cref="CheckBoxColumnEditingElementStyle"/>.
    /// </summary>
    public static readonly StyledProperty<Style?> CheckBoxColumnEditingElementStyleProperty =
        AvaloniaProperty.Register<DataGrid, Style?>(
            nameof(CheckBoxColumnEditingElementStyle), null);

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGrid"/> class.
    /// </summary>
    public DataGrid()
    {
        // Additional initialization can be added here
    }

    /// <summary>
    /// Gets or sets a style to apply to all checkbox columns in the DataGrid.
    /// </summary>
    public Style? CheckBoxColumnElementStyle
    {
        get => GetValue(CheckBoxColumnElementStyleProperty);
        set => SetValue(CheckBoxColumnElementStyleProperty, value);
    }

    /// <summary>
    /// Gets or sets a style to apply to all checkbox columns in editing mode in the DataGrid.
    /// </summary>
    public Style? CheckBoxColumnEditingElementStyle
    {
        get => GetValue(CheckBoxColumnEditingElementStyleProperty);
        set => SetValue(CheckBoxColumnEditingElementStyleProperty, value);
    }
}
