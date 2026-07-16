// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Contains picker initialization behavior.</summary>
public partial class PickerControlBase
{
    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        Color = new(this);
        Color.PropertyChanged += (_, _) => UpdateSelectedColor();
        ColorChanged += (_, args) => ApplySelectedColor(args);
    }

    /// <summary>Updates the selected color from the notifyable color.</summary>
    private void UpdateSelectedColor()
    {
        var newColor = System.Windows.Media.Color.FromArgb(
            (byte)Math.Round(Color.A),
            (byte)Math.Round(Color.RGB_R),
            (byte)Math.Round(Color.RGB_G),
            (byte)Math.Round(Color.RGB_B));
        if (newColor == _previousColor)
        {
            return;
        }

        RaiseEvent(new ColorRoutedEventArgs(ColorChangedEvent, newColor));
        _previousColor = newColor;
    }

    /// <summary>Applies the selected routed color.</summary>
    /// <param name="args">The routed color arguments.</param>
    private void ApplySelectedColor(RoutedEventArgs args)
    {
        if (_ignoreColorChange)
        {
            return;
        }

        _ignoreColorPropertyChange = true;
        SelectedColor = ((ColorRoutedEventArgs)args).Color;
        _ignoreColorPropertyChange = false;
    }
}
