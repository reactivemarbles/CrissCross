// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CP.Reactive.Collections;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// ReactiveTreeViewModel.
/// </summary>
/// <seealso cref="RxObject" />
public class ReactiveTreeViewModel : RxObject
{
    /// <summary>
    /// Gets or sets the children.
    /// </summary>
    /// <value>
    /// The children.
    /// </value>
    public ReactiveList<ReactiveTreeItem> Children { get; set; } = [];

    /// <summary>
    /// Releases all resources used by the current instance, including child components, and optionally disposes managed
    /// resources.
    /// </summary>
    /// <remarks>Overrides the base class implementation to ensure that child components are properly disposed
    /// when the instance is disposed.</remarks>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Children.Dispose();
        }

        base.Dispose(disposing);
    }
}
