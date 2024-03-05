// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ReactiveUI;
using static ReactiveUI.TransitioningContentControl;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Navigation Window.
/// </summary>
/// <seealso cref="FluentWindow" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IUseNavigation" />
/// <seealso cref="IActivatableView" />
public class FluentNavigationWindow : FluentWindow, ISetNavigation, IUseNavigation, IActivatableView
{
    /// <summary>
    /// The navigate back is enabled property.
    /// </summary>
    public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(
        nameof(NavigateBackIsEnabled),
        typeof(bool?),
        typeof(FluentNavigationWindow),
        new PropertyMetadata(true));

    /// <summary>
    /// The application title property.
    /// </summary>
    public static readonly DependencyProperty ApplicationTitleProperty = DependencyProperty.Register(
            nameof(ApplicationTitle),
            typeof(string),
            typeof(FluentNavigationWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// The navigation frame property.
    /// </summary>
    public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register(
        nameof(NavigationFrame),
        typeof(ViewModelRoutedViewHost),
        typeof(FluentNavigationWindow));

    /// <summary>
    /// The transition property.
    /// </summary>
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
        nameof(Transition),
        typeof(TransitionType),
        typeof(FluentNavigationWindow),
        new PropertyMetadata(TransitionType.Fade));

    /// <summary>
    /// The TitleIcon property.
    /// </summary>
    public static readonly DependencyProperty TitleIconProperty = DependencyProperty.Register(
        nameof(TitleIcon),
        typeof(ImageSource),
        typeof(FluentNavigationWindow));

    static FluentNavigationWindow() => DefaultStyleKeyProperty.OverrideMetadata(
           typeof(FluentNavigationWindow),
           new FrameworkPropertyMetadata(typeof(FluentNavigationWindow)));

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentNavigationWindow"/> class.
    /// </summary>
    public FluentNavigationWindow() => SetResourceReference(StyleProperty, typeof(FluentNavigationWindow));

    /// <summary>
    /// Gets the can navigate back.
    /// </summary>
    /// <value>
    /// The can navigate back.
    /// </value>
    public IObservable<bool?> CanNavigateBack =>
        NavigationFrame.CanNavigateBackObservable;

    /// <summary>
    /// Gets or sets a value indicating whether [navigate back is enabled].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    public bool? NavigateBackIsEnabled
    {
        get => (bool?)GetValue(NavigateBackIsEnabledProperty);
        set => SetValue(NavigateBackIsEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the Source on this Image.
    /// </summary>
    public ImageSource? TitleIcon
    {
        get => (ImageSource)GetValue(TitleIconProperty);
        set => SetValue(TitleIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the application title.
    /// </summary>
    /// <value>
    /// The application title.
    /// </value>
    public string ApplicationTitle
    {
        get => (string)GetValue(ApplicationTitleProperty);
        set => SetValue(ApplicationTitleProperty, value);
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

    /// <inheritdoc/>
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
