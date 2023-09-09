// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        new PropertyMetadata(SourceChanged));

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
        typeof(WebView2Wpf));

    /// <summary>
    /// The navigate back is enabled property.
    /// </summary>
    public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(object),
        typeof(WebView2Wpf),
        new PropertyMetadata(true, ContentChanged));
#pragma warning restore SA1202 // Elements should be ordered by access

    private readonly WebView2 _WebBrowser;
    private WindowHost<Window>? _windowHost;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebView2Wpf"/> class.
    /// </summary>
    public WebView2Wpf()
    {
        _WebBrowser = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        Unloaded += (s, e) => Dispose();
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
    public bool ZoomFactor
    {
        get => (bool)GetValue(ZoomFactorProperty);
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
        _windowHost = new(Name);
        _windowHost.Window.HorizontalAlignment = HorizontalAlignment;
        _windowHost.Window.VerticalAlignment = VerticalAlignment;
        _windowHost.Window.Content = Content;
        var layoutRoot = new Grid();
        layoutRoot.Children.Add(_WebBrowser);
        layoutRoot.Children.Add(_windowHost);
        base.Content = layoutRoot;
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

    private static void CreationPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2Wpf browser && e.NewValue is CoreWebView2CreationProperties creationProperties)
        {
            browser._WebBrowser.CreationProperties = creationProperties;
        }
    }

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
}
