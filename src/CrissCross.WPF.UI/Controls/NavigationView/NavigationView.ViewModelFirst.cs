// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>
/// Partial ViewModel-first integration for <see cref="NavigationView"/>.
/// Enables automatic activation/highlighting of items when ViewModel-based navigation occurs
/// via CrissCross navigation hosts.
/// </summary>
public partial class NavigationView : IUseHostedNavigation, IDisposable
{
    /// <summary>Stores the _disposed value.</summary>
    private bool _disposed;

    /// <summary>Stores the _viewModelNavigationSubscriptions value.</summary>
    private CompositeDisposable? _viewModelNavigationSubscriptions;

    /// <summary>Disposes resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Core dispose pattern.</summary>
    /// <param name="disposing">True if disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _viewModelNavigationSubscriptions?.Dispose();
            _viewModelNavigationSubscriptions = null;
        }

        _disposed = true;
    }

    /// <summary>Provides the EnumerateWithChildren member.</summary>
    /// <param name="item">The item value.</param>
    /// <returns>The result.</returns>
    private static IEnumerable<INavigationViewItem> EnumerateWithChildren(INavigationViewItem item)
    {
        yield return item;
        if (item.MenuItems is null)
        {
            yield break;
        }

        foreach (var candidate in item.MenuItems)
        {
            if (candidate is not INavigationViewItem child)
            {
                continue;
            }

            foreach (var d in EnumerateWithChildren(child))
            {
                yield return d;
            }
        }
    }

    /// <summary>Initializes ViewModel-first wiring (called lazily at first navigation).</summary>
    private void EnsureViewModelFirstWiring()
    {
        if (_viewModelNavigationSubscriptions is not null)
        {
            return;
        }

        _viewModelNavigationSubscriptions = [];

        // Hook into existing Navigated event; rely on DataContext/ViewModel assignment.
        Navigated += (_, __) =>
        {
            object? currentContent = null;
            try
            {
                currentContent = NavigationViewContentPresenter?.Content;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            var vm = (currentContent as FrameworkElement)?.DataContext;
            if (vm is null || !HasAnyViewModelTargets())
            {
                return;
            }

            var viewModelType = vm.GetType();
            _ = Dispatcher.BeginInvoke(() => ActivateItemForViewModel(viewModelType), DispatcherPriority.Background);
        };
    }

    /// <summary>Activates the first navigation item whose TargetViewModelType matches the supplied type.</summary>
    /// <param name="viewModelType">The ViewModel type.</param>
    private void ActivateItemForViewModel(Type viewModelType)
    {
        List<INavigationViewItem> allItems = [];
        INavigationViewItem? match = null;
        foreach (var item in EnumerateAllItems())
        {
            allItems.Add(item);
            if (match is null && item is NavigationViewItem { TargetViewModelType: { } targetViewModelType } && targetViewModelType == viewModelType)
            {
                match = item;
            }
        }

        if (match is null)
        {
            return;
        }

        // Deactivate others
        foreach (var item in allItems)
        {
            if (item == match)
            {
                continue;
            }

            if (item.IsActive)
            {
                item.Deactivate(this);
            }
        }

        // Activate and set selection
        match.Activate(this);
        if (SelectedItem == match)
        {
            return;
        }

        SelectedItem = match;
        OnSelectionChanged();
    }

    /// <summary>Provides the HasAnyViewModelTargets member.</summary>
    /// <returns>The result.</returns>
    private bool HasAnyViewModelTargets()
    {
        foreach (var item in EnumerateAllItems())
        {
            if (item is NavigationViewItem { TargetViewModelType: not null })
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Provides the EnumerateAllItems member.</summary>
    /// <returns>The result.</returns>
    private IEnumerable<INavigationViewItem> EnumerateAllItems()
    {
        foreach (var candidate in MenuItems)
        {
            if (candidate is not INavigationViewItem item)
            {
                continue;
            }

            foreach (var x in EnumerateWithChildren(item))
            {
                yield return x;
            }
        }

        foreach (var candidate in FooterMenuItems)
        {
            if (candidate is not INavigationViewItem item)
            {
                continue;
            }

            foreach (var x in EnumerateWithChildren(item))
            {
                yield return x;
            }
        }
    }
}
