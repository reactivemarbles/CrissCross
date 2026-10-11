// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AvaloniaWindow = Avalonia.Controls.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Identifies a window whose rich text automation bridge was disabled after native content was added.</summary>
public sealed class RichTextWindowsAutomationUnavailableEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="RichTextWindowsAutomationUnavailableEventArgs"/> class.</summary>
    /// <param name="window">The window containing unsupported native content.</param>
    internal RichTextWindowsAutomationUnavailableEventArgs(AvaloniaWindow window) => Window = window;

    /// <summary>Gets the window whose registration was disabled.</summary>
    public AvaloniaWindow Window { get; }

    /// <summary>Gets the reason the bridge was disabled.</summary>
    public string Reason => "A NativeControlHost was added to the window. "
        + "The rich text Windows automation bridge was removed and Avalonia's original automation provider restored.";
}
