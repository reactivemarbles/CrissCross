// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using static ReactiveUI.TransitioningContentControl;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// A reactive UserControl that hosts a ViewModelRoutedViewHost for ViewModel-based navigation.
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IUseNavigation" />
/// <seealso cref="IActivatableView" />
#if NET6_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows10.0.17763.0")]
#endif
public class NavigationUserControl : UserControl, ISetNavigation, IUseNavigation, IActivatableView
{
    /// <summary>
    /// The navigate back is enabled property.
    /// </summary>
    public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(
        nameof(NavigateBackIsEnabled),
        typeof(bool?),
        typeof(NavigationUserControl),
        new PropertyMetadata(true, OnNavigateBackIsEnabledChanged));

    /// <summary>
    /// The navigation frame property.
    /// </summary>
    public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register(
        nameof(NavigationFrame),
        typeof(ViewModelRoutedViewHost),
        typeof(NavigationUserControl));

    /// <summary>
    /// The transition property.
    /// </summary>
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
        nameof(Transition),
        typeof(TransitionType),
        typeof(NavigationUserControl),
        new PropertyMetadata(TransitionType.Fade, OnTransitionChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationUserControl"/> class.
    /// </summary>
    public NavigationUserControl()
    {
        this.Events().Loaded.Subscribe(_ => EnsureHost());
    }

    /// <summary>
    /// Gets an observable indicating whether the host can navigate back.
    /// </summary>
    public IObservable<bool?> CanNavigateBack => NavigationFrame.CanNavigateBackObservable;

    /// <summary>
    /// Gets or sets a value indicating whether navigating back is enabled.
    /// </summary>
    public bool? NavigateBackIsEnabled
    {
        get => (bool?)GetValue(NavigateBackIsEnabledProperty);
        set => SetValue(NavigateBackIsEnabledProperty, value);
    }

    /// <summary>
    /// Gets the navigation host control.
    /// </summary>
    public ViewModelRoutedViewHost NavigationFrame
    {
        get => (ViewModelRoutedViewHost)GetValue(NavigationFrameProperty);
        private set => SetValue(NavigationFrameProperty, value);
    }

    /// <summary>
    /// Gets or sets the transition used when switching views.
    /// </summary>
    public TransitionType Transition
    {
        get => (TransitionType)GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    private static void OnNavigateBackIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationUserControl nuc && nuc.NavigationFrame is not null)
        {
            nuc.NavigationFrame.NavigateBackIsEnabled = (bool?)e.NewValue;
        }
    }

    private static void OnTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationUserControl nuc && nuc.NavigationFrame is not null)
        {
            nuc.NavigationFrame.Transition = (TransitionType)e.NewValue;
        }
    }

    private void EnsureHost()
    {
        NavigationFrame ??= new ViewModelRoutedViewHost
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HostName = Name,
                NavigateBackIsEnabled = NavigateBackIsEnabled,
                Transition = Transition
            };

        // Adopt as content if none set by the consumer
        Content ??= NavigationFrame;

        this.SetMainNavigationHost(NavigationFrame);
    }
}
