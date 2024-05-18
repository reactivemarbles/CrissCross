// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Metadata;
using ReactiveUI;

[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross", "CrissCross")]
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross", "CrissCross.Avalonia")]
[assembly: XmlnsPrefix("https://github.com/reactivemarbles/CrissCross", "rxNav")]

namespace CrissCross.Avalonia;

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
        AvaloniaProperty.Register<NavigationWindow, bool?>(nameof(NavigateBackIsEnabled), defaultValue: true);

    /// <summary>
    /// The navigation frame property.
    /// </summary>
    public static readonly StyledProperty<ViewModelRoutedViewHost?> NavigationFrameProperty =
        AvaloniaProperty.Register<NavigationWindow, ViewModelRoutedViewHost?>(nameof(NavigationFrame));

    static NavigationWindow() =>
        NavigationFrameProperty.Changed.Subscribe(static (e) =>
        {
            if (e.Sender is NavigationWindow navigationWindow && e.NewValue.Value is ViewModelRoutedViewHost host)
            {
                host.HostName = navigationWindow.Name;
                navigationWindow.SetMainNavigationHost(host);
            }
        });

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
        NavigationFrame = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HostName = Name,
            NavigateBackIsEnabled = NavigateBackIsEnabled
        };
    }

    /// <summary>
    /// Registers the content presenter.
    /// </summary>
    /// <param name="presenter">The presenter.</param>
    /// <returns>A bool.</returns>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter == null)
        {
            return false;
        }

        if (presenter.Name == "PART_ContentPresenter" && presenter.Content == null)
        {
            presenter.Content = NavigationFrame;
        }

        return base.RegisterContentPresenter(presenter);
    }
}
