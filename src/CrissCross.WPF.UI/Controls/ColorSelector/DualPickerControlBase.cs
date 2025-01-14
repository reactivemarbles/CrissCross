// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// DualPickerControlBase.
/// </summary>
/// <seealso cref="PickerControlBase" />
/// <seealso cref="ISecondColorStorage" />
/// <seealso cref="IHintColorStateStorage" />
public class DualPickerControlBase : PickerControlBase, ISecondColorStorage, IHintColorStateStorage
{
    /// <summary>
    /// The second color state property.
    /// </summary>
    public static readonly DependencyProperty SecondColorStateProperty =
            DependencyProperty.Register(
                nameof(SecondColorState),
                typeof(ColorState),
                typeof(DualPickerControlBase),
                new PropertyMetadata(new ColorState(1, 1, 1, 1, 0, 0, 1, 0, 0, 1), OnSecondColorStatePropertyChange));

    /// <summary>
    /// The secondary color property.
    /// </summary>
    public static readonly DependencyProperty SecondaryColorProperty =
        DependencyProperty.Register(
            nameof(SecondaryColor),
            typeof(Color),
            typeof(DualPickerControlBase),
            new PropertyMetadata(Colors.White, OnSecondaryColorPropertyChange));

    /// <summary>
    /// The hint color state property.
    /// </summary>
    public static readonly DependencyProperty HintColorStateProperty =
        DependencyProperty.Register(
            nameof(HintColorState),
            typeof(ColorState),
            typeof(DualPickerControlBase),
            new PropertyMetadata(new ColorState(0, 0, 0, 0, 0, 0, 0, 0, 0, 0), OnHintColorStatePropertyChange));

    /// <summary>
    /// The hint color property.
    /// </summary>
    public static readonly DependencyProperty HintColorProperty =
        DependencyProperty.Register(
            nameof(HintColor),
            typeof(Color),
            typeof(DualPickerControlBase),
            new PropertyMetadata(Colors.Transparent, OnHintColorPropertyChanged));

    /// <summary>
    /// The use hint color property.
    /// </summary>
    public static readonly DependencyProperty UseHintColorProperty =
        DependencyProperty.Register(nameof(UseHintColor), typeof(bool), typeof(DualPickerControlBase), new PropertyMetadata(false));

    private readonly SecondColorDecorator _secondColorDecorator;
    private readonly HintColorDecorator _hintColorDecorator;
    private bool _ignoreSecondaryColorChange;

    private bool _ignoreSecondaryColorPropertyChange;

    private bool _ignoreHintNotifyableColorChange;

    private bool _ignoreHintColorPropertyChange;

    /// <summary>
    /// Initializes a new instance of the <see cref="DualPickerControlBase"/> class.
    /// </summary>
    public DualPickerControlBase()
    {
        _secondColorDecorator = new SecondColorDecorator(this);
        _hintColorDecorator = new HintColorDecorator(this);

        SecondColor = new NotifyableColor(_secondColorDecorator);
        SecondColor.PropertyChanged += (sender, args) =>
        {
            if (!_ignoreSecondaryColorChange)
            {
                _ignoreSecondaryColorPropertyChange = true;
                SecondaryColor = System.Windows.Media.Color.FromArgb(
                    (byte)Math.Round(SecondColor.A),
                    (byte)Math.Round(SecondColor.RGB_R),
                    (byte)Math.Round(SecondColor.RGB_G),
                    (byte)Math.Round(SecondColor.RGB_B));
                _ignoreSecondaryColorPropertyChange = false;
            }
        };

        HintNotifyableColor = new NotifyableColor(_hintColorDecorator);
        HintNotifyableColor.PropertyChanged += (sender, args) =>
        {
            if (!_ignoreHintNotifyableColorChange)
            {
                _ignoreHintColorPropertyChange = true;
                HintColor = System.Windows.Media.Color.FromArgb(
                    (byte)Math.Round(HintNotifyableColor.A),
                    (byte)Math.Round(HintNotifyableColor.RGB_R),
                    (byte)Math.Round(HintNotifyableColor.RGB_G),
                    (byte)Math.Round(HintNotifyableColor.RGB_B));
                _ignoreHintColorPropertyChange = false;
            }
        };
    }

    /// <summary>
    /// Gets or sets the state of the second color.
    /// </summary>
    /// <value>
    /// The state of the second color.
    /// </value>
    public ColorState SecondColorState
    {
        get => (ColorState)GetValue(SecondColorStateProperty);
        set => SetValue(SecondColorStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the second.
    /// </summary>
    /// <value>
    /// The color of the second.
    /// </value>
    public NotifyableColor SecondColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the secondary.
    /// </summary>
    /// <value>
    /// The color of the secondary.
    /// </value>
    public Color SecondaryColor
    {
        get => (Color)GetValue(SecondaryColorProperty);
        set => SetValue(SecondaryColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the state of the hint color.
    /// </summary>
    /// <value>
    /// The state of the hint color.
    /// </value>
    public ColorState HintColorState
    {
        get => (ColorState)GetValue(HintColorStateProperty);
        set => SetValue(HintColorStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the hint notifyable.
    /// </summary>
    /// <value>
    /// The color of the hint notifyable.
    /// </value>
    public NotifyableColor HintNotifyableColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the hint.
    /// </summary>
    /// <value>
    /// The color of the hint.
    /// </value>
    public Color HintColor
    {
        get => (Color)GetValue(HintColorProperty);
        set => SetValue(HintColorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [use hint color].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [use hint color]; otherwise, <c>false</c>.
    /// </value>
    public bool UseHintColor
    {
        get => (bool)GetValue(UseHintColorProperty);
        set => SetValue(UseHintColorProperty, value);
    }

    /// <summary>
    /// Swaps the colors.
    /// </summary>
    public void SwapColors() => (SecondColorState, ColorState) = (ColorState, SecondColorState);

    /// <summary>
    /// Sets the color of the main color from hint.
    /// </summary>
    public void SetMainColorFromHintColor() => ColorState = HintColorState;

    private static void OnSecondColorStatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args) =>
        ((DualPickerControlBase)d).SecondColor.UpdateEverything((ColorState)args.OldValue);

    private static void OnHintColorStatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args) =>
        ((DualPickerControlBase)d).HintNotifyableColor.UpdateEverything((ColorState)args.OldValue);

    private static void OnHintColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        var sender = (DualPickerControlBase)d;
        var newValue = (Color)args.NewValue;
        if (sender._ignoreHintColorPropertyChange)
        {
            return;
        }

        sender._ignoreHintNotifyableColorChange = true;
        sender.HintNotifyableColor.A = newValue.A;
        sender.HintNotifyableColor.RGB_R = newValue.R;
        sender.HintNotifyableColor.RGB_G = newValue.G;
        sender.HintNotifyableColor.RGB_B = newValue.B;
        sender._ignoreHintNotifyableColorChange = false;
    }

    private static void OnSecondaryColorPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        var sender = (DualPickerControlBase)d;
        if (sender._ignoreSecondaryColorPropertyChange)
        {
            return;
        }

        var newValue = (Color)args.NewValue;
        sender._ignoreSecondaryColorChange = true;
        sender.SecondColor.A = newValue.A;
        sender.SecondColor.RGB_R = newValue.R;
        sender.SecondColor.RGB_G = newValue.G;
        sender.SecondColor.RGB_B = newValue.B;
        sender._ignoreSecondaryColorChange = false;
    }
}
