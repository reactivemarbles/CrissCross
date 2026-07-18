// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
#if REACTIVE_SHIM
using CP.Reactive.Collections;
#else
using CP.Primitives.Collections;
#endif
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
using ReactiveUI.SourceGenerators;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Manages chart-object properties in the right properties panel.</summary>
/// <remarks>This class provides properties and commands for interacting with chart object settings, such as line
/// color, line width, item name, and visibility. It is intended for use in Windows 10.0.19041 or later environments.
/// The view model exposes collections of available line colors and visibility options, and provides a command to save
/// changes to the selected chart object. Thread safety and reactive property change notifications are handled via the
/// base class and reactive extensions.</remarks>
[SupportedOSPlatform("windows")]
public partial class RightPropertiesViewModel : RxObject
{
    /// <summary>Stores the selected setting value.</summary>
    [Reactive]
    private ChartObjects? _selectedSetting;

    /// <summary>Stores the line width value.</summary>
    [Reactive]
    private double _lineWidth;

    /// <summary>Stores the line color value.</summary>
    [Reactive]
    private string? _lineColor;

    /// <summary>Stores the item name value.</summary>
    [Reactive]
    private string? _itemName;

    /// <summary>Stores the item visibility value.</summary>
    [Reactive]
    private string? _itemVisibility;

    /// <summary>Initializes a new instance of the <see cref="RightPropertiesViewModel"/> class.</summary>
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
            System.Drawing.Color.Gray.Name,];

        Visibilities = ["Visible", "Invisible"];

        SaveConfiguration = ReactiveCommand.Create(() =>
        {
            if (SelectedSetting is null)
            {
                return;
            }

            ApplyIfSpecified(ItemName, value => SelectedSetting.ItemName = value);

            SelectedSetting.LineWidth = LineWidth;

            ApplyIfSpecified(LineColor, value => SelectedSetting.Color = value);
            ApplyIfSpecified(ItemVisibility, value => SelectedSetting.Visibility = value);
        });
    }

    /// <summary>Gets the command that saves the current configuration settings.</summary>
    /// <remarks>This command can be executed to persist any changes made to the configuration. The command
    /// completes when the save operation finishes. Typically used in UI scenarios to trigger saving from a button or
    /// menu item.</remarks>
    public ReactiveCommand<Unit, Unit> SaveConfiguration { get; }

    /// <summary>Gets or sets the collection of colors used for lines in the chart or visualization.</summary>
    /// <remarks>The list contains up to four color values, which are applied in order to the lines rendered.
    /// If fewer than four colors are specified, default colors may be used for remaining lines. Modifying this property
    /// allows customization of line appearance.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<string> LineColors { get; set; }
#else
    public ReactiveList<string> LineColors { get; set; }
#endif

    /// <summary>Gets or sets the colors used to render chart lines.</summary>
    /// <remarks>The order of colors in the collection determines which color is applied to each line.
    /// Modifying this collection updates the appearance of the corresponding lines. The property is observable; changes
    /// to the collection will automatically propagate to any listeners.</remarks>
#if NET6_0_OR_GREATER
    public QuaternaryList<string> Visibilities { get; set; }
#else
    public ReactiveList<string> Visibilities { get; set; }
#endif

    /// <summary>Applies a text value when it has content.</summary>
    /// <param name="value">The text value.</param>
    /// <param name="apply">The update action.</param>
    private static void ApplyIfSpecified(string? value, Action<string> apply)
    {
        if (value is not { Length: > 0 })
        {
            return;
        }

        apply(value);
    }
}
