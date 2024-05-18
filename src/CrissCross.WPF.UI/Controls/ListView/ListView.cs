// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extends <see cref="System.Windows.Controls.ListView"/>, and adds customized support <see cref="ListViewViewState.GridView"/> or <see cref="ListViewViewState.Default"/>.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:ListView ItemsSource="{Binding ...}" &gt;
///     &lt;ui:ListView.View&gt;
///         &lt;GridView&gt;
///             &lt;GridViewColumn
///                 DisplayMemberBinding="{Binding FirstName}"
///                 Header="First Name" /&gt;
///             &lt;GridViewColumn
///                 DisplayMemberBinding="{Binding LastName}"
///                 Header="Last Name" /&gt;
///         &lt;/GridView&gt;
///     &lt;/ui:ListView.View&gt;
/// &lt;/ui:ListView&gt;
/// </code>
/// </example>
public class ListView : System.Windows.Controls.ListView
{
    /// <summary>Identifies the <see cref="ViewState"/> dependency property.</summary>
    public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(nameof(ViewState), typeof(ListViewViewState), typeof(ListView), new FrameworkPropertyMetadata(ListViewViewState.Default, OnViewStateChanged));

    static ListView() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ListView), new FrameworkPropertyMetadata(typeof(ListView)));

    /// <summary>
    /// Initializes a new instance of the <see cref="ListView"/> class.
    /// </summary>
    public ListView() => Loaded += OnLoaded;

    /// <summary>
    /// Gets or sets the view state of the <see cref="ListView"/>, enabling custom logic based on the current view.
    /// </summary>
    /// <value>The current view state of the <see cref="ListView"/>.</value>
    public ListViewViewState ViewState
    {
        get => (ListViewViewState)GetValue(ViewStateProperty);
        set => SetValue(ViewStateProperty, value);
    }

    /// <summary>
    /// Raises the <see cref="E:ViewStateChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnViewStateChanged(DependencyPropertyChangedEventArgs e)
    {
        // Hook for derived classes to react to ViewState property changes
    }

    private static void OnViewStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ListView self)
        {
            return;
        }

        self.OnViewStateChanged(e);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded; // prevent memory leaks

        // Setup initial ViewState and hook into View property changes
        var descriptor = DependencyPropertyDescriptor.FromProperty(System.Windows.Controls.ListView.ViewProperty, typeof(System.Windows.Controls.ListView));
        descriptor?.AddValueChanged(this, OnViewPropertyChanged);
        UpdateViewState(); // set the initial state
    }

    private void OnViewPropertyChanged(object? sender, EventArgs e) => UpdateViewState();

    private void UpdateViewState()
    {
        var viewState = View switch
        {
            System.Windows.Controls.GridView => ListViewViewState.GridView,
            null => ListViewViewState.Default,
            _ => ListViewViewState.Default
        };

        SetCurrentValue(ViewStateProperty, viewState);
    }
}
