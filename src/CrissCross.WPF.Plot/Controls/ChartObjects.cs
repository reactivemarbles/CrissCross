// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using PlotColor = ScottPlot.Color;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents a collection of chart-related objects and appearance settings for managing visual elements such as
/// crosshairs, markers, and labels within a plot. Provides properties and methods to control the visibility, styling,
/// and interaction of these elements in a Windows environment.
/// </summary>
/// <remarks>This class is intended for use with WPF plots and supports reactive updates to appearance and
/// visibility properties. It enables dynamic control of chart elements, including crosshair and marker display, color
/// customization, and visibility toggling. Thread safety is not guaranteed; access from multiple threads should be
/// synchronized. The class is supported only on Windows platforms.</remarks>
[SupportedOSPlatform("windows")]
public partial class ChartObjects : RxObject, IAppearance
{
    /// <summary>The default chart item name.</summary>
    private const string DefaultItemName = "---";

    /// <summary>The default chart item color.</summary>
    private const string DefaultColor = "Green";

    /// <summary>The visible state value.</summary>
    private const string VisibleState = "Visible";

    /// <summary>The invisible state value.</summary>
    private const string InvisibleState = "Invisible";

    /// <summary>The default chart line width.</summary>
    private const double DefaultLineWidth = 3;

    /// <summary>The cursor marker size.</summary>
    private const float CursorMarkerSize = 17;

    /// <summary>The cursor marker stroke width.</summary>
    private const float CursorMarkerLineWidth = 2;

    /// <summary>The cursor marker text offset.</summary>
    private const float CursorMarkerTextOffset = 7;

    /// <summary>Stores the is cross hair visible value.</summary>
    [Reactive]
    private bool _isCrossHairVisible;

    /// <summary>Stores the icon value.</summary>
    [Reactive]
    private string? _icon;

    /// <summary>Stores the auto scale value.</summary>
    [Reactive]
    private bool _autoScale;

    /// <summary>Stores the color value.</summary>
    [Reactive]
    private string? _color;

    /// <summary>Stores the color text value.</summary>
    [Reactive]
    private string? _colorText;

    /// <summary>Stores the displayed value.</summary>
    [Reactive]
    private int _displayedValue;

    /// <summary>Stores the is checked value.</summary>
    [Reactive]
    private bool _isChecked;

    /// <summary>Stores the is paused value.</summary>
    [Reactive]
    private bool _isPaused;

    /// <summary>Stores the is visible value.</summary>
    [Reactive]
    private bool _isVisible;

    /// <summary>Stores the visibility value.</summary>
    [Reactive]
    private string? _visibility;

    /// <summary>Stores the item name value.</summary>
    [Reactive]
    private string? _itemName;

    /// <summary>Stores the line width value.</summary>
    [Reactive]
    private double _lineWidth;

    /// <summary>Stores the opacity check box value.</summary>
    [Reactive]
    private string? _opacityCheckBox;

    /// <summary>Initializes a new instance of the <see cref="ChartObjects"/> class.</summary>
    public ChartObjects()
        : this(DefaultItemName)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ChartObjects"/> class.</summary>
    /// <param name="itemName">The name of the chart item to be displayed.</param>
    public ChartObjects(string itemName)
        : this(itemName, DefaultColor)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ChartObjects"/> class.</summary>
    /// <param name="itemName">The name of the chart item to be displayed.</param>
    /// <param name="color">The color used to represent the chart item.</param>
    public ChartObjects(string itemName, string color)
    {
        Color = color;
        ItemName = itemName;
        ColorText = "#FFD3D3D3";
        OpacityCheckBox = "1";
        LineWidth = DefaultLineWidth;
        IsVisible = true;
        IsChecked = true;
        DisplayedValue = 0;
        Visibility = VisibleState;
        AutoScale = false;
        IsPaused = false;
        IsCrossHairVisible = false;
        Crosshair = new();
        Marker = new();
        MarkerText = new();
    }

    /// <summary>Gets or sets the crosshair configuration used for rendering or interaction purposes.</summary>
    public Crosshair Crosshair { get; set; }

    /// <summary>Gets or sets the marker associated with the current instance.</summary>
    public Marker Marker { get; set; }

    /// <summary>Gets or sets the text displayed by the marker.</summary>
    public Text MarkerText { get; set; }

    /// <summary>Gets or sets the command that is executed when the checked state changes.</summary>
    /// <remarks>This command can be bound to UI elements to handle changes in checked or selected state, such
    /// as toggling a checkbox. The command executes with no parameters and does not return a value.</remarks>
    public ReactiveCommand<Unit, Unit>? IsCheckedCmd { get; set; }

    /// <summary>Initializes visual elements on the specified plot to highlight cursor positions, including a crosshair,
    /// marker, and text label using the provided color.</summary>
    /// <param name="wpfPlot">The wpfPlot value.</param>
    /// <param name="colorName">The colorName value.</param>
    public void CreateCursorValues(WpfPlot wpfPlot, string colorName)
    {
        if (wpfPlot is null)
        {
            return;
        }

        // Create a crosshair to highlight the point under the cursor
        Crosshair = wpfPlot.Plot.Add.Crosshair(0, 0);
        Crosshair.IsVisible = false;
        Crosshair!.LineColor = ResolveColor(colorName);

        // Create a marker to highlight the point under the cursor
        Marker = wpfPlot!.Plot.Add.Marker(0, 0);
        Marker.Shape = MarkerShape.OpenCircle;
        Marker.Size = CursorMarkerSize;
        Marker.LineWidth = CursorMarkerLineWidth;
        Marker.IsVisible = false;
        Marker!.Color = ResolveColor(colorName);

        // Create a text label to place near the highlighted value
        MarkerText = wpfPlot!.Plot.Add.Text(" ", 0, 0);
        MarkerText.LabelAlignment = Alignment.LowerLeft;
        MarkerText.LabelBold = true;
        MarkerText.OffsetX = CursorMarkerTextOffset;
        MarkerText.OffsetY = -CursorMarkerTextOffset;
        MarkerText.IsVisible = false;
        MarkerText!.LabelFontColor = ResolveColor(colorName);
        MarkerText.LabelStyle.BackgroundColor = Colors.White;
    }

    /// <summary>Synchronizes the appearance and visibility properties of the specified plotable object with the current
    /// view model state and updates the provided WpfPlot accordingly.</summary>
    /// <typeparam name="T">The T type.</typeparam>
    /// <param name="plot">The plot value.</param>
    /// <param name="plotable">The plotable value.</param>
    public void AppearanceSubsriptions<T>(WpfPlot plot, T plotable)
        where T : IHasLine, IHasMarker, IPlottable
    {
        _ = this.WhenAnyValue(x => x.LineWidth, x => x.Color, x => x.Visibility)
            .Subscribe(x =>
            {
                var color = ResolveColor(x.Value2);
                plotable!.LineStyle.Width = (float)x.Value1;
                plotable!.LineStyle.Color = color;
                IsChecked = x.Value3 != InvisibleState;
                plotable!.IsVisible = x.Value3 != InvisibleState;
                Crosshair!.LineColor = color;
                Marker!.Color = color;
                MarkerText!.LabelFontColor = color;
                plot.Refresh();
            })
            .DisposeWith(Disposables);

        _ = this.WhenAnyValue(x => x.IsChecked)
            .Subscribe(x => Visibility = !x ? InvisibleState : VisibleState)
            .DisposeWith(Disposables);

        _ = this.WhenAnyValue(x => x.IsCrossHairVisible, x => x.Visibility)
            .Subscribe(x =>
            {
                var visibility = x.Value1 && x.Value2 != InvisibleState;
                Marker.IsVisible = visibility;
                Crosshair.IsVisible = visibility;
                MarkerText.IsVisible = visibility;
            })
            .DisposeWith(Disposables);
    }

    /// <summary>Provides the AppearanceSubsriptions member.</summary>
    /// <remarks>This method establishes reactive subscriptions to properties such as line width, color,
    /// visibility, and crosshair state. When these properties change, the plot control is refreshed to reflect the
    /// updated appearance. This ensures that UI elements remain synchronized with the underlying data model.</remarks>
    /// <param name="wpfPlot">The plot control to refresh when appearance-related properties change.</param>
    public void AppearanceSubsriptions(WpfPlot wpfPlot)
    {
        _ = this.WhenAnyValue(x => x.LineWidth, x => x.Color, x => x.Visibility)
            .Subscribe(x =>
            {
                var color = ResolveColor(x.Value2);
                IsChecked = x.Value3 != InvisibleState;
                Crosshair!.LineColor = color;
                Marker!.Color = color;
                MarkerText!.LabelFontColor = color;
                wpfPlot.Refresh();
            })
            .DisposeWith(Disposables);

        _ = this.WhenAnyValue(x => x.IsChecked)
            .Subscribe(x => Visibility = !x ? InvisibleState : VisibleState)
            .DisposeWith(Disposables);

        _ = this.WhenAnyValue(x => x.IsCrossHairVisible, x => x.Visibility)
            .Subscribe(x =>
            {
                var visibility = x.Value1 && x.Value2 != InvisibleState;
                Marker.IsVisible = visibility;
                Crosshair.IsVisible = visibility;
                MarkerText.IsVisible = visibility;
            })
            .DisposeWith(Disposables);
    }

    /// <summary>Subsriptions for appearance.</summary>
    public void CrosshairSubscription() { }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">The disposing value.</param>
    protected override void Dispose(bool disposing)
    {
        IsCheckedCmd?.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>Handles the ResolveColor operation.</summary>
    /// <param name="colorName">The colorName value.</param>
    /// <returns>The result.</returns>
    private static PlotColor ResolveColor(string? colorName)
    {
        if (string.IsNullOrWhiteSpace(colorName))
        {
            return Colors.White;
        }

        if (colorName!.StartsWith("#", StringComparison.Ordinal))
        {
            try
            {
                return PlotColor.FromHex(colorName);
            }
            catch (FormatException)
            {
                return Colors.White;
            }
        }

        var systemColor = System.Drawing.Color.FromName(colorName);
        return systemColor.A == 0
               && !string.Equals(
                   colorName,
                   nameof(System.Drawing.Color.Transparent),
                   StringComparison.OrdinalIgnoreCase)
            ? Colors.White
            : PlotColor.FromColor(systemColor);
    }
}
