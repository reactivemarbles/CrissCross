// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigationModel.
/// </summary>
public partial class NavigationModel : RxObject
{
    private readonly Type? _viewModel;
    private readonly ICollection<NavigationModel> _navigationModels;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    [Reactive]
    private string _name = string.Empty;

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    [Reactive]
    private IconElement? _icon;

    /// <summary>
    /// Gets or sets the view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    [Reactive]
    private bool _isExpander;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    [Reactive]
    private bool _isSelected;

    [Reactive]
    private bool _isExpanded = true;

    [Reactive]
    private string _navigationHost = "mainWindow";

    [Reactive]
    private Visibility _visibility = Visibility.Visible;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationModel"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="navigationModels">The navigation models.</param>
    /// <exception cref="System.ArgumentNullException">navigationModels.</exception>
    public NavigationModel(Type? viewModel, ICollection<NavigationModel> navigationModels)
    {
        InitializeOAPH();
        _viewModel = viewModel;
        _navigationModels = navigationModels ?? throw new ArgumentNullException(nameof(navigationModels));
    }

    /// <summary>
    /// Determines whether [is name visible].
    /// </summary>
    /// <returns>
    ///   <c>true</c> if [is name visible]; otherwise, <c>false</c>.
    /// </returns>
    [ObservableAsProperty]
    private IObservable<Visibility> IsNameVisible() => this.WhenAnyValue(x => x.IsExpanded, x => x.IsExpander).Select(x => (x.Item1 && !x.Item2) ? Visibility.Visible : Visibility.Collapsed);

    [ObservableAsProperty]
    private IObservable<Visibility> IsSelectedVisible() => this.WhenAnyValue(x => x.IsSelected).Select(x => x ? Visibility.Visible : Visibility.Hidden);

    [ObservableAsProperty]
    private IObservable<HorizontalAlignment> IsExpanderHorizontalAlignment() => this.WhenAnyValue(x => x.IsExpander).Select(x => x ? HorizontalAlignment.Left : HorizontalAlignment.Stretch);

    [ObservableAsProperty]
    private IObservable<VerticalAlignment> IsExpanderVerticalAlignment() => this.WhenAnyValue(x => x.IsExpander).Select(x => x ? VerticalAlignment.Top : VerticalAlignment.Stretch);

    [ReactiveCommand]
    private void Navigate()
    {
        if (_viewModel == null)
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

        this.NavigateToView(_viewModel, NavigationHost);
        IsSelected = true;
    }
}
