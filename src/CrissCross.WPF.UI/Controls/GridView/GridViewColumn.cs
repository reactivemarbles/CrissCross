// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Reflection;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extends <see cref="System.Windows.Controls.GridViewColumn"/> with MinWidth and MaxWidth properties.
/// It can be used with <see cref="ListView"/> when in GridView mode.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:ListView&gt;
///     &lt;ui:ListView.View&gt;
///         &lt;ui:GridView&gt;
///             &lt;ui:GridViewColumn
///                 MinWidth="100"
///                 MaxWidth="200"
///                 DisplayMemberBinding="{Binding FirstName}"
///                 Header="First Name" /&gt;
///         &lt;/ui:GridView&gt;
///     &lt;/ui:ListView.View&gt;
/// &lt;/ui:ListView&gt;
/// </code>
/// </example>
public class GridViewColumn : System.Windows.Controls.GridViewColumn
{
    // use reflection to get the `_desiredWidth` private field.
    private static readonly Lazy<FieldInfo> _desiredWidthField =
        new(
            () =>
                typeof(System.Windows.Controls.GridViewColumn).GetField(
                    "_desiredWidth",
                    BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("The `_desiredWidth` field was not found."));

    // use reflection to get the `UpdateActualWidth` private method.
    private static readonly Lazy<MethodInfo> _updateActualWidthMethod =
        new(() =>
        {
            var methodInfo =
                typeof(System.Windows.Controls.GridViewColumn).GetMethod(
                    "UpdateActualWidth",
                    BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("The `UpdateActualWidth` method was not found.");
            return methodInfo;
        });

    /// <summary>Identifies the <see cref="MinWidth"/> dependency property.</summary>
#pragma warning disable SA1202 // Elements should be ordered by access
    public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register(
#pragma warning restore SA1202 // Elements should be ordered by access
        nameof(MinWidth),
        typeof(double),
        typeof(GridViewColumn),
        new FrameworkPropertyMetadata(0.0, OnMinWidthChanged));

    /// <summary>Identifies the <see cref="MaxWidth"/> dependency property.</summary>
    public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register(
        nameof(MaxWidth),
        typeof(double),
        typeof(GridViewColumn),
        new FrameworkPropertyMetadata(double.PositiveInfinity, OnMaxWidthChanged));

    /// <summary>
    /// Gets or sets the minimum width of the column.
    /// </summary>
    public double MinWidth
    {
        get => (double)GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    /// <summary>
    /// gets or sets the maximum width of the column.
    /// </summary>
    public double MaxWidth
    {
        get => (double)GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    private static FieldInfo DesiredWidthField => _desiredWidthField.Value;

    private static MethodInfo UpdateActualWidthMethod => _updateActualWidthMethod.Value;

    /// <summary>
    /// Updates the desired width of the column to be clamped between MinWidth and MaxWidth).
    /// </summary>
    /// <remarks>
    /// Uses reflection to directly set the private `_desiredWidth` field on the `System.Windows.Controls.GridViewColumn`.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if reflection fails to access the `_desiredWidth` field.
    /// </exception>
    internal void UpdateDesiredWidth()
    {
        var currentWidth = (double)(
            DesiredWidthField.GetValue(this)
            ?? throw new InvalidOperationException("Failed to get the current `_desiredWidth`."));
        var clampedWidth = Math.Max(MinWidth, Math.Min(currentWidth, MaxWidth));
        DesiredWidthField.SetValue(this, clampedWidth);
        _ = UpdateActualWidthMethod.Invoke(this, null);
    }

    /// <summary>
    /// Raises the <see cref="E:MinWidthChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnMinWidthChanged(DependencyPropertyChangedEventArgs e)
    {
        // Hook for derived classes to react to MinWidth property changes
    }

    /// <summary>
    /// Raises the <see cref="E:MaxWidthChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnMaxWidthChanged(DependencyPropertyChangedEventArgs e)
    {
        // Hook for derived classes to react to MaxWidth property changes
    }

    private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GridViewColumn self)
        {
            return;
        }

        self.OnMinWidthChanged(e);
    }

    private static void OnMaxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GridViewColumn self)
        {
            return;
        }

        self.OnMaxWidthChanged(e);
    }
}
