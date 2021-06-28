using ReactiveUI;
using System;
using System.Windows;
using static ReactiveUI.TransitioningContentControl;

namespace CrissCross.WPF
{
    /// <summary>
    /// Navigation Window.
    /// </summary>
    /// <seealso cref="Window" />
    /// <seealso cref="ISetNavigation" />
    /// <seealso cref="IUseNavigation" />
    /// <seealso cref="IActivatableView" />
    public class NavigationWindow : Window, ISetNavigation, IUseNavigation, IActivatableView
    {
        /// <summary>
        /// The transition property
        /// </summary>
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
            nameof(Transition),
            typeof(TransitionType),
            typeof(NavigationWindow),
            new PropertyMetadata(TransitionType.Fade));

        /// <summary>
        /// The navigate back is enabled property
        /// </summary>
        public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(
            nameof(NavigateBackIsEnabled),
            typeof(bool),
            typeof(NavigationWindow),
            new PropertyMetadata(true));

        /// <summary>
        /// The navigation frame property
        /// </summary>
        public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register(
            nameof(NavigationFrame),
            typeof(ViewModelRoutedViewHost),
            typeof(NavigationWindow));

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationWindow"/> class.
        /// </summary>
        public NavigationWindow()
        {
            DefaultStyleKey = typeof(NavigationWindow);
        }

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>
        /// The transition.
        /// </value>
        public TransitionType Transition
        {
            get => (TransitionType)GetValue(TransitionProperty);
            set => SetValue(TransitionProperty, value);
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
        /// Gets the can navigate back.
        /// </summary>
        /// <value>
        /// The can navigate back.
        /// </value>
        public IObservable<bool> CanNavigateBack => NavigationFrame.CanNavigateBackObservable;

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            NavigationFrame = (ViewModelRoutedViewHost)Template.FindName(nameof(NavigationFrame), this);
            if (NavigationFrame == null)
            {
                throw new Exception($"{nameof(NavigationFrame)} as a {nameof(ViewModelRoutedViewHost)} is missing from the Style template.");
            }

            NavigationFrame.HostName = Name;
            this.SetMainNavigationHost(NavigationFrame);
        }
    }
}