// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Check Box Result.</summary>
public class CheckBoxResultEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="CheckBoxResultEventArgs"/> class.</summary>
    public CheckBoxResultEventArgs() { }

    /// <summary>Initializes a new instance of the <see cref="CheckBoxResultEventArgs"/> class.</summary>
    /// <param name="userClicked">if set to <c>true</c> [user clicked].</param>
    /// <param name="result">if set to <c>true</c> [result].</param>
    public CheckBoxResultEventArgs(bool userClicked, bool result)
    {
        UserClicked = userClicked;
        Checked = result;
    }

    /// <summary>Gets or sets whether this CheckBoxResultEventArgs is checked.</summary>
    /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
    public bool Checked { get; set; }

    /// <summary>Gets or sets a value indicating whether [user clicked].</summary>
    /// <value><c>true</c> if [user clicked]; otherwise, <c>false</c>.</value>
    public bool UserClicked { get; set; }
}
