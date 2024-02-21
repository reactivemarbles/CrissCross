// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls
{
    /// <summary>
    /// VisualStateGroupListener.
    /// </summary>
    /// <seealso cref="System.Windows.FrameworkElement" />
    public class VisualStateGroupListener : FrameworkElement
    {
        /// <summary>
        /// The group property.
        /// </summary>
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register(
                nameof(Group),
                typeof(VisualStateGroup),
                typeof(VisualStateGroupListener),
                new PropertyMetadata(OnGroupChanged));

        /// <summary>
        /// The listener property.
        /// </summary>
        public static readonly DependencyProperty ListenerProperty =
            DependencyProperty.RegisterAttached(
                "Listener",
                typeof(VisualStateGroupListener),
                typeof(VisualStateGroupListener),
                new PropertyMetadata(OnListenerChanged));

        private static readonly DependencyPropertyKey CurrentStateNamePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(CurrentStateName),
                typeof(string),
                typeof(VisualStateGroupListener),
                null);

        /// <summary>
        /// The current state name property.
        /// </summary>
#pragma warning disable SA1202 // Elements should be ordered by access
        public static readonly DependencyProperty CurrentStateNameProperty =
#pragma warning restore SA1202 // Elements should be ordered by access
            CurrentStateNamePropertyKey!.DependencyProperty;

        static VisualStateGroupListener() =>
            VisibilityProperty.OverrideMetadata(typeof(VisualStateGroupListener), new FrameworkPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public VisualStateGroup Group
        {
            get => (VisualStateGroup)GetValue(GroupProperty);
            set => SetValue(GroupProperty, value);
        }

        /// <summary>
        /// Gets the name of the current state.
        /// </summary>
        /// <value>
        /// The name of the current state.
        /// </value>
        public string CurrentStateName
        {
            get => (string)GetValue(CurrentStateNameProperty);
            private set => SetValue(CurrentStateNamePropertyKey, value);
        }

        /// <summary>
        /// Gets the listener.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>Visual State Group Listener.</returns>
        public static VisualStateGroupListener GetListener(VisualStateGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            return (VisualStateGroupListener)group.GetValue(ListenerProperty);
        }

        /// <summary>
        /// Sets the listener.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="value">The value.</param>
        public static void SetListener(VisualStateGroup group, VisualStateGroupListener value)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            group.SetValue(ListenerProperty, value);
        }

        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((VisualStateGroupListener)d).OnGroupChanged((VisualStateGroup)e.OldValue, (VisualStateGroup)e.NewValue);

        private static void OnListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is VisualStateGroupListener oldListener)
            {
                oldListener.ClearValue(GroupProperty);
            }

            if (e.NewValue is VisualStateGroupListener newListener)
            {
                newListener.Group = (VisualStateGroup)d;
            }
        }

        private void OnGroupChanged(VisualStateGroup oldGroup, VisualStateGroup newGroup)
        {
            if (oldGroup != null)
            {
                oldGroup.CurrentStateChanged -= OnCurrentStateChanged;
            }

            if (newGroup != null)
            {
                newGroup.CurrentStateChanged += OnCurrentStateChanged;
            }

            UpdateCurrentStateName(newGroup?.CurrentState);
        }

        private void OnCurrentStateChanged(object? sender, VisualStateChangedEventArgs e) => UpdateCurrentStateName(e.NewState);

        private void UpdateCurrentStateName(VisualState? currentState)
        {
            if (currentState != null)
            {
                CurrentStateName = currentState.Name;
            }
            else
            {
                ClearValue(CurrentStateNamePropertyKey);
            }
        }
    }
}
