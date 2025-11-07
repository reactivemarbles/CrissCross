// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// CircularGauge displays a value within a specified range using a radial scale and a needle.
/// It supports theming via dynamic resources present in CrissCross (accent, stroke and text brushes).
/// </summary>
[TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
[TemplatePart(Name = "Pointer", Type = typeof(Path))]
[TemplatePart(Name = "PointerCap", Type = typeof(Ellipse))]
[TemplatePart(Name = "RangeIndicatorLight", Type = typeof(Ellipse))]
public sealed class CircularGauge : Control
{
    /// <summary>
    /// Dependency property to Get/Set the Above Optimal Range Color.
    /// </summary>
    public static readonly DependencyProperty AboveOptimalRangeColorProperty =
        DependencyProperty.Register(nameof(AboveOptimalRangeColor), typeof(Brush), typeof(CircularGauge), new PropertyMetadata(Brushes.Transparent));

    /// <summary>
    /// Dependency property to Get/Set the Below Optimal Range Color.
    /// </summary>
    public static readonly DependencyProperty BelowOptimalRangeColorProperty =
        DependencyProperty.Register(nameof(BelowOptimalRangeColor), typeof(Brush), typeof(CircularGauge), new PropertyMetadata(Brushes.Transparent));

    /// <summary>
    /// The decimals property.
    /// </summary>
    public static readonly DependencyProperty DecimalsProperty =
       DependencyProperty.Register(nameof(Decimals), typeof(int), typeof(CircularGauge), new PropertyMetadata(2));

    /// <summary>
    /// The detection time out property.
    /// </summary>
    public static readonly DependencyProperty DetectionTimeOutProperty =
        DependencyProperty.Register(nameof(DetectionTimeOut), typeof(int), typeof(CircularGauge), new PropertyMetadata(1));

    /// <summary>
    /// The detect value or error property.
    /// </summary>
    public static readonly DependencyProperty DetectValueOrErrorProperty =
        DependencyProperty.Register(nameof(DetectValueOrError), typeof(bool), typeof(CircularGauge), new PropertyMetadata(false, OnDetectValueOrErrorPropertyChanged));

    /// <summary>
    /// Dependency property to Get/Set the Dial Text Font Size.
    /// </summary>
    public static readonly DependencyProperty DialTextFontSizeProperty =
        DependencyProperty.Register(nameof(DialTextFontSize), typeof(double), typeof(CircularGauge), new PropertyMetadata(14d));

    /// <summary>
    /// The value text font size property.
    /// </summary>
    public static readonly DependencyProperty ValueTextFontSizeProperty =
        DependencyProperty.Register(nameof(ValueTextFontSize), typeof(double), typeof(CircularGauge), new PropertyMetadata(14d));

    /// <summary>
    /// Dependency property to Get/Set the Dial Text Offset.
    /// </summary>
    public static readonly DependencyProperty DialTextOffsetProperty =
        DependencyProperty.Register(nameof(DialTextOffset), typeof(double), typeof(CircularGauge), new PropertyMetadata(-40d));

    /// <summary>
    /// Dependency property to Get/Set the Dial Text.
    /// </summary>
    public static readonly DependencyProperty DialTextProperty =
        DependencyProperty.Register(nameof(DialText), typeof(string), typeof(CircularGauge), new PropertyMetadata("CrissCross"));

    /// <summary>
    /// The display alignment property.
    /// </summary>
    public static readonly DependencyProperty DisplayAlignmentProperty =
        DependencyProperty.Register(nameof(DisplayAlignment), typeof(TextAlignment), typeof(CircularGauge), new PropertyMetadata(TextAlignment.Center));

    /// <summary>
    /// The display border colour property.
    /// </summary>
    public static readonly DependencyProperty DisplayBorderColourProperty =
      DependencyProperty.Register(nameof(DisplayBorderColour), typeof(Brush), typeof(CircularGauge), new PropertyMetadata(null));

    /// <summary>
    /// The display value property.
    /// </summary>
    public static readonly DependencyProperty DisplayValueProperty =
        DependencyProperty.Register(nameof(DisplayValue), typeof(Visibility), typeof(CircularGauge), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The error property.
    /// </summary>
    public static readonly DependencyProperty ErrorProperty =
        DependencyProperty.Register(nameof(Error), typeof(bool), typeof(CircularGauge), new PropertyMetadata(false));

    /// <summary>
    /// Dependency property to Get/Set the image offset.
    /// </summary>
    public static readonly DependencyProperty ImageOffsetProperty =
        DependencyProperty.Register(nameof(ImageOffset), typeof(double), typeof(CircularGauge), new PropertyMetadata(-50d));

    /// <summary>
    /// Dependency property to Get/Set the image Size.
    /// </summary>
    public static readonly DependencyProperty ImageSizeProperty =
        DependencyProperty.Register(nameof(ImageSize), typeof(Size), typeof(CircularGauge), new PropertyMetadata(new Size(40, 50)));

    /// <summary>
    /// Dependency property to Get/Set the image source.
    /// </summary>
    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CircularGauge), null);

    /// <summary>
    /// Dependency property to Get/Set the number of major divisions on the scale.
    /// </summary>
    public static readonly DependencyProperty MajorDivisionsCountProperty =
        DependencyProperty.Register(nameof(MajorDivisionsCount), typeof(double), typeof(CircularGauge), new PropertyMetadata(10d, AMajorValueHasChanged));

    /// <summary>
    /// Dependency property to Get/Set the Major Tick Size.
    /// </summary>
    public static readonly DependencyProperty MajorTickSizeProperty =
        DependencyProperty.Register(nameof(MajorTickSize), typeof(Size), typeof(CircularGauge), new PropertyMetadata(new Size(10, 3)));

    /// <summary>
    /// Dependency property to Get/Set the Maximum Value.
    /// </summary>
    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(CircularGauge), new PropertyMetadata(1000d, AMajorValueHasChanged));

    /// <summary>
    /// Dependency property to Get/Set the number of minor divisions on the scale.
    /// </summary>
    public static readonly DependencyProperty MinorDivisionsCountProperty =
        DependencyProperty.Register(nameof(MinorDivisionsCount), typeof(double), typeof(CircularGauge), new PropertyMetadata(5d, AMajorValueHasChanged));

    /// <summary>
    /// Dependency property to Get/Set the Minor Tick Size.
    /// </summary>
    public static readonly DependencyProperty MinorTickSizeProperty =
        DependencyProperty.Register(nameof(MinorTickSize), typeof(Size), typeof(CircularGauge), new PropertyMetadata(new Size(3, 1)));

    /// <summary>
    /// Dependency property to Get/Set the Minimum Value.
    /// </summary>
    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(CircularGauge), new PropertyMetadata(0d, AMajorValueHasChanged));

    /// <summary>
    /// Dependency property to Get/Set the Optimal Range Color.
    /// </summary>
    public static readonly DependencyProperty OptimalRangeColorProperty =
        DependencyProperty.Register(nameof(OptimalRangeColor), typeof(Brush), typeof(CircularGauge), new PropertyMetadata(Brushes.Transparent));

    /// <summary>
    /// Dependency property to Get/Set Optimal Range End Value.
    /// </summary>
    public static readonly DependencyProperty OptimalRangeEndValueProperty =
        DependencyProperty.Register(nameof(OptimalRangeEndValue), typeof(double), typeof(CircularGauge), new PropertyMetadata(0.9d, OnOptimalRangeEndValuePropertyChanged));

    /// <summary>
    /// Dependency property to Get/Set Optimal Range Start Value.
    /// </summary>
    public static readonly DependencyProperty OptimalRangeStartValueProperty =
        DependencyProperty.Register(nameof(OptimalRangeStartValue), typeof(double), typeof(CircularGauge), new PropertyMetadata(0.1d, OnOptimalRangeStartValuePropertyChanged));

    /// <summary>
    /// Dependency property to Get/Set the Pointer cap Radius.
    /// </summary>
    public static readonly DependencyProperty PointerCapRadiusProperty =
        DependencyProperty.Register(nameof(PointerCapRadius), typeof(double), typeof(CircularGauge), new PropertyMetadata(35d));

    /// <summary>
    /// Dependency property to Get/Set the pointer length.
    /// </summary>
    public static readonly DependencyProperty PointerLengthProperty =
        DependencyProperty.Register(nameof(PointerLength), typeof(double), typeof(CircularGauge), new PropertyMetadata(85d));

    /// <summary>
    /// Dependency property to Get/Set the Pointer Thickness.
    /// </summary>
    public static readonly DependencyProperty PointerThicknessProperty =
        DependencyProperty.Register(nameof(PointerThickness), typeof(double), typeof(CircularGauge), new PropertyMetadata(16d));

    /// <summary>
    /// Dependency property to Get/Set the Radius of the gauge.
    /// </summary>
    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(nameof(Radius), typeof(double), typeof(CircularGauge), new PropertyMetadata(150d));

    /// <summary>
    /// Dependency property to Get/Set the range indicator light offset.
    /// </summary>
    public static readonly DependencyProperty RangeIndicatorLightOffsetProperty =
        DependencyProperty.Register(nameof(RangeIndicatorLightOffset), typeof(double), typeof(CircularGauge), new PropertyMetadata(80d));

    /// <summary>
    /// Dependency property to Get/Set the Range Indicator light Radius.
    /// </summary>
    public static readonly DependencyProperty RangeIndicatorLightRadiusProperty =
        DependencyProperty.Register(nameof(RangeIndicatorLightRadius), typeof(double), typeof(CircularGauge), new PropertyMetadata(10d));

    /// <summary>
    /// The range indicator light visible property.
    /// </summary>
    public static readonly DependencyProperty RangeIndicatorLightVisibleProperty =
        DependencyProperty.Register(nameof(RangeIndicatorLightVisible), typeof(Visibility), typeof(CircularGauge), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Dependency property to Get/Set the Range Indicator Radius.
    /// </summary>
    public static readonly DependencyProperty RangeIndicatorRadiusProperty =
        DependencyProperty.Register(nameof(RangeIndicatorRadius), typeof(double), typeof(CircularGauge), new PropertyMetadata(120d));

    /// <summary>
    /// Dependency property to Get/Set the Range Indicator Thickness.
    /// </summary>
    public static readonly DependencyProperty RangeIndicatorThicknessProperty =
        DependencyProperty.Register(nameof(RangeIndicatorThickness), typeof(double), typeof(CircularGauge), new PropertyMetadata(8d));

    /// <summary>
    /// Dependency property to Get/Set the an option to reset the pointer on start up to the
    /// minimum value.
    /// </summary>
    public static readonly DependencyProperty ResetPointerOnStartUpProperty =
        DependencyProperty.Register(nameof(ResetPointerOnStartUp), typeof(bool), typeof(CircularGauge), new PropertyMetadata(true));

    /// <summary>
    /// Dependency property to Get/Set the Scale Label Foreground.
    /// </summary>
    public static readonly DependencyProperty ScaleForegroundProperty =
        DependencyProperty.Register(nameof(ScaleForeground), typeof(Brush), typeof(CircularGauge), new PropertyMetadata(ForegroundProperty.DefaultMetadata.DefaultValue, ScalePropertyChanged));

    /// <summary>
    /// Dependency property to Get/Set the Scale Label FontSize.
    /// </summary>
    public static readonly DependencyProperty ScaleLabelFontSizeProperty =
        DependencyProperty.Register(nameof(ScaleLabelFontSize), typeof(double), typeof(CircularGauge), new PropertyMetadata(10d));

    /// <summary>
    /// Dependency property to Get/Set the scale label Radius.
    /// </summary>
    public static readonly DependencyProperty ScaleLabelRadiusProperty =
        DependencyProperty.Register(nameof(ScaleLabelRadius), typeof(double), typeof(CircularGauge), new PropertyMetadata(90d));

    /// <summary>
    /// Dependency property to Get/Set the Scale Label Size.
    /// </summary>
    public static readonly DependencyProperty ScaleLabelSizeProperty =
        DependencyProperty.Register(nameof(ScaleLabelSize), typeof(Size), typeof(CircularGauge), new PropertyMetadata(new Size(40, 20)));

    /// <summary>
    /// Dependency property to Get/Set the scale Radius.
    /// </summary>
    public static readonly DependencyProperty ScaleRadiusProperty =
        DependencyProperty.Register(nameof(ScaleRadius), typeof(double), typeof(CircularGauge), new PropertyMetadata(110d));

    /// <summary>
    /// Dependency property to Get/Set the starting angle of scale.
    /// </summary>
    public static readonly DependencyProperty ScaleStartAngleProperty =
        DependencyProperty.Register(nameof(ScaleStartAngle), typeof(double), typeof(CircularGauge), new PropertyMetadata(120d));

    /// <summary>
    /// Dependency property to Get/Set the sweep angle of scale.
    /// </summary>
    public static readonly DependencyProperty ScaleSweepAngleProperty =
        DependencyProperty.Register(nameof(ScaleSweepAngle), typeof(double), typeof(CircularGauge), new PropertyMetadata(300d));

    /// <summary>
    /// Dependency property to Get/Set the Scale Value Precision.
    /// </summary>
    public static readonly DependencyProperty ScaleValuePrecisionProperty =
        DependencyProperty.Register(nameof(ScaleValuePrecision), typeof(int), typeof(CircularGauge), new PropertyMetadata(5));

    /// <summary>
    /// The show error property.
    /// </summary>
    public static readonly DependencyProperty ShowErrorProperty =
        DependencyProperty.Register(nameof(ShowError), typeof(Visibility), typeof(CircularGauge), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// The unit property.
    /// </summary>
    public static readonly DependencyProperty UnitProperty =
         DependencyProperty.Register(nameof(Unit), typeof(string), typeof(CircularGauge), new PropertyMetadata(string.Empty));

    /// <summary>
    /// The value background property.
    /// </summary>
    public static readonly DependencyProperty ValueBackgroundProperty =
        DependencyProperty.Register(nameof(ValueBackground), typeof(Brush), typeof(CircularGauge), new PropertyMetadata(null));

    /// <summary>
    /// Dependency property to Get/Set the value.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(CircularGauge), new PropertyMetadata(0d, OnValuePropertyChanged));

    private const int AnimatingSpeedFactor = 6;
    private readonly Hashtable _ht = [];
    private readonly object _lockObject = new();
    private double _arcradius1;
    private double _arcradius2;
    private bool _detectValuesChanging;
    private Ellipse? _lightIndicator;
    private int _numberOfMinorpoints;
    private int _numberOfpoints;
    private double _oldcurrRealworldunit;
    private Path? _pointer;
    private Ellipse? _pointerCap;
    private Path? _rangeIndicator1;
    private Path? _rangeIndicator2;
    private Path? _rangeIndicator3;
    private Grid? _rootGrid;
    private bool _valueChanged;

    /// <summary>
    /// Initializes static members of the <see cref="CircularGauge"/> class.
    /// </summary>
    static CircularGauge() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularGauge), new FrameworkPropertyMetadata(typeof(CircularGauge)));

    /// <summary>
    /// Gets or sets /Sets Above Optimal Range Color.
    /// </summary>
    [Description("Gets/Sets Above Optimal Range Color")]
    public Brush AboveOptimalRangeColor
    {
        get => (Brush)GetValue(AboveOptimalRangeColorProperty); set => SetValue(AboveOptimalRangeColorProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets Below Optimal Range Color.
    /// </summary>
    [Description("Gets/Sets Below Optimal Range Color")]
    public Brush BelowOptimalRangeColor
    {
        get => (Brush)GetValue(BelowOptimalRangeColorProperty); set => SetValue(BelowOptimalRangeColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the decimals.
    /// </summary>
    /// <value>The decimals.</value>
    [Description("Gets or sets the decimals.")]
    [Category("CrissCross Important")]
    public int Decimals
    {
        get => (int)GetValue(DecimalsProperty); set => SetValue(DecimalsProperty, value);
    }

    /// <summary>
    /// Gets or sets the detection time out.
    /// </summary>
    /// <value>The detection timeout.</value>
    [Description("Gets or sets the detection time out, so the value hasn't changed for a period and thus an error will show.")]
    [Category("CrissCross Detection")]
    public int DetectionTimeOut
    {
        get => (int)GetValue(DetectionTimeOutProperty); set => SetValue(DetectionTimeOutProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [detect value or error].
    /// </summary>
    /// <value><c>true</c> if [detect value or error]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether to start checking if the value is changing or it has an error.")]
    [Category("CrissCross Detection")]
    public bool DetectValueOrError
    {
        get => (bool)GetValue(DetectValueOrErrorProperty); set => SetValue(DetectValueOrErrorProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets Dial Text.
    /// </summary>
    [Description("Gets/Sets Dial Text")]
    [Category("CrissCross Dial Text")]
    public string DialText
    {
        get => (string)GetValue(DialTextProperty); set => SetValue(DialTextProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets Dial Text Font Size.
    /// </summary>
    [Description("Gets/Sets Dial Text Font Size")]
    [Category("CrissCross Dial Text")]
    public double DialTextFontSize
    {
        get => (double)GetValue(DialTextFontSizeProperty); set => SetValue(DialTextFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets Dial Text Font Size.
    /// </summary>
    [Description("Gets/Sets Value Text Font Size")]
    [Category("CrissCross Dial Text")]
    public double ValueTextFontSize
    {
        get => (double)GetValue(ValueTextFontSizeProperty); set => SetValue(ValueTextFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets Dial Text Offset.
    /// </summary>
    [Description("Gets/Sets Dial Text Offset")]
    [Category("CrissCross Dial Text")]
    public double DialTextOffset
    {
        get => (double)GetValue(DialTextOffsetProperty); set => SetValue(DialTextOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the display alignment.
    /// </summary>
    /// <value>The display alignment.</value>
    [Description("Gets or sets the display alignment.")]
    [Category("CrissCross Important")]
    public TextAlignment DisplayAlignment
    {
        get => (TextAlignment)GetValue(DisplayAlignmentProperty); set => SetValue(DisplayAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the display border colour.
    /// </summary>
    /// <value>The display border colour.</value>
    [Description("Gets or sets the display border colour.")]
    public Brush DisplayBorderColour
    {
        get => (Brush)GetValue(DisplayBorderColourProperty); set => SetValue(DisplayBorderColourProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [display value].
    /// </summary>
    /// <value><c>true</c> if [display value]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether [display value].")]
    [Category("CrissCross Important")]
    public Visibility DisplayValue
    {
        get => (Visibility)GetValue(DisplayValueProperty); set => SetValue(DisplayValueProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the error.
    /// </summary>
    /// <value>The error.</value>
    [Description("Gets or sets the error")]
    [Category("CrissCross")]
    public bool Error
    {
        get => (bool)GetValue(ErrorProperty); set => SetValue(ErrorProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Image offset.
    /// </summary>
    [Description("Gets/Sets the Image offset")]
    [Category("CrissCross Image")]
    public double ImageOffset
    {
        get => (double)GetValue(ImageOffsetProperty); set => SetValue(ImageOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Image width and height.
    /// </summary>
    [Description("Gets/Sets the Image width and height")]
    [Category("CrissCross Image")]
    public Size ImageSize
    {
        get => (Size)GetValue(ImageSizeProperty); set => SetValue(ImageSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Gauge image source.
    /// </summary>
    [Description("Gets/Sets the Gauge image source")]
    [Category("CrissCross Image")]
    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the number of major divisions on the scale.
    /// </summary>
    [Description("Gets/Sets the number of major divisions on the scale")]
    [Category("CrissCross")]
    public double MajorDivisionsCount
    {
        get => (double)GetValue(MajorDivisionsCountProperty); set => SetValue(MajorDivisionsCountProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Major Tick Size.
    /// </summary>
    [Description("Gets/Sets the Major Tick Size")]
    [Category("CrissCross")]
    public Size MajorTickSize
    {
        get => (Size)GetValue(MajorTickSizeProperty); set => SetValue(MajorTickSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Maximum Value.
    /// </summary>
    [Description("Gets/Sets the Maximum Value")]
    [Category("CrissCross Important")]
    public double MaxValue
    {
        get => (double)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the number of minor divisions on the scale.
    /// </summary>
    [Description("Gets/Sets the number of minor divisions on the scale")]
    [Category("CrissCross")]
    public double MinorDivisionsCount
    {
        get => (double)GetValue(MinorDivisionsCountProperty); set => SetValue(MinorDivisionsCountProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Minor Tick Size.
    /// </summary>
    [Description("Gets/Sets the Minor Tick Size")]
    [Category("CrissCross")]
    public Size MinorTickSize
    {
        get => (Size)GetValue(MinorTickSizeProperty); set => SetValue(MinorTickSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Minimum Value.
    /// </summary>
    [Description("Gets/Sets the Minimum Value")]
    [Category("CrissCross Important")]
    public double MinValue
    {
        get => (double)GetValue(MinValueProperty); set
        {
            SetValue(MinValueProperty, value);
            RefreshDialRange();
        }
    }

    /// <summary>
    /// Gets or sets /Sets Optimal Range Color.
    /// </summary>
    [Description("Gets/Sets Optimal Range Color")]
    public Brush OptimalRangeColor
    {
        get => (Brush)GetValue(OptimalRangeColorProperty); set => SetValue(OptimalRangeColorProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Optimal range end value.
    /// </summary>
    [Description("Gets/Sets the Optimal range end value")]
    [Category("CrissCross Important")]
    public double OptimalRangeEndValue
    {
        get => (double)GetValue(OptimalRangeEndValueProperty); set => SetValue(OptimalRangeEndValueProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Optimal Range Start Value.
    /// </summary>
    [Description("Gets/Sets the Optimal Range Start Value")]
    [Category("CrissCross Important")]
    public double OptimalRangeStartValue
    {
        get => (double)GetValue(OptimalRangeStartValueProperty); set => SetValue(OptimalRangeStartValueProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Pointer cap radius.
    /// </summary>
    [Description("Gets/Sets the Pointer cap radius")]
    [Category("CrissCross Pointer")]
    public double PointerCapRadius
    {
        get => (double)GetValue(PointerCapRadiusProperty); set => SetValue(PointerCapRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Pointer Length.
    /// </summary>
    [Description("Gets/Sets the Pointer Length")]
    [Category("CrissCross Pointer")]
    public double PointerLength
    {
        get => (double)GetValue(PointerLengthProperty); set => SetValue(PointerLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Pointer Thickness.
    /// </summary>
    [Description("Gets/Sets the Pointer Thickness")]
    [Category("CrissCross Pointer")]
    public double PointerThickness
    {
        get => (double)GetValue(PointerThicknessProperty); set => SetValue(PointerThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Minimum Value.
    /// </summary>
    [Description("Gets/Sets the Minimum Value")]
    [Category("CrissCross")]
    public double Radius
    {
        get => (double)GetValue(RadiusProperty); set => SetValue(RadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Range Indicator Light offset.
    /// </summary>
    [Description("Gets/Sets the Range Indicator Light offset")]
    [Category("CrissCross")]
    public double RangeIndicatorLightOffset
    {
        get => (double)GetValue(RangeIndicatorLightOffsetProperty); set => SetValue(RangeIndicatorLightOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets Range Indicator Light Radius.
    /// </summary>
    [Description("Gets/Sets Range Indicator Light Radius")]
    [Category("CrissCross")]
    public double RangeIndicatorLightRadius
    {
        get => (double)GetValue(RangeIndicatorLightRadiusProperty); set => SetValue(RangeIndicatorLightRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the range indicator light visible.
    /// </summary>
    /// <value>The range indicator light visible.</value>
    [Description("Gets/Sets the Range Indicator Light Visible value")]
    [Category("CrissCross")]
    public Visibility RangeIndicatorLightVisible
    {
        get => (Visibility)GetValue(RangeIndicatorLightVisibleProperty); set => SetValue(RangeIndicatorLightVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Range Indicator Radius.
    /// </summary>
    [Description("Gets/Sets the Range Indicator Radius")]
    [Category("CrissCross")]
    public double RangeIndicatorRadius
    {
        get => (double)GetValue(RangeIndicatorRadiusProperty); set => SetValue(RangeIndicatorRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Range Indicator Thickness.
    /// </summary>
    [Description("Gets/Sets the Range Indicator Thickness")]
    [Category("CrissCross")]
    public double RangeIndicatorThickness
    {
        get => (double)GetValue(RangeIndicatorThicknessProperty); set => SetValue(RangeIndicatorThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets/Sets option to reset the pointer to minimum on start up, Default is true.
    /// </summary>
    [Description("Gets/Sets option to reset the pointer to minimum on start up, Default is true")]
    [Category("CrissCross Pointer")]
    public bool ResetPointerOnStartUp
    {
        get => (bool)GetValue(ResetPointerOnStartUpProperty); set => SetValue(ResetPointerOnStartUpProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Scale Label Foreground.
    /// </summary>
    [Description("Gets/Sets the Scale Label Foreground")]
    public Brush ScaleForeground
    {
        get => (Brush)GetValue(ScaleForegroundProperty); set => SetValue(ScaleForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Scale Label Font Size.
    /// </summary>
    [Description("Gets/Sets the Scale Label Font Size")]
    [Category("CrissCross Scale")]
    public double ScaleLabelFontSize
    {
        get => (double)GetValue(ScaleLabelFontSizeProperty); set => SetValue(ScaleLabelFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Scale Label Radius.
    /// </summary>
    [Description("Gets/Sets the Scale Label Radius")]
    [Category("CrissCross Scale")]
    public double ScaleLabelRadius
    {
        get => (double)GetValue(ScaleLabelRadiusProperty); set => SetValue(ScaleLabelRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Scale Label Size.
    /// </summary>
    [Description("Gets/Sets the Scale Label Size")]
    [Category("CrissCross Scale")]
    public Size ScaleLabelSize
    {
        get => (Size)GetValue(ScaleLabelSizeProperty); set => SetValue(ScaleLabelSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the Scale radius.
    /// </summary>
    [Description("Gets/Sets the Scale radius")]
    [Category("CrissCross Scale")]
    public double ScaleRadius
    {
        get => (double)GetValue(ScaleRadiusProperty); set => SetValue(ScaleRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the scale start angle.
    /// </summary>
    [Description("Gets/Sets the scale start angle")]
    [Category("CrissCross Scale")]
    public double ScaleStartAngle
    {
        get => (double)GetValue(ScaleStartAngleProperty); set => SetValue(ScaleStartAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the scale sweep angle.
    /// </summary>
    [Description("Gets/Sets the scale sweep angle")]
    [Category("CrissCross Scale")]
    public double ScaleSweepAngle
    {
        get => (double)GetValue(ScaleSweepAngleProperty); set => SetValue(ScaleSweepAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets scale value precision.
    /// </summary>
    [Description("Gets/Sets scale value precision")]
    [Category("CrissCross Scale")]
    public int ScaleValuePrecision
    {
        get => (int)GetValue(ScaleValuePrecisionProperty); set => SetValue(ScaleValuePrecisionProperty, value);
    }

    /// <summary>
    /// Gets or sets the show error.
    /// </summary>
    /// <value>The show error.</value>
    [Description("Gets or sets the show error red cross")]
    [Category("CrissCross")]
    public Visibility ShowError
    {
        get => (Visibility)GetValue(ShowErrorProperty); set => SetValue(ShowErrorProperty, value);
    }

    /// <summary>
    /// Gets or sets the unit.
    /// </summary>
    /// <value>The unit.</value>
    [Description("Gets or sets the unit")]
    [Category("CrissCross Important")]
    public string Unit
    {
        get => (string)GetValue(UnitProperty); set => SetValue(UnitProperty, value);
    }

    /// <summary>
    /// Gets or sets /Sets the current value.
    /// </summary>
    [Description("Gets/Sets the current value")]
    [Category("CrissCross Important")]
    public double Value
    {
        get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the value background.
    /// </summary>
    /// <value>The value background.</value>
    public Brush ValueBackground
    {
        get => (Brush)GetValue(ValueBackgroundProperty); set => SetValue(ValueBackgroundProperty, value);
    }

    /// <summary>
    /// Load the visualization template.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Get reference to known elements on the control template
        _rootGrid = GetTemplateChild("LayoutRoot") as Grid;
        _pointer = GetTemplateChild("Pointer") as Path;
        _pointerCap = GetTemplateChild("PointerCap") as Ellipse;
        _lightIndicator = GetTemplateChild("RangeIndicatorLight") as Ellipse;

        // Draw scale and range indicator
        DrawScale();
        DrawRangeIndicator();

        // Set Z index of pointer and pointer cap to a really high number so that it stays on top
        // of the scale and the range indicator
        Panel.SetZIndex(_pointer, 100000);
        Panel.SetZIndex(_pointerCap, 100001);

        // Reset Pointer
        if (ResetPointerOnStartUp)
        {
            MovePointer(ScaleStartAngle);
        }
    }

    /// <summary>
    /// Raises the <see cref="E:ValueChanged"/> event.
    /// </summary>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    public void OnValueChanged(DependencyPropertyChangedEventArgs e)
    {
        // Validate and set the new value
        var newValue = (double)e.NewValue;

        if (newValue > MaxValue)
        {
            newValue = MaxValue;
        }
        else if (newValue < MinValue)
        {
            newValue = MinValue;
        }

        if (_pointer != null)
        {
            var range = MaxValue - MinValue;
            var proccessValue = (newValue - MinValue) / range;
            var newcurrRealworldunit = proccessValue * ScaleSweepAngle;

            if (newcurrRealworldunit.Equals(double.NaN))
            {
                newcurrRealworldunit = 0;
            }

            // Animate the pointer from the old value to the new value
            AnimatePointer(ScaleStartAngle + _oldcurrRealworldunit, ScaleStartAngle + newcurrRealworldunit);
            _oldcurrRealworldunit = newcurrRealworldunit;
        }
    }

    /// <summary>
    /// Refreshes the dial range.
    /// </summary>
    public void RefreshDialRange()
    {
        try
        {
            foreach (var item in _ht.Values)
            {
                if (item is TextBlock textBlock)
                {
                    _rootGrid?.Children.Remove(textBlock);
                }

                if (item is Rectangle element)
                {
                    _rootGrid?.Children.Remove(element);
                }
            }

            _ht.Clear();
            DrawScale();
        }
        catch
        {
        }
    }

    /// <summary>
    /// Refreshes the range indicator.
    /// </summary>
    public void RefreshRangeIndicator()
    {
        try
        {
            if (_rangeIndicator1 != null && _rootGrid != null)
            {
                _rootGrid.Children.Remove(_rangeIndicator1);
            }

            if (_rangeIndicator2 != null && _rootGrid != null)
            {
                _rootGrid.Children.Remove(_rangeIndicator2);
            }

            if (_rangeIndicator3 != null && _rootGrid != null)
            {
                _rootGrid.Children.Remove(_rangeIndicator3);
            }

            DrawRangeIndicator();
        }
        catch
        {
        }
    }

    /// <summary>
    /// as the major value has changed.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private static void AMajorValueHasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CircularGauge gauge)
        {
            gauge.RefreshDialRange();
            gauge.RefreshRangeIndicator();
            lock (gauge._lockObject)
            {
                var g = gauge.Value;
                gauge.AnimatePointer(gauge.ScaleStartAngle + gauge._oldcurrRealworldunit, gauge.ScaleStartAngle);
                gauge._oldcurrRealworldunit = gauge.ScaleStartAngle;

                gauge.Value = gauge.MinValue;
                gauge.Value = g;
            }
        }
    }

    private static GradientBrush GetRangeIndicatorGradEffect(Brush gradientColor)
    {
        var gradient = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1)
        };

        var color1 = new GradientStop { Offset = 0.2, Color = gradientColor == Brushes.Transparent ? Colors.Transparent : Colors.LightGray };
        gradient.GradientStops.Add(color1);
        gradient.GradientStops.Add(new GradientStop { Color = ((SolidColorBrush)gradientColor).Color, Offset = 0.5 });
        gradient.GradientStops.Add(new GradientStop { Color = ((SolidColorBrush)gradientColor).Color, Offset = 0.8 });
        return gradient;
    }

    private static async void OnDetectValueOrErrorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var gauge = d as CircularGauge;
        if ((bool)e.NewValue)
        {
            // Start checking
            await gauge!.DetectectingThread();
        }
        else
        {
            // Stop checking
            gauge?._detectValuesChanging = false;
        }
    }

    private static void OnOptimalRangeEndValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Get access to the instance of CircularGaugeConrol whose property value changed
        if (d is CircularGauge gauge)
        {
            if ((double)e.NewValue > gauge.MaxValue)
            {
                gauge.OptimalRangeEndValue = gauge.MaxValue;
            }

            if ((double)e.NewValue < gauge.OptimalRangeStartValue)
            {
                gauge.OptimalRangeEndValue = Math.Min(gauge.OptimalRangeStartValue, gauge.MaxValue);
            }

            gauge.RefreshRangeIndicator();
        }
    }

    private static void OnOptimalRangeStartValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Get access to the instance of CircularGaugeConrol whose property value changed
        if (d is CircularGauge gauge)
        {
            if ((double)e.NewValue < gauge.MinValue)
            {
                gauge.OptimalRangeStartValue = gauge.MinValue;
            }
            else if ((double)e.NewValue > gauge.OptimalRangeEndValue)
            {
                gauge.OptimalRangeStartValue = gauge.OptimalRangeEndValue;
            }
            else
            {
                gauge.OptimalRangeStartValue = (double)e.NewValue;
            }

            gauge.RefreshRangeIndicator();
        }
    }

    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Get access to the instance of CircularGauge whose property value changed
        if (d is CircularGauge gauge)
        {
            lock (gauge._lockObject)
            {
                gauge._valueChanged = true;
                gauge.OnValueChanged(e);
            }
        }
    }

    private static void ScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CircularGauge gauge)
        {
            gauge.RefreshDialRange();
        }
    }

    private void AnimatePointer(double oldValueAngle, double newValueAngle)
    {
        if (_pointer != null && newValueAngle != oldValueAngle)
        {
            var da = new DoubleAnimation
            {
                From = oldValueAngle,
                To = newValueAngle,
                Duration = new Duration(TimeSpan.FromMilliseconds(Math.Abs(oldValueAngle - newValueAngle) * AnimatingSpeedFactor))
            };

            var sb = new Storyboard();
            sb.Completed += Sb_Completed;
            sb.Children.Add(da);
            Storyboard.SetTarget(da, _pointer);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
            sb.Begin();
        }
    }

    private async Task DetectectingThread()
    {
        _detectValuesChanging = true;
        var count = 0;
        while (_detectValuesChanging)
        {
            if (_valueChanged)
            {
                count = 0;
                _valueChanged = false;
                ShowError = Visibility.Collapsed;
            }
            else
            {
                count++;
                if (count >= DetectionTimeOut * 2)
                {
                    count = 0;
                    ShowError = Visibility.Visible;
                }
            }

            await Task.Delay(500).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Draw the range indicator.
    /// </summary>
    private void DrawRangeIndicator()
    {
        var realworldunit = ScaleSweepAngle / (MaxValue - MinValue);
        var optimalStartAngle = double.NaN;
        var optimalEndAngle = double.NaN;
        var db = 0d;

        // Checking whether the OptimalRangeStartvalue is -
        if (OptimalRangeStartValue < 0)
        {
            db = MinValue + Math.Abs(OptimalRangeStartValue);
            optimalStartAngle = Math.Abs(db * realworldunit);
        }
        else
        {
            db = Math.Abs(MinValue) + OptimalRangeStartValue;
            optimalStartAngle = db * realworldunit;
        }

        // Checking whether the OptimalRangeEndvalue is -
        if (OptimalRangeEndValue < 0)
        {
            db = MinValue + Math.Abs(OptimalRangeEndValue);
            optimalEndAngle = Math.Abs(db * realworldunit);
        }
        else
        {
            db = Math.Abs(MinValue) + OptimalRangeEndValue;
            optimalEndAngle = db * realworldunit;
        }

        // Calculating the angle for optimal Start value
        var optimalStartAngleFromStart = ScaleStartAngle + optimalStartAngle;

        // Calculating the angle for optimal Start value
        var optimalEndAngleFromStart = ScaleStartAngle + optimalEndAngle;

        // Calculating the Radius of the two arc for segment
        _arcradius1 = RangeIndicatorRadius + RangeIndicatorThickness;
        _arcradius2 = RangeIndicatorRadius;
        var endAngle = ScaleStartAngle + ScaleSweepAngle;

        // Calculating the Points for the below Optimal Range segment from the center of the gauge
        var isReflexAngle = Math.Abs(optimalStartAngleFromStart - ScaleStartAngle) > 180.0;
        _rangeIndicator1 = DrawSegment(
                                                GetCircumferencePoint(ScaleStartAngle, _arcradius1),
                                                GetCircumferencePoint(ScaleStartAngle, _arcradius2),
                                                GetCircumferencePoint(optimalStartAngleFromStart, _arcradius2),
                                                GetCircumferencePoint(optimalStartAngleFromStart, _arcradius1),
                                                isReflexAngle,
                                                BelowOptimalRangeColor);

        // Calculating the Points for the Optimal Range segment from the center of the gauge
        var isReflexAngle1 = Math.Abs(optimalEndAngleFromStart - optimalStartAngleFromStart) > 180.0;
        _rangeIndicator2 = DrawSegment(
                                                GetCircumferencePoint(optimalStartAngleFromStart, _arcradius1),
                                                GetCircumferencePoint(optimalStartAngleFromStart, _arcradius2),
                                                GetCircumferencePoint(optimalEndAngleFromStart, _arcradius2),
                                                GetCircumferencePoint(optimalEndAngleFromStart, _arcradius1),
                                                isReflexAngle1,
                                                OptimalRangeColor);

        // Calculating the Points for the Above Optimal Range segment from the center of the gauge
        var isReflexAngle2 = Math.Abs(endAngle - optimalEndAngleFromStart) > 180.0;
        _rangeIndicator3 = DrawSegment(
                                                GetCircumferencePoint(optimalEndAngleFromStart, _arcradius1),
                                                GetCircumferencePoint(optimalEndAngleFromStart, _arcradius2),
                                                GetCircumferencePoint(endAngle, _arcradius2),
                                                GetCircumferencePoint(endAngle, _arcradius1),
                                                isReflexAngle2,
                                                AboveOptimalRangeColor);
    }

    /// <summary>
    /// Drawing the scale with the Scale Radius.
    /// </summary>
    private void DrawScale()
    {
        if (_rootGrid == null)
        {
            return;
        }

        // Calculate one major tick angle
        var majorTickUnitAngle = ScaleSweepAngle / MajorDivisionsCount;

        // Obtaining One major ticks value
        var majorTicksUnitValue = (MaxValue - MinValue) / MajorDivisionsCount;
        majorTicksUnitValue = Math.Round(majorTicksUnitValue, ScaleValuePrecision);
        var minvalue = MinValue;

        // Drawing Major scale ticks
        for (var i = ScaleStartAngle; i <= (ScaleStartAngle + ScaleSweepAngle); i += majorTickUnitAngle)
        {
            var majortickgp = new TransformGroup();
            majortickgp.Children.Add(new RotateTransform { Angle = i });

            // Obtaining the angle in radians for calculating the points
            var iRadian = (i * Math.PI) / 180;

            // Finding the point on the Scale where the major ticks are drawn here drawing the
            // points with center as (0,0)
            var majorticktt = new TranslateTransform
            {
                X = (int)(ScaleRadius * Math.Cos(iRadian)),
                Y = (int)(ScaleRadius * Math.Sin(iRadian))
            };

            // Points for the text block which hold the scale value here drawing the points with
            // center as (0,0)
            var majorscalevaluett = new TranslateTransform
            {
                X = (int)(ScaleLabelRadius * Math.Cos(iRadian)),
                Y = (int)(ScaleLabelRadius * Math.Sin(iRadian))
            };

            // Defining the properties of the scale value text box
            var tb = new TextBlock
            {
                Height = ScaleLabelSize.Height,
                Width = ScaleLabelSize.Width,
                FontSize = ScaleLabelFontSize,
                Foreground = ScaleForeground,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                RenderTransform = majorscalevaluett
            };

            // Writing and appending the scale value checking min value < max value w.r.t scale
            // precision value
            if (Math.Round(minvalue, ScaleValuePrecision) <= Math.Round(MaxValue, ScaleValuePrecision))
            {
                minvalue = Math.Round(minvalue, ScaleValuePrecision);
                tb.Text = minvalue.ToString(CultureInfo.InvariantCulture);
                minvalue += majorTicksUnitValue;
            }
            else
            {
                break;
            }

            majortickgp.Children.Add(majorticktt);

            // Major tick is drawn as a rectangle
            var majortickrect = new Rectangle
            {
                Height = MajorTickSize.Height,
                Width = MajorTickSize.Width,
                Fill = ScaleForeground,
                RenderTransformOrigin = new Point(0.5, 0.5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransform = majortickgp
            };
            _rootGrid.Children.Add(majortickrect);
            _rootGrid.Children.Add(tb);

            // Drawing the minor axis ticks
            var onedegree = ((i + majorTickUnitAngle) - i) / MinorDivisionsCount;
            if ((i < (ScaleStartAngle + ScaleSweepAngle)) && (Math.Round(minvalue, ScaleValuePrecision) <= Math.Round(MaxValue, ScaleValuePrecision)))
            {
                // Drawing the minor scale
                for (var mi = i + onedegree; mi < (i + majorTickUnitAngle); mi += onedegree)
                {
                    // Obtaining the angle in radians for calculating the points
                    var miRadian = (mi * Math.PI) / 180;

                    // Finding the point on the Scale where the minor ticks are drawn
                    var minorticktt = new TranslateTransform
                    {
                        X = (int)(ScaleRadius * Math.Cos(miRadian)),
                        Y = (int)(ScaleRadius * Math.Sin(miRadian))
                    };
                    var minortickgp = new TransformGroup();
                    minortickgp.Children.Add(new RotateTransform { Angle = mi });
                    minortickgp.Children.Add(minorticktt);

                    // here the minor tick is drawn as a rectangle
                    var mr = new Rectangle
                    {
                        Height = MinorTickSize.Height,
                        Width = MinorTickSize.Width,
                        Fill = ScaleForeground,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransformOrigin = new Point(0.5, 0.5),
                        RenderTransform = minortickgp
                    };
                    _rootGrid.Children.Add(mr);
                    _ht.Add("mr_" + _numberOfpoints + "_" + _numberOfMinorpoints, mr);
                    _numberOfMinorpoints++;
                }
            }

            _ht.Add("tb_" + _numberOfpoints, tb);
            _ht.Add("rec_" + _numberOfpoints, majortickrect);
            _numberOfpoints++;
        }
    }

    /// <summary>
    /// Drawing the segment with two arc and two line.
    /// </summary>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="reflexangle">if set to <c>true</c> [reflexangle].</param>
    /// <param name="clr">The color.</param>
    /// <returns>The Path.</returns>
    private Path DrawSegment(Point p1, Point p2, Point p3, Point p4, bool reflexangle, Brush clr)
    {
        // Segment Geometry
        var segments = new PathSegmentCollection
            {
                new LineSegment { Point = p2 },
                new ArcSegment
                {
                    Size = new Size(_arcradius2, _arcradius2),
                    Point = p3, SweepDirection = SweepDirection.Clockwise, IsLargeArc = reflexangle
                },
                new LineSegment { Point = p4 }, new ArcSegment
                {
                    Size = new Size(_arcradius1, _arcradius1),
                    Point = p1, SweepDirection = SweepDirection.Counterclockwise, IsLargeArc = reflexangle
                }
            };

        // First line segment from pt p1 - pt p2
        // Arc drawn from pt p2 - pt p3 with the RangeIndicatorRadius
        // Second line segment from pt p3 - pt p4
        // Arc drawn from pt p4 - pt p1 with the Radius of arcradius1
        // Defining the segment path properties
        var rangestrokecolor = clr == Brushes.Transparent ? clr : Brushes.White;
        var range = new Path
        {
            StrokeLineJoin = PenLineJoin.Round,
            Stroke = rangestrokecolor,
            Fill = clr,
            Opacity = 0.65,
            StrokeThickness = 0.25,
            Data = new PathGeometry
            {
                Figures =
                    [
                        new PathFigure
                        {
                            IsClosed = true,
                            StartPoint = p1,
                            Segments = segments
                        }
                    ]
            }
        };

        // Set Z index of range indicator
        range.SetValue(Panel.ZIndexProperty, 150);

        // Adding the segment to the root grid
        _rootGrid?.Children.Add(range);

        return range;
    }

    private Point GetCircumferencePoint(double angle, double radius)
    {
        var angleRadian = (angle * Math.PI) / 180;

        // Radius-- is the Radius of the gauge
        return new Point(Radius + (radius * Math.Cos(angleRadian)), Radius + (radius * Math.Sin(angleRadian)));
    }

    private void MovePointer(double angleValue)
    {
        if (_pointer?.RenderTransform is TransformGroup transformGroup && transformGroup.Children[0] is RotateTransform rotateTransform)
        {
            rotateTransform.Angle = angleValue;
        }
    }

    private void Sb_Completed(object? sender, EventArgs e)
    {
        if (Value > OptimalRangeEndValue)
        {
            _lightIndicator?.Fill = GetRangeIndicatorGradEffect(AboveOptimalRangeColor);
        }
        else if (Value <= OptimalRangeEndValue && Value >= OptimalRangeStartValue)
        {
            _lightIndicator?.Fill = GetRangeIndicatorGradEffect(OptimalRangeColor);
        }
        else if (Value < OptimalRangeStartValue)
        {
            _lightIndicator?.Fill = GetRangeIndicatorGradEffect(BelowOptimalRangeColor);
        }
    }
}
