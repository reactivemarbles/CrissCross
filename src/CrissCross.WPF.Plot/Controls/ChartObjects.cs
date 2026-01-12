// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

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
    [Reactive]
    private bool _isCrossHairVisible;
    [Reactive]
    private string? _icon;
    [Reactive]
    private bool _autoScale;
    [Reactive]
    private string? _color;
    [Reactive]
    private string? _colorText;
    [Reactive]
    private int _displayedValue;
    [Reactive]
    private bool _isChecked;
    [Reactive]
    private bool _isPaused;
    [Reactive]
    private bool _isVisible;
    [Reactive]
    private string? _visibility;
    [Reactive]
    private string? _itemName;
    [Reactive]
    private double _lineWidth;
    [Reactive]
    private string? _opacityCheckBox;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartObjects"/> class with the specified item name and color.
    /// </summary>
    /// <param name="itemName">The name of the chart item to be displayed. If not specified, defaults to "---".</param>
    /// <param name="color">The color used to represent the chart item. If not specified, defaults to "Green".</param>
    public ChartObjects(string itemName = "---", string color = "Green")
    {
        Color = color;
        ItemName = itemName;
        ColorText = "#FFD3D3D3";
        OpacityCheckBox = "1";
        LineWidth = 3;
        IsVisible = true;
        IsChecked = true;
        DisplayedValue = 0;
        Visibility = "Visible";
        AutoScale = false;
        IsPaused = false;
        IsCrossHairVisible = false;
        Crosshair = new Crosshair();
        Marker = new Marker();
        MarkerText = new Text();
    }

    /// <summary>
    /// Gets or sets the crosshair configuration used for rendering or interaction purposes.
    /// </summary>
    public Crosshair Crosshair { get; set; }

    /// <summary>
    /// Gets or sets the marker associated with the current instance.
    /// </summary>
    public Marker Marker { get; set; }

    /// <summary>
    /// Gets or sets the text displayed by the marker.
    /// </summary>
    public Text MarkerText { get; set; }

    /// <summary>
    /// Gets or sets the command that is executed when the checked state changes.
    /// </summary>
    /// <remarks>This command can be bound to UI elements to handle changes in checked or selected state, such
    /// as toggling a checkbox. The command executes with no parameters and does not return a value.</remarks>
    public ReactiveCommand<Unit, Unit>? IsCheckedCmd { get; set; }

    /// <summary>
    /// Initializes visual elements on the specified plot to highlight cursor positions, including a crosshair, marker,
    /// and text label using the provided color.
    /// </summary>
    /// <remarks>The created visual elements are initially hidden and can be shown or updated to reflect
    /// cursor interactions. This method does not modify the plot if the provided control is null.</remarks>
    /// <param name="wpfPlot">The plot control on which cursor-related visual elements will be created. If null, no elements are created.</param>
    /// <param name="colorName">The name of the color to apply to the crosshair, marker, and text label. Must correspond to a valid system color
    /// name.</param>
    public void CreateCursorValues(WpfPlot wpfPlot, string colorName)
    {
        if (wpfPlot == null)
        {
            return;
        }

        // Create a crosshair to highlight the point under the cursor
        Crosshair = wpfPlot.Plot.Add.Crosshair(0, 0);
        Crosshair.IsVisible = false;
        Crosshair!.LineColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(colorName!));

        // Create a marker to highlight the point under the cursor
        Marker = wpfPlot!.Plot.Add.Marker(0, 0);
        Marker.Shape = MarkerShape.OpenCircle;
        Marker.Size = 17;
        Marker.LineWidth = 2;
        Marker.IsVisible = false;
        Marker!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(colorName!));

        // Create a text label to place near the highlighted value
        MarkerText = wpfPlot!.Plot.Add.Text(" ", 0, 0);
        MarkerText.LabelAlignment = Alignment.LowerLeft;
        MarkerText.LabelBold = true;
        MarkerText.OffsetX = 7;
        MarkerText.OffsetY = -7;
        MarkerText.IsVisible = false;
        MarkerText!.LabelFontColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(colorName!));
        MarkerText.LabelStyle.BackgroundColor = ScottPlot.Colors.White;
    }

    /// <summary>
    /// Synchronizes the appearance and visibility properties of the specified plotable object with the current view
    /// model state and updates the provided WpfPlot accordingly.
    /// </summary>
    /// <remarks>This method establishes reactive subscriptions that automatically update the plotable
    /// object's appearance and visibility based on changes to the view model's properties. The WpfPlot is refreshed
    /// whenever relevant properties change to ensure the display remains current. The method disposes subscriptions
    /// with the view model's disposables to manage resources effectively.</remarks>
    /// <typeparam name="T">The type of the plotable object to synchronize. Must implement IHasLine, IHasMarker, and ScottPlot.IPlottable.</typeparam>
    /// <param name="wpfPlot">The WpfPlot instance to refresh when appearance or visibility changes occur.</param>
    /// <param name="plotable">The plotable object whose line, marker, and visibility properties will be updated in response to view model
    /// changes.</param>
    public void AppearanceSubsriptions<T>(WpfPlot wpfPlot, T plotable)
        where T : IHasLine, IHasMarker, ScottPlot.IPlottable
    {
        this.WhenAnyValue(x => x.LineWidth, x => x.Color, x => x.Visibility)
            .Subscribe(x =>
            {
                plotable!.LineStyle.Width = (float)x.Item1;
                plotable!.LineStyle.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
                IsChecked = x.Item3 != "Invisible";
                plotable!.IsVisible = x.Item3 != "Invisible";
                Crosshair!.LineColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
                Marker!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
                MarkerText!.LabelFontColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
                wpfPlot.Refresh();
            }).DisposeWith(Disposables);

        this.WhenAnyValue(x => x.IsChecked)
            .Subscribe(x => Visibility = !x ? "Invisible" : "Visible")
            .DisposeWith(Disposables);

        this.WhenAnyValue(x => x.IsCrossHairVisible, x => x.Visibility)
            .Subscribe(x =>
            {
                var visibility = x.Item1 && x.Item2 != "Invisible";
                Marker.IsVisible = visibility;
                Crosshair.IsVisible = visibility;
                MarkerText.IsVisible = visibility;
            }).DisposeWith(Disposables);
    }

    /// <summary>
    /// Subscribes to property changes related to appearance and updates the specified plot control accordingly.
    /// </summary>
    /// <remarks>This method establishes reactive subscriptions to properties such as line width, color,
    /// visibility, and crosshair state. When these properties change, the plot control is refreshed to reflect the
    /// updated appearance. This ensures that UI elements remain synchronized with the underlying data model.</remarks>
    /// <param name="wpfPlot">The plot control to refresh when appearance-related properties change.</param>
    public void AppearanceSubsriptions(WpfPlot wpfPlot)
    {
        this.WhenAnyValue(x => x.LineWidth, x => x.Color, x => x.Visibility)
            .Subscribe(x =>
            {
                IsChecked = x.Item3 != "Invisible";
                Crosshair!.LineColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
                Marker!.Color = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
                MarkerText!.LabelFontColor = ScottPlot.Color.FromColor(System.Drawing.Color.FromName(x.Item2!));
                wpfPlot.Refresh();
            }).DisposeWith(Disposables);

        this.WhenAnyValue(x => x.IsChecked)
            .Subscribe(x => Visibility = !x ? "Invisible" : "Visible")
            .DisposeWith(Disposables);

        this.WhenAnyValue(x => x.IsCrossHairVisible, x => x.Visibility)
            .Subscribe(x =>
            {
                var visibility = x.Item1 && x.Item2 != "Invisible";
                Marker.IsVisible = visibility;
                Crosshair.IsVisible = visibility;
                MarkerText.IsVisible = visibility;
            }).DisposeWith(Disposables);
    }

    /// <summary>
    /// Subsriptions for appearance.
    /// </summary>
    public void CrosshairSubscription()
    {
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        IsCheckedCmd?.Dispose();
        base.Dispose(disposing);
    }
}
