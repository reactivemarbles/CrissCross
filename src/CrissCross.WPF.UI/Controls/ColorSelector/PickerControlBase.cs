// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI;

/// <summary>Represents PickerControlBase.</summary>
/// <seealso cref="UserControl" />
/// <seealso cref="IColorStateStorage" />
public partial class PickerControlBase : UserControl, IColorStateStorage
{
    /// <summary>The color state property.</summary>
    public static readonly DependencyProperty ColorStateProperty = DependencyProperty.Register(
        nameof(ColorState),
        typeof(ColorState),
        typeof(PickerControlBase),
        new PropertyMetadata(new ColorState(new(0, 0, 0), 1, new(0, 0, 0), new(0, 0, 0)), OnColorStatePropertyChange));

    /// <summary>The selected color property.</summary>
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
        nameof(SelectedColor),
        typeof(Color),
        typeof(PickerControlBase),
        new PropertyMetadata(Colors.Black, OnSelectedColorPropertyChange));

    /// <summary>The color changed event.</summary>
    public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ColorChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(PickerControlBase));

    /// <summary>The impossible alpha channel used to seed the previous color sentinel.</summary>
    private const byte PreviousColorSentinelChannel = 5;

    /// <summary>Stores the _ignoreColorPropertyChange value.</summary>
    private bool _ignoreColorPropertyChange;

    /// <summary>Stores the _ignoreColorChange value.</summary>
    private bool _ignoreColorChange;

    /// <summary>Stores the _previousColor value.</summary>
    private Color _previousColor = System.Windows.Media.Color.FromArgb(
        PreviousColorSentinelChannel,
        PreviousColorSentinelChannel,
        PreviousColorSentinelChannel,
        PreviousColorSentinelChannel);

    /// <summary>Occurs when [color changed].</summary>
    public event RoutedEventHandler ColorChanged
    {
        add => AddHandler(ColorChangedEvent, value);
        remove => RemoveHandler(ColorChangedEvent, value);
    }

    /// <summary>Gets or sets the state of the color.</summary>
    /// <value>
    /// The state of the color.
    /// </value>
    public ColorState ColorState
    {
        get => (ColorState)GetValue(ColorStateProperty);
        set => SetValue(ColorStateProperty, value);
    }

    /// <summary>Gets or sets the color of the selected.</summary>
    /// <value>
    /// The color of the selected.
    /// </value>
    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>Gets or sets the color.</summary>
    /// <value>
    /// The color.
    /// </value>
    public NotifyableColor Color { get; set; } = null!;

    /// <summary>Provides the OnColorStatePropertyChange member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnColorStatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args) =>
        ((PickerControlBase)d).Color.UpdateEverything((ColorState)args.OldValue);

    /// <summary>Provides the OnSelectedColorPropertyChange member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="args">The event arguments.</param>
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
