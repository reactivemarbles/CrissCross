// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI;

/// <summary>
/// PickerControlBase.
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="IColorStateStorage" />
public class PickerControlBase : UserControl, IColorStateStorage
{
    /// <summary>
    /// The color state property.
    /// </summary>
    public static readonly DependencyProperty ColorStateProperty =
        DependencyProperty.Register(
            nameof(ColorState),
            typeof(ColorState),
            typeof(PickerControlBase),
            new PropertyMetadata(new ColorState(0, 0, 0, 1, 0, 0, 0, 0, 0, 0), OnColorStatePropertyChange));

    /// <summary>
    /// The selected color property.
    /// </summary>
    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(Color),
            typeof(PickerControlBase),
            new PropertyMetadata(Colors.Black, OnSelectedColorPropertyChange));

    /// <summary>
    /// The color changed event.
    /// </summary>
    public static readonly RoutedEvent ColorChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(ColorChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(PickerControlBase));

    private bool _ignoreColorPropertyChange;
    private bool _ignoreColorChange;
    private Color _previousColor = System.Windows.Media.Color.FromArgb(5, 5, 5, 5);

    /// <summary>
    /// Initializes a new instance of the <see cref="PickerControlBase"/> class.
    /// </summary>
    public PickerControlBase()
    {
        Color = new NotifyableColor(this);
        Color.PropertyChanged += (sender, args) =>
        {
            var newColor = System.Windows.Media.Color.FromArgb(
                (byte)Math.Round(Color.A),
                (byte)Math.Round(Color.RGB_R),
                (byte)Math.Round(Color.RGB_G),
                (byte)Math.Round(Color.RGB_B));
            if (newColor != _previousColor)
            {
                RaiseEvent(new ColorRoutedEventArgs(ColorChangedEvent, newColor));
                _previousColor = newColor;
            }
        };
        ColorChanged += (sender, newColor) =>
        {
            if (!_ignoreColorChange)
            {
                _ignoreColorPropertyChange = true;
                SelectedColor = ((ColorRoutedEventArgs)newColor).Color;
                _ignoreColorPropertyChange = false;
            }
        };
    }

    /// <summary>
    /// Occurs when [color changed].
    /// </summary>
    public event RoutedEventHandler ColorChanged
    {
        add => AddHandler(ColorChangedEvent, value);
        remove => RemoveHandler(ColorChangedEvent, value);
    }

    /// <summary>
    /// Gets or sets the state of the color.
    /// </summary>
    /// <value>
    /// The state of the color.
    /// </value>
    public ColorState ColorState
    {
        get => (ColorState)GetValue(ColorStateProperty);
        set => SetValue(ColorStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the selected.
    /// </summary>
    /// <value>
    /// The color of the selected.
    /// </value>
    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public NotifyableColor Color
    {
        get;
        set;
    }

    private static void OnColorStatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args) =>
        ((PickerControlBase)d).Color.UpdateEverything((ColorState)args.OldValue);

    private static void OnSelectedColorPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        var sender = (PickerControlBase)d;
        if (sender._ignoreColorPropertyChange)
        {
            return;
        }

        var newValue = (Color)args.NewValue;
        sender._ignoreColorChange = true;
        sender.Color.A = newValue.A;
        sender.Color.RGB_R = newValue.R;
        sender.Color.RGB_G = newValue.G;
        sender.Color.RGB_B = newValue.B;
        sender._ignoreColorChange = false;
    }
}
