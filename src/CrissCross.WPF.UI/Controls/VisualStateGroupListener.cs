// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents VisualStateGroupListener.</summary>
/// <seealso cref="FrameworkElement" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VisualStateGroupListener : FrameworkElement
{
    /// <summary>The group property.</summary>
    public static readonly DependencyProperty GroupProperty = DependencyProperty.Register(
        nameof(Group),
        typeof(VisualStateGroup),
        typeof(VisualStateGroupListener),
        new(OnGroupChanged));

    /// <summary>The listener property.</summary>
    public static readonly DependencyProperty ListenerProperty = DependencyProperty.RegisterAttached(
        "Listener",
        typeof(VisualStateGroupListener),
        typeof(VisualStateGroupListener),
        new(OnListenerChanged));

    /// <summary>Provides the CurrentStateNamePropertyKey member.</summary>
    public static readonly DependencyPropertyKey CurrentStateNamePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(CurrentStateName),
        typeof(string),
        typeof(VisualStateGroupListener),
        null);

    /// <summary>The current state name property.</summary>
    public static readonly DependencyProperty CurrentStateNameProperty = CurrentStateNamePropertyKey.DependencyProperty;

    /// <summary>Provides the VisualStateGroupListener member.</summary>
    static VisualStateGroupListener()
    {
        VisibilityProperty.OverrideMetadata(
            typeof(VisualStateGroupListener),
            new FrameworkPropertyMetadata(Visibility.Collapsed));
    }

    /// <summary>Gets or sets the group.</summary>
    /// <value>
    /// The group.
    /// </value>
    public VisualStateGroup Group
    {
        get => (VisualStateGroup)GetValue(GroupProperty);
        set => SetValue(GroupProperty, value);
    }

    /// <summary>Gets the name of the current state.</summary>
    /// <value>
    /// The name of the current state.
    /// </value>
    public string CurrentStateName
    {
        get => (string)GetValue(CurrentStateNameProperty);
        private set => SetValue(CurrentStateNamePropertyKey, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Gets the listener.</summary>
    /// <param name="group">The group.</param>
    /// <returns>Visual State Group Listener.</returns>
    public static VisualStateGroupListener GetListener(VisualStateGroup group)
    {
        ThrowHelper.ThrowIfNull(group, nameof(group));

        return (VisualStateGroupListener)group.GetValue(ListenerProperty);
    }

    /// <summary>Sets the listener.</summary>
    /// <param name="group">The group.</param>
    /// <param name="value">The value.</param>
    public static void SetListener(VisualStateGroup group, VisualStateGroupListener value)
    {
        ThrowHelper.ThrowIfNull(group, nameof(group));

        group.SetValue(ListenerProperty, value);
    }

    /// <summary>Provides the OnGroupChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((VisualStateGroupListener)d).OnGroupChanged((VisualStateGroup)e.OldValue, (VisualStateGroup)e.NewValue);

    /// <summary>Provides the OnListenerChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is VisualStateGroupListener oldListener)
        {
            oldListener.ClearValue(GroupProperty);
        }

        if (e.NewValue is not VisualStateGroupListener newListener)
        {
            return;
        }

        newListener.Group = (VisualStateGroup)d;
    }

    /// <summary>Provides the OnGroupChanged member.</summary>
    /// <param name="oldGroup">The oldGroup value.</param>
    /// <param name="newGroup">The newGroup value.</param>
    private void OnGroupChanged(VisualStateGroup oldGroup, VisualStateGroup newGroup)
    {
        if (oldGroup is not null)
        {
            oldGroup.CurrentStateChanged -= OnCurrentStateChanged;
        }

        if (newGroup is not null)
        {
            newGroup.CurrentStateChanged += OnCurrentStateChanged;
        }

        UpdateCurrentStateName(newGroup?.CurrentState);
    }

    /// <summary>Provides the OnCurrentStateChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnCurrentStateChanged(object? sender, VisualStateChangedEventArgs e) =>
        UpdateCurrentStateName(e.NewState);

    /// <summary>Provides the UpdateCurrentStateName member.</summary>
    /// <param name="currentState">The currentState value.</param>
    private void UpdateCurrentStateName(VisualState? currentState)
    {
        if (currentState is not null)
        {
            CurrentStateName = currentState.Name;
        }
        else
        {
            ClearValue(CurrentStateNamePropertyKey);
        }
    }
}
