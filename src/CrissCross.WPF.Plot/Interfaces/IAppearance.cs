// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI;
using ScottPlot;
using ScottPlot.WPF;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Defines the contract for configuring and managing the appearance-related properties and commands of a UI element,
/// including color, visibility, state, and subscriptions for interactive features.
/// </summary>
/// <remarks>Implementations of this interface provide access to appearance settings such as color, opacity,
/// visibility, and state, as well as commands and subscription methods for handling UI interactions. This interface is
/// typically used to abstract the appearance logic for controls or visual elements in applications that support dynamic
/// or interactive visualization. Thread safety and specific behavior depend on the implementing class.</remarks>
public interface IAppearance
{
    /// <summary>
    /// Gets or sets the color CheckBox.
    /// </summary>
    /// <value>
    /// The color CheckBox.
    /// </value>
    string Color { get; set; }

    /// <summary>
    /// Gets or sets the color text.
    /// </summary>
    /// <value>
    /// The color text.
    /// </value>
    string? ColorText { get; set; }

    /// <summary>
    /// Gets or sets the displayed value.
    /// </summary>
    /// <value>
    /// The displayed value.
    /// </value>
    int DisplayedValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value>
    bool IsChecked { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is paused.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
    /// </value>
    bool IsPaused { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    /// <value>
    /// The name of the item.
    /// </value>
    string? ItemName { get; set; }

    /// <summary>
    /// Gets or sets the width of the line.
    /// </summary>
    /// <value>
    /// The width of the line.
    /// </value>
    double LineWidth { get; set; }

    /// <summary>
    /// Gets or sets the opacity CheckBox.
    /// </summary>
    /// <value>
    /// The opacity CheckBox.
    /// </value>
    string? OpacityCheckBox { get; set; }

    /// <summary>
    /// Gets or sets the is checked command.
    /// </summary>
    /// <value>
    /// The is checked command.
    /// </value>
    ReactiveCommand<Unit, Unit>? IsCheckedCmd { get; set; }

    /// <summary>
    /// Subscribes to crosshair events, enabling the receiver to respond to crosshair interactions.
    /// </summary>
    void CrosshairSubscription();

    /// <summary>
    /// Configures appearance-related subscriptions for the specified plottable on the given plot.
    /// </summary>
    /// <typeparam name="T">The type of the plottable to configure. Must implement <see cref="IHasLine"/>, <see cref="IHasMarker"/>, and
    /// <see cref="ScottPlot.IPlottable"/>.</typeparam>
    /// <param name="plot">The plot on which the appearance subscriptions will be configured.</param>
    /// <param name="plotable">The plottable object whose appearance subscriptions are to be set up.</param>
    void AppearanceSubsriptions<T>(WpfPlot plot, T plotable)
        where T : IHasLine, IHasMarker, ScottPlot.IPlottable;
}
