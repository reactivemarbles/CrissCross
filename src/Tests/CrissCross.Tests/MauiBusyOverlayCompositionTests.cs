// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Verifies the MAUI busy overlay composes native visual content around consumer content.</summary>
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class MauiBusyOverlayCompositionTests
{
    /// <summary>Provides the determinate progress used by the busy operation test.</summary>
    private const double DeterminateProgress = 0.4D;

    /// <summary>Provides the expected single template part count.</summary>
    private const int ExpectedSingleCount = 1;

    /// <summary>Provides the expected root child count.</summary>
    private const int ExpectedRootChildCount = 2;

    /// <summary>Provides the operation title used by busy overlay tests.</summary>
    private const string OperationTitle = "Saving";

    /// <summary>Provides the operation message used by busy overlay tests.</summary>
    private const string OperationMessage = "Writing values";

    /// <summary>Gets a debugger-safe representation of this test fixture.</summary>
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Verifies the default control template preserves consumer content and layers a non-transparent blocker above it.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Template_PreservesConsumerContentAndCreatesBusyOnlyBlockingLayer()
    {
        var coveredContent = new Label { Text = "Covered content" };
        var overlay = new BusyOverlay { Content = coveredContent };
        var root = CreateTemplateRoot(overlay);
        var blockingLayer = GetBlockingLayer(root);

        await Assert.That(overlay.Content).IsEqualTo(coveredContent);
        await Assert.That(root.Children.Count).IsEqualTo(ExpectedRootChildCount);
        await Assert.That(CountChildrenOfType<ContentPresenter>(root)).IsEqualTo(ExpectedSingleCount);
        await Assert.That(ReferenceEquals(FindChild<ContentPresenter>(root).Content, coveredContent)).IsTrue();
        await Assert.That(blockingLayer.InputTransparent).IsFalse();
        await Assert.That(blockingLayer.IsVisible).IsFalse();
        await Assert.That(FindSingleDescendant<ActivityIndicator>(root).IsRunning).IsFalse();
    }

    /// <summary>Verifies determinate operations show progress, text, and a command-backed cancel action.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Template_DeterminateOperationShowsProgressTextAndCancelCommand()
    {
        var command = new TrackingCommand();
        var overlay = new BusyOverlay { Operation = new(OperationTitle, OperationMessage, DeterminateProgress, command) };
        var root = CreateTemplateRoot(overlay);
        var labels = FindDescendants<Label>(root);
        var progress = FindSingleDescendant<ProgressBar>(root);
        var activity = FindSingleDescendant<ActivityIndicator>(root);
        var cancel = FindSingleDescendant<Button>(root);

        cancel.Command?.Execute(null);

        await Assert.That(overlay.IsBusy).IsTrue();
        await Assert.That(SemanticProperties.GetDescription(overlay)).IsEqualTo("Saving Writing values");
        await Assert.That(labels.Exists(static label => label.Text == OperationTitle)).IsTrue();
        await Assert.That(labels.Exists(static label => label.Text == OperationMessage)).IsTrue();
        await Assert.That(progress.IsVisible).IsTrue();
        await Assert.That(progress.Progress).IsEqualTo(DeterminateProgress);
        await Assert.That(activity.IsVisible).IsFalse();
        await Assert.That(activity.IsRunning).IsFalse();
        await Assert.That(cancel.IsVisible).IsTrue();
        await Assert.That(cancel.Text).IsEqualTo("Cancel");
        await Assert.That(command.ExecutionCount).IsEqualTo(ExpectedSingleCount);
    }

    /// <summary>Verifies indeterminate operations show the activity indicator and hide optional determinate/cancel controls.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Template_IndeterminateOperationShowsSpinnerAndHidesOptionalControls()
    {
        var overlay = new BusyOverlay { Operation = new(OperationTitle) };
        var root = CreateTemplateRoot(overlay);
        var progress = FindSingleDescendant<ProgressBar>(root);
        var activity = FindSingleDescendant<ActivityIndicator>(root);
        var cancel = FindSingleDescendant<Button>(root);
        var message = FindDescendants<Label>(root).Find(static label => label.Text?.Length == 0) ?? throw new InvalidOperationException("Expected an empty message label.");

        await Assert.That(activity.IsVisible).IsTrue();
        await Assert.That(activity.IsRunning).IsTrue();
        await Assert.That(progress.IsVisible).IsFalse();
        await Assert.That(cancel.IsVisible).IsFalse();
        await Assert.That(message.IsVisible).IsFalse();
        overlay.Operation = new(" ");
        await Assert.That(activity.IsRunning).IsFalse();
        await Assert.That(overlay.IsBusy).IsFalse();
        await Assert.That(SemanticProperties.GetDescription(overlay)).IsEqualTo("Idle");
    }

    /// <summary>Creates the root visual from an overlay template.</summary>
    /// <param name="overlay">The overlay under test.</param>
    /// <returns>The template root grid.</returns>
    private static Grid CreateTemplateRoot(BusyOverlay overlay) =>
        (Grid)GetSingleVisualChild(overlay);

    /// <summary>Gets the overlay layer that blocks covered content while busy.</summary>
    /// <param name="root">The template root.</param>
    /// <returns>The blocking layer.</returns>
    private static Grid GetBlockingLayer(Grid root) =>
        FindChild<Grid>(root);

    /// <summary>Finds a single descendant of the requested type.</summary>
    /// <typeparam name="T">The descendant type.</typeparam>
    /// <param name="root">The root element.</param>
    /// <returns>The matching descendant.</returns>
    private static T FindSingleDescendant<T>(Element root)
        where T : Element =>
        GetSingle(FindDescendants<T>(root));

    /// <summary>Counts direct visual children of the requested type.</summary>
    /// <typeparam name="T">The child type.</typeparam>
    /// <param name="root">The root layout.</param>
    /// <returns>The number of matching direct children.</returns>
    private static int CountChildrenOfType<T>(Layout root)
        where T : Element
    {
        var count = 0;
        foreach (var child in root.Children)
        {
            if (child is T)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Finds the single direct visual child of the requested type.</summary>
    /// <typeparam name="T">The child type.</typeparam>
    /// <param name="root">The root layout.</param>
    /// <returns>The matching direct child.</returns>
    private static T FindChild<T>(Layout root)
        where T : Element
    {
        var matches = new List<T>();
        foreach (var child in root.Children)
        {
            if (child is T typedChild)
            {
                matches.Add(typedChild);
            }
        }

        return GetSingle(matches);
    }

    /// <summary>Gets the only visual child from a templated element.</summary>
    /// <param name="element">The visual tree element.</param>
    /// <returns>The only child element.</returns>
    private static IVisualTreeElement GetSingleVisualChild(IVisualTreeElement element)
    {
        IVisualTreeElement? match = null;
        foreach (var child in element.GetVisualChildren())
        {
            if (match is not null)
            {
                throw new InvalidOperationException("Expected a single visual child.");
            }

            match = child;
        }

        return match ?? throw new InvalidOperationException("Expected a visual child.");
    }

    /// <summary>Gets a single item from a list.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items to inspect.</param>
    /// <returns>The only item.</returns>
    private static T GetSingle<T>(List<T> items)
    {
        if (items.Count != ExpectedSingleCount)
        {
            throw new InvalidOperationException("Expected a single matching item.");
        }

        return items[0];
    }

    /// <summary>Finds descendants of the requested type.</summary>
    /// <typeparam name="T">The descendant type.</typeparam>
    /// <param name="root">The root element.</param>
    /// <returns>The matching descendants.</returns>
    private static List<T> FindDescendants<T>(Element root)
        where T : Element
    {
        var matches = new List<T>();
        AddDescendants(root, matches);
        return matches;
    }

    /// <summary>Adds descendants of the requested type to the supplied list.</summary>
    /// <typeparam name="T">The descendant type.</typeparam>
    /// <param name="element">The element to inspect.</param>
    /// <param name="matches">The collected matches.</param>
    private static void AddDescendants<T>(Element element, ICollection<T> matches)
        where T : Element
    {
        if (element is T match)
        {
            matches.Add(match);
        }

        if (element is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is not Element childElement)
                {
                    continue;
                }

                AddDescendants(childElement, matches);
            }
        }

        if (element is Border { Content: Element borderContent })
        {
            AddDescendants(borderContent, matches);
        }

        if (element is not ContentView { Content: Element content })
        {
            return;
        }

        AddDescendants(content, matches);
    }

    /// <summary>Tracks command executions from the busy overlay cancel button.</summary>
    private sealed class TrackingCommand : ICommand
    {
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <summary>Gets the number of executions.</summary>
        public int ExecutionCount { get; private set; }

        /// <inheritdoc />
        public bool CanExecute(object? parameter) => true;

        /// <inheritdoc />
        public void Execute(object? parameter)
        {
            ExecutionCount++;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
