// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents NavigationModel.</summary>
public partial class NavigationModel : RxObject
{
    /// <summary>Stores the _viewModel value.</summary>
    private readonly Type? _viewModel;

    /// <summary>Stores the _navigationModels value.</summary>
    private readonly ICollection<NavigationModel> _navigationModels;

    /// <summary>Stores the _navigationService value.</summary>
    private readonly IUseHostedNavigation _navigationService;

    /// <summary>Gets or sets the name.</summary>
    /// <value>
    /// The name.
    /// </value>
    [Reactive]
    private string _name = string.Empty;

    /// <summary>Gets or sets the icon.</summary>
    /// <value>
    /// The icon.
    /// </value>
    [Reactive]
    private IconElement? _icon;

    /// <summary>Gets or sets the view.</summary>
    /// <value>
    /// The view.
    /// </value>
    [Reactive]
    private bool _isExpander;

    /// <summary>Gets or sets a value indicating whether this instance is selected.</summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _isSelected;

    /// <summary>Stores the _isExpanded value.</summary>
    [Reactive]
    private bool _isExpanded = true;

    /// <summary>Stores the _navigationHost value.</summary>
    [Reactive]
    private string _navigationHost = "mainWindow";

    /// <summary>Stores the _visibility value.</summary>
    [Reactive]
    private Visibility _visibility = Visibility.Visible;

    /// <summary>Stores the _parameter value.</summary>
    [Reactive]
    private object? _parameter;

    /// <summary>Initializes a new instance of the <see cref="NavigationModel" /> class.</summary>
    /// <exception cref="System.ArgumentNullException">navigationModels.</exception>
    /// <param name="viewModel">The view model.</param>
    /// <param name="navigationModels">The navigation models.</param>
    /// <param name="navigationService">The navigation service.</param>
    public NavigationModel(Type? viewModel, ICollection<NavigationModel> navigationModels, IUseHostedNavigation? navigationService = null)
    {
        InitializeOAPH();
        _viewModel = viewModel;
        _navigationModels = navigationModels ?? throw new ArgumentNullException(nameof(navigationModels));
        _navigationService = navigationService ?? this;
        NavigateCommand = ReactiveCommand.Create(Navigate);
    }

    /// <summary>Gets the command that invokes navigation for this model.</summary>
    public ReactiveCommand<Unit, Unit> NavigateCommand { get; }

    /// <summary>Determines whether [is name visible].</summary>
    /// <returns>
    ///   <c>true</c> if [is name visible]; otherwise, <c>false</c>.
    /// </returns>
    [ObservableAsProperty]
    private IObservable<Visibility> IsNameVisible() => this.WhenAnyValue(x => x.IsExpanded, x => x.IsExpander).Select(x => (x.Value1 && !x.Value2) ? Visibility.Visible : Visibility.Collapsed);

    /// <summary>Provides the IsSelectedVisible member.</summary>
    /// <returns>The result.</returns>
    [ObservableAsProperty]
    private IObservable<Visibility> IsSelectedVisible() => this.WhenAnyValue(x => x.IsSelected).Select(x => x ? Visibility.Visible : Visibility.Hidden);

    /// <summary>Provides the IsExpanderHorizontalAlignment member.</summary>
    /// <returns>The result.</returns>
    [ObservableAsProperty]
    private IObservable<HorizontalAlignment> IsExpanderHorizontalAlignment() => this.WhenAnyValue(x => x.IsExpander).Select(x => x ? HorizontalAlignment.Left : HorizontalAlignment.Stretch);

    /// <summary>Provides the IsExpanderVerticalAlignment member.</summary>
    /// <returns>The result.</returns>
    [ObservableAsProperty]
    private IObservable<VerticalAlignment> IsExpanderVerticalAlignment() => this.WhenAnyValue(x => x.IsExpander).Select(x => x ? VerticalAlignment.Top : VerticalAlignment.Stretch);

    /// <summary>Provides the Navigate member.</summary>
    private void Navigate()
    {
        if (_viewModel is null)
        {
            if (IsExpander)
            {
                foreach (var item in _navigationModels)
                {
                    item.IsExpanded = !item.IsExpanded;
                }
            }

            return;
        }

        foreach (var item in _navigationModels)
        {
            item.IsSelected = false;
        }

        if (_navigationService is BreadcrumbBar breadcrumbBar)
        {
            breadcrumbBar.NavigateTo(_viewModel, parameter: Parameter, breadcrumbItemContent: Name);
        }
        else
        {
            _navigationService.NavigateToView(_viewModel, hostName: NavigationHost, parameter: Parameter);
        }

        IsSelected = true;
    }
}
