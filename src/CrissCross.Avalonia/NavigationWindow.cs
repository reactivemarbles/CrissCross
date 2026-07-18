// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia;
#else
namespace CrissCross.Avalonia;
#endif

/// <summary>Hosts Avalonia navigation content in a window.</summary>
/// <seealso cref="TemplatedControl" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IUseNavigation" />
/// <seealso cref="IActivatableView" />
public class NavigationWindow : Window, ISetNavigation, IUseNavigation, IActivatableView
{
    /// <summary>The navigate back is enabled property.</summary>
    public static readonly StyledProperty<bool?> NavigateBackIsEnabledProperty = AvaloniaProperty.Register<
        NavigationWindow,
        bool?
    >(nameof(NavigateBackIsEnabled), defaultValue: true);

    /// <summary>The navigation frame property.</summary>
    public static readonly StyledProperty<ViewModelRoutedViewHost?> NavigationFrameProperty = AvaloniaProperty.Register<
        NavigationWindow,
        ViewModelRoutedViewHost?
    >(nameof(NavigationFrame));

    /// <summary>Stores the navigation Host Name value.</summary>
    private string? _navigationHostName;

    /// <summary>Initializes static members of the <see cref="NavigationWindow"/> class.</summary>
    static NavigationWindow() =>
        _ = NavigationFrameProperty.Changed.Subscribe(
            static (e) =>
            {
                if (
                    e.Sender is not NavigationWindow navigationWindow
                    || e.NewValue.Value is not ViewModelRoutedViewHost host)
                {
                    return;
                }

                navigationWindow.ConfigureNavigationHost(host, nameof(NavigationWindow));
            });

    /// <summary>Gets the can navigate back.</summary>
    /// <value>
    /// The can navigate back.
    /// </value>
    public IObservable<bool?>? CanNavigateBack => NavigationFrame?.CanNavigateBackObservable;

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    public bool? NavigateBackIsEnabled
    {
        get => GetValue(NavigateBackIsEnabledProperty);
        set => SetValue(NavigateBackIsEnabledProperty, value);
    }

    /// <summary>Gets the navigation frame.</summary>
    /// <value>
    /// The navigation frame.
    /// </value>
    public ViewModelRoutedViewHost? NavigationFrame
    {
        get => GetValue(NavigationFrameProperty);
        private set => SetValue(NavigationFrameProperty, value);
    }

    /// <inheritdoc/>
    string? ISetNavigation.Name => _navigationHostName ?? Name;

    /// <inheritdoc/>
    string? IUseNavigation.Name => _navigationHostName ?? Name;

    /// <summary>Called when the control finishes initialization.</summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var hostName = ResolveNavigationHostName(nameof(NavigationWindow));
        var navigateBackIsEnabled = NavigateBackIsEnabled;
        _navigationHostName = hostName;
        NavigationFrame = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HostName = hostName,
            NavigateBackIsEnabled = navigateBackIsEnabled,
        };
    }

    /// <summary>Registers the content presenter.</summary>
    /// <param name="presenter">The presenter.</param>
    /// <returns>A bool.</returns>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter is null)
        {
            return false;
        }

        if (presenter.Name == "PART_ContentPresenter" && presenter.Content is null)
        {
            presenter.Content = NavigationFrame;
        }

        return base.RegisterContentPresenter(presenter);
    }

    /// <summary>Runs the configure Navigation Host operation.</summary>
    /// <param name="host">The navigation host.</param>
    /// <param name="fallbackPrefix">The fallback host-name prefix.</param>
    private void ConfigureNavigationHost(ViewModelRoutedViewHost host, string fallbackPrefix)
    {
        var hostName = ResolveNavigationHostName(fallbackPrefix);
        _navigationHostName = hostName;
        host.HostName = hostName;

        if (string.IsNullOrWhiteSpace(host.Name))
        {
            host.Name = hostName;
        }

        this.SetMainNavigationHost(host);
    }

    /// <summary>Runs the resolve Navigation Host Name operation.</summary>
    /// <param name="fallbackPrefix">The fallback host-name prefix.</param>
    /// <returns>The resolved host name.</returns>
    private string ResolveNavigationHostName(string fallbackPrefix)
    {
        if (!string.IsNullOrWhiteSpace(_navigationHostName))
        {
            return _navigationHostName!;
        }

        return !string.IsNullOrWhiteSpace(Name)
            ? Name!
            : $"__crisscross_navhost_{fallbackPrefix}_{RuntimeHelpers.GetHashCode(this):X8}";
    }
}
