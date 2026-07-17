// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides visual state group extension members.</summary>
internal static class VisualStateGroupExtensions
{
    /// <summary>Provides the CurrentStoryboardsProperty member.</summary>
    private static readonly DependencyProperty CurrentStoryboardsProperty = DependencyProperty.RegisterAttached(
        "CurrentStoryboards",
        typeof(Collection<Storyboard>),
        typeof(VisualStateGroupExtensions));

    /// <summary>Stores the _setCurrentState value.</summary>
    private static readonly Lazy<Action<VisualStateGroup, VisualState>> _setCurrentState = new(
        CreateSetCurrentStateDelegate);

    /// <summary>Gets the IsSupported value.</summary>
    internal static bool IsSupported => _setCurrentState.Value is not null;

    /// <summary>Provides extension members.</summary>
    /// <param name="group">The group value.</param>
    extension(VisualStateGroup group)
    {
        /// <summary>Provides the SetCurrentState member.</summary>
        /// <param name="value">The value.</param>
        internal void SetCurrentState(VisualState value)
        {
            if (!IsSupported)
            {
                throw new InvalidOperationException();
            }

            _setCurrentState.Value(group, value);
            Debug.Assert(ReferenceEquals(group.CurrentState, value), "Current State.");
        }

        /// <summary>Provides the GetState member.</summary>
        /// <param name="stateName">The stateName value.</param>
        /// <returns>The result.</returns>
        internal VisualState GetState(string stateName)
        {
            for (var stateIndex = 0; stateIndex < group.States.Count; ++stateIndex)
            {
                var state = (VisualState?)group.States[stateIndex];
                if (state?.Name == stateName)
                {
                    return state;
                }
            }

            return null!;
        }

        /// <summary>Provides the StartNewThenStopOld member.</summary>
        /// <param name="element">The element value.</param>
        /// <param name="newStoryboards">The newStoryboards value.</param>
        internal void StartNewThenStopOld(FrameworkElement element, params Storyboard[] newStoryboards)
        {
            var currentStoryboards = GetCurrentStoryboards(group);

            // Remove the old Storyboards. Remove is delayed until the next TimeManager tick, so the
            // handoff to the new storyboard is unaffected.
            for (var index = 0; index < currentStoryboards.Count; ++index)
            {
                if (currentStoryboards[index] is null)
                {
                    continue;
                }

                currentStoryboards[index].Remove(element);
            }

            currentStoryboards.Clear();

            // Start the new Storyboards
            for (var index = 0; index < newStoryboards.Length; ++index)
            {
                if (newStoryboards[index] is null)
                {
                    continue;
                }

                newStoryboards[index].Begin(element, HandoffBehavior.SnapshotAndReplace, true);

                // Hold on to the running Storyboards
                currentStoryboards.Add(newStoryboards[index]);

                // Silverlight had an issue where initially, a checked CheckBox would not show the check mark
                // until the second frame. They chose to do a Seek(0) at this point, which this line
                // is supposed to mimic. It does not seem to be equivalent, though, and WPF ends up
                // with some odd animation behavior. I haven't seen the CheckBox issue on WPF, so
                // The original Silverlight seek is intentionally not applied here.
            }
        }
    }

    /// <summary>Gets the currently running storyboards for the visual state group.</summary>
    /// <param name="group">The group value.</param>
    /// <returns>The result.</returns>
    internal static Collection<Storyboard> GetCurrentStoryboards(VisualStateGroup group)
    {
        var currentStoryboards = (Collection<Storyboard>)group.GetValue(CurrentStoryboardsProperty);
        if (currentStoryboards is null)
        {
            currentStoryboards = [];
            group.SetValue(CurrentStoryboardsProperty, currentStoryboards);
        }

        return currentStoryboards;
    }

    /// <summary>Provides the CreateSetCurrentStateDelegate member.</summary>
    /// <returns>The result.</returns>
    private static Action<VisualStateGroup, VisualState> CreateSetCurrentStateDelegate()
    {
        try
        {
            return DelegateHelper.CreatePropertySetter<VisualStateGroup, VisualState>(
                nameof(VisualStateGroup.CurrentState),
                nonPublic: true);
        }
        catch (Exception)
        {
            return null!;
        }
    }
}
