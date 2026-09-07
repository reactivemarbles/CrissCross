// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CrissCross.Avalonia.UI.Appearance;
using Controls = CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies appearance and interaction brushes in attached button templates.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaButtonAppearanceTests
{
    /// <summary>Verifies every public appearance uses its intended palette across theme changes.</summary>
    /// <param name="appearance">The requested button appearance.</param>
    /// <param name="backgroundKey">The semantic background key or literal color.</param>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    [Arguments(Controls.ControlAppearance.Primary, "AccentButtonBackground")]
    [Arguments(Controls.ControlAppearance.Secondary, "ButtonBackground")]
    [Arguments(Controls.ControlAppearance.Info, "SystemFillColorAttention")]
    [Arguments(Controls.ControlAppearance.Success, "SystemFillColorSuccessBrush")]
    [Arguments(Controls.ControlAppearance.Caution, "SystemFillColorCautionBrush")]
    [Arguments(Controls.ControlAppearance.Danger, "SystemFillColorCriticalBrush")]
    [Arguments(Controls.ControlAppearance.Dark, "Black")]
    [Arguments(Controls.ControlAppearance.Light, "White")]
    [Arguments(Controls.ControlAppearance.Transparent, "Transparent")]
    public async Task AppearanceAndInteraction_UseConfiguredBrushes(Controls.ControlAppearance appearance, string backgroundKey)
    {
        var button = new InteractionButton { Content = "Operate", Appearance = appearance };
        var window = new Window { Content = button };
        try
        {
            window.Show();
            foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark, ApplicationThemeManager.HighContrastThemeVariant })
            {
                window.RequestedThemeVariant = variant;
                window.UpdateLayout();
                var expected = ResolveColor(backgroundKey, variant);
                await Assert.That(GetBrushColor(button.Background)).IsEqualTo(expected);
                var border = GetRootBorder(button);
                await Assert.That(GetBrushColor(border.Background)).IsEqualTo(expected);

                button.MouseOverBackground = Brushes.Magenta;
                button.MouseOverForeground = Brushes.White;
                button.PressedBackground = Brushes.Cyan;
                button.PressedForeground = Brushes.Black;
                button.SetInteractionState(true, false);
                await Assert.That(GetBrushColor(border.Background)).IsEqualTo(Colors.Magenta);
                await Assert.That(GetBrushColor(button.Foreground)).IsEqualTo(Colors.White);
                button.SetInteractionState(true, true);
                await Assert.That(GetBrushColor(border.Background)).IsEqualTo(Colors.Cyan);
                await Assert.That(GetBrushColor(button.Foreground)).IsEqualTo(Colors.Black);
                button.IsEnabled = false;
                await Assert.That(GetBrushColor(border.Background)).IsEqualTo(ResolveColor("ButtonBackgroundDisabled", variant));
                button.IsEnabled = true;
                button.SetInteractionState(false, false);
                await Assert.That(GetBrushColor(border.Background)).IsEqualTo(expected);
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies gel controls use contrasting text for each accent palette.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task GelButtons_UseThemeAwareAccentForeground()
    {
        const double StandardDisabledOpacity = 0.5;
        var gel = new Controls.GelButton { Content = "Operate" };
        var repeat = new Controls.GelRepeatButton { Content = "Repeat" };
        var toggle = new Controls.GelToggleButton { Content = "Toggle" };
        var window = new Window { Content = new StackPanel { Children = { gel, repeat, toggle } } };
        try
        {
            window.Show();
            foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark, ApplicationThemeManager.HighContrastThemeVariant })
            {
                window.RequestedThemeVariant = variant;
                window.UpdateLayout();
                var expected = ResolveColor("AccentButtonForeground", variant);
                await Assert.That(GetBrushColor(gel.Foreground)).IsEqualTo(expected);
                await Assert.That(GetBrushColor(gel.MouseOverForeground)).IsEqualTo(expected);
                await Assert.That(GetBrushColor(gel.PressedForeground)).IsEqualTo(expected);
                await Assert.That(GetBrushColor(repeat.Foreground)).IsEqualTo(expected);
                await Assert.That(GetBrushColor(toggle.Foreground)).IsEqualTo(expected);
                gel.IsEnabled = false;
                repeat.IsEnabled = false;
                toggle.IsEnabled = false;
                var expectedOpacity = variant == ApplicationThemeManager.HighContrastThemeVariant ? 1 : StandardDisabledOpacity;
                await Assert.That(gel.Opacity).IsEqualTo(expectedOpacity);
                await Assert.That(repeat.Opacity).IsEqualTo(expectedOpacity);
                await Assert.That(toggle.Opacity).IsEqualTo(expectedOpacity);
                gel.IsEnabled = true;
                repeat.IsEnabled = true;
                toggle.IsEnabled = true;
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies modern checkbox selection glyphs contrast with their accent background.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task ModernCheckBox_UsesThemeAwareSelectionGlyphs()
    {
        const double StandardDisabledOpacity = 0.5;
        const int SelectionGlyphCount = 2;
        var checkBox = new Controls.CheckBoxModern { Content = "Selected", IsChecked = true, IsThreeState = true };
        var window = new Window { Content = checkBox };
        try
        {
            window.Show();
            foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark, ApplicationThemeManager.HighContrastThemeVariant })
            {
                window.RequestedThemeVariant = variant;
                window.UpdateLayout();
                var glyphCount = 0;
                var expected = ResolveColor("AccentButtonForeground", variant);
                foreach (var descendant in checkBox.GetVisualDescendants())
                {
                    if (descendant is PathIcon { Name: "CheckGlyph" } checkGlyph)
                    {
                        await Assert.That(GetBrushColor(checkGlyph.Foreground)).IsEqualTo(expected);
                        glyphCount++;
                    }
                    else if (descendant is Border { Name: "IndeterminateGlyph" } indeterminateGlyph)
                    {
                        await Assert.That(GetBrushColor(indeterminateGlyph.Background)).IsEqualTo(expected);
                        glyphCount++;
                    }
                }

                await Assert.That(glyphCount).IsEqualTo(SelectionGlyphCount);
                checkBox.IsEnabled = false;
                var expectedOpacity = variant == ApplicationThemeManager.HighContrastThemeVariant ? 1 : StandardDisabledOpacity;
                await Assert.That(checkBox.Opacity).IsEqualTo(expectedOpacity);
                checkBox.IsEnabled = true;
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Resolves a semantic brush or a literal appearance color.</summary>
    /// <param name="key">The semantic key or color name.</param>
    /// <param name="variant">The active palette.</param>
    /// <returns>The resolved color.</returns>
    private static Color ResolveColor(string key, ThemeVariant variant)
    {
        if (Application.Current!.TryGetResource(key, variant, out var resource))
        {
            return resource is Color color ? color : GetBrushColor(resource as IBrush);
        }

        return Color.Parse(key);
    }

    /// <summary>Reads a rendered solid brush.</summary>
    /// <param name="brush">The rendered brush.</param>
    /// <returns>The solid color.</returns>
    private static Color GetBrushColor(IBrush? brush) =>
        brush is ISolidColorBrush solid ? solid.Color : throw new InvalidOperationException("The template requires a solid brush.");

    /// <summary>Locates the border that displays the button background.</summary>
    /// <param name="button">The attached button.</param>
    /// <returns>The rendered root border.</returns>
    private static Border GetRootBorder(Control button)
    {
        foreach (var descendant in button.GetVisualDescendants())
        {
            if (descendant is Border { Name: "RootBorder" } border)
            {
                return border;
            }
        }

        throw new InvalidOperationException("The button template did not render its root border.");
    }

    /// <summary>Exposes interaction pseudo-classes for deterministic template verification.</summary>
    private sealed class InteractionButton : Controls.Button
    {
        /// <summary>Sets pointer-over and pressed states without desktop input timing.</summary>
        /// <param name="pointerOver">Whether the pointer is over the button.</param>
        /// <param name="pressed">Whether the button is pressed.</param>
        public void SetInteractionState(bool pointerOver, bool pressed)
        {
            PseudoClasses.Set(":pointerover", pointerOver);
            PseudoClasses.Set(":pressed", pressed);
        }
    }
}
