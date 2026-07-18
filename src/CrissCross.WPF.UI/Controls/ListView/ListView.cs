// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extends ListView adding customized support for GridView vs Default view states.</summary>
public class ListView : System.Windows.Controls.ListView, IDisposable
{
    /// <summary>Dependency property backing <see cref="ViewState"/>.</summary>
    public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(
        nameof(ViewState),
        typeof(ListViewViewState),
        typeof(ListView),
        new FrameworkPropertyMetadata(ListViewViewState.Default, OnViewStateChanged));

    /// <summary>Stores the _unloadedDisposable value.</summary>
    private readonly SerialDisposable _unloadedDisposable = new();

    /// <summary>Stores the _descriptor value.</summary>
    private DependencyPropertyDescriptor? _descriptor;

    /// <summary>Stores the _disposed value.</summary>
    private bool _disposed;

    /// <summary>Provides the ListView member.</summary>
    static ListView() =>
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ListView), new FrameworkPropertyMetadata(typeof(ListView)));

    /// <summary>Initializes a new instance of the <see cref="ListView"/> class.</summary>
    public ListView()
    {
        _ = Observable
            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Loaded += h, h => Loaded -= h)
            .Take(1)
            .Subscribe(_ => OnLoadedReactive());
    }

    /// <summary>Gets or sets the current visual state of the list (Default or GridView).</summary>
    public ListViewViewState ViewState
    {
        get => (ListViewViewState)GetValue(ViewStateProperty);
        set => SetValue(ViewStateProperty, value);
    }

    /// <summary>Disposes managed / unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Called when <see cref="ViewState"/> changes. Override to react to changes.</summary>
    /// <param name="e">Property changed arguments.</param>
    protected virtual void OnViewStateChanged(DependencyPropertyChangedEventArgs e) { }

    /// <summary>Protected dispose pattern implementation.</summary>
    /// <param name="disposing">If true, dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _unloadedDisposable.Dispose();
            _descriptor?.RemoveValueChanged(this, OnViewPropertyChanged);
            _descriptor = null;
        }

        _disposed = true;
    }

    /// <summary>Provides the OnViewStateChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnViewStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ListView lv)
        {
            return;
        }

        lv.OnViewStateChanged(e);
    }

    /// <summary>Provides the OnLoadedReactive member.</summary>
    private void OnLoadedReactive()
    {
        _descriptor = DependencyPropertyDescriptor.FromProperty(
            System.Windows.Controls.ListView.ViewProperty,
            typeof(System.Windows.Controls.ListView));
        _descriptor?.AddValueChanged(this, OnViewPropertyChanged);

        _unloadedDisposable.Disposable = Observable
            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Unloaded += h, h => Unloaded -= h)
            .Take(1)
            .Subscribe(_ => OnUnloadedReactive());

        UpdateViewState();
    }

    /// <summary>Provides the OnUnloadedReactive member.</summary>
    private void OnUnloadedReactive()
    {
        _descriptor?.RemoveValueChanged(this, OnViewPropertyChanged);
        _descriptor = null;
        _unloadedDisposable.Disposable = null;
    }

    /// <summary>Provides the OnViewPropertyChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnViewPropertyChanged(object? sender, EventArgs e) => UpdateViewState();

    /// <summary>Provides the UpdateViewState member.</summary>
    private void UpdateViewState()
    {
        var viewState =
            View is System.Windows.Controls.GridView ? ListViewViewState.GridView : ListViewViewState.Default;
        SetCurrentValue(ViewStateProperty, viewState);
    }
}
