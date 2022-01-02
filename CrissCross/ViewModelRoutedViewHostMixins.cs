using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(" CrissCross.WPF")]
[assembly: InternalsVisibleTo(" CrissCross.XamForms")]

namespace CrissCross
{
    /// <summary>
    /// View Model Routed View Host Mixins.
    /// </summary>
    public static class ViewModelRoutedViewHostMixins
    {
        internal static Subject<Unit> ASetupCompleted { get; } = new();
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
        /// <returns></returns>
        public static IObservable<bool> CanNavigateBack(this IUseNavigation @this)
        {
            if (NavigationHost.Count > 0 && @this.Name != null)
            {
                if (@this.Name.Length == 0)
                {
                    return NavigationHost.First().Value.CanNavigateBackObservable;
                }
                else
                {
                    if (NavigationHost.ContainsKey(@this.Name))
                    {
                        return NavigationHost[@this.Name].CanNavigateBackObservable;
                    }
                }
            }

            return Observable.Empty<bool>();
        }

        /// <summary>
        /// Determines whether this instance [can navigate back] the specified host name.
        /// </summary>
        /// <param name="_">The .</param>
        /// <param name="hostName">Name of the host.</param>
        /// <returns></returns>
        public static IObservable<bool> CanNavigateBack(this IUseHostedNavigation _, string hostName = "")
        {
            if (NavigationHost.Count > 0 && hostName != null)
            {
                if (hostName.Length == 0)
                {
                    return NavigationHost.First().Value.CanNavigateBackObservable;
                }
                else
                {
                    if (NavigationHost.ContainsKey(hostName))
                    {
                        return NavigationHost[hostName].CanNavigateBackObservable;
                    }
                }
            }

            return Observable.Empty<bool>();
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        /// <param name="this">The dummy.</param>
        public static void ClearHistory(this IUseNavigation @this)
        {
            if (NavigationHost.Count > 0 && @this.Name != null)
            {
                if (@this.Name.Length == 0)
                {
                    NavigationHost.First().Value.ClearHistory();
                }
                else
                {
                    NavigationHost[@this.Name].ClearHistory();
                }
            }
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        /// <param name="_">The dummy.</param>
        /// <param name="hostName">Name of the host.</param>
        public static void ClearHistory(this IUseHostedNavigation _, string hostName = "")
        {
            if (NavigationHost.Count > 0 && hostName != null)
            {
                if (hostName.Length == 0)
                {
                    NavigationHost.First().Value.ClearHistory();
                }
                else
                {
                    NavigationHost[hostName].ClearHistory();
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
            if (NavigationHost.Count > 0 && @this.Name != null)
            {
                if (@this.Name.Length == 0)
                {
                    NavigationHost.First().Value.NavigateBack(parameter);
                }
                else
                {
                    if (NavigationHost.ContainsKey(@this.Name))
                    {
                        NavigationHost[@this.Name].NavigateBack(parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Navigates backwards.
        /// </summary>
        /// <param name="_">The dummy.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="parameter">The parameter.</param>
        public static void NavigateBack(this IUseHostedNavigation _, string hostName = "", object? parameter = null)
        {
            if (NavigationHost.Count > 0 && hostName != null)
            {
                if (hostName.Length == 0)
                {
                    NavigationHost.First().Value.NavigateBack(parameter);
                }
                else
                {
                    if (NavigationHost.ContainsKey(hostName))
                    {
                        NavigationHost[hostName].NavigateBack(parameter);
                    }
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
            if (NavigationHost.Count > 0 && @this.Name != null)
            {
                if (@this.Name.Length == 0)
                {
                    NavigationHost.First().Value.Navigate<T>(contract, parameter);
                }
                else
                {
                    NavigationHost[@this.Name].Navigate<T>(contract, parameter);
                }
            }
        }

        /// <summary>
        /// Navigates to view.
        /// </summary>
        /// <typeparam name="T">The Type.</typeparam>
        /// <param name="_">The dummy.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The parameter.</param>
        public static void NavigateToView<T>(this IUseHostedNavigation _, string? hostName = "", string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            if (NavigationHost.Count > 0 && hostName != null)
            {
                if (hostName.Length == 0)
                {
                    NavigationHost.First().Value.Navigate<T>(contract, parameter);
                }
                else
                {
                    if (NavigationHost.ContainsKey(hostName))
                    {
                        NavigationHost[hostName].Navigate<T>(contract, parameter);
                    }
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
            if (NavigationHost.Count > 0 && @this.Name != null)
            {
                if (@this.Name.Length == 0)
                {
                    NavigationHost.First().Value.NavigateAndReset<T>(contract, parameter);
                }
                else
                {
                    if (NavigationHost.ContainsKey(@this.Name))
                    {
                        NavigationHost[@this.Name].NavigateAndReset<T>(contract, parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Navigates to view and clear history.
        /// </summary>
        /// <typeparam name="T">The Type.</typeparam>
        /// <param name="_">The dummy.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="parameter">The parameter.</param>
        public static void NavigateToViewAndClearHistory<T>(this IUseHostedNavigation _, string hostName = "", string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            if (NavigationHost.Count > 0 && hostName != null)
            {
                if (hostName.Length == 0)
                {
                    NavigationHost.First().Value.NavigateAndReset<T>(contract, parameter);
                }
                else
                {
                    if (NavigationHost.ContainsKey(hostName))
                    {
                        NavigationHost[hostName].NavigateAndReset<T>(contract, parameter);
                    }
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
            if (NavigationHost.ContainsKey(@this.Name))
            {
                return;
            }

            WhenSetupSubjects.Add(@this.Name, new(1));
            NavigationHost.Add(@this.Name, viewHost);
            CurrentViewDisposable.Add(@this.Name, new CompositeDisposable());
            ResultNavigating.Add(@this.Name, new Subject<IViewModelNavigatingEventArgs>());

            if (viewHost.RequiresSetup)
            {
                viewHost.Setup();
            }
            ASetupCompleted.OnNext(Unit.Default);
            WhenSetupSubjects[@this.Name].OnNext(true);
        }

        /// <summary>
        /// Whens the navigated from.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="e">The e.</param>
        public static void WhenNavigatedFrom(this INotifiyNavigation @this, Action<IViewModelNavigationEventArgs> e)
        {
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
            @this.ISetupNavigatedTo = true;
            var vm = (@this as IViewFor)?.ViewModel as INotifiyRoutableViewModel;
            SetWhenNavigated.Where(x => x?.To?.Name == vm?.Name).Subscribe(ea =>
            {
                if (ea.NavigationType == NavigationType.New)
                {
                    CurrentViewDisposable[ea.HostName]?.Dispose();
                    CurrentViewDisposable[ea.HostName] = new CompositeDisposable();
                }

                e(ea, CurrentViewDisposable[ea.HostName]);
                ea?.To?.WhenNavigatedTo(ea, CurrentViewDisposable[ea.HostName]);
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

                    ResultNavigating[ea.HostName].OnNext(ea);
                }
            }).DisposeWith(@this.CleanUp);
        }

        /// <summary>
        /// Whens the activated.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <returns></returns>
        public static IObservable<bool> WhenSetup(this IUseNavigation @this)
        {
            return Observable.Create<bool>(obs =>
             {
                 var dis = new CompositeDisposable();
                 ASetupCompleted.Subscribe(_ =>
                  {
                      if (WhenSetupSubjects.Count > 0 && @this.Name != null)
                      {
                          if (@this.Name.Length == 0)
                          {
                              WhenSetupSubjects.First().Value.Where(x => x).Subscribe(obs).DisposeWith(dis);
                          }
                          else
                          {
                              if (NavigationHost.ContainsKey(@this.Name))
                              {
                                  WhenSetupSubjects[@this.Name].Where(x => x).Subscribe(obs).DisposeWith(dis);
                              }
                          }
                      }
                  }).DisposeWith(dis);
                 return dis;
             });
        }

        /// <summary>
        /// Whens the activated.
        /// </summary>
        /// <param name="_">The .</param>
        /// <param name="hostName">Name of the host.</param>
        /// <returns></returns>
        public static IObservable<bool> WhenSetup(this IUseHostedNavigation _, string? hostName = "")
        {
            return Observable.Create<bool>(obs =>
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
        }
    }
}