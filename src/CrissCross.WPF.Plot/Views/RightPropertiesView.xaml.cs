// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
#if REACTIVE_SHIM
using ReactiveUI.Binding.Reactive.Observables;
using BindingDependencyObjectObservableForProperty = ReactiveUI.Binding.Reactive.Wpf.DependencyObjectObservableForProperty;
using LinqExpression = System.Linq.Expressions.Expression;
#else
using ReactiveUI.Binding.Observables;
using BindingDependencyObjectObservableForProperty = ReactiveUI.Binding.Wpf.DependencyObjectObservableForProperty;
using LinqExpression = System.Linq.Expressions.Expression;
#endif
#if !REACTIVE_SHIM
using ReactiveUI;
#endif
using ReactiveUI.SourceGenerators;

#if REACTIVELIST_REACTIVE
using NumberBox = CrissCross.Reactive.WPF.UI.Controls.NumberBox;
namespace CrissCross.Reactive.WPF.Plot;
#else
using NumberBox = CrissCross.WPF.UI.Controls.NumberBox;
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

    /// <summary>Gets the item name text box.</summary>
    public TextBox ItemNameTextBox => textbox1;

    /// <summary>Gets the line-width number box.</summary>
    public NumberBox LineWidthNumberBox => LineWidth;

    /// <summary>Gets the color combo box.</summary>
    public ComboBox ColorsComboBox => colorsComboBox;

    /// <summary>Gets the visibility combo box.</summary>
    public ComboBox VisibilityComboBox => visibilityComboBox;

    /// <summary>Observes a WPF dependency property and invokes an update when the value changes.</summary>
    /// <param name="target">The dependency object to observe.</param>
    /// <param name="propertyName">The dependency property name.</param>
    /// <param name="update">The update action to run when the dependency property changes.</param>
    /// <returns>A disposable dependency-property observation subscription.</returns>
    private static IDisposable ObserveDependencyProperty(DependencyObject target, string propertyName, Action update)
    {
        BindingDependencyObjectObservableForProperty factory = new();
        return factory
            .GetNotificationForProperty(target, LinqExpression.Constant(target), propertyName, false, false)
            .Subscribe(_ => update());
    }

    /// <summary>Binds the view model to the view controls.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void BindViewModel(ActivationDisposable disposables)
    {
        ViewModel = new();
        DataContext = ViewModel;
        _ = this.BindCommand(ViewModel, vm => vm.SaveConfiguration, v => v.SaveBtn).DisposeWith(disposables);

        _ = new PropertyObservable<string?>(
                ViewModel,
                nameof(RightPropertiesViewModel.ItemName),
                static source => ((RightPropertiesViewModel)source).ItemName,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(value => ItemNameTextBox.Text = value)
            .DisposeWith(disposables);
        _ = ObserveDependencyProperty(ItemNameTextBox, nameof(TextBox.Text), () => ViewModel.ItemName = ItemNameTextBox.Text)
            .DisposeWith(disposables);

        _ = new PropertyObservable<double>(
                ViewModel,
                nameof(RightPropertiesViewModel.LineWidth),
                static source => ((RightPropertiesViewModel)source).LineWidth,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(value => LineWidthNumberBox.Value = value)
            .DisposeWith(disposables);
        _ = ObserveDependencyProperty(
                LineWidthNumberBox,
                nameof(NumberBox.Value),
                () => ViewModel.LineWidth = LineWidthNumberBox.Value ?? 0D)
            .DisposeWith(disposables);

        _ = new PropertyObservable<string?>(
                ViewModel,
                nameof(RightPropertiesViewModel.LineColor),
                static source => ((RightPropertiesViewModel)source).LineColor,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(value => ColorsComboBox.SelectedItem = value)
            .DisposeWith(disposables);
        _ = ObserveDependencyProperty(
                ColorsComboBox,
                nameof(ComboBox.SelectedItem),
                () => ViewModel.LineColor = ColorsComboBox.SelectedItem as string)
            .DisposeWith(disposables);

        _ = new PropertyObservable<string?>(
                ViewModel,
                nameof(RightPropertiesViewModel.ItemVisibility),
                static source => ((RightPropertiesViewModel)source).ItemVisibility,
                true)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(value => VisibilityComboBox.SelectedItem = value)
            .DisposeWith(disposables);
        _ = ObserveDependencyProperty(
                VisibilityComboBox,
                nameof(ComboBox.SelectedItem),
                () => ViewModel.ItemVisibility = VisibilityComboBox.SelectedItem as string)
            .DisposeWith(disposables);
    }
}
