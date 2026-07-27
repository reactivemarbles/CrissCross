// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.SourceGenerators;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Interaction logic for ReactiveTreeView.xaml.</summary>
[IViewFor<ReactiveTreeViewModel>]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class ReactiveTreeView
{
    ////https://stackoverflow.com/questions/459375/customizing-the-treeview-to-allow-multi-select
    /// <summary>Provides the ReactiveTreeView member.</summary>
    static ReactiveTreeView() =>
        Splat.AppLocator.CurrentMutable.Register<IViewFor<ReactiveTreeViewModel>>(static () => new ReactiveTreeView());

    /// <summary>Initializes a new instance of the <see cref="ReactiveTreeView"/> class.</summary>
    public ReactiveTreeView()
    {
        InitializeComponent();
        ViewModel = new();
        BorderThickness = new(0);
        _ = this.WhenActivated(OnActivated);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Connects the tree items to the active view model.</summary>
    /// <param name="disposables">The activation disposables.</param>
    private void OnActivated(CompositeDisposable disposables) =>
        this.WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .Select(static vm => vm!.WhenAnyValue(x => x.Children))
            .Switch()
            .SelectMany(static children => children.CurrentItems)
            .Subscribe(items => ItemsSource = items)
            .DisposeWith(disposables);
}
