// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Automation.Provider;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Adapts public Avalonia patterns to native UI Automation.</summary>
internal static partial class WindowsPatternAdapters
{
    /// <summary>Forwards native SelectionItem operations to the public Avalonia provider.</summary>
    /// <param name="node">The associated node.</param>
    /// <param name="provider">The associated provider.</param>
    [SupportedOSPlatform("windows")]
    [RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
    [RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class SelectionItemAdapter(WindowsAutomationNode node, ISelectionItemProvider provider) : IWindowsSelectionItemProvider
    {
        /// <summary>The associated native node.</summary>
        private readonly WindowsAutomationNode _node = node;

        /// <summary>The associated Avalonia provider.</summary>
        private readonly ISelectionItemProvider _provider = provider;

        /// <inheritdoc/>
        public void Select() => _node.Context.Invoke(_provider.Select);

        /// <inheritdoc/>
        public void AddToSelection() => _node.Context.Invoke(_provider.AddToSelection);

        /// <inheritdoc/>
        public void RemoveFromSelection() => _node.Context.Invoke(_provider.RemoveFromSelection);

        /// <inheritdoc/>
        public bool ReadIsSelected() => _node.Context.Invoke(() => _provider.IsSelected);

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple? GetSelectionContainer() => _node.Context.Invoke(FindSelectionContainer);

        /// <summary>Resolves an explicitly composed provider through its public parent peers.</summary>
        /// <returns>The selection container node.</returns>
        private WindowsAutomationNode? FindSelectionContainer()
        {
            var container = _provider.SelectionContainer;
            if (container is null)
            {
                return null;
            }

            if (container is global::Avalonia.Automation.Peers.AutomationPeer peer)
            {
                return _node.Context.Node(peer);
            }

            for (var parent = _node.Peer.GetParent(); parent is not null; parent = parent.GetParent())
            {
                if (ReferenceEquals(parent.GetProvider<ISelectionProvider>(), container))
                {
                    return _node.Context.Node(parent);
                }
            }

            throw new InvalidOperationException("The selection container provider has no associated public automation peer.");
        }
    }
}
