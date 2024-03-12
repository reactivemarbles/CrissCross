// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
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

    /// <summary>
    /// The title content property.
    /// </summary>
    public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
        nameof(TitleContent),
        typeof(object),
        typeof(FluentNavigationWindow));

    /// <summary>
    /// The top content property.
    /// </summary>
    public static readonly DependencyProperty TopContentProperty = DependencyProperty.Register(
        nameof(TopContent),
        typeof(object),
        typeof(FluentNavigationWindow));

    /// <summary>
    /// The left content property.
    /// </summary>
    public static readonly DependencyProperty LeftContentProperty = DependencyProperty.Register(
        nameof(LeftContent),
        typeof(object),
        typeof(FluentNavigationWindow));

    /// <summary>
    /// The right content property.
    /// </summary>
    public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register(
        nameof(RightContent),
        typeof(object),
        typeof(FluentNavigationWindow));

    /// <summary>
    /// The bottom content property.
    /// </summary>
    public static readonly DependencyProperty BottomContentProperty = DependencyProperty.Register(
        nameof(BottomContent),
        typeof(object),
        typeof(FluentNavigationWindow));

    /// <summary>
    /// The title header property.
    /// </summary>
    public static readonly DependencyProperty TitleHeaderProperty = DependencyProperty.Register(
        nameof(TitleHeader),
        typeof(object),
        typeof(FluentNavigationWindow));

    static FluentNavigationWindow() => DefaultStyleKeyProperty.OverrideMetadata(
           typeof(FluentNavigationWindow),
           new FrameworkPropertyMetadata(typeof(FluentNavigationWindow)));

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentNavigationWindow"/> class.
    /// </summary>
    public FluentNavigationWindow() =>
        SetResourceReference(StyleProperty, typeof(FluentNavigationWindow));

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
    /// Gets or sets the content of the title.
    /// </summary>
    /// <value>
    /// The content of the title.
    /// </value>
    public object? TitleContent
    {
        get => GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the top.
    /// </summary>
    /// <value>
    /// The content of the top.
    /// </value>
    public object? TopContent
    {
        get => GetValue(TopContentProperty);
        set => SetValue(TopContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the bottom.
    /// </summary>
    /// <value>
    /// The content of the bottom.
    /// </value>
    public object? BottomContent
    {
        get => GetValue(BottomContentProperty);
        set => SetValue(BottomContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the left.
    /// </summary>
    /// <value>
    /// The content of the left.
    /// </value>
    public object? LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the right.
    /// </summary>
    /// <value>
    /// The content of the right.
    /// </value>
    public object? RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the title header.
    /// </summary>
    /// <value>
    /// The title header.
    /// </value>
    public object? TitleHeader
    {
        get => GetValue(TitleHeaderProperty);
        set => SetValue(TitleHeaderProperty, value);
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
