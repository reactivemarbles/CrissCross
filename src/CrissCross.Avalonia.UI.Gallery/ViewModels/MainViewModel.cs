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
    private const string HostName = "mainNavHost";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel() =>
        this.BuildComplete(() =>
        {
            DisplayName = "Gallery";

            // Navigation commands for each control category - use IUseHostedNavigation with explicit host name
            GotoHome = ReactiveCommand.Create(() => this.NavigateToView<HomePageViewModel>(HostName));
            GotoButtons = ReactiveCommand.Create(() => this.NavigateToView<ButtonsPageViewModel>(HostName));
            GotoInput = ReactiveCommand.Create(() => this.NavigateToView<InputPageViewModel>(HostName));
            GotoProgress = ReactiveCommand.Create(() => this.NavigateToView<ProgressPageViewModel>(HostName));
            GotoCheckBox = ReactiveCommand.Create(() => this.NavigateToView<CheckBoxPageViewModel>(HostName));
            GotoRadioButton = ReactiveCommand.Create(() => this.NavigateToView<RadioButtonPageViewModel>(HostName));
            GotoComboBox = ReactiveCommand.Create(() => this.NavigateToView<ComboBoxPageViewModel>(HostName));
            GotoSlider = ReactiveCommand.Create(() => this.NavigateToView<SliderPageViewModel>(HostName));
            GotoDatePicker = ReactiveCommand.Create(() => this.NavigateToView<DatePickerPageViewModel>(HostName));
            GotoColorPicker = ReactiveCommand.Create(() => this.NavigateToView<ColorPickerPageViewModel>(HostName));
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
