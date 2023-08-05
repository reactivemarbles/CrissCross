// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ReactiveUI;

namespace CrissCross.Avalonia
{
    /// <summary>
    /// NavigationWindow.
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    /// <seealso cref="ISetNavigation" />
    /// <seealso cref="IUseNavigation" />
    /// <seealso cref="IActivatableView" />
    public class NavigationWindow : Window, ISetNavigation, IUseNavigation, IActivatableView
    {
        /// <summary>
        /// The navigate back is enabled property.
        /// </summary>
        public static readonly StyledProperty<bool?> NavigateBackIsEnabledProperty =
            AvaloniaProperty.Register<NavigationWindow, bool?>(nameof(NavigateBackIsEnabled));

        /// <summary>
        /// The navigation frame property.
        /// </summary>
        public static readonly StyledProperty<ViewModelRoutedViewHost?> NavigationFrameProperty =
            AvaloniaProperty.Register<NavigationWindow, ViewModelRoutedViewHost?>(nameof(NavigationFrame));

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationWindow"/> class.
        /// </summary>
        public NavigationWindow()
        {
            ////NavigationFrame = new();
        }

        /// <summary>
        /// Gets the can navigate back.
        /// </summary>
        /// <value>
        /// The can navigate back.
        /// </value>
        public IObservable<bool?>? CanNavigateBack =>
            NavigationFrame?.CanNavigateBackObservable;

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool? NavigateBackIsEnabled
        {
            get => GetValue(NavigateBackIsEnabledProperty);
            set => SetValue(NavigateBackIsEnabledProperty, value);
        }

        /// <summary>
        /// Gets the navigation frame.
        /// </summary>
        /// <value>
        /// The navigation frame.
        /// </value>
        public ViewModelRoutedViewHost? NavigationFrame
        {
            get => GetValue(NavigationFrameProperty);
            private set => SetValue(NavigationFrameProperty, value);
        }

        /// <summary>
        /// Called when the control finishes initialization.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            NavigationFrame = this.FindControl<ViewModelRoutedViewHost>("PART_NavigationFrame");
            NavigationFrame!.HostName = Name;
            this.SetMainNavigationHost(NavigationFrame!);
        }
    }
}
