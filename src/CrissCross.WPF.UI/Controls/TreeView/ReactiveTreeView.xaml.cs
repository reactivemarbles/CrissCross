// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Interaction logic for ReactiveTreeView.xaml.</summary>
[IViewFor<ReactiveTreeViewModel>]
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
        _ = this.WhenActivated(d =>
            this.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .Select(vm => vm!.WhenAnyValue(x => x.Children))
                .Switch()
                .SelectMany(children => children.CurrentItems)
                .Subscribe(items => ItemsSource = items)
                .DisposeWith(d));
    }
}
