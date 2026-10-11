// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Adapts public Avalonia patterns to native UI Automation.</summary>
internal static partial class WindowsPatternAdapters
{
    /// <summary>Adapts a top-level Avalonia window through public window APIs.</summary>
    /// <param name="node">The associated node.</param>
    /// <param name="window">The associated window.</param>
    [SupportedOSPlatform("windows")]
    [RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
    [RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class WindowAdapter(WindowsAutomationNode node, global::Avalonia.Controls.Window window) : IWindowsWindowProvider
    {
        /// <summary>The native minimized visual state.</summary>
        private const int MinimizedState = 2;

        /// <summary>The native ready interaction state.</summary>
        private const int ReadyState = 2;

        /// <summary>The native blocked interaction state.</summary>
        private const int BlockedState = 3;

        /// <summary>The associated automation node.</summary>
        private readonly WindowsAutomationNode _node = node;

        /// <summary>The associated window.</summary>
        private readonly global::Avalonia.Controls.Window _window = window;

        /// <inheritdoc/>
        public bool ReadCanMaximize() => _node.Context.Invoke(() => _window.CanMaximize);

        /// <inheritdoc/>
        public bool ReadCanMinimize() => _node.Context.Invoke(() => _window.CanMinimize);

        /// <inheritdoc/>
        public bool ReadIsModal() => _node.Context.Invoke(() => _window.IsDialog);

        /// <inheritdoc/>
        public int GetWindowVisualState() => _node.Context.Invoke(() => _window.WindowState switch
        {
            WindowState.Normal => 0,
            WindowState.Maximized => 1,
            WindowState.Minimized => MinimizedState,
            _ => throw new InvalidOperationException("The window has an unsupported visual state."),
        });

        /// <inheritdoc/>
        public int GetWindowInteractionState() => _node.Context.Invoke(() => _window.IsEnabled ? ReadyState : BlockedState);

        /// <inheritdoc/>
        public bool ReadIsTopmost() => _node.Context.Invoke(() => _window.Topmost);

        /// <inheritdoc/>
        public void SetVisualState(int state) => _node.Context.Invoke(() => _window.WindowState = state switch
        {
            0 => WindowState.Normal,
            1 => WindowState.Maximized,
            MinimizedState => WindowState.Minimized,
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        });

        /// <inheritdoc/>
        public void Close() => _node.Context.Invoke(_window.Close);

        /// <inheritdoc/>
        public bool WaitForInputIdle(int milliseconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(milliseconds);
            using var process = Process.GetCurrentProcess();
            return process.WaitForInputIdle(milliseconds);
        }
    }
}
