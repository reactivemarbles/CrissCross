// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CrissCross.WPF.UI.Input;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// The <see cref="BreadcrumbBar"/> control provides the direct path of pages or folders to the current location.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:BreadcrumbBar x:Name="BreadcrumbBar" /&gt;
/// </code>
/// </example>
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(BreadcrumbBarItem))]
public class BreadcrumbBar : System.Windows.Controls.ItemsControl
{
    /// <summary>
    /// Property for <see cref="Command"/>.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(BreadcrumbBar),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="TemplateButtonCommand"/>.
    /// </summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty = DependencyProperty.Register(
        nameof(TemplateButtonCommand),
        typeof(IRelayCommand),
        typeof(BreadcrumbBar),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="ItemClicked"/>.
    /// </summary>
    public static readonly RoutedEvent ItemClickedRoutedEvent = EventManager.RegisterRoutedEvent(
        nameof(ItemClicked),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<BreadcrumbBar, BreadcrumbBarItemClickedEventArgs>),
        typeof(BreadcrumbBar));

    /// <summary>
    /// Initializes a new instance of the <see cref="BreadcrumbBar"/> class.
    /// </summary>
    public BreadcrumbBar()
    {
        SetValue(TemplateButtonCommandProperty, new RelayCommand<object>(OnTemplateButtonClick));

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Occurs when an item is clicked in the <see cref="BreadcrumbBar"/>.
    /// </summary>
    public event TypedEventHandler<BreadcrumbBar, BreadcrumbBarItemClickedEventArgs> ItemClicked
    {
        add => AddHandler(ItemClickedRoutedEvent, value);
        remove => RemoveHandler(ItemClickedRoutedEvent, value);
    }

    /// <summary>
    /// Gets the <see cref="RelayCommand{T}"/> triggered after clicking.
    /// </summary>
    public IRelayCommand TemplateButtonCommand => (IRelayCommand)GetValue(TemplateButtonCommandProperty);

    /// <summary>
    /// Gets or sets custom command executed after selecting the item.
    /// </summary>
    [Bindable(true)]
    [Category("Action")]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Called when [item clicked].
    /// </summary>
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

        if (Command?.CanExecute(null) ?? false)
        {
            Command.Execute(null);
        }
    }

    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>
    /// true if the item is (or is eligible to be) its own container; otherwise, false.
    /// </returns>
    protected override bool IsItemItsOwnContainerOverride(object item) => item is BreadcrumbBarItem;

    /// <summary>
    /// Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <returns>
    /// The element that is used to display the given item.
    /// </returns>
    protected override DependencyObject GetContainerForItemOverride() => new BreadcrumbBarItem();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorOnItemsChanged;
        ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;

        UpdateLastContainer();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorOnItemsChanged;
        ItemContainerGenerator.StatusChanged -= ItemContainerGeneratorOnStatusChanged;
    }

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

        InteractWithItemContainer(2, static item => item.IsLast = false);
        UpdateLastContainer();
    }

    private void ItemContainerGeneratorOnItemsChanged(object sender, ItemsChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Remove)
        {
            return;
        }

        UpdateLastContainer();
    }

    private void OnTemplateButtonClick(object? obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException("Item content is null");
        }

        var container = ItemContainerGenerator.ContainerFromItem(obj);
        var index = container == null ? -1 : ItemContainerGenerator.IndexFromContainer(container);

        OnItemClicked(obj, index);
    }

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

    private void UpdateLastContainer() => InteractWithItemContainer(1, static item => item.IsLast = true);
}
