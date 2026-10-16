// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Media;
using CrissCross.WPF.UI.Appearance;
using Brush = System.Windows.Media.Brush;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Verifies accent brushes reuse immutable snapshots and track color changes.</summary>
public class AccentBrushTests
{
    /// <summary>Verifies all accent getters reuse frozen brushes and observe updated resources.</summary>
    /// <param name="reactive">Whether to exercise the System.Reactive package family.</param>
    /// <returns>A task representing the asynchronous assertions.</returns>
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task AccentBrushes_ReuseFrozenSnapshotsUntilColorChanges(bool reactive)
    {
        var snapshot = await WpfTestDispatcher.RunAsync(() =>
        {
            _ = WpfTestDispatcher.GetApplication();
            var resources = reactive
                ? CrissCross.Reactive.WPF.UI.UiApplication.Current.Resources
                : UiApplication.Current.Resources;
            string[] keys = ["SystemAccentColor", "SystemAccentColorPrimary", "SystemAccentColorSecondary", "SystemAccentColorTertiary"];
            Func<Brush>[] getters = reactive ? [
                static () => CrissCross.Reactive.WPF.UI.Appearance.ApplicationAccentColorManager.SystemAccentBrush,
                static () => CrissCross.Reactive.WPF.UI.Appearance.ApplicationAccentColorManager.PrimaryAccentBrush,
                static () => CrissCross.Reactive.WPF.UI.Appearance.ApplicationAccentColorManager.SecondaryAccentBrush,
                static () => CrissCross.Reactive.WPF.UI.Appearance.ApplicationAccentColorManager.TertiaryAccentBrush] : [
                static () => ApplicationAccentColorManager.SystemAccentBrush,
                static () => ApplicationAccentColorManager.PrimaryAccentBrush,
                static () => ApplicationAccentColorManager.SecondaryAccentBrush,
                static () => ApplicationAccentColorManager.TertiaryAccentBrush];
            var independent = true;
            var current = true;
            for (var index = 0; index < keys.Length; index++)
            {
                var key = keys[index];
                var hadLocalValue = ContainsLocalResource(resources, key);
                var original = resources[key];
                try
                {
                    resources[key] = Colors.Red;
                    var first = (SolidColorBrush)getters[index]();
                    var second = (SolidColorBrush)getters[index]();
                    var clone = first.Clone();
                    clone.Color = Colors.Blue;
                    independent &= ReferenceEquals(first, second) && second.Color == Colors.Red && second.IsFrozen;
                    resources[key] = Colors.Green;
                    var updated = (SolidColorBrush)getters[index]();
                    current &= updated.Color == Colors.Green && updated.IsFrozen && !ReferenceEquals(updated, second) && second.Color == Colors.Red;
                }
                finally
                {
                    if (hadLocalValue)
                    {
                        resources[key] = original;
                    }
                    else
                    {
                        resources.Remove(key);
                    }
                }
            }

            return (Independent: independent, Current: current);
        });

        await Assert.That(snapshot.Independent).IsTrue();
        await Assert.That(snapshot.Current).IsTrue();
    }

    /// <summary>Checks local resources without including merged dictionaries.</summary>
    /// <param name="resources">The dictionary to inspect.</param>
    /// <param name="key">The resource key.</param>
    /// <returns>Whether the key belongs to this dictionary.</returns>
    private static bool ContainsLocalResource(ResourceDictionary resources, string key)
    {
        foreach (var resourceKey in resources.Keys)
        {
            if (Equals(resourceKey, key))
            {
                return true;
            }
        }

        return false;
    }
}
