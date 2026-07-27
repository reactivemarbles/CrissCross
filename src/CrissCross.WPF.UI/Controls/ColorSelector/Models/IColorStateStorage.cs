// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents IColorStateStorage.</summary>
public interface IColorStateStorage
{
    /// <summary>Gets or sets the state of the color.</summary>
    /// <value>
    /// The state of the color.
    /// </value>
    ColorState ColorState { get; set; }
}
