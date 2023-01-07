// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF
{
    /// <summary>
    /// Navigation Window.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="CrissCross.WPF.NavigationWindow" />
    /// <seealso cref="ReactiveUI.IViewFor&lt;TViewModel&gt;" />
    public class NavigationWindow<TViewModel> : NavigationWindow, IViewFor<TViewModel>
        where TViewModel : class, IRxObject, new()
    {
        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(TViewModel),
                typeof(NavigationWindow<TViewModel>),
                new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationWindow{TViewModel}"/> class.
        /// </summary>
        public NavigationWindow()
        {
            this.WhenActivated(d =>
            {
                ViewModel ??= Locator.Current.GetService<TViewModel>() ?? new();
            });
        }

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
}