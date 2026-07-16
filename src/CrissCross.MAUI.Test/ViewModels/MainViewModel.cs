// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.MAUI.Test;

/// <summary>MainViewModel member.</summary>
/// <seealso cref="RxObject" />
public class MainViewModel : RxObject
{
    /// <summary>Provides the cached first-view navigation command.</summary>
    private ICommand? _gotoFirst;

    /// <summary>Provides the cached gallery navigation command.</summary>
    private ICommand? _gotoControlsGallery;

    /// <summary>Provides the cached back-navigation command.</summary>
    private ICommand? _gotoMain;

    /// <summary>Gets the goto first.</summary>
    /// <value>
    /// The goto first.
    /// </value>
    public ICommand GotoFirst => _gotoFirst ??= ReactiveCommand.Create(
        () => this.NavigateToView(new NavigationKeyRequest<FirstViewModel>()));

    /// <summary>Gets the goto controls gallery command.</summary>
    /// <value>
    /// The goto controls gallery command.
    /// </value>
    public ICommand GotoControlsGallery => _gotoControlsGallery ??=
        ReactiveCommand.Create(
            () => this.NavigateToView(new NavigationKeyRequest<ControlsGalleryViewModel>()));

    /// <summary>Gets the goto main.</summary>
    /// <value>
    /// The goto main.
    /// </value>
    public ICommand GotoMain => _gotoMain ??= ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack());

    /// <summary>WhenNavigatedTo member.</summary>
    /// <inheritdoc />
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        Debug.WriteLine($"{DateTime.Now} Navigated To: {e.To?.Name} From: {e.From?.Name} with Host {e.HostName}");
        base.WhenNavigatedTo(e, disposables);
    }

    /// <summary>WhenNavigatedFrom member.</summary>
    /// <inheritdoc />
    public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        Debug.WriteLine($"{DateTime.Now} Navigated From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigatedFrom(e);
    }

    /// <summary>WhenNavigating member.</summary>
    /// <inheritdoc />
    public override void WhenNavigating(IViewModelNavigatingEventArgs e)
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        Debug.WriteLine($"{DateTime.Now} Navigating From: {e.From?.Name!} To: {e.To?.Name!} with Host {e.HostName!}");
        base.WhenNavigating(e);
    }
}
