// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.WPF.UI;

/// <summary>
/// Navigation Window.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="ModernWindow" />
/// <seealso cref="IViewFor&lt;TViewModel&gt;" />
public class ModernWindow<TViewModel> : ModernWindow, IViewFor<TViewModel>
    where TViewModel : class, IRxObject, new()
{
    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ModernWindow<TViewModel>),
            new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="ModernWindow{TViewModel}"/> class.
    /// </summary>
    public ModernWindow() =>
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
