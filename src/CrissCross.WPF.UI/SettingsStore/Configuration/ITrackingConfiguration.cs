// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Configuration;

/// <summary>Represents ITrackingConfiguration.</summary>
public interface ITrackingConfiguration
{
    /// <summary>Gets the persist triggers.</summary>
    /// <value>
    /// The persist triggers.
    /// </value>
    List<Trigger> PersistTriggers { get; }

    /// <summary>Gets or sets the stop tracking trigger.</summary>
    /// <value>
    /// The stop tracking trigger.
    /// </value>
    Trigger? StopTrackingTrigger { get; set; }

    /// <summary>Gets the type of the target.</summary>
    /// <value>
    /// The type of the target.
    /// </value>
    Type? TargetType { get; }

    /// <summary>Gets the tracked properties.</summary>
    /// <value>
    /// The tracked properties.
    /// </value>
    Dictionary<string, TrackedPropertyInfo> TrackedProperties { get; }

    /// <summary>Gets the tracker.</summary>
    /// <value>
    /// The tracker.
    /// </value>
    Tracker? Tracker { get; }

    /// <summary>Determines whether this instance can persist the specified can persist function.</summary>
    /// <param name="canPersistFunc">The canPersistFunc value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration CanPersist(Func<object, bool> canPersistFunc);

    /// <summary>Gets the store identifier.</summary>
    /// <param name="target">The target value.</param>
    /// <returns>A string.</returns>
    string GetStoreId(object target);

    /// <summary>Identifiers the specified identifier function.</summary>
    /// <param name="idFunc">The idFunc value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration Id(Func<object, string> idFunc);

    /// <summary>Identifiers the specified identifier function.</summary>
    /// <param name="idFunc">The idFunc value.</param>
    /// <param name="namespace">The namespace value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration Id(Func<object, string> idFunc, object? @namespace);

    /// <summary>Identifiers the specified identifier function.</summary>
    /// <param name="idFunc">The idFunc value.</param>
    /// <param name="namespace">The namespace value.</param>
    /// <param name="includeType">The includeType value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration Id(Func<object, string> idFunc, object? @namespace, bool includeType);

    /// <summary>Persists the on.</summary>
    /// <param name="eventNames">The eventNames value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration PersistOn(params string[] eventNames);

    /// <summary>Persists the on.</summary>
    /// <param name="eventName">The eventName value.</param>
    /// <param name="eventSourceGetter">The eventSourceGetter value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration PersistOn(string eventName, Func<object, object> eventSourceGetter);

    /// <summary>Persists the on.</summary>
    /// <param name="eventName">The eventName value.</param>
    /// <param name="eventSourceObject">The eventSourceObject value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration PersistOn(string eventName, object eventSourceObject);

    /// <summary>Stops the tracking on.</summary>
    /// <param name="eventName">The eventName value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration StopTrackingOn(string eventName);

    /// <summary>Stops the tracking on.</summary>
    /// <param name="eventName">The eventName value.</param>
    /// <param name="eventSourceGetter">The eventSourceGetter value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration StopTrackingOn(string eventName, Func<object, object> eventSourceGetter);

    /// <summary>Stops the tracking on.</summary>
    /// <param name="eventName">The eventName value.</param>
    /// <param name="eventSource">The eventSource value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration StopTrackingOn(string eventName, object eventSource);

    /// <summary>Whens the state of the applied.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenAppliedState(Action<object> action);

    /// <summary>Whens the applying property.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenApplyingProperty(Action<object, PropertyOperationData> action);

    /// <summary>Whens the persisted.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenPersisted(Action<object> action);

    /// <summary>Whens the persisting property.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>ITracking Configuration.</returns>
    ITrackingConfiguration WhenPersistingProperty(Action<object, PropertyOperationData> action);
}
