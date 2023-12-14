// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using ReactiveUI;

[assembly: InternalsVisibleTo(" CrissCross.WPF")]
[assembly: InternalsVisibleTo(" CrissCross.XamForms")]
[assembly: InternalsVisibleTo(" CrissCross.MAUI")]
[assembly: InternalsVisibleTo(" CrissCross.WinForms")]
[assembly: InternalsVisibleTo(" CrissCross.Avalonia")]

namespace CrissCross;

#pragma warning disable RCS1175 // Unused 'this' parameter.

/// <summary>
/// View Model Routed View Host Mixins.
/// </summary>
public static class ViewModelRoutedViewHostMixins
{
    internal static ReplaySubject<Unit> ASetupCompleted { get; } = new(1);

    internal static Dictionary<string, CompositeDisposable> CurrentViewDisposable { get; } = new();

    internal static Dictionary<string, IViewModelRoutedViewHost> NavigationHost { get; } = new();

    internal static Dictionary<string, Subject<IViewModelNavigatingEventArgs>> ResultNavigating { get; } = new();

    internal static Subject<IViewModelNavigationEventArgs> SetWhenNavigated { get; } = new();

    internal static Subject<IViewModelNavigatingEventArgs> SetWhenNavigating { get; } = new();

    internal static Dictionary<string, ReplaySubject<bool>> WhenSetupSubjects { get; } = new();

    /// <summary>
    /// Determines whether this instance [can navigate back] the specified this.
    /// </summary>
    /// <param name="this">The this.</param>
    /// <returns>A bool.</returns>
    public static IObservable<bool> CanNavigateBack(this IUseNavigation @this)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return Observable.Create<bool>(obs =>
        {
            var dis = new CompositeDisposable();
            @this.WhenSetup().Subscribe(_ =>
            {
                if (NavigationHost.Count > 0 && @this.Name != null)
                {
                    if (@this.Name.Length == 0)
                    {
                        NavigationHost.First().Value.CanNavigateBackObservable
                        .DistinctUntilChanged()
                        .Subscribe(x => obs.OnNext(x == true))
                        .DisposeWith(dis);
                    }

                    if (NavigationHost.TryGetValue(@this.Name, out var value))
                    {
                        value.CanNavigateBackObservable
                        .DistinctUntilChanged()
                        .Subscribe(x => obs.OnNext(x == true))
                        .DisposeWith(dis);
                    }
                }
            });

            obs.OnNext(false);

            return dis;
        });
    }

    /// <summary>
    /// Determines whether this instance [can navigate back] the specified host name.
    /// </summary>
    /// <param name="this">The navigation host.</param>
    /// <param name="hostName">Name of the host.</param>
    /// <returns>
    /// A bool.
    /// </returns>
    public static IObservable<bool> CanNavigateBack(this IUseHostedNavigation @this, string hostName = "")
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return Observable.Create<bool>(obs =>
         {
             var dis = new CompositeDisposable();
             @this.WhenSetup(hostName).Subscribe(_ =>
             {
                 if (NavigationHost.Count > 0 && hostName != null)
                 {
                     if (hostName.Length == 0)
                     {
                         NavigationHost.First().Value.CanNavigateBackObservable
                         .DistinctUntilChanged()
                         .Subscribe(x => obs.OnNext(x == true))
                         .DisposeWith(dis);
                     }

                     if (NavigationHost.TryGetValue(hostName, out var value))
                     {
                         value.CanNavigateBackObservable
                         .DistinctUntilChanged()
                         .Subscribe(x => obs.OnNext(x == true))
                         .DisposeWith(dis);
                     }
                 }
             }).DisposeWith(dis);

             obs.OnNext(false);

             return dis;
         });
    }

    /// <summary>
    /// Clears the history.
    /// </summary>
    /// <param name="this">The dummy.</param>
    public static void ClearHistory(this IUseNavigation @this)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && @this.Name != null)
        {
            switch (@this.Name.Length)
            {
                case 0:
                    NavigationHost.First().Value.ClearHistory();
                    break;
                default:
                    NavigationHost[@this.Name].ClearHistory();
                    break;
            }
        }
    }

    /// <summary>
    /// Clears the history.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="hostName">Name of the host.</param>
    public static void ClearHistory(this IUseHostedNavigation dummy, string hostName = "")
    {
        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && hostName != null)
        {
            switch (hostName.Length)
            {
                case 0:
                    NavigationHost.First().Value.ClearHistory();
                    break;
                default:
                    NavigationHost[hostName].ClearHistory();
                    break;
            }
        }
    }

    /// <summary>
    /// Navigates the back.
    /// </summary>
    /// <param name="this">The this.</param>
    /// <param name="parameter">The parameter.</param>
    public static void NavigateBack(this IUseNavigation @this, object? parameter = null)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && @this.Name != null)
        {
            switch (@this.Name.Length)
            {
                case 0:
                    NavigationHost.First().Value.NavigateBack(parameter);
                    break;
                default:
                    if (NavigationHost.TryGetValue(@this.Name, out var value))
                    {
                        value.NavigateBack(parameter);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Navigates backwards.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="hostName">Name of the host.</param>
    /// <param name="parameter">The parameter.</param>
    public static void NavigateBack(this IUseHostedNavigation dummy, string hostName = "", object? parameter = null)
    {
        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && hostName != null)
        {
            switch (hostName.Length)
            {
                case 0:
                    NavigationHost.First().Value.NavigateBack(parameter);
                    break;
                default:
                    if (NavigationHost.TryGetValue(hostName, out var value))
                    {
                        value.NavigateBack(parameter);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Navigates the specified contract.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="this">The this.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public static void NavigateToView<T>(this IUseNavigation @this, string? contract = null, object? parameter = null)
        where T : class, IRxObject
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && @this.Name != null)
        {
            switch (@this.Name.Length)
            {
                case 0:
                    NavigationHost.First().Value.Navigate<T>(contract, parameter);
                    break;
                default:
                    NavigationHost[@this.Name].Navigate<T>(contract, parameter);
                    break;
            }
        }
    }

    /// <summary>
    /// Navigates to view.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="dummy">The dummy.</param>
    /// <param name="hostName">Name of the host.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public static void NavigateToView<T>(this IUseHostedNavigation dummy, string? hostName = "", string? contract = null, object? parameter = null)
        where T : class, IRxObject
    {
        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && hostName != null)
        {
            switch (hostName.Length)
            {
                case 0:
                    NavigationHost.First().Value.Navigate<T>(contract, parameter);
                    break;
                default:
                    if (NavigationHost.TryGetValue(hostName, out var value))
                    {
                        value.Navigate<T>(contract, parameter);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Navigates the and reset.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="this">The this.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public static void NavigateToViewAndClearHistory<T>(this IUseNavigation @this, string? contract = null, object? parameter = null)
        where T : class, IRxObject
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && @this.Name != null)
        {
            switch (@this.Name.Length)
            {
                case 0:
                    NavigationHost.First().Value.NavigateAndReset<T>(contract, parameter);
                    break;
                default:
                    if (NavigationHost.TryGetValue(@this.Name, out var value))
                    {
                        value.NavigateAndReset<T>(contract, parameter);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Navigates to view and clear history.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="dummy">The dummy.</param>
    /// <param name="hostName">Name of the host.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public static void NavigateToViewAndClearHistory<T>(this IUseHostedNavigation dummy, string hostName = "", string? contract = null, object? parameter = null)
        where T : class, IRxObject
    {
        if (NavigationHost.Count == 0)
        {
            throw new InvalidOperationException("No navigation host registered, please ensure that the NavigationShell has a Name.");
        }

        if (NavigationHost.Count > 0 && hostName != null)
        {
            switch (hostName.Length)
            {
                case 0:
                    NavigationHost.First().Value.NavigateAndReset<T>(contract, parameter);
                    break;
                default:
                    if (NavigationHost.TryGetValue(hostName, out var value))
                    {
                        value.NavigateAndReset<T>(contract, parameter);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Sets the main navigation host.
    /// </summary>
    /// <param name="this">The dummy.</param>
    /// <param name="viewHost">The view host.</param>
    public static void SetMainNavigationHost(this ISetNavigation @this, IViewModelRoutedViewHost viewHost)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        if (viewHost == null)
        {
            throw new ArgumentNullException(nameof(viewHost));
        }

        if (NavigationHost.ContainsKey(@this.Name!))
        {
            return;
        }

        WhenSetupSubjects.Add(@this.Name!, new(1));
        NavigationHost.Add(@this.Name!, viewHost);
        CurrentViewDisposable.Add(@this.Name!, new());
        ResultNavigating.Add(@this.Name!, new Subject<IViewModelNavigatingEventArgs>());

        if (viewHost.RequiresSetup)
        {
            viewHost.Setup();
        }

        ASetupCompleted.OnNext(Unit.Default);
        WhenSetupSubjects[@this.Name!].OnNext(true);
    }

    /// <summary>
    /// Whens the navigated from.
    /// </summary>
    /// <param name="this">The this.</param>
    /// <param name="e">The e.</param>
    public static void WhenNavigatedFrom(this INotifiyNavigation @this, Action<IViewModelNavigationEventArgs> e)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        @this.ISetupNavigatedFrom = true;
        var vm = (@this as IViewFor)?.ViewModel as INotifiyRoutableViewModel;
        SetWhenNavigated.Where(x => x.From != null && x.From.Name == vm?.Name).Subscribe(ea =>
        {
            e(ea);
            ea.From?.WhenNavigatedFrom(ea);
        }).DisposeWith(@this.CleanUp);
    }

    /// <summary>
    /// Whens the navigated to.
    /// </summary>
    /// <param name="this">The this.</param>
    /// <param name="e">The e.</param>
    public static void WhenNavigatedTo(this INotifiyNavigation @this, Action<IViewModelNavigationEventArgs, CompositeDisposable> e)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        @this.ISetupNavigatedTo = true;
        var vm = (@this as IViewFor)?.ViewModel as INotifiyRoutableViewModel;
        SetWhenNavigated.Where(x => x?.To?.Name == vm?.Name).Subscribe(ea =>
        {
            if (ea.NavigationType == NavigationType.New)
            {
                CurrentViewDisposable[ea.HostName!]?.Dispose();
                CurrentViewDisposable[ea.HostName!] = new();
            }

            e(ea, CurrentViewDisposable[ea.HostName!]);
            ea?.To?.WhenNavigatedTo(ea, CurrentViewDisposable[ea.HostName!]);
        }).DisposeWith(@this.CleanUp);
    }

    /// <summary>
    /// Called when [navigating].
    /// </summary>
    /// <param name="this">The this.</param>
    /// <param name="e">
    /// The <see cref="IViewModelNavigatingEventArgs"/> instance containing the event data.
    /// </param>
    public static void WhenNavigating(this INotifiyNavigation @this, Func<IViewModelNavigatingEventArgs, IViewModelNavigatingEventArgs> e)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        @this.ISetupNavigating = true;
        var vm = (@this as IViewFor)?.ViewModel as INotifiyRoutableViewModel;
        SetWhenNavigating.Where(x => x?.From == null || x.From.Name == vm?.Name).Subscribe(ea =>
        {
            if (ea != null)
            {
                if (ea.From != null)
                {
                    e(ea);
                }

                ea.From?.WhenNavigating(ea);

                ResultNavigating[ea.HostName!].OnNext(ea);
            }
        }).DisposeWith(@this.CleanUp);
    }

    /// <summary>
    /// Notify When the Host is setup.
    /// </summary>
    /// <param name="this">The this.</param>
    /// <returns>A Bool.</returns>
    public static IObservable<bool> WhenSetup(this IUseNavigation @this) =>
        Observable.Create<bool>(obs =>
            {
                var dis = new CompositeDisposable();
                @this.BuildComplete(() => ASetupCompleted.Subscribe(_ =>
                    {
                        if (WhenSetupSubjects.Count > 0 && @this.Name != null)
                        {
                            switch (@this.Name.Length)
                            {
                                case 0:
                                    {
                                        WhenSetupSubjects.First().Value.Where(x => x).Subscribe(obs).DisposeWith(dis);
                                        break;
                                    }

                                default:
#pragma warning disable CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
                                    if (NavigationHost.ContainsKey(@this.Name))
                                    {
                                        WhenSetupSubjects[@this.Name].Where(x => x).Subscribe(obs).DisposeWith(dis);
                                    }

                                    break;
                            }
                        }
                    }).DisposeWith(dis));
                return dis;
            });

    /// <summary>
    /// Notify When the Host is setup.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="hostName">Name of the host.</param>
    /// <returns>
    /// A Bool.
    /// </returns>
    public static IObservable<bool> WhenSetup(this IUseHostedNavigation dummy, string? hostName = "") =>
        Observable.Create<bool>(obs =>
            {
                var dis = new CompositeDisposable();
                ASetupCompleted.Subscribe(_ =>
                {
                    if (WhenSetupSubjects.Count > 0)
                    {
                        if (hostName?.Length > 0)
                        {
                            if (NavigationHost.ContainsKey(hostName))
                            {
                                WhenSetupSubjects[hostName].Where(x => x).Subscribe(obs).DisposeWith(dis);
                            }
                        }
                        else
                        {
                            WhenSetupSubjects.First().Value.Where(x => x).Subscribe(obs).DisposeWith(dis);
                        }
                    }
                }).DisposeWith(dis);
                return dis;
            });
#pragma warning restore CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
#pragma warning restore RCS1175 // Unused 'this' parameter.
}
