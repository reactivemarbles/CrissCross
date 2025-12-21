// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Interaction logic for MainView.xaml.
/// </summary>
[IViewFor<RightPropertiesV2ViewModel>]
[SupportedOSPlatform("windows10.0.19041")]
public partial class RightPropertiesV2View
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RightPropertiesV2View"/> class.
    /// </summary>
    public RightPropertiesV2View()
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
