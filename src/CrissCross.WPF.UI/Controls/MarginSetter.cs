// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Margin Setter.</summary>
public static class MarginSetter
{
    /// <summary>The margin property.</summary>
    public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
        "Margin",
        typeof(Thickness),
        typeof(MarginSetter),
        new UIPropertyMetadata(default(Thickness), MarginChangedCallback));

    /// <summary>Gets the margin.</summary>
    /// <param name="obj">The object.</param>
    /// <returns>A Value.</returns>
    public static Thickness GetMargin(DependencyObject obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (Thickness)obj.GetValue(MarginProperty);
    }

    /// <summary>Handles margin changes.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        // Make sure this is put on a panel
        if (sender is not Panel panel)
        {
            return;
        }

        panel.Loaded += Panel_Loaded;
    }

    /// <summary>Sets the margin.</summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetMargin(DependencyObject obj, Thickness value)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValue(MarginProperty, value);
    }

    /// <summary>Handles the Loaded event of the panel control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private static void Panel_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        // Go over the children and set margin for them:
        foreach (var child in panel.Children)
        {
            if (child is not FrameworkElement fe)
            {
                continue;
            }

            fe.Margin = GetMargin(panel);
        }
    }
}
