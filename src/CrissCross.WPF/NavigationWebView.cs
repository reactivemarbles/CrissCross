// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using Microsoft.Web.WebView2.Core;
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
    /// The automatic dispose property.
    /// </summary>
    public static readonly DependencyProperty AutoDisposeProperty =
        DependencyProperty.Register(
            nameof(AutoDispose),
            typeof(bool),
            typeof(NavigationWebView),
            new PropertyMetadata(true, AutoDisposePropertyChanged));

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
        typeof(Uri),
        typeof(NavigationWebView),
        new PropertyMetadata(SourceChanged));

    /// <summary>
    /// The WPF DependencyProperty which backs the Microsoft.Web.WebView2.Wpf.WebView2.ZoomFactor property.
    /// </summary>
    public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
        nameof(ZoomFactor),
        typeof(double),
        typeof(NavigationWebView),
        new PropertyMetadata(1.0, ZoomFactorPropertyChanged));

    /// <summary>
    /// The navigate back is enabled property.
    /// </summary>
    public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(object),
        typeof(NavigationWebView),
        new PropertyMetadata(null, ContentChanged));

    /// <summary>
    /// The default background color property.
    /// </summary>
    public static readonly DependencyProperty DefaultBackgroundColorProperty = DependencyProperty.Register(
            nameof(DefaultBackgroundColor),
            typeof(System.Drawing.Color),
            typeof(NavigationWebView),
            new PropertyMetadata(System.Drawing.Color.White, DefaultBackgroundColorPropertyChanged));

    /// <summary>
    /// The allow external drop property.
    /// </summary>
    public static readonly DependencyProperty AllowExternalDropProperty = DependencyProperty.Register(
            nameof(AllowExternalDrop),
            typeof(bool),
            typeof(NavigationWebView),
            new PropertyMetadata(true, AllowExternalDropPropertyChanged));

    /// <summary>
    /// The design mode foreground color property.
    /// </summary>
    public static readonly DependencyProperty DesignModeForegroundColorProperty = DependencyProperty.Register(
        nameof(DesignModeForegroundColor),
        typeof(System.Drawing.Color),
        typeof(NavigationWebView),
        new PropertyMetadata(System.Drawing.Color.Black, DesignModeForegroundColorChanged));

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
    [Category("Common")]
    public bool? NavigateBackIsEnabled
    {
        get => (bool?)GetValue(NavigateBackIsEnabledProperty);
        set => SetValue(NavigateBackIsEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the design mode foreground.
    /// </summary>
    /// <value>
    /// The color of the design mode foreground.
    /// </value>
    [Category("Common")]
    public System.Drawing.Color DesignModeForegroundColor
    {
        get => (System.Drawing.Color)GetValue(DesignModeForegroundColorProperty);
        set => SetValue(DesignModeForegroundColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    [Category("Common")]
    public Uri Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the default color of the background.
    /// </summary>
    /// <value>
    /// The default color of the background.
    /// </value>
    [Category("Common")]
    public System.Drawing.Color DefaultBackgroundColor
    {
        get => (System.Drawing.Color)GetValue(DefaultBackgroundColorProperty);
        set => SetValue(DefaultBackgroundColorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [allow external drop].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [allow external drop]; otherwise, <c>false</c>.
    /// </value>
    [Category("Common")]
    public bool AllowExternalDrop
    {
        get => (bool)GetValue(AllowExternalDropProperty);
        set => SetValue(AllowExternalDropProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether this instance can go back.
    /// if the WebView can navigate to a previous page in the navigation
    ///     history. Wrapper around the Microsoft.Web.WebView2.Core.CoreWebView2.CanGoBack
    ///     property of Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2. If Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2
    ///     isn't initialized yet then returns false.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance can go back; otherwise, <c>false</c>.
    /// </value>
    [Browsable(false)]
    public bool CanGoBack => _WebBrowser.CanGoBack;

    /// <summary>
    /// Gets a value indicating whether this instance can go forward.
    /// if the WebView can navigate to a next page in the navigation history.
    ///     Wrapper around the Microsoft.Web.WebView2.Core.CoreWebView2.CanGoForward property
    ///     of Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2. If Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2
    ///     isn't initialized yet then returns false.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance can go forward; otherwise, <c>false</c>.
    /// </value>
    [Browsable(false)]
    public bool CanGoForward => _WebBrowser.CanGoForward;

    /// <summary>
    /// Gets or sets a value indicating whether [zoom factor].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [zoom factor]; otherwise, <c>false</c>.
    /// </value>
    [Category("Common")]
    public double ZoomFactor
    {
        get => (double)GetValue(ZoomFactorProperty);
        set => SetValue(ZoomFactorProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the XAML overlay />.
    /// </summary>
    [Bindable(true)]
    [Category("Content")]
    public new object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets the opacity mask.
    /// </summary>
    /// <value>
    /// The opacity mask.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new System.Windows.Media.Brush OpacityMask => _WebBrowser.OpacityMask;

    /// <summary>
    /// Gets the opacity.
    /// </summary>
    /// <value>
    /// The opacity.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new double Opacity => _WebBrowser.Opacity;

    /// <summary>
    /// Gets the effect.
    /// </summary>
    /// <value>
    /// The effect.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Effect Effect => _WebBrowser.Effect;

    /// <summary>
    /// Gets the context menu.
    /// </summary>
    /// <value>
    /// The context menu.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ContextMenu ContextMenu => _WebBrowser.ContextMenu;

    /// <summary>
    /// Gets the focus visual style.
    /// </summary>
    /// <value>
    /// The focus visual style.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Style FocusVisualStyle => _WebBrowser.FocusVisualStyle;

    /// <summary>
    /// Gets the input scope.
    /// </summary>
    /// <value>
    /// The input scope.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new InputScope InputScope => _WebBrowser.InputScope;

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
    [Category("Common")]
    public TransitionType Transition
    {
        get => (TransitionType)GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic dispose].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [automatic dispose]; otherwise, <c>false</c>.
    /// </value>
    public bool AutoDispose
    {
        get => (bool)GetValue(AutoDisposeProperty);
        set => SetValue(AutoDisposeProperty, value);
    }

    /// <summary>
    /// Navigates the WebView to the previous page in the navigation history. Equivalent
    ///     to calling Microsoft.Web.WebView2.Core.CoreWebView2.GoBack on Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2
    ///     If CoreWebView2 hasn't been initialized yet then does nothing.
    /// </summary>
    public void GoBack() => _WebBrowser?.GoBack();

    /// <summary>
    /// Navigates the WebView to the next page in the navigation history. Equivalent
    ///     to calling Microsoft.Web.WebView2.Core.CoreWebView2.GoForward on Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2
    ///     If CoreWebView2 hasn't been initialized yet then does nothing.
    /// </summary>
    public void GoForward() => _WebBrowser?.GoForward();

    /// <summary>
    /// Reloads the current page. Equivalent to calling Microsoft.Web.WebView2.Core.CoreWebView2.Reload
    ///     on Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2.
    /// </summary>
    public void Reload() => _WebBrowser?.Reload();

    /// <summary>
    /// Stops all navigations and pending resource fetches. Equivalent to calling Microsoft.Web.WebView2.Core.CoreWebView2.Stop
    ///     on Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2.
    /// </summary>
    public void Stop() => _WebBrowser?.Stop();

    /// <summary>
    /// Initiates a navigation to htmlContent as source HTML of a new document. Equivalent
    ///     to calling Microsoft.Web.WebView2.Core.CoreWebView2.NavigateToString(System.String)
    ///     on Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2.
    /// </summary>
    /// <param name="htmlContent">Content of the HTML.</param>
    public void NavigateToString(string htmlContent) => _WebBrowser?.NavigateToString(htmlContent);

    /// <summary>
    /// Executes JavaScript code from the javaScript parameter in the current top level
    ///     document rendered in the WebView. Equivalent to calling Microsoft.Web.WebView2.Core.CoreWebView2.ExecuteScriptAsync(System.String)
    ///     on Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2.
    /// </summary>
    /// <param name="javaScript">The java script.</param>
    /// <returns>A string.</returns>
    public async Task<string> ExecuteScriptAsync(string javaScript) => await _WebBrowser.ExecuteScriptAsync(javaScript);

    /// <summary>
    /// Ensures the core web view2 asynchronous.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <param name="controllerOptions">The controller options.</param>
    /// <returns>A Task that represents the background initialization process. When the task completes
    ///     then the Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2 property will be available
    ///     for use (i.e. non-null). Note that the control's Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2InitializationCompleted
    ///     event will be invoked before the task completes.</returns>
    public Task EnsureCoreWebView2Async(CoreWebView2Environment? environment = null, CoreWebView2ControllerOptions? controllerOptions = null) =>
        _WebBrowser.EnsureCoreWebView2Async(environment, controllerOptions);

    /// <summary>
    /// Ensures the core web view2 asynchronous.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <returns>A Task that represents the background initialization process. When the task completes
    ///     then the Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2 property will be available
    ///     for use (i.e. non-null). Note that the control's Microsoft.Web.WebView2.Wpf.WebView2.CoreWebView2InitializationCompleted
    ///     event will be invoked before the task completes.</returns>
    public Task EnsureCoreWebView2Async(CoreWebView2Environment environment) => _WebBrowser.EnsureCoreWebView2Async(environment);

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
        AutoDisposePropertyChanged(this, new DependencyPropertyChangedEventArgs(AutoDisposeProperty, null, null));
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

    private static void DefaultBackgroundColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser && e.NewValue is System.Drawing.Color color)
        {
            browser._WebBrowser.DefaultBackgroundColor = color;
        }
    }

    private static void ZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser && e.NewValue is double zoom)
        {
            browser._WebBrowser.ZoomFactor = zoom;
        }
    }

    private static void AllowExternalDropPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser && e.NewValue is bool allowDrop)
        {
            browser._WebBrowser.AllowExternalDrop = allowDrop;
        }
    }

    private static void DesignModeForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser && e.NewValue is System.Drawing.Color color)
        {
            browser._WebBrowser.DesignModeForegroundColor = color;
        }
    }

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser && e.NewValue is Uri source)
        {
            browser._WebBrowser.Source = source;
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

    private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser && browser._navigationWindowHost?.Window is not null)
        {
            browser._navigationWindowHost.Window.Content = e.NewValue;
        }
    }

    private static void AutoDisposePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationWebView browser)
        {
            if (browser.AutoDispose)
            {
                browser._WebBrowser.Unloaded += (s, e) => browser.Dispose();
            }
            else
            {
                browser._WebBrowser.Unloaded -= (s, e) => browser.Dispose();
            }
        }
    }
}
