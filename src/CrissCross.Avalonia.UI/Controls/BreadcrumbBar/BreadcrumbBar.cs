// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the BreadcrumbBar member.</summary>
public class BreadcrumbBar : ItemsControl, IUseHostedNavigation
{
    /// <summary>Property for <see cref="Command"/>.</summary>
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<
        BreadcrumbBar,
        ICommand
    >(nameof(Command));

    /// <summary>Property for <see cref="TemplateButtonCommand"/>.</summary>
    public static readonly StyledProperty<IReactiveCommand> TemplateButtonCommandProperty = AvaloniaProperty.Register<
        BreadcrumbBar,
        IReactiveCommand
    >(nameof(TemplateButtonCommand));

    /// <summary>Property for <see cref="ItemClicked"/>.</summary>
    public static readonly RoutedEvent ItemClickedEvent = RoutedEvent.Register<BreadcrumbBarItemClickedEventArgs>(
        nameof(ItemClicked),
        RoutingStrategies.Bubble,
        typeof(BreadcrumbBar));

    /// <summary>Error shown when navigation is used before host setup.</summary>
    private const string MissingHostNameMessage =
        "Host name is not set. Call SetupNavigation and pass the Host Name of the Navigation host.";

    /// <summary>Provides the _hostName member.</summary>
    private string? _hostName;

    /// <summary>Initializes a new instance of the <see cref="BreadcrumbBar"/> class.</summary>
    public BreadcrumbBar()
    {
        _ = SetValue(
            TemplateButtonCommandProperty,
            ReactiveCommand.Create<object?>(content => ArgumentNullException.ThrowIfNull(content)));
    }

    /// <summary>Occurs when an item is clicked in the <see cref="BreadcrumbBar"/>.</summary>
    public event EventHandler<BreadcrumbBarItemClickedEventArgs> ItemClicked
    {
        add => AddHandler(ItemClickedEvent, value);
        remove => RemoveHandler(ItemClickedEvent, value);
    }

    /// <summary>Gets the <see cref="ReactiveCommand{Tin, Tout}"/> triggered after clicking.</summary>
    public IReactiveCommand TemplateButtonCommand => GetValue(TemplateButtonCommandProperty);

    /// <summary>Gets or sets custom command executed after selecting the item.</summary>
    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Setups the navigation.</summary>
    /// <param name="hostName">Name of the host.</param>
    public void SetupNavigation(string hostName)
    {
        _hostName = hostName;
    }

    /// <summary>Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.</summary>
    /// <param name="type">Type of ViewModel.</param>
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model type requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model type may require members removed by trimming.")]
    public void NavigateTo(Type type) => NavigateTo(type, null, null, null);

    /// <summary>Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.</summary>
    /// <param name="type">Type of ViewModel.</param>
    /// <param name="contract">The viewmodel contract.</param>
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model type requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model type may require members removed by trimming.")]
    public void NavigateTo(Type type, string? contract) => NavigateTo(type, contract, null, null);

    /// <summary>Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.</summary>
    /// <param name="type">Type of ViewModel.</param>
    /// <param name="contract">The viewmodel contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model type requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model type may require members removed by trimming.")]
    public void NavigateTo(Type type, string? contract, object? parameter) =>
        NavigateTo(type, contract, parameter, null);

    /// <summary>Navigates to the specified view as registered to the viewmodel and updates the Breadcrumb.</summary>
    /// <param name="type">Type of ViewModel.</param>
    /// <param name="contract">The viewmodel contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    /// <param name="breadcrumbItemContent">Content of the breadcrumb item.</param>
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model type requires runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model type may require members removed by trimming.")]
    public void NavigateTo(Type type, string? contract, object? parameter, string? breadcrumbItemContent)
    {
        if (string.IsNullOrEmpty(_hostName))
        {
            throw new InvalidOperationException(MissingHostNameMessage);
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        UpdateItems(type, breadcrumbItemContent);

        this.NavigateToView(
            type,
            new NavigationRequestOptions
            {
                HostName = _hostName,
                Contract = contract,
                Parameter = parameter,
            });
    }

    /// <summary>Navigates back and updates the Breadcrumb to remove the last item.</summary>
    /// <returns>The target ViewModel.</returns>
    public IRxObject? NavigateBack() => NavigateBack(null);

    /// <summary>Navigates back and updates the Breadcrumb to remove the last item.</summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The target ViewModel.</returns>
    public IRxObject? NavigateBack(object? parameter)
    {
        if (string.IsNullOrEmpty(_hostName))
        {
            throw new InvalidOperationException(MissingHostNameMessage);
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

    /// <summary>Called when an item is clicked in the <see cref="BreadcrumbBar"/>.</summary>
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

        if (Command?.CanExecute(null) != true)
        {
            return;
        }

        Command.Execute(null);
    }

    /// <summary>Creates or identifies the element that is used to display the given item.</summary>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    /// <param name="recycleKey">The recycle key.</param>
    /// <returns>
    /// The element that is used to display the given item.
    /// </returns>
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
        new BreadcrumbBarItem();

    /// <summary>Called when the <see cref="BreadcrumbBar"/> is loaded.</summary>
    /// <param name="e">The routed event arguments.</param>
    protected override void OnLoaded(RoutedEventArgs e) { }

    /// <summary>Called when the <see cref="BreadcrumbBar"/> is unloaded.</summary>
    /// <param name="e">The routed event arguments.</param>
    protected override void OnUnloaded(RoutedEventArgs e) { }

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

    //// private void InteractWithItemContainer(int offsetFromEnd, Action<BreadcrumbBarItem> action)
    ////     if (ItemContainerGenerator.Items.Count == 0)
}
