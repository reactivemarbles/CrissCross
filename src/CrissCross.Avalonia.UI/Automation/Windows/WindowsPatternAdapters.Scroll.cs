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
    /// <summary>Forwards native Scroll operations to the public Avalonia provider.</summary>
    /// <param name="node">The associated node.</param>
    /// <param name="provider">The associated provider.</param>
    [SupportedOSPlatform("windows")]
    [RequiresDynamicCode("Native UI Automation uses runtime COM callable wrappers.")]
    [RequiresUnreferencedCode("Native COM callable wrappers require provider metadata.")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class ScrollAdapter(WindowsAutomationNode node, IScrollProvider provider) : IWindowsScrollProvider
    {
        /// <summary>The associated native node.</summary>
        private readonly WindowsAutomationNode _node = node;

        /// <summary>The associated Avalonia provider.</summary>
        private readonly IScrollProvider _provider = provider;

        /// <inheritdoc/>
        public void Scroll(int horizontalAmount, int verticalAmount) => _node.Context.Invoke(() => _provider.Scroll((ScrollAmount)horizontalAmount, (ScrollAmount)verticalAmount));

        /// <inheritdoc/>
        public void SetScrollPercent(double horizontalPercent, double verticalPercent) => _node.Context.Invoke(() => _provider.SetScrollPercent(horizontalPercent, verticalPercent));

        /// <inheritdoc/>
        public double GetHorizontalScrollPercent() => _node.Context.Invoke(() => _provider.HorizontalScrollPercent);

        /// <inheritdoc/>
        public double GetVerticalScrollPercent() => _node.Context.Invoke(() => _provider.VerticalScrollPercent);

        /// <inheritdoc/>
        public double GetHorizontalViewSize() => _node.Context.Invoke(() => _provider.HorizontalViewSize);

        /// <inheritdoc/>
        public double GetVerticalViewSize() => _node.Context.Invoke(() => _provider.VerticalViewSize);

        /// <inheritdoc/>
        public bool ReadHorizontallyScrollable() => _node.Context.Invoke(() => _provider.HorizontallyScrollable);

        /// <inheritdoc/>
        public bool ReadVerticallyScrollable() => _node.Context.Invoke(() => _provider.VerticallyScrollable);
    }
}
