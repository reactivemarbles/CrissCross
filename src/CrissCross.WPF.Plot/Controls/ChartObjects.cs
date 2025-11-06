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
/// Settings.
/// </summary>
/// <seealso cref="ChartObjects" />
/// <remarks>
/// Initializes a new instance of the <see cref="ChartObjects" /> class.
/// </remarks>
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
    /// Initializes a new instance of the <see cref="ChartObjects" /> class.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <param name="color">The color.</param>
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
    /// Gets or sets the marker.
    /// </summary>
    /// <value>
    /// The marker.
    /// </value>
    public Crosshair Crosshair { get; set; }

    /// <summary>
    /// Gets or sets the marker.
    /// </summary>
    /// <value>
    /// The marker.
    /// </value>
    public Marker Marker { get; set; }

    /// <summary>
    /// Gets or sets the marker text.
    /// </summary>
    /// <value>
    /// The marker text.
    /// </value>
    public Text MarkerText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value> //// = ReactiveCommand.Create(() => { });
    public ReactiveCommand<Unit, Unit>? IsCheckedCmd { get; set; }

    /// <summary>
    /// Creates the cursor values.
    /// </summary>
    /// <param name="wpfPlot">The plot.</param>
    /// <param name="colorName">Name of the color.</param>
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
    /// Subsriptions for appearance.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="wpfPlot">The plot.</param>
    /// <param name="plotable">The plotable.</param>
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
    /// Appearances the subsriptions.
    /// </summary>
    /// <param name="wpfPlot">The WPF plot.</param>
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
