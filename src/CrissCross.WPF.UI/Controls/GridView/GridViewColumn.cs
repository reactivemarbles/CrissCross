// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extends GridViewColumn with MinWidth and MaxWidth properties.</summary>
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
    /// <summary>Identifies the <see cref="MinWidth"/> dependency property.</summary>
    public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register(
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

    /// <summary>Uses reflection to get the `_desiredWidth` private field.</summary>
    private static readonly Lazy<FieldInfo> _desiredWidthField = new(() =>
        typeof(System.Windows.Controls.GridViewColumn).GetField(
            "_desiredWidth",
            BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("The `_desiredWidth` field was not found."));

    /// <summary>Uses reflection to get the `UpdateActualWidth` private method.</summary>
    private static readonly Lazy<MethodInfo> _updateActualWidthMethod = new(() =>
        typeof(System.Windows.Controls.GridViewColumn).GetMethod(
            "UpdateActualWidth",
            BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("The `UpdateActualWidth` method was not found."));

    /// <summary>Gets or sets the minimum width of the column.</summary>
    public double MinWidth
    {
        get => (double)GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    /// <summary>Gets or sets the gets or sets the maximum width of the column. value.</summary>
    public double MaxWidth
    {
        get => (double)GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    /// <summary>Gets the DesiredWidthField value.</summary>
    private static FieldInfo DesiredWidthField => _desiredWidthField.Value;

    /// <summary>Gets the UpdateActualWidthMethod value.</summary>
    private static MethodInfo UpdateActualWidthMethod => _updateActualWidthMethod.Value;

    /// <summary>Updates the desired width of the column to be clamped between MinWidth and MaxWidth).</summary>
    /// <remarks>
    /// Uses reflection to directly set the private `_desiredWidth` field on the
    /// `System.Windows.Controls.GridViewColumn`.
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

    /// <summary>Raises the <see cref="E:MinWidthChanged" /> event.</summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnMinWidthChanged(DependencyPropertyChangedEventArgs e)
    {
        // Hook for derived classes to react to MinWidth property changes
    }

    /// <summary>Raises the <see cref="E:MaxWidthChanged" /> event.</summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnMaxWidthChanged(DependencyPropertyChangedEventArgs e)
    {
        // Hook for derived classes to react to MaxWidth property changes
    }

    /// <summary>Provides the OnMinWidthChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GridViewColumn self)
        {
            return;
        }

        self.OnMinWidthChanged(e);
    }

    /// <summary>Provides the OnMaxWidthChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnMaxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GridViewColumn self)
        {
            return;
        }

        self.OnMaxWidthChanged(e);
    }
}
