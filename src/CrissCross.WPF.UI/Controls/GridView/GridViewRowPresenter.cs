// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the GridViewRowPresenter member.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class GridViewRowPresenter : System.Windows.Controls.GridViewRowPresenter
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

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
