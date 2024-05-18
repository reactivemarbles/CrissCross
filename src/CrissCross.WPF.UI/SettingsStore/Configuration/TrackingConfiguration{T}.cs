// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace CrissCross.WPF.UI.Configuration;

/// <summary>
/// A TrackingConfiguration determines how a target object will be tracked.
/// This includes list of properties to track, persist triggers and id getter.
/// </summary>
/// <typeparam name="T">The Type.</typeparam>
/// <seealso cref="ITrackingConfiguration" />
/// <remarks>
/// Derives from TrackingConfiguration and adds a generic strongly typed API for configuring tracking.
/// This class does not provide any new functionality nor store any additional state.All calls are forwarded to the base class.
/// </remarks>
public sealed class TrackingConfiguration<T> : ITrackingConfiguration
{
    private readonly TrackingConfiguration _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackingConfiguration{T}"/> class.
    /// </summary>
    /// <param name="inner">The inner.</param>
    internal TrackingConfiguration(TrackingConfiguration inner) => _inner = inner;

    /// <summary>
    /// Gets the tracker.
    /// </summary>
    /// <value>
    /// The tracker.
    /// </value>
    public Tracker? Tracker => _inner.Tracker;

    /// <summary>
    /// Gets the persist triggers.
    /// </summary>
    /// <value>
    /// The persist triggers.
    /// </value>
    public List<Trigger> PersistTriggers => _inner.PersistTriggers;

    /// <summary>
    /// Gets or sets the stop tracking trigger.
    /// </summary>
    /// <value>
    /// The stop tracking trigger.
    /// </value>
    public Trigger? StopTrackingTrigger { get => _inner.StopTrackingTrigger; set => _inner.StopTrackingTrigger = value; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    /// <value>
    /// The type of the target.
    /// </value>
    public Type? TargetType => _inner.TargetType;

    /// <summary>
    /// Gets the tracked properties.
    /// </summary>
    /// <value>
    /// The tracked properties.
    /// </value>
    public Dictionary<string, TrackedPropertyInfo> TrackedProperties => _inner.TrackedProperties;

    /// <summary>
    /// Start tracking the target object. This will apply any previously stored data and start
    /// listening for events that indicate persisting new data is required.
    /// </summary>
    /// <param name="target">The target object to track.</param>
    public void Track(T target)
        => _inner.Tracker?.Track(target!, _inner);

    /// <summary>
    /// Allows value conversion and cancellation when applying a stored value to a property.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> WhenApplyingProperty(Action<T, PropertyOperationData> action)
    {
        _inner.WhenApplyingProperty((obj, prop) => action((T)obj, prop));
        return this;
    }

    /// <summary>
    /// Allows supplying a callback that will be called when all saved state is applied to a target object.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> WhenAppliedState(Action<T> action)
    {
        _inner.WhenAppliedState(obj => action((T)obj));
        return this;
    }

    /// <summary>
    /// Allows value conversion and cancellation when persisting a property of the target object.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> WhenPersistingProperty(Action<T, PropertyOperationData> action)
    {
        _inner.WhenPersistingProperty((obj, prop) => action((T)obj, prop));
        return this;
    }

    /// <summary>
    /// Whens the persisted.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> WhenPersisted(Action<T> action)
    {
        _inner.WhenPersisted(obj => action((T)obj));
        return this;
    }

    /// <summary>
    /// Identifiers the specified identifier function.
    /// </summary>
    /// <param name="idFunc">The provided function will be used to get an identifier for a target object in order to identify the data that belongs to it.</param>
    /// <param name="namespace">Serves to distinguish objects with the same ids that are used in different contexts.</param>
    /// <param name="includeType">If true, the name of the type will be included in the id. This prevents id clashes with different types.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> Id(Func<T, string> idFunc, object? @namespace = null, bool includeType = true)
    {
        _inner.Id(t => idFunc((T)t), @namespace, includeType);
        return this;
    }

    /// <summary>
    /// Determines whether this instance can persist the specified can persist function.
    /// </summary>
    /// <param name="canPersistFunc">The provided function will be used to get an identifier for a target object in order to identify the data that belongs to it.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> CanPersist(Func<T, bool> canPersistFunc)
    {
        _inner.CanPersist(t => canPersistFunc((T)t));
        return this;
    }

    /// <summary>
    /// Registers the specified event of the target object as a trigger that will cause the target's data to be persisted.
    /// </summary>
    /// <param name="eventNames">The names of the events that will cause the target object's data to be persisted.</param>
    /// <returns>
    /// Tracking Configuration.
    /// </returns>
    /// <remarks>
    /// Automatically persist a target object when it fires the specified name.
    /// </remarks>
    /// <example>
    /// For a Window object, "LocationChanged" and/or "SizeChanged" would be appropriate.
    /// </example>
    public TrackingConfiguration<T> PersistOn(params Func<T, string>[] eventNames)
    {
        _inner.PersistOn(eventNames.Select(x => x(default!)).ToArray());
        return this;
    }

    /// <summary>
    /// Automatically persist a target object when the specified eventSourceObject fires the specified event.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceObject">If not provided.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> PersistOn(Func<T, string> eventName, object eventSourceObject)
    {
        if (eventName is null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        _inner.PersistOn(eventName(default!), eventSourceObject);
        return this;
    }

    /// <summary>
    /// Automatically persist a target object when the specified eventSourceObject fires the specified event.
    /// </summary>
    /// <param name="eventName">The name of the event that should trigger persisting stete.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> PersistOn(Func<T, string> eventName, Func<T, object> eventSourceGetter)
    {
        if (eventName is null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        _inner.PersistOn(eventName(default!), t => eventSourceGetter((T)t));
        return this;
    }

    /// <summary>
    /// Stop tracking the target when it fires the specified event.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> StopTrackingOn(Func<T, string> eventName)
    {
        if (eventName is null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        _inner.StopTrackingOn(eventName(default!));
        return this;
    }

    /// <summary>
    /// Stop tracking the target when the specified eventSource object fires the specified event.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSource">The event source.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> StopTrackingOn(Func<T, string> eventName, object eventSource)
    {
        if (eventName is null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        _inner.StopTrackingOn(eventName(default!), eventSource);
        return this;
    }

    /// <summary>
    /// Stop tracking the target when the specified eventSource object fires the specified event.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> StopTrackingOn(Func<T, string> eventName, Func<T, object> eventSourceGetter)
    {
        if (eventName is null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        _inner.StopTrackingOn(eventName(default!), t => eventSourceGetter((T)t));
        return this;
    }

    /// <summary>
    /// Set up tracking for the specified property.
    /// </summary>
    /// <typeparam name="TResult">The type.</typeparam>
    /// <param name="propertyAccessExpression">The property access expression.</param>
    /// <param name="name">The name.</param>
    /// <returns>
    /// Tracking Configuration.
    /// </returns>
    public TrackingConfiguration<T> Property<TResult>(Expression<Func<T, TResult?>> propertyAccessExpression, string? name = null) => Property(name, propertyAccessExpression, false, default);

    /// <summary>
    /// Set up tracking for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertyAccessExpression">The expression that points to the specified property. Can navigate multiple levels.</param>
    /// <param name="defaultValue">If there is no value in the store for the property, the defaultValue will be used.</param>
    /// <param name="name">The name of the property in the store.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> Property<TProperty>(Expression<Func<T, TProperty?>> propertyAccessExpression, TProperty defaultValue, string? name = null) => Property(name, propertyAccessExpression, true, defaultValue);

    /// <summary>
    /// Set up tracking for one or more properties.
    /// </summary>
    /// <param name="projection">Describes which properties of the target object to track by returning an anonymous type projection (e.g. x =&gt; new { x.MyProp1, x.MyProp2 }).</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> Properties(Expression<Func<T, object>> projection)
    {
        _inner.Properties(projection);
        return this;
    }

    /// <summary>
    /// Determines whether this instance can persist the specified can persist function.
    /// </summary>
    /// <param name="canPersistFunc">The can persist function.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    public ITrackingConfiguration CanPersist(Func<object, bool> canPersistFunc)
        => _inner.CanPersist(canPersistFunc);

    /// <summary>
    /// Gets the store identifier.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>
    /// A string.
    /// </returns>
    public string GetStoreId(object target)
        => _inner.GetStoreId(target);

    /// <summary>
    /// Identifiers the specified identifier function.
    /// </summary>
    /// <param name="idFunc">The identifier function.</param>
    /// <param name="namespace">The namespace.</param>
    /// <param name="includeType">if set to <c>true</c> [include type].</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.Id(Func<object, string> idFunc, object? @namespace, bool includeType)
    {
        _inner.Id(idFunc, @namespace, includeType);
        return this;
    }

    /// <summary>
    /// Persists the on.
    /// </summary>
    /// <param name="eventNames">The event names.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.PersistOn(params string[] eventNames)
    {
        _inner.PersistOn(eventNames);
        return this;
    }

    /// <summary>
    /// Persists the on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.PersistOn(string eventName, Func<object, object> eventSourceGetter)
    {
        _inner.PersistOn(eventName, eventSourceGetter);
        return this;
    }

    /// <summary>
    /// Persists the on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceObject">The event source object.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.PersistOn(string eventName, object eventSourceObject)
    {
        _inner.PersistOn(eventName, eventSourceObject);
        return this;
    }

    /// <summary>
    /// Stops the tracking on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.StopTrackingOn(string eventName)
    {
        _inner.StopTrackingOn(eventName);
        return this;
    }

    /// <summary>
    /// Stops the tracking on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.StopTrackingOn(string eventName, Func<object, object> eventSourceGetter)
    {
        _inner.StopTrackingOn(eventName, eventSourceGetter);
        return this;
    }

    /// <summary>
    /// Stops the tracking on.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSource">The event source.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.StopTrackingOn(string eventName, object eventSource)
    {
        _inner.StopTrackingOn(eventName, eventSource);
        return this;
    }

    /// <summary>
    /// Whens the state of the applied.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.WhenAppliedState(Action<object> action)
    {
        _inner.WhenAppliedState(action);
        return this;
    }

    /// <summary>
    /// Whens the applying property.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.WhenApplyingProperty(Action<object, PropertyOperationData> action)
    {
        _inner.WhenApplyingProperty(action);
        return this;
    }

    /// <summary>
    /// Whens the persisted.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.WhenPersisted(Action<object> action)
    {
        _inner.WhenPersisted(action);
        return this;
    }

    /// <summary>
    /// Whens the persisting property.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>
    /// ITracking Configuration.
    /// </returns>
    ITrackingConfiguration ITrackingConfiguration.WhenPersistingProperty(Action<object, PropertyOperationData> action)
    {
        _inner.WhenPersistingProperty(action);
        return this;
    }

    private TrackingConfiguration<T> Property<TProperty>(string? name, Expression<Func<T, TProperty?>> propertyAccessExpression, bool defaultSpecified, TProperty defaultValue)
    {
        _inner.Property(name, propertyAccessExpression, defaultSpecified, defaultValue);
        return this;
    }
}
