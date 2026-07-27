// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using CrissCross.Avalonia.UI.Extensions;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises visual-tree helper behavior with an in-memory control tree.</summary>
public sealed class AvaloniaExtensionCoverageTests
{
    /// <summary>Verifies child traversal and missing-parent fallback behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ControlExtensions_WhenWalkingSimpleControlTrees_ReturnExpectedResults()
    {
        var leaf = new TextBlock();
        var nestedPanel = new StackPanel();
        nestedPanel.Children.Add(leaf);
        var root = new StackPanel();
        root.Children.Add(nestedPanel);

        await Assert.That(root.FindChild(typeof(TextBlock))).IsSameReferenceAs(leaf);
        await Assert.That(leaf.FindParent(typeof(StackPanel))).IsSameReferenceAs(nestedPanel);
        await Assert.That(leaf.FindChild(typeof(TextBlock))).IsNull();
        await Assert.That(leaf.GetBoundsRelativeToRoot()).IsEqualTo(default(global::Avalonia.Rect));
    }
}
