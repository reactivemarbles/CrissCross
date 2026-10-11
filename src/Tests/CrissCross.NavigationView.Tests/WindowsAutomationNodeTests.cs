// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using CrissCross.Avalonia.UI.Controls.Automation.Windows;
using RichTextBox = CrissCross.Avalonia.UI.Controls.RichTextBox;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies preservation of ordinary controls alongside native rich text automation.</summary>
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Tests exercise the native Windows COM provider.")]
[RequiresUnreferencedCode("Tests exercise the native Windows COM provider.")]
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class WindowsAutomationNodeTests
{
    /// <summary>The native Invoke pattern identifier.</summary>
    private const int InvokePatternId = 10_000;

    /// <summary>The native Value pattern identifier.</summary>
    private const int ValuePatternId = 10_002;

    /// <summary>The native Text pattern identifier.</summary>
    private const int TextPatternId = 10_014;

    /// <summary>The native Toggle pattern identifier.</summary>
    private const int TogglePatternId = 10_015;

    /// <summary>The native name property identifier.</summary>
    private const int NamePropertyId = 30_005;

    /// <summary>The native value property identifier.</summary>
    private const int ValuePropertyId = 30_045;

    /// <summary>The native element-unavailable HRESULT.</summary>
    private const int ElementUnavailableError = unchecked((int)0x8004_0201);

    /// <summary>The replacement used by native value providers.</summary>
    private const string ReplacementText = "replacement";

    /// <summary>Native adapters preserve button, checkbox, and ordinary textbox behavior.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task GetPatternProvider_MixedControls_PreservesManagedControlPatterns()
    {
        var editor = new RichTextBox { Text = "rich text" };
        var button = new Button { Content = "Invoke me" };
        var checkbox = new CheckBox { Content = "Check me" };
        var textbox = new TextBox { Text = "ordinary text" };
        var panel = new StackPanel();
        panel.Children.Add(editor);
        panel.Children.Add(button);
        panel.Children.Add(checkbox);
        panel.Children.Add(textbox);
        var window = new Window { Content = panel };
        using var context = new WindowsAutomationContext(window);
        var buttonNode = context.Node(ControlAutomationPeer.CreatePeerForElement(button));
        var checkboxNode = context.Node(ControlAutomationPeer.CreatePeerForElement(checkbox));
        var textboxNode = context.Node(ControlAutomationPeer.CreatePeerForElement(textbox));
        var editorNode = context.Node(ControlAutomationPeer.CreatePeerForElement(editor));
        var invocationCount = 0;
        button.Click += (_, _) => invocationCount++;
        var invoke = await Assert.That(buttonNode.GetPatternProvider(InvokePatternId) as IWindowsInvokeProvider).IsNotNull()
            ?? throw new InvalidOperationException("Invoke provider is missing.");
        invoke.Invoke();
        var toggle = await Assert.That(checkboxNode.GetPatternProvider(TogglePatternId) as IWindowsToggleProvider).IsNotNull()
            ?? throw new InvalidOperationException("Toggle provider is missing.");
        toggle.Toggle();
        var value = await Assert.That(textboxNode.GetPatternProvider(ValuePatternId) as IWindowsValueProvider).IsNotNull()
            ?? throw new InvalidOperationException("Value provider is missing.");
        value.SetValue(ReplacementText);
        await Assert.That(invocationCount).IsEqualTo(1);
        await Assert.That(checkbox.IsChecked).IsTrue();
        await Assert.That(textbox.Text).IsEqualTo(ReplacementText);
        await Assert.That(editorNode.GetPatternProvider(TextPatternId)).IsNotNull();
        await Assert.That(textboxNode.GetPatternProvider(TextPatternId)).IsNull();
        await Assert.That(buttonNode.GetPropertyValue(NamePropertyId)).IsEqualTo("Invoke me");
        await Assert.That(textboxNode.GetPropertyValue(ValuePropertyId)).IsEqualTo(ReplacementText);
    }

    /// <summary>Providers retain identity and fail clearly after registration disposal.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Node_SamePeerAndDisposedContext_PreservesIdentityAndRejectsQueries()
    {
        var editor = new RichTextBox { Text = "content" };
        var window = new Window { Content = editor };
        var context = new WindowsAutomationContext(window);
        var peer = ControlAutomationPeer.CreatePeerForElement(editor);
        var node = context.Node(peer);
        var runtimeId = node.GetRuntimeId();
        await Assert.That(context.Node(peer)).IsSameReferenceAs(node);
        await Assert.That(node.GetRuntimeId()[1]).IsEqualTo(runtimeId[1]);
        await Assert.That(node.GetFragmentRoot()).IsSameReferenceAs(context.RootNode);
        context.Dispose();
        context.Dispose();
        var failure = await Assert.That(() => node.GetPropertyValue(NamePropertyId)).Throws<WindowsAutomationException>()
            ?? throw new InvalidOperationException("The disposed provider did not report a native error.");
        await Assert.That(failure.HResult).IsEqualTo(ElementUnavailableError);
    }

    /// <summary>Unsupported native islands are rejected before native hooks are registered.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Register_UnrealizedNativeHost_RejectsUnsupportedIsland()
    {
        var host = new NativeControlHost();
        var window = new Window { Content = host };
        await Assert.That(() =>
        {
            using var context = new WindowsAutomationContext(window);
        }).Throws<NotSupportedException>();
    }

    /// <summary>Automation navigation follows the actual managed peer tree.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Navigate_MixedVisualTree_PreservesParentAndSiblingRelationships()
    {
        var editor = new RichTextBox { Text = "document" };
        var button = new Button { Content = "action" };
        var panel = new StackPanel();
        panel.Children.Add(editor);
        panel.Children.Add(button);
        var window = new Window { Content = panel };
        window.Show();
        try
        {
            using var context = new WindowsAutomationContext(window);
            var panelNode = context.Node(ControlAutomationPeer.CreatePeerForElement(panel));
            var editorNode = context.Node(ControlAutomationPeer.CreatePeerForElement(editor));
            var buttonNode = context.Node(ControlAutomationPeer.CreatePeerForElement(button));
            await Assert.That(panelNode.Navigate(WindowsNavigateDirection.FirstChild)).IsSameReferenceAs(editorNode);
            await Assert.That(editorNode.Navigate(WindowsNavigateDirection.Parent)).IsSameReferenceAs(panelNode);
            await Assert.That(editorNode.Navigate(WindowsNavigateDirection.NextSibling)).IsSameReferenceAs(buttonNode);
            await Assert.That(buttonNode.Navigate(WindowsNavigateDirection.PreviousSibling)).IsSameReferenceAs(editorNode);
            await Assert.That(panelNode.Navigate(WindowsNavigateDirection.LastChild)).IsSameReferenceAs(buttonNode);
        }
        finally
        {
            window.Close();
        }
    }
}
