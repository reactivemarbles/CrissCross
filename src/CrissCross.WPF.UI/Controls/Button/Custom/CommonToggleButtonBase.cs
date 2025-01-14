// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using System.Windows.Markup;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// CommonToggleButtonBase.
/// </summary>
/// <seealso cref="System.Windows.Controls.Primitives.ToggleButton" />
public abstract class CommonToggleButtonBase : System.Windows.Controls.Primitives.ToggleButton
{
    /// <summary>
    /// The corner radius1 property.
    /// </summary>
    public static readonly DependencyProperty CornerRadius1Property = DependencyProperty.Register("CornerRadius1", typeof(CornerRadius), typeof(CommonToggleButtonBase), new PropertyMetadata(new CornerRadius(3.0)));

    /// <summary>
    /// The corner radius2 property.
    /// </summary>
    public static readonly DependencyProperty CornerRadius2Property = DependencyProperty.Register("CornerRadius2", typeof(CornerRadius), typeof(CommonToggleButtonBase), new PropertyMetadata(new CornerRadius(2.0)));

    /// <summary>
    /// The focus border thickness property.
    /// </summary>
    public static readonly DependencyProperty FocusBorderThicknessProperty = DependencyProperty.Register("FocusBorderThickness", typeof(Thickness), typeof(CommonToggleButtonBase), new PropertyMetadata(new Thickness(2.0)));

    /// <summary>
    /// The focus brush property.
    /// </summary>
    public static readonly DependencyProperty FocusBrushProperty = DependencyProperty.Register("FocusBrush", typeof(Brush), typeof(CommonToggleButtonBase), new PropertyMetadata(Brushes.Orange));

    /// <summary>
    /// The glare brush property.
    /// </summary>
    public static readonly DependencyProperty GlareBrushProperty = DependencyProperty.Register("GlareBrush", typeof(Brush), typeof(CommonToggleButtonBase), new PropertyMetadata(null));

    /// <summary>
    /// The minor border brush1 property.
    /// </summary>
    public static readonly DependencyProperty MinorBorderBrush1Property = DependencyProperty.Register("MinorBorderBrush1", typeof(Brush), typeof(CommonToggleButtonBase), new PropertyMetadata(new LinearGradientBrush(SystemColors.ControlDarkDarkColor, SystemColors.ControlDarkDarkColor, 45)));

    /// <summary>
    /// The minor border thickness1 property.
    /// </summary>
    public static readonly DependencyProperty MinorBorderThickness1Property = DependencyProperty.Register("MinorBorderThickness1", typeof(Thickness), typeof(CommonToggleButtonBase), new PropertyMetadata(new Thickness(0.0)));

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonToggleButtonBase"/> class.
    /// </summary>
    /// <param name="styleName">Name of the style.</param>
    protected CommonToggleButtonBase(string styleName)
    {
        try
        {
            var style = CommonToggleButtonBase.ControlStyle(styleName);
            if (style == null)
            {
                return;
            }

            Style = style;
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(exception.Message, exception.TargetSite?.ToString(), System.Windows.MessageBoxButton.OK, MessageBoxImage.Hand);
        }

        IsEnabledChanged += CommonButtonBase_IsEnabledChanged;
        Loaded += CommonToggleButtonBase_Loaded;
        Checked += (sender, e) =>
        {
            if (sender is CommonToggleButtonBase button && button.IsChecked is bool isChecked)
            {
                    if (isChecked)
                    {
                        button.Content = "Checked";
                    }
                    else
                    {
                        button.Content = "Unchecked";
                    }
            }
        };
    }

    /// <summary>
    /// Gets or sets the corner radius1.
    /// </summary>
    /// <value>
    /// The corner radius1.
    /// </value>
    public CornerRadius CornerRadius1
    {
        get => (CornerRadius)GetValue(CornerRadius1Property);
        set => SetValue(CornerRadius1Property, value);
    }

    /// <summary>
    /// Gets or sets the corner radius2.
    /// </summary>
    /// <value>
    /// The corner radius2.
    /// </value>
    public CornerRadius CornerRadius2
    {
        get => (CornerRadius)GetValue(CornerRadius2Property);
        set => SetValue(CornerRadius2Property, value);
    }

    /// <summary>
    /// Gets or sets the focus border thickness.
    /// </summary>
    /// <value>
    /// The focus border thickness.
    /// </value>
    public Thickness FocusBorderThickness
    {
        get => (Thickness)GetValue(FocusBorderThicknessProperty);
        set => SetValue(FocusBorderThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the focus brush.
    /// </summary>
    /// <value>
    /// The focus brush.
    /// </value>
    public Brush FocusBrush
    {
        get => (Brush)GetValue(FocusBrushProperty);
        set => SetValue(FocusBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the glare brush.
    /// </summary>
    /// <value>
    /// The glare brush.
    /// </value>
    public Brush GlareBrush
    {
        get => (Brush)GetValue(GlareBrushProperty);
        set => SetValue(GlareBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the minor border brush1.
    /// </summary>
    /// <value>
    /// The minor border brush1.
    /// </value>
    public Brush MinorBorderBrush1
    {
        get => (Brush)GetValue(MinorBorderBrush1Property);
        set => SetValue(MinorBorderBrush1Property, value);
    }

    /// <summary>
    /// Gets or sets the minor border thickness1.
    /// </summary>
    /// <value>
    /// The minor border thickness1.
    /// </value>
    public Thickness MinorBorderThickness1
    {
        get => (Thickness)GetValue(MinorBorderThickness1Property);
        set => SetValue(MinorBorderThickness1Property, value);
    }

    /// <summary>
    /// Controls the style.
    /// </summary>
    /// <param name="styleName">Name of the style.</param>
    /// <returns>A Style.</returns>
    protected static Style? ControlStyle(string styleName)
    {
        try
        {
            var manifestResourceStream = typeof(CommonToggleButtonBase).Module.Assembly.GetManifestResourceStream(styleName + ".xaml");
            if (manifestResourceStream == null)
            {
                return null;
            }

            var style = (Style)XamlReader.Load(manifestResourceStream);
            manifestResourceStream.Close();
            return style;
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(exception.ToString(), exception.TargetSite?.ToString());
            return null;
        }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.UIElement.LostFocus" /> routed event by using the event data that is provided.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs" /> that contains event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus" /> event.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        SetIndicatorBorderSize(0.0);
    }

    /// <summary>
    /// Provides class handling for the <see cref="P:System.Windows.Controls.Primitives.ButtonBase.ClickMode" /> routed event that occurs when the mouse enters this control.
    /// </summary>
    /// <param name="e">The event data for the <see cref="E:System.Windows.Input.Mouse.MouseEnter" /> event.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        SetIndicatorBorderSize(2.0);
    }

    /// <summary>
    /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeave" /> routed event that occurs when the mouse leaves an element.
    /// </summary>
    /// <param name="e">The event data for the <see cref="E:System.Windows.Input.Mouse.MouseLeave" /> event.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (IsFocused)
        {
            SetIndicatorBorderSize(1.0);
        }
        else
        {
            SetIndicatorBorderSize(0.0);
        }
    }

    /// <summary>
    /// Returns a value that indicates whether serialization processes should serialize the value for the provided dependency property.
    /// </summary>
    /// <param name="dp">The identifier for the dependency property that should be serialized.</param>
    /// <returns>
    /// true if the dependency property that is supplied should be value-serialized; otherwise, false.
    /// </returns>
    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (dp == StyleProperty)
        {
            return false;
        }

        return base.ShouldSerializeProperty(dp);
    }

    private void CommonButtonBase_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var border = UserHintBorder();

        if (border != null)
        {
            if (IsEnabled)
            {
                border.Background = null;
            }
            else
            {
                border.Background = new SolidColorBrush(Color.FromArgb(0x55, 0, 0, 0));
            }
        }
    }

    private void CommonToggleButtonBase_Loaded(object sender, RoutedEventArgs e) => SetIndicatorBorderSize(0.0);

    private void SetIndicatorBorderSize(double size)
    {
        var border = UserHintBorder();

        if (border != null)
        {
            if (size == 1)
            {
                border.BorderThickness = new Thickness(
                    FocusBorderThickness.Left <= 2 ? 1 : (FocusBorderThickness.Left - 2),
                    FocusBorderThickness.Top <= 2 ? 1 : (FocusBorderThickness.Top - 2),
                    FocusBorderThickness.Right <= 2 ? 1 : (FocusBorderThickness.Right - 2),
                    FocusBorderThickness.Bottom <= 2 ? 1 : (FocusBorderThickness.Bottom - 2));
                if (GetType() == typeof(BezelToggleButton))
                {
                    border.Margin = new Thickness(0);
                    if (border.BorderThickness.Left == 1.0)
                    {
                        border.Margin = new Thickness(1.0);
                    }
                }
            }
            else if (size == 0)
            {
                border.BorderThickness = new Thickness(0.0);
            }
            else
            {
                border.BorderThickness = FocusBorderThickness;
            }
        }
    }

    private Border? UserHintBorder()
    {
        if (Template == null)
        {
            return null;
        }

        return Template.FindName("PART_UserHintBorder", this) as Border;
    }
}
