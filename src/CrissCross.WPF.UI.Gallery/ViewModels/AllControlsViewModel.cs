// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Data;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// AllControlsViewModel provides a searchable, data-driven list of control demos.
/// </summary>
public partial class AllControlsViewModel : RxObject
{
    private readonly ObservableCollection<ControlItem> _controls;
    private string _filterText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllControlsViewModel"/> class.
    /// </summary>
    public AllControlsViewModel()
    {
        _controls = [];

        this.WhenAnyValue(vm => vm.ButtonsCommand)
            .Take(1)
            .Subscribe(_ => Populate());

        FilteredControls = CollectionViewSource.GetDefaultView(_controls);
        FilteredControls.Filter = FilterPredicate;

        this.WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(150))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ => FilteredControls.Refresh());
    }

    /// <summary>
    /// Gets or sets filter text entered by the user.
    /// </summary>
    public string FilterText
    {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    /// <summary>
    /// Gets the collection view over all controls applying the current filter.
    /// </summary>
    public ICollectionView FilteredControls { get; }

    private void Populate()
    {
        if (_controls.Count > 0)
        {
            return;
        }

        _controls.Add(new ControlItem { Name = "Buttons", Icon = "/Assets/ControlImages/Button.png", Command = ButtonsCommand, Description = "Push buttons, repeat buttons and styles." });
        _controls.Add(new ControlItem { Name = "CheckBox", Icon = "/Assets/ControlImages/CheckBox.png", Command = CheckBoxCommand, Description = "Standard and tri-state check boxes." });
        _controls.Add(new ControlItem { Name = "ComboBox", Icon = "/Assets/ControlImages/ComboBox.png", Command = ComboBoxCommand, Description = "ComboBox / AutoSuggest scenarios." });
        _controls.Add(new ControlItem { Name = "DatePicker", Icon = "/Assets/ControlImages/DatePicker.png", Command = DatePickerCommand, Description = "Date / calendar pickers." });
        _controls.Add(new ControlItem { Name = "Image", Icon = "/Assets/ControlImages/Image.png", Command = ImageCommand, Description = "Static, animated and icon imagery." });
        _controls.Add(new ControlItem { Name = "Numeric", Icon = "/Assets/ControlImages/NumberBox.png", Command = NumericPushButtonCommand, Description = "Numeric input controls (pads / number box)." });
        _controls.Add(new ControlItem { Name = "PasswordBox", Icon = "/Assets/ControlImages/PasswordBox.png", Command = PasswordBoxCommand, Description = "Password entry field." });
        _controls.Add(new ControlItem { Name = "RadioButton", Icon = "/Assets/ControlImages/RadioButton.png", Command = RadioButtonCommand, Description = "Grouped radio button selection." });
        _controls.Add(new ControlItem { Name = "Slider", Icon = "/Assets/ControlImages/Slider.png", Command = SliderCommand, Description = "Slider, progress and related indicators." });
        _controls.Add(new ControlItem { Name = "TextBlock", Icon = "/Assets/ControlImages/TextBlock.png", Command = TextBlockCommand, Description = "Static text, formatting examples." });
        _controls.Add(new ControlItem { Name = "TextBox", Icon = "/Assets/ControlImages/TextBox.png", Command = TextBoxCommand, Description = "Rich text, multi-line and validation." });
        _controls.Add(new ControlItem { Name = "ToggleButton", Icon = "/Assets/ControlImages/ToggleButton.png", Command = ToggleButtonCommand, Description = "Toggle / switch states." });
        _controls.Add(new ControlItem { Name = "ColorPicker", Icon = "/Assets/ControlImages/ColorPicker.png", Command = ColorPickerCommand, Description = "Color selection variants." });
    }

    private bool FilterPredicate(object obj)
    {
        if (obj is not ControlItem item)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(FilterText))
        {
            return true;
        }

        return item.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
               (item.Description?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    // Navigation Commands (ReactiveUI Source Generator)
    [ReactiveCommand]
    private void Buttons() => MainWindow.Navigation?.NavigateTo<ButtonsViewModel>(breadcrumbItemContent: "Buttons");

    [ReactiveCommand]
    private void CheckBox() => MainWindow.Navigation?.NavigateTo<CheckBoxViewModel>(breadcrumbItemContent: "CheckBox");

    [ReactiveCommand]
    private void ComboBox() => MainWindow.Navigation?.NavigateTo<ComboBoxViewModel>(breadcrumbItemContent: "ComboBox");

    [ReactiveCommand]
    private void DatePicker() => MainWindow.Navigation?.NavigateTo<DatePickerViewModel>(breadcrumbItemContent: "DatePicker");

    [ReactiveCommand]
    private void Image() => MainWindow.Navigation?.NavigateTo<ImageViewModel>(breadcrumbItemContent: "Image");

    [ReactiveCommand]
    private void NumericPushButton() => MainWindow.Navigation?.NavigateTo<NumericPushButtonViewModel>(breadcrumbItemContent: "NumericPushButton");

    [ReactiveCommand]
    private void PasswordBox() => MainWindow.Navigation?.NavigateTo<PasswordBoxViewModel>(breadcrumbItemContent: "PasswordBox");

    [ReactiveCommand]
    private void RadioButton() => MainWindow.Navigation?.NavigateTo<RadioButtonViewModel>(breadcrumbItemContent: "RadioButton");

    [ReactiveCommand]
    private void Slider() => MainWindow.Navigation?.NavigateTo<SliderViewModel>(breadcrumbItemContent: "Slider");

    [ReactiveCommand]
    private void TextBlock() => MainWindow.Navigation?.NavigateTo<TextBlockViewModel>(breadcrumbItemContent: "TextBlock");

    [ReactiveCommand]
    private void TextBox() => MainWindow.Navigation?.NavigateTo<TextBoxViewModel>(breadcrumbItemContent: "TextBox");

    [ReactiveCommand]
    private void ToggleButton() => MainWindow.Navigation?.NavigateTo<ToggleButtonViewModel>(breadcrumbItemContent: "ToggleButton");

    [ReactiveCommand]
    private void ColorPicker() => MainWindow.Navigation?.NavigateTo<ColorPickersViewModel>(breadcrumbItemContent: "ColorPicker");
}
