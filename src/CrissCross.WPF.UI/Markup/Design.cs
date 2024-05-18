// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Markup;

/// <summary>
/// Custom design time attributes based on Marcin Najder implementation.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:FluentWindow
///     xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"
///     ui:Design.Background="{DynamicResource ApplicationBackgroundBrush}"
///     ui:Design.Foreground="{DynamicResource TextFillColorPrimaryBrush}"&gt;
///     &lt;Button Content="Hello World" /&gt;
/// &lt;/FluentWindow&gt;
/// </code>
/// </example>
public static class Design
{
    /// <summary>
    /// The background property.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    public static DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
        "Background",
        typeof(Brush),
        typeof(Design),
        new PropertyMetadata(OnBackgroundPropertyChanged));

    /// <summary>
    /// The foreground property.
    /// </summary>
    public static DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(
        "Foreground",
        typeof(Brush),
        typeof(Design),
        new PropertyMetadata(OnForegroundPropertyChanged));
#pragma warning restore SA1401 // Fields should be private

    private const string DesignProcessName = "devenv";

    private static bool? _inDesignMode;

    /// <summary>
    /// Gets a value indicating whether indicates whether or not the framework is in design-time mode. (Caliburn.Micro implementation).
    /// </summary>
    private static bool InDesignMode
    {
        get
        {
            if (_inDesignMode != null)
            {
                return _inDesignMode ?? false;
            }

            _inDesignMode = (bool)DependencyPropertyDescriptor
                    .FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement))
                    .Metadata.DefaultValue;

            if (
                !(_inDesignMode ?? false)
                && System
                    .Diagnostics.Process.GetCurrentProcess()
                    .ProcessName.StartsWith(DesignProcessName, System.StringComparison.Ordinal))
            {
                _inDesignMode = true;
            }

            return _inDesignMode ?? false;
        }
    }

    /// <summary>
    /// Gets the background.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A Brush.</returns>
    public static Brush? GetBackground(DependencyObject dependencyObject) =>
        dependencyObject?.GetValue(BackgroundProperty) as Brush;

    /// <summary>
    /// Sets the background.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetBackground(DependencyObject dependencyObject, Brush? value) =>
        dependencyObject?.SetValue(BackgroundProperty, value);

    /// <summary>
    /// Gets the foreground.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A Brush.</returns>
    public static Brush? GetForeground(DependencyObject dependencyObject) =>
        dependencyObject?.GetValue(ForegroundProperty) as Brush;

    /// <summary>
    /// Sets the foreground.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetForeground(DependencyObject dependencyObject, Brush? value) =>
        dependencyObject?.SetValue(ForegroundProperty, value);

    private static void OnBackgroundPropertyChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (!InDesignMode)
        {
            return;
        }

        d?.GetType()?.GetProperty("Background")?.SetValue(d, e.NewValue, null);
    }

    private static void OnForegroundPropertyChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (!InDesignMode)
        {
            return;
        }

        d?.GetType()?.GetProperty("Foreground")?.SetValue(d, e.NewValue, null);
    }
}
