// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Automation;
using Avalonia.Automation.Peers;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Adapts a public Avalonia automation peer to the native Windows provider tree.</summary>
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
[RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
internal sealed partial class WindowsAutomationNode : IWindowsRawElementProviderSimple, IWindowsRawElementProviderFragment, IWindowsRawElementProviderFragmentRoot
{
    /// <summary>The native Text pattern identifier.</summary>
    private const int TextPatternId = 10_014;

    /// <summary>The UI Automation runtime ID append marker.</summary>
    private const int AppendRuntimeId = 3;

    /// <summary>The native text selection event identifier.</summary>
    private const int TextSelectionChangedEventId = 20_014;

    /// <summary>The native text content event identifier.</summary>
    private const int TextChangedEventId = 20_015;

    /// <summary>The native server-side provider flag.</summary>
    private const int ServerSideProvider = 2;

    /// <summary>The native live-region changed event identifier.</summary>
    private const int LiveRegionChangedEventId = 20_024;

    /// <summary>The last allocated runtime identifier.</summary>
    private static int _nextRuntimeId;

    /// <summary>The stable native runtime identifier.</summary>
    private readonly int[] _runtimeId;

    /// <summary>Initializes a new instance of the <see cref="WindowsAutomationNode"/> class.</summary>
    /// <param name="context">The window registration.</param>
    /// <param name="peer">The managed peer.</param>
    internal WindowsAutomationNode(WindowsAutomationContext context, AutomationPeer peer)
    {
        Context = context;
        Peer = peer;
        _runtimeId = [AppendRuntimeId, Interlocked.Increment(ref _nextRuntimeId)];
    }

    /// <summary>Gets the managed automation peer.</summary>
    internal AutomationPeer Peer { get; }

    /// <summary>Gets the associated window registration.</summary>
    internal WindowsAutomationContext Context { get; }

    /// <summary>Gets the editor when this node represents a rich text document.</summary>
    internal RichTextBox? Editor => (Peer as RichTextBoxAutomationPeer)?.Editor;

    /// <summary>Gets native provider options for a managed server-side provider.</summary>
    /// <returns>The server-side provider flag.</returns>
    public int GetProviderOptions() => ServerSideProvider;

    /// <summary>Gets the HWND host provider only for the window root.</summary>
    /// <returns>The native HWND host provider, or null for child nodes.</returns>
    public IWindowsRawElementProviderSimple? GetHostRawElementProvider() => ReferenceEquals(this, Context.RootNode)
        ? Context.Invoke(() => WindowsAutomationNative.HostProvider(Context.Handle))
        : null;

    /// <summary>Gets the screen-pixel bounding rectangle.</summary>
    /// <returns>The native screen bounds.</returns>
    public WindowsAutomationRect GetBoundingRectangle() => Context.Invoke(() => Context.ToScreenRectangle(Peer.GetBoundingRectangle()));

    /// <summary>Gets the fragment root.</summary>
    /// <returns>The window's root provider.</returns>
    public IWindowsRawElementProviderFragmentRoot GetFragmentRoot() => Context.Invoke<IWindowsRawElementProviderFragmentRoot>(() => Context.RootNode);

    /// <summary>Gets the provider for a native pattern identifier.</summary>
    /// <param name="patternId">The UI Automation pattern identifier.</param>
    /// <returns>The native pattern provider, if supported.</returns>
    public object? GetPatternProvider(int patternId) => Context.Invoke(() => patternId == TextPatternId && Editor is not null
        ? new WindowsTextProvider(this)
        : WindowsPatternAdapters.GetProvider(this, patternId));

    /// <summary>Gets a native property value.</summary>
    /// <param name="propertyId">The UI Automation property identifier.</param>
    /// <returns>The property value, or null when unsupported.</returns>
    public object? GetPropertyValue(int propertyId) => Context.Invoke(() => ReadProperty(propertyId));

    /// <summary>Gets this peer's stable native runtime identifier.</summary>
    /// <returns>The native runtime identifier.</returns>
    public int[] GetRuntimeId() => Context.Invoke(() => (int[])_runtimeId.Clone());

    /// <summary>Navigates the managed automation tree.</summary>
    /// <param name="direction">The requested relationship.</param>
    /// <returns>The related native provider.</returns>
    public IWindowsRawElementProviderFragment? Navigate(WindowsNavigateDirection direction) => Context.Invoke(() => NavigateCore(direction));

    /// <summary>Gets embedded native fragment roots.</summary>
    /// <returns>An empty array because unsupported native hosts are rejected during registration.</returns>
    public IWindowsRawElementProviderFragmentRoot[] GetEmbeddedFragmentRoots() => [];

    /// <summary>Focuses the associated managed control.</summary>
    public void SetFocus() => Context.Invoke(Peer.SetFocus);

    /// <summary>Gets the provider under a screen point.</summary>
    /// <param name="x">The horizontal screen coordinate.</param>
    /// <param name="y">The vertical screen coordinate.</param>
    /// <returns>The matching native fragment.</returns>
    public IWindowsRawElementProviderFragment? ElementProviderFromPoint(double x, double y) => Context.Invoke(() => Context.FromPoint(x, y));

    /// <summary>Gets the focused control provider.</summary>
    /// <returns>The focused fragment, if any.</returns>
    public IWindowsRawElementProviderFragment? GetFocus() => Context.Invoke(Context.GetFocus);

    /// <summary>Attaches subscriptions after construction completes.</summary>
    internal void Attach()
    {
        Peer.ChildrenChanged += OnChildrenChanged;
        Peer.PropertyChanged += OnPropertyChanged;
        if (Peer is not RichTextBoxAutomationPeer editorPeer)
        {
            return;
        }

        editorPeer.TextSelectionChanged += OnTextSelectionChanged;
        editorPeer.Editor.Document.TextChanged += OnTextChanged;
    }

    /// <summary>Detaches subscriptions when the registration ends.</summary>
    internal void Detach()
    {
        Peer.ChildrenChanged -= OnChildrenChanged;
        Peer.PropertyChanged -= OnPropertyChanged;
        if (Peer is not RichTextBoxAutomationPeer editorPeer)
        {
            return;
        }

        editorPeer.TextSelectionChanged -= OnTextSelectionChanged;
        editorPeer.Editor.Document.TextChanged -= OnTextChanged;
    }

    /// <summary>Navigates peers while on the UI thread.</summary>
    /// <param name="direction">The requested relationship.</param>
    /// <returns>The related native provider.</returns>
    private WindowsAutomationNode? NavigateCore(WindowsNavigateDirection direction)
    {
        var parent = Peer.GetParent();
        var children = Peer.GetChildren();
        return direction switch
        {
            WindowsNavigateDirection.Parent => parent is null ? null : Context.Node(parent),
            WindowsNavigateDirection.FirstChild => children.Count == 0 ? null : Context.Node(children[0]),
            WindowsNavigateDirection.LastChild => children.Count == 0 ? null : Context.Node(children[^1]),
            WindowsNavigateDirection.NextSibling => GetSibling(parent, 1),
            WindowsNavigateDirection.PreviousSibling => GetSibling(parent, -1),
            _ => null,
        };
    }

    /// <summary>Finds a sibling without constructing an alternative tree.</summary>
    /// <param name="parent">The managed parent.</param>
    /// <param name="offset">The sibling direction.</param>
    /// <returns>The matching provider.</returns>
    private WindowsAutomationNode? GetSibling(AutomationPeer? parent, int offset)
    {
        if (parent is null)
        {
            return null;
        }

        var siblings = parent.GetChildren();
        for (var index = 0; index < siblings.Count; index++)
        {
            if (ReferenceEquals(siblings[index], Peer))
            {
                var target = index + offset;
                return target >= 0 && target < siblings.Count ? Context.Node(siblings[target]) : null;
            }
        }

        return null;
    }

    /// <summary>Announces tree invalidation.</summary>
    /// <param name="sender">The peer.</param>
    /// <param name="args">The event.</param>
    private void OnChildrenChanged(object? sender, EventArgs args) => WindowsAutomationNative.RaiseChildrenChanged(this);

    /// <summary>Announces document selection changes.</summary>
    /// <param name="sender">The peer.</param>
    /// <param name="args">The event.</param>
    private void OnTextSelectionChanged(object? sender, EventArgs args) => WindowsAutomationNative.RaiseEvent(this, TextSelectionChangedEventId);

    /// <summary>Announces document content changes.</summary>
    /// <param name="sender">The document.</param>
    /// <param name="args">The event.</param>
    private void OnTextChanged(object? sender, EventArgs args) => WindowsAutomationNative.RaiseEvent(this, TextChangedEventId);

    /// <summary>Announces mapped managed property changes.</summary>
    /// <param name="sender">The peer.</param>
    /// <param name="args">The change.</param>
    private void OnPropertyChanged(object? sender, AutomationPropertyChangedEventArgs args)
    {
        if (!PropertyIds.TryGetValue(args.Property, out var identifier))
        {
            return;
        }

        WindowsAutomationNative.RaisePropertyChanged(this, identifier, ConvertProperty(args.OldValue), ConvertProperty(args.NewValue));
        if (identifier != PropertyId.Name || Peer.GetLiveSetting() == AutomationLiveSetting.Off)
        {
            return;
        }

        WindowsAutomationNative.RaiseEvent(this, LiveRegionChangedEventId);
    }
}
