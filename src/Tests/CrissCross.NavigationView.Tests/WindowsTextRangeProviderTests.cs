// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using CrissCross.Avalonia.UI.Controls;
using CrissCross.Avalonia.UI.Controls.Automation.Windows;

using Window = global::Avalonia.Controls.Window;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies native text range operations against the actual rich document.</summary>
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Tests exercise the native Windows COM provider.")]
[RequiresUnreferencedCode("Tests exercise the native Windows COM provider.")]
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class WindowsTextRangeProviderTests
{
    /// <summary>Clones retain their own insertion endpoints and searches stay inside the range.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task Clone_SearchAndMove_PreservesIndependentEndpoints()
    {
        const int finalOccurrenceStart = 8;
        var editor = new RichTextBox { Text = "one two one" };
        var window = new Window { Content = editor };
        using var context = new WindowsAutomationContext(window);
        var range = new WindowsTextRangeProvider(context.Node(new RichTextBoxAutomationPeer(editor)), 0, editor.Document.Length);
        var clone = range.Clone();
        var match = range.FindText("one", true, false);
        await Assert.That(match?.GetText(-1)).IsEqualTo("one");
        await Assert.That(match?.CompareEndpoints(WindowsTextPatternRangeEndpoint.Start, range, WindowsTextPatternRangeEndpoint.Start)).IsEqualTo(finalOccurrenceStart);
        range.ExpandToEnclosingUnit(WindowsTextUnit.Word);
        await Assert.That(range.GetText(-1)).IsEqualTo("one ");
        await Assert.That(clone.GetText(-1)).IsEqualTo("one two one");
    }

    /// <summary>Crossing the opposite endpoint collapses a range and selection uses the editor.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task MoveEndpoint_CrossesEnd_CollapsesAndSelectsCaret()
    {
        const int initialEnd = 3;
        const int requestedMovement = 4;
        const int caretOffset = 5;
        var editor = new RichTextBox { Text = "abcdef" };
        var window = new Window { Content = editor };
        using var context = new WindowsAutomationContext(window);
        var range = new WindowsTextRangeProvider(context.Node(new RichTextBoxAutomationPeer(editor)), 1, initialEnd);
        var moved = range.MoveEndpointByUnit(WindowsTextPatternRangeEndpoint.Start, WindowsTextUnit.Character, requestedMovement);
        range.Select();
        await Assert.That(moved).IsEqualTo(requestedMovement);
        await Assert.That(range.GetText(-1)).IsEqualTo(string.Empty);
        await Assert.That(editor.SelectionStart).IsEqualTo(caretOffset);
        await Assert.That(editor.SelectionEnd).IsEqualTo(caretOffset);
    }

    /// <summary>Font sizes are expressed in typographic points and search respects formatting.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task FontSize_UsesPoints_AndFindsFormattedRun()
    {
        const int defaultFontSize = 16;
        const int fontSizeAttributeId = 40_006;
        const double expectedPointSize = 15.0;
        var editor = new RichTextBox { Text = "<p><span style='font-size:20px'>large</span> small</p>", FontSize = defaultFontSize };
        var window = new Window { Content = editor };
        using var context = new WindowsAutomationContext(window);
        var range = new WindowsTextRangeProvider(context.Node(new RichTextBoxAutomationPeer(editor)), 0, editor.Document.Length);
        var match = range.FindAttribute(fontSizeAttributeId, expectedPointSize, false);
        await Assert.That(match?.GetText(-1)).IsEqualTo("large");
        await Assert.That(match?.GetAttributeValue(fontSizeAttributeId)).IsEqualTo(expectedPointSize);
    }
}
