// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a control that lets a user pick a color using sliders and a color preview.</summary>
public class ColorPicker : TemplatedControl
{
    /// <summary>Property for <see cref="Color"/>.</summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<ColorPicker, Color>(nameof(Color), Colors.Red);

    /// <summary>Property for <see cref="Red"/>.</summary>
    public static readonly StyledProperty<byte> RedProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Red), 255);

    /// <summary>Property for <see cref="Green"/>.</summary>
    public static readonly StyledProperty<byte> GreenProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Green), 0);

    /// <summary>Property for <see cref="Blue"/>.</summary>
    public static readonly StyledProperty<byte> BlueProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Blue), 0);

    /// <summary>Property for <see cref="Alpha"/>.</summary>
    public static readonly StyledProperty<byte> AlphaProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Alpha), 255);

    /// <summary>Property for <see cref="IsAlphaEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsAlphaEnabledProperty =
        AvaloniaProperty.Register<ColorPicker, bool>(nameof(IsAlphaEnabled), true);

    /// <summary>Property for <see cref="HexValue"/>.</summary>
    public static readonly StyledProperty<string> HexValueProperty =
        AvaloniaProperty.Register<ColorPicker, string>(nameof(HexValue), "#FF0000");

    /// <summary>Provides the _isUpdating member.</summary>
    private bool _isUpdating;

    /// <summary>Provides the _redSlider member.</summary>
    private global::Avalonia.Controls.Slider? _redSlider;

    /// <summary>Provides the _greenSlider member.</summary>
    private global::Avalonia.Controls.Slider? _greenSlider;

    /// <summary>Provides the _blueSlider member.</summary>
    private global::Avalonia.Controls.Slider? _blueSlider;

    /// <summary>Provides the _alphaSlider member.</summary>
    private global::Avalonia.Controls.Slider? _alphaSlider;

    /// <summary>Provides the _colorPreview member.</summary>
    private global::Avalonia.Controls.Border? _colorPreview;

    /// <summary>Provides the _hexTextBox member.</summary>
    private global::Avalonia.Controls.TextBox? _hexTextBox;

    /// <summary>Initializes a new instance of the <see cref="ColorPicker"/> class.</summary>
    public ColorPicker()
    {
        UpdateColorFromComponents();
        UpdateHexValue();
    }

    /// <summary>Gets or sets the currently selected color.</summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>Gets or sets the red component of the color (0-255).</summary>
    public byte Red
    {
        get => GetValue(RedProperty);
        set => SetValue(RedProperty, value);
    }

    /// <summary>Gets or sets the green component of the color (0-255).</summary>
    public byte Green
    {
        get => GetValue(GreenProperty);
        set => SetValue(GreenProperty, value);
    }

    /// <summary>Gets or sets the blue component of the color (0-255).</summary>
    public byte Blue
    {
        get => GetValue(BlueProperty);
        set => SetValue(BlueProperty, value);
    }

    /// <summary>Gets or sets the alpha component of the color (0-255).</summary>
    public byte Alpha
    {
        get => GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether alpha channel editing is enabled.</summary>
    public bool IsAlphaEnabled
    {
        get => GetValue(IsAlphaEnabledProperty);
        set => SetValue(IsAlphaEnabledProperty, value);
    }

    /// <summary>Gets the hex representation of the current color.</summary>
    public string HexValue
    {
        get => GetValue(HexValueProperty);
        private set => SetValue(HexValueProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null || _isUpdating)
        {
            return;
        }

        if (change.Property == ColorProperty)
        {
            UpdateComponentsFromColor();
            UpdateHexValue();
        }
        else if (change.Property == RedProperty ||
                 change.Property == GreenProperty ||
                 change.Property == BlueProperty ||
                 change.Property == AlphaProperty)
        {
            UpdateColorFromComponents();
            UpdateHexValue();
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e is null)
        {
            return;
        }

        // Unsubscribe from old sliders
        if (_redSlider is not null)
        {
            _redSlider.PropertyChanged -= OnSliderValueChanged;
        }

        if (_greenSlider is not null)
        {
            _greenSlider.PropertyChanged -= OnSliderValueChanged;
        }

        if (_blueSlider is not null)
        {
            _blueSlider.PropertyChanged -= OnSliderValueChanged;
        }

        if (_alphaSlider is not null)
        {
            _alphaSlider.PropertyChanged -= OnSliderValueChanged;
        }

        // Get template parts - use Avalonia.Controls types
        _redSlider = e.NameScope.Find<global::Avalonia.Controls.Slider>("PART_RedSlider");
        _greenSlider = e.NameScope.Find<global::Avalonia.Controls.Slider>("PART_GreenSlider");
        _blueSlider = e.NameScope.Find<global::Avalonia.Controls.Slider>("PART_BlueSlider");
        _alphaSlider = e.NameScope.Find<global::Avalonia.Controls.Slider>("PART_AlphaSlider");
        _colorPreview = e.NameScope.Find<global::Avalonia.Controls.Border>("PART_ColorPreview");
        _hexTextBox = e.NameScope.Find<global::Avalonia.Controls.TextBox>("PART_HexTextBox");

        // Subscribe to slider changes
        if (_redSlider is not null)
        {
            _redSlider.PropertyChanged += OnSliderValueChanged;
        }

        if (_greenSlider is not null)
        {
            _greenSlider.PropertyChanged += OnSliderValueChanged;
        }

        if (_blueSlider is not null)
        {
            _blueSlider.PropertyChanged += OnSliderValueChanged;
        }

        if (_alphaSlider is not null)
        {
            _alphaSlider.PropertyChanged += OnSliderValueChanged;
        }

        // Initialize values
        UpdateSlidersFromComponents();
        UpdatePreview();
        UpdateHexTextBox();
    }

    /// <summary>Provides the OnSliderValueChanged member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnSliderValueChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != RangeBase.ValueProperty || _isUpdating)
        {
            return;
        }

        _isUpdating = true;

        if (sender == _redSlider)
        {
            Red = (byte)_redSlider!.Value;
        }
        else if (sender == _greenSlider)
        {
            Green = (byte)_greenSlider!.Value;
        }
        else if (sender == _blueSlider)
        {
            Blue = (byte)_blueSlider!.Value;
        }
        else if (sender == _alphaSlider)
        {
            Alpha = (byte)_alphaSlider!.Value;
        }

        Color = new(Alpha, Red, Green, Blue);
        UpdatePreview();
        UpdateHexValue();
        UpdateHexTextBox();

        _isUpdating = false;
    }

    /// <summary>Provides the UpdateColorFromComponents member.</summary>
    private void UpdateColorFromComponents()
    {
        _isUpdating = true;
        Color = new(Alpha, Red, Green, Blue);
        UpdatePreview();
        _isUpdating = false;
    }

    /// <summary>Provides the UpdateComponentsFromColor member.</summary>
    private void UpdateComponentsFromColor()
    {
        _isUpdating = true;
        Red = Color.R;
        Green = Color.G;
        Blue = Color.B;
        Alpha = Color.A;
        UpdateSlidersFromComponents();
        UpdatePreview();
        _isUpdating = false;
    }

    /// <summary>Provides the UpdateSlidersFromComponents member.</summary>
    private void UpdateSlidersFromComponents()
    {
        _redSlider?.Value = Red;

        _greenSlider?.Value = Green;

        _blueSlider?.Value = Blue;

        _alphaSlider?.Value = Alpha;
    }

    /// <summary>Provides the UpdatePreview member.</summary>
    private void UpdatePreview() => _colorPreview?.Background = new SolidColorBrush(Color);

    /// <summary>Provides the UpdateHexValue member.</summary>
    private void UpdateHexValue() => HexValue = IsAlphaEnabled
        ? $"#{Alpha:X2}{Red:X2}{Green:X2}{Blue:X2}"
        : $"#{Red:X2}{Green:X2}{Blue:X2}";

    /// <summary>Provides the UpdateHexTextBox member.</summary>
    private void UpdateHexTextBox() => _hexTextBox?.Text = HexValue;
}
