// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using CrissCross.WPF.UI.Appearance;
using CrissCross.WPF.UI.Controls;
using CrissCross.WPF.UI.Markup;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using ContentPresenter = System.Windows.Controls.ContentPresenter;
using Control = System.Windows.Controls.Control;
using GradientBrush = System.Windows.Media.GradientBrush;
using GradientStopCollection = System.Windows.Media.GradientStopCollection;
using Orientation = System.Windows.Controls.Orientation;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using StackPanel = System.Windows.Controls.StackPanel;
using TextBlock = System.Windows.Controls.TextBlock;
using VisualTreeHelper = System.Windows.Media.VisualTreeHelper;
using WpfWindow = System.Windows.Window;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Verifies Bezel button text and face colors follow the active WPF UI theme.</summary>
public sealed class BezelButtonThemeTests
{
    /// <summary>The shared host height for template rendering.</summary>
    private const double HostHeight = 160D;

    /// <summary>The shared host width for template rendering.</summary>
    private const double HostWidth = 420D;

    /// <summary>The minimum expected text contrast ratio for normal text.</summary>
    private const double MinimumTextContrastRatio = 4.5D;

    /// <summary>The generated toggle button content text.</summary>
    private const string BezelToggleText = "Bezel toggle";

    /// <summary>The maximum value for an 8-bit color channel.</summary>
    private const double MaximumColorChannelValue = 255D;

    /// <summary>The luminance offset used by WCAG contrast calculations.</summary>
    private const double ContrastLuminanceOffset = 0.05D;

    /// <summary>The red channel relative luminance weight.</summary>
    private const double RedLuminanceWeight = 0.2126D;

    /// <summary>The green channel relative luminance weight.</summary>
    private const double GreenLuminanceWeight = 0.7152D;

    /// <summary>The blue channel relative luminance weight.</summary>
    private const double BlueLuminanceWeight = 0.0722D;

    /// <summary>The sRGB threshold below which the channel is linearized by division.</summary>
    private const double SrgbLinearThreshold = 0.03928D;

    /// <summary>The sRGB divisor for the linear segment.</summary>
    private const double SrgbLinearDivisor = 12.92D;

    /// <summary>The sRGB transfer function offset.</summary>
    private const double SrgbTransferOffset = 0.055D;

    /// <summary>The sRGB transfer function scale.</summary>
    private const double SrgbTransferScale = 1.055D;

    /// <summary>The sRGB transfer function exponent.</summary>
    private const double SrgbTransferExponent = 2.4D;

    /// <summary>Verifies each Bezel button type clears inherited text backgrounds and responds to theme changes.</summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task BezelButtons_WhenThemeChanges_UseTransparentTextBackgroundAndContrastingDefaultColors()
    {
        var snapshot = await RunOnStaThreadAsync(CaptureDefaultThemeBehavior);

        await Assert.That(snapshot.LightButtons.Count).IsEqualTo(snapshot.DarkButtons.Count);
        foreach (var button in snapshot.LightButtons)
        {
            await Assert.That(button.TextBlockBackgroundIsTransparent).IsTrue();
            await Assert.That(button.RaisedBorderPadding.Left).IsGreaterThan(0D);
            await Assert.That(button.RaisedBorderPadding).IsEqualTo(button.ControlPadding);
            await Assert.That(button.BackgroundLuminance).IsGreaterThan(button.ForegroundLuminance);
            await Assert.That(button.TextContrastRatio).IsGreaterThanOrEqualTo(MinimumTextContrastRatio);
        }

        foreach (var button in snapshot.DarkButtons)
        {
            await Assert.That(button.TextBlockBackgroundIsTransparent).IsTrue();
            await Assert.That(button.RaisedBorderPadding.Left).IsGreaterThan(0D);
            await Assert.That(button.RaisedBorderPadding).IsEqualTo(button.ControlPadding);
            await Assert.That(button.BackgroundLuminance).IsLessThan(button.ForegroundLuminance);
            await Assert.That(button.TextContrastRatio).IsGreaterThanOrEqualTo(MinimumTextContrastRatio);
        }

        for (var index = 0; index < snapshot.LightButtons.Count; index++)
        {
            await Assert.That(snapshot.LightButtons[index].BackgroundLuminance)
                .IsGreaterThan(snapshot.DarkButtons[index].BackgroundLuminance);
            await Assert.That(snapshot.DarkButtons[index].ForegroundLuminance)
                .IsGreaterThan(snapshot.LightButtons[index].ForegroundLuminance);
            await Assert.That(snapshot.LightButtons[index].BorderBrushLuminance)
                .IsGreaterThan(snapshot.DarkButtons[index].BorderBrushLuminance);
            await Assert.That(snapshot.LightButtons[index].InnerBorderBrushLuminance)
                .IsGreaterThan(snapshot.DarkButtons[index].InnerBorderBrushLuminance);
        }
    }

    /// <summary>Verifies explicit control brush overrides survive active theme changes.</summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task BezelButtons_WhenForegroundAndBackgroundAreOverridden_PreserveExplicitBrushes()
    {
        var snapshots = await RunOnStaThreadAsync(CaptureExplicitOverrideBehavior);

        foreach (var button in snapshots)
        {
            await Assert.That(button.BackgroundColor).IsEqualTo(Colors.MediumPurple);
            await Assert.That(button.ForegroundColor).IsEqualTo(Colors.Gold);
            await Assert.That(button.PresenterTextForegroundColor).IsEqualTo(Colors.Gold);
            await Assert.That(button.TextBlockForegroundColor).IsEqualTo(Colors.Gold);
            await Assert.That(button.TextBlockBackgroundIsTransparent).IsTrue();
            await Assert.That(button.RaisedBorderPadding).IsEqualTo(button.ControlPadding);
        }
    }

    /// <summary>Verifies local text-block brush overrides remain local when a Bezel button hosts explicit content.</summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task BezelButtons_WhenContentTextBlockOverridesBrushes_PreserveLocalTextBlockBrushes()
    {
        var snapshots = await RunOnStaThreadAsync(CaptureExplicitTextBlockOverrideBehavior);

        foreach (var button in snapshots)
        {
            await Assert.That(button.TextBlockBackgroundColor).IsEqualTo(Colors.Orange);
            await Assert.That(button.TextBlockForegroundColor).IsEqualTo(Colors.Cyan);
        }
    }

    /// <summary>Verifies BezelToggleButton keeps text transparent in every checked state.</summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task BezelToggleButton_WhenCheckedAndIndeterminate_KeepsTextBackgroundTransparent()
    {
        var snapshots = await RunOnStaThreadAsync(CaptureToggleStateBehavior);

        foreach (var button in snapshots)
        {
            await Assert.That(button.TextBlockBackgroundIsTransparent).IsTrue();
            await Assert.That(button.TextContrastRatio).IsGreaterThanOrEqualTo(MinimumTextContrastRatio);
        }
    }

    /// <summary>Captures the default Bezel button behavior under light and dark themes.</summary>
    /// <returns>The captured theme behavior.</returns>
    private static BezelThemeSnapshot CaptureDefaultThemeBehavior()
    {
        var host = CreateHostWindow();
        var themeDictionary = AddCrissCrossResources(host, ApplicationTheme.Light);
        var panel = CreatePanelWithAmbientTextBackground();
        BezelControlSpec[] specs = CreateDefaultSpecs();

        try
        {
            host.Content = panel;
            foreach (var spec in specs)
            {
                _ = panel.Children.Add(spec.Control);
            }

            host.Show();
            ApplyTheme(host, themeDictionary, ApplicationTheme.Light);
            var lightButtons = CaptureRenderedButtons(host, specs);

            ApplyTheme(host, themeDictionary, ApplicationTheme.Dark);
            var darkButtons = CaptureRenderedButtons(host, specs);

            return new(lightButtons, darkButtons);
        }
        finally
        {
            host.Close();
        }
    }

    /// <summary>Captures text rendering across checked, unchecked, and indeterminate toggle states.</summary>
    /// <returns>The captured toggle snapshots.</returns>
    private static List<BezelButtonSnapshot> CaptureToggleStateBehavior()
    {
        var host = CreateHostWindow();
        var themeDictionary = AddCrissCrossResources(host, ApplicationTheme.Light);
        var panel = CreatePanelWithAmbientTextBackground();
        BezelToggleButton toggleButton = new() { Content = BezelToggleText, IsThreeState = true };

        BezelControlSpec[] specs = [new(toggleButton)];
        List<BezelButtonSnapshot> snapshots = [];

        try
        {
            host.Content = panel;
            _ = panel.Children.Add(toggleButton);
            host.Show();
            ApplyTheme(host, themeDictionary, ApplicationTheme.Dark);

            toggleButton.IsChecked = false;
            snapshots.Add(CaptureRenderedButtons(host, specs)[0]);
            toggleButton.IsChecked = true;
            snapshots.Add(CaptureRenderedButtons(host, specs)[0]);
            toggleButton.IsChecked = null;
            snapshots.Add(CaptureRenderedButtons(host, specs)[0]);

            return snapshots;
        }
        finally
        {
            host.Close();
        }
    }

    /// <summary>Captures explicit Bezel foreground and background overrides after a theme switch.</summary>
    /// <returns>The captured override behavior.</returns>
    private static List<BezelButtonSnapshot> CaptureExplicitOverrideBehavior()
    {
        var host = CreateHostWindow();
        var themeDictionary = AddCrissCrossResources(host, ApplicationTheme.Light);
        var panel = CreatePanelWithAmbientTextBackground();
        BezelControlSpec[] specs = CreateDefaultSpecs();
        SolidColorBrush explicitBackground = new(Colors.MediumPurple);
        SolidColorBrush explicitForeground = new(Colors.Gold);

        try
        {
            host.Content = panel;
            foreach (var spec in specs)
            {
                spec.Control.Background = explicitBackground;
                spec.Control.Foreground = explicitForeground;
                _ = panel.Children.Add(spec.Control);
            }

            host.Show();
            ApplyTheme(host, themeDictionary, ApplicationTheme.Light);
            ApplyTheme(host, themeDictionary, ApplicationTheme.Dark);

            return CaptureRenderedButtons(host, specs);
        }
        finally
        {
            host.Close();
        }
    }

    /// <summary>Captures explicit text-block brush overrides after theme changes.</summary>
    /// <returns>The captured override behavior.</returns>
    private static List<BezelButtonSnapshot> CaptureExplicitTextBlockOverrideBehavior()
    {
        var host = CreateHostWindow();
        var themeDictionary = AddCrissCrossResources(host, ApplicationTheme.Light);
        var panel = CreatePanelWithAmbientTextBackground();
        BezelControlSpec[] specs = CreateExplicitTextBlockSpecs();

        try
        {
            host.Content = panel;
            foreach (var spec in specs)
            {
                _ = panel.Children.Add(spec.Control);
            }

            host.Show();
            ApplyTheme(host, themeDictionary, ApplicationTheme.Light);
            ApplyTheme(host, themeDictionary, ApplicationTheme.Dark);

            return CaptureRenderedButtons(host, specs);
        }
        finally
        {
            host.Close();
        }
    }

    /// <summary>Creates the concrete Bezel controls under test.</summary>
    /// <returns>The control specifications.</returns>
    private static BezelControlSpec[] CreateDefaultSpecs() =>
        [
            new(new BezelButton { Content = "Bezel button" }),
            new(new BezelRepeatButton { Content = "Bezel repeat" }),
            new(new BezelToggleButton { Content = BezelToggleText }),
        ];

    /// <summary>Creates the concrete Bezel controls with explicit text-block content under test.</summary>
    /// <returns>The control specifications.</returns>
    private static BezelControlSpec[] CreateExplicitTextBlockSpecs() =>
        [
            new(new BezelButton { Content = CreateOverriddenTextBlock("Bezel button") }),
            new(new BezelRepeatButton { Content = CreateOverriddenTextBlock("Bezel repeat") }),
            new(new BezelToggleButton { Content = CreateOverriddenTextBlock(BezelToggleText) }),
        ];

    /// <summary>Creates an explicit text block whose local brushes should not be replaced by the template style.</summary>
    /// <param name="text">The text content.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateOverriddenTextBlock(string text) =>
        new() { Background = Brushes.Orange, Foreground = Brushes.Cyan, Text = text };

    /// <summary>Creates the host window used to realize templates.</summary>
    /// <returns>The host window.</returns>
    private static WpfWindow CreateHostWindow() =>
        new() { Height = HostHeight, ShowInTaskbar = false, Width = HostWidth, WindowStartupLocation = WindowStartupLocation.Manual };

    /// <summary>Adds the CrissCross controls and selected theme dictionaries to the host resources.</summary>
    /// <param name="host">The host window.</param>
    /// <param name="theme">The initial theme.</param>
    /// <returns>The mutable theme dictionary.</returns>
    private static ThemesDictionary AddCrissCrossResources(WpfWindow host, ApplicationTheme theme)
    {
        ThemesDictionary themes = new() { Theme = theme };
        host.Resources.MergedDictionaries.Add(new ControlsDictionary());
        host.Resources.MergedDictionaries.Add(themes);
        host.SetResourceReference(Control.BackgroundProperty, "ApplicationBackgroundBrush");
        return themes;
    }

    /// <summary>Creates a panel that reproduces the ambient text background leak from the gallery screenshot.</summary>
    /// <returns>The panel.</returns>
    private static StackPanel CreatePanelWithAmbientTextBackground()
    {
        StackPanel panel = new() { Orientation = Orientation.Horizontal };
        Style ambientTextBlockStyle = new(typeof(TextBlock));
        ambientTextBlockStyle.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.Fuchsia));
        panel.Resources.Add(typeof(TextBlock), ambientTextBlockStyle);
        return panel;
    }

    /// <summary>Applies a theme and drains pending WPF work.</summary>
    /// <param name="host">The host window.</param>
    /// <param name="themeDictionary">The mutable theme dictionary.</param>
    /// <param name="theme">The theme to apply.</param>
    private static void ApplyTheme(WpfWindow host, ThemesDictionary themeDictionary, ApplicationTheme theme)
    {
        themeDictionary.Theme = theme;
        DrainDispatcher();
        host.UpdateLayout();
    }

    /// <summary>Captures rendered template brush state for all Bezel controls.</summary>
    /// <param name="host">The host window.</param>
    /// <param name="specs">The rendered control specifications.</param>
    /// <returns>The captured snapshots.</returns>
    private static List<BezelButtonSnapshot> CaptureRenderedButtons(
        WpfWindow host,
        IEnumerable<BezelControlSpec> specs)
    {
        List<BezelButtonSnapshot> snapshots = [];
        var pageBackground = GetRequiredSolidColor(host.Background).Color;
        foreach (var spec in specs)
        {
            _ = spec.Control.ApplyTemplate();
            var presenter = (ContentPresenter?)spec.Control.Template.FindName("Content", spec.Control);
            var raisedBorder = (RaisedBorder?)spec.Control.Template.FindName("PART_RaisedBorder", spec.Control)
                ?? throw new InvalidOperationException("Expected Bezel button template to render a RaisedBorder.");
            var textBlock = FindDescendant<TextBlock>(spec.Control)
                ?? throw new InvalidOperationException("Expected Bezel button content to render a TextBlock.");
            var backgroundColor = GetRequiredSolidColor(spec.Control.Background).Color;
            var foregroundColor = GetRequiredSolidColor(spec.Control.Foreground).Color;
            var renderedBackgroundColor = CompositeOver(backgroundColor, pageBackground);
            var renderedForegroundColor = CompositeOver(foregroundColor, renderedBackgroundColor);

            snapshots.Add(
                new(
                    backgroundColor,
                    foregroundColor,
                    GetRequiredSolidColor(TextElement.GetForeground(presenter!)).Color,
                    GetRequiredSolidColor(textBlock.Foreground).Color,
                    GetBrushColor(textBlock.Background),
                    spec.Control.Padding,
                    raisedBorder.Padding,
                    IsTransparentBrush(textBlock.Background),
                    GetRelativeLuminance(renderedBackgroundColor),
                    GetRelativeLuminance(renderedForegroundColor),
                    GetContrastRatio(renderedForegroundColor, renderedBackgroundColor),
                    GetRelativeLuminance(GetAverageBrushColor(raisedBorder.BorderBrush)),
                    GetRelativeLuminance(GetAverageBrushColor(raisedBorder.MinorBorderBrush1))));
        }

        return snapshots;
    }

    /// <summary>Finds the first visual descendant with the requested type.</summary>
    /// <typeparam name="T">The requested descendant type.</typeparam>
    /// <param name="root">The visual root.</param>
    /// <returns>The descendant when found; otherwise, <see langword="null"/>.</returns>
    private static T? FindDescendant<T>(DependencyObject root)
        where T : DependencyObject
    {
        var childCount = VisualTreeHelper.GetChildrenCount(root);
        for (var index = 0; index < childCount; index++)
        {
            var child = VisualTreeHelper.GetChild(root, index);
            if (child is T typedChild)
            {
                return typedChild;
            }

            var descendant = FindDescendant<T>(child);
            if (descendant is not null)
            {
                return descendant;
            }
        }

        return null;
    }

    /// <summary>Gets a required solid-color brush.</summary>
    /// <param name="brush">The brush to convert.</param>
    /// <returns>The solid-color brush.</returns>
    private static SolidColorBrush GetRequiredSolidColor(Brush brush) =>
        brush as SolidColorBrush ?? throw new InvalidOperationException("Expected a solid color brush.");

    /// <summary>Gets the color from an optional solid-color brush.</summary>
    /// <param name="brush">The brush.</param>
    /// <returns>The brush color.</returns>
    private static Color? GetBrushColor(Brush? brush) =>
        brush is SolidColorBrush solidColorBrush ? solidColorBrush.Color : null;

    /// <summary>Gets an average representative color for a theme brush.</summary>
    /// <param name="brush">The brush.</param>
    /// <returns>The average brush color.</returns>
    private static Color GetAverageBrushColor(Brush brush) =>
        brush switch
        {
            SolidColorBrush solidColorBrush => solidColorBrush.Color,
            GradientBrush gradientBrush => AverageGradientStopColors(gradientBrush.GradientStops),
            _ => throw new InvalidOperationException("Expected a solid or gradient brush."),
        };

    /// <summary>Averages gradient stop colors.</summary>
    /// <param name="gradientStops">The gradient stops.</param>
    /// <returns>The average color.</returns>
    private static Color AverageGradientStopColors(GradientStopCollection gradientStops)
    {
        if (gradientStops.Count == 0)
        {
            throw new InvalidOperationException("Expected at least one gradient stop.");
        }

        var alpha = 0D;
        var red = 0D;
        var green = 0D;
        var blue = 0D;
        foreach (var stop in gradientStops)
        {
            alpha += stop.Color.A;
            red += stop.Color.R;
            green += stop.Color.G;
            blue += stop.Color.B;
        }

        return Color.FromArgb(
            (byte)Math.Round(alpha / gradientStops.Count, MidpointRounding.AwayFromZero),
            (byte)Math.Round(red / gradientStops.Count, MidpointRounding.AwayFromZero),
            (byte)Math.Round(green / gradientStops.Count, MidpointRounding.AwayFromZero),
            (byte)Math.Round(blue / gradientStops.Count, MidpointRounding.AwayFromZero));
    }

    /// <summary>Determines whether a brush is transparent.</summary>
    /// <param name="brush">The brush to test.</param>
    /// <returns><see langword="true"/> when the brush is transparent.</returns>
    private static bool IsTransparentBrush(Brush? brush) =>
        brush is null or SolidColorBrush { Color.A: 0 };

    /// <summary>Calculates the contrast ratio between two composited colors.</summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns>The contrast ratio.</returns>
    private static double GetContrastRatio(Color foreground, Color background)
    {
        var foregroundLuminance = GetRelativeLuminance(foreground);
        var backgroundLuminance = GetRelativeLuminance(background);
        var lighter = Math.Max(foregroundLuminance, backgroundLuminance);
        var darker = Math.Min(foregroundLuminance, backgroundLuminance);
        return (lighter + ContrastLuminanceOffset) / (darker + ContrastLuminanceOffset);
    }

    /// <summary>Composites a foreground color over a background color.</summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns>The composited color.</returns>
    private static Color CompositeOver(Color foreground, Color background)
    {
        var alpha = foreground.A / MaximumColorChannelValue;
        return Color.FromRgb(
            CompositeChannel(foreground.R, background.R, alpha),
            CompositeChannel(foreground.G, background.G, alpha),
            CompositeChannel(foreground.B, background.B, alpha));
    }

    /// <summary>Composites one color channel.</summary>
    /// <param name="foreground">The foreground channel value.</param>
    /// <param name="background">The background channel value.</param>
    /// <param name="alpha">The foreground alpha.</param>
    /// <returns>The composited channel value.</returns>
    private static byte CompositeChannel(byte foreground, byte background, double alpha) =>
        (byte)Math.Round((foreground * alpha) + (background * (1D - alpha)), MidpointRounding.AwayFromZero);

    /// <summary>Calculates the relative luminance for color comparison.</summary>
    /// <param name="color">The color.</param>
    /// <returns>The relative luminance.</returns>
    private static double GetRelativeLuminance(Color color) =>
        (RedLuminanceWeight * ToLinearColorChannel(color.R))
        + (GreenLuminanceWeight * ToLinearColorChannel(color.G))
        + (BlueLuminanceWeight * ToLinearColorChannel(color.B));

    /// <summary>Converts an RGB channel to linear color space.</summary>
    /// <param name="channel">The channel value.</param>
    /// <returns>The linearized channel value.</returns>
    private static double ToLinearColorChannel(byte channel)
    {
        var value = channel / MaximumColorChannelValue;
        return value <= SrgbLinearThreshold
            ? value / SrgbLinearDivisor
            : Math.Pow((value + SrgbTransferOffset) / SrgbTransferScale, SrgbTransferExponent);
    }

    /// <summary>Drains pending dispatcher work.</summary>
    private static void DrainDispatcher() =>
        Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ApplicationIdle);

    /// <summary>Runs WPF work on an STA thread.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The WPF action.</param>
    /// <returns>A task that completes with the action result.</returns>
    private static Task<TResult> RunOnStaThreadAsync<TResult>(Func<TResult> action)
    {
        TaskCompletionSource<TResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Thread thread = new(
            () =>
            {
                try
                {
                    completion.SetResult(action());
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }

    /// <summary>Stores a concrete Bezel control under test.</summary>
    /// <param name="Control">The control.</param>
    private sealed record BezelControlSpec(Control Control);

    /// <summary>Captures default Bezel button rendering under both standard themes.</summary>
    /// <param name="LightButtons">The light-theme button snapshots.</param>
    /// <param name="DarkButtons">The dark-theme button snapshots.</param>
    private sealed record BezelThemeSnapshot(
        List<BezelButtonSnapshot> LightButtons,
        List<BezelButtonSnapshot> DarkButtons);

    /// <summary>Captures the brush state for one rendered Bezel button.</summary>
    /// <param name="BackgroundColor">The control background color.</param>
    /// <param name="ForegroundColor">The control foreground color.</param>
    /// <param name="PresenterTextForegroundColor">The content presenter foreground color.</param>
    /// <param name="TextBlockForegroundColor">The rendered text block foreground color.</param>
    /// <param name="TextBlockBackgroundColor">The rendered text block background color.</param>
    /// <param name="ControlPadding">The control padding.</param>
    /// <param name="RaisedBorderPadding">The template raised-border padding.</param>
    /// <param name="TextBlockBackgroundIsTransparent">Whether the rendered text block background is transparent.</param>
    /// <param name="BackgroundLuminance">The control background luminance.</param>
    /// <param name="ForegroundLuminance">The control foreground luminance.</param>
    /// <param name="TextContrastRatio">The composited contrast ratio between the foreground and button background.</param>
    /// <param name="BorderBrushLuminance">The template border brush luminance.</param>
    /// <param name="InnerBorderBrushLuminance">The template inner border brush luminance.</param>
    private sealed record BezelButtonSnapshot(
        Color BackgroundColor,
        Color ForegroundColor,
        Color PresenterTextForegroundColor,
        Color TextBlockForegroundColor,
        Color? TextBlockBackgroundColor,
        Thickness ControlPadding,
        Thickness RaisedBorderPadding,
        bool TextBlockBackgroundIsTransparent,
        double BackgroundLuminance,
        double ForegroundLuminance,
        double TextContrastRatio,
        double BorderBrushLuminance,
        double InnerBorderBrushLuminance);
}
