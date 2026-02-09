// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents the view component for editing and displaying properties in the right panel of the application,
/// supporting data binding to a <see cref="RightPropertiesViewModel"/>.
/// </summary>
/// <remarks>This view is intended for use on Windows 10 version 19041 or later. It utilizes ReactiveUI for data
/// binding and command handling, enabling interactive editing of item properties such as name, line width, color, and
/// visibility. The view automatically initializes its data context and binds UI controls to corresponding view model
/// properties when activated.</remarks>
[IViewFor<RightPropertiesViewModel>]
[SupportedOSPlatform("windows")]
public partial class RightPropertiesView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RightPropertiesView"/> class and sets up data bindings between the view and its.
    /// associated ViewModel.
    /// </summary>
    /// <remarks>This constructor configures the view to activate its bindings when displayed, enabling
    /// editing and saving of configuration properties through the user interface. The view's controls are bound to
    /// corresponding properties in the ViewModel, allowing for real-time updates and command execution. This setup is
    /// intended for use within a reactive UI framework and assumes that the ViewModel provides the necessary properties
    /// and commands.</remarks>
    public RightPropertiesView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            DataContext = ViewModel = new();
            this.BindCommand(ViewModel, vm => vm.SaveConfiguration, v => v.SaveBtn).DisposeWith(d);

            // Bind form fields to ViewModel properties for editing
            this.Bind(ViewModel, vm => vm.ItemName, v => v.textbox1.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.LineWidth, v => v.LineWidth.Value).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.LineColor, v => v.colorsComboBox.SelectedItem).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.ItemVisibility, v => v.visibilityComboBox.SelectedItem).DisposeWith(d);
        });
    }
}
