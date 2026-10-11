// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Gallery.Views;

/// <summary>Registers the gallery's optional Windows accessibility bridge.</summary>
public partial class MainWindow
{
    /// <summary>The native accessibility registration for this window.</summary>
    private IDisposable? _windowsAutomation;

    /// <summary>Gets or sets the optional native automation factory configured by the executable entry point.</summary>
    internal static Func<Window, IDisposable>? WindowsAutomationFactory { get; set; }

    /// <summary>Wires the Windows automation registration to the native window lifetime.</summary>
    private void RegisterWindowsAutomation()
    {
        Opened += OnWindowsAutomationOpened;
        Closed += OnWindowsAutomationClosed;
    }

    /// <summary>Enables native TextPattern after the HWND exists.</summary>
    /// <param name="sender">The window.</param>
    /// <param name="args">The opened event.</param>
    private void OnWindowsAutomationOpened(object? sender, EventArgs args) =>
        _windowsAutomation = WindowsAutomationFactory?.Invoke(this);

    /// <summary>Releases native accessibility registration when the window closes.</summary>
    /// <param name="sender">The window.</param>
    /// <param name="args">The closed event.</param>
    private void OnWindowsAutomationClosed(object? sender, EventArgs args)
    {
        _windowsAutomation?.Dispose();
        _windowsAutomation = null;
    }
}
