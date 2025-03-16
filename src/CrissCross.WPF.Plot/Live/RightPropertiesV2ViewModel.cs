// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using CP.Reactive;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Window1ViewModel.
/// </summary>
/// <seealso cref="RxObject" />
////[SupportedOSPlatform("windows10.0.19041")]
public partial class RightPropertiesV2ViewModel : RxObject
{
    private Visibility _isVisible = Visibility.Visible;
    [Reactive]
    private Settings? _selectedSetting;
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
        LineColors = [];

        LineColors.Add(System.Drawing.Color.Blue.Name);
        LineColors.Add(System.Drawing.Color.Red.Name);
        LineColors.Add(System.Drawing.Color.Orange.Name);
        LineColors.Add(System.Drawing.Color.Yellow.Name);
        LineColors.Add(System.Drawing.Color.Green.Name);
        LineColors.Add(System.Drawing.Color.Brown.Name);
        LineColors.Add(System.Drawing.Color.Violet.Name);
        LineColors.Add(System.Drawing.Color.Pink.Name);
        LineColors.Add(System.Drawing.Color.Gray.Name);

        Visibilities = [];
        Visibilities.Add("Visible");
        Visibilities.Add("Invisible");

        this.WhenAnyValue(x => x.SelectedSetting).WhereNotNull().Subscribe(myitem =>
        {
            ////NameTextBox = myitem.ItemName;
            ////LineWidth = myitem.LineWidth;
            ////if (SelectedItem!.IsVisible)
            ////{
            ////    VisibilityOption = 0;
            ////}
            ////else
            ////{
            ////    VisibilityOption = 1;
            ////}

            ////if (SelectedItem!.IsPaused)
            ////{
            ////    LivePauseDataOption = 1;
            ////}
            ////else
            ////{
            ////    LivePauseDataOption = 0;
            ////}
        }).DisposeWith(Disposables);

        // update configuration
        SaveConfiguration = ReactiveCommand.Create(() =>
        {
            if (SelectedSetting == null)
            {
                return;
            }

            ////SelectedItem.Streamer.LineWidth = LineWidth;
            ////SelectedItem.IsVisible = VisibilityOption == 0;
            ////////SelectedMetric.SetPaused(LivePauseDataOption == 1);
            ////if (SelectedItem.IsVisible)
            ////{
            ////    ////SelectedMetric.ColorText = "#FFD3D3D3";
            ////    SelectedItem.OpacityCheckBox = "1";
            ////}
            ////else
            ////{
            ////    ////SelectedMetric.ColorText = "#FF232222";
            ////    SelectedItem.OpacityCheckBox = "0.3";
            ////}
        }).DisposeWith(Disposables);
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
