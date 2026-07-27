// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the GridViewHeaderRowPresenter member.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class GridViewHeaderRowPresenter : System.Windows.Controls.GridViewHeaderRowPresenter
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

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
            foreach (var candidate in Columns)
            {
                if (candidate is GridViewColumn column)
                {
                    column.UpdateDesiredWidth();
                }
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
            foreach (var candidate in Columns)
            {
                if (candidate is GridViewColumn column)
                {
                    column.UpdateDesiredWidth();
                }
            }
        }

        return base.MeasureOverride(constraint);
    }
}
