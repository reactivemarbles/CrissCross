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

namespace CrissCross.WPF;

/// <summary>
/// Navigation Web View.
/// </summary>
/// <seealso cref="ContentControl" />
/// <seealso cref="IDisposable" />
[ToolboxItem(true)]
public class WebView2Wpf : ContentControl, IDisposable
{
    private static readonly DependencyPropertyKey CanGoBackPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(CanGoBack),
        typeof(bool),
        typeof(WebView2Wpf),
        new PropertyMetadata(false));

    private static readonly DependencyPropertyKey CanGoForwardPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(CanGoForward),
        typeof(bool),
        typeof(WebView2Wpf),
        new PropertyMetadata(false));

#pragma warning disable SA1202 // Elements should be ordered by access

    /// <summary>
    /// The automatic dispose property.
    /// </summary>
    public static readonly DependencyProperty AutoDisposeProperty =
        DependencyProperty.Register(
            nameof(AutoDispose),
            typeof(bool),
            typeof(WebView2Wpf),
            new PropertyMetadata(true, AutoDisposePropertyChanged));

    /// <summary>
    /// The WPF DependencyProperty which backs the Microsoft.Web.WebView2.Wpf.WebView2.CreationProperties property.
    /// </summary>
    public static readonly DependencyProperty CreationPropertiesProperty = DependencyProperty.Register(
        nameof(CreationProperties),
        typeof(CoreWebView2CreationProperties),
        typeof(WebView2Wpf),
        new PropertyMetadata(CreationPropertiesChanged));

    /// <summary>
    /// The WPF DependencyProperty which backs the Microsoft.Web.WebView2.Wpf.WebView2.Source property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(Uri),
        typeof(WebView2Wpf),
        new PropertyMetadata(SourcePropertyChanged));

    /// <summary>
    /// The WPF DependencyProperty which backs the Microsoft.Web.WebView2.Wpf.WebView2.CanGoBack property.
    /// </summary>
    public static readonly DependencyProperty CanGoBackProperty = CanGoBackPropertyKey!.DependencyProperty;

    /// <summary>
    /// The WPF DependencyProperty which backs the Microsoft.Web.WebView2.Wpf.WebView2.CanGoForward property.
    /// </summary>
    public static readonly DependencyProperty CanGoForwardProperty = CanGoForwardPropertyKey!.DependencyProperty;

    /// <summary>
    /// The WPF DependencyProperty which backs the Microsoft.Web.WebView2.Wpf.WebView2.ZoomFactor property.
    /// </summary>
    public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
        nameof(ZoomFactor),
        typeof(double),
        typeof(WebView2Wpf),
        new PropertyMetadata(1.0, ZoomFactorPropertyChanged));

    /// <summary>
    /// The navigate back is enabled property.
    /// </summary>
    public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(object),
        typeof(WebView2Wpf),
        new PropertyMetadata(true, ContentChanged));

    /// <summary>
    /// The default background color property.
    /// </summary>
    public static readonly DependencyProperty DefaultBackgroundColorProperty = DependencyProperty.Register(
            nameof(DefaultBackgroundColor),
            typeof(System.Drawing.Color),
            typeof(WebView2Wpf),
            new PropertyMetadata(System.Drawing.Color.White, DefaultBackgroundColorPropertyChanged));

    /// <summary>
    /// The allow external drop property.
    /// </summary>
    public static readonly DependencyProperty AllowExternalDropProperty = DependencyProperty.Register(
            nameof(AllowExternalDrop),
            typeof(bool),
            typeof(WebView2Wpf),
            new PropertyMetadata(true, AllowExternalDropPropertyChanged));

    // Using a DependencyProperty as the backing store for DesignModeForegroundColor.  This enables animation, styling, binding, etc...
    /// <summary>
    /// The design mode foreground color property.
    /// </summary>
    public static readonly DependencyProperty DesignModeForegroundColorProperty = DependencyProperty.Register(
        nameof(DesignModeForegroundColor),
        typeof(System.Drawing.Color),
        typeof(WebView2Wpf),
        new PropertyMetadata(System.Drawing.Color.Black, DesignModeForegroundColorChanged));

#pragma warning restore SA1202 // Elements should be ordered by access

    private readonly WebView2 _WebBrowser;
    private WindowHost<Window>? _windowHost;
    private bool _disposedValue;
    private Window? _parentWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebView2Wpf"/> class.
    /// </summary>
    public WebView2Wpf() => _WebBrowser = new()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
    };

    /// <summary>
    /// Occurs when [core web view2 initialization completed].
    /// </summary>
    public event EventHandler<CoreWebView2InitializationCompletedEventArgs> CoreWebView2InitializationCompleted
    {
        add => _WebBrowser.CoreWebView2InitializationCompleted += value;
        remove => _WebBrowser.CoreWebView2InitializationCompleted -= value;
    }

    /// <summary>
    /// Occurs when [source changed].
    /// </summary>
    public event EventHandler<CoreWebView2SourceChangedEventArgs> SourceChanged
    {
        add => _WebBrowser.SourceChanged += value;
        remove => _WebBrowser.SourceChanged -= value;
    }

    /// <summary>
    /// Occurs when [navigation starting].
    /// </summary>
    public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting
    {
        add => _WebBrowser.NavigationStarting += value;
        remove => _WebBrowser.NavigationStarting -= value;
    }

    /// <summary>
    /// Occurs when [navigation completed].
    /// </summary>
    public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted
    {
        add => _WebBrowser.NavigationCompleted += value;
        remove => _WebBrowser.NavigationCompleted -= value;
    }

    /// <summary>
    /// Occurs when [zoom factor changed].
    /// </summary>
    public event EventHandler<EventArgs> ZoomFactorChanged
    {
        add => _WebBrowser.ZoomFactorChanged += value;
        remove => _WebBrowser.ZoomFactorChanged -= value;
    }

    /// <summary>
    /// Occurs when [content loading].
    /// </summary>
    public event EventHandler<CoreWebView2ContentLoadingEventArgs> ContentLoading
    {
        add => _WebBrowser.ContentLoading += value;
        remove => _WebBrowser.ContentLoading -= value;
    }

    /// <summary>
    /// Occurs when [web message received].
    /// </summary>
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived
    {
        add => _WebBrowser.WebMessageReceived += value;
        remove => _WebBrowser.WebMessageReceived -= value;
    }

    /// <summary>
    /// Gets accesses the complete functionality of the underlying Microsoft.Web.WebView2.Core.CoreWebView2
    ///     COM API. Returns null until initialization has completed. See the Microsoft.Web.WebView2.Wpf.WebView2
    ///     class documentation for an initialization overview.
    /// </summary>
    /// <value>
    /// The core web view2.
    /// </value>
    [Browsable(false)]
    public CoreWebView2 CoreWebView2 => _WebBrowser.CoreWebView2;

    /// <summary>
    /// Gets or sets the creation properties.
    /// </summary>
    /// <value>
    /// The creation properties.
    /// </value>
    [Category("Common")]
    public CoreWebView2CreationProperties CreationProperties
    {
        get => (CoreWebView2CreationProperties)GetValue(CreationPropertiesProperty);
        set => SetValue(CreationPropertiesProperty, value);
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
        try
        {
            _parentWindow = Window.GetWindow(this);
        }
        catch
        {
        }

        if (_parentWindow == null || _parentWindow.IsLoaded)
        {
            _windowHost = new(Name);
            _windowHost.Window.HorizontalAlignment = HorizontalAlignment;
            _windowHost.Window.VerticalAlignment = VerticalAlignment;
            _windowHost.Window.Content = Content;
            var layoutRoot = new Grid();
            layoutRoot.Children.Add(_WebBrowser);
            layoutRoot.Children.Add(_windowHost);
            base.Content = layoutRoot;
            AutoDisposePropertyChanged(this, new DependencyPropertyChangedEventArgs(AutoDisposeProperty, null, null));
        }
        else
        {
            _parentWindow.Loaded += ParentWindow_Loaded;
        }
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
                _windowHost?.Close();
                _windowHost?.Dispose();
            }

            _disposedValue = true;
        }
    }

    private static void DefaultBackgroundColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && e.NewValue is System.Drawing.Color color)
        {
            browser._WebBrowser.DefaultBackgroundColor = color;
        }
    }

    private static void ZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && e.NewValue is double zoom)
        {
            browser._WebBrowser.ZoomFactor = zoom;
        }
    }

    private static void AllowExternalDropPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && e.NewValue is bool allowDrop)
        {
            browser._WebBrowser.AllowExternalDrop = allowDrop;
        }
    }

    private static void DesignModeForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && e.NewValue is System.Drawing.Color color)
        {
            browser._WebBrowser.DesignModeForegroundColor = color;
        }
    }

    private static void CreationPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && e.NewValue is CoreWebView2CreationProperties creationProperties)
        {
            browser._WebBrowser.CreationProperties = creationProperties;
        }
    }

    private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && e.NewValue is Uri source)
        {
            browser._WebBrowser.Source = source;
        }
    }

    private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && browser._windowHost?.Window is not null)
        {
            browser._windowHost.Window.Content = e.NewValue;
        }
    }

    private static void AutoDisposePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser)
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

    private void ParentWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _parentWindow!.Loaded -= ParentWindow_Loaded;
        _windowHost = new(Name);
        _windowHost.Window.HorizontalAlignment = HorizontalAlignment;
        _windowHost.Window.VerticalAlignment = VerticalAlignment;
        _windowHost.Window.Content = Content;
        var layoutRoot = new Grid();
        layoutRoot.Children.Add(_WebBrowser);
        layoutRoot.Children.Add(_windowHost);
        base.Content = layoutRoot;
        AutoDisposePropertyChanged(this, new DependencyPropertyChangedEventArgs(AutoDisposeProperty, null, null));
    }
}
