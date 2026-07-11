// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents CommonRepeatButtonBase.</summary>
public abstract class CommonRepeatButtonBase : RepeatButton
{
    /// <summary>The corner radius1 property.</summary>
    public static readonly DependencyProperty CornerRadius1Property = DependencyProperty.Register(nameof(CornerRadius1), typeof(CornerRadius), typeof(CommonRepeatButtonBase), new PropertyMetadata(new CornerRadius(3.0)));

    /// <summary>The corner radius2 property.</summary>
    public static readonly DependencyProperty CornerRadius2Property = DependencyProperty.Register(nameof(CornerRadius2), typeof(CornerRadius), typeof(CommonRepeatButtonBase), new PropertyMetadata(new CornerRadius(2.0)));

    /// <summary>The focus border thickness property.</summary>
    public static readonly DependencyProperty FocusBorderThicknessProperty = DependencyProperty.Register(nameof(FocusBorderThickness), typeof(Thickness), typeof(CommonRepeatButtonBase), new PropertyMetadata(new Thickness(2.0)));

    /// <summary>The focus brush property.</summary>
    public static readonly DependencyProperty FocusBrushProperty = DependencyProperty.Register(nameof(FocusBrush), typeof(Brush), typeof(CommonRepeatButtonBase), new PropertyMetadata(Brushes.Orange));

    /// <summary>The glare brush property.</summary>
    public static readonly DependencyProperty GlareBrushProperty = DependencyProperty.Register(nameof(GlareBrush), typeof(Brush), typeof(CommonRepeatButtonBase), new PropertyMetadata(null));

    /// <summary>The minor border brush1 property.</summary>
    public static readonly DependencyProperty MinorBorderBrush1Property = DependencyProperty.Register(nameof(MinorBorderBrush1), typeof(Brush), typeof(CommonRepeatButtonBase), new PropertyMetadata(new LinearGradientBrush(SystemColors.ControlDarkDarkColor, SystemColors.ControlDarkDarkColor, 45)));

    /// <summary>The minor border thickness1 property.</summary>
    public static readonly DependencyProperty MinorBorderThickness1Property = DependencyProperty.Register(nameof(MinorBorderThickness1), typeof(Thickness), typeof(CommonRepeatButtonBase), new PropertyMetadata(new Thickness(0.0)));

    /// <summary>The inset subtracted from the configured focus border thickness.</summary>
    private const double FocusBorderInset = 2d;

    /// <summary>The border thickness used while the pointer hovers over the button.</summary>
    private const double HoverIndicatorBorderSize = 2d;

    /// <summary>Initializes a new instance of the <see cref="CommonRepeatButtonBase"/> class.</summary>
    /// <param name="styleName">Name of the style.</param>
    protected CommonRepeatButtonBase(string styleName)
    {
        try
        {
            var style = CommonRepeatButtonBase.ControlStyle(styleName);
            if (style is null)
            {
                return;
            }

            Style = style;
        }
        catch (Exception exception)
        {
            _ = System.Windows.MessageBox.Show(exception.Message, exception.TargetSite?.ToString(), System.Windows.MessageBoxButton.OK, MessageBoxImage.Hand);
        }

        IsEnabledChanged += CommonButtonBase_IsEnabledChanged;
        Loaded += CommonRepeatButtonBase_Loaded;
    }

    /// <summary>Gets or sets the corner radius1.</summary>
    /// <value>
    /// The corner radius1.
    /// </value>
    public CornerRadius CornerRadius1
    {
        get => (CornerRadius)GetValue(CornerRadius1Property);
        set => SetValue(CornerRadius1Property, value);
    }

    /// <summary>Gets or sets the corner radius2.</summary>
    /// <value>
    /// The corner radius2.
    /// </value>
    public CornerRadius CornerRadius2
    {
        get => (CornerRadius)GetValue(CornerRadius2Property);
        set => SetValue(CornerRadius2Property, value);
    }

    /// <summary>Gets or sets the focus border thickness.</summary>
    /// <value>
    /// The focus border thickness.
    /// </value>
    public Thickness FocusBorderThickness
    {
        get => (Thickness)GetValue(FocusBorderThicknessProperty);
        set => SetValue(FocusBorderThicknessProperty, value);
    }

    /// <summary>Gets or sets the focus brush.</summary>
    /// <value>
    /// The focus brush.
    /// </value>
    public Brush FocusBrush
    {
        get => (Brush)GetValue(FocusBrushProperty);
        set => SetValue(FocusBrushProperty, value);
    }

    /// <summary>Gets or sets the glare brush.</summary>
    /// <value>
    /// The glare brush.
    /// </value>
    public Brush GlareBrush
    {
        get => (Brush)GetValue(GlareBrushProperty);
        set => SetValue(GlareBrushProperty, value);
    }

    /// <summary>Gets or sets the minor border brush1.</summary>
    /// <value>
    /// The minor border brush1.
    /// </value>
    public Brush MinorBorderBrush1
    {
        get => (Brush)GetValue(MinorBorderBrush1Property);
        set => SetValue(MinorBorderBrush1Property, value);
    }

    /// <summary>Gets or sets the minor border thickness1.</summary>
    /// <value>
    /// The minor border thickness1.
    /// </value>
    public Thickness MinorBorderThickness1
    {
        get => (Thickness)GetValue(MinorBorderThickness1Property);
        set => SetValue(MinorBorderThickness1Property, value);
    }

    /// <summary>Controls the style.</summary>
    /// <param name="styleName">Name of the style.</param>
    /// <returns>A Style.</returns>
    protected static Style? ControlStyle(string styleName)
    {
        Style? style = null;
        Stream? manifestResourceStream = null;
        try
        {
            manifestResourceStream = typeof(CommonRepeatButtonBase).Module.Assembly.GetManifestResourceStream(styleName + ".xaml");
            if (manifestResourceStream is null)
            {
                return null;
            }

            style = (Style)XamlReader.Load(manifestResourceStream);
            manifestResourceStream.Close();
            return style;
        }
        catch (Exception exception)
        {
            _ = System.Windows.MessageBox.Show(exception.ToString(), exception.TargetSite?.ToString());
            return null;
        }
    }

    /// <summary>Raises the <see cref="E:System.Windows.UIElement.LostFocus" /> routed event by using the event data that is provided.</summary>
    /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs" /> that contains event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus" /> event.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        SetIndicatorBorderSize(0);
    }

    /// <summary>Reports when the mouse enters an element.</summary>
    /// <param name="e">The event data for a <see cref="E:System.Windows.UIElement.MouseEnter" /> event.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        SetIndicatorBorderSize(HoverIndicatorBorderSize);
    }

    /// <summary>Reports when the mouse leaves an element.</summary>
    /// <param name="e">The event data for a <see cref="E:System.Windows.UIElement.MouseLeave" /> event.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (IsFocused)
        {
            SetIndicatorBorderSize(1);
        }
        else
        {
            SetIndicatorBorderSize(0);
        }
    }

    /// <summary>Returns a value that indicates whether serialization processes should serialize the value for the provided dependency property.</summary>
    /// <param name="dp">The identifier for the dependency property that should be serialized.</param>
    /// <returns>
    /// true if the dependency property that is supplied should be value-serialized; otherwise, false.
    /// </returns>
    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        return dp == StyleProperty ? false : base.ShouldSerializeProperty(dp);
    }

    /// <summary>Provides the CommonButtonBase_IsEnabledChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void CommonButtonBase_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ = sender;
        _ = e;
        var border = UserHintBorder();

        border?.Background = IsEnabled ? null : new SolidColorBrush(Color.FromArgb(0x55, 0, 0, 0));
    }

    /// <summary>Provides the CommonRepeatButtonBase_Loaded member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void CommonRepeatButtonBase_Loaded(object sender, RoutedEventArgs e) => SetIndicatorBorderSize(0);

    /// <summary>Provides the SetIndicatorBorderSize member.</summary>
    /// <param name="size">The size value.</param>
    private void SetIndicatorBorderSize(double size)
    {
        var border = UserHintBorder();

        if (border is null)
        {
            return;
        }

        if (size == 0)
        {
            border.BorderThickness = new(0);
            return;
        }

        if (size != 1)
        {
            border.BorderThickness = FocusBorderThickness;
            return;
        }

        border.BorderThickness = GetFocusedBorderThickness();
        if (GetType() != typeof(BezelRepeatButton))
        {
            return;
        }

        border.Margin = border.BorderThickness.Left == 1 ? new(1) : new(0);
    }

    /// <summary>Gets the focused border thickness.</summary>
    /// <returns>The focused border thickness.</returns>
    private Thickness GetFocusedBorderThickness() =>
        new(
            FocusBorderThickness.Left <= FocusBorderInset ? 1 : (FocusBorderThickness.Left - FocusBorderInset),
            FocusBorderThickness.Top <= FocusBorderInset ? 1 : (FocusBorderThickness.Top - FocusBorderInset),
            FocusBorderThickness.Right <= FocusBorderInset ? 1 : (FocusBorderThickness.Right - FocusBorderInset),
            FocusBorderThickness.Bottom <= FocusBorderInset ? 1 : (FocusBorderThickness.Bottom - FocusBorderInset));

    /// <summary>Provides the UserHintBorder member.</summary>
    /// <returns>The result.</returns>
    private System.Windows.Controls.Border? UserHintBorder()
    {
        return Template is null ? null : Template.FindName("PART_UserHintBorder", this) as System.Windows.Controls.Border;
    }
}
