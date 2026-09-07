// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays validation and metadata for a reactive form field.</summary>
public class ReactiveFormField : ContentView
{
    /// <summary>Bindable property for <see cref="FieldState"/>.</summary>
    public static readonly BindableProperty FieldStateProperty = BindableProperty.Create(
        nameof(FieldState),
        typeof(FormFieldState),
        typeof(ReactiveFormField),
        FormFieldState.Normal,
        propertyChanged: static (view, _, value) => ((ReactiveFormField)view).State = (FormFieldState)value,
        coerceValue: static (_, value) => value ?? FormFieldState.Normal);

    /// <summary>Bindable property for <see cref="State"/>.</summary>
    public static readonly BindableProperty StateProperty = BindableProperty.Create(
        nameof(State),
        typeof(FormFieldState),
        typeof(ReactiveFormField),
        FormFieldState.Normal,
        propertyChanged: static (view, _, value) => ((ReactiveFormField)view).UpdateState((FormFieldState)value));

    /// <summary>Bindable property for <see cref="Header"/>.</summary>
    public static readonly BindableProperty HeaderProperty = CreateProperty<object?>(nameof(Header), null);

    /// <summary>Bindable property for <see cref="HelperText"/>.</summary>
    public static readonly BindableProperty HelperTextProperty = CreateProperty<string?>(nameof(HelperText), null);

    /// <summary>Bindable property for <see cref="FieldKey"/>.</summary>
    public static readonly BindableProperty FieldKeyProperty = CreateProperty<string?>(nameof(FieldKey), null);

    /// <summary>Bindable property for <see cref="Messages"/>.</summary>
    public static readonly BindableProperty MessagesProperty = CreateProperty<IEnumerable<ValidationMessage>?>(nameof(Messages), null);

    /// <summary>Bindable property for <see cref="IsRequired"/>.</summary>
    public static readonly BindableProperty IsRequiredProperty = CreateProperty(nameof(IsRequired), false);

    /// <summary>Spacing between input metadata rows.</summary>
    private const double RowSpacing = 6;

    /// <summary>Visual state rail width.</summary>
    private const double StateRailWidth = 3;

    /// <summary>The semantic color used for secondary field metadata.</summary>
    private const string MutedTextResource = "CrissCrossMutedTextColor";

    /// <summary>The semantic color used for active input and pending validation.</summary>
    private const string AccentResource = "CrissCrossAccentColor";

    /// <summary>Hosts the field header.</summary>
    private ContentView? _header;

    /// <summary>Displays required-field metadata.</summary>
    private Label? _required;

    /// <summary>Displays input guidance.</summary>
    private Label? _helper;

    /// <summary>Displays validation state in text as well as color.</summary>
    private Label? _status;

    /// <summary>Displays the state accent beside the input.</summary>
    private BoxView? _rail;

    /// <summary>Hosts validation messages.</summary>
    private VerticalStackLayout? _messages;

    /// <summary>Initializes a new instance of the <see cref="ReactiveFormField"/> class.</summary>
    public ReactiveFormField() => ControlTemplate = new(CreateTemplate);

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public FormFieldState? FieldState
    {
        get => (FormFieldState?)GetValue(FieldStateProperty);
        set => SetValue(FieldStateProperty, value ?? FormFieldState.Normal);
    }

    /// <summary>Gets or sets the current field validation state.</summary>
    public FormFieldState State
    {
        get => (FormFieldState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets or sets the field header content.</summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>Gets or sets guidance displayed below the input.</summary>
    public string? HelperText
    {
        get => (string?)GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    /// <summary>Gets or sets the stable validation field key.</summary>
    public string? FieldKey
    {
        get => (string?)GetValue(FieldKeyProperty);
        set => SetValue(FieldKeyProperty, value);
    }

    /// <summary>Gets or sets the validation messages displayed for this field.</summary>
    public IEnumerable<ValidationMessage>? Messages
    {
        get => (IEnumerable<ValidationMessage>?)GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether input is required.</summary>
    public bool IsRequired
    {
        get => (bool)GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    /// <summary>Creates a property that refreshes the composed template.</summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="name">The property name.</param>
    /// <param name="defaultValue">The initial value.</param>
    /// <returns>The bindable property.</returns>
    private static BindableProperty CreateProperty<T>(string name, T defaultValue) => BindableProperty.Create(
        name,
        typeof(T),
        typeof(ReactiveFormField),
        defaultValue,
        propertyChanged: static (view, _, _) => ((ReactiveFormField)view).Refresh());

    /// <summary>Creates the input slot and its surrounding metadata.</summary>
    /// <returns>The composed template root.</returns>
    private VerticalStackLayout CreateTemplate()
    {
        _header = new();
        _required = new() { Text = "Required" };
        _helper = new();
        _status = new();
        _rail = new() { WidthRequest = StateRailWidth };
        _messages = new() { Spacing = RowSpacing };
        _required.SetDynamicResource(Label.TextColorProperty, MutedTextResource);
        _helper.SetDynamicResource(Label.TextColorProperty, MutedTextResource);
        var presenter = new ContentPresenter();
        var input = new Grid { ColumnSpacing = RowSpacing, ColumnDefinitions = [new(GridLength.Auto), new(GridLength.Star)], Children = { _rail, presenter } };
        Grid.SetColumn(presenter, 1);
        var layout = new VerticalStackLayout { Spacing = RowSpacing, Children = { _header, _required, input, _helper, _status, _messages } };
        Refresh();
        return layout;
    }

    /// <summary>Keeps both state properties synchronized.</summary>
    /// <param name="state">The current state.</param>
    private void UpdateState(FormFieldState state)
    {
        SetValue(FieldStateProperty, state);
        Refresh();
    }

    /// <summary>Refreshes metadata and accessible validation feedback.</summary>
    private void Refresh()
    {
        if (_header is null || _required is null || _helper is null || _status is null || _rail is null || _messages is null)
        {
            return;
        }

        _header.Content = Header as View ?? new Label { Text = Header?.ToString(), FontAttributes = FontAttributes.Bold };
        _header.IsVisible = Header is not null;
        _required.IsVisible = IsRequired;
        _helper.Text = HelperText;
        _helper.IsVisible = !string.IsNullOrWhiteSpace(HelperText);
        RefreshStatus(_status, _rail);
        RefreshMessages(_messages);
        SemanticProperties.SetDescription(this, $"{FieldKey} {Header} {(IsRequired ? "Required" : string.Empty)} {_status.Text} {HelperText}".Trim());
    }

    /// <summary>Updates the visible validation state.</summary>
    /// <param name="status">The status label.</param>
    /// <param name="rail">The input state rail.</param>
    private void RefreshStatus(Label status, BoxView rail)
    {
        var (text, stateResource) = State switch
        {
            FormFieldState.Focused => ("Focused", AccentResource),
            FormFieldState.Valid => ("Valid", "CrissCrossSuccessColor"),
            FormFieldState.Warning => ("Warning", "CrissCrossCautionColor"),
            FormFieldState.Invalid => ("Invalid", "CrissCrossDangerColor"),
            FormFieldState.Pending => ("Validating…", AccentResource),
            _ => (string.Empty, "CrissCrossBorderColor"),
        };
        status.Text = text;
        status.IsVisible = State != FormFieldState.Normal;
        status.SetDynamicResource(Label.TextColorProperty, stateResource);
        rail.SetDynamicResource(BoxView.ColorProperty, stateResource);
    }

    /// <summary>Updates validation messages from the current snapshot.</summary>
    /// <param name="messages">The message panel.</param>
    private void RefreshMessages(VerticalStackLayout messages)
    {
        messages.Children.Clear();
        foreach (var message in Messages ?? [])
        {
            var label = new Label { Text = message.Message };
            var resource = message.Severity switch
            {
                ValidationSeverity.Error => "CrissCrossDangerColor",
                ValidationSeverity.Warning => "CrissCrossCautionColor",
                ValidationSeverity.Success => "CrissCrossSuccessColor",
                ValidationSeverity.Pending => AccentResource,
                _ => MutedTextResource,
            };
            label.SetDynamicResource(Label.TextColorProperty, resource);
            messages.Children.Add(label);
        }

        messages.IsVisible = messages.Children.Count > 0;
    }
}
