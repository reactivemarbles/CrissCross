// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides the GridViewHeaderRowPresenter member.</summary>
public class GridViewHeaderRowPresenter : System.Windows.Controls.GridViewHeaderRowPresenter
{
    /// <summary>The width of the column resize indicator.</summary>
    private const double IndicatorWidth = 3D;

    /// <summary>Initializes a new instance of the <see cref="GridViewHeaderRowPresenter"/> class.</summary>
    public GridViewHeaderRowPresenter() => Loaded += OnLoaded;

    /// <summary>Provides the ArrangeOverride member.</summary>
    /// <param name="arrangeSize">The area that is available for the column header row.</param>
    /// <returns>
    /// The actual <see cref="T:System.Windows.Size" /> for the column header row.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        // update the desired width of each column (clamps desiredwidth to MinWidth and MaxWidth)
        if (Columns is not null)
        {
            foreach (var column in Columns.OfType<GridViewColumn>())
            {
                column.UpdateDesiredWidth();
            }
        }

        return base.ArrangeOverride(arrangeSize);
    }

    /// <summary>Determines the area that is required to display the column header row.</summary>
    /// <param name="constraint">The amount of area that is available to display the column header row.</param>
    /// <returns>
    /// The required <see cref="T:System.Windows.Size" /> for the column header row.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        if (Columns is not null)
        {
            foreach (var column in Columns.OfType<GridViewColumn>())
            {
                column.UpdateDesiredWidth();
            }
        }

        return base.MeasureOverride(constraint);
    }

    /// <summary>Provides the OnLoaded member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnLoaded(object sender, RoutedEventArgs e) => UpdateIndicatorStyle();

    /// <summary>Provides the UpdateIndicatorStyle member.</summary>
    private void UpdateIndicatorStyle()
    {
        var indicatorField = typeof(System.Windows.Controls.GridViewHeaderRowPresenter).GetField(
            "_indicator",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (indicatorField is null)
        {
            Debug.WriteLine("Failed to get the _indicator field");
            return;
        }

        if (indicatorField.GetValue(this) is not Separator indicator)
        {
            return;
        }

        indicator.Margin = new(0);
        indicator.Width = IndicatorWidth;

        ResourceDictionary resourceDictionary = new()
        {
            Source = new(
                "pack://application:,,,/CrissCross.WPF.UI;component/Controls/GridView/GridViewHeaderRowIndicator.xaml",
                UriKind.Absolute),
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
