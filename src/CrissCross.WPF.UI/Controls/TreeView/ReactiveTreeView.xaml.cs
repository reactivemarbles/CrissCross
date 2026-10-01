// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ReactiveUIBindingExtensions;
#else
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Interaction logic for ReactiveTreeView.xaml.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class ReactiveTreeView : IViewFor<ReactiveTreeViewModel>
{
    /// <summary>Identifies the <see cref="ViewModel"/> dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(ReactiveTreeViewModel),
        typeof(ReactiveTreeView),
        new(null));

    ////https://stackoverflow.com/questions/459375/customizing-the-treeview-to-allow-multi-select
    /// <summary>Provides the ReactiveTreeView member.</summary>
    static ReactiveTreeView() =>
        AppLocator.CurrentMutable.Register<IViewFor<ReactiveTreeViewModel>>(static () => new ReactiveTreeView());

    /// <summary>Initializes a new instance of the <see cref="ReactiveTreeView"/> class.</summary>
    public ReactiveTreeView()
    {
        InitializeComponent();
        ViewModel = new();
        BorderThickness = new(0);
        _ = this.WhenActivated(OnActivated);
    }

    /// <summary>Gets or sets the view model for this tree view.</summary>
    public ReactiveTreeViewModel? ViewModel
    {
        get => (ReactiveTreeViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>Gets the binding root view model.</summary>
    public ReactiveTreeViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ReactiveTreeViewModel?)value;
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Connects the tree items to the active view model.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void OnActivated(ActivationDisposable disposables) =>
        this.WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .Select(static vm => vm!.WhenAnyValue(x => x.Children))
            .SwitchTo()
            .SelectMany(static children => children.CurrentItems)
            .Subscribe(items => ItemsSource = items)
            .DisposeWith(disposables);
}
