// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>The <see cref="BreadcrumbBar"/> control provides the direct path of pages or folders to the current location.</summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:BreadcrumbBar x:Name="BreadcrumbBar" /&gt;
/// </code>
/// </example>
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(BreadcrumbBarItem))]
public class BreadcrumbBar : System.Windows.Controls.ItemsControl, IUseHostedNavigation
{
    /// <summary>Property for <see cref="Command"/>.</summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(BreadcrumbBar),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="TemplateButtonCommand"/>.</summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty = DependencyProperty.Register(
        nameof(TemplateButtonCommand),
        typeof(IReactiveCommand),
        typeof(BreadcrumbBar),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="ItemClicked"/>.</summary>
    public static readonly RoutedEvent ItemClickedRoutedEvent = EventManager.RegisterRoutedEvent(
        nameof(ItemClicked),
        RoutingStrategy.Bubble,
        typeof(EventHandler<BreadcrumbBarItemClickedEventArgs>),
        typeof(BreadcrumbBar));

    /// <summary>The offset from the end used to update the item before the current last item.</summary>
    private const int PreviousItemOffset = 2;

    /// <summary>Stores the _hostName value.</summary>
    private string? _hostName;

    /// <summary>Initializes a new instance of the <see cref="BreadcrumbBar"/> class.</summary>
    public BreadcrumbBar()
    {
        SetValue(TemplateButtonCommandProperty, ReactiveCommand.Create<object?>(OnTemplateButtonClick));
        Loaded += (_, e) => OnLoaded(e);
        Unloaded += (_, e) => OnUnloaded(e);
    }

    /// <summary>Occurs when an item is clicked in the <see cref="BreadcrumbBar"/>.</summary>
    public event EventHandler<BreadcrumbBarItemClickedEventArgs> ItemClicked
    {
        add => AddHandler(ItemClickedRoutedEvent, value);
        remove => RemoveHandler(ItemClickedRoutedEvent, value);
    }

    /// <summary>Gets the <see cref="ReactiveCommand{Tin, Tout}"/> triggered after clicking.</summary>
    public IReactiveCommand TemplateButtonCommand => (IReactiveCommand)GetValue(TemplateButtonCommandProperty);

    /// <summary>Gets or sets custom command executed after selecting the item.</summary>
    [Bindable(true)]
    [Category("Action")]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Setups the navigation.</summary>
    /// <param name="hostName">Name of the host.</param>
    public void SetupNavigation(string hostName)
    {
        _hostName = hostName;
        ItemClicked += (_, args) =>
        {
            if (args.Item is not BreadcrumbBarItem item)
            {
                return;
            }

            NavigateTo(item.NavigationType);
        };
    }

    /// <summary>Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.</summary>
    /// <typeparam name="T">Type of ViewModel.</typeparam>
    /// <param name="contract">The viewmodel contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="breadcrumbItemContent">Content of the breadcrumb item.</param>
    public void NavigateTo<T>(string? contract = null, object? parameter = null, string? breadcrumbItemContent = null)
        where T : class, IRxObject
    {
        if (string.IsNullOrEmpty(_hostName))
        {
            throw new InvalidOperationException("Host name is not set. Call SetupNavigation and pass the Host Name of the Navigation host.");
        }

        UpdateItems(typeof(T), breadcrumbItemContent);

        this.NavigateToView<T>(_hostName, contract, parameter);
    }

    /// <summary>Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.</summary>
    /// <exception cref="System.ArgumentNullException">type.</exception>
    /// <param name="type">Type of ViewModel.</param>
    /// <param name="contract">The viewmodel contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="breadcrumbItemContent">Content of the breadcrumb item.</param>
    public void NavigateTo(Type type, string? contract = null, object? parameter = null, string? breadcrumbItemContent = null)
    {
        if (string.IsNullOrEmpty(_hostName))
        {
            throw new InvalidOperationException("Host name is not set. Call SetupNavigation and pass the Host Name of the Navigation host.");
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        UpdateItems(type, breadcrumbItemContent);

        this.NavigateToView(type, _hostName, contract, parameter);
    }

    /// <summary>Navigates back and updates the Breadcrumb to remove the last item.</summary>
    /// <exception cref="System.InvalidOperationException">Host name is not set. Call SetupNavigation and pass the Host Name of the Navigation host.</exception>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The target ViewModel.</returns>
    public IRxObject? NavigateBack(object? parameter = null)
    {
        if (string.IsNullOrEmpty(_hostName))
        {
            throw new InvalidOperationException("Host name is not set. Call SetupNavigation and pass the Host Name of the Navigation host.");
        }

        if (Items.Count == 0)
        {
            return null;
        }

        var vm = this.NavigateBack(_hostName, parameter);
        if (vm is not null)
        {
            UpdateItems(vm.GetType(), vm.DisplayName);
        }

        return vm;
    }

    /// <summary>Called when [item clicked].</summary>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    protected virtual void OnItemClicked(object item, int index)
    {
        var args = new BreadcrumbBarItemClickedEventArgs(ItemClickedRoutedEvent, this, item, index);
        RaiseEvent(args);

        if (Command?.CanExecute(item) ?? false)
        {
            Command.Execute(item);
        }

        if (Command?.CanExecute(null) != true)
        {
            return;
        }

        Command.Execute(null);
    }

    /// <summary>Determines if the specified item is (or is eligible to be) its own container.</summary>
    /// <param name="item">The item to check.</param>
    /// <returns>
    /// true if the item is (or is eligible to be) its own container; otherwise, false.
    /// </returns>
    protected override bool IsItemItsOwnContainerOverride(object item) => item is BreadcrumbBarItem;

    /// <summary>Creates or identifies the element that is used to display the given item.</summary>
    /// <returns>
    /// The element that is used to display the given item.
    /// </returns>
    protected override DependencyObject GetContainerForItemOverride() => new BreadcrumbBarItem();

    /// <summary>Provides the UpdateItems member.</summary>
    /// <param name="typeName">The typeName value.</param>
    /// <param name="content">The content value.</param>
    private void UpdateItems(Type typeName, string? content = null)
    {
        var list = Items.OfType<BreadcrumbBarItem>().ToList().Where(x => x.NavigationType == typeName).ToList();
        if (list.Count != 0)
        {
            var index = Items.IndexOf(list[0]);
            for (var i = Items.Count - 1; i > index; i--)
            {
                Items.RemoveAt(i);
            }
        }
        else
        {
            _ = Items.Add(new BreadcrumbBarItem { NavigationType = typeName, Content = content ?? typeName.Name });
        }
    }

    /// <summary>Provides the OnLoaded member.</summary>
    /// <param name="e">The event arguments.</param>
    private void OnLoaded(RoutedEventArgs e)
    {
        _ = e;
        ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorOnItemsChanged;
        ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;

        UpdateLastContainer();
    }

    /// <summary>Provides the OnUnloaded member.</summary>
    /// <param name="e">The event arguments.</param>
    private void OnUnloaded(RoutedEventArgs e)
    {
        _ = e;
        ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorOnItemsChanged;
        ItemContainerGenerator.StatusChanged -= ItemContainerGeneratorOnStatusChanged;
    }

    /// <summary>Provides the ItemContainerGeneratorOnStatusChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void ItemContainerGeneratorOnStatusChanged(object? sender, EventArgs e)
    {
        if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        {
            return;
        }

        if (ItemContainerGenerator.Items.Count <= 1)
        {
            UpdateLastContainer();
            return;
        }

        InteractWithItemContainer(PreviousItemOffset, static item => item.IsLast = false);
        UpdateLastContainer();
    }

    /// <summary>Provides the ItemContainerGeneratorOnItemsChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void ItemContainerGeneratorOnItemsChanged(object sender, ItemsChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Remove)
        {
            return;
        }

        UpdateLastContainer();
    }

    /// <summary>Provides the OnTemplateButtonClick member.</summary>
    /// <param name="obj">The obj value.</param>
    private void OnTemplateButtonClick(object? obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException("Item content is null");
        }

        var container = ItemContainerGenerator.ContainerFromItem(obj);
        var index = container is null ? -1 : ItemContainerGenerator.IndexFromContainer(container);

        OnItemClicked(obj, index);
    }

    /// <summary>Provides the InteractWithItemContainer member.</summary>
    /// <param name="offsetFromEnd">The offsetFromEnd value.</param>
    /// <param name="action">The action value.</param>
    private void InteractWithItemContainer(int offsetFromEnd, Action<BreadcrumbBarItem> action)
    {
        if (ItemContainerGenerator.Items.Count == 0)
        {
            return;
        }

        var item = ItemContainerGenerator.Items[^offsetFromEnd];
        var container = (BreadcrumbBarItem)ItemContainerGenerator.ContainerFromItem(item);

        action.Invoke(container);
    }

    /// <summary>Provides the UpdateLastContainer member.</summary>
    private void UpdateLastContainer() => InteractWithItemContainer(1, static item => item.IsLast = true);
}
