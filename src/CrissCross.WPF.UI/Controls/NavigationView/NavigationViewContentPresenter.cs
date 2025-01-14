// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using CrissCross.WPF.UI.Animations;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigationViewContentPresenter.
/// </summary>
/// <seealso cref="Frame" />
public class NavigationViewContentPresenter : Frame
{
    /// <summary>
    /// Property for <see cref="TransitionDuration"/>.
    /// </summary>
    public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register(
        nameof(TransitionDuration),
        typeof(int),
        typeof(NavigationViewContentPresenter),
        new FrameworkPropertyMetadata(200));

    /// <summary>
    /// Property for <see cref="Transition"/>.
    /// </summary>
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
        nameof(Transition),
        typeof(Transition),
        typeof(NavigationViewContentPresenter),
        new FrameworkPropertyMetadata(Transition.FadeInWithSlide));

    /// <summary>
    /// Property for <see cref="IsDynamicScrollViewerEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsDynamicScrollViewerEnabledProperty =
        DependencyProperty.Register(
            nameof(IsDynamicScrollViewerEnabled),
            typeof(bool),
            typeof(NavigationViewContentPresenter),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

    static NavigationViewContentPresenter()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationViewContentPresenter),
            new FrameworkPropertyMetadata(typeof(NavigationViewContentPresenter)));

        NavigationUIVisibilityProperty.OverrideMetadata(
            typeof(NavigationViewContentPresenter),
            new FrameworkPropertyMetadata(NavigationUIVisibility.Hidden));

        SandboxExternalContentProperty.OverrideMetadata(
            typeof(NavigationViewContentPresenter),
            new FrameworkPropertyMetadata(true));

        JournalOwnershipProperty.OverrideMetadata(
            typeof(NavigationViewContentPresenter),
            new FrameworkPropertyMetadata(JournalOwnership.UsesParentJournal));

        ScrollViewer.CanContentScrollProperty.OverrideMetadata(
            typeof(Page),
            new FrameworkPropertyMetadata(true));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationViewContentPresenter"/> class.
    /// </summary>
    public NavigationViewContentPresenter()
    {
        Navigating += static (sender, eventArgs) =>
        {
            if (eventArgs?.Content is null)
            {
                return;
            }

            var self = (NavigationViewContentPresenter)sender;
            self.OnNavigating(eventArgs);
        };

        Navigated += static (sender, eventArgs) =>
        {
            var self = (NavigationViewContentPresenter)sender;

            if (eventArgs.Content is null)
            {
                return;
            }

            self.OnNavigated(eventArgs);
        };
    }

    /// <summary>
    /// Gets or sets the duration of the transition.
    /// </summary>
    /// <value>
    /// The duration of the transition.
    /// </value>
    [Bindable(true)]
    [Category("Appearance")]
    public int TransitionDuration
    {
        get => (int)GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets type of <see cref="NavigationViewContentPresenter"/> transitions during navigation.
    /// </summary>
    public Transition Transition
    {
        get => (Transition)GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dynamic scroll viewer is enabled.
    /// </summary>
    public bool IsDynamicScrollViewerEnabled
    {
        get => (bool)GetValue(IsDynamicScrollViewerEnabledProperty);
        protected set => SetValue(IsDynamicScrollViewerEnabledProperty, value);
    }

    /// <summary>
    /// Raises the <see cref="E:Initialized" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        // I didn't understand something, but why is it necessary?
        Unloaded += static (sender, _) =>
        {
            if (sender is NavigationViewContentPresenter navigator)
            {
                NotifyContentAboutNavigatingFrom(navigator.Content);
            }
        };
    }

    /// <summary>
    /// Raises the <see cref="E:MouseDown" /> event.
    /// </summary>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e?.ChangedButton is MouseButton.XButton1 or MouseButton.XButton2)
        {
            e.Handled = true;
        }

        base.OnMouseDown(e);
    }

    /// <summary>
    /// Raises the <see cref="E:Navigating" /> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Navigation.NavigatingCancelEventArgs"/> instance containing the event data.</param>
    protected virtual void OnNavigating(System.Windows.Navigation.NavigatingCancelEventArgs eventArgs)
    {
        NotifyContentAboutNavigatingTo(eventArgs?.Content);

        if (eventArgs?.Navigator is not NavigationViewContentPresenter navigator)
        {
            return;
        }

        NotifyContentAboutNavigatingFrom(navigator.Content);
    }

    /// <summary>
    /// Raises the <see cref="E:Navigated" /> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
    protected virtual void OnNavigated(NavigationEventArgs eventArgs)
    {
        ApplyTransitionEffectToNavigatedPage(eventArgs?.Content);

        if (eventArgs?.Content is not DependencyObject dependencyObject)
        {
            return;
        }

        IsDynamicScrollViewerEnabled = ScrollViewer.GetCanContentScroll(dependencyObject);
    }

    private static void NotifyContentAboutNavigatingTo(object? content)
    {
        if (content is INavigationAware navigationAwareNavigationContent)
        {
            navigationAwareNavigationContent.OnNavigatedTo();
        }

        if (
            content is INavigableView<object>
            {
                ViewModel: INavigationAware navigationAwareNavigableViewViewModel
            })
        {
            navigationAwareNavigableViewViewModel.OnNavigatedTo();
        }

        if (content is FrameworkElement { DataContext: INavigationAware navigationAwareCurrentContent })
        {
            navigationAwareCurrentContent.OnNavigatedTo();
        }
    }

    private static void NotifyContentAboutNavigatingFrom(object content)
    {
        if (content is INavigationAware navigationAwareNavigationContent)
        {
            navigationAwareNavigationContent.OnNavigatedFrom();
        }

        if (
            content is INavigableView<object>
            {
                ViewModel: INavigationAware navigationAwareNavigableViewViewModel
            })
        {
            navigationAwareNavigableViewViewModel.OnNavigatedFrom();
        }

        if (content is FrameworkElement { DataContext: INavigationAware navigationAwareCurrentContent })
        {
            navigationAwareCurrentContent.OnNavigatedFrom();
        }
    }

    private void ApplyTransitionEffectToNavigatedPage(object? content)
    {
        if (content == null || TransitionDuration < 1)
        {
            return;
        }

        TransitionAnimationProvider.ApplyTransition(content, Transition, TransitionDuration);
    }
}
