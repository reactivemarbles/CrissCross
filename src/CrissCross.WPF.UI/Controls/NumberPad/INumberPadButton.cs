// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NumberPad Button.
/// </summary>
public interface INumberPadButton
{
    /// <summary>
    /// Occurs when [value changed].
    /// </summary>
    event EventHandler<(bool UserChanged, double Value)> ValueChanged;

    /// <summary>
    /// Gets or sets the decimal places.
    /// </summary>
    /// <value>The decimal places.</value>
    int DecimalPlaces { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>The maximum.</value>
    double? Maximum { get; set; }

    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>The minimum.</value>
    double? Minimum { get; set; }

    /// <summary>
    /// Gets or sets the units.
    /// </summary>
    /// <value>The units.</value>
    string Units { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [units on new line].
    /// </summary>
    /// <value><c>true</c> if [units on new line]; otherwise, <c>false</c>.</value>
    bool UnitsOnNewLine { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [user changed].
    /// </summary>
    /// <value><c>true</c> if [user changed]; otherwise, <c>false</c>.</value>
    bool UserChanged { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    double Value { get; set; }

    /// <summary>
    /// Gets or sets the edited value.
    /// </summary>
    /// <value>The value.</value>
    double EditedValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the edited value.
    /// </summary>
    /// <value>The value.</value>
    bool UseSeperateEditValue { get; set; }

    /// <summary>
    /// Gets the value double observable.
    /// </summary>
    /// <value>The value double observable.</value>
    IObservable<(bool UserChanged, double Value)> ValueDoubleObservable { get; }

    /// <summary>
    /// Gets the value single.
    /// </summary>
    /// <value>The value single.</value>
    float ValueSingle { get; }

    /// <summary>
    /// Gets the value single observable.
    /// </summary>
    /// <value>The value single observable.</value>
    IObservable<(bool UserChanged, float Value)> ValueSingleObservable { get; }

    /// <summary>
    /// Disposes the keypad.
    /// </summary>
    void DisposeKeypad();
}
