// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>A text box for entering hex color values.</summary>
public class HexColorTextBox : TextBox
{
    /// <summary>Property for <see cref="Color"/>.</summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<HexColorTextBox, Color>(nameof(Color), Colors.Red);

    /// <summary>Provides the _updating member.</summary>
    private bool _updating;

    /// <summary>Provides the HexColorTextBox member.</summary>
    static HexColorTextBox()
    {
        _ = ColorProperty.Changed.AddClassHandler<HexColorTextBox>((x, e) => x.OnColorChanged(e));
        _ = TextProperty.Changed.AddClassHandler<HexColorTextBox>((x, e) => x.OnTextChanged(e));
    }

    /// <summary>Initializes a new instance of the <see cref="HexColorTextBox"/> class.</summary>
    public HexColorTextBox()
    {
        Text = "#FF0000";
    }

    /// <summary>Gets or sets the color.</summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>Provides the OnColorChanged member.</summary>
    /// <param name="e">The e value.</param>
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

    /// <summary>Provides the OnTextChanged member.</summary>
    /// <param name="e">The e value.</param>
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
