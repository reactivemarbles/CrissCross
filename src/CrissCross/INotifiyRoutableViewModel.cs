// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>INotifiy Routable ViewModel.</summary>
/// <seealso cref="IReactiveObject" />
/// <seealso cref="IUseHostedNavigation" />
public interface INotifiyRoutableViewModel : IReactiveObject, IUseHostedNavigation
{
    /// <summary>Gets the name.</summary>
    /// <value>
    /// The name.
    /// </value>
    new string? Name { get; }

    /// <summary>Raises the <see cref="E:NavigatedFrom"/> event.</summary>
    /// <param name="e">The navigation event data.</param>
    void WhenNavigatedFrom(IViewModelNavigationEventArgs e);

    /// <summary>Raises the <see cref="E:NavigatedTo"/> event.</summary>
    /// <param name="e">The navigation event data.</param>
    /// <param name="disposables">The disposable resources for the active view.</param>
    void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables);

    /// <summary>Raises the <see cref="E:Navigating"/> event.</summary>
    /// <param name="e">The navigating event data.</param>
    void WhenNavigating(IViewModelNavigatingEventArgs e);
}
