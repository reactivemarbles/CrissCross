// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;
using ReactiveUI;
using static ReactiveUI.TransitioningContentControl;

namespace CrissCross.WPF;

/// <summary>
/// Navigation Web View.
/// </summary>
/// <seealso cref="ContentControl" />
/// <seealso cref="IDisposable" />
public class NavigationWebView : ContentControl, IDisposable, IUseNavigation, IActivatableView
{
    /// <summary>
    /// The navigate back is enabled property.
    /// </summary>
    public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(
        nameof(NavigateBackIsEnabled),
        typeof(bool?),
        typeof(NavigationWebView),
        new PropertyMetadata(true, NavigateBackChanged));

    /// <summary>
    /// The navigation frame property.
    /// </summary>
    public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register(
        nameof(NavigationFrame),
        typeof(ViewModelRoutedViewHost),
        typeof(NavigationWebView));

    /// <summary>
    /// The transition property.
    /// </summary>
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
        nameof(Transition),
        typeof(TransitionType),
        typeof(NavigationWebView),
        new PropertyMetadata(TransitionType.Fade, TransitionChanged));

    /// <summary>
    /// The source property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(string),
        typeof(NavigationWebView),
        new PropertyMetadata(string.Empty, SourceChanged));

    private readonly WebView2 _WebBrowser;
    private WindowHost<NavigationWindow>? _navigationWindowHost;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationWebView"/> class.
    /// </summary>
    public NavigationWebView() => _WebBrowser = new()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch
    };

    /// <summary>
    /// Gets the can navigate back.
    /// </summary>
    /// <value>
    /// The can navigate back.
    /// </value>
    public IObservable<bool?> CanNavigateBack =>
        _navigationWindowHost!.Window.CanNavigateBack;

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
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    public string? Source
    {
        get => (string?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
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

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized" /> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized" /> is set to <see langword="true" /> internally.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        _navigationWindowHost = new(Name);
        _navigationWindowHost.Window.HorizontalAlignment = HorizontalAlignment;
        _navigationWindowHost.Window.VerticalAlignment = VerticalAlignment;
        _navigationWindowHost.Window.NavigateBackIsEnabled = NavigateBackIsEnabled;
        _navigationWindowHost.Window.Transition = Transition;

        var layoutRoot = new Grid();
        layoutRoot.Children.Add(_WebBrowser);
        layoutRoot.Children.Add(_navigationWindowHost);
        Content = layoutRoot;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _WebBrowser.Dispose();
                _navigationWindowHost?.Close();
                _navigationWindowHost?.Dispose();
            }

            _disposedValue = true;
        }
    }

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser)
        {
            browser._WebBrowser.Source = new((string)e.NewValue);
        }
    }

    private static void NavigateBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser)
        {
            browser._navigationWindowHost!.Window.NavigateBackIsEnabled = (bool?)e.NewValue;
        }
    }

    private static void TransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser)
        {
            browser._navigationWindowHost!.Window.Transition = (TransitionType)e.NewValue;
        }
    }
}
