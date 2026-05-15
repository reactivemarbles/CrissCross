// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Reflection;
using AvaloniaControls = CrissCross.Avalonia.UI.Controls;
using WpfControls = CrissCross.WPF.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>
/// Regression tests for platform navigation view journal moves.
/// </summary>
public class NavigationViewJournalRegressionTests
{
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

    private sealed class TestWpfNavigationView : WpfControls.NavigationView;

    private sealed class TestAvaloniaNavigationView : AvaloniaControls.NavigationView;

    private sealed record JournalMoveResult(bool Moved, bool CanGoBack, bool CanGoForward, bool IsBackEnabled);

    private sealed class WpfTestPage : System.Windows.Controls.UserControl;

    private sealed class AvaloniaTestPage : global::Avalonia.Controls.UserControl;

    private static void SeedWpfAlreadyCurrentJournalMove(
        WpfControls.NavigationView navigationView,
        WpfControls.INavigationViewItem item,
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

    private static void SeedAvaloniaAlreadyCurrentJournalMove(
        AvaloniaControls.NavigationView navigationView,
        AvaloniaControls.INavigationViewItem item,
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

    private static void SeedJournal(object navigationView, string itemId, int currentIndex)
    {
        var journal = GetFieldValue<List<string>>(navigationView, "_journal");
        journal.Add(itemId);
        journal.Add(itemId);
        SetCurrentIndex(navigationView, currentIndex);
    }

    private static T GetFieldValue<T>(object target, string fieldName)
    {
        var field = FindField(target.GetType(), fieldName);
        return (T)field.GetValue(target)!;
    }

    private static void SetCurrentIndex(object navigationView, int currentIndex)
    {
        FindField(navigationView.GetType(), "_currentIndexInJournal").SetValue(navigationView, currentIndex);
    }

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
}
