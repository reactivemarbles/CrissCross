// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents ISecondColorStorage.</summary>
internal interface ISecondColorStorage
{
    /// <summary>Gets or sets the state of the second color.</summary>
    /// <value>
    /// The state of the second color.
    /// </value>
    ColorState SecondColorState { get; set; }
}
