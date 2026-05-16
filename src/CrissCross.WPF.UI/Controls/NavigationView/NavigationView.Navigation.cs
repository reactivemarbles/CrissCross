// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigationView.
/// </summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="INavigationView" />
public partial class NavigationView
{
    /// <summary>
    /// The journal.
    /// </summary>
    private readonly List<string> _journal = new(50);

    /// <summary>
    /// The navigation stack.
    /// </summary>
    private readonly ObservableCollection<INavigationViewItem> _navigationStack = [];

    private readonly NavigationCache _cache = new();

    private readonly Dictionary<
        INavigationViewItem,
        List<INavigationViewItem?[]>
    > _complexNavigationStackHistory = [];

    private IServiceProvider? _serviceProvider;
    private IPageService? _pageService;

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
        if (!_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem))
        {
            return TryToNavigateWithoutINavigationViewItem(pageType, false, dataContext);
        }

        return NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc />
    public virtual bool Navigate(string pageIdOrTargetTag, object? dataContext = null)
    {
        if (
            !_pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(
                pageIdOrTargetTag,
                out var navigationViewItem))
        {
            return false;
        }

        return NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc />
    public virtual bool NavigateWithHierarchy(Type pageType, object? dataContext = null)
    {
        if (!_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem))
        {
            return TryToNavigateWithoutINavigationViewItem(pageType, true, dataContext);
        }

        return NavigateInternal(navigationViewItem, dataContext, true);
    }

    /// <inheritdoc />
    public virtual bool ReplaceContent(Type? pageTypeToEmbed)
    {
        if (pageTypeToEmbed == null)
        {
            return false;
        }

        if (_serviceProvider != null)
        {
            UpdateContent(_serviceProvider.GetService(pageTypeToEmbed));

            return true;
        }

        if (_pageService == null)
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
        if (!NavigationJournal.TryMoveForward(_journal, _currentIndexInJournal, out var nextIndex, out var itemId) || itemId is null)
        {
            return false;
        }

        return _pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(itemId, out var navigationViewItem) &&
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

    private static void ApplyAttachedProperties(INavigationViewItem viewItem, object pageInstance)
    {
        if (
            pageInstance is FrameworkElement frameworkElement
            && GetHeaderContent(frameworkElement) is { } headerContent)
        {
            viewItem.Content = headerContent;
        }
    }

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

        if (_navigationStack.Count > 0 && SelectedItem != _navigationStack[0] && _navigationStack[0].IsMenuElement)
        {
            SelectedItem = _navigationStack[0];
            OnSelectionChanged();
        }

        return true;
    }

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

        if (_journal.Count > 0)
        {
            System.Diagnostics.Debug.WriteLineIf(EnableDebugMessages, $"JOURNAL LAST ELEMENT {_journal[^1]}");
        }
#endif
    }

    private object GetNavigationItemInstance(INavigationViewItem viewItem)
    {
        if (viewItem.TargetPageType is null)
        {
            throw new ArgumentNullException(nameof(viewItem.TargetPageType));
        }

        if (_serviceProvider is not null)
        {
            return _serviceProvider.GetService(viewItem.TargetPageType)
                ?? throw new InvalidOperationException($"{nameof(_serviceProvider.GetService)} returned null");
        }

        if (_pageService is not null)
        {
            return _pageService.GetPage(viewItem.TargetPageType)
                ?? throw new InvalidOperationException($"{nameof(_pageService.GetPage)} returned null");
        }

        return _cache.Remember(
                viewItem.TargetPageType,
                viewItem.NavigationCacheMode,
                ComputeCachedNavigationInstance)
            ?? throw new ArgumentNullException(
                $"Unable to get or create instance of {viewItem.TargetPageType} from cache.");

        object? ComputeCachedNavigationInstance() => GetPageInstanceFromCache(viewItem.TargetPageType);
    }

    private object? GetPageInstanceFromCache(Type? targetPageType)
    {
        if (targetPageType is null)
        {
            return default;
        }

        if (_serviceProvider is not null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(
                $"Getting {targetPageType} from cache using IServiceProvider.");
#endif

            return _serviceProvider.GetService(targetPageType)
                ?? throw new InvalidOperationException($"{nameof(_serviceProvider.GetService)} returned null");
        }

        if (_pageService is not null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Getting {targetPageType} from cache using IPageService.");
#endif

            return _pageService.GetPage(targetPageType)
                ?? throw new InvalidOperationException($"{nameof(_pageService.GetPage)} returned null");
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Getting {targetPageType} from cache using reflection.");
#endif

        return NavigationViewActivator.CreateInstance(targetPageType)
            ?? throw new ArgumentException("Failed to create instance of the page");
    }

    private void UpdateContent(object? content, object? dataContext = null)
    {
        if (dataContext is not null && content is FrameworkElement frameworkViewContent)
        {
            frameworkViewContent.DataContext = dataContext;
        }

        NavigationViewContentPresenter.Navigate(content);
    }

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

        for (var i = startIndex; i < latestHistory.Length; i++)
        {
            if (latestHistory[i] is null)
            {
                break;
            }

            AddToNavigationStack(latestHistory[i]!, true, false);
        }

        historyList.Remove(latestHistory);
        /*if (historyList.Count == 0)
            _complexNavigationStackHistory.Remove(item);
        */

#if NET6_0_OR_GREATER
        System.Buffers.ArrayPool<INavigationViewItem>.Shared.Return(latestHistory!, true);
#endif

        AddToNavigationStack(item, true, false);
    }

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
            historyList = new List<INavigationViewItem?[]>(5);
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
        if (i < latestHistory.Length)
        {
            System.Array.Clear(latestHistory, i, latestHistory.Length - i);
        }
    }

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
            _navigationStack.Remove(_navigationStack[j]);
        }
    }

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

    private void ReplaceFirstElementInNavigationStack(INavigationViewItem newItem)
    {
        _navigationStack[0].Deactivate(this);
        _navigationStack[0] = newItem;
        _navigationStack[0].Activate(this);
    }
}
