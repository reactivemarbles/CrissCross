// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A text box for entering hex color values.
/// </summary>
public class HexColorTextBox : TextBox
{
    /// <summary>
    /// Property for <see cref="Color"/>.
    /// </summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<HexColorTextBox, Color>(nameof(Color), Colors.Red);

    private bool _updating;

    static HexColorTextBox()
    {
        ColorProperty.Changed.AddClassHandler<HexColorTextBox>((x, e) => x.OnColorChanged(e));
        TextProperty.Changed.AddClassHandler<HexColorTextBox>((x, e) => x.OnTextChanged(e));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HexColorTextBox"/> class.
    /// </summary>
    public HexColorTextBox()
    {
        Text = "#FF0000";
    }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private void OnColorChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_updating)
        {
            return;
        }

        _updating = true;
        try
        {
            var color = (Color)e.NewValue!;
            Text = color.ToString();
        }
        finally
        {
            _updating = false;
        }
    }

    private void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_updating)
        {
            return;
        }

        _updating = true;
        try
        {
            var text = (string?)e.NewValue;
            if (!string.IsNullOrEmpty(text) && Color.TryParse(text, out var color))
            {
                Color = color;
            }
        }
        finally
        {
            _updating = false;
        }
    }
}
