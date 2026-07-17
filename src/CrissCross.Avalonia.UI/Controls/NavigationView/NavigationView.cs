// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>NavigationView provides a collapsible navigation menu for top-level navigation.</summary>
public partial class NavigationView : TemplatedControl, INavigationView
{
    /// <summary>The template element navigation view content presenter.</summary>
    protected static readonly string TemplateElementNavigationViewContentPresenter =
        "PART_NavigationViewContentPresenter";

    /// <summary>The template element menu items control.</summary>
    protected static readonly string TemplateElementMenuItemsItemsControl = "PART_MenuItemsItemsControl";

    /// <summary>The template element footer menu items control.</summary>
    protected static readonly string TemplateElementFooterMenuItemsItemsControl =
        "PART_FooterMenuItemsItemsControl";

    /// <summary>The template element back button.</summary>
    protected static readonly string TemplateElementBackButton = "PART_BackButton";

    /// <summary>The template element toggle button.</summary>
    protected static readonly string TemplateElementToggleButton = "PART_ToggleButton";

    /// <summary>The template element auto suggest box symbol button.</summary>
    protected static readonly string TemplateElementAutoSuggestBoxSymbolButton =
        "PART_AutoSuggestBoxSymbolButton";

    /// <summary>The journal.</summary>
    private readonly List<string> _journal = new(50);

    /// <summary>The navigation stack.</summary>
    private readonly ObservableCollection<INavigationViewItem> _navigationStack = [];

    /// <summary>The page identifier or target tag navigation views dictionary.</summary>
    private readonly Dictionary<
        string,
        INavigationViewItem
    > _pageIdOrTargetTagNavigationViewsDictionary = [];

    /// <summary>The page type navigation views dictionary.</summary>
    private readonly Dictionary<Type, INavigationViewItem> _pageTypeNavigationViewsDictionary = [];

    /// <summary>Provides the _menuItems member.</summary>
    private readonly ObservableCollection<object> _menuItems = [];

    /// <summary>Provides the _footerMenuItems member.</summary>
    private readonly ObservableCollection<object> _footerMenuItems = [];

    /// <summary>Provides the _cache member.</summary>
    private readonly NavigationCache _cache = new();

    /// <summary>Provides the _serviceProvider member.</summary>
    private IServiceProvider? _serviceProvider;

    /// <summary>Provides the _pageService member.</summary>
    private IPageService? _pageService;

    /// <summary>Provides the _currentIndexInJournal member.</summary>
    private int _currentIndexInJournal = -1;

    /// <summary>Provides the _navigationViewContentPresenter member.</summary>
    private ContentPresenter? _navigationViewContentPresenter;

    /// <summary>Provides the _backButton member.</summary>
    private Button? _backButton;

    /// <summary>Provides the _toggleButton member.</summary>
    private ToggleButton? _toggleButton;

    /// <summary>Provides the NavigationView member.</summary>
    static NavigationView()
    {
        _ = IsPaneOpenProperty.Changed.AddClassHandler<NavigationView>(
            (x, e) => x.OnIsPaneOpenChanged(e));
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationView"/> class.</summary>
    public NavigationView()
    {
        _menuItems.CollectionChanged += OnMenuItemsCollectionChanged;
        _footerMenuItems.CollectionChanged += OnMenuItemsCollectionChanged;
    }

    /// <inheritdoc/>
    public void SetPageService(IPageService pageService) => _pageService = pageService;

    /// <inheritdoc/>
    public void SetServiceProvider(IServiceProvider serviceProvider) =>
        _serviceProvider = serviceProvider;

    /// <inheritdoc/>
    public bool Navigate(Type pageType) => Navigate(pageType, null);

    /// <inheritdoc />
    public virtual bool Navigate(Type pageType, object? dataContext)
    {
        return !_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem)
            ? TryToNavigateWithoutINavigationViewItem(pageType, dataContext)
            : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc/>
    public bool Navigate(string pageIdOrTargetTag) => Navigate(pageIdOrTargetTag, null);

    /// <inheritdoc />
    public virtual bool Navigate(string pageIdOrTargetTag, object? dataContext)
    {
        return !_pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(
            pageIdOrTargetTag,
            out var navigationViewItem)
            ? false
            : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc/>
    public bool NavigateWithHierarchy(Type pageType) => NavigateWithHierarchy(pageType, null);

    /// <inheritdoc />
    public virtual bool NavigateWithHierarchy(Type pageType, object? dataContext)
    {
        return !_pageTypeNavigationViewsDictionary.TryGetValue(pageType, out var navigationViewItem)
            ? TryToNavigateWithoutINavigationViewItem(pageType, dataContext)
            : NavigateInternal(navigationViewItem, dataContext);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool ReplaceContent(Control pageInstanceToEmbed) => ReplaceContent(pageInstanceToEmbed, null);

    /// <inheritdoc />
    public virtual bool ReplaceContent(Control pageInstanceToEmbed, object? dataContext)
    {
        UpdateContent(pageInstanceToEmbed, dataContext);
        return true;
    }

    /// <inheritdoc/>
    public virtual bool GoForward()
    {
        return NavigationJournal.TryMoveForward(_journal, _currentIndexInJournal, out var nextIndex, out var itemId)
            && itemId is not null
            && _pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(itemId, out var navigationViewItem)
            && NavigateInternal(navigationViewItem, isJournalNavigation: true, journalIndex: nextIndex);
    }

    /// <inheritdoc/>
    public virtual bool GoBack()
    {
        if (
            !NavigationJournal.TryMoveBack(
                _journal,
                _currentIndexInJournal,
                out var nextIndex,
                out var itemId) || itemId is null)
        {
            return false;
        }

        RaiseEvent(new RoutedEventArgs(BackRequestedEvent));
        return _pageIdOrTargetTagNavigationViewsDictionary.TryGetValue(
                itemId,
                out var navigationViewItem)
            && NavigateInternal(
                navigationViewItem,
                isJournalNavigation: true,
                journalIndex: nextIndex);
    }

    /// <inheritdoc/>
    public virtual void ClearJournal()
    {
        NavigationJournal.Clear(_journal, ref _currentIndexInJournal);
        _cache.Clear();
    }

    /// <summary>Called when a navigation view item is clicked.</summary>
    /// <param name="navigationViewItem">The navigation view item.</param>
    internal void OnNavigationViewItemClick(NavigationViewItem navigationViewItem)
    {
        RaiseEvent(new RoutedEventArgs(ItemInvokedEvent));
        _ = NavigateInternal(navigationViewItem);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ArgumentNullException.ThrowIfNull(e);

        _navigationViewContentPresenter = e.NameScope.Find<ContentPresenter>(
            TemplateElementNavigationViewContentPresenter);
        _backButton = e.NameScope.Find<Button>(TemplateElementBackButton);
        _toggleButton = e.NameScope.Find<ToggleButton>(TemplateElementToggleButton);

        if (_backButton is not null)
        {
            _backButton.Click += OnBackButtonClick;
        }

        if (_toggleButton is not null)
        {
            _toggleButton.Click += OnToggleButtonClick;
        }

        AddItemsToDictionaries();
    }

    /// <summary>Called when back button is clicked.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnBackButtonClick(object? sender, RoutedEventArgs e) => GoBack();

    /// <summary>Called when toggle button is clicked.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnToggleButtonClick(object? sender, RoutedEventArgs e) =>
        IsPaneOpen = !IsPaneOpen;

    /// <summary>Called when IsPaneOpen changes.</summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnIsPaneOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        var isOpen = (bool)e.NewValue!;
        RaiseEvent(new RoutedEventArgs(isOpen ? PaneOpenedEvent : PaneClosedEvent));
    }

    /// <summary>Adds items to dictionaries.</summary>
    protected virtual void AddItemsToDictionaries()
    {
        AddItemsToDictionaries(MenuItems);
        AddItemsToDictionaries(FooterMenuItems);
    }

    /// <summary>Adds items to dictionaries.</summary>
    /// <param name="list">The list.</param>
    protected virtual void AddItemsToDictionaries(IEnumerable? list)
    {
        if (list is null)
        {
            return;
        }

        foreach (var singleNavigationViewItem in list.OfType<NavigationViewItem>())
        {
            if (
                !_pageIdOrTargetTagNavigationViewsDictionary.ContainsKey(
                    singleNavigationViewItem.Id))
            {
                _pageIdOrTargetTagNavigationViewsDictionary.Add(
                    singleNavigationViewItem.Id,
                    singleNavigationViewItem);
            }

            if (
                !string.IsNullOrEmpty(singleNavigationViewItem.TargetPageTag)
                && !_pageIdOrTargetTagNavigationViewsDictionary.ContainsKey(
                    singleNavigationViewItem.TargetPageTag))
            {
                _pageIdOrTargetTagNavigationViewsDictionary.Add(
                    singleNavigationViewItem.TargetPageTag,
                    singleNavigationViewItem);
            }

            if (
                singleNavigationViewItem.TargetPageType is not null
                && !_pageTypeNavigationViewsDictionary.ContainsKey(
                    singleNavigationViewItem.TargetPageType))
            {
                _pageTypeNavigationViewsDictionary.Add(
                    singleNavigationViewItem.TargetPageType,
                    singleNavigationViewItem);
            }

            singleNavigationViewItem.IsMenuElement = true;

            if (singleNavigationViewItem.HasMenuItems)
            {
                AddItemsToDictionaries(singleNavigationViewItem.MenuItems);
            }
        }
    }

    /// <summary>Provides the TryToNavigateWithoutINavigationViewItem member.</summary>
    /// <param name="pageType">The pageType value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private bool TryToNavigateWithoutINavigationViewItem(Type pageType, object? dataContext = null)
    {
        var navigationViewItem = new NavigationViewItem(pageType);

        if (!NavigateInternal(navigationViewItem, dataContext))
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
    /// <param name="isJournalNavigation">The isJournalNavigation value.</param>
    /// <param name="journalIndex">The journalIndex value.</param>
    /// <returns>The result.</returns>
    private bool NavigateInternal(
        INavigationViewItem viewItem,
        object? dataContext = null,
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

        var navigatingArgs = new NavigatingCancelEventArgs(NavigatingEvent, this)
        {
            Page = pageInstance,
        };
        RaiseEvent(navigatingArgs);

        if (navigatingArgs.Cancel)
        {
            return false;
        }

        var navigatedArgs = new NavigatedEventArgs(NavigatedEvent, this) { Page = pageInstance };
        RaiseEvent(navigatedArgs);

        UpdateContent(pageInstance, dataContext);

        AddToJournal(viewItem, isJournalNavigation, journalIndex);

        if (
            _navigationStack.Count == 0
            || SelectedItem == _navigationStack[0]
            || !_navigationStack[0].IsMenuElement)
        {
            return true;
        }

        SelectedItem = _navigationStack[0];
        RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));

        return true;
    }

    /// <summary>Provides the AddToJournal member.</summary>
    /// <param name="viewItem">The viewItem value.</param>
    /// <param name="isJournalNavigation">The isJournalNavigation value.</param>
    /// <param name="journalIndex">The journalIndex value.</param>
    private void AddToJournal(
        INavigationViewItem viewItem,
        bool isJournalNavigation,
        int journalIndex)
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

        if (_serviceProvider is not null)
        {
            return _serviceProvider.GetService(viewItem.TargetPageType)
                ?? throw new InvalidOperationException(
                    $"GetService returned null for {viewItem.TargetPageType}");
        }

        if (_pageService is not null)
        {
            return _pageService.GetPage(viewItem.TargetPageType)
                ?? throw new InvalidOperationException(
                    $"GetPage returned null for {viewItem.TargetPageType}");
        }

        if (viewItem.TargetPageFactory is null)
        {
            throw new InvalidOperationException(
                $"No page service or AOT-safe page factory is configured for {viewItem.TargetPageType}.");
        }

        return _cache.Remember(
                viewItem.TargetPageType,
                viewItem.NavigationCacheMode,
                viewItem.TargetPageFactory)
            ?? throw new ArgumentNullException(
                $"Unable to create instance of {viewItem.TargetPageType}");
    }

    /// <summary>Provides the UpdateContent member.</summary>
    /// <param name="content">The content value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    private void UpdateContent(object? content, object? dataContext = null)
    {
        if (dataContext is not null && content is Control control)
        {
            control.DataContext = dataContext;
        }

        if (_navigationViewContentPresenter is not null)
        {
            _navigationViewContentPresenter.Content = content;
        }

        Content = content;
    }

    /// <summary>Provides the OnMenuItemsCollectionChanged member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private void OnMenuItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AddItemsToDictionaries();
    }
}
