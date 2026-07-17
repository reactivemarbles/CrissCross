// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents AnimationErrorKind.</summary>
public enum AnimationErrorKind
{
    /// <summary>The loading.</summary>
    Loading,

    /// <summary>The rendering.</summary>
    Rendering,
}
