// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Avalonia;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia
{
    /// <summary>
    /// Navigation Window.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="NavigationWindow" />
    /// <seealso cref="IViewFor&lt;TViewModel&gt;" />
    public class NavigationWindow<TViewModel> : NavigationWindow, IViewFor<TViewModel>
        where TViewModel : class, IRxObject, new()
    {

        /// <summary>
        /// The view model dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1002", Justification = "Generic avalonia property is expected here.")]
        public static readonly StyledProperty<TViewModel?> ViewModelProperty = AvaloniaProperty
            .Register<NavigationWindow<TViewModel>, TViewModel?>(nameof(ViewModel));

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationWindow{TViewModel}"/> class.
        /// </summary>
        public NavigationWindow() =>
            this.WhenActivated(_ => ViewModel ??= Locator.Current.GetService<TViewModel>() ?? new());

        /// <summary>
        /// Gets the binding root view model.
        /// </summary>
        public TViewModel? BindingRoot => ViewModel;

        /// <inheritdoc/>
        public TViewModel? ViewModel
        {
            get => GetValue(ViewModelProperty);
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
