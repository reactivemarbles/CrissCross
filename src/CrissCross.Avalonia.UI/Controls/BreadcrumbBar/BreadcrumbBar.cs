// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// The <see cref="BreadcrumbBar"/> control provides the direct path of pages or folders to the current location.
/// </summary>
public partial class BreadcrumbBar : ItemsControl, IUseHostedNavigation
{
    /// <summary>
    /// Property for <see cref="Command"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<BreadcrumbBar, ICommand>(
        nameof(Command));

    /// <summary>
    /// Property for <see cref="TemplateButtonCommand"/>.
    /// </summary>
    public static readonly StyledProperty<IReactiveCommand> TemplateButtonCommandProperty = AvaloniaProperty.Register<BreadcrumbBar, IReactiveCommand>(
        nameof(TemplateButtonCommand));

    /// <summary>
    /// Property for <see cref="ItemClicked"/>.
    /// </summary>
    public static readonly RoutedEvent ItemClickedEvent = RoutedEvent.Register<BreadcrumbBarItemClickedEventArgs>(
        nameof(ItemClicked),
        RoutingStrategies.Bubble,
        typeof(BreadcrumbBar));

    private readonly CompositeDisposable _disposables = [];
    private string? _hostName;

    /// <summary>
    /// Initializes a new instance of the <see cref="BreadcrumbBar"/> class.
    /// </summary>
    public BreadcrumbBar()
    {
        SetValue(TemplateButtonCommandProperty, OnTemplateButtonClickCommand);
        this.Events().Loaded.Subscribe(OnLoaded).DisposeWith(_disposables);
        this.Events().Unloaded.Subscribe(OnUnloaded).DisposeWith(_disposables);
    }

    /// <summary>
    /// Occurs when an item is clicked in the <see cref="BreadcrumbBar"/>.
    /// </summary>
    public event EventHandler<BreadcrumbBarItemClickedEventArgs> ItemClicked
    {
        add => AddHandler(ItemClickedEvent, value);
        remove => RemoveHandler(ItemClickedEvent, value);
    }

    /// <summary>
    /// Gets the <see cref="ReactiveCommand{Tin, Tout}"/> triggered after clicking.
    /// </summary>
    public IReactiveCommand TemplateButtonCommand => GetValue(TemplateButtonCommandProperty);

    /// <summary>
    /// Gets or sets custom command executed after selecting the item.
    /// </summary>
    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Setups the navigation.
    /// </summary>
    /// <param name="hostName">Name of the host.</param>
    public void SetupNavigation(string hostName)
    {
        _hostName = hostName;
    }

    /// <summary>
    /// Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.
    /// </summary>
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

    /// <summary>
    /// Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.
    /// </summary>
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

    /// <summary>
    /// Navigates back and updates the Breadcrumb to remove the last item.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The target ViewModel.</returns>
    public IRxObject? NavigateBack(object? parameter = null)
    {
        if (string.IsNullOrEmpty(_hostName))
        {
            throw new InvalidOperationException("Host name is not set. Call SetupNavigation and pass the Host Name of the Navigation host.");
        }

        if (Items.Count != 0)
        {
            var vm = this.NavigateBack(_hostName, parameter);
            if (vm != null)
            {
                UpdateItems(vm.GetType(), vm.DisplayName);
            }

            return vm;
        }

        return null;
    }

    /// <summary>
    /// Called when an item is clicked in the <see cref="BreadcrumbBar"/>.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    protected virtual void OnItemClicked(object item, int index)
    {
        var args = new BreadcrumbBarItemClickedEventArgs(ItemClickedEvent, this, item, index);
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
    /// Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    /// <param name="recycleKey">The recycle key.</param>
    /// <returns>
    /// The element that is used to display the given item.
    /// </returns>
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new BreadcrumbBarItem();

    /// <summary>
    /// Called when the <see cref="BreadcrumbBar"/> is loaded.
    /// </summary>
    /// <param name="e">The routed event arguments.</param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        this.Events().PointerPressed.Subscribe(args => { });

        // ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorOnItemsChanged;
        // ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;

        // UpdateLastContainer();
    }

    /// <summary>
    /// Called when the <see cref="BreadcrumbBar"/> is unloaded.
    /// </summary>
    /// <param name="e">The routed event arguments.</param>
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _disposables.Dispose();

        // ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorOnItemsChanged;
        // ItemContainerGenerator.StatusChanged -= ItemContainerGeneratorOnStatusChanged;
    }

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
            Items.Add(new BreadcrumbBarItem { NavigationType = typeName, Content = content ?? typeName.Name });
        }
    }

    [ReactiveCommand]
    private void OnTemplateButtonClick(object? obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException("Item content is null");
        }

        // var container = ItemContainerGenerator.ContainerFromItem(obj);
        // var index = container == null ? -1 : ItemContainerGenerator.IndexFromContainer(container);

        // OnItemClicked(obj, index);
    }

    //// private void InteractWithItemContainer(int offsetFromEnd, Action<BreadcrumbBarItem> action)
    //// {
    ////     if (ItemContainerGenerator.Items.Count == 0)
    ////     {
    ////         return;
    ////     }

    ////     var item = ItemContainerGenerator.Items[^offsetFromEnd];
    ////     var container = (BreadcrumbBarItem)ItemContainerGenerator.ContainerFromItem(item);

    ////     action.Invoke(container);
    //// }

    // private void UpdateLastContainer() => InteractWithItemContainer(1, static item => item.IsLast = true);
}
