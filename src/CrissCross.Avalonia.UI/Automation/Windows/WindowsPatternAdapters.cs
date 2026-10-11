// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Avalonia.Automation.Provider;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Adapts public Avalonia patterns to native UI Automation.</summary>
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
[RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
internal static partial class WindowsPatternAdapters
{
    /// <summary>The native Invoke pattern identifier.</summary>
    private const int InvokePatternId = 10_000;

    /// <summary>The native Selection pattern identifier.</summary>
    private const int SelectionPatternId = 10_001;

    /// <summary>The native Value pattern identifier.</summary>
    private const int ValuePatternId = 10_002;

    /// <summary>The native RangeValue pattern identifier.</summary>
    private const int RangeValuePatternId = 10_003;

    /// <summary>The native Scroll pattern identifier.</summary>
    private const int ScrollPatternId = 10_004;

    /// <summary>The native ExpandCollapse pattern identifier.</summary>
    private const int ExpandCollapsePatternId = 10_005;

    /// <summary>The native SelectionItem pattern identifier.</summary>
    private const int SelectionItemPatternId = 10_010;

    /// <summary>The native Toggle pattern identifier.</summary>
    private const int TogglePatternId = 10_015;

    /// <summary>Gets the supported native pattern for an automation node.</summary>
    /// <param name="node">The node argument.</param>
    /// <param name="patternId">The patternId argument.</param>
    /// <returns>The native pattern result.</returns>
    internal static object? GetProvider(WindowsAutomationNode node, int patternId) => patternId switch
    {
        InvokePatternId => node.Peer.GetProvider<IInvokeProvider>() is { } provider ? new InvokeAdapter(node, provider) : null,
        SelectionPatternId => node.Peer.GetProvider<ISelectionProvider>() is { } provider ? new SelectionAdapter(node, provider) : null,
        ValuePatternId => node.Peer.GetProvider<IValueProvider>() is { } provider ? new ValueAdapter(node, provider) : null,
        RangeValuePatternId => node.Peer.GetProvider<IRangeValueProvider>() is { } provider ? new RangeValueAdapter(node, provider) : null,
        ScrollPatternId => node.Peer.GetProvider<IScrollProvider>() is { } provider ? new ScrollAdapter(node, provider) : null,
        ExpandCollapsePatternId => node.Peer.GetProvider<IExpandCollapseProvider>() is { } provider ? new ExpandCollapseAdapter(node, provider) : null,
        SelectionItemPatternId => node.Peer.GetProvider<ISelectionItemProvider>() is { } provider ? new SelectionItemAdapter(node, provider) : null,
        TogglePatternId => node.Peer.GetProvider<IToggleProvider>() is { } provider ? new ToggleAdapter(node, provider) : null,
        _ => GetDocumentProvider(node, patternId),
    };

    /// <summary>Maps selected Avalonia peers to their stable native nodes.</summary>
    /// <param name="node">The node argument.</param>
    /// <param name="provider">The provider argument.</param>
    /// <returns>The native pattern result.</returns>
    private static IWindowsRawElementProviderSimple[] MapSelection(WindowsAutomationNode node, ISelectionProvider provider)
    {
        var selection = provider.GetSelection();
        var result = new IWindowsRawElementProviderSimple[selection.Count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = node.Context.Node(selection[i]);
        }

        return result;
    }
}
