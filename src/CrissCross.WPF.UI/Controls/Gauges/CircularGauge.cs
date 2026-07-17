// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>
/// CircularGauge displays a value within a specified range using a radial scale and a needle.
/// It supports theming via dynamic resources present in CrissCross (accent, stroke and text brushes).
/// </summary>
[TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
[TemplatePart(Name = Pointer, Type = typeof(Path))]
[TemplatePart(Name = "PointerCap", Type = typeof(Ellipse))]
[TemplatePart(Name = "RangeIndicatorLight", Type = typeof(Ellipse))]
public sealed partial class CircularGauge : Control
{
    /// <summary>Dependency property to Get/Set the Above Optimal Range Color.</summary>
    public static readonly DependencyProperty AboveOptimalRangeColorProperty = DependencyProperty.Register(
        nameof(AboveOptimalRangeColor),
        typeof(Brush),
        typeof(CircularGauge),
        new PropertyMetadata(Brushes.Transparent));

    /// <summary>Dependency property to Get/Set the Below Optimal Range Color.</summary>
    public static readonly DependencyProperty BelowOptimalRangeColorProperty = DependencyProperty.Register(
        nameof(BelowOptimalRangeColor),
        typeof(Brush),
        typeof(CircularGauge),
        new PropertyMetadata(Brushes.Transparent));

    /// <summary>The decimals property.</summary>
    public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
        nameof(Decimals),
        typeof(int),
        typeof(CircularGauge),
        new PropertyMetadata(2));

    /// <summary>The detection time out property.</summary>
    public static readonly DependencyProperty DetectionTimeOutProperty = DependencyProperty.Register(
        nameof(DetectionTimeOut),
        typeof(int),
        typeof(CircularGauge),
        new PropertyMetadata(1));

    /// <summary>The detect value or error property.</summary>
    public static readonly DependencyProperty DetectValueOrErrorProperty = DependencyProperty.Register(
        nameof(DetectValueOrError),
        typeof(bool),
        typeof(CircularGauge),
        new PropertyMetadata(false, OnDetectValueOrErrorPropertyChanged));

    /// <summary>Dependency property to Get/Set the Dial Text Font Size.</summary>
    public static readonly DependencyProperty DialTextFontSizeProperty = DependencyProperty.Register(
        nameof(DialTextFontSize),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(14D));

    /// <summary>The value text font size property.</summary>
    public static readonly DependencyProperty ValueTextFontSizeProperty = DependencyProperty.Register(
        nameof(ValueTextFontSize),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(14D));

    /// <summary>Dependency property to Get/Set the Dial Text Offset.</summary>
    public static readonly DependencyProperty DialTextOffsetProperty = DependencyProperty.Register(
        nameof(DialTextOffset),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(-40D));

    /// <summary>Dependency property to Get/Set the Dial Text.</summary>
    public static readonly DependencyProperty DialTextProperty = DependencyProperty.Register(
        nameof(DialText),
        typeof(string),
        typeof(CircularGauge),
        new PropertyMetadata("CrissCross"));

    /// <summary>The display alignment property.</summary>
    public static readonly DependencyProperty DisplayAlignmentProperty = DependencyProperty.Register(
        nameof(DisplayAlignment),
        typeof(TextAlignment),
        typeof(CircularGauge),
        new PropertyMetadata(TextAlignment.Center));

    /// <summary>The display border colour property.</summary>
    public static readonly DependencyProperty DisplayBorderColourProperty = DependencyProperty.Register(
        nameof(DisplayBorderColour),
        typeof(Brush),
        typeof(CircularGauge),
        new PropertyMetadata(null));

    /// <summary>The display value property.</summary>
    public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(
        nameof(DisplayValue),
        typeof(Visibility),
        typeof(CircularGauge),
        new PropertyMetadata(Visibility.Visible));

    /// <summary>The error property.</summary>
    public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(
        nameof(Error),
        typeof(bool),
        typeof(CircularGauge),
        new PropertyMetadata(false));

    /// <summary>Dependency property to Get/Set the image offset.</summary>
    public static readonly DependencyProperty ImageOffsetProperty = DependencyProperty.Register(
        nameof(ImageOffset),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(-50D));

    /// <summary>Dependency property to Get/Set the image Size.</summary>
    public static readonly DependencyProperty ImageSizeProperty = DependencyProperty.Register(
        nameof(ImageSize),
        typeof(Size),
        typeof(CircularGauge),
        new PropertyMetadata(new Size(40, 50)));

    /// <summary>Dependency property to Get/Set the image source.</summary>
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(CircularGauge),
        null);

    /// <summary>Dependency property to Get/Set the number of major divisions on the scale.</summary>
    public static readonly DependencyProperty MajorDivisionsCountProperty = DependencyProperty.Register(
        nameof(MajorDivisionsCount),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(10D, AMajorValueHasChanged));

    /// <summary>Dependency property to Get/Set the Major Tick Size.</summary>
    public static readonly DependencyProperty MajorTickSizeProperty = DependencyProperty.Register(
        nameof(MajorTickSize),
        typeof(Size),
        typeof(CircularGauge),
        new PropertyMetadata(new Size(10, 3)));

    /// <summary>Dependency property to Get/Set the Maximum Value.</summary>
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
        nameof(MaxValue),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(1000D, AMajorValueHasChanged));

    /// <summary>Dependency property to Get/Set the number of minor divisions on the scale.</summary>
    public static readonly DependencyProperty MinorDivisionsCountProperty = DependencyProperty.Register(
        nameof(MinorDivisionsCount),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(5D, AMajorValueHasChanged));

    /// <summary>Dependency property to Get/Set the Minor Tick Size.</summary>
    public static readonly DependencyProperty MinorTickSizeProperty = DependencyProperty.Register(
        nameof(MinorTickSize),
        typeof(Size),
        typeof(CircularGauge),
        new PropertyMetadata(new Size(3, 1)));

    /// <summary>Dependency property to Get/Set the Minimum Value.</summary>
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
        nameof(MinValue),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(0D, AMajorValueHasChanged));

    /// <summary>Dependency property to Get/Set the Optimal Range Color.</summary>
    public static readonly DependencyProperty OptimalRangeColorProperty = DependencyProperty.Register(
        nameof(OptimalRangeColor),
        typeof(Brush),
        typeof(CircularGauge),
        new PropertyMetadata(Brushes.Transparent));

    /// <summary>Dependency property to Get/Set Optimal Range End Value.</summary>
    public static readonly DependencyProperty OptimalRangeEndValueProperty = DependencyProperty.Register(
        nameof(OptimalRangeEndValue),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(0.9D, OnOptimalRangeEndValuePropertyChanged));

    /// <summary>Dependency property to Get/Set Optimal Range Start Value.</summary>
    public static readonly DependencyProperty OptimalRangeStartValueProperty = DependencyProperty.Register(
        nameof(OptimalRangeStartValue),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(0.1D, OnOptimalRangeStartValuePropertyChanged));

    /// <summary>Dependency property to Get/Set the Pointer cap Radius.</summary>
    public static readonly DependencyProperty PointerCapRadiusProperty = DependencyProperty.Register(
        nameof(PointerCapRadius),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(35D));

    /// <summary>Dependency property to Get/Set the pointer length.</summary>
    public static readonly DependencyProperty PointerLengthProperty = DependencyProperty.Register(
        nameof(PointerLength),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(85D));

    /// <summary>Dependency property to Get/Set the Pointer Thickness.</summary>
    public static readonly DependencyProperty PointerThicknessProperty = DependencyProperty.Register(
        nameof(PointerThickness),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(16D));

    /// <summary>Dependency property to Get/Set the Radius of the gauge.</summary>
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
        nameof(Radius),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(150D));

    /// <summary>Dependency property to Get/Set the range indicator light offset.</summary>
    public static readonly DependencyProperty RangeIndicatorLightOffsetProperty = DependencyProperty.Register(
        nameof(RangeIndicatorLightOffset),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(80D));

    /// <summary>Dependency property to Get/Set the Range Indicator light Radius.</summary>
    public static readonly DependencyProperty RangeIndicatorLightRadiusProperty = DependencyProperty.Register(
        nameof(RangeIndicatorLightRadius),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(10D));

    /// <summary>The range indicator light visible property.</summary>
    public static readonly DependencyProperty RangeIndicatorLightVisibleProperty = DependencyProperty.Register(
        nameof(RangeIndicatorLightVisible),
        typeof(Visibility),
        typeof(CircularGauge),
        new PropertyMetadata(Visibility.Visible));

    /// <summary>Dependency property to Get/Set the Range Indicator Radius.</summary>
    public static readonly DependencyProperty RangeIndicatorRadiusProperty = DependencyProperty.Register(
        nameof(RangeIndicatorRadius),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(120D));

    /// <summary>Dependency property to Get/Set the Range Indicator Thickness.</summary>
    public static readonly DependencyProperty RangeIndicatorThicknessProperty = DependencyProperty.Register(
        nameof(RangeIndicatorThickness),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(8D));

    /// <summary>Provides the Register member.</summary>
    public static readonly DependencyProperty ResetPointerOnStartUpProperty = DependencyProperty.Register(
        nameof(ResetPointerOnStartUp),
        typeof(bool),
        typeof(CircularGauge),
        new PropertyMetadata(true));

    /// <summary>Dependency property to Get/Set the Scale Label Foreground.</summary>
    public static readonly DependencyProperty ScaleForegroundProperty = DependencyProperty.Register(
        nameof(ScaleForeground),
        typeof(Brush),
        typeof(CircularGauge),
        new PropertyMetadata(ForegroundProperty.DefaultMetadata.DefaultValue, ScalePropertyChanged));

    /// <summary>Dependency property to Get/Set the Scale Label FontSize.</summary>
    public static readonly DependencyProperty ScaleLabelFontSizeProperty = DependencyProperty.Register(
        nameof(ScaleLabelFontSize),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(10D));

    /// <summary>Dependency property to Get/Set the scale label Radius.</summary>
    public static readonly DependencyProperty ScaleLabelRadiusProperty = DependencyProperty.Register(
        nameof(ScaleLabelRadius),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(90D));

    /// <summary>Dependency property to Get/Set the Scale Label Size.</summary>
    public static readonly DependencyProperty ScaleLabelSizeProperty = DependencyProperty.Register(
        nameof(ScaleLabelSize),
        typeof(Size),
        typeof(CircularGauge),
        new PropertyMetadata(new Size(40, 20)));

    /// <summary>Dependency property to Get/Set the scale Radius.</summary>
    public static readonly DependencyProperty ScaleRadiusProperty = DependencyProperty.Register(
        nameof(ScaleRadius),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(110D));

    /// <summary>Dependency property to Get/Set the starting angle of scale.</summary>
    public static readonly DependencyProperty ScaleStartAngleProperty = DependencyProperty.Register(
        nameof(ScaleStartAngle),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(120D));

    /// <summary>Dependency property to Get/Set the sweep angle of scale.</summary>
    public static readonly DependencyProperty ScaleSweepAngleProperty = DependencyProperty.Register(
        nameof(ScaleSweepAngle),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(300D));

    /// <summary>Dependency property to Get/Set the Scale Value Precision.</summary>
    public static readonly DependencyProperty ScaleValuePrecisionProperty = DependencyProperty.Register(
        nameof(ScaleValuePrecision),
        typeof(int),
        typeof(CircularGauge),
        new PropertyMetadata(5));

    /// <summary>The show error property.</summary>
    public static readonly DependencyProperty ShowErrorProperty = DependencyProperty.Register(
        nameof(ShowError),
        typeof(Visibility),
        typeof(CircularGauge),
        new PropertyMetadata(Visibility.Collapsed));

    /// <summary>The unit property.</summary>
    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
        nameof(Unit),
        typeof(string),
        typeof(CircularGauge),
        new PropertyMetadata(string.Empty));

    /// <summary>The value background property.</summary>
    public static readonly DependencyProperty ValueBackgroundProperty = DependencyProperty.Register(
        nameof(ValueBackground),
        typeof(Brush),
        typeof(CircularGauge),
        new PropertyMetadata(null));

    /// <summary>Dependency property to Get/Set the value.</summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(CircularGauge),
        new PropertyMetadata(0D, OnValuePropertyChanged));
}
