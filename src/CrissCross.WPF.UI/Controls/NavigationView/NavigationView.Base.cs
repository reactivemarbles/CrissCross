// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a container that enables navigation of app content. It has a header, a view for the main content, and a menu pane for navigation commands.
/// </summary>
[ToolboxItem(true)]
[System.Drawing.ToolboxBitmap(typeof(NavigationView), "NavigationView.bmp")]
public partial class NavigationView : System.Windows.Controls.Control, INavigationView
{
    /// <summary>
    /// The page identifier or target tag navigation views dictionary.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected Dictionary<string, INavigationViewItem> PageIdOrTargetTagNavigationViewsDictionary = [];

    /// <summary>
    /// The page type navigation views dictionary.
    /// </summary>
    protected Dictionary<Type, INavigationViewItem> PageTypeNavigationViewsDictionary = [];
#pragma warning restore SA1401 // Fields should be private

    private static readonly Thickness titleBarPaneOpenMargin = new(35, 0, 0, 0);
    private static readonly Thickness titleBarPaneCompactMargin = new(55, 0, 0, 0);
    private static readonly Thickness autoSuggestBoxMargin = new(8, 8, 8, 16);
    private static readonly Thickness frameMargin = new(0, 50, 0, 0);
    private readonly ObservableCollection<string> _autoSuggestBoxItems = [];
    private readonly ObservableCollection<NavigationViewBreadcrumbItem> _breadcrumbBarItems = [];

    /// <summary>
    /// Initializes static members of the <see cref="NavigationView"/> class.
    /// </summary>
    static NavigationView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationView),
            new FrameworkPropertyMetadata(typeof(NavigationView)));
        MarginProperty.OverrideMetadata(
            typeof(NavigationView),
            new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationView"/> class.
    /// </summary>
    public NavigationView()
    {
        NavigationParent = this;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;

        // Initialize MenuItems collection
        var menuItems = new ObservableCollection<object>();
        menuItems.CollectionChanged += OnMenuItems_CollectionChanged;
        SetValue(MenuItemsPropertyKey, menuItems);

        var footerMenuItems = new ObservableCollection<object>();
        footerMenuItems.CollectionChanged += OnMenuItems_CollectionChanged;
        SetValue(FooterMenuItemsPropertyKey, footerMenuItems);
    }

    /// <inheritdoc/>
    public INavigationViewItem? SelectedItem { get; protected set; }

    ////internal void ToggleAllExpands()
    ////{
    ////    // TODO: When shift clicked on navigationviewitem
    ////}

    internal void OnNavigationViewItemClick(NavigationViewItem navigationViewItem)
    {
        OnItemInvoked();

        NavigateInternal(navigationViewItem);
    }

    /// <summary>
    /// Updates the state of the visual.
    /// </summary>
    /// <param name="navigationView">The navigation view.</param>
    protected static void UpdateVisualState(NavigationView navigationView)
    {
        // Skip display modes that don't have multiple states
        if (navigationView == null || navigationView.PaneDisplayMode is
            NavigationViewPaneDisplayMode.LeftFluent or
            NavigationViewPaneDisplayMode.Top or
            NavigationViewPaneDisplayMode.Bottom)
        {
            return;
        }

        _ = VisualStateManager.GoToState(navigationView, navigationView.IsPaneOpen ? "PaneOpen" : "PaneCompact", true);
    }

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        NavigationStack.CollectionChanged += NavigationStackOnCollectionChanged;

        InvalidateArrange();
        InvalidateVisual();
        UpdateLayout();

        UpdateAutoSuggestBoxSuggestions();

        AddItemsToDictionaries();
    }

    /// <summary>
    /// This virtual method is called when this element is detached form a loaded tree.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
        SizeChanged -= OnSizeChanged;

        NavigationStack.CollectionChanged -= NavigationStackOnCollectionChanged;

        PageIdOrTargetTagNavigationViewsDictionary.Clear();
        PageTypeNavigationViewsDictionary.Clear();

        ClearJournal();

        if (AutoSuggestBox is not null)
        {
            AutoSuggestBox.SuggestionChosen -= AutoSuggestBoxOnSuggestionChosen;
            AutoSuggestBox.QuerySubmitted -= AutoSuggestBoxOnQuerySubmitted;
        }

        if (Header is BreadcrumbBar breadcrumbBar)
        {
            breadcrumbBar.ItemClicked -= BreadcrumbBarOnItemClicked;
        }

        if (ToggleButton is not null)
        {
            ToggleButton.Click -= OnToggleButtonClick;
        }

        if (BackButton is not null)
        {
            BackButton.Click -= OnToggleButtonClick;
        }

        if (AutoSuggestBoxSymbolButton is not null)
        {
            AutoSuggestBoxSymbolButton.Click -= AutoSuggestBoxSymbolButtonOnClick;
        }
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        // Back button
        if (e?.ChangedButton is MouseButton.XButton1)
        {
            GoBack();
            e.Handled = true;
        }

        base.OnMouseDown(e);
    }

    /// <summary>
    /// This virtual method is called when ActualWidth or ActualHeight (or both) changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // TODO: Update reveal
    }

    /// <summary>
    /// This virtual method is called when <see cref="BackButton" /> is clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnBackButtonClick(object sender, RoutedEventArgs e) => GoBack();

    /// <summary>
    /// This virtual method is called when <see cref="ToggleButton" /> is clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnToggleButtonClick(object sender, RoutedEventArgs e) => IsPaneOpen = !IsPaneOpen;

    /// <summary>
    /// This virtual method is called when <see cref="AutoSuggestBoxSymbolButton" /> is clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void AutoSuggestBoxSymbolButtonOnClick(object sender, RoutedEventArgs e)
    {
        IsPaneOpen = !IsPaneOpen;
        AutoSuggestBox?.Focus();
    }

    /// <summary>
    /// This virtual method is called when <see cref="PaneDisplayMode"/> is changed.
    /// </summary>
    protected virtual void OnPaneDisplayModeChanged()
    {
        switch (PaneDisplayMode)
        {
            case NavigationViewPaneDisplayMode.LeftFluent:
                IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
                IsPaneToggleVisible = false;
                break;
        }
    }

    /// <summary>
    /// This virtual method is called when <see cref="ItemTemplate"/> is changed.
    /// </summary>
    protected virtual void OnItemTemplateChanged() => UpdateMenuItemsTemplate();

    /// <summary>
    /// Breadcrumbs the bar on item clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="BreadcrumbBarItemClickedEventArgs"/> instance containing the event data.</param>
    protected virtual void BreadcrumbBarOnItemClicked(
        BreadcrumbBar sender,
        BreadcrumbBarItemClickedEventArgs e)
    {
        if (e?.Item is not NavigationViewBreadcrumbItem item)
        {
            return;
        }

        Navigate(item.PageId);
    }

    /// <summary>
    /// Adds the items to dictionaries.
    /// </summary>
    /// <param name="list">The list.</param>
    protected virtual void AddItemsToDictionaries(IEnumerable list)
    {
        if (list is null)
        {
            return;
        }

        foreach (var singleNavigationViewItem in list.OfType<NavigationViewItem>())
        {
            if (!PageIdOrTargetTagNavigationViewsDictionary.ContainsKey(singleNavigationViewItem.Id))
            {
                PageIdOrTargetTagNavigationViewsDictionary.Add(
                    singleNavigationViewItem.Id,
                    singleNavigationViewItem);
            }

            if (
                !PageIdOrTargetTagNavigationViewsDictionary.ContainsKey(
                    singleNavigationViewItem.TargetPageTag))
            {
                PageIdOrTargetTagNavigationViewsDictionary.Add(
                    singleNavigationViewItem.TargetPageTag,
                    singleNavigationViewItem);
            }

            if (
                singleNavigationViewItem.TargetPageType is not null
                && !PageTypeNavigationViewsDictionary.ContainsKey(singleNavigationViewItem.TargetPageType))
            {
                PageTypeNavigationViewsDictionary.Add(
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

    /// <summary>
    /// Adds the items to dictionaries.
    /// </summary>
    protected virtual void AddItemsToDictionaries()
    {
        AddItemsToDictionaries(MenuItems);
        AddItemsToDictionaries(FooterMenuItems);
    }

    /// <summary>
    /// Adds the items to automatic suggest box items.
    /// </summary>
    /// <param name="list">The list.</param>
    protected virtual void AddItemsToAutoSuggestBoxItems(IEnumerable list)
    {
        if (list is null)
        {
            return;
        }

        foreach (var singleNavigationViewItem in list.OfType<NavigationViewItem>())
        {
            if (
                singleNavigationViewItem is { Content: string content, TargetPageType: { } }
                && !string.IsNullOrWhiteSpace(content))
            {
                _autoSuggestBoxItems.Add(content);
            }

            if (singleNavigationViewItem.HasMenuItems)
            {
                AddItemsToAutoSuggestBoxItems(singleNavigationViewItem.MenuItems);
            }
        }
    }

    /// <summary>
    /// Adds the items to automatic suggest box items.
    /// </summary>
    protected virtual void AddItemsToAutoSuggestBoxItems()
    {
        AddItemsToAutoSuggestBoxItems(MenuItems);
        AddItemsToAutoSuggestBoxItems(FooterMenuItems);
    }

    /// <summary>
    /// Navigates to menu item from automatic suggest box.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="selectedSuggestBoxItem">The selected suggest box item.</param>
    /// <returns>A bool.</returns>
    protected virtual bool NavigateToMenuItemFromAutoSuggestBox(IEnumerable list, string selectedSuggestBoxItem)
    {
        if (list is null)
        {
            return false;
        }

        foreach (var singleNavigationViewItem in list.OfType<NavigationViewItem>())
        {
            if (singleNavigationViewItem.Content is string content && content == selectedSuggestBoxItem)
            {
                NavigateInternal(singleNavigationViewItem);
                singleNavigationViewItem.BringIntoView();
                singleNavigationViewItem.Focus(); // TODO: Element or content?

                return true;
            }

            if (NavigateToMenuItemFromAutoSuggestBox(singleNavigationViewItem.MenuItems, selectedSuggestBoxItem))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates the menu items template.
    /// </summary>
    /// <param name="list">The list.</param>
    protected virtual void UpdateMenuItemsTemplate(IList list)
    {
        if (list is null)
        {
            return;
        }

        for (var i = 0; i < list.Count; i++)
        {
            var singleMenuItem = list[i];

            if (singleMenuItem is not NavigationViewItem singleNavigationViewItem)
            {
                continue;
            }

            if (ItemTemplate is not null && singleNavigationViewItem.Template != ItemTemplate)
            {
                singleNavigationViewItem.Template = ItemTemplate;
            }
        }
    }

    /// <summary>
    /// Updates the menu items template.
    /// </summary>
    protected virtual void UpdateMenuItemsTemplate()
    {
        UpdateMenuItemsTemplate(MenuItems);
        UpdateMenuItemsTemplate(FooterMenuItems);
    }

    /// <summary>
    /// Closes the navigation view item menus.
    /// </summary>
    protected virtual void CloseNavigationViewItemMenus()
    {
        if (Journal.Count == 0 || IsPaneOpen)
        {
            return;
        }

        DeactivateMenuItems(MenuItems);
        DeactivateMenuItems(FooterMenuItems);

        var currentItem = PageIdOrTargetTagNavigationViewsDictionary[Journal[^1]];
        if (currentItem.NavigationViewItemParent is null)
        {
            currentItem.Activate(this);
            return;
        }

        currentItem.Deactivate(this);
        currentItem.NavigationViewItemParent?.Activate(this);
    }

    /// <summary>
    /// Deactivates the menu items.
    /// </summary>
    /// <param name="list">The list.</param>
    protected void DeactivateMenuItems(IEnumerable list)
    {
        if (list is null)
        {
            return;
        }

        foreach (var item in list)
        {
            if (item is NavigationViewItem singleNavigationViewItem)
            {
                singleNavigationViewItem.Deactivate(this);
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => UpdateVisualState((NavigationView)sender);

    private void UpdateAutoSuggestBoxSuggestions()
    {
        if (AutoSuggestBox == null)
        {
            return;
        }

        _autoSuggestBoxItems.Clear();

        AddItemsToAutoSuggestBoxItems();
    }

    /// <summary>
    /// Navigate to the page after its name is selected in <see cref="AutoSuggestBox"/>.
    /// </summary>
    private void AutoSuggestBoxOnSuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (sender.IsSuggestionListOpen)
        {
            return;
        }

        if (args.SelectedItem is not string selectedSuggestBoxItem)
        {
            return;
        }

        if (NavigateToMenuItemFromAutoSuggestBox(MenuItems, selectedSuggestBoxItem))
        {
            return;
        }

        NavigateToMenuItemFromAutoSuggestBox(FooterMenuItems, selectedSuggestBoxItem);
    }

    private void AutoSuggestBoxOnQuerySubmitted(
        AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var suggestions = new List<string>();
        var querySplit = args.QueryText.Split(' ');

        foreach (var item in _autoSuggestBoxItems)
        {
            var isMatch = true;

            foreach (var queryToken in querySplit)
            {
                if (item.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    isMatch = false;
                }
            }

            if (isMatch)
            {
                suggestions.Add(item);
            }
        }

        if (suggestions.Count == 0)
        {
            return;
        }

        var element = suggestions[0];

        if (NavigateToMenuItemFromAutoSuggestBox(MenuItems, element))
        {
            return;
        }

        NavigateToMenuItemFromAutoSuggestBox(FooterMenuItems, element);
    }

    [DebuggerStepThrough]
    private void NavigationStackOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                _breadcrumbBarItems.Add(
                    new NavigationViewBreadcrumbItem((INavigationViewItem)e.NewItems![0]!));
                break;
            case NotifyCollectionChangedAction.Remove:
                _breadcrumbBarItems.RemoveAt(e.OldStartingIndex);
                break;
            case NotifyCollectionChangedAction.Replace:
                _breadcrumbBarItems[0] = new NavigationViewBreadcrumbItem(
                    (INavigationViewItem)e.NewItems![0]!);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                _breadcrumbBarItems.Clear();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
