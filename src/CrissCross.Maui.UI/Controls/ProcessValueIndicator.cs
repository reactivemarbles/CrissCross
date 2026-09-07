// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays an industrial process reading with range, quality, and alarm state.</summary>
public class ProcessValueIndicator : ContentView
{
    /// <summary>Bindable property for <see cref="State"/>.</summary>
    public static readonly BindableProperty StateProperty;

    /// <summary>Bindable property for <see cref="Label"/>.</summary>
    public static readonly BindableProperty LabelProperty;

    /// <summary>Bindable property for <see cref="DisplayText"/>.</summary>
    public static readonly BindableProperty DisplayTextProperty;

    /// <summary>Bindable property for <see cref="StatusText"/>.</summary>
    public static readonly BindableProperty StatusTextProperty;

    /// <summary>Bindable property for <see cref="Percentage"/>.</summary>
    public static readonly BindableProperty PercentageProperty;

    /// <summary>Bindable property for <see cref="NormalizedValue"/>.</summary>
    public static readonly BindableProperty NormalizedValueProperty;

    /// <summary>Bindable property for <see cref="HasValidValue"/>.</summary>
    public static readonly BindableProperty HasValidValueProperty;

    /// <summary>Bindable property for <see cref="IsAlarm"/>.</summary>
    public static readonly BindableProperty IsAlarmProperty;

    /// <summary>Bindable property for <see cref="Status"/>.</summary>
    public static readonly BindableProperty StatusProperty;

    /// <summary>Bindable property for <see cref="NormalStatusColor"/>.</summary>
    public static readonly BindableProperty NormalStatusColorProperty;

    /// <summary>Bindable property for <see cref="AlarmStatusColor"/>.</summary>
    public static readonly BindableProperty AlarmStatusColorProperty;

    /// <summary>Bindable property for <see cref="BadQualityStatusColor"/>.</summary>
    public static readonly BindableProperty BadQualityStatusColorProperty;

    /// <summary>Bindable property for <see cref="InvalidConfigurationStatusColor"/>.</summary>
    public static readonly BindableProperty InvalidConfigurationStatusColorProperty;

    /// <summary>Bindable property for <see cref="RangeTrackColor"/>.</summary>
    public static readonly BindableProperty RangeTrackColorProperty;

    /// <summary>Minimum visual bar width used for non-zero readings.</summary>
    private const double MinimumFilledBarWidth = 2;

    /// <summary>Maximum percentage value for display calculations.</summary>
    private const double MaximumPercentage = 100;

    /// <summary>Layout spacing between rows.</summary>
    private const double LayoutSpacing = 6;

    /// <summary>Default range bar height.</summary>
    private const double RangeBarHeight = 8;

    /// <summary>Default value label size.</summary>
    private const double ValueFontSize = 22;

    /// <summary>Default metadata label size.</summary>
    private const double MetadataFontSize = 14;

    /// <summary>Fallback state used when no process-value snapshot has been supplied.</summary>
    private static readonly ProcessValueState EmptyState = new(
        "Process value",
        null,
        string.Empty,
        0D,
        MaximumPercentage,
        new ProcessValueOptions { IsGoodQuality = false });

    /// <summary>Bindable property key for <see cref="Label"/>.</summary>
    private static readonly BindablePropertyKey LabelPropertyKey;

    /// <summary>Bindable property key for <see cref="DisplayText"/>.</summary>
    private static readonly BindablePropertyKey DisplayTextPropertyKey;

    /// <summary>Bindable property key for <see cref="StatusText"/>.</summary>
    private static readonly BindablePropertyKey StatusTextPropertyKey;

    /// <summary>Bindable property key for <see cref="Percentage"/>.</summary>
    private static readonly BindablePropertyKey PercentagePropertyKey;

    /// <summary>Bindable property key for <see cref="NormalizedValue"/>.</summary>
    private static readonly BindablePropertyKey NormalizedValuePropertyKey;

    /// <summary>Bindable property key for <see cref="HasValidValue"/>.</summary>
    private static readonly BindablePropertyKey HasValidValuePropertyKey;

    /// <summary>Bindable property key for <see cref="IsAlarm"/>.</summary>
    private static readonly BindablePropertyKey IsAlarmPropertyKey;

    /// <summary>Bindable property key for <see cref="Status"/>.</summary>
    private static readonly BindablePropertyKey StatusPropertyKey;

    /// <summary>Displays the process label.</summary>
    private readonly Label _label = new() { FontAttributes = FontAttributes.Bold, FontSize = MetadataFontSize };

    /// <summary>Displays the process value and unit.</summary>
    private readonly Label _value = new() { FontAttributes = FontAttributes.Bold, FontSize = ValueFontSize };

    /// <summary>Displays the process condition text.</summary>
    private readonly Label _status = new() { FontSize = MetadataFontSize };

    /// <summary>Hosts the normalized range bar.</summary>
    private readonly Grid _bar = new() { HeightRequest = RangeBarHeight };

    /// <summary>Displays the normalized range fill.</summary>
    private readonly BoxView _barFill = new() { HorizontalOptions = LayoutOptions.Start };

    /// <summary>Initializes static members of the <see cref="ProcessValueIndicator"/> class.</summary>
    static ProcessValueIndicator()
    {
        StateProperty = BindableProperty.Create(
            nameof(State),
            typeof(ProcessValueState),
            typeof(ProcessValueIndicator),
            propertyChanged: static (view, _, _) => ((ProcessValueIndicator)view).Refresh());
        LabelPropertyKey = CreateReadOnlyProperty(nameof(Label), EmptyState.Label);
        DisplayTextPropertyKey = CreateReadOnlyProperty(nameof(DisplayText), EmptyState.DisplayText);
        StatusTextPropertyKey = CreateReadOnlyProperty(nameof(StatusText), EmptyState.StatusText);
        PercentagePropertyKey = CreateReadOnlyProperty(nameof(Percentage), EmptyState.Percentage);
        NormalizedValuePropertyKey = CreateReadOnlyProperty(nameof(NormalizedValue), EmptyState.NormalizedValue);
        HasValidValuePropertyKey = CreateReadOnlyProperty(nameof(HasValidValue), EmptyState.HasValidValue);
        IsAlarmPropertyKey = CreateReadOnlyProperty(nameof(IsAlarm), EmptyState.IsAlarm);
        StatusPropertyKey = CreateReadOnlyProperty(nameof(Status), EmptyState.Status);
        LabelProperty = LabelPropertyKey.BindableProperty;
        DisplayTextProperty = DisplayTextPropertyKey.BindableProperty;
        StatusTextProperty = StatusTextPropertyKey.BindableProperty;
        PercentageProperty = PercentagePropertyKey.BindableProperty;
        NormalizedValueProperty = NormalizedValuePropertyKey.BindableProperty;
        HasValidValueProperty = HasValidValuePropertyKey.BindableProperty;
        IsAlarmProperty = IsAlarmPropertyKey.BindableProperty;
        StatusProperty = StatusPropertyKey.BindableProperty;
        NormalStatusColorProperty = CreateThemeColorProperty(nameof(NormalStatusColor));
        AlarmStatusColorProperty = CreateThemeColorProperty(nameof(AlarmStatusColor));
        BadQualityStatusColorProperty = CreateThemeColorProperty(nameof(BadQualityStatusColor));
        InvalidConfigurationStatusColorProperty = CreateThemeColorProperty(nameof(InvalidConfigurationStatusColor));
        RangeTrackColorProperty = CreateThemeColorProperty(nameof(RangeTrackColor));
    }

    /// <summary>Initializes a new instance of the <see cref="ProcessValueIndicator"/> class.</summary>
    public ProcessValueIndicator()
    {
        var layout = new VerticalStackLayout { Spacing = LayoutSpacing, Children = { _label, _value, _bar, _status } };
        _bar.Children.Add(_barFill);
        _bar.SizeChanged += (_, _) => RefreshBarWidth();
        Content = layout;
        Refresh();
    }

    /// <summary>Gets or sets the process-value snapshot projected by the control.</summary>
    public ProcessValueState? State
    {
        get => (ProcessValueState?)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets the measurement label projected from <see cref="State"/>.</summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        private set => SetValue(LabelPropertyKey, value);
    }

    /// <summary>Gets the measurement text projected from <see cref="State"/>.</summary>
    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        private set => SetValue(DisplayTextPropertyKey, value);
    }

    /// <summary>Gets the condition text projected from <see cref="State"/>.</summary>
    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        private set => SetValue(StatusTextPropertyKey, value);
    }

    /// <summary>Gets the range percentage projected from <see cref="State"/>.</summary>
    public double Percentage
    {
        get => (double)GetValue(PercentageProperty);
        private set => SetValue(PercentagePropertyKey, value);
    }

    /// <summary>Gets the normalized range position projected from <see cref="State"/>.</summary>
    public double NormalizedValue
    {
        get => (double)GetValue(NormalizedValueProperty);
        private set => SetValue(NormalizedValuePropertyKey, value);
    }

    /// <summary>Gets a value indicating whether the current reading is displayable.</summary>
    public bool HasValidValue
    {
        get => (bool)GetValue(HasValidValueProperty);
        private set => SetValue(HasValidValuePropertyKey, value);
    }

    /// <summary>Gets a value indicating whether the current reading is in alarm.</summary>
    public bool IsAlarm
    {
        get => (bool)GetValue(IsAlarmProperty);
        private set => SetValue(IsAlarmPropertyKey, value);
    }

    /// <summary>Gets the current process-value condition.</summary>
    public ProcessValueStatus Status
    {
        get => (ProcessValueStatus)GetValue(StatusProperty);
        private set => SetValue(StatusPropertyKey, value);
    }

    /// <summary>Gets or sets the theme color used for normal readings.</summary>
    public Color? NormalStatusColor
    {
        get => (Color?)GetValue(NormalStatusColorProperty);
        set => SetValue(NormalStatusColorProperty, value);
    }

    /// <summary>Gets or sets the theme color used for alarm readings.</summary>
    public Color? AlarmStatusColor
    {
        get => (Color?)GetValue(AlarmStatusColorProperty);
        set => SetValue(AlarmStatusColorProperty, value);
    }

    /// <summary>Gets or sets the theme color used for bad-quality readings.</summary>
    public Color? BadQualityStatusColor
    {
        get => (Color?)GetValue(BadQualityStatusColorProperty);
        set => SetValue(BadQualityStatusColorProperty, value);
    }

    /// <summary>Gets or sets the theme color used for invalid configuration readings.</summary>
    public Color? InvalidConfigurationStatusColor
    {
        get => (Color?)GetValue(InvalidConfigurationStatusColorProperty);
        set => SetValue(InvalidConfigurationStatusColorProperty, value);
    }

    /// <summary>Gets or sets the theme color used for the unfilled range track.</summary>
    public Color? RangeTrackColor
    {
        get => (Color?)GetValue(RangeTrackColorProperty);
        set => SetValue(RangeTrackColorProperty, value);
    }

    /// <summary>Creates a read-only bindable property key for a projected state value.</summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="name">The public property name.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The read-only bindable property key.</returns>
    private static BindablePropertyKey CreateReadOnlyProperty<T>(string name, T defaultValue) => BindableProperty.CreateReadOnly(
        name,
        typeof(T),
        typeof(ProcessValueIndicator),
        defaultValue);

    /// <summary>Creates a theme color property that refreshes the composed native view.</summary>
    /// <param name="name">The public property name.</param>
    /// <returns>The bindable property definition.</returns>
    private static BindableProperty CreateThemeColorProperty(string name) => BindableProperty.Create(
        name,
        typeof(Color),
        typeof(ProcessValueIndicator),
        propertyChanged: static (view, _, _) => ((ProcessValueIndicator)view).ApplyStatusTheme());

    /// <summary>Updates the visual and accessibility projection from the current state snapshot.</summary>
    private void Refresh()
    {
        var state = State ?? EmptyState;
        Label = state.Label;
        DisplayText = state.DisplayText;
        StatusText = state.StatusText;
        Percentage = state.Percentage;
        NormalizedValue = state.NormalizedValue;
        HasValidValue = state.HasValidValue;
        IsAlarm = state.IsAlarm;
        Status = state.Status;

        _label.Text = Label;
        _label.IsVisible = !string.IsNullOrWhiteSpace(Label);
        _value.Text = DisplayText;
        _status.Text = StatusText;
        ApplyStatusTheme();
        RefreshBarWidth();
        SemanticProperties.SetDescription(this, $"{Label} {DisplayText} {StatusText}".Trim());
    }

    /// <summary>Updates semantic colors for the current condition.</summary>
    private void ApplyStatusTheme()
    {
        var statusColor = Status switch
        {
            ProcessValueStatus.LowAlarm or ProcessValueStatus.HighAlarm => AlarmStatusColor,
            ProcessValueStatus.BadQuality => BadQualityStatusColor,
            ProcessValueStatus.InvalidConfiguration => InvalidConfigurationStatusColor,
            _ => NormalStatusColor,
        };

        _status.TextColor = statusColor;
        _barFill.Color = statusColor;
        _bar.BackgroundColor = RangeTrackColor;
    }

    /// <summary>Updates the filled range bar width from the current percentage.</summary>
    private void RefreshBarWidth()
    {
        var width = _bar.Width;
        if (width <= 0)
        {
            return;
        }

        var fillWidth = width * Math.Clamp(Percentage, 0, MaximumPercentage) / MaximumPercentage;
        _barFill.WidthRequest = fillWidth <= 0 ? 0 : Math.Max(fillWidth, MinimumFilledBarWidth);
    }
}
