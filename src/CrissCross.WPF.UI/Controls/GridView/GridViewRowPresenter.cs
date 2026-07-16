// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides the GridViewRowPresenter member.</summary>
public class GridViewRowPresenter : System.Windows.Controls.GridViewRowPresenter
{
    /// <summary>Positions the content of a row according to the size of the corresponding <see
    /// cref="T:System.Windows.Controls.GridViewColumn" /> objects.</summary>
    /// <param name="arrangeSize">The area to use to display the <see
    /// cref="P:System.Windows.Controls.GridViewRowPresenter.Content" />.</param>
    /// <returns>
    /// The actual <see cref="T:System.Windows.Size" /> that is used to display the <see
    /// cref="P:System.Windows.Controls.GridViewRowPresenter.Content" />.
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

    /// <summary>Determines the area that is required to display the row.</summary>
    /// <param name="constraint">The maximum area to use to display the <see
    /// cref="P:System.Windows.Controls.GridViewRowPresenter.Content" />.</param>
    /// <returns>
    /// The actual <see cref="T:System.Windows.Size" /> of the area that displays the <see
    /// cref="P:System.Windows.Controls.GridViewRowPresenter.Content" />.
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
}
