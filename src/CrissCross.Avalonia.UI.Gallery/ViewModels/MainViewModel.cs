// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>
/// Main view model for the gallery application.
/// </summary>
public class MainViewModel : RxObject, IUseHostedNavigation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel() =>
        this.BuildComplete(() =>
        {
            DisplayName = "Gallery";

            // Navigation commands for each control category.
            GotoHome = ReactiveCommand.Create(() => this.NavigateToView<HomePageViewModel>());
            GotoButtons = ReactiveCommand.Create(() => this.NavigateToView<ButtonsPageViewModel>());
            GotoInput = ReactiveCommand.Create(() => this.NavigateToView<InputPageViewModel>());
            GotoProgress = ReactiveCommand.Create(() => this.NavigateToView<ProgressPageViewModel>());
            GotoCheckBox = ReactiveCommand.Create(() => this.NavigateToView<CheckBoxPageViewModel>());
            GotoRadioButton = ReactiveCommand.Create(() => this.NavigateToView<RadioButtonPageViewModel>());
            GotoComboBox = ReactiveCommand.Create(() => this.NavigateToView<ComboBoxPageViewModel>());
            GotoSlider = ReactiveCommand.Create(() => this.NavigateToView<SliderPageViewModel>());
            GotoDatePicker = ReactiveCommand.Create(() => this.NavigateToView<DatePickerPageViewModel>());
            GotoColorPicker = ReactiveCommand.Create(() => this.NavigateToView<ColorPickerPageViewModel>());
        });

    /// <summary>
    /// Gets the goto home command.
    /// </summary>
    public ICommand? GotoHome { get; private set; }

    /// <summary>
    /// Gets the goto buttons command.
    /// </summary>
    public ICommand? GotoButtons { get; private set; }

    /// <summary>
    /// Gets the goto input command.
    /// </summary>
    public ICommand? GotoInput { get; private set; }

    /// <summary>
    /// Gets the goto progress command.
    /// </summary>
    public ICommand? GotoProgress { get; private set; }

    /// <summary>
    /// Gets the goto checkbox command.
    /// </summary>
    public ICommand? GotoCheckBox { get; private set; }

    /// <summary>
    /// Gets the goto radiobutton command.
    /// </summary>
    public ICommand? GotoRadioButton { get; private set; }

    /// <summary>
    /// Gets the goto combobox command.
    /// </summary>
    public ICommand? GotoComboBox { get; private set; }

    /// <summary>
    /// Gets the goto slider command.
    /// </summary>
    public ICommand? GotoSlider { get; private set; }

    /// <summary>
    /// Gets the goto datepicker command.
    /// </summary>
    public ICommand? GotoDatePicker { get; private set; }

    /// <summary>
    /// Gets the goto colorpicker command.
    /// </summary>
    public ICommand? GotoColorPicker { get; private set; }
}
