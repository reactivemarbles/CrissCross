// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
using ReactiveUI.SourceGenerators;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

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
    /// <summary>Initializes a new instance of the <see cref="RightPropertiesView"/> class.</summary>
    /// <remarks>This constructor configures the view to activate its bindings when displayed, enabling
    /// editing and saving of configuration properties through the user interface. The view's controls are bound to
    /// corresponding properties in the ViewModel, allowing for real-time updates and command execution. This setup is
    /// intended for use within a reactive UI framework and assumes that the ViewModel provides the necessary properties
    /// and commands.</remarks>
    public RightPropertiesView()
    {
        InitializeComponent();

        _ = this.WhenActivated(BindViewModel);
    }

    /// <summary>Binds the view model to the view controls.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindViewModel(CompositeDisposable disposables)
    {
        ViewModel = new();
        DataContext = ViewModel;
        _ = this.BindCommand(ViewModel, vm => vm.SaveConfiguration, v => v.SaveBtn).DisposeWith(disposables);

        _ = this.Bind(ViewModel, vm => vm.ItemName, v => v.textbox1.Text).DisposeWith(disposables);
        _ = this.Bind(ViewModel, vm => vm.LineWidth, v => v.LineWidth.Value).DisposeWith(disposables);
        _ = this.Bind(ViewModel, vm => vm.LineColor, v => v.colorsComboBox.SelectedItem).DisposeWith(disposables);
        _ = this.Bind(ViewModel, vm => vm.ItemVisibility, v => v.visibilityComboBox.SelectedItem)
            .DisposeWith(disposables);
    }
}
