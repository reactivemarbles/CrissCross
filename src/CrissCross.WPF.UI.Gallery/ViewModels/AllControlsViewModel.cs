// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
        ControlCatalogCommand = ReactiveCommand.Create(ControlCatalog);

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

    /// <summary>Gets the command that navigates to the curated control catalog.</summary>
    public ReactiveCommand<Unit, Unit> ControlCatalogCommand { get; }

    /// <summary>Navigates to the app bar button demo.</summary>
    private static void AppBarButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<AppBarButtonViewModel>(), "AppBarButton");

    /// <summary>Navigates to the BBCode block demo.</summary>
    private static void BBCodeBlock() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<BBCodeBlockViewModel>(), "BBCodeBlock");

    /// <summary>Navigates to the buttons demo.</summary>
    private static void Buttons() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ButtonsViewModel>(), "Buttons");

    /// <summary>Navigates to the check box demo.</summary>
    private static void CheckBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<CheckBoxViewModel>(), "CheckBox");

    /// <summary>Navigates to the combo box demo.</summary>
    private static void ComboBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ComboBoxViewModel>(), "ComboBox");

    /// <summary>Navigates to the date picker demo.</summary>
    private static void DatePicker() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<DatePickerViewModel>(), "DatePicker");

    /// <summary>Navigates to the image demo.</summary>
    private static void Image() => MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ImageViewModel>(), "Image");

    /// <summary>Navigates to the numeric push button demo.</summary>
    private static void NumericPushButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<NumericPushButtonViewModel>(), "NumericPushButton");

    /// <summary>Navigates to the password box demo.</summary>
    private static void PasswordBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<PasswordBoxViewModel>(), "PasswordBox");

    /// <summary>Navigates to the radio button demo.</summary>
    private static void RadioButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<RadioButtonViewModel>(), "RadioButton");

    /// <summary>Navigates to the slider demo.</summary>
    private static void Slider() => MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<SliderViewModel>(), "Slider");

    /// <summary>Navigates to the text block demo.</summary>
    private static void TextBlock() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<TextBlockViewModel>(), "TextBlock");

    /// <summary>Navigates to the text box demo.</summary>
    private static void TextBox() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<TextBoxViewModel>(), "TextBox");

    /// <summary>Navigates to the toggle button demo.</summary>
    private static void ToggleButton() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ToggleButtonViewModel>(), "ToggleButton");

    /// <summary>Navigates to the color picker demo.</summary>
    private static void ColorPicker() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ColorPickersViewModel>(), "ColorPicker");

    /// <summary>Navigates to the curated WPF control catalog.</summary>
    private static void ControlCatalog() =>
        MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<ControlCatalogViewModel>(), "ControlCatalog");

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
        PopulateCatalogControls();
    }

    /// <summary>Adds command-oriented controls to the gallery.</summary>
    private void PopulateCommandControls()
    {
        AddControl(nameof(AppBarButton), "/Assets/ControlImages/AppBarButton.png", AppBarButtonCommand, "Circular command buttons with embedded icons and glyphs.");
        AddControl(nameof(BBCodeBlock), "/Assets/ControlImages/RichTextBlock.png", BBCodeBlockCommand, "Theme-aware BBCode reference and extension rendering.");
        AddControl(nameof(Buttons), "/Assets/ControlImages/Button.png", ButtonsCommand, "Push buttons, repeat buttons and styles.");
        AddControl(nameof(CheckBox), "/Assets/ControlImages/CheckBox.png", CheckBoxCommand, "Standard and tri-state check boxes.");
        AddControl(nameof(ComboBox), "/Assets/ControlImages/ComboBox.png", ComboBoxCommand, "ComboBox / AutoSuggest scenarios.");
    }

    /// <summary>Adds input and media controls to the gallery.</summary>
    private void PopulateInputControls()
    {
        AddControl(nameof(DatePicker), "/Assets/ControlImages/DatePicker.png", DatePickerCommand, "Date / calendar pickers.");
        AddControl(nameof(Image), "/Assets/ControlImages/Image.png", ImageCommand, "Static, animated and icon imagery.");
        AddControl("Numeric", "/Assets/ControlImages/NumberBox.png", NumericPushButtonCommand, "Numeric input controls (pads / number box).");
    }

    /// <summary>Adds selection controls to the gallery.</summary>
    private void PopulateSelectionControls()
    {
        AddControl(nameof(PasswordBox), "/Assets/ControlImages/PasswordBox.png", PasswordBoxCommand, "Password entry field.");
        AddControl(nameof(RadioButton), "/Assets/ControlImages/RadioButton.png", RadioButtonCommand, "Grouped radio button selection.");
        AddControl(nameof(Slider), "/Assets/ControlImages/Slider.png", SliderCommand, "Slider, progress and related indicators.");
    }

    /// <summary>Adds text and color controls to the gallery.</summary>
    private void PopulateTextControls()
    {
        AddControl(nameof(TextBlock), "/Assets/ControlImages/TextBlock.png", TextBlockCommand, "Static text, formatting examples.");
        AddControl(nameof(TextBox), "/Assets/ControlImages/TextBox.png", TextBoxCommand, "Rich text, multi-line and validation.");
        AddControl(nameof(ToggleButton), "/Assets/ControlImages/ToggleButton.png", ToggleButtonCommand, "Toggle / switch states.");
        AddControl(nameof(ColorPicker), "/Assets/ControlImages/ColorPicker.png", ColorPickerCommand, "Color selection variants.");
    }

    /// <summary>Adds the catalog that covers the remaining constructible WPF controls.</summary>
    private void PopulateCatalogControls() =>
        AddControl("Control catalog", "/Assets/ControlImages/AppBarButton.png", ControlCatalogCommand, "A themed, interactive catalog of the remaining WPF UI controls.");

    /// <summary>Adds a control item to the gallery collection.</summary>
    /// <param name="name">The control display name.</param>
    /// <param name="icon">The control icon path.</param>
    /// <param name="command">The navigation command.</param>
    /// <param name="description">The control description.</param>
    private void AddControl(string name, string icon, System.Windows.Input.ICommand command, string description) =>
        _controls.Add(new() { Name = name, Icon = icon, Command = command, Description = description });

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
}
