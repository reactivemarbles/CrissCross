// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
#if REACTIVELIST_REACTIVE
using AppBarButton = CrissCross.Reactive.WPF.UI.Controls.AppBarButton;
using AppBarIcons = CrissCross.Reactive.WPF.UI.Controls.AppBarIcons;
using PortableColorPicker = CrissCross.Reactive.WPF.UI.PortableColorPicker;
#else
using AppBarButton = CrissCross.WPF.UI.Controls.AppBarButton;
using AppBarIcons = CrissCross.WPF.UI.Controls.AppBarIcons;
using PortableColorPicker = CrissCross.WPF.UI.PortableColorPicker;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Provides the plot configuration popup.</summary>
public partial class LiveChart
{
    /// <summary>The popup content padding.</summary>
    private const double PopupPadding = 16;

    /// <summary>The popup content width.</summary>
    private const double PopupWidth = 420;

    /// <summary>The color picker swatch height.</summary>
    private const double ColorPickerHeight = 32;

    /// <summary>The minimum popup height.</summary>
    private const double MinimumPopupHeight = 220;

    /// <summary>The space reserved for the toolbar.</summary>
    private const double PopupHeightInset = 100;

    /// <summary>The space between settings sections.</summary>
    private const double SectionSpacing = 10;

    /// <summary>The numeric input spacing.</summary>
    private const double InputSpacing = 6;

    /// <summary>The space below labels.</summary>
    private const double LabelSpacing = 2;

    /// <summary>The space below descriptions.</summary>
    private const double DescriptionSpacing = 4;

    /// <summary>The popup background red and green channels.</summary>
    private const byte PopupBackgroundRedGreen = 37;

    /// <summary>The popup background blue channel.</summary>
    private const byte PopupBackgroundBlue = 38;

    /// <summary>The label for pointer coordinate features.</summary>
    private const string PointerValuesLabel = "Pointer values";

    /// <summary>The popup actions whose icons reflect their current state.</summary>
    private readonly List<(AppBarButton Button, Func<AppBarIcons> Icon)> _settingsActions = [];

    /// <summary>The currently open settings popup.</summary>
    private Popup? _settingsPopup;

    /// <summary>The time window used to group incoming reactive updates.</summary>
    private double _batchWindowMilliseconds;

    /// <summary>The maximum number of updates in one batch.</summary>
    private int _maximumBatchSize = 1;

    /// <summary>The strategy used when retained data exceeds its limit.</summary>
    private ReactivePlotOverflowStrategy _overflowStrategy;

    /// <summary>The policy used for invalid reactive updates.</summary>
    private ReactivePlotErrorMode _errorMode;

    /// <summary>Returns choices for a settings enumeration.</summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <returns>The available values.</returns>
    private static T[] GetSettingsChoices<T>()
        where T : struct, Enum =>
#if NET6_0_OR_GREATER
        Enum.GetValues<T>();
#else
        (T[])Enum.GetValues(typeof(T));
#endif

    /// <summary>Adds an unbound numeric value input.</summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="label">The value label.</param>
    /// <param name="value">The initial value.</param>
    /// <returns>The value editor.</returns>
    private static TextBox AddValueInput(Panel panel, string label, double value)
    {
        _ = panel.Children.Add(new TextBlock { Text = label, Foreground = Brushes.White, Margin = new(0, InputSpacing, 0, LabelSpacing) });
        var input = new TextBox { Text = value.ToString(CultureInfo.CurrentCulture), ToolTip = label };
        _ = panel.Children.Add(input);
        return input;
    }

    /// <summary>Reads a finite numeric editor value.</summary>
    /// <param name="input">The numeric editor.</param>
    /// <param name="value">The parsed value.</param>
    /// <returns>Whether the input is a finite number.</returns>
    private static bool TryReadFinite(TextBox input, out double value) =>
        double.TryParse(input.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
        && !double.IsNaN(value) && !double.IsInfinity(value);

    /// <summary>Adds a control description.</summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="label">The control label.</param>
    /// <param name="description">The control description.</param>
    private static void AddDescription(Panel panel, string label, string description)
    {
        if (label.Length > 0)
        {
            _ = panel.Children.Add(new TextBlock { Text = label, Foreground = Brushes.White, FontWeight = FontWeights.SemiBold, Margin = new(0, SectionSpacing, 0, LabelSpacing) });
        }

        _ = panel.Children.Add(new TextBlock { Text = description, Foreground = Brushes.LightGray, TextWrapping = TextWrapping.Wrap, Margin = new(0, 0, 0, DescriptionSpacing) });
    }

    /// <summary>Adds a bound toggle.</summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="label">The toggle label.</param>
    /// <param name="description">The toggle description.</param>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The source property.</param>
    private static void AddBoundToggle(Panel panel, string label, string description, object source, string property)
    {
        AddDescription(panel, label, description);
        var toggle = new CheckBox { Content = label, Foreground = Brushes.White, ToolTip = description };
        var binding = CreateSettingsBinding(source, property);
        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        _ = toggle.SetBinding(ToggleButton.IsCheckedProperty, binding);
        _ = panel.Children.Add(toggle);
    }

    /// <summary>Adds a bound value editor.</summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="label">The editor label.</param>
    /// <param name="description">The editor description.</param>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The source property.</param>
    private static void AddBoundEditor(Panel panel, string label, string description, object source, string property)
    {
        AddDescription(panel, label, description);
        var editor = new TextBox { ToolTip = description, Margin = new(0, 0, 0, DescriptionSpacing) };
        _ = editor.SetBinding(TextBox.TextProperty, CreateSettingsBinding(source, property));
        _ = panel.Children.Add(editor);
    }

    /// <summary>Adds a color swatch that opens the portable color picker.</summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="source">The series settings.</param>
    private static void AddBoundColorPicker(Panel panel, object source)
    {
        const string description = "Select the color swatch to choose a series color and transparency. Changes apply immediately.";
        AddDescription(panel, "Color", description);
        var picker = new PortableColorPicker { Height = ColorPickerHeight, ToolTip = description, Margin = new(0, 0, 0, DescriptionSpacing), ShowAlpha = true };
        System.Windows.Automation.AutomationProperties.SetName(picker, "Series color");
        var binding = new Binding(nameof(ChartObjects.Color))
        {
            Source = source,
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Converter = new PlotSettingsColorConverter(),
        };
        _ = picker.SetBinding(PortableColorPicker.SelectedColorProperty, binding);
        _ = panel.Children.Add(picker);
    }

    /// <summary>Adds a bound choice editor.</summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="label">The choice label.</param>
    /// <param name="description">The choice description.</param>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The source property.</param>
    /// <param name="choices">The available choices.</param>
    private static void AddBoundChoice(Panel panel, string label, string description, object source, string property, Array choices)
    {
        AddDescription(panel, label, description);
        var editor = new ComboBox { ItemsSource = choices, ToolTip = description };
        var binding = CreateSettingsBinding(source, property);
        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        _ = editor.SetBinding(Selector.SelectedItemProperty, binding);
        _ = panel.Children.Add(editor);
    }

    /// <summary>Creates an immediate, validated settings binding.</summary>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The source property.</param>
    /// <returns>The settings binding.</returns>
    private static Binding CreateSettingsBinding(object source, string property)
    {
        var binding = new Binding(property)
        {
            Source = source,
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
            ValidatesOnExceptions = true,
            ValidatesOnDataErrors = true,
        };
        if (property is nameof(NumberPointsPlotted) or nameof(ChartObjects.LineWidth) or nameof(ChartObjects.Color))
        {
            binding.ValidationRules.Add(new SettingsValueValidationRule(property));
        }

        return binding;
    }

    /// <summary>Adds supported appearance and streaming editors.</summary>
    /// <param name="series">The series editor panel.</param>
    /// <param name="settings">The attached series settings.</param>
    private static void AddReactiveAppearanceEditors(Panel series, ReactivePlotSeriesSettings settings)
    {
        AddBoundEditor(series, "Series name", "The label shown in the series selector and plot legend; the feed identity remains stable.", settings, nameof(ReactivePlotSeriesSettings.SeriesLabel));
        AddBoundColorPicker(series, settings);
        if (settings.PlotType != PlotType.Bar)
        {
            AddBoundEditor(series, "Line width", "Stroke thickness in pixels. Zero hides connecting lines.", settings, nameof(ReactivePlotSeriesSettings.LineWidth));
        }

        if (settings.PlotType == PlotType.Streamer)
        {
            AddBoundChoice(
                series,
                "Streaming display",
                "Scroll the sample window left or right, or overwrite a fixed window with a sweep moving left or right.",
                settings,
                nameof(ReactivePlotSeriesSettings.StreamerViewMode),
                GetSettingsChoices<StreamerViewMode>());
            AddBoundEditor(series, "Sample interval", "Positive numeric X-axis distance between streamed samples.", settings, nameof(ReactivePlotSeriesSettings.SamplePeriod));
        }

        if (settings.PlotType != PlotType.Bar)
        {
            AddBoundEditor(series, "Marker size", "Size of point symbols in pixels. Zero hides point symbols.", settings, nameof(ReactivePlotSeriesSettings.MarkerSize));
            AddBoundChoice(
                series,
                "Line and points",
                "Connect samples, show individual points, combine both, or hide both.",
                settings,
                nameof(ReactivePlotSeriesSettings.LineMode),
                GetSettingsChoices<PlotLineMode>());
        }

        if (settings.PlotType == PlotType.Area)
        {
            AddBoundChoice(
                series,
                "Baseline mode",
                "Fill the area down to zero or a custom Y reference, or turn off the fill.",
                settings,
                nameof(ReactivePlotSeriesSettings.BaselineMode),
                GetSettingsChoices<PlotBaselineMode>());
            AddBoundEditor(series, "Custom baseline", "The reference Y value used when the baseline mode is Custom.", settings, nameof(ReactivePlotSeriesSettings.Baseline));
        }

        AddBoundEditor(
            series,
            "Retained points",
            "Maximum retained samples. Empty uses source retention. Resizing a streamer clears its sample buffer.",
            settings,
            nameof(ReactivePlotSeriesSettings.MaxPoints));
    }

    /// <summary>Adds a described AppBar action.</summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="label">The action label.</param>
    /// <param name="description">The action description.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="stateIcon">The icon for the current feature state.</param>
    private void AddAction(Panel panel, string label, string description, Action action, Func<AppBarIcons>? stateIcon = null)
    {
        var icon = label switch
        {
            "Follow / explore data" => AppBarIcons.Md_lock_open,
            PointerValuesLabel => AppBarIcons.Md_crosshairs,
            "Fit axes now" => AppBarIcons.Md_fit_to_screen,
            "Light / dark plot" => AppBarIcons.Md_palette,
            "Plot legend" or "Series selector" => AppBarIcons.Md_format_list_bulleted,
            "Grid lines" => AppBarIcons.Md_grid,
            "Y-axis visibility" => AppBarIcons.Md_axis_y_arrow,
            "Clear annotations" => AppBarIcons.Md_broom,
            "Add crosshair" => AppBarIcons.Md_plus,
            "Apply fixed ranges" => AppBarIcons.Md_axis,
            _ => AppBarIcons.Md_autorenew,
        };
        var button = new AppBarButton { Icon = stateIcon?.Invoke() ?? icon, ToolTip = description, HorizontalAlignment = HorizontalAlignment.Left };
        System.Windows.Automation.AutomationProperties.SetName(button, label);
        System.Windows.Automation.AutomationProperties.SetHelpText(button, description);
        if (stateIcon is not null)
        {
            _settingsActions.Add((button, stateIcon));
        }

        button.Click += (_, _) =>
        {
            action();
            UpdateSettingsActionIcons();
        };
        var row = new DockPanel { Margin = new(0, DescriptionSpacing, 0, DescriptionSpacing) };
        DockPanel.SetDock(button, Dock.Left);
        _ = row.Children.Add(button);
        var text = new StackPanel { Margin = new(InputSpacing, 0, 0, 0) };
        AddDescription(text, label, description);
        _ = row.Children.Add(text);
        _ = panel.Children.Add(row);
    }

    /// <summary>Opens an editor for plot-wide and attached-series settings.</summary>
    private void OpenPlotSettings()
    {
        if (_settingsPopup is { IsOpen: true })
        {
            _settingsPopup.IsOpen = false;
            return;
        }

        _settingsActions.Clear();
        var panel = new StackPanel { Margin = new(PopupPadding), Width = PopupWidth };
        AddDescription(panel, "Plot settings", "Changes apply immediately. Expand a series below to configure its attached feed.");
        AddPlotActions(panel);
        AddPlotEditors(panel);
        AddSeriesEditors(panel);

        _settingsPopup = new Popup
        {
            PlacementTarget = PlotSettings,
            Placement = PlacementMode.Bottom,
            StaysOpen = false,
            AllowsTransparency = true,
            Child = new Border
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(PopupBackgroundRedGreen, PopupBackgroundRedGreen, PopupBackgroundBlue)),
                BorderBrush = Brushes.Gray,
                BorderThickness = new(1),
                Child = new ScrollViewer { MaxHeight = Math.Max(MinimumPopupHeight, ActualHeight - PopupHeightInset), VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = panel },
            },
        };
        _settingsPopup.Opened += SettingsPopup_Opened;
        _settingsPopup.Closed += SettingsPopup_Closed;
        _settingsPopup.IsOpen = true;
    }

    /// <summary>Observes input before automatic popup dismissal.</summary>
    /// <param name="sender">The opened popup.</param>
    /// <param name="e">The popup event.</param>
    private void SettingsPopup_Opened(object? sender, EventArgs e) => InputManager.Current.PreProcessInput += SettingsPopup_PreProcessInput;

    /// <summary>Releases the popup input observer when the popup closes.</summary>
    /// <param name="sender">The closed popup.</param>
    /// <param name="e">The popup event.</param>
    private void SettingsPopup_Closed(object? sender, EventArgs e) => InputManager.Current.PreProcessInput -= SettingsPopup_PreProcessInput;

    /// <summary>Closes the popup when its trigger is pressed without reopening it on the subsequent click.</summary>
    /// <param name="sender">The input manager.</param>
    /// <param name="e">The pending input.</param>
    private void SettingsPopup_PreProcessInput(object sender, PreProcessInputEventArgs e)
    {
        if (_settingsPopup is not { IsOpen: true }
            || e.StagingItem.Input is not MouseButtonEventArgs { ChangedButton: MouseButton.Left, ButtonState: MouseButtonState.Pressed } mouse
            || mouse.RoutedEvent != Mouse.PreviewMouseDownEvent)
        {
            return;
        }

        if (!new Rect(default(Point), PlotSettings.RenderSize).Contains(mouse.GetPosition(PlotSettings)))
        {
            return;
        }

        _settingsPopup.IsOpen = false;
        e.Cancel();
    }

    /// <summary>Refreshes stateful action icons without rebuilding the popup.</summary>
    private void UpdateSettingsActionIcons()
    {
        foreach (var (button, icon) in _settingsActions)
        {
            button.Icon = icon();
        }
    }

    /// <summary>Adds editors for each attached series.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddSeriesEditors(Panel panel)
    {
        foreach (var settings in ViewModel!.SeriesSettings)
        {
            var series = new StackPanel();
            AddDescription(series, settings.PlotType.ToString(), "Appearance and retention settings apply to this feed and persist as new data arrives.");
            AddBoundToggle(series, "Visible", "Display this series. Hiding it does not pause incoming data.", settings, nameof(ReactivePlotSeriesSettings.IsVisible));
            AddBoundToggle(
                series,
                "Pause feed",
                "Freeze this series until resumed. Snapshot feeds may then display their retained history.",
                settings,
                nameof(ReactivePlotSeriesSettings.IsPaused));
            if (settings.PlotType is PlotType.Signal or PlotType.SignalXY)
            {
                AddBoundToggle(
                    series,
                    PointerValuesLabel,
                    "Show the nearest sample and its coordinates as the pointer moves.",
                    settings,
                    nameof(ReactivePlotSeriesSettings.IsCrossHairVisible));
            }

            AddBoundToggle(series, "Include in plot legend", "Include the series name in the legend drawn inside the plot.", settings, nameof(ReactivePlotSeriesSettings.ShowInLegend));
            AddReactiveAppearanceEditors(series, settings);
            _ = panel.Children.Add(new Expander { Header = settings.SeriesName, Content = series, Margin = new(0, SectionSpacing, 0, 0) });
        }

        AddLegacySeriesEditors(panel);
    }

    /// <summary>Checks whether a series already has a reactive editor.</summary>
    /// <param name="name">The series name.</param>
    /// <returns>Whether a reactive editor exists.</returns>
    private bool IsReactiveSeries(string? name)
    {
        foreach (var settings in ViewModel!.SeriesSettings)
        {
            if (settings.SeriesLabel == name)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Adds editors for legacy series.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddLegacySeriesEditors(Panel panel)
    {
        foreach (var line in ViewModel!.PlotLinesCollectionUI)
        {
            var series = new StackPanel();
            var settings = line.ChartSettings;
            if (IsReactiveSeries(settings.ItemName))
            {
                continue;
            }

            AddBoundToggle(series, "Visible", "Display or hide the series while continuing to receive its data.", settings, nameof(ChartObjects.IsChecked));
            if (line is not SignalXY_UI)
            {
                AddBoundToggle(series, "Pause feed", "Stop accepting new samples until resumed; skipped samples are not replayed.", settings, nameof(ChartObjects.IsPaused));
            }

            if (line is SignalUI or SignalXY_UI)
            {
                AddBoundToggle(series, PointerValuesLabel, "Show nearest-point marker, crosshair, and coordinates for this series.", settings, nameof(ChartObjects.IsCrossHairVisible));
            }

            AddBoundEditor(series, "Series name", "The name shown in the series selector.", settings, nameof(ChartObjects.ItemName));
            AddBoundColorPicker(series, settings);
            AddBoundEditor(series, "Line width", "Thickness of connecting lines in pixels.", settings, nameof(ChartObjects.LineWidth));
            _ = panel.Children.Add(new Expander { Header = settings.ItemName, Content = series, Margin = new(0, SectionSpacing, 0, 0) });
        }
    }

    /// <summary>Adds immediate plot actions.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddPlotActions(Panel panel)
    {
        AddAction(
            panel,
            "Follow / explore data",
            "Toggle mouse pan and zoom. Following data continuously fits the axes to incoming samples.",
            ExecuteLockUnlock,
            () => _locked ? AppBarIcons.Md_lock : AppBarIcons.Md_lock_open);
        AddAction(
            panel,
            PointerValuesLabel,
            "Toggle nearest-point markers and coordinates for signal feeds.",
            ExecuteMarkerOnOff,
            () => ViewModel!.CrossHairEnabled ? AppBarIcons.Md_crosshairs : AppBarIcons.Md_crosshairs_off);
        AddAction(panel, "Fit axes now", "Fit all axes to visible data once, preserving the current follow or exploration mode.", () =>
        {
            ViewModel!.WpfPlot1vm!.Plot.Axes.AutoScale();
            ViewModel.WpfPlot1vm.Refresh();
        });
        AddAppearanceActions(panel);
        AddAnnotationActions(panel);
    }

    /// <summary>Adds plot appearance and visibility actions.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddAppearanceActions(Panel panel)
    {
        AddAction(
            panel,
            "Light / dark plot",
            "Switch plot background, grid, axis, and legend colors between light and dark themes.",
            () => ViewModel!.ApplyTheme(ViewModel.CurrentTheme == ReactivePlotTheme.Dark ? ReactivePlotTheme.Light : ReactivePlotTheme.Dark),
            () => ViewModel!.CurrentTheme == ReactivePlotTheme.Dark ? AppBarIcons.Md_weather_night : AppBarIcons.Md_white_balance_sunny);
        AddAction(
            panel,
            "Plot legend",
            "Show or hide the legend drawn inside the plot. Series can be excluded separately.",
            () =>
        {
            ViewModel!.WpfPlot1vm!.Plot.Legend.IsVisible = !ViewModel.WpfPlot1vm.Plot.Legend.IsVisible;
            ViewModel.WpfPlot1vm.Refresh();
        },
            () => ViewModel!.WpfPlot1vm!.Plot.Legend.IsVisible ? AppBarIcons.Md_format_list_bulleted : AppBarIcons.Md_eye_off);
        AddAction(
            panel,
            "Grid lines",
            "Show or hide the major horizontal and vertical grid lines behind the data.",
            () =>
        {
            var grid = ViewModel!.WpfPlot1vm!.Plot.Grid;
            var show = grid.XAxisStyle.MajorLineStyle.Width == 0;
            grid.XAxisStyle.MajorLineStyle.Width = show ? 1 : 0;
            grid.YAxisStyle.MajorLineStyle.Width = show ? 1 : 0;
            ViewModel.WpfPlot1vm.Refresh();
        },
            () => ViewModel!.WpfPlot1vm!.Plot.Grid.XAxisStyle.MajorLineStyle.Width > 0 ? AppBarIcons.Md_grid : AppBarIcons.Md_grid_off);
        AddAction(
            panel,
            "Series selector",
            "Show or hide the external series selector without affecting the legend inside the plot.",
            () =>
        {
            var show = TopLegend.Visibility != Visibility.Visible && RightLegend.Visibility != Visibility.Visible;
            TopLegend.Visibility = show && LegendPosition == LegendPosition.Top ? Visibility.Visible : Visibility.Collapsed;
            RightLegend.Visibility = show && LegendPosition == LegendPosition.Right ? Visibility.Visible : Visibility.Collapsed;
        },
            () => TopLegend.Visibility == Visibility.Visible || RightLegend.Visibility == Visibility.Visible ? AppBarIcons.Md_eye : AppBarIcons.Md_eye_off);
        AddAction(
            panel,
            "Y-axis visibility",
            "Show or hide all configured vertical axes; data remains visible.",
            () =>
        {
            var show = !HasVisibleYAxis();
            foreach (var axis in ViewModel!.YAxisList)
            {
                axis.IsVisible = show;
            }

            ViewModel.WpfPlot1vm!.Refresh();
        },
            () => HasVisibleYAxis() ? AppBarIcons.Md_axis_y_arrow : AppBarIcons.Md_eye_off);
    }

    /// <summary>Adds pointer annotation actions.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddAnnotationActions(Panel panel)
    {
        AddAction(panel, "Add crosshair", "Add a crosshair that follows the pointer on the primary axes. Clear annotations removes added crosshairs.", () => ViewModel!.AddCrosshair());
        AddAction(panel, "Clear annotations", "Remove Ctrl+click coordinate labels and added pointer crosshairs without deleting feed data.", () =>
        {
            ViewModel!.ClearLabels();
            ViewModel.ClearAxisCrosshairs();
        });
    }

    /// <summary>Checks whether any configured vertical axis is visible.</summary>
    /// <returns>Whether a vertical axis is visible.</returns>
    private bool HasVisibleYAxis()
    {
        foreach (var axis in ViewModel!.YAxisList)
        {
            if (axis.IsVisible)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Adds plot-wide value editors.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddPlotEditors(Panel panel)
    {
        AddBoundEditor(panel, "Title", "Text displayed above the chart. A single space hides the title.", this, nameof(TitleContent));
        AddBoundChoice(
            panel,
            "Series selector position",
            "Place the external feed visibility selector above or beside the chart. Left hides the current selector.",
            this,
            nameof(LegendPosition),
            GetSettingsChoices<LegendPosition>());
        AddBoundToggle(panel, "Limit displayed points", "Display the most recent samples instead of the full retained history.", this, nameof(UseFixedNumberOfPoints));
        AddBoundEditor(panel, "Displayed point count", "Number of recent samples shown when the point limit is enabled; must be positive.", this, nameof(NumberPointsPlotted));
        AddAxisEditors(panel);
        AddBindingEditors(panel);
    }

    /// <summary>Adds manual range editors for the configured axes.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddAxisEditors(Panel panel)
    {
        var content = new StackPanel();
        AddDescription(content, "Manual axis ranges", "Apply a fixed range and enable exploration. Date/time X limits use OLE Automation day values, matching the plot coordinates.");
        var limits = ViewModel!.WpfPlot1vm!.Plot.Axes.GetLimits();
        var left = AddValueInput(content, "X minimum", limits.Left);
        var right = AddValueInput(content, "X maximum", limits.Right);
        var axis = new ComboBox { ItemsSource = ViewModel.YAxisList, SelectedIndex = 0, DisplayMemberPath = "Label.Text", ToolTip = "Select the vertical axis whose range will be set." };
        _ = content.Children.Add(axis);
        var bottom = AddValueInput(content, "Y minimum", limits.Bottom);
        var top = AddValueInput(content, "Y maximum", limits.Top);
        var error = new TextBlock { Foreground = Brushes.OrangeRed, TextWrapping = TextWrapping.Wrap };
        AddAction(content, "Apply fixed ranges", "Set the X range and selected Y-axis range. Automatic following stops until Follow data is selected.", () =>
        {
            if (!TryReadFinite(left, out var xMinimum) || !TryReadFinite(right, out var xMaximum)
                || !TryReadFinite(bottom, out var yMinimum) || !TryReadFinite(top, out var yMaximum)
                || xMinimum >= xMaximum || yMinimum >= yMaximum || axis.SelectedItem is not ScottPlot.IYAxis selected)
            {
                error.Text = "Enter finite numbers with each minimum smaller than its maximum, and select a Y axis.";
                return;
            }

            error.Text = string.Empty;
            _needLock = false;
            ExecuteLockUnlock();
            ViewModel.WpfPlot1vm.Plot.Axes.SetLimitsX(xMinimum, xMaximum, ViewModel.XAxis1);
            ViewModel.WpfPlot1vm.Plot.Axes.SetLimitsY(yMinimum, yMaximum, selected);
            ViewModel.WpfPlot1vm.Refresh();
        });
        _ = content.Children.Add(error);
        _ = panel.Children.Add(new Expander { Header = "Axis ranges", Content = content, Margin = new(0, SectionSpacing, 0, 0) });
    }

    /// <summary>Adds explicit reactive connection configuration.</summary>
    /// <param name="panel">The target panel.</param>
    private void AddBindingEditors(Panel panel)
    {
        if (ReactivePlotSources is null)
        {
            return;
        }

        var content = new StackPanel();
        AddDescription(
            content,
            "Feed connection",
            $"State: {_reactivePlotConnection?.CurrentState}. Applying reconnects feeds, clears history and resets series settings. Replay depends on the source.");
        AddDescription(
            content,
            "Batching",
            "Group updates for this many milliseconds, then dispatch them to the plot. Zero delivers updates immediately. The maximum batch size can deliver a batch before the time window ends.");
        var window = AddValueInput(content, "Batch window (milliseconds)", _batchWindowMilliseconds);
        var count = AddValueInput(content, "Maximum batch size", _maximumBatchSize);
        AddDescription(
            content,
            "Overflow policy",
            "DropOldest / KeepLatest retain recent data. DropNewest preserves older data. BlockSource behaves as DropOldest; it does not block producers.");
        var overflow = new ComboBox { ItemsSource = GetSettingsChoices<ReactivePlotOverflowStrategy>(), SelectedItem = _overflowStrategy };
        _ = content.Children.Add(overflow);
        AddDescription(
            content,
            "Invalid updates",
            "StopSeries stops a failed feed. ContinueWithRetry accepts further updates but cannot restart a terminated source. IgnoreInvalidUpdates skips invalid envelopes.");
        var errors = new ComboBox { ItemsSource = GetSettingsChoices<ReactivePlotErrorMode>(), SelectedItem = _errorMode };
        _ = content.Children.Add(errors);
        var error = new TextBlock { Foreground = Brushes.OrangeRed, TextWrapping = TextWrapping.Wrap };
        AddAction(content, "Apply and reconnect feeds", "Reconnect using the selected delivery policies; clear history and reset per-series configuration.", () =>
        {
            if (!TryReadFinite(window, out var milliseconds) || milliseconds < 0 || milliseconds > int.MaxValue
                || !int.TryParse(count.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var maximum) || maximum < 1)
            {
                error.Text = "Use a nonnegative batch window no greater than 2147483647 milliseconds and a positive integer batch size.";
                return;
            }

            _batchWindowMilliseconds = milliseconds;
            _maximumBatchSize = maximum;
            _overflowStrategy = (ReactivePlotOverflowStrategy)overflow.SelectedItem;
            _errorMode = (ReactivePlotErrorMode)errors.SelectedItem;
            ChangeReactivePlotSources(ReactivePlotSources);
            _settingsPopup!.IsOpen = false;
            OpenPlotSettings();
        });
        _ = content.Children.Add(error);
        _ = panel.Children.Add(new Expander { Header = "Feed delivery and overflow", Content = content, Margin = new(0, SectionSpacing, 0, 0) });
    }

    /// <summary>Rejects invalid legacy settings before mutating the chart.</summary>
    /// <param name="property">The property being edited.</param>
    private sealed class SettingsValueValidationRule(string property) : ValidationRule
    {
        /// <inheritdoc />
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var text = value as string ?? string.Empty;
            if (property == nameof(NumberPointsPlotted))
            {
                return int.TryParse(text, NumberStyles.Integer, cultureInfo, out var count) && count > 0
                    ? ValidationResult.ValidResult
                    : new ValidationResult(false, "Enter a positive integer point count.");
            }

            return property == nameof(ChartObjects.LineWidth) ? ValidateWidth(text, cultureInfo) : ValidateColor(text);
        }

        /// <summary>Validates a nonnegative line width.</summary>
        /// <param name="text">The proposed value.</param>
        /// <param name="culture">The numeric culture.</param>
        /// <returns>The validation result.</returns>
        private static ValidationResult ValidateWidth(string text, CultureInfo culture) =>
            double.TryParse(text, NumberStyles.Float, culture, out var width)
            && !double.IsNaN(width) && !double.IsInfinity(width) && width >= 0 && width <= float.MaxValue
                ? ValidationResult.ValidResult
                : new(false, "Enter a finite, nonnegative line width.");

        /// <summary>Validates a named or hexadecimal color.</summary>
        /// <param name="text">The proposed color.</param>
        /// <returns>The validation result.</returns>
        private static ValidationResult ValidateColor(string text)
        {
            if (!text.StartsWith('#'))
            {
                return System.Drawing.Color.FromName(text).IsKnownColor
                    ? ValidationResult.ValidResult
                    : new(false, "Enter a named color or a hexadecimal color such as #377EB8.");
            }

            try
            {
                _ = ScottPlot.Color.FromHex(text);
                return ValidationResult.ValidResult;
            }
            catch (FormatException)
            {
                return new(false, "Enter a valid hexadecimal color such as #377EB8.");
            }
        }
    }
}
