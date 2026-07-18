// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents WindowBackdropType.</summary>
public enum WindowBackdropType
{
    /// <summary>No backdrop effect.</summary>
    None,

    /// <summary>Sets <c>DWMWA_SYSTEMBACKDROP_TYPE</c> to <see langword="0"></see>.</summary>
    Auto,

    /// <summary>Windows 11 Mica effect.</summary>
    Mica,

    /// <summary>Windows Acrylic effect.</summary>
    Acrylic,

    /// <summary>Windows 11 wallpaper blur effect.</summary>
    Tabbed,
}
