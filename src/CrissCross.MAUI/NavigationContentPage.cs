// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.MAUI
{
    /// <summary>
    /// NavigationContentPage.
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.ContentPage" />
    public class NavigationContentPage : ContentPage, ISetNavigation, IUseNavigation, IActivatableView
    {
        /// <summary>
        /// The navigate back is enabled property.
        /// </summary>
        public static readonly BindableProperty NavigateBackIsEnabledProperty = BindableProperty.Create(
            nameof(NavigateBackIsEnabled),
            typeof(bool),
            typeof(NavigationContentPage),
            false);

        /// <summary>
        /// The navigation frame property.
        /// </summary>
        public static readonly BindableProperty NavigationFrameProperty = BindableProperty.Create(
            nameof(NavigationFrame),
            typeof(ViewModelRoutedViewHost),
            typeof(NavigationContentPage));

        /// <summary>
        /// The host name property.
        /// </summary>
        public static readonly BindableProperty NameProperty = BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(ViewModelRoutedViewHost),
            string.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContentPage"/> class.
        /// </summary>
        public NavigationContentPage()
        {
            //// DefaultStyleKey = typeof(NavigationContentPage);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool NavigateBackIsEnabled
        {
            get => (bool)GetValue(NavigateBackIsEnabledProperty);
            set => SetValue(NavigateBackIsEnabledProperty, value);
        }

        /// <summary>
        /// Gets the navigation frame.
        /// </summary>
        /// <value>
        /// The navigation frame.
        /// </value>
        public ViewModelRoutedViewHost NavigationFrame
        {
            get => (ViewModelRoutedViewHost)GetValue(NavigationFrameProperty);
            private set => SetValue(NavigationFrameProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            NavigationFrame = (ViewModelRoutedViewHost)FindByName(nameof(NavigationFrame));
            if (NavigationFrame == null)
            {
                throw new Exception($"{nameof(NavigationFrame)} as a {nameof(ViewModelRoutedViewHost)} is missing from the Style template.");
            }

            NavigationFrame.HostName = Name;
            this.SetMainNavigationHost(NavigationFrame);
        }
    }
}
