// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Runtime.Versioning;
using CP.Reactive;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents the view model for managing and editing properties of chart objects in the right properties panel.
/// </summary>
/// <remarks>This class provides properties and commands for interacting with chart object settings, such as line
/// color, line width, item name, and visibility. It is intended for use in Windows 10.0.19041 or later environments.
/// The view model exposes collections of available line colors and visibility options, and provides a command to save
/// changes to the selected chart object. Thread safety and reactive property change notifications are handled via the
/// base class and reactive extensions.</remarks>
[SupportedOSPlatform("windows10.0.19041")]
public partial class RightPropertiesViewModel : RxObject
{
    [Reactive]
    private ChartObjects? _selectedSetting;
    [Reactive]
    private double _lineWidth;
    [Reactive]
    private string? _lineColor;
    [Reactive]
    private string? _itemName;
    [Reactive]
    private string? _itemVisibility;

    /// <summary>
    /// Initializes a new instance of the <see cref="RightPropertiesViewModel"/> class and sets up default values for line colors,.
    /// visibilities, and the save configuration command.
    /// </summary>
    /// <remarks>The constructor populates the LineColors and Visibilities collections with predefined values
    /// and configures the SaveConfiguration command to apply changes to the selected setting. This ensures that the
    /// view model is ready for use immediately after instantiation.</remarks>
    public RightPropertiesViewModel()
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

        SaveConfiguration = ReactiveCommand.Create(() =>
        {
            if (SelectedSetting != null)
            {
                // Apply all pending changes to the selected setting
                if (!string.IsNullOrEmpty(ItemName))
                {
                    SelectedSetting.ItemName = ItemName;
                }

                SelectedSetting.LineWidth = LineWidth;

                if (!string.IsNullOrEmpty(LineColor))
                {
                    SelectedSetting.Color = LineColor;
                }

                if (!string.IsNullOrEmpty(ItemVisibility))
                {
                    SelectedSetting.Visibility = ItemVisibility;
                }
            }
        });
    }

    /// <summary>
    /// Gets the command that saves the current configuration settings.
    /// </summary>
    /// <remarks>This command can be executed to persist any changes made to the configuration. The command
    /// completes when the save operation finishes. Typically used in UI scenarios to trigger saving from a button or
    /// menu item.</remarks>
    public ReactiveCommand<Unit, Unit> SaveConfiguration { get; }

    /// <summary>
    /// Gets or sets the collection of colors used for lines in the chart or visualization.
    /// </summary>
    /// <remarks>The list contains up to four color values, which are applied in order to the lines rendered.
    /// If fewer than four colors are specified, default colors may be used for remaining lines. Modifying this property
    /// allows customization of line appearance.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<string> LineColors { get; set; }
#else
    public ReactiveList<string> LineColors { get; set; }
#endif

    /// <summary>
    /// Gets or sets the collection of colors used to render individual lines in the chart or visualization.
    /// </summary>
    /// <remarks>The order of colors in the collection determines which color is applied to each line.
    /// Modifying this collection updates the appearance of the corresponding lines. The property is observable; changes
    /// to the collection will automatically propagate to any listeners.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<string> Visibilities { get; set; }
#else
    public ReactiveList<string> Visibilities { get; set; }
#endif
}
