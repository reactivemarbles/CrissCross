// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using CrissCross.Avalonia.UI.Controls;
using CrissCrossExpander = CrissCross.Avalonia.UI.Controls.Expander;

namespace CrissCross.NavigationView.Tests;

/// <summary>Covers the constructible Avalonia control catalog's public initialization paths.</summary>
public sealed class AvaloniaControlCoverageTests
{
    /// <summary>The number of controls in the lightweight construction catalog.</summary>
    private const int ControlCount = 14;

    /// <summary>Verifies lightweight controls can be constructed without a visual host.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ConstructibleControls_WhenCreated_ExposeNativeAvaloniaControlBase()
    {
        Control[] controls =
        [
            new AppBar(),
            new Arc(),
            new Badge(),
            new BezelButton(),
            new Card(),
            new CardAction(),
            new CardColor(),
            new CardControl(),
            new CardExpander(),
            new Chip(),
            new ChipGroup(),
            new EmptyState(),
            new CrissCrossExpander(),
            new FilterBar(),
        ];

        await Assert.That(controls.Length).IsEqualTo(ControlCount);
        foreach (var control in controls)
        {
            await Assert.That(control).IsNotNull();
        }
    }

    /// <summary>Verifies every public parameterless Avalonia styled element can be constructed from the UI assembly.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PublicParameterlessStyledElements_WhenConstructed_ProduceStyledElements()
    {
        var assembly = typeof(AppBar).Assembly;
        var constructedCount = 0;
        var roundTrippedPropertyCount = 0;

        foreach (var type in assembly.GetExportedTypes())
        {
            if (type.IsAbstract || !typeof(StyledElement).IsAssignableFrom(type) || type.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            var element = (StyledElement)Activator.CreateInstance(type)!;
            await Assert.That(element is StyledElement).IsTrue();
            constructedCount++;

            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (!IsRoundTrippableCrissCrossProperty(property))
                {
                    continue;
                }

                var value = property.GetValue(element);
                property.SetValue(element, value);
                roundTrippedPropertyCount++;
            }
        }

        await Assert.That(constructedCount).IsGreaterThan(ControlCount);
        await Assert.That(roundTrippedPropertyCount).IsGreaterThan(0);
    }

    /// <summary>Verifies card, badge, and chip public styled properties retain state and derived flags.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CommonControls_WhenStyledPropertiesChange_ExposeExpectedState()
    {
        var badge = new Badge { Appearance = ControlAppearance.Success };
        var card = new Card { Header = "header", Footer = "footer" };
        var chip = new Chip { Icon = "icon", IsRemovable = true, IsSelected = true, Text = "chip" };

        await Assert.That(badge.Appearance).IsEqualTo(ControlAppearance.Success);
        await Assert.That(card.HasHeader).IsTrue();
        await Assert.That(card.HasFooter).IsTrue();
        await Assert.That(chip.Text).IsEqualTo("chip");
        await Assert.That(chip.Icon).IsEqualTo("icon");
        await Assert.That(chip.IsSelected).IsTrue();
        await Assert.That(chip.IsRemovable).IsTrue();
    }

    /// <summary>Determines whether a property can safely be round-tripped by the construction catalog.</summary>
    /// <param name="property">The candidate property.</param>
    /// <returns><see langword="true" /> when the property belongs to CrissCross and can be read and written.</returns>
    private static bool IsRoundTrippableCrissCrossProperty(System.Reflection.PropertyInfo property) =>
        property is { CanRead: true, CanWrite: true }
        && property.GetIndexParameters().Length == 0
        && property.DeclaringType?.Namespace?.StartsWith("CrissCross.", StringComparison.Ordinal) == true;
}
