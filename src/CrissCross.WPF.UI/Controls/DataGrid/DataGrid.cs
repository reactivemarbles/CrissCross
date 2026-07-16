// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides the DataGrid member.</summary>
public class DataGrid : System.Windows.Controls.DataGrid
{
    /// <summary>The DependencyProperty that represents the <see cref="CheckBoxColumnElementStyle"/> property.</summary>
    public static readonly DependencyProperty CheckBoxColumnElementStyleProperty = DependencyProperty.Register(
        nameof(CheckBoxColumnElementStyle),
        typeof(Style),
        typeof(DataGrid),
        new FrameworkPropertyMetadata(null));

    /// <summary>The DependencyProperty that represents the CheckBoxColumnEditingElementStyle property.</summary>
    public static readonly DependencyProperty CheckBoxColumnEditingElementStyleProperty = DependencyProperty.Register(
        nameof(CheckBoxColumnEditingElementStyle),
        typeof(Style),
        typeof(DataGrid),
        new FrameworkPropertyMetadata(null));

    /// <summary>Gets or sets a style to apply to all checkbox column in the DataGrid.</summary>
    public Style CheckBoxColumnElementStyle
    {
        get => (Style)GetValue(CheckBoxColumnElementStyleProperty);
        set => SetValue(CheckBoxColumnElementStyleProperty, value);
    }

    /// <summary>Gets or sets a style to apply to all checkbox column in the DataGrid.</summary>
    public Style CheckBoxColumnEditingElementStyle
    {
        get => (Style)GetValue(CheckBoxColumnEditingElementStyleProperty);
        set => SetValue(CheckBoxColumnEditingElementStyleProperty, value);
    }

    /// <summary>Raises the <see cref="E:System.Windows.FrameworkElement.Initialized" /> event. This method is invoked
    /// whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized" /> is set to true internally.</summary>
    /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
        Columns.CollectionChanged += ColumnsOnCollectionChanged;

        UpdateColumnElementStyles();

        base.OnInitialized(e);
    }

    /// <summary>Provides the ColumnsOnCollectionChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void ColumnsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        UpdateColumnElementStyles();

    /// <summary>Provides the UpdateColumnElementStyles member.</summary>
    private void UpdateColumnElementStyles()
    {
        foreach (var singleColumn in Columns)
        {
            UpdateSingleColumn(singleColumn);
        }
    }

    /// <summary>Provides the UpdateSingleColumn member.</summary>
    /// <param name="dataGridColumn">The dataGridColumn value.</param>
    private void UpdateSingleColumn(DataGridColumn dataGridColumn)
    {
        if (dataGridColumn is not DataGridCheckBoxColumn checkBoxColumn)
        {
            return;
        }

        if (checkBoxColumn.ReadLocalValue(DataGridBoundColumn.ElementStyleProperty) == DependencyProperty.UnsetValue)
        {
            _ = BindingOperations.SetBinding(
                checkBoxColumn,
                DataGridBoundColumn.ElementStyleProperty,
                new Binding { Path = new(CheckBoxColumnElementStyleProperty), Source = this });
        }

        if (
            checkBoxColumn.ReadLocalValue(DataGridBoundColumn.EditingElementStyleProperty)
            != DependencyProperty.UnsetValue)
        {
            return;
        }

        _ = BindingOperations.SetBinding(
            checkBoxColumn,
            DataGridBoundColumn.EditingElementStyleProperty,
            new Binding { Path = new(CheckBoxColumnEditingElementStyleProperty), Source = this });
    }
}
