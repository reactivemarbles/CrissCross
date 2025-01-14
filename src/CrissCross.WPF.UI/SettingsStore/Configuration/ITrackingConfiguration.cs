// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Configuration;

/// <summary>
/// ITrackingConfiguration.
/// </summary>
public interface ITrackingConfiguration
{
    /// <summary>
    /// Gets the persist triggers.
    /// </summary>
    /// <value>
    /// The persist triggers.
    /// </value>
    List<Trigger> PersistTriggers { get; }

    /// <summary>
    /// Gets or sets the stop tracking trigger.
    /// </summary>
    /// <value>
    /// The stop tracking trigger.
    /// </value>
    Trigger? StopTrackingTrigger { get; set; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    /// <value>
    /// The type of the target.
    /// </value>
    Type? TargetType { get; }

    /// <summary>
    /// Gets the tracked properties.
    /// </summary>
    /// <value>
    /// The tracked properties.
    /// </value>
    Dictionary<string, TrackedPropertyInfo> TrackedProperties { get; }

    /// <summary>
    /// Gets the tracker.
    /// </summary>
    /// <value>
    /// The tracker.
    /// </value>
    Tracker? Tracker { get; }

    /// <summary>
    /// Determines whether this instance can persist the specified can persist function.
    /// </summary>
    /// <param name="canPersistFunc">The can persist function.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration CanPersist(Func<object, bool> canPersistFunc);

    /// <summary>
    /// Gets the store identifier.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>A string.</returns>
    string GetStoreId(object target);

    /// <summary>
    /// Identifiers the specified identifier function.
    /// </summary>
    /// <param name="idFunc">The identifier function.</param>
    /// <param name="namespace">The namespace.</param>
    /// <param name="includeType">if set to <c>true</c> [include type].</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration Id(Func<object, string> idFunc, object? @namespace = null, bool includeType = true);

    /// <summary>
    /// Persists the on.
    /// </summary>
    /// <param name="eventNames">The event names.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration PersistOn(params string[] eventNames);

    /// <summary>
    /// Persists the on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration PersistOn(string eventName, Func<object, object> eventSourceGetter);

    /// <summary>
    /// Persists the on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceObject">The event source object.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration PersistOn(string eventName, object eventSourceObject);

    /// <summary>
    /// Stops the tracking on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration StopTrackingOn(string eventName);

    /// <summary>
    /// Stops the tracking on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration StopTrackingOn(string eventName, Func<object, object> eventSourceGetter);

    /// <summary>
    /// Stops the tracking on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSource">The event source.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration StopTrackingOn(string eventName, object eventSource);

    /// <summary>
    /// Whens the state of the applied.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenAppliedState(Action<object> action);

    /// <summary>
    /// Whens the applying property.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenApplyingProperty(Action<object, PropertyOperationData> action);

    /// <summary>
    /// Whens the persisted.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenPersisted(Action<object> action);

    /// <summary>
    /// Whens the persisting property.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenPersistingProperty(Action<object, PropertyOperationData> action);
}
