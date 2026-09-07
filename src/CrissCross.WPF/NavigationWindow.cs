// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using ReactiveUI;

#if REACTIVE_SHIM
using static ReactiveUI.Reactive.TransitioningContentControl;
#else
using static ReactiveUI.TransitioningContentControl;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF;
#else
namespace CrissCross.WPF;
#endif

/// <summary>Navigation Window.</summary>
/// <seealso cref="Window" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IUseNavigation" />
/// <seealso cref="IActivatableView" />
public class NavigationWindow : Window, ISetNavigation, IUseNavigation, IActivatableView
{
    /// <summary>The navigate back is enabled property.</summary>
    public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(
        nameof(NavigateBackIsEnabled),
        typeof(bool?),
        typeof(NavigationWindow),
        new(true));

    /// <summary>The navigation frame property.</summary>
    public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register(
        nameof(NavigationFrame),
        typeof(ViewModelRoutedViewHost),
        typeof(NavigationWindow));

    /// <summary>The transition property.</summary>
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
        nameof(Transition),
        typeof(TransitionType),
        typeof(NavigationWindow),
        new(TransitionType.Fade));

    /// <summary>Stores the navigation Host Name value.</summary>
    private string? _navigationHostName;

    /// <summary>Initializes a new instance of the <see cref="NavigationWindow"/> class.</summary>
    public NavigationWindow() => DefaultStyleKey = typeof(NavigationWindow);

    /// <summary>Gets the can navigate back.</summary>
    /// <value>
    /// The can navigate back.
    /// </value>
    public IObservable<bool?> CanNavigateBack => NavigationFrame.CanNavigateBackObservable;

    /// <summary>Gets or sets the stable routed navigation host name.</summary>
    public string? HostName
    {
        get => _navigationHostName ?? Name;
        set
        {
            _navigationHostName = string.IsNullOrWhiteSpace(value) ? null : value;
            if (NavigationFrame is null)
            {
                return;
            }

            ConfigureNavigationHost(NavigationFrame);
        }
    }

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    public bool? NavigateBackIsEnabled
    {
        get => (bool?)GetValue(NavigateBackIsEnabledProperty);
        set => SetValue(NavigateBackIsEnabledProperty, value);
    }

    /// <summary>Gets the navigation frame.</summary>
    /// <value>
    /// The navigation frame.
    /// </value>
    public ViewModelRoutedViewHost NavigationFrame
    {
        get => (ViewModelRoutedViewHost)GetValue(NavigationFrameProperty);
        private set => SetValue(NavigationFrameProperty, value);
    }

    /// <summary>Gets or sets the transition.</summary>
    /// <value>
    /// The transition.
    /// </value>
    public TransitionType Transition
    {
        get => (TransitionType)GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    /// <inheritdoc/>
    string? ISetNavigation.Name => HostName;

    /// <inheritdoc/>
    string? IUseNavigation.Name => HostName;

    /// <inheritdoc/>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        NavigationFrame =
            (Template.FindName(nameof(NavigationFrame), this) as ViewModelRoutedViewHost)
            ?? throw new InvalidOperationException(
                $"{$"{nameof(NavigationFrame)} as a {nameof(ViewModelRoutedViewHost)} "}is missing from the Style template.");

        ConfigureNavigationHost(NavigationFrame);
    }

    /// <inheritdoc/>
    protected override void OnClosed(EventArgs e)
    {
        NavigationFrame?.Dispose();
        base.OnClosed(e);
    }

    /// <summary>Runs the configure Navigation Host operation.</summary>
    /// <param name="host">The navigation host.</param>
    private void ConfigureNavigationHost(ViewModelRoutedViewHost host)
    {
        var hostName = ResolveNavigationHostName();
        _navigationHostName = hostName;
        host.HostName = hostName;

        if (string.IsNullOrWhiteSpace(host.Name))
        {
            host.Name = hostName;
        }

        this.SetMainNavigationHost(host);
    }

    /// <summary>Runs the resolve Navigation Host Name operation.</summary>
    /// <returns>The resolved host name.</returns>
    private string ResolveNavigationHostName()
    {
        if (!string.IsNullOrWhiteSpace(_navigationHostName))
        {
            return _navigationHostName!;
        }

        return !string.IsNullOrWhiteSpace(Name)
            ? Name
            : $"__crisscross_navhost_{nameof(NavigationWindow)}_{RuntimeHelpers.GetHashCode(this):X8}";
    }
}
