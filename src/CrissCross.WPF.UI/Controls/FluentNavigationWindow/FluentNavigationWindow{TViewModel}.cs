// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Navigation Window.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="FluentNavigationWindow" />
/// <seealso cref="IViewFor&lt;TViewModel&gt;" />
public class FluentNavigationWindow<TViewModel> : FluentNavigationWindow, IViewFor<TViewModel>
    where TViewModel : class, IRxObject, new()
{
    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(FluentNavigationWindow<TViewModel>),
            new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentNavigationWindow{TViewModel}"/> class.
    /// </summary>
    public FluentNavigationWindow() =>
        this.WhenActivated(_ => ViewModel ??= Locator.Current.GetService<TViewModel>() ?? new());

    /// <summary>
    /// Gets the binding root view model.
    /// </summary>
    public TViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }
}
