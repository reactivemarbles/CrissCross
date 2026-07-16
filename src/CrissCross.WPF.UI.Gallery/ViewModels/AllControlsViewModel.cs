// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>AllControlsViewModel provides a searchable, data-driven list of control demos.</summary>
public class AllControlsViewModel : RxObject
{
    /// <summary>The delay applied before refreshing the control filter.</summary>
    private const int FilterThrottleMilliseconds = 150;

    /// <summary>Stores the available control demos.</summary>
    private readonly ObservableCollection<ControlItem> _controls;

    /// <summary>Initializes a new instance of the <see cref="AllControlsViewModel"/> class.</summary>
    public AllControlsViewModel()
    {
        _controls = [];

        AppBarButtonCommand = ReactiveCommand.Create(AppBarButton);
        BBCodeBlockCommand = ReactiveCommand.Create(BBCodeBlock);
        ButtonsCommand = ReactiveCommand.Create(Buttons);
        CheckBoxCommand = ReactiveCommand.Create(CheckBox);
        ComboBoxCommand = ReactiveCommand.Create(ComboBox);
        DatePickerCommand = ReactiveCommand.Create(DatePicker);
        ImageCommand = ReactiveCommand.Create(Image);
        NumericPushButtonCommand = ReactiveCommand.Create(NumericPushButton);
        PasswordBoxCommand = ReactiveCommand.Create(PasswordBox);
        RadioButtonCommand = ReactiveCommand.Create(RadioButton);
        SliderCommand = ReactiveCommand.Create(Slider);
        TextBlockCommand = ReactiveCommand.Create(TextBlock);
        TextBoxCommand = ReactiveCommand.Create(TextBox);
        ToggleButtonCommand = ReactiveCommand.Create(ToggleButton);
        ColorPickerCommand = ReactiveCommand.Create(ColorPicker);

        FilteredControls = CollectionViewSource.GetDefaultView(_controls);
        FilteredControls.Filter = FilterPredicate;

        Populate();

        _ = this.WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(FilterThrottleMilliseconds))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ => FilteredControls.Refresh());
    }

    /// <summary>Gets or sets filter text entered by the user.</summary>
    public string FilterText
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets the collection view over all controls applying the current filter.</summary>
    public ICollectionView FilteredControls { get; }

    /// <summary>Gets the command that navigates to the app bar button demo.</summary>
    public ReactiveCommand<Unit, Unit> AppBarButtonCommand { get; }

    /// <summary>Gets the command that navigates to the BBCode block demo.</summary>
    public ReactiveCommand<Unit, Unit> BBCodeBlockCommand { get; }

    /// <summary>Gets the command that navigates to the button demos.</summary>
    public ReactiveCommand<Unit, Unit> ButtonsCommand { get; }

    /// <summary>Gets the command that navigates to the check box demos.</summary>
    public ReactiveCommand<Unit, Unit> CheckBoxCommand { get; }

    /// <summary>Gets the command that navigates to the combo box demos.</summary>
    public ReactiveCommand<Unit, Unit> ComboBoxCommand { get; }

    /// <summary>Gets the command that navigates to the date picker demos.</summary>
    public ReactiveCommand<Unit, Unit> DatePickerCommand { get; }

    /// <summary>Gets the command that navigates to the image demos.</summary>
    public ReactiveCommand<Unit, Unit> ImageCommand { get; }

    /// <summary>Gets the command that navigates to the numeric input demos.</summary>
    public ReactiveCommand<Unit, Unit> NumericPushButtonCommand { get; }

    /// <summary>Gets the command that navigates to the password box demos.</summary>
    public ReactiveCommand<Unit, Unit> PasswordBoxCommand { get; }

    /// <summary>Gets the command that navigates to the radio button demos.</summary>
    public ReactiveCommand<Unit, Unit> RadioButtonCommand { get; }

    /// <summary>Gets the command that navigates to the slider demos.</summary>
    public ReactiveCommand<Unit, Unit> SliderCommand { get; }

    /// <summary>Gets the command that navigates to the text block demos.</summary>
    public ReactiveCommand<Unit, Unit> TextBlockCommand { get; }

    /// <summary>Gets the command that navigates to the text box demos.</summary>
    public ReactiveCommand<Unit, Unit> TextBoxCommand { get; }

    /// <summary>Gets the command that navigates to the toggle button demos.</summary>
    public ReactiveCommand<Unit, Unit> ToggleButtonCommand { get; }

    /// <summary>Gets the command that navigates to the color picker demos.</summary>
    public ReactiveCommand<Unit, Unit> ColorPickerCommand { get; }

    /// <summary>Populates the control demo collection.</summary>
    private void Populate()
    {
        if (_controls.Count > 0)
        {
            return;
        }

        PopulateCommandControls();
        PopulateInputControls();
        PopulateSelectionControls();
        PopulateTextControls();
    }

    /// <summary>Adds command-oriented controls to the gallery.</summary>
    private void PopulateCommandControls()
    {
        _controls.Add(
            new ControlItem
            {
                Name = "AppBarButton",
                Icon = "/Assets/ControlImages/AppBarButton.png",
                Command = AppBarButtonCommand,
                Description = "Circular command buttons with embedded icons and glyphs.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "BBCodeBlock",
                Icon = "/Assets/ControlImages/RichTextBlock.png",
                Command = BBCodeBlockCommand,
                Description = "Theme-aware BBCode reference and extension rendering.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "Buttons",
                Icon = "/Assets/ControlImages/Button.png",
                Command = ButtonsCommand,
                Description = "Push buttons, repeat buttons and styles.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "CheckBox",
                Icon = "/Assets/ControlImages/CheckBox.png",
                Command = CheckBoxCommand,
                Description = "Standard and tri-state check boxes.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "ComboBox",
                Icon = "/Assets/ControlImages/ComboBox.png",
                Command = ComboBoxCommand,
                Description = "ComboBox / AutoSuggest scenarios.",
            });
    }

    /// <summary>Adds input and media controls to the gallery.</summary>
    private void PopulateInputControls()
    {
        _controls.Add(
            new ControlItem
            {
                Name = "DatePicker",
                Icon = "/Assets/ControlImages/DatePicker.png",
                Command = DatePickerCommand,
                Description = "Date / calendar pickers.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "Image",
                Icon = "/Assets/ControlImages/Image.png",
                Command = ImageCommand,
                Description = "Static, animated and icon imagery.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "Numeric",
                Icon = "/Assets/ControlImages/NumberBox.png",
                Command = NumericPushButtonCommand,
                Description = "Numeric input controls (pads / number box).",
            });
    }

    /// <summary>Adds selection controls to the gallery.</summary>
    private void PopulateSelectionControls()
    {
        _controls.Add(
            new ControlItem
            {
                Name = "PasswordBox",
                Icon = "/Assets/ControlImages/PasswordBox.png",
                Command = PasswordBoxCommand,
                Description = "Password entry field.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "RadioButton",
                Icon = "/Assets/ControlImages/RadioButton.png",
                Command = RadioButtonCommand,
                Description = "Grouped radio button selection.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "Slider",
                Icon = "/Assets/ControlImages/Slider.png",
                Command = SliderCommand,
                Description = "Slider, progress and related indicators.",
            });
    }

    /// <summary>Adds text and color controls to the gallery.</summary>
    private void PopulateTextControls()
    {
        _controls.Add(
            new ControlItem
            {
                Name = "TextBlock",
                Icon = "/Assets/ControlImages/TextBlock.png",
                Command = TextBlockCommand,
                Description = "Static text, formatting examples.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "TextBox",
                Icon = "/Assets/ControlImages/TextBox.png",
                Command = TextBoxCommand,
                Description = "Rich text, multi-line and validation.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "ToggleButton",
                Icon = "/Assets/ControlImages/ToggleButton.png",
                Command = ToggleButtonCommand,
                Description = "Toggle / switch states.",
            });
        _controls.Add(
            new ControlItem
            {
                Name = "ColorPicker",
                Icon = "/Assets/ControlImages/ColorPicker.png",
                Command = ColorPickerCommand,
                Description = "Color selection variants.",
            });
    }

    /// <summary>Filters a control item by the current search text.</summary>
    /// <param name="obj">The item to filter.</param>
    /// <returns>true when the item is visible.</returns>
    private bool FilterPredicate(object obj)
    {
        if (obj is not ControlItem item)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(FilterText)
            ? true
            : item.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase)
                || (item.Description?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    /// <summary>Navigates to the app bar button demo.</summary>
    private void AppBarButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<AppBarButtonViewModel>(), "AppBarButton");

    /// <summary>Navigates to the BBCode block demo.</summary>
    private void BBCodeBlock() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<BBCodeBlockViewModel>(), "BBCodeBlock");

    /// <summary>Navigates to the buttons demo.</summary>
    private void Buttons() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ButtonsViewModel>(), "Buttons");

    /// <summary>Navigates to the check box demo.</summary>
    private void CheckBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<CheckBoxViewModel>(), "CheckBox");

    /// <summary>Navigates to the combo box demo.</summary>
    private void ComboBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ComboBoxViewModel>(), "ComboBox");

    /// <summary>Navigates to the date picker demo.</summary>
    private void DatePicker() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<DatePickerViewModel>(), "DatePicker");

    /// <summary>Navigates to the image demo.</summary>
    private void Image() => MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ImageViewModel>(), "Image");

    /// <summary>Navigates to the numeric push button demo.</summary>
    private void NumericPushButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<NumericPushButtonViewModel>(), "NumericPushButton");

    /// <summary>Navigates to the password box demo.</summary>
    private void PasswordBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<PasswordBoxViewModel>(), "PasswordBox");

    /// <summary>Navigates to the radio button demo.</summary>
    private void RadioButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<RadioButtonViewModel>(), "RadioButton");

    /// <summary>Navigates to the slider demo.</summary>
    private void Slider() => MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<SliderViewModel>(), "Slider");

    /// <summary>Navigates to the text block demo.</summary>
    private void TextBlock() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<TextBlockViewModel>(), "TextBlock");

    /// <summary>Navigates to the text box demo.</summary>
    private void TextBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<TextBoxViewModel>(), "TextBox");

    /// <summary>Navigates to the toggle button demo.</summary>
    private void ToggleButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ToggleButtonViewModel>(), "ToggleButton");

    /// <summary>Navigates to the color picker demo.</summary>
    private void ColorPicker() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ColorPickersViewModel>(), "ColorPicker");
}
