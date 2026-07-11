// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents NavigationView.</summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="INavigationView" />
public partial class NavigationView
{
    /// <summary>The journal.</summary>
    private readonly List<string> _journal = new(50);

    /// <summary>The navigation stack.</summary>
    private readonly ObservableCollection<INavigationViewItem> _navigationStack = [];

    /// <summary>Stores the _cache value.</summary>
    private readonly NavigationCache _cache = new();

    /// <summary>Stores the complex navigation stack history.</summary>
    private readonly Dictionary<INavigationViewItem, List<INavigationViewItem?[]>> _complexNavigationStackHistory = [];

    /// <summary>Stores the _serviceProvider value.</summary>
    private IServiceProvider? _serviceProvider;

    /// <summary>Stores the _pageService value.</summary>
    private IPageService? _pageService;

    /// <summary>Stores the _currentIndexInJournal value.</summary>
    private int _currentIndexInJournal = -1;

    /// <inheritdoc />
    public bool CanGoBack => NavigationJournal.CanGoBack(_journal, _currentIndexInJournal);

    /// <inheritdoc />
    public bool CanGoForward => NavigationJournal.CanGoForward(_journal, _currentIndexInJournal);

    /// <inheritdoc />
    public void SetPageService(IPageService pageService) => _pageService = pageService;

    /// <inheritdoc />
    public void SetServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <inheritdoc />
    public virtual bool Navigate(Type pageType, object? dataContext = null)
    {
        return !_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem) ? TryToNavigateWithoutINavigationViewItem(pageType, false, dataContext) : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc />
    public virtual bool Navigate(string pageIdOrTargetTag, object? dataContext = null)
    {
        return !_pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(
                pageIdOrTargetTag,
                out var navigationViewItem) ? false : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc />
    public virtual bool NavigateWithHierarchy(Type pageType, object? dataContext = null)
    {
        return !_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem) ? TryToNavigateWithoutINavigationViewItem(pageType, true, dataContext) : NavigateInternal(navigationViewItem, dataContext, true);
    }

    /// <inheritdoc />
    public virtual bool ReplaceContent(Type? pageTypeToEmbed)
    {
        if (pageTypeToEmbed is null)
        {
            return false;
        }

        if (_serviceProvider is not null)
        {
            UpdateContent(_serviceProvider.GetService(pageTypeToEmbed));

            return true;
        }

        if (_pageService is null)
        {
            return false;
        }

        UpdateContent(_pageService.GetPage(pageTypeToEmbed));

        return true;
    }

    /// <inheritdoc />
    public virtual bool ReplaceContent(UIElement pageInstanceToEmbed, object? dataContext = null)
    {
        UpdateContent(pageInstanceToEmbed, dataContext);

        return true;
    }

    /// <inheritdoc />
    public virtual bool GoForward()
    {
        return !NavigationJournal.TryMoveForward(_journal, _currentIndexInJournal, out var nextIndex, out var itemId) || itemId is null ? false : _pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(itemId, out var navigationViewItem) &&
            NavigateInternal(navigationViewItem, null, false, false, true, nextIndex);
    }

    /// <inheritdoc />
    public virtual bool GoBack()
    {
        if (!NavigationJournal.TryMoveBack(_journal, _currentIndexInJournal, out var nextIndex, out var itemId) || itemId is null)
        {
            return false;
        }

        OnBackRequested();
        return _pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(itemId, out var navigationViewItem) &&
            NavigateInternal(navigationViewItem, null, false, true, true, nextIndex);
    }

    /// <inheritdoc />
    public virtual void ClearJournal()
    {
        NavigationJournal.Clear(_journal, ref _currentIndexInJournal);
        _complexNavigationStackHistory.Clear();
    }

    /// <summary>Provides the ApplyAttachedProperties member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <param name="pageInstance">The pageInstance value.</param>
    private static void ApplyAttachedProperties(INavigationViewItem viewItem, object pageInstance)
    {
        if (
            pageInstance is not FrameworkElement frameworkElement
            || !(GetHeaderContent(frameworkElement) is { } headerContent))
        {
            return;
        }

        viewItem.Content = headerContent;
    }

    /// <summary>Provides the TryToNavigateWithoutINavigationViewItem member.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <param name="addToNavigationStack">The addToNavigationStack value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private bool TryToNavigateWithoutINavigationViewItem(
        Type pageType,
        bool addToNavigationStack,
        object? dataContext = null)
    {
        var navigationViewItem = new NavigationViewItem(pageType);

        if (!NavigateInternal(navigationViewItem, dataContext, addToNavigationStack))
        {
            return false;
        }

        _pageTypeNavigationViewsDictionary.Add(pageType, navigationViewItem);
        _pageIdOrTargetTagNavigationViewsDictionary.Add(navigationViewItem.Id, navigationViewItem);

        return true;
    }

    /// <summary>Provides the NavigateInternal member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <param name="addToNavigationStack">The addToNavigationStack value.</param>
    /// <param name="isBackwardsNavigated">The isBackwardsNavigated value.</param>
    /// <param name="isJournalNavigation">The isJournalNavigation value.</param>
    /// <param name="journalIndex">The journalIndex value.</param>
    /// <returns>The result.</returns>
    private bool NavigateInternal(
        INavigationViewItem viewItem,
        object? dataContext = null,
        bool addToNavigationStack = false,
        bool isBackwardsNavigated = false,
        bool isJournalNavigation = false,
        int journalIndex = -1)
    {
        if (_navigationStack.Count > 0 && _navigationStack[^1] == viewItem)
        {
            if (isJournalNavigation)
            {
                AddToJournal(viewItem, true, journalIndex);
                return true;
            }

            return false;
        }

        var pageInstance = GetNavigationItemInstance(viewItem);

        if (OnNavigating(pageInstance))
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLineIf(EnableDebugMessages, "Navigation canceled");
#endif

            return false;
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLineIf(
            EnableDebugMessages,
            $"DEBUG | {viewItem.Id} - {(string.IsNullOrEmpty(viewItem.TargetPageTag) ? "NO_TAG" : viewItem.TargetPageTag)} - {viewItem.TargetPageType} | NAVIGATED");
#endif

        OnNavigated(pageInstance);

        EnsureViewModelFirstWiring();

        ApplyAttachedProperties(viewItem, pageInstance);
        UpdateContent(pageInstance, dataContext);

        AddToNavigationStack(viewItem, addToNavigationStack, isBackwardsNavigated);
        AddToJournal(viewItem, isJournalNavigation, journalIndex);

        if (_navigationStack.Count == 0 || SelectedItem == _navigationStack[0] || !_navigationStack[0].IsMenuElement)
        {
            return true;
        }

        SelectedItem = _navigationStack[0];
        OnSelectionChanged();

        return true;
    }

    /// <summary>Provides the AddToJournal member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <param name="isJournalNavigation">The isJournalNavigation value.</param>
    /// <param name="journalIndex">The journalIndex value.</param>
    private void AddToJournal(INavigationViewItem viewItem, bool isJournalNavigation, int journalIndex)
    {
        if (isJournalNavigation)
        {
            _currentIndexInJournal = journalIndex;
        }
        else
        {
            NavigationJournal.Record(_journal, ref _currentIndexInJournal, viewItem.Id);
        }

        IsBackEnabled = CanGoBack;

#if DEBUG
        System.Diagnostics.Debug.WriteLineIf(EnableDebugMessages, $"JOURNAL INDEX {_currentIndexInJournal}");

        System.Diagnostics.Debug.WriteLineIf(EnableDebugMessages && _journal.Count > 0, $"JOURNAL LAST ELEMENT {_journal[^1]}");
#endif
    }

    /// <summary>Provides the GetNavigationItemInstance member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <returns>The result.</returns>
    private object GetNavigationItemInstance(INavigationViewItem viewItem)
    {
        if (viewItem.TargetPageType is null)
        {
            throw new ArgumentNullException(nameof(viewItem.TargetPageType));
        }

        return (_serviceProvider, _pageService) switch
        {
            ({ } serviceProvider, _) => serviceProvider.GetService(viewItem.TargetPageType)
                ?? throw new InvalidOperationException($"{nameof(_serviceProvider.GetService)} returned null"),
            (_, { } pageService) => pageService.GetPage(viewItem.TargetPageType)
                ?? throw new InvalidOperationException($"{nameof(_pageService.GetPage)} returned null"),
            _ => _cache.Remember(
                viewItem.TargetPageType,
                viewItem.NavigationCacheMode,
                ComputeCachedNavigationInstance)
                ?? throw new ArgumentNullException(
                    $"Unable to get or create instance of {viewItem.TargetPageType} from cache."),
        };
        object? ComputeCachedNavigationInstance() => GetPageInstanceFromCache(viewItem.TargetPageType);
    }

    /// <summary>Provides the GetPageInstanceFromCache member.</summary>
    /// <param name="targetPageType">The targetPageType value.</param>
    /// <returns>The result.</returns>
    private object? GetPageInstanceFromCache(Type? targetPageType)
    {
        return targetPageType is null ? default : ResolvePageInstance(targetPageType);

        object ResolvePageInstance(Type pageType)
        {
            return (_serviceProvider, _pageService) switch
            {
                ({ } serviceProvider, _) => GetPageFromServiceProvider(serviceProvider),
                (_, { } pageService) => pageService.GetPage(pageType)
                    ?? throw new InvalidOperationException($"{nameof(_pageService.GetPage)} returned null"),
                _ => NavigationViewActivator.CreateInstance(pageType)
                    ?? throw new ArgumentException("Failed to create instance of the page"),
            };

            object GetPageFromServiceProvider(IServiceProvider serviceProvider)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(
                    $"Getting {targetPageType} from cache using IServiceProvider.");
#endif

                return serviceProvider.GetService(pageType)
                    ?? throw new InvalidOperationException($"{nameof(_serviceProvider.GetService)} returned null");
            }
        }
    }

    /// <summary>Provides the UpdateContent member.</summary>
    /// <param name="content">The content value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    private void UpdateContent(object? content, object? dataContext = null)
    {
        if (dataContext is not null && content is FrameworkElement frameworkViewContent)
        {
            frameworkViewContent.DataContext = dataContext;
        }

        _ = NavigationViewContentPresenter.Navigate(content);
    }

    /// <summary>Provides the OnNavigationViewContentPresenterNavigated member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnNavigationViewContentPresenterNavigated(
        object sender,
        System.Windows.Navigation.NavigationEventArgs e)
    {
        if (sender is not System.Windows.Controls.Frame frame)
        {
            return;
        }

        _ = frame.RemoveBackEntry();
    }

    /// <summary>Provides the AddToNavigationStack member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <param name="addToNavigationStack">The addToNavigationStack value.</param>
    /// <param name="isBackwardsNavigated">The isBackwardsNavigated value.</param>
    private void AddToNavigationStack(
        INavigationViewItem viewItem,
        bool addToNavigationStack,
        bool isBackwardsNavigated)
    {
        if (isBackwardsNavigated)
        {
            RecreateNavigationStackFromHistory(viewItem);
        }

        if (addToNavigationStack && !_navigationStack.Contains(viewItem))
        {
            viewItem.Activate(this);
            _navigationStack.Add(viewItem);
        }

        if (!addToNavigationStack)
        {
            UpdateCurrentNavigationStackItem(viewItem);
        }

        ClearNavigationStack(viewItem);
    }

    /// <summary>Provides the UpdateCurrentNavigationStackItem member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    private void UpdateCurrentNavigationStackItem(INavigationViewItem viewItem)
    {
        if (_navigationStack.Contains(viewItem))
        {
            return;
        }

        if (_navigationStack.Count > 1)
        {
            AddToNavigationStackHistory(viewItem);
        }

        if (_navigationStack.Count == 0)
        {
            viewItem.Activate(this);
            _navigationStack.Add(viewItem);
        }
        else
        {
            ReplaceFirstElementInNavigationStack(viewItem);
        }

        ClearNavigationStack(1);
    }

    /// <summary>Provides the RecreateNavigationStackFromHistory member.</summary>
    /// <param name="item">The item value.</param>
    private void RecreateNavigationStackFromHistory(INavigationViewItem item)
    {
        if (!_complexNavigationStackHistory.TryGetValue(item, out var historyList) || historyList.Count == 0)
        {
            return;
        }

        var latestHistory = historyList[^1];
        var startIndex = 0;

        if (latestHistory[0]!.IsMenuElement)
        {
            startIndex = 1;
            ReplaceFirstElementInNavigationStack(latestHistory[0]!);
        }

        for (var i = startIndex; i < latestHistory.Length && latestHistory[i] is not null; i++)
        {
            AddToNavigationStack(latestHistory[i]!, true, false);
        }

        _ = historyList.Remove(latestHistory);
        /*if (historyList.Count == 0)
            _complexNavigationStackHistory.Remove(item);
        */

#if NET6_0_OR_GREATER
        System.Buffers.ArrayPool<INavigationViewItem>.Shared.Return(latestHistory!, true);
#endif

        AddToNavigationStack(item, true, false);
    }

    /// <summary>Provides the AddToNavigationStackHistory member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    private void AddToNavigationStackHistory(INavigationViewItem viewItem)
    {
        var lastItem = _navigationStack[^1];
        var startIndex = _navigationStack.IndexOf(viewItem);

        if (startIndex < 0)
        {
            startIndex = 0;
        }

        if (!_complexNavigationStackHistory.TryGetValue(lastItem, out var historyList))
        {
            historyList = new(5);
            _complexNavigationStackHistory.Add(lastItem, historyList);
        }

        var arrayLength = _navigationStack.Count - 1 - startIndex;
        INavigationViewItem[] array;

//// Initializing an array every time well... not an ideal
#if NET6_0_OR_GREATER
        array = System.Buffers.ArrayPool<INavigationViewItem>.Shared.Rent(arrayLength);
#else
        array = new INavigationViewItem[arrayLength];
#endif

        historyList.Add(array);

        var latestHistory = historyList[^1];
        var i = 0;

        for (var j = startIndex; j < _navigationStack.Count - 1; j++)
        {
            latestHistory[i] = _navigationStack[j];
            i++;
        }

        // Ensure unused slots are null when using pooled arrays to avoid stale references
        if (i >= latestHistory.Length)
        {
            return;
        }

        System.Array.Clear(latestHistory, i, latestHistory.Length - i);
    }

    /// <summary>Provides the ClearNavigationStack member.</summary>
    /// <param name="navigationStackItemIndex">The navigationStackItemIndex value.</param>
    private void ClearNavigationStack(int navigationStackItemIndex)
    {
        var navigationStackCount = _navigationStack.Count;
        var length = navigationStackCount - navigationStackItemIndex;

        if (length == 0)
        {
            return;
        }

        for (var j = navigationStackCount - 1; j >= navigationStackCount - length; j--)
        {
            _ = _navigationStack.Remove(_navigationStack[j]);
        }
    }

    /// <summary>Provides the ClearNavigationStack member.</summary>
    /// <param name="item">The item value.</param>
    private void ClearNavigationStack(INavigationViewItem item)
    {
        var navigationStackCount = _navigationStack.Count;
        if (navigationStackCount <= 1)
        {
            return;
        }

        var index = _navigationStack.IndexOf(item);
        if (index >= navigationStackCount - 1)
        {
            return;
        }

        AddToNavigationStackHistory(item);
        ClearNavigationStack(++index);
    }

    /// <summary>Provides the ReplaceFirstElementInNavigationStack member.</summary>
    /// <param name="newItem">The newItem value.</param>
    private void ReplaceFirstElementInNavigationStack(INavigationViewItem newItem)
    {
        _navigationStack[0].Deactivate(this);
        _navigationStack[0] = newItem;
        _navigationStack[0].Activate(this);
    }
}
