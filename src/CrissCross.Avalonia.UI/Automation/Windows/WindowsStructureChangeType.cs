// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Defines the native structure event kind used by document updates.</summary>
internal enum WindowsStructureChangeType
{
    /// <summary>Indicates that clients must refresh the child collection.</summary>
    ChildrenInvalidated = 2,
}
