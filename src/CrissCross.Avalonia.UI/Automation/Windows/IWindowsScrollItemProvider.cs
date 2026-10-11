// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation ScrollItemProvider ABI.</summary>
[ComVisible(true)]
[Guid("2360c714-4bf1-4b26-ba65-9b21316127eb")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsScrollItemProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void ScrollIntoView();
}
