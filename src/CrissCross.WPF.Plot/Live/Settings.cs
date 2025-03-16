// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Settings.
/// </summary>
/// <seealso cref="Settings" />
/// <remarks>
/// Initializes a new instance of the <see cref="Settings" /> class.
/// </remarks>
public partial class Settings : RxObject, IAppearance
{
    /// <summary>
    /// Gets or sets the color text.
    /// </summary>
    [Reactive]
    private string? _icon;

    /// <summary>
    /// Gets or sets a value indicating whether [automatic scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [automatic scale]; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _autoScale;

    /// <summary>
    /// Gets or sets the color CheckBox.
    /// </summary>
    /// <value>
    /// The color CheckBox.
    /// </value>
    [Reactive]
    private string? _color;

    /// <summary>
    /// Gets or sets the color text.
    /// </summary>
    /// <value>
    /// The color text.
    /// </value>
    [Reactive]
    private string? _colorText;

    /// <summary>
    /// Gets or sets the displayed value.
    /// </summary>
    /// <value>
    /// The displayed value.
    /// </value>
    [Reactive]
    private int _displayedValue;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _isChecked;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is paused.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _isPaused;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _isVisible;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private string? _visibility;

    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    /// <value>
    /// The name of the item.
    /// </value>
    [Reactive]
    private string? _itemName;

    /// <summary>
    /// Gets or sets the width of the line.
    /// </summary>
    /// <value>
    /// The width of the line.
    /// </value>
    [Reactive]
    private double _lineWidth;

    /////// <summary>
    /////// Gets or sets the number points plotted.
    /////// </summary>
    /////// <value>
    /////// The number points plotted.
    /////// </value>
    ////[Reactive]
    ////private int _numberPointsPlotted;

    /// <summary>
    /// Gets or sets the opacity CheckBox.
    /// </summary>
    /// <value>
    /// The opacity CheckBox.
    /// </value>
    [Reactive]
    private string? _opacityCheckBox;

    [Reactive]
    private bool _isCrossHairVisible;

    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class.
    /// </summary>
    public Settings()
    {
        Color = "Green";
        ItemName = "---";
        ColorText = "#FFD3D3D3";
        OpacityCheckBox = "1";
        LineWidth = 3;
        IsVisible = true;
        IsChecked = false;
        DisplayedValue = 0;
        Visibility = "Visible";
        AutoScale = false;
        IsPaused = false;
        IsCrossHairVisible = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value> //// = ReactiveCommand.Create(() => { });
    public ReactiveCommand<Unit, Unit>? IsCheckedCmd { get; set; }

    /// <summary>
    /// Subsriptions for appearance.
    /// </summary>
    public void AppearanceSubsriptions()
    {
    }
}
