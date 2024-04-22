// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Interaction logic for ReactiveTreeView.xaml.
/// </summary>
public partial class ReactiveTreeView : IViewFor<ReactiveTreeViewModel>
{
    ////https://stackoverflow.com/questions/459375/customizing-the-treeview-to-allow-multi-select

    /// <summary>
    /// The view model property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(ReactiveTreeViewModel),
        typeof(ReactiveTreeView),
        new PropertyMetadata(default(ReactiveTreeViewModel)));

    static ReactiveTreeView() =>
        Splat.Locator.CurrentMutable.Register(() => new ReactiveTreeView(), typeof(IViewFor<ReactiveTreeViewModel>));

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTreeView"/> class.
    /// </summary>
    public ReactiveTreeView()
    {
        InitializeComponent();
        ViewModel = new();
        BorderThickness = new Thickness(0);
        this.WhenActivated(d => ViewModel?.Children.CurrentItems.Subscribe(x => ItemsSource = x).DisposeWith(d));
    }

    /// <summary>
    /// Gets or sets the ViewModel corresponding to this specific View. This should be
    /// a DependencyProperty if you're using XAML.
    /// </summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ReactiveTreeViewModel?)value;
    }

    /// <summary>
    /// Gets or sets the ViewModel corresponding to this specific View. This should be
    /// a DependencyProperty if you're using XAML.
    /// </summary>
    public ReactiveTreeViewModel? ViewModel
    {
        get => (ReactiveTreeViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
