// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaWindow = Avalonia.Controls.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Owns the native automation tree for one window.</summary>
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
[RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
internal sealed class WindowsAutomationContext : IDisposable
{
    /// <summary>The native focus-changed event identifier.</summary>
    private const int FocusChangedEventId = 20_005;

    /// <summary>The native element-unavailable error.</summary>
    private const int ElementUnavailableError = unchecked((int)0x8004_0201);

    /// <summary>The native element-disabled error.</summary>
    private const int ElementDisabledError = unchecked((int)0x8004_0200);

    /// <summary>The native invalid-argument error.</summary>
    private const int InvalidArgumentError = unchecked((int)0x8007_0057);

    /// <summary>The window supplying coordinates and focus.</summary>
    private readonly AvaloniaWindow _window;

    /// <summary>Stable providers associated with managed peers.</summary>
    private readonly Dictionary<AutomationPeer, WindowsAutomationNode> _nodes = [with(ReferenceEqualityComparer.Instance)];

    /// <summary>The retained native message hook.</summary>
    private readonly Win32Properties.CustomWndProcHookCallback _hook;

    /// <summary>Whether this registration has been released.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="WindowsAutomationContext"/> class.</summary>
    /// <param name="window">The window to expose.</param>
    internal WindowsAutomationContext(AvaloniaWindow window)
    {
        _window = window;
        _hook = OnWindowMessage;
        ValidateNativeHosts(window);
        RootNode = Node(ControlAutomationPeer.CreatePeerForElement(window));
        window.Closed += OnClosed;
        window.AddHandler(InputElement.GotFocusEvent, OnFocusChanged, RoutingStrategies.Bubble);
        Win32Properties.AddWndProcHookCallback(window, _hook);
    }

    /// <summary>Gets the root provider.</summary>
    internal WindowsAutomationNode RootNode { get; }

    /// <summary>Gets the top-level native handle.</summary>
    internal nint Handle => _window.TryGetPlatformHandle()?.Handle ?? 0;

    /// <summary>Gets whether this registration has been released.</summary>
    internal bool IsDisposed => _disposed;

    /// <summary>Releases native hooks and managed peer event subscriptions.</summary>
    public void Dispose() => Dispatcher.UIThread.Invoke(Release);

    /// <summary>Gets the stable provider for a peer.</summary>
    /// <param name="peer">The managed automation peer.</param>
    /// <returns>The associated provider.</returns>
    internal WindowsAutomationNode Node(AutomationPeer peer)
    {
        Dispatcher.UIThread.VerifyAccess();
        ObjectDisposedException.ThrowIf(_disposed, this);
        ref var node = ref CollectionsMarshal.GetValueRefOrAddDefault(_nodes, peer, out var exists);
        if (exists)
        {
            return node!;
        }

        node = new(this, peer);
        node.Attach();
        return node;
    }

    /// <summary>Executes a provider operation on the UI thread.</summary>
    /// <param name="action">The operation.</param>
    internal void Invoke(Action action)
    {
        try
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                if (_disposed)
                {
                    throw new WindowsAutomationException("The automation registration is no longer available.", ElementUnavailableError);
                }

                action();
            });
        }
        catch (ElementNotEnabledException exception)
        {
            throw new WindowsAutomationException(exception.Message, ElementDisabledError, exception);
        }
        catch (ArgumentException exception)
        {
            throw new WindowsAutomationException(exception.Message, InvalidArgumentError, exception);
        }
    }

    /// <summary>Executes a provider query on the UI thread.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="query">The query.</param>
    /// <returns>The result.</returns>
    internal T Invoke<T>(Func<T> query)
    {
        T result = default!;
        Invoke((Action)(() => result = query()));
        return result;
    }

    /// <summary>Converts top-level device-independent bounds to screen pixels.</summary>
    /// <param name="rectangle">The top-level bounds.</param>
    /// <returns>The native screen rectangle.</returns>
    internal WindowsAutomationRect ToScreenRectangle(in Rect rectangle)
    {
        var origin = _window.PointToScreen(rectangle.TopLeft);
        var scale = _window.RenderScaling;
        return new(origin.X, origin.Y, rectangle.Width * scale, rectangle.Height * scale);
    }

    /// <summary>Converts screen pixels to top-level device-independent coordinates.</summary>
    /// <param name="x">The screen horizontal coordinate.</param>
    /// <param name="y">The screen vertical coordinate.</param>
    /// <returns>The point relative to the window.</returns>
    internal Point ToRootPoint(double x, double y) => _window.PointToClient(new((int)x, (int)y));

    /// <summary>Finds the focused managed control's automation provider.</summary>
    /// <returns>The focused provider, if any.</returns>
    internal WindowsAutomationNode? GetFocus() => _window.FocusManager?.GetFocusedElement() is Control control
        ? Node(ControlAutomationPeer.CreatePeerForElement(control))
        : null;

    /// <summary>Finds the deepest automation peer under a screen point.</summary>
    /// <param name="x">The screen horizontal coordinate.</param>
    /// <param name="y">The screen vertical coordinate.</param>
    /// <returns>The matching provider.</returns>
    internal WindowsAutomationNode? FromPoint(double x, double y) => FindPeer(RootNode, ToRootPoint(x, y));

    /// <summary>Rejects native islands whose providers cannot be preserved through public APIs.</summary>
    /// <param name="visual">The subtree to validate.</param>
    private static void ValidateNativeHosts(Visual visual)
    {
        if (!HasNativeHosts(visual))
        {
            return;
        }

        throw new NotSupportedException("The rich text Windows automation bridge cannot replace a window containing NativeControlHost. "
            + "Avalonia does not expose the native automation island handle through public APIs.");
    }

    /// <summary>Detects native islands in both realized visuals and pending logical content.</summary>
    /// <param name="visual">The subtree to inspect.</param>
    /// <returns>Whether the subtree contains a native control host.</returns>
    private static bool HasNativeHosts(Visual visual)
    {
        HashSet<Visual> visited = [with(ReferenceEqualityComparer.Instance)];
        Stack<Visual> pending = [];
        pending.Push(visual);
        while (pending.TryPop(out var current))
        {
            if (!visited.Add(current))
            {
                continue;
            }

            if (current is NativeControlHost)
            {
                return true;
            }

            AddVisualChildren(current, pending);
        }

        return false;
    }

    /// <summary>Adds visible and logical child controls to the validation traversal.</summary>
    /// <param name="visual">The parent visual.</param>
    /// <param name="pending">The traversal stack.</param>
    private static void AddVisualChildren(Visual visual, Stack<Visual> pending)
    {
        foreach (var child in visual.GetVisualChildren())
        {
            pending.Push(child);
        }

        if (visual is not ILogical logical)
        {
            return;
        }

        foreach (var child in logical.GetLogicalChildren())
        {
            if (child is not Visual childVisual)
            {
                continue;
            }

            pending.Push(childVisual);
        }
    }

    /// <summary>Walks automation children in reverse drawing order for hit testing.</summary>
    /// <param name="node">The subtree provider.</param>
    /// <param name="point">The top-level point.</param>
    /// <returns>The deepest provider containing the point.</returns>
    private WindowsAutomationNode? FindPeer(WindowsAutomationNode node, Point point)
    {
        var children = node.Peer.GetChildren();
        for (var index = children.Count - 1; index >= 0; index--)
        {
            var child = Node(children[index]);
            if (!child.Peer.IsOffscreen() && child.Peer.GetBoundingRectangle().Contains(point))
            {
                return FindPeer(child, point) ?? child;
            }
        }

        return node.Peer.GetBoundingRectangle().Contains(point) ? node : null;
    }

    /// <summary>Supplies the provider in response to a native UI Automation request.</summary>
    /// <param name="windowHandle">The HWND.</param>
    /// <param name="message">The message identifier.</param>
    /// <param name="wordParameter">The first message parameter.</param>
    /// <param name="longParameter">The second message parameter.</param>
    /// <param name="handled">Whether the framework should skip processing.</param>
    /// <returns>The native UI Automation message result.</returns>
    private nint OnWindowMessage(nint windowHandle, uint message, nint wordParameter, nint longParameter, ref bool handled)
    {
        const uint GetObjectMessage = 0x003D;
        const int UiaRootObjectId = -25;
        if (_disposed || message != GetObjectMessage || unchecked((int)longParameter) != UiaRootObjectId)
        {
            return 0;
        }

        if (HasNativeHosts(_window))
        {
            Release();
            RichTextWindowsAutomation.ReportUnavailable(_window);
            return 0;
        }

        handled = true;
        return WindowsAutomationNative.ReturnProvider(windowHandle, wordParameter, longParameter, RootNode);
    }

    /// <summary>Releases hooks when the window closes.</summary>
    /// <param name="sender">The window.</param>
    /// <param name="args">The event.</param>
    private void OnClosed(object? sender, EventArgs args) => Release();

    /// <summary>Announces keyboard focus to native clients.</summary>
    /// <param name="sender">The window.</param>
    /// <param name="args">The focus event.</param>
    private void OnFocusChanged(object? sender, RoutedEventArgs args)
    {
        if (GetFocus() is not { } focused)
        {
            return;
        }

        WindowsAutomationNative.RaiseEvent(focused, FocusChangedEventId);
    }

    /// <summary>Releases registration resources on the UI thread.</summary>
    private void Release()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Win32Properties.RemoveWndProcHookCallback(_window, _hook);
        _window.Closed -= OnClosed;
        _window.RemoveHandler(InputElement.GotFocusEvent, OnFocusChanged);
        foreach (var node in _nodes.Values)
        {
            node.Detach();
        }

        _nodes.Clear();
    }
}
