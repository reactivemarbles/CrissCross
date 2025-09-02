// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// AllControlsViewModel.
/// </summary>
/// <seealso cref="CrissCross.RxObject" />
public partial class AllControlsViewModel : RxObject
{
    [ReactiveCommand]
    private void Buttons()
    {
        MainWindow.Navigation?.NavigateTo<ButtonsViewModel>(breadcrumbItemContent: "Buttons");
    }

    [ReactiveCommand]
    private void CheckBox()
    {
        MainWindow.Navigation?.NavigateTo<CheckBoxViewModel>(breadcrumbItemContent: "CheckBox");
    }

    [ReactiveCommand]
    private void ComboBox()
    {
        MainWindow.Navigation?.NavigateTo<ComboBoxViewModel>(breadcrumbItemContent: "ComboBox");
    }

    [ReactiveCommand]
    private void DatePicker()
    {
        MainWindow.Navigation?.NavigateTo<DatePickerViewModel>(breadcrumbItemContent: "DatePicker");
    }

    [ReactiveCommand]
    private void Image()
    {
        MainWindow.Navigation?.NavigateTo<ImageViewModel>(breadcrumbItemContent: "Image");
    }

    [ReactiveCommand]
    private void NumericPushButton()
    {
        MainWindow.Navigation?.NavigateTo<NumericPushButtonViewModel>(breadcrumbItemContent: "NumericPushButton");
    }

    [ReactiveCommand]
    private void PasswordBox()
    {
        MainWindow.Navigation?.NavigateTo<PasswordBoxViewModel>(breadcrumbItemContent: "PasswordBox");
    }

    [ReactiveCommand]
    private void RadioButton()
    {
        MainWindow.Navigation?.NavigateTo<RadioButtonViewModel>(breadcrumbItemContent: "RadioButton");
    }

    [ReactiveCommand]
    private void Slider()
    {
        MainWindow.Navigation?.NavigateTo<SliderViewModel>(breadcrumbItemContent: "Slider");
    }

    [ReactiveCommand]
    private void TextBlock()
    {
        MainWindow.Navigation?.NavigateTo<TextBlockViewModel>(breadcrumbItemContent: "TextBlock");
    }

    [ReactiveCommand]
    private void TextBox()
    {
        MainWindow.Navigation?.NavigateTo<TextBoxViewModel>(breadcrumbItemContent: "TextBox");
    }

    [ReactiveCommand]
    private void ToggleButton()
    {
        MainWindow.Navigation?.NavigateTo<ToggleButtonViewModel>(breadcrumbItemContent: "ToggleButton");
    }

    [ReactiveCommand]
    private void ColorPicker()
    {
        MainWindow.Navigation?.NavigateTo<ColorPickersViewModel>(breadcrumbItemContent: "ColorPicker");
    }
}
