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
    /// <summary>Forwards native Selection operations to the public Avalonia provider.</summary>
    /// <param name="node">The associated node.</param>
    /// <param name="provider">The associated provider.</param>
    [SupportedOSPlatform("windows")]
    [RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
    [RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class SelectionAdapter(WindowsAutomationNode node, ISelectionProvider provider) : IWindowsSelectionProvider
    {
        /// <summary>The associated native node.</summary>
        private readonly WindowsAutomationNode _node = node;

        /// <summary>The associated Avalonia provider.</summary>
        private readonly ISelectionProvider _provider = provider;

        /// <inheritdoc/>
        public bool ReadCanSelectMultiple() => _node.Context.Invoke(() => _provider.CanSelectMultiple);

        /// <inheritdoc/>
        public bool ReadIsSelectionRequired() => _node.Context.Invoke(() => _provider.IsSelectionRequired);

        /// <inheritdoc/>
        public IWindowsRawElementProviderSimple[] GetSelection() => _node.Context.Invoke(() => MapSelection(_node, _provider));
    }
}
