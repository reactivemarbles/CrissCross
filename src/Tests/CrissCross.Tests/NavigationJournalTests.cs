// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Tests;

/// <summary>
/// Tests for platform-neutral navigation journal operations used by UI navigation controls.
/// </summary>
public class NavigationJournalTests
{
    [Test]
    public async Task Record_BackAndForward_PreservesForwardEntryUntilNewNavigation()
    {
        var journal = new List<string>();
        var currentIndex = -1;

        NavigationJournal.Record(journal, ref currentIndex, "home");
        NavigationJournal.Record(journal, ref currentIndex, "orders");
        NavigationJournal.Record(journal, ref currentIndex, "details");

        var canMoveBack = NavigationJournal.TryMoveBack(journal, currentIndex, out var backIndex, out var backEntryId);
        var canMoveForward = NavigationJournal.TryMoveForward(journal, backIndex, out var forwardIndex, out var forwardEntryId);

        await Assert.That(canMoveBack).IsTrue();
        await Assert.That(backIndex).IsEqualTo(1);
        await Assert.That(backEntryId).IsEqualTo("orders");
        await Assert.That(NavigationJournal.CanGoForward(journal, backIndex)).IsTrue();
        await Assert.That(canMoveForward).IsTrue();
        await Assert.That(forwardIndex).IsEqualTo(2);
        await Assert.That(forwardEntryId).IsEqualTo("details");
    }

    [Test]
    public async Task Record_AfterBack_TruncatesForwardEntries()
    {
        var journal = new List<string>();
        var currentIndex = -1;

        NavigationJournal.Record(journal, ref currentIndex, "home");
        NavigationJournal.Record(journal, ref currentIndex, "orders");
        NavigationJournal.Record(journal, ref currentIndex, "details");
        _ = NavigationJournal.TryMoveBack(journal, currentIndex, out currentIndex, out _);

        NavigationJournal.Record(journal, ref currentIndex, "settings");

        await Assert.That(currentIndex).IsEqualTo(2);
        await Assert.That(journal).IsEquivalentTo(["home", "orders", "settings"]);
        await Assert.That(NavigationJournal.CanGoForward(journal, currentIndex)).IsFalse();
    }

    [Test]
    public async Task TryMoveBack_AtFirstEntry_ReturnsFalseAndKeepsIndex()
    {
        var journal = new List<string>();
        var currentIndex = -1;

        NavigationJournal.Record(journal, ref currentIndex, "home");

        var canMoveBack = NavigationJournal.TryMoveBack(journal, currentIndex, out var backIndex, out var backEntryId);

        await Assert.That(canMoveBack).IsFalse();
        await Assert.That(backIndex).IsEqualTo(0);
        await Assert.That(backEntryId).IsNull();
    }
}
