// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Defines platform-neutral theme choices exposed by theme selection controls.</summary>
public enum ThemeChoice
{
    /// <summary>Follow the current operating-system or host application theme.</summary>
    System,

    /// <summary>Use the light application theme.</summary>
    Light,

    /// <summary>Use the dark application theme.</summary>
    Dark,

    /// <summary>Use a high-contrast theme when the current platform supports it.</summary>
    HighContrast
}
