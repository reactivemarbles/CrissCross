﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Runtime.Versioning;
using System.Windows;
using CP.Reactive;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Window1ViewModel.
/// </summary>
/// <seealso cref="RxObject" />
[SupportedOSPlatform("windows10.0.19041")]
public partial class RightPropertiesV2ViewModel : RxObject
{
    private Visibility _isVisible = Visibility.Visible;
    [Reactive]
    private ChartObjects? _selectedSetting;
    [Reactive]
    private double _lineWidth;
    [Reactive]
    private string? _lineColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RightPropertiesV2ViewModel"/> class.
    /// MainViewModel.
    /// </summary>
    public RightPropertiesV2ViewModel()
    {
        LineColors =
            [
                System.Drawing.Color.Blue.Name,
                System.Drawing.Color.Red.Name,
                System.Drawing.Color.Orange.Name,
                System.Drawing.Color.Yellow.Name,
                System.Drawing.Color.Green.Name,
                System.Drawing.Color.Brown.Name,
                System.Drawing.Color.Violet.Name,
                System.Drawing.Color.Pink.Name,
                System.Drawing.Color.Gray.Name
            ];

        Visibilities = ["Visible", "Invisible"];
    }

    /// <summary>
    /// Gets the save configuration.
    /// </summary>
    /// <value>
    /// The save configuration.
    /// </value>
    public ReactiveCommand<Unit, Unit> SaveConfiguration { get; }

    /// <summary>
    /// Gets or sets the line colors.
    /// </summary>
    /// <value>
    /// The line colors.
    /// </value>
    public ReactiveList<string> LineColors { get; set; }

    /// <summary>
    /// Gets or sets the visibilities.
    /// </summary>
    /// <value>
    /// The visibilities.
    /// </value>
    public ReactiveList<string> Visibilities { get; set; }
}
