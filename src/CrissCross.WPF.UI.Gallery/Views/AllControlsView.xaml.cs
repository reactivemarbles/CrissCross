// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>
/// Interaction logic for AllControlsView.xaml.
/// </summary>
[IViewFor<AllControlsViewModel>]
public partial class AllControlsView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllControlsView"/> class.
    /// </summary>
    public AllControlsView()
    {
        InitializeComponent();
        DataContext = ViewModel = Locator.Current.GetService<AllControlsViewModel>()!;
        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, vm => vm!.ButtonsCommand, v => v.Buttons).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.CheckBoxCommand, v => v.CheckBox).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.ComboBoxCommand, v => v.ComboBox).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.DatePickerCommand, v => v.DatePicker).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.ImageCommand, v => v.Image).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.NumericPushButtonCommand, v => v.NumericPushButton).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.PasswordBoxCommand, v => v.PasswordBox).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.RadioButtonCommand, v => v.RadioButton).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.SliderCommand, v => v.Slider).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.TextBlockCommand, v => v.TextBlock).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.TextBoxCommand, v => v.TextBox).DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm!.ToggleButtonCommand, v => v.ToggleButton).DisposeWith(disposables);
        });
    }
}
