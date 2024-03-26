// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Diagnostics;
using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extends <see cref="System.Windows.Controls.GridViewHeaderRowPresenter"/>, and adds layout support for <see cref="GridViewColumn"/>, which can have <see cref="GridViewColumn.MinWidth"/> and <see cref="GridViewColumn.MaxWidth"/>.
/// </summary>
public class GridViewHeaderRowPresenter : System.Windows.Controls.GridViewHeaderRowPresenter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridViewHeaderRowPresenter"/> class.
    /// </summary>
    public GridViewHeaderRowPresenter() => Loaded += OnLoaded;

    /// <summary>
    /// Arranges the content of the header row elements, and computes the actual size of the header row.
    /// </summary>
    /// <param name="arrangeSize">The area that is available for the column header row.</param>
    /// <returns>
    /// The actual <see cref="T:System.Windows.Size" /> for the column header row.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        // update the desired width of each column (clamps desiredwidth to MinWidth and MaxWidth)
        if (Columns != null)
        {
            foreach (var column in Columns.OfType<GridViewColumn>())
            {
                column.UpdateDesiredWidth();
            }
        }

        return base.ArrangeOverride(arrangeSize);
    }

    /// <summary>
    /// Determines the area that is required to display the column header row.
    /// </summary>
    /// <param name="constraint">The amount of area that is available to display the column header row.</param>
    /// <returns>
    /// The required <see cref="T:System.Windows.Size" /> for the column header row.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        if (Columns != null)
        {
            foreach (var column in Columns.OfType<GridViewColumn>())
            {
                column.UpdateDesiredWidth();
            }
        }

        return base.MeasureOverride(constraint);
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => UpdateIndicatorStyle();

    private void UpdateIndicatorStyle()
    {
        var indicatorField = typeof(System.Windows.Controls.GridViewHeaderRowPresenter).GetField(
            "_indicator",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (indicatorField == null)
        {
            Debug.WriteLine("Failed to get the _indicator field");
            return;
        }

        if (indicatorField.GetValue(this) is Separator indicator)
        {
            indicator.Margin = new Thickness(0);
            indicator.Width = 3.0;

            ResourceDictionary resourceDictionary =
                new()
                {
                    Source = new Uri(
                        "pack://application:,,,/CrissCross.WPF.UI;component/Controls/GridView/GridViewHeaderRowIndicator.xaml",
                        UriKind.Absolute)
                };

            if (resourceDictionary["GridViewHeaderRowIndicatorTemplate"] is ControlTemplate template)
            {
                indicator.Template = template;
            }
            else
            {
                Debug.WriteLine("Failed to get the GridViewHeaderRowIndicatorTemplate");
            }
        }
    }
}
