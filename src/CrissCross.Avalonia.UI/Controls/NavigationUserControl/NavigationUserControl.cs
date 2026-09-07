// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

#if REACTIVELIST_REACTIVE
using CoreViewModelRoutedViewHost = CrissCross.Reactive.Avalonia.ViewModelRoutedViewHost;
#else
using CoreViewModelRoutedViewHost = CrissCross.Avalonia.ViewModelRoutedViewHost;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Hosts a routed Avalonia view-model navigation frame inside a UI user control.</summary>
/// <seealso cref="UserControl" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IUseNavigation" />
/// <seealso cref="IActivatableView" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class NavigationUserControl : UserControl, ISetNavigation, IUseNavigation, IActivatableView, IDisposable
{
    /// <summary>Identifies the XAML-addressable view-model property shared by all navigation control types.</summary>
    public static readonly StyledProperty<object?> ViewModelProperty =
        AvaloniaProperty.Register<NavigationUserControl, object?>(nameof(ViewModel));

    /// <summary>The navigate back is enabled property.</summary>
    public static readonly StyledProperty<bool?> NavigateBackIsEnabledProperty = AvaloniaProperty.Register<
        NavigationUserControl,
        bool?
    >(nameof(NavigateBackIsEnabled), defaultValue: true);

    /// <summary>The navigation frame property.</summary>
    public static readonly StyledProperty<CoreViewModelRoutedViewHost?> NavigationFrameProperty =
        AvaloniaProperty.Register<NavigationUserControl, CoreViewModelRoutedViewHost?>(nameof(NavigationFrame));

    /// <summary>Stores the navigation Host Name value.</summary>
    private string? _navigationHostName;

    /// <summary>Stores a value indicating whether this control is restoring its own content host.</summary>
    private bool _isUpdatingContent;

    /// <summary>Stores a value indicating whether this control has already disposed its navigation host.</summary>
    private bool _disposedValue;

    /// <summary>Initializes static members of the <see cref="NavigationUserControl"/> class.</summary>
    static NavigationUserControl() =>
        _ = NavigationFrameProperty.Changed.Subscribe(
            static (e) =>
            {
                if (
                    e.Sender is not NavigationUserControl navigationControl
                    || e.NewValue.Value is not CoreViewModelRoutedViewHost host)
                {
                    return;
                }

                navigationControl.ConfigureNavigationHost(host, nameof(NavigationUserControl));
            });

    /// <summary>Gets an observable indicating whether the host can navigate back.</summary>
    public IObservable<bool?>? CanNavigateBack => NavigationFrame?.CanNavigateBackObservable;

    /// <summary>Gets or sets the stable routed navigation host name.</summary>
    public string? HostName
    {
        get => _navigationHostName ?? Name;
        set
        {
            _navigationHostName = string.IsNullOrWhiteSpace(value) ? null : value;
            if (NavigationFrame is not { } host)
            {
                return;
            }

            ConfigureNavigationHost(host, nameof(NavigationUserControl));
        }
    }

    /// <summary>Gets or sets a value indicating whether back navigation is enabled.</summary>
    public bool? NavigateBackIsEnabled
    {
        get => GetValue(NavigateBackIsEnabledProperty);
        set => SetValue(NavigateBackIsEnabledProperty, value);
    }

    /// <summary>Gets or sets the view model exposed to XAML and strongly typed navigation controls.</summary>
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>Gets the composed routed navigation frame.</summary>
    public CoreViewModelRoutedViewHost? NavigationFrame
    {
        get => GetValue(NavigationFrameProperty);
        private set => SetValue(NavigationFrameProperty, value);
    }

    /// <inheritdoc/>
    string? ISetNavigation.Name => HostName;

    /// <inheritdoc/>
    string? IUseNavigation.Name => HostName;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Disposes the composed navigation frame and unregisters its navigation host aliases.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Called when the control finishes initialization.</summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        EnsureHost();
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureHost();
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == NavigateBackIsEnabledProperty && NavigationFrame is { } host)
        {
            host.NavigateBackIsEnabled = NavigateBackIsEnabled;
            return;
        }

        if (change.Property == NameProperty)
        {
            HandleNameChanged();
            return;
        }

        if (change.Property != ContentProperty)
        {
            return;
        }

        HandleContentChanged(change.NewValue);
    }

    /// <summary>Releases managed resources.</summary>
    /// <param name="disposing">Whether managed resources should be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            var frame = NavigationFrame;
            NavigationFrame = null;
            if (ReferenceEquals(Content, frame))
            {
                Content = null;
            }

            frame?.Dispose();
        }

        _disposedValue = true;
    }

    /// <summary>Ensures the composed navigation frame exists and owns the visible content.</summary>
    private void EnsureHost()
    {
        if (_disposedValue)
        {
            return;
        }

        NavigationFrame ??= CreateNavigationFrame();
        MoveConsumerContentIntoNavigationFrame(NavigationFrame);
        ConfigureNavigationHost(NavigationFrame, nameof(NavigationUserControl));
    }

    /// <summary>Creates a routed navigation frame using the current control state.</summary>
    /// <returns>The created navigation frame.</returns>
    private CoreViewModelRoutedViewHost CreateNavigationFrame()
    {
        var hostName = ResolveNavigationHostName(nameof(NavigationUserControl));
        _navigationHostName = hostName;
        return new()
        {
            Name = hostName,
            HostName = hostName,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            NavigateBackIsEnabled = NavigateBackIsEnabled,
        };
    }

    /// <summary>Moves content supplied by consumers into the composed navigation frame.</summary>
    /// <param name="host">The composed navigation host.</param>
    private void MoveConsumerContentIntoNavigationFrame(CoreViewModelRoutedViewHost host)
    {
        if (ReferenceEquals(Content, host))
        {
            return;
        }

        var consumerContent = Content;
        _isUpdatingContent = true;
        try
        {
            if (consumerContent is not null)
            {
                Content = null;
            }

            if (consumerContent is not null && host.Content is null)
            {
                host.Content = consumerContent;
            }

            Content = host;
        }
        finally
        {
            _isUpdatingContent = false;
        }
    }

    /// <summary>Handles content set after the navigation frame has been created.</summary>
    /// <param name="newValue">The new content value.</param>
    private void HandleContentChanged(object? newValue)
    {
        if (_isUpdatingContent || NavigationFrame is not { } host || ReferenceEquals(newValue, host))
        {
            return;
        }

        _isUpdatingContent = true;
        try
        {
            Content = null;
            host.Content = newValue;
            Content = host;
        }
        finally
        {
            _isUpdatingContent = false;
        }
    }

    /// <summary>Handles updates to the Avalonia control name.</summary>
    private void HandleNameChanged()
    {
        if (string.IsNullOrWhiteSpace(Name) || !string.IsNullOrWhiteSpace(_navigationHostName))
        {
            return;
        }

        _navigationHostName = Name;
        if (NavigationFrame is not { } host)
        {
            return;
        }

        ConfigureNavigationHost(host, nameof(NavigationUserControl));
    }

    /// <summary>Configures and registers the composed navigation host.</summary>
    /// <param name="host">The navigation host.</param>
    /// <param name="fallbackPrefix">The fallback host-name prefix.</param>
    private void ConfigureNavigationHost(CoreViewModelRoutedViewHost host, string fallbackPrefix)
    {
        var hostName = ResolveNavigationHostName(fallbackPrefix);
        _navigationHostName = hostName;
        host.HostName = hostName;

        this.SetMainNavigationHost(host);
    }

    /// <summary>Resolves a stable navigation host name for this control.</summary>
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
