// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Contains CircularGauge state and dependency-property wrappers.</summary>
public sealed partial class CircularGauge
{
    /// <summary>Provides the pointer template part name.</summary>
    private const string Pointer = nameof(Pointer);

    /// <summary>Provides the foreground pointer z-index.</summary>
    private const int PointerZIndex = 100_000;

    /// <summary>Provides the pointer cap z-index.</summary>
    private const int PointerCapZIndex = PointerZIndex + 1;

    /// <summary>Provides the first range gradient stop offset.</summary>
    private const double RangeGradientStartOffset = 0.2;

    /// <summary>Provides the middle range gradient stop offset.</summary>
    private const double RangeGradientMiddleOffset = 0.5;

    /// <summary>Provides the final range gradient stop offset.</summary>
    private const double RangeGradientEndOffset = 0.8;

    /// <summary>Provides the full semi-circle angle in degrees.</summary>
    private const double SemiCircleDegrees = 180.0;

    /// <summary>Provides the center render transform origin coordinate.</summary>
    private const double CenterOrigin = 0.5;

    /// <summary>Provides the value detection poll interval.</summary>
    private const int DetectionPollIntervalMilliseconds = 500;

    /// <summary>Provides the number of detection polls per timeout unit.</summary>
    private const int DetectionPollsPerTimeoutUnit = 2;

    /// <summary>Provides the range segment opacity.</summary>
    private const double RangeSegmentOpacity = 0.65;

    /// <summary>Provides the range segment stroke thickness.</summary>
    private const double RangeSegmentStrokeThickness = 0.25;

    /// <summary>Provides the range segment z-index.</summary>
    private const int RangeSegmentZIndex = 150;

    /// <summary>Provides the AnimatingSpeedFactor member.</summary>
    private const int AnimatingSpeedFactor = 6;

    /// <summary>Stores the _ht value.</summary>
    private readonly Hashtable _ht = [];

    /// <summary>Stores the _lockObject value.</summary>
    private readonly object _lockObject = new();

    /// <summary>Stores the _arcradius1 value.</summary>
    private double _arcradius1;

    /// <summary>Stores the _arcradius2 value.</summary>
    private double _arcradius2;

    /// <summary>Stores the _detectValuesChanging value.</summary>
    private bool _detectValuesChanging;

    /// <summary>Stores the _lightIndicator value.</summary>
    private Ellipse? _lightIndicator;

    /// <summary>Stores the _numberOfMinorpoints value.</summary>
    private int _numberOfMinorpoints;

    /// <summary>Stores the _numberOfpoints value.</summary>
    private int _numberOfpoints;

    /// <summary>Stores the _oldcurrRealworldunit value.</summary>
    private double _oldcurrRealworldunit;

    /// <summary>Stores the _pointer value.</summary>
    private Path? _pointer;

    /// <summary>Stores the _pointerCap value.</summary>
    private Ellipse? _pointerCap;

    /// <summary>Stores the _rangeIndicator1 value.</summary>
    private Path? _rangeIndicator1;

    /// <summary>Stores the _rangeIndicator2 value.</summary>
    private Path? _rangeIndicator2;

    /// <summary>Stores the _rangeIndicator3 value.</summary>
    private Path? _rangeIndicator3;

    /// <summary>Stores the _rootGrid value.</summary>
    private Grid? _rootGrid;

    /// <summary>Stores the _valueChanged value.</summary>
    private bool _valueChanged;

    /// <summary>Initializes static members of the <see cref="CircularGauge"/> class.</summary>
    static CircularGauge() =>
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CircularGauge),
            new FrameworkPropertyMetadata(typeof(CircularGauge)));

    /// <summary>Gets or sets /Sets Above Optimal Range Color.</summary>
    [Description("Gets/Sets Above Optimal Range Color")]
    public Brush AboveOptimalRangeColor
    {
        get => (Brush)GetValue(AboveOptimalRangeColorProperty);
        set => SetValue(AboveOptimalRangeColorProperty, value);
    }

    /// <summary>Gets or sets /Sets Below Optimal Range Color.</summary>
    [Description("Gets/Sets Below Optimal Range Color")]
    public Brush BelowOptimalRangeColor
    {
        get => (Brush)GetValue(BelowOptimalRangeColorProperty);
        set => SetValue(BelowOptimalRangeColorProperty, value);
    }

    /// <summary>Gets or sets the decimals.</summary>
    /// <value>The decimals.</value>
    [Description("Gets or sets the decimals.")]
    [Category("CrissCross Important")]
    public int Decimals
    {
        get => (int)GetValue(DecimalsProperty);
        set => SetValue(DecimalsProperty, value);
    }

    /// <summary>Gets or sets the detection time out.</summary>
    /// <value>The detection timeout.</value>
    [Description(
        "Gets or sets the detection time out, so the value hasn't changed for a period and thus an error will show.")]
    [Category("CrissCross Detection")]
    public int DetectionTimeOut
    {
        get => (int)GetValue(DetectionTimeOutProperty);
        set => SetValue(DetectionTimeOutProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether [detect value or error].</summary>
    /// <value><c>true</c> if [detect value or error]; otherwise, <c>false</c>.</value>
    [Description(
        "Gets or sets a value indicating whether to start checking if the value is changing or it has an error.")]
    [Category("CrissCross Detection")]
    public bool DetectValueOrError
    {
        get => (bool)GetValue(DetectValueOrErrorProperty);
        set => SetValue(DetectValueOrErrorProperty, value);
    }

    /// <summary>Gets or sets /Sets Dial Text.</summary>
    [Description("Gets/Sets Dial Text")]
    [Category("CrissCross Dial Text")]
    public string DialText
    {
        get => (string)GetValue(DialTextProperty);
        set => SetValue(DialTextProperty, value);
    }

    /// <summary>Gets or sets /Sets Dial Text Font Size.</summary>
    [Description("Gets/Sets Dial Text Font Size")]
    [Category("CrissCross Dial Text")]
    public double DialTextFontSize
    {
        get => (double)GetValue(DialTextFontSizeProperty);
        set => SetValue(DialTextFontSizeProperty, value);
    }

    /// <summary>Gets or sets /Sets Dial Text Font Size.</summary>
    [Description("Gets/Sets Value Text Font Size")]
    [Category("CrissCross Dial Text")]
    public double ValueTextFontSize
    {
        get => (double)GetValue(ValueTextFontSizeProperty);
        set => SetValue(ValueTextFontSizeProperty, value);
    }

    /// <summary>Gets or sets /Sets Dial Text Offset.</summary>
    [Description("Gets/Sets Dial Text Offset")]
    [Category("CrissCross Dial Text")]
    public double DialTextOffset
    {
        get => (double)GetValue(DialTextOffsetProperty);
        set => SetValue(DialTextOffsetProperty, value);
    }

    /// <summary>Gets or sets the display alignment.</summary>
    /// <value>The display alignment.</value>
    [Description("Gets or sets the display alignment.")]
    [Category("CrissCross Important")]
    public TextAlignment DisplayAlignment
    {
        get => (TextAlignment)GetValue(DisplayAlignmentProperty);
        set => SetValue(DisplayAlignmentProperty, value);
    }

    /// <summary>Gets or sets the display border colour.</summary>
    /// <value>The display border colour.</value>
    [Description("Gets or sets the display border colour.")]
    public Brush DisplayBorderColour
    {
        get => (Brush)GetValue(DisplayBorderColourProperty);
        set => SetValue(DisplayBorderColourProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether [display value].</summary>
    /// <value><c>true</c> if [display value]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether [display value].")]
    [Category("CrissCross Important")]
    public Visibility DisplayValue
    {
        get => (Visibility)GetValue(DisplayValueProperty);
        set => SetValue(DisplayValueProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets the error.</summary>
    /// <value>The error.</value>
    [Description("Gets or sets the error")]
    [Category("CrissCross")]
    public bool Error
    {
        get => (bool)GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    /// <summary>Gets or sets /Sets the Image offset.</summary>
    [Description("Gets/Sets the Image offset")]
    [Category("CrissCross Image")]
    public double ImageOffset
    {
        get => (double)GetValue(ImageOffsetProperty);
        set => SetValue(ImageOffsetProperty, value);
    }

    /// <summary>Gets or sets /Sets the Image width and height.</summary>
    [Description("Gets/Sets the Image width and height")]
    [Category("CrissCross Image")]
    public Size ImageSize
    {
        get => (Size)GetValue(ImageSizeProperty);
        set => SetValue(ImageSizeProperty, value);
    }

    /// <summary>Gets or sets /Sets the Gauge image source.</summary>
    [Description("Gets/Sets the Gauge image source")]
    [Category("CrissCross Image")]
    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    /// <summary>Gets or sets /Sets the number of major divisions on the scale.</summary>
    [Description("Gets/Sets the number of major divisions on the scale")]
    [Category("CrissCross")]
    public double MajorDivisionsCount
    {
        get => (double)GetValue(MajorDivisionsCountProperty);
        set => SetValue(MajorDivisionsCountProperty, value);
    }

    /// <summary>Gets or sets /Sets the Major Tick Size.</summary>
    [Description("Gets/Sets the Major Tick Size")]
    [Category("CrissCross")]
    public Size MajorTickSize
    {
        get => (Size)GetValue(MajorTickSizeProperty);
        set => SetValue(MajorTickSizeProperty, value);
    }

    /// <summary>Gets or sets /Sets the Maximum Value.</summary>
    [Description("Gets/Sets the Maximum Value")]
    [Category("CrissCross Important")]
    public double MaxValue
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>Gets or sets /Sets the number of minor divisions on the scale.</summary>
    [Description("Gets/Sets the number of minor divisions on the scale")]
    [Category("CrissCross")]
    public double MinorDivisionsCount
    {
        get => (double)GetValue(MinorDivisionsCountProperty);
        set => SetValue(MinorDivisionsCountProperty, value);
    }

    /// <summary>Gets or sets /Sets the Minor Tick Size.</summary>
    [Description("Gets/Sets the Minor Tick Size")]
    [Category("CrissCross")]
    public Size MinorTickSize
    {
        get => (Size)GetValue(MinorTickSizeProperty);
        set => SetValue(MinorTickSizeProperty, value);
    }

    /// <summary>Gets or sets /Sets the Minimum Value.</summary>
    [Description("Gets/Sets the Minimum Value")]
    [Category("CrissCross Important")]
    public double MinValue
    {
        get => (double)GetValue(MinValueProperty);
        set
        {
            SetValue(MinValueProperty, value);
            RefreshDialRange();
        }
    }

    /// <summary>Gets or sets /Sets Optimal Range Color.</summary>
    [Description("Gets/Sets Optimal Range Color")]
    public Brush OptimalRangeColor
    {
        get => (Brush)GetValue(OptimalRangeColorProperty);
        set => SetValue(OptimalRangeColorProperty, value);
    }

    /// <summary>Gets or sets /Sets the Optimal range end value.</summary>
    [Description("Gets/Sets the Optimal range end value")]
    [Category("CrissCross Important")]
    public double OptimalRangeEndValue
    {
        get => (double)GetValue(OptimalRangeEndValueProperty);
        set => SetValue(OptimalRangeEndValueProperty, value);
    }

    /// <summary>Gets or sets /Sets the Optimal Range Start Value.</summary>
    [Description("Gets/Sets the Optimal Range Start Value")]
    [Category("CrissCross Important")]
    public double OptimalRangeStartValue
    {
        get => (double)GetValue(OptimalRangeStartValueProperty);
        set => SetValue(OptimalRangeStartValueProperty, value);
    }

    /// <summary>Gets or sets /Sets the Pointer cap radius.</summary>
    [Description("Gets/Sets the Pointer cap radius")]
    [Category("CrissCross Pointer")]
    public double PointerCapRadius
    {
        get => (double)GetValue(PointerCapRadiusProperty);
        set => SetValue(PointerCapRadiusProperty, value);
    }

    /// <summary>Gets or sets /Sets the Pointer Length.</summary>
    [Description("Gets/Sets the Pointer Length")]
    [Category("CrissCross Pointer")]
    public double PointerLength
    {
        get => (double)GetValue(PointerLengthProperty);
        set => SetValue(PointerLengthProperty, value);
    }

    /// <summary>Gets or sets /Sets the Pointer Thickness.</summary>
    [Description("Gets/Sets the Pointer Thickness")]
    [Category("CrissCross Pointer")]
    public double PointerThickness
    {
        get => (double)GetValue(PointerThicknessProperty);
        set => SetValue(PointerThicknessProperty, value);
    }

    /// <summary>Gets or sets /Sets the Minimum Value.</summary>
    [Description("Gets/Sets the Minimum Value")]
    [Category("CrissCross")]
    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    /// <summary>Gets or sets /Sets the Range Indicator Light offset.</summary>
    [Description("Gets/Sets the Range Indicator Light offset")]
    [Category("CrissCross")]
    public double RangeIndicatorLightOffset
    {
        get => (double)GetValue(RangeIndicatorLightOffsetProperty);
        set => SetValue(RangeIndicatorLightOffsetProperty, value);
    }

    /// <summary>Gets or sets /Sets Range Indicator Light Radius.</summary>
    [Description("Gets/Sets Range Indicator Light Radius")]
    [Category("CrissCross")]
    public double RangeIndicatorLightRadius
    {
        get => (double)GetValue(RangeIndicatorLightRadiusProperty);
        set => SetValue(RangeIndicatorLightRadiusProperty, value);
    }

    /// <summary>Gets or sets the range indicator light visible.</summary>
    /// <value>The range indicator light visible.</value>
    [Description("Gets/Sets the Range Indicator Light Visible value")]
    [Category("CrissCross")]
    public Visibility RangeIndicatorLightVisible
    {
        get => (Visibility)GetValue(RangeIndicatorLightVisibleProperty);
        set => SetValue(RangeIndicatorLightVisibleProperty, value);
    }

    /// <summary>Gets or sets /Sets the Range Indicator Radius.</summary>
    [Description("Gets/Sets the Range Indicator Radius")]
    [Category("CrissCross")]
    public double RangeIndicatorRadius
    {
        get => (double)GetValue(RangeIndicatorRadiusProperty);
        set => SetValue(RangeIndicatorRadiusProperty, value);
    }

    /// <summary>Gets or sets /Sets the Range Indicator Thickness.</summary>
    [Description("Gets/Sets the Range Indicator Thickness")]
    [Category("CrissCross")]
    public double RangeIndicatorThickness
    {
        get => (double)GetValue(RangeIndicatorThicknessProperty);
        set => SetValue(RangeIndicatorThicknessProperty, value);
    }

    /// <summary>Gets or sets the GetValue value.</summary>
    [Description("Gets/Sets option to reset the pointer to minimum on start up, Default is true")]
    [Category("CrissCross Pointer")]
    public bool ResetPointerOnStartUp
    {
        get => (bool)GetValue(ResetPointerOnStartUpProperty);
        set => SetValue(ResetPointerOnStartUpProperty, value);
    }

    /// <summary>Gets or sets /Sets the Scale Label Foreground.</summary>
    [Description("Gets/Sets the Scale Label Foreground")]
    public Brush ScaleForeground
    {
        get => (Brush)GetValue(ScaleForegroundProperty);
        set => SetValue(ScaleForegroundProperty, value);
    }

    /// <summary>Gets or sets /Sets the Scale Label Font Size.</summary>
    [Description("Gets/Sets the Scale Label Font Size")]
    [Category("CrissCross Scale")]
    public double ScaleLabelFontSize
    {
        get => (double)GetValue(ScaleLabelFontSizeProperty);
        set => SetValue(ScaleLabelFontSizeProperty, value);
    }

    /// <summary>Gets or sets /Sets the Scale Label Radius.</summary>
    [Description("Gets/Sets the Scale Label Radius")]
    [Category("CrissCross Scale")]
    public double ScaleLabelRadius
    {
        get => (double)GetValue(ScaleLabelRadiusProperty);
        set => SetValue(ScaleLabelRadiusProperty, value);
    }

    /// <summary>Gets or sets /Sets the Scale Label Size.</summary>
    [Description("Gets/Sets the Scale Label Size")]
    [Category("CrissCross Scale")]
    public Size ScaleLabelSize
    {
        get => (Size)GetValue(ScaleLabelSizeProperty);
        set => SetValue(ScaleLabelSizeProperty, value);
    }

    /// <summary>Gets or sets /Sets the Scale radius.</summary>
    [Description("Gets/Sets the Scale radius")]
    [Category("CrissCross Scale")]
    public double ScaleRadius
    {
        get => (double)GetValue(ScaleRadiusProperty);
        set => SetValue(ScaleRadiusProperty, value);
    }

    /// <summary>Gets or sets /Sets the scale start angle.</summary>
    [Description("Gets/Sets the scale start angle")]
    [Category("CrissCross Scale")]
    public double ScaleStartAngle
    {
        get => (double)GetValue(ScaleStartAngleProperty);
        set => SetValue(ScaleStartAngleProperty, value);
    }

    /// <summary>Gets or sets /Sets the scale sweep angle.</summary>
    [Description("Gets/Sets the scale sweep angle")]
    [Category("CrissCross Scale")]
    public double ScaleSweepAngle
    {
        get => (double)GetValue(ScaleSweepAngleProperty);
        set => SetValue(ScaleSweepAngleProperty, value);
    }

    /// <summary>Gets or sets /Sets scale value precision.</summary>
    [Description("Gets/Sets scale value precision")]
    [Category("CrissCross Scale")]
    public int ScaleValuePrecision
    {
        get => (int)GetValue(ScaleValuePrecisionProperty);
        set => SetValue(ScaleValuePrecisionProperty, value);
    }

    /// <summary>Gets or sets the show error.</summary>
    /// <value>The show error.</value>
    [Description("Gets or sets the show error red cross")]
    [Category("CrissCross")]
    public Visibility ShowError
    {
        get => (Visibility)GetValue(ShowErrorProperty);
        set => SetValue(ShowErrorProperty, value);
    }

    /// <summary>Gets or sets the unit.</summary>
    /// <value>The unit.</value>
    [Description("Gets or sets the unit")]
    [Category("CrissCross Important")]
    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>Gets or sets /Sets the current value.</summary>
    [Description("Gets/Sets the current value")]
    [Category("CrissCross Important")]
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Gets or sets the value background.</summary>
    /// <value>The value background.</value>
    public Brush ValueBackground
    {
        get => (Brush)GetValue(ValueBackgroundProperty);
        set => SetValue(ValueBackgroundProperty, value);
    }
}
