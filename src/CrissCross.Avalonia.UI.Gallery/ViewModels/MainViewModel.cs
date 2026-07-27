// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>Main view model for the gallery application.</summary>
public class MainViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="MainViewModel"/> class.</summary>
    public MainViewModel() => DisplayName = "Gallery";

    /// <summary>Gets the goto home command.</summary>
    public ICommand GotoHome => field ??= CreateNavigationCommand<HomePageViewModel>();

    /// <summary>Gets the goto buttons command.</summary>
    public ICommand GotoButtons => field ??= CreateNavigationCommand<ButtonsPageViewModel>();

    /// <summary>Gets the goto input command.</summary>
    public ICommand GotoInput => field ??= CreateNavigationCommand<InputPageViewModel>();

    /// <summary>Gets the goto progress command.</summary>
    public ICommand GotoProgress => field ??= CreateNavigationCommand<ProgressPageViewModel>();

    /// <summary>Gets the goto checkbox command.</summary>
    public ICommand GotoCheckBox => field ??= CreateNavigationCommand<CheckBoxPageViewModel>();

    /// <summary>Gets the goto radiobutton command.</summary>
    public ICommand GotoRadioButton => field ??= CreateNavigationCommand<RadioButtonPageViewModel>();

    /// <summary>Gets the goto combobox command.</summary>
    public ICommand GotoComboBox => field ??= CreateNavigationCommand<ComboBoxPageViewModel>();

    /// <summary>Gets the goto slider command.</summary>
    public ICommand GotoSlider => field ??= CreateNavigationCommand<SliderPageViewModel>();

    /// <summary>Gets the goto datepicker command.</summary>
    public ICommand GotoDatePicker => field ??= CreateNavigationCommand<DatePickerPageViewModel>();

    /// <summary>Gets the goto colorpicker command.</summary>
    public ICommand GotoColorPicker => field ??= CreateNavigationCommand<ColorPickerPageViewModel>();

    /// <summary>Gets the BBCodeBlock gallery navigation command.</summary>
    public ICommand GotoBBCodeBlock => field ??= CreateNavigationCommand<BBCodeBlockPageViewModel>();

    /// <summary>Gets the goto feature playground command.</summary>
    public ICommand GotoFeaturePlayground => field ??= CreateNavigationCommand<FeaturePlaygroundPageViewModel>();

    /// <summary>Gets the workflow and feedback gallery navigation command.</summary>
    public ICommand GotoWorkflow => field ??= CreateNavigationCommand<WorkflowPageViewModel>();

    /// <summary>Gets the control and host coverage catalog navigation command.</summary>
    public ICommand GotoControlCatalog => field ??= CreateNavigationCommand<ControlCatalogPageViewModel>();

    /// <summary>Creates a command that navigates to a registered view model.</summary>
    /// <typeparam name="TViewModel">The destination view model type.</typeparam>
    /// <returns>The navigation command.</returns>
    private ReactiveCommand<Unit, Unit> CreateNavigationCommand<TViewModel>()
        where TViewModel : class, IRxObject =>
        ReactiveCommand.Create(() => this.NavigateToView(new NavigationKeyRequest<TViewModel>()));
}
