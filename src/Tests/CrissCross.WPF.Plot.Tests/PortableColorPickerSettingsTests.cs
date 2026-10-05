// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using ScottPlot.Plottables;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
using CrissCross.Reactive.WPF.UI;
using CrissCross.Reactive.WPF.UI.UIExtensions;
using ReactiveUI.Reactive;
using ReactiveUI.Reactive.Builder;
#else
using CrissCross.WPF.Plot;
using CrissCross.WPF.UI;
using CrissCross.WPF.UI.UIExtensions;
using ReactiveUI;
using ReactiveUI.Builder;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Verifies plot settings color pickers keep reactive and legacy colors synchronized.</summary>
[NotInParallel]
public sealed class PortableColorPickerSettingsTests
{
    /// <summary>The reactive series name.</summary>
    private const string ReactiveSeriesName = "Reactive series";

    /// <summary>The legacy series name.</summary>
    private const string LegacySeriesName = "Legacy series";

    /// <summary>The edited alpha channel.</summary>
    private const byte EditedAlpha = 128;

    /// <summary>The edited red channel.</summary>
    private const byte EditedRed = 18;

    /// <summary>The edited green channel.</summary>
    private const byte EditedGreen = 52;

    /// <summary>The edited blue channel.</summary>
    private const byte EditedBlue = 86;

    /// <summary>The hue used by the picker control test.</summary>
    private const double SelectedHue = 120D;

    /// <summary>The saturation used by the picker control test.</summary>
    private const double SelectedSaturation = 50D;

    /// <summary>The value used by the picker control test.</summary>
    private const double SelectedValue = 80D;

    /// <summary>The expected red channel after HSV conversion.</summary>
    private const byte ExpectedRed = 102;

    /// <summary>The expected green channel after HSV conversion.</summary>
    private const byte ExpectedGreen = 204;

    /// <summary>The expected blue channel after HSV conversion.</summary>
    private const byte ExpectedBlue = 102;

    /// <summary>The next reactive sample.</summary>
    private const double NextSample = 2D;

    /// <summary>The next reactive update sequence.</summary>
    private const long NextSequence = 2L;

    /// <summary>Verifies the color picker initializes from named and hexadecimal setting values.</summary>
    /// <param name="color">The configured color.</param>
    /// <param name="red">The expected red channel.</param>
    /// <param name="green">The expected green channel.</param>
    /// <param name="blue">The expected blue channel.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments("Blue", 0, 0, 255)]
    [Arguments("#123456", 18, 52, 86)]
    public async Task ReactiveColorPicker_InitialColor_UsesCurrentSetting(string color, byte red, byte green, byte blue)
    {
        var selected = await OnSta((color, red, green, blue), static (chart, state) =>
        {
            using var updates = CreateReactiveSeries(chart);
            var settings = chart.ViewModel!.SeriesSettings[0];
            settings.Color = state.color;

            OpenSettings(chart);
            var expander = FindExpander(Popup(chart).Child, ReactiveSeriesName);
            expander.IsExpanded = true;
            DrainDispatcher();
            var colorPicker = FindColorPicker(expander);
            return colorPicker.SelectedColor;
        });

        await Assert.That(selected).IsEqualTo(Color.FromRgb(red, green, blue));
    }

    /// <summary>Verifies picker edits update the rendered reactive series and survive data and popup rebuilds.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task ReactiveColorPicker_SelectedColor_UpdatesSeriesAndPersists()
    {
        var observed = await OnSta(ExerciseReactiveColorSelection);

        var expectedWpfColor = Color.FromArgb(EditedAlpha, ExpectedRed, ExpectedGreen, ExpectedBlue);
        var expectedScottPlotColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromArgb(EditedAlpha, ExpectedRed, ExpectedGreen, ExpectedBlue));
        await Assert.That(observed.selected).IsEqualTo(expectedWpfColor);
        await Assert.That(observed.standardSelected).IsEqualTo(expectedWpfColor);
        await Assert.That(observed.updatedSetting).IsEqualTo(expectedScottPlotColor.ToHex());
        await Assert.That(observed.updatedPlotColor).IsEqualTo(expectedScottPlotColor);
        await Assert.That(observed.reopened).IsEqualTo(expectedWpfColor);
        await Assert.That(observed.FollowingData).IsEqualTo(expectedScottPlotColor);
        await Assert.That(observed.squareStateBindingPreserved).IsTrue();
        await Assert.That(observed.hueBindingPreserved).IsTrue();
        await Assert.That(observed.saturationBindingPreserved).IsTrue();
        await Assert.That(observed.valueBindingPreserved).IsTrue();
        await Assert.That(observed.alphaBindingPreserved).IsTrue();
        await Assert.That(observed.nestedStateBindingPreserved).IsTrue();
        await Assert.That(observed.selectedColorBindingPreserved).IsTrue();
    }

    /// <summary>Verifies the legacy series color picker updates its setting and preserves both bindings.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task LegacyColorPicker_SelectedColor_UpdatesChartSettings()
    {
        var observed = await OnSta(static chart =>
        {
            chart.ViewModel!.InitializeLinesForSignalPoints((LegacySeriesName, [1D], [1D], 0));
            var settings = chart.ViewModel.PlotLinesCollectionUI[0].ChartSettings;
            settings.Color = "Red";

            OpenSettings(chart);
            var expander = FindExpander(Popup(chart).Child, LegacySeriesName);
            expander.IsExpanded = true;
            DrainDispatcher();
            var colorPicker = FindColorPicker(expander);
            var initializedColor = colorPicker.SelectedColor;
            var standardPicker = OpenStandardPicker(colorPicker);
            standardPicker.Color.A = EditedAlpha;
            standardPicker.Color.RGB_R = EditedRed;
            standardPicker.Color.RGB_G = EditedGreen;
            standardPicker.Color.RGB_B = EditedBlue;
            DrainDispatcher();

            return (
                initializedColor,
                settings.Color,
                colorPicker.SelectedColor,
                OuterBindingPreserved: BindingOperations.IsDataBound(colorPicker, PickerControlBase.SelectedColorProperty),
                InnerBindingPreserved: BindingOperations.IsDataBound(standardPicker, PickerControlBase.ColorStateProperty));
        });

        var expected = ScottPlot.Color.FromColor(System.Drawing.Color.FromArgb(EditedAlpha, EditedRed, EditedGreen, EditedBlue));
        await Assert.That(observed.initializedColor).IsEqualTo(Colors.Red);
        await Assert.That(observed.Color).IsEqualTo(expected.ToHex());
        await Assert.That(observed.SelectedColor).IsEqualTo(Color.FromArgb(EditedAlpha, EditedRed, EditedGreen, EditedBlue));
        await Assert.That(observed.OuterBindingPreserved).IsTrue();
        await Assert.That(observed.InnerBindingPreserved).IsTrue();
    }

    /// <summary>Verifies the picker converter rejects invalid setting values without changing the binding.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task ColorConverter_InvalidValues_ReturnBindingSentinels()
    {
        var observed = await OnSta(static chart =>
        {
            using var updates = CreateReactiveSeries(chart);
            OpenSettings(chart);
            var expander = FindExpander(Popup(chart).Child, ReactiveSeriesName);
            expander.IsExpanded = true;
            DrainDispatcher();
            var colorPicker = FindColorPicker(expander);
            var binding = BindingOperations.GetBinding(colorPicker, PickerControlBase.SelectedColorProperty)
                ?? throw new InvalidOperationException("Expected the picker color binding.");
            var converter = binding.Converter ?? throw new InvalidOperationException("Expected the plot color converter.");
            var targetType = typeof(Color);
            var culture = CultureInfo.InvariantCulture;
            var invalidObject = new object();
            return (
                NullValue: converter.Convert(null!, targetType, null, culture),
                ObjectValue: converter.Convert(invalidObject, targetType, null, culture),
                WhitespaceValue: converter.Convert(" ", targetType, null, culture),
                NonColorValue: converter.ConvertBack(invalidObject, typeof(string), null, culture));
        });

        await Assert.That(observed.NullValue).IsSameReferenceAs(DependencyProperty.UnsetValue);
        await Assert.That(observed.ObjectValue).IsSameReferenceAs(DependencyProperty.UnsetValue);
        await Assert.That(observed.WhitespaceValue).IsSameReferenceAs(DependencyProperty.UnsetValue);
        await Assert.That(observed.NonColorValue).IsSameReferenceAs(Binding.DoNothing);
    }

    /// <summary>Exercises one reactive picker edit and its persisted state.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The selected colors, setting values, and binding states.</returns>
    private static (
        Color selected,
        Color standardSelected,
        string updatedSetting,
        ScottPlot.Color updatedPlotColor,
        Color reopened,
        ScottPlot.Color FollowingData,
        bool squareStateBindingPreserved,
        bool hueBindingPreserved,
        bool saturationBindingPreserved,
        bool valueBindingPreserved,
        bool alphaBindingPreserved,
        bool nestedStateBindingPreserved,
        bool selectedColorBindingPreserved) ExerciseReactiveColorSelection(LiveChart chart)
    {
        using var updates = CreateReactiveSeries(chart);
        var settings = chart.ViewModel!.SeriesSettings[0];
        settings.Color = "Blue";
        var colorPicker = OpenReactiveColorPicker(chart);
        var edit = ChangeReactiveColorWithSliders(colorPicker);
        var updatedPlotColor = GetScatter(chart).LineColor;
        var reopened = ReopenReactiveColorPicker(chart, colorPicker);
        updates.OnNext(new(new(ReactiveSeriesName, 0), PlotType.Line, ReactivePlotUpdateKind.Append, [NextSample], [NextSample], PlotXAxisKind.Numeric, NextSequence));
        DrainDispatcher();

        return (
            edit.selected,
            edit.standardSelected,
            settings.Color,
            updatedPlotColor,
            reopened,
            FollowingData: GetScatter(chart).LineColor,
            edit.squareStateBindingPreserved,
            edit.hueBindingPreserved,
            edit.saturationBindingPreserved,
            edit.valueBindingPreserved,
            edit.alphaBindingPreserved,
            edit.nestedStateBindingPreserved,
            edit.selectedColorBindingPreserved);
    }

    /// <summary>Opens the reactive settings editor for the test series.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The series color picker.</returns>
    private static PortableColorPicker OpenReactiveColorPicker(LiveChart chart)
    {
        OpenSettings(chart);
        var expander = FindExpander(Popup(chart).Child, ReactiveSeriesName);
        expander.IsExpanded = true;
        DrainDispatcher();
        return FindColorPicker(expander);
    }

    /// <summary>Changes a reactive series color through the nested picker sliders.</summary>
    /// <param name="colorPicker">The portable picker.</param>
    /// <returns>The color values and binding states after selection.</returns>
    private static (
        Color selected,
        Color standardSelected,
        bool squareStateBindingPreserved,
        bool hueBindingPreserved,
        bool saturationBindingPreserved,
        bool valueBindingPreserved,
        bool alphaBindingPreserved,
        bool nestedStateBindingPreserved,
        bool selectedColorBindingPreserved) ChangeReactiveColorWithSliders(PortableColorPicker colorPicker)
    {
        var standardPicker = OpenStandardPicker(colorPicker);
        var squarePicker = FindDescendant<SquarePicker>(standardPicker) ?? throw new InvalidOperationException("Expected the standard picker's square color editor.");
        var hueSlider = FindDescendant<HueSlider>(squarePicker) ?? throw new InvalidOperationException("Expected the hue slider.");
        var squareSlider = FindDescendant<SquareSlider>(squarePicker) ?? throw new InvalidOperationException("Expected the saturation and value slider.");
        var alphaSlider = FindDescendant<RgbColorSlider>(standardPicker, static slider => slider.SliderArgbType == "A") ?? throw new InvalidOperationException("Expected the alpha slider.");
        hueSlider.SetCurrentValue(HueSlider.ValueProperty, SelectedHue);
        squareSlider.SetCurrentValue(SquareSlider.HeadXProperty, SelectedSaturation);
        squareSlider.SetCurrentValue(SquareSlider.HeadYProperty, SelectedValue);
        alphaSlider.SetCurrentValue(Slider.ValueProperty, (double)EditedAlpha);
        DrainDispatcher();

        return (
            colorPicker.SelectedColor,
            standardPicker.SelectedColor,
            BindingOperations.IsDataBound(squarePicker, PickerControlBase.ColorStateProperty),
            BindingOperations.IsDataBound(hueSlider, HueSlider.ValueProperty),
            BindingOperations.IsDataBound(squareSlider, SquareSlider.HeadXProperty),
            BindingOperations.IsDataBound(squareSlider, SquareSlider.HeadYProperty),
            BindingOperations.IsDataBound(alphaSlider, Slider.ValueProperty),
            BindingOperations.IsDataBound(standardPicker, PickerControlBase.ColorStateProperty),
            BindingOperations.IsDataBound(colorPicker, PickerControlBase.SelectedColorProperty));
    }

    /// <summary>Reopens the outer popup and reads the selected reactive color.</summary>
    /// <param name="chart">The chart.</param>
    /// <param name="colorPicker">The currently open picker.</param>
    /// <returns>The color restored by the rebuilt editor.</returns>
    private static Color ReopenReactiveColorPicker(LiveChart chart, PortableColorPicker colorPicker)
    {
        PickerToggle(colorPicker).IsChecked = false;
        Click(chart.PlotSettings);
        Click(chart.PlotSettings);
        var expander = FindExpander(Popup(chart).Child, ReactiveSeriesName);
        expander.IsExpanded = true;
        DrainDispatcher();
        return FindColorPicker(expander).SelectedColor;
    }

    /// <summary>Creates a reactive line feed attached to the chart.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The owned update stream.</returns>
    private static Signal<ReactivePlotUpdate> CreateReactiveSeries(LiveChart chart)
    {
        var updates = new Signal<ReactivePlotUpdate>();
        chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(new(ReactiveSeriesName, 0), PlotType.Line, updates)];
        updates.OnNext(new(new(ReactiveSeriesName, 0), PlotType.Line, ReactivePlotUpdateKind.Append, [1D], [1D], PlotXAxisKind.Numeric, 1));
        DrainDispatcher();
        return updates;
    }

    /// <summary>Gets the rendered reactive scatter line.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The rendered line.</returns>
    private static Scatter GetScatter(LiveChart chart)
    {
        foreach (var plottable in chart.ViewModel!.WpfPlot1vm!.Plot.PlottableList)
        {
            if (plottable is Scatter scatter)
            {
                return scatter;
            }
        }

        throw new InvalidOperationException("Expected the reactive series scatter line.");
    }

    /// <summary>Opens the chart settings popup.</summary>
    /// <param name="chart">The chart.</param>
    private static void OpenSettings(LiveChart chart)
    {
        Click(chart.PlotSettings);
        DrainDispatcher();
    }

    /// <summary>Finds the settings popup.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The popup.</returns>
    private static Popup Popup(LiveChart chart) => SettingsPopup(chart) ?? throw new InvalidOperationException("The plot settings popup is not open.");

    /// <summary>Reads the popup field without runtime reflection.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The popup field reference.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_settingsPopup")]
    private static extern ref Popup? SettingsPopup(LiveChart chart);

    /// <summary>Reads the nested standard picker without runtime reflection.</summary>
    /// <param name="picker">The portable picker.</param>
    /// <returns>The nested standard picker field reference.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "test")]
    private static extern ref StandardColorPicker StandardPicker(PortableColorPicker picker);

    /// <summary>Reads the picker toggle without runtime reflection.</summary>
    /// <param name="picker">The portable picker.</param>
    /// <returns>The nested toggle field reference.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "toggleButton")]
    private static extern ref ToggleButton PickerToggle(PortableColorPicker picker);

    /// <summary>Opens the nested standard picker UI.</summary>
    /// <param name="picker">The portable picker.</param>
    /// <returns>The initialized standard picker.</returns>
    private static StandardColorPicker OpenStandardPicker(PortableColorPicker picker)
    {
        PickerToggle(picker).IsChecked = true;
        DrainDispatcher();
        return StandardPicker(picker);
    }

    /// <summary>Finds a color picker inside a settings expander.</summary>
    /// <param name="root">The expander.</param>
    /// <returns>The picker.</returns>
    private static PortableColorPicker FindColorPicker(DependencyObject root) =>
        FindDescendant<PortableColorPicker>(root) ?? throw new InvalidOperationException("Expected a portable color picker.");

    /// <summary>Finds a named expander in the popup tree.</summary>
    /// <param name="root">The popup content.</param>
    /// <param name="header">The expander header.</param>
    /// <returns>The matching expander.</returns>
    private static Expander FindExpander(DependencyObject root, string header) =>
        FindDescendant<Expander>(root, expander => Equals(expander.Header, header)) ?? throw new InvalidOperationException($"Expander '{header}' was not found.");

    /// <summary>Finds a descendant in the WPF logical tree.</summary>
    /// <typeparam name="T">The requested control type.</typeparam>
    /// <param name="root">The tree root.</param>
    /// <param name="predicate">An optional filter.</param>
    /// <returns>The matching descendant.</returns>
    private static T? FindDescendant<T>(DependencyObject root, Func<T, bool>? predicate = null)
        where T : DependencyObject
    {
        if (root is T match && (predicate is null || predicate(match)))
        {
            return match;
        }

        foreach (var child in LogicalTreeHelper.GetChildren(root))
        {
            if (child is DependencyObject dependency && FindDescendant(dependency, predicate) is { } result)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>Raises a real button click and drains queued UI work.</summary>
    /// <param name="button">The button.</param>
    private static void Click(Button button)
    {
        button.RaiseEvent(new(ButtonBase.ClickEvent));
        DrainDispatcher();
    }

    /// <summary>Processes queued UI work.</summary>
    private static void DrainDispatcher() => Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ApplicationIdle);

    /// <summary>Runs chart interactions on a Windows STA dispatcher.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The chart interaction.</param>
    /// <returns>The interaction result.</returns>
    private static Task<T> OnSta<T>(Func<LiveChart, T> action)
    {
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            T result = default!;
            Exception? failure = null;
            var previous = RxSchedulers.MainThreadScheduler;
            try
            {
                _ = Dispatcher.CurrentDispatcher;
                _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
                RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
                var chart = new LiveChart { LegendPosition = LegendPosition.Right };
                chart.ViewModel!.YAxesSetup((["Test"], ["#FFFFFF"]));
                var window = new Window { Content = chart, ShowInTaskbar = false };
                try
                {
                    window.Show();
                    DrainDispatcher();
                    result = action(chart);
                }
                finally
                {
                    if (SettingsPopup(chart) is { } popup)
                    {
                        popup.IsOpen = false;
                    }

                    window.Content = null;
                    window.Close();
                    chart.ViewModel?.Dispose();
                }
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                RxSchedulers.MainThreadScheduler = previous;
            }

            if (failure is null)
            {
                _ = completion.TrySetResult(result);
            }
            else
            {
                _ = completion.TrySetException(failure);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }

    /// <summary>Runs chart work on an STA thread while passing immutable test data without a closure.</summary>
    /// <typeparam name="TState">The test data type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="state">The test data.</param>
    /// <param name="action">The chart interaction.</param>
    /// <returns>The interaction result.</returns>
    private static Task<TResult> OnSta<TState, TResult>(TState state, Func<LiveChart, TState, TResult> action)
    {
        var completion = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            TResult result = default!;
            Exception? failure = null;
            var previous = RxSchedulers.MainThreadScheduler;
            try
            {
                _ = Dispatcher.CurrentDispatcher;
                _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
                RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
                var chart = new LiveChart { LegendPosition = LegendPosition.Right };
                chart.ViewModel!.YAxesSetup((["Test"], ["#FFFFFF"]));
                var window = new Window { Content = chart, ShowInTaskbar = false };
                try
                {
                    window.Show();
                    DrainDispatcher();
                    result = action(chart, state);
                }
                finally
                {
                    if (SettingsPopup(chart) is { } popup)
                    {
                        popup.IsOpen = false;
                    }

                    window.Content = null;
                    window.Close();
                    chart.ViewModel?.Dispose();
                }
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                RxSchedulers.MainThreadScheduler = previous;
            }

            if (failure is null)
            {
                _ = completion.TrySetResult(result);
            }
            else
            {
                _ = completion.TrySetException(failure);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }
}
