// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
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
        this.NavigateToView<ButtonsViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void CheckBox()
    {
        this.NavigateToView<CheckBoxViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void ComboBox()
    {
        this.NavigateToView<ComboBoxViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void DatePicker()
    {
        this.NavigateToView<DatePickerViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void Image()
    {
        this.NavigateToView<ImageViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void NumericPushButton()
    {
        this.NavigateToView<NumericPushButtonViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void PasswordBox()
    {
        this.NavigateToView<PasswordBoxViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void RadioButton()
    {
        this.NavigateToView<RadioButtonViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void Slider()
    {
        this.NavigateToView<SliderViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void TextBlock()
    {
        this.NavigateToView<TextBlockViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void TextBox()
    {
        this.NavigateToView<TextBoxViewModel>("mainWindow");
    }

    [ReactiveCommand]
    private void ToggleButton()
    {
        this.NavigateToView<ToggleButtonViewModel>("mainWindow");
    }
}
