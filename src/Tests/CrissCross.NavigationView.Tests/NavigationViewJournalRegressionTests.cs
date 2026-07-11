// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Reflection;
using AvaloniaControls = CrissCross.Avalonia.UI.Controls;
using WpfControls = CrissCross.WPF.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Regression tests for platform navigation view journal moves.</summary>
public class NavigationViewJournalRegressionTests
{
    /// <summary>Provides the WpfGoForward_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WpfGoForward_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex()
    {
        var result = RunOnStaThread(() =>
        {
            var item = new WpfControls.NavigationViewItem(typeof(WpfTestPage));
            var navigationView = new TestWpfNavigationView();
            SeedWpfAlreadyCurrentJournalMove(navigationView, item, currentIndex: 0);
            var moved = navigationView.GoForward();
            return new JournalMoveResult(moved, navigationView.CanGoBack, navigationView.CanGoForward, navigationView.IsBackEnabled);
        });

        await Assert.That(result.Moved).IsTrue();
        await Assert.That(result.CanGoForward).IsFalse();
        await Assert.That(result.CanGoBack).IsTrue();
        await Assert.That(result.IsBackEnabled).IsTrue();
    }

    /// <summary>Provides the WpfGoBack_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WpfGoBack_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex()
    {
        var result = RunOnStaThread(() =>
        {
            var item = new WpfControls.NavigationViewItem(typeof(WpfTestPage));
            var navigationView = new TestWpfNavigationView();
            SeedWpfAlreadyCurrentJournalMove(navigationView, item, currentIndex: 1);
            var moved = navigationView.GoBack();
            return new JournalMoveResult(moved, navigationView.CanGoBack, navigationView.CanGoForward, navigationView.IsBackEnabled);
        });

        await Assert.That(result.Moved).IsTrue();
        await Assert.That(result.CanGoForward).IsTrue();
        await Assert.That(result.CanGoBack).IsFalse();
        await Assert.That(result.IsBackEnabled).IsFalse();
    }

    /// <summary>Provides the AvaloniaGoForward_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaGoForward_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex()
    {
        var item = new AvaloniaControls.NavigationViewItem(typeof(AvaloniaTestPage));
        var navigationView = new TestAvaloniaNavigationView();
        SeedAvaloniaAlreadyCurrentJournalMove(navigationView, item, currentIndex: 0);

        var moved = navigationView.GoForward();

        await Assert.That(moved).IsTrue();
        await Assert.That(navigationView.CanGoForward).IsFalse();
        await Assert.That(navigationView.CanGoBack).IsTrue();
        await Assert.That(navigationView.IsBackEnabled).IsTrue();
    }

    /// <summary>Provides the AvaloniaGoBack_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaGoBack_WhenRequestedJournalItemIsAlreadyStackTop_MovesJournalIndex()
    {
        var item = new AvaloniaControls.NavigationViewItem(typeof(AvaloniaTestPage));
        var navigationView = new TestAvaloniaNavigationView();
        SeedAvaloniaAlreadyCurrentJournalMove(navigationView, item, currentIndex: 1);

        var moved = navigationView.GoBack();

        await Assert.That(moved).IsTrue();
        await Assert.That(navigationView.CanGoForward).IsTrue();
        await Assert.That(navigationView.CanGoBack).IsFalse();
        await Assert.That(navigationView.IsBackEnabled).IsFalse();
    }

    /// <summary>Provides the SeedWpfAlreadyCurrentJournalMove member.</summary>
    /// <param name="navigationView">The navigationView value.</param>
    /// <param name="item">The item value.</param>
    /// <param name="currentIndex">The currentIndex value.</param>
    private static void SeedWpfAlreadyCurrentJournalMove(
        WpfControls.NavigationView navigationView,
        WpfControls.NavigationViewItem item,
        int currentIndex)
    {
        GetFieldValue<Dictionary<string, WpfControls.INavigationViewItem>>(
            navigationView,
            "_pageIdOrTargetTagNavigationViewsDictionary")[item.Id] = item;
        GetFieldValue<ObservableCollection<WpfControls.INavigationViewItem>>(
            navigationView,
            "_navigationStack").Add(item);
        SeedJournal(navigationView, item.Id, currentIndex);
    }

    /// <summary>Provides the SeedAvaloniaAlreadyCurrentJournalMove member.</summary>
    /// <param name="navigationView">The navigationView value.</param>
    /// <param name="item">The item value.</param>
    /// <param name="currentIndex">The currentIndex value.</param>
    private static void SeedAvaloniaAlreadyCurrentJournalMove(
        AvaloniaControls.NavigationView navigationView,
        AvaloniaControls.NavigationViewItem item,
        int currentIndex)
    {
        GetFieldValue<Dictionary<string, AvaloniaControls.INavigationViewItem>>(
            navigationView,
            "_pageIdOrTargetTagNavigationViewsDictionary")[item.Id] = item;
        GetFieldValue<ObservableCollection<AvaloniaControls.INavigationViewItem>>(
            navigationView,
            "_navigationStack").Add(item);
        SeedJournal(navigationView, item.Id, currentIndex);
    }

    /// <summary>Provides the SeedJournal member.</summary>
    /// <param name="navigationView">The navigationView value.</param>
    /// <param name="itemId">The itemId value.</param>
    /// <param name="currentIndex">The currentIndex value.</param>
    private static void SeedJournal(object navigationView, string itemId, int currentIndex)
    {
        var journal = GetFieldValue<List<string>>(navigationView, "_journal");
        journal.Add(itemId);
        journal.Add(itemId);
        SetCurrentIndex(navigationView, currentIndex);
    }

    /// <summary>Provides the GetFieldValue member.</summary>
    /// <typeparam name="T">The T type.</typeparam>
    /// <param name="target">The target value.</param>
    /// <param name="fieldName">The fieldName value.</param>
    /// <returns>The field value.</returns>
    private static T GetFieldValue<T>(object target, string fieldName)
    {
        var field = FindField(target.GetType(), fieldName);
        return (T)field.GetValue(target)!;
    }

    /// <summary>Provides the SetCurrentIndex member.</summary>
    /// <param name="navigationView">The navigationView value.</param>
    /// <param name="currentIndex">The currentIndex value.</param>
    private static void SetCurrentIndex(object navigationView, int currentIndex)
    {
        FindField(navigationView.GetType(), "_currentIndexInJournal").SetValue(navigationView, currentIndex);
    }

    /// <summary>Provides the RunOnStaThread member.</summary>
    /// <typeparam name="T">The T type.</typeparam>
    /// <param name="action">The action value.</param>
    /// <returns>The action result.</returns>
    private static T RunOnStaThread<T>(Func<T> action)
    {
        T? result = default;
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                result = action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception is not null)
        {
            throw exception;
        }

        return result!;
    }

    /// <summary>Provides the FindField member.</summary>
    /// <param name="type">The type value.</param>
    /// <param name="fieldName">The fieldName value.</param>
    /// <returns>The result.</returns>
    private static FieldInfo FindField(Type type, string fieldName)
    {
        var current = type;
        while (current is not null)
        {
            var field = current.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field is not null)
            {
                return field;
            }

            current = current.BaseType;
        }

        throw new MissingFieldException(type.FullName, fieldName);
    }

    /// <summary>Provides the TestWpfNavigationView member.</summary>
    private sealed class TestWpfNavigationView : WpfControls.NavigationView;

    /// <summary>Provides the TestAvaloniaNavigationView member.</summary>
    private sealed class TestAvaloniaNavigationView : AvaloniaControls.NavigationView;

    /// <summary>Provides the WpfTestPage member.</summary>
    private sealed class WpfTestPage : System.Windows.Controls.UserControl;

    /// <summary>Provides the AvaloniaTestPage member.</summary>
    private sealed class AvaloniaTestPage : global::Avalonia.Controls.UserControl;

    /// <summary>Provides the JournalMoveResult member.</summary>
    /// <param name="Moved">The Moved value.</param>
    /// <param name="CanGoBack">The CanGoBack value.</param>
    /// <param name="CanGoForward">The CanGoForward value.</param>
    /// <param name="IsBackEnabled">The IsBackEnabled value.</param>
    /// <returns>The result.</returns>
    private sealed record JournalMoveResult(bool Moved, bool CanGoBack, bool CanGoForward, bool IsBackEnabled);
}
