// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Markup;

/// <summary>Represents Design.</summary>
public static class Design
{
    /// <summary>Provides the DesignProcessName member.</summary>
    private const string DesignProcessName = "devenv";

    /// <summary>Stores the _inDesignMode value.</summary>
    private static bool? _inDesignMode;

    /// <summary>Gets the background property.</summary>
    public static DependencyProperty BackgroundProperty { get; } = DependencyProperty.RegisterAttached(
        "Background",
        typeof(Brush),
        typeof(Design),
        new PropertyMetadata(OnBackgroundPropertyChanged));

    /// <summary>Gets the foreground property.</summary>
    public static DependencyProperty ForegroundProperty { get; } = DependencyProperty.RegisterAttached(
        "Foreground",
        typeof(Brush),
        typeof(Design),
        new PropertyMetadata(OnForegroundPropertyChanged));

    /// <summary>Gets a value indicating whether indicates whether or not the framework is in design-time mode. (Caliburn.Micro implementation).</summary>
    private static bool InDesignMode
    {
        get
        {
            if (_inDesignMode.HasValue)
            {
                return _inDesignMode.Value;
            }

            var inDesignMode = (bool)DependencyPropertyDescriptor
                    .FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement))
                    .Metadata.DefaultValue;

            if (
                !inDesignMode
                && Process.GetCurrentProcess()
                    .ProcessName.StartsWith(DesignProcessName, System.StringComparison.Ordinal))
            {
                inDesignMode = true;
            }

            _inDesignMode = inDesignMode;
            return inDesignMode;
        }
    }

    /// <summary>Gets the background.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A Brush.</returns>
    public static Brush? GetBackground(DependencyObject dependencyObject) =>
        dependencyObject?.GetValue(BackgroundProperty) as Brush;

    /// <summary>Sets the background.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetBackground(DependencyObject dependencyObject, Brush? value) =>
        dependencyObject?.SetValue(BackgroundProperty, value);

    /// <summary>Gets the foreground.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A Brush.</returns>
    public static Brush? GetForeground(DependencyObject dependencyObject) =>
        dependencyObject?.GetValue(ForegroundProperty) as Brush;

    /// <summary>Sets the foreground.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetForeground(DependencyObject dependencyObject, Brush? value) =>
        dependencyObject?.SetValue(ForegroundProperty, value);

    /// <summary>Provides the OnBackgroundPropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnBackgroundPropertyChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (!InDesignMode)
        {
            return;
        }

        d?.GetType()?.GetProperty("Background")?.SetValue(d, e.NewValue, null);
    }

    /// <summary>Provides the OnForegroundPropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnForegroundPropertyChanged(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (!InDesignMode)
        {
            return;
        }

        d?.GetType()?.GetProperty("Foreground")?.SetValue(d, e.NewValue, null);
    }
}
