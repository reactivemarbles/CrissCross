// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Windows.Threading;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Partial ViewModel-first integration for <see cref="NavigationView"/>.
/// Enables automatic activation/highlighting of items when ViewModel-based navigation occurs
/// via CrissCross navigation hosts.
/// </summary>
public partial class NavigationView : IUseHostedNavigation, IDisposable
{
    private bool _disposed;
    private CompositeDisposable? _vmNavigationSubscriptions;

    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Core dispose pattern.
    /// </summary>
    /// <param name="disposing">True if disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _vmNavigationSubscriptions?.Dispose();
            _vmNavigationSubscriptions = null;
        }

        _disposed = true;
    }

    private static IEnumerable<INavigationViewItem> EnumerateWithChildren(INavigationViewItem item)
    {
        yield return item;
        if (item.MenuItems == null)
        {
            yield break;
        }

        foreach (var child in item.MenuItems.OfType<INavigationViewItem>())
        {
            foreach (var d in EnumerateWithChildren(child))
            {
                yield return d;
            }
        }
    }

    /// <summary>
    /// Initializes ViewModel-first wiring (called lazily at first navigation).
    /// </summary>
    private void EnsureViewModelFirstWiring()
    {
        if (_vmNavigationSubscriptions != null)
        {
            return;
        }

        _vmNavigationSubscriptions = [];

        // Hook into existing Navigated event; rely on DataContext/ViewModel assignment.
        Navigated += (_, __) =>
        {
            object? currentContent = null;
            try
            {
                currentContent = NavigationViewContentPresenter?.Content;
            }
            catch
            {
            }

            var vm = (currentContent as FrameworkElement)?.DataContext;
            if (vm == null || !HasAnyViewModelTargets())
            {
                return;
            }

            var vmType = vm.GetType();
            Dispatcher.BeginInvoke(() => ActivateItemForViewModel(vmType), DispatcherPriority.Background);
        };
    }

    /// <summary>
    /// Activates the first navigation item whose TargetViewModelType matches the supplied type.
    /// </summary>
    /// <param name="vmType">The ViewModel type.</param>
    private void ActivateItemForViewModel(Type vmType)
    {
        var allItems = EnumerateAllItems().ToList();
        var match = allItems.FirstOrDefault(x => (x as NavigationViewItem)?.TargetViewModelType == vmType);
        if (match == null)
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
        if (SelectedItem != match)
        {
            SelectedItem = match;
            OnSelectionChanged();
        }
    }

    private bool HasAnyViewModelTargets() => EnumerateAllItems().Any(i => i is NavigationViewItem { TargetViewModelType: not null });

    private IEnumerable<INavigationViewItem> EnumerateAllItems()
    {
        foreach (var o in MenuItems.OfType<INavigationViewItem>())
        {
            foreach (var x in EnumerateWithChildren(o))
            {
                yield return x;
            }
        }

        foreach (var o in FooterMenuItems.OfType<INavigationViewItem>())
        {
            foreach (var x in EnumerateWithChildren(o))
            {
                yield return x;
            }
        }
    }
}
