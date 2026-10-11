// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using CrissCross.Avalonia.UI.Controls.Automation.Windows;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies ordinary Avalonia controls retain native automation patterns.</summary>
[SupportedOSPlatform("windows")]
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class WindowsPatternAdaptersTests
{
    /// <summary>The native invoke identifier.</summary>
    private const int InvokePatternId = 10_000;

    /// <summary>The native value identifier.</summary>
    private const int ValuePatternId = 10_002;

    /// <summary>The native range value identifier.</summary>
    private const int RangeValuePatternId = 10_003;

    /// <summary>The native toggle identifier.</summary>
    private const int TogglePatternId = 10_015;

    /// <summary>The native window identifier.</summary>
    private const int WindowPatternId = 10_009;

    /// <summary>The native grid identifier.</summary>
    private const int GridPatternId = 10_006;

    /// <summary>The native table identifier.</summary>
    private const int TablePatternId = 10_012;

    /// <summary>The expected table width.</summary>
    private const int TableWidth = 2;

    /// <summary>The native selection item identifier.</summary>
    private const int SelectionItemPatternId = 10_010;

    /// <summary>Verifies common controls remain available through their native patterns.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task GetProvider_OrdinaryControls_PreservesAvaloniaPatterns()
    {
        var window = new global::Avalonia.Controls.Window();
        using var context = new WindowsAutomationContext(window);
        var button = new global::Avalonia.Controls.Button();
        var text = new global::Avalonia.Controls.TextBox { Text = "ordinary text" };
        var checkbox = new global::Avalonia.Controls.CheckBox { IsChecked = true };
        var slider = new global::Avalonia.Controls.Slider();
        var buttonNode = context.Node(ControlAutomationPeer.CreatePeerForElement(button));
        var textNode = context.Node(ControlAutomationPeer.CreatePeerForElement(text));
        var toggleNode = context.Node(ControlAutomationPeer.CreatePeerForElement(checkbox));
        var rangeNode = context.Node(ControlAutomationPeer.CreatePeerForElement(slider));
        await Assert.That(WindowsPatternAdapters.GetProvider(buttonNode, InvokePatternId) is IWindowsInvokeProvider).IsTrue();
        var value = WindowsPatternAdapters.GetProvider(textNode, ValuePatternId) as IWindowsValueProvider;
        await Assert.That(value?.ReadValue()).IsEqualTo(text.Text);
        var toggle = WindowsPatternAdapters.GetProvider(toggleNode, TogglePatternId) as IWindowsToggleProvider;
        await Assert.That(toggle?.GetToggleState()).IsEqualTo((int)ToggleState.On);
        var range = WindowsPatternAdapters.GetProvider(rangeNode, RangeValuePatternId) as IWindowsRangeValueProvider;
        await Assert.That(range?.ReadValue()).IsEqualTo(slider.Value);
        await Assert.That(WindowsPatternAdapters.GetProvider(context.RootNode, WindowPatternId) is IWindowsWindowProvider).IsTrue();
    }

    /// <summary>Verifies native value setters delegate to the original control.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task SetValue_OrdinaryTextBox_ChangesOriginalControl()
    {
        var window = new global::Avalonia.Controls.Window();
        using var context = new WindowsAutomationContext(window);
        var text = new global::Avalonia.Controls.TextBox { Text = "before" };
        var node = context.Node(ControlAutomationPeer.CreatePeerForElement(text));
        var provider = WindowsPatternAdapters.GetProvider(node, ValuePatternId) as IWindowsValueProvider
            ?? throw new InvalidOperationException("The text box value pattern is missing.");
        provider.SetValue("after");
        await Assert.That(text.Text).IsEqualTo("after");
    }

    /// <summary>Verifies grid positions in a spanning cell resolve to one native element.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task GetItem_SpanningTableCell_ReturnsStableCellAndHeaders()
    {
        var window = new global::Avalonia.Controls.Window();
        using var context = new WindowsAutomationContext(window);
        var editor = new CrissCross.Avalonia.UI.Controls.RichTextBox { Text = "<table><tr><th>A</th><th>B</th></tr><tr><td colspan=\"2\">body</td></tr></table>", IsDocumentEnabled = true };
        var peer = ControlAutomationPeer.CreatePeerForElement(editor);
        var tablePeer = GetSingleChild(peer);
        var node = context.Node(tablePeer);
        var grid = WindowsPatternAdapters.GetProvider(node, GridPatternId) as IWindowsGridProvider
            ?? throw new InvalidOperationException("The rich table grid pattern is missing.");
        var table = WindowsPatternAdapters.GetProvider(node, TablePatternId) as IWindowsTableProvider
            ?? throw new InvalidOperationException("The rich table pattern is missing.");
        await Assert.That(grid.GetColumnCount()).IsEqualTo(TableWidth);
        await Assert.That(grid.GetItem(1, 0)).IsEqualTo(grid.GetItem(1, 1));
        await Assert.That(table.GetColumnHeaders().Length).IsEqualTo(TableWidth);
    }

    /// <summary>Verifies composed selection containers resolve through their public parent peer.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task GetSelectionContainer_ComposedProvider_ResolvesAncestorNode()
    {
        var window = new global::Avalonia.Controls.Window();
        using var context = new WindowsAutomationContext(window);
        var provider = new CompositeSelectionProvider();
        var child = new CompositeSelectionItemPeer(new global::Avalonia.Controls.Control(), provider);
        var parent = new CompositeSelectionPeer(new global::Avalonia.Controls.Control(), child, provider);
        _ = parent.GetChildren();
        var childNode = context.Node(child);
        var parentNode = context.Node(parent);
        var item = WindowsPatternAdapters.GetProvider(childNode, SelectionItemPatternId) as IWindowsSelectionItemProvider
            ?? throw new InvalidOperationException("The composed selection item pattern is missing.");
        await Assert.That(item.GetSelectionContainer()).IsEqualTo(parentNode);
    }

    /// <summary>Requires a single child of a semantic table peer.</summary>
    /// <param name="peer">The semantic parent.</param>
    /// <returns>The single child.</returns>
    private static AutomationPeer GetSingleChild(AutomationPeer peer)
    {
        AutomationPeer? child = null;
        foreach (var candidate in peer.GetChildren())
        {
            if (child is not null)
            {
                throw new InvalidOperationException("The table automation peer has multiple children.");
            }

            child = candidate;
        }

        return child ?? throw new InvalidOperationException("The table automation peer has no child.");
    }

    /// <summary>Supplies a selection provider independently of the peer.</summary>
    private sealed class CompositeSelectionProvider : ISelectionProvider
    {
        /// <inheritdoc/>
        public bool CanSelectMultiple => true;

        /// <inheritdoc/>
        public bool IsSelectionRequired => false;

        /// <inheritdoc/>
        public IReadOnlyList<AutomationPeer> GetSelection() => [];
    }

    /// <summary>Supplies a selectable item through composition.</summary>
    /// <param name="container">The composed container.</param>
    private sealed class CompositeSelectionItemProvider(ISelectionProvider container) : ISelectionItemProvider
    {
        /// <summary>The selected state.</summary>
        private bool _selected;

        /// <inheritdoc/>
        public bool IsSelected => _selected;

        /// <inheritdoc/>
        public ISelectionProvider SelectionContainer { get; } = container;

        /// <inheritdoc/>
        public void AddToSelection() => _selected = true;

        /// <inheritdoc/>
        public void RemoveFromSelection() => _selected = false;

        /// <inheritdoc/>
        public void Select() => AddToSelection();
    }

    /// <summary>Exposes a child and a composed selection provider.</summary>
    /// <param name="owner">The owner.</param>
    /// <param name="child">The child peer.</param>
    /// <param name="provider">The composed provider.</param>
    private sealed class CompositeSelectionPeer(
        global::Avalonia.Controls.Control owner,
        AutomationPeer child,
        ISelectionProvider provider) : ControlAutomationPeer(owner)
    {
        /// <summary>The associated child.</summary>
        private readonly AutomationPeer _child = child;

        /// <summary>The composed provider.</summary>
        private readonly ISelectionProvider _provider = provider;

        /// <inheritdoc/>
        protected override object? GetProviderCore(Type providerType) =>
            providerType == typeof(ISelectionProvider) ? _provider : base.GetProviderCore(providerType);

        /// <inheritdoc/>
        protected override IReadOnlyList<AutomationPeer> GetChildrenCore() => [_child];
    }

    /// <summary>Exposes a composed selection item provider.</summary>
    /// <param name="owner">The owner.</param>
    /// <param name="container">The selection container.</param>
    private sealed class CompositeSelectionItemPeer(
        global::Avalonia.Controls.Control owner,
        ISelectionProvider container) : ControlAutomationPeer(owner)
    {
        /// <summary>The composed item provider.</summary>
        private readonly CompositeSelectionItemProvider _provider = new(container);

        /// <inheritdoc/>
        protected override object? GetProviderCore(Type providerType) =>
            providerType == typeof(ISelectionItemProvider) ? _provider : base.GetProviderCore(providerType);
    }
}
