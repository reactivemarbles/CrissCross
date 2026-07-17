// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI;
using Splat;

#if REACTIVE_SHIM
using ReactiveUI.Reactive;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF;
#else
namespace CrissCross.WPF;
#endif

/// <summary>Navigation Window.</summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="NavigationWindow" />
/// <seealso cref="IViewFor&lt;TViewModel&gt;" />
public class NavigationWindow<TViewModel> : NavigationWindow, IViewFor<TViewModel>
    where TViewModel : class, IRxObject, new()
{
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(TViewModel),
        typeof(NavigationWindow<TViewModel>),
        new PropertyMetadata(null));

    /// <summary>Initializes a new instance of the <see cref="NavigationWindow{TViewModel}"/> class.</summary>
    public NavigationWindow() =>
        this.WhenActivated(
            (CompositeDisposable _) => ViewModel ??= AppLocator.Current.GetService<TViewModel>() ?? new());

    /// <summary>Gets the binding root view model.</summary>
    public TViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }
}
