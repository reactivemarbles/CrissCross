// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.Plot;
using PortableColorPicker = CrissCross.Reactive.Avalonia.UI.Controls.ColorPicker;
#else
using CrissCross.Avalonia.Plot;
using PortableColorPicker = CrissCross.Avalonia.UI.Controls.ColorPicker;
#endif

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Exercises typed UI editor bindings through the actual plot settings popup.</summary>
[NotInParallel]
public sealed class AvaloniaSettingsEditorTests
{
    /// <summary>The visible series selection.</summary>
    private const string VisibleSelection = "Visible";

    /// <summary>The accepted displayed point limit.</summary>
    private const int EditedPointCount = 25;

    /// <summary>Verifies popup toggles, numeric validation, and alpha-aware color swatches.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Popup_Editors_UpdateAndValidateSettings()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            using var vm = chart.ViewModel!;
            var key = new PlotSeriesKey("editable", 0);
            chart.ReactivePlotSources = [ReactivePlotSource.FromStatic(PlotSeriesData.Numeric(key.Name, 0, [1D], [1D]), PlotType.Line)];
            var window = new Window { Content = chart };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            var settingsButton = chart.FindControl<Button>("PlotSettings")!;
            Rect buttonArea = new(settingsButton.Bounds.Size);
            var location = settingsButton.TranslatePoint(buttonArea.Center, window)!.Value;
            PlotTestApp.SaveSnapshot(window, "settings-closed.png");
            window.MouseDown(location, MouseButton.Left);
            window.MouseUp(location, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            var popup = Find<Popup>(chart, static candidate => candidate.IsOpen);
            var content = popup.Child!;
            var settings = vm.SeriesSettings[0];
            Find<CheckBox>(content, static toggle => Equals(toggle.Content, VisibleSelection)).IsChecked = false;
            Find<CheckBox>(content, static toggle => Equals(toggle.Content, "Pause feed")).IsChecked = true;
            Find<CheckBox>(content, static toggle => Equals(toggle.Content, "Limit displayed points")).IsChecked = true;
            var editor = FindEditor(content, "Displayed point count");
            Edit(editor, "25");
            var editedCount = chart.NumberPointsPlotted;
            Edit(editor, "-1");
            var retainedCount = chart.NumberPointsPlotted;
            var swatch = Find<Button>(content, static button => AutomationProperties.GetName(button) == "Series color");
            var colorResult = EditColor(swatch, settings);
            var result = (colorResult, settings.IsVisible, settings.IsPaused, chart.UseFixedNumberOfPoints, editedCount, retainedCount,
                HasSwatch: swatch.Background is not null, ButtonWidth: settingsButton.Bounds.Width, ButtonHeight: settingsButton.Bounds.Height);
            window.Close();
            return result;
        });
        await Assert.That(observed.IsVisible).IsFalse();
        await Assert.That(observed.IsPaused).IsTrue();
        await Assert.That(observed.UseFixedNumberOfPoints).IsTrue();
        await Assert.That(observed.editedCount).IsEqualTo(EditedPointCount);
        await Assert.That(observed.retainedCount).IsEqualTo(EditedPointCount);
        await Assert.That(observed.HasSwatch).IsTrue();
        await Assert.That(observed.colorResult).IsTrue();
        await Assert.That(observed.ButtonWidth).IsGreaterThan(0);
        await Assert.That(observed.ButtonHeight).IsGreaterThan(0);
    }

    /// <summary>Verifies replacement models update the right-panel editors and detach prior handlers.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task RightPanel_ModelReplacement_UpdatesOnlyCurrentModel()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var panel = new RightPropertiesView();
            using var original = panel.ViewModel!;
            using var replacement = new RightPropertiesViewModel { ItemName = "replacement", LineWidth = 1, ItemVisibility = VisibleSelection, LineColor = "Red" };
            panel.ViewModel = replacement;
            var initialName = panel.ItemNameTextBox.Text;
            original.ItemName = "obsolete";
            panel.ItemNameTextBox.Text = "edited";
            panel.LineWidthNumberBox.Value = EditedPointCount;
            var knownColor = panel.ColorsComboBox.SelectedItem;
            replacement.LineColor = "#0C223880";
            var retainedColor = replacement.LineColor;
            return (retainedColor, ColorCount: panel.ColorsComboBox.Items.Count, Visibility: panel.VisibilityComboBox.SelectedItem, Color: knownColor, initialName,
                CurrentName: replacement.ItemName, replacement.LineWidth, PriorName: original.ItemName);
        });
        await Assert.That(observed.ColorCount).IsGreaterThan(0);
        await Assert.That(observed.Color).IsEqualTo("Red");
        await Assert.That(observed.retainedColor).IsEqualTo("#0C223880");
        await Assert.That(observed.Visibility).IsEqualTo(VisibleSelection);
        await Assert.That(observed.initialName).IsEqualTo("replacement");
        await Assert.That(observed.CurrentName).IsEqualTo("edited");
        await Assert.That(observed.LineWidth).IsEqualTo((double)EditedPointCount);
        await Assert.That(observed.PriorName).IsEqualTo("obsolete");
    }

    /// <summary>Commits an editor using the same focus transition as a user.</summary>
    /// <param name="editor">The numeric editor.</param>
    /// <param name="value">The entered text.</param>
    private static void Edit(TextBox editor, string value)
    {
        editor.Text = value;
        editor.RaiseEvent(new FocusChangedEventArgs(InputElement.LostFocusEvent));
        Dispatcher.UIThread.RunJobs();
    }

    /// <summary>Edits alpha and verifies the native flyout retains the selected color when reopened.</summary>
    /// <param name="swatch">The series color button.</param>
    /// <param name="settings">The bound settings.</param>
    /// <returns>Whether both source and reopened editor retain the color.</returns>
    private static bool EditColor(Button swatch, ReactivePlotSeriesSettings settings)
    {
        const byte alpha = 128;
        const byte red = 12;
        const byte green = 34;
        const byte blue = 56;
        var flyout = (Flyout)swatch.Flyout!;
        var picker = (PortableColorPicker)flyout.Content!;
        flyout.ShowAt(swatch);
        var selected = global::Avalonia.Media.Color.FromArgb(alpha, red, green, blue);
        picker.Color = selected;
        Dispatcher.UIThread.RunJobs();
        flyout.Hide();
        flyout.ShowAt(swatch);
        Dispatcher.UIThread.RunJobs();
        var retained = picker.Color == selected && picker.IsAlphaEnabled && settings.Color == new ScottPlot.Color(red, green, blue, alpha).ToHex();
        flyout.Hide();
        return retained;
    }

    /// <summary>Finds the text editor associated with a popup label.</summary>
    /// <param name="root">The popup content.</param>
    /// <param name="label">The visible editor label.</param>
    /// <returns>The associated editor.</returns>
    private static TextBox FindEditor(Control root, string label)
    {
        var text = Find<TextBlock>(root, candidate => candidate.Text == label);
        var parent = (Panel)text.Parent!;
        var seenLabel = false;
        foreach (var child in parent.Children)
        {
            seenLabel |= ReferenceEquals(child, text);
            if (seenLabel && child is TextBox editor)
            {
                return editor;
            }
        }

        throw new InvalidOperationException($"Missing editor: {label}");
    }

    /// <summary>Finds a descendant control matching a semantic predicate.</summary>
    /// <typeparam name="T">The desired control type.</typeparam>
    /// <param name="root">The content root.</param>
    /// <param name="predicate">The semantic match.</param>
    /// <returns>The matching control.</returns>
    private static T Find<T>(Control root, Func<T, bool> predicate)
        where T : Control
    {
        foreach (var child in root.GetLogicalDescendants())
        {
            if (child is T match && predicate(match))
            {
                return match;
            }
        }

        throw new InvalidOperationException($"Missing editor control: {typeof(T).Name}");
    }
}
