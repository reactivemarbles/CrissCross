// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Expression = System.Linq.Expressions.Expression;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Configuration;
#else
namespace CrissCross.WPF.UI.Configuration;
#endif

/// <summary>A TrackingConfiguration is an object that determines how a target object will be tracked.</summary>
public class TrackingConfiguration : ITrackingConfiguration
{
    /// <summary>Stores the _idFunc value.</summary>
    private Func<object, string>? _idFunc;

    /// <summary>Stores the _canPersistFunc value.</summary>
    private Func<object, bool> _canPersistFunc = _ => true;

    /// <summary>Stores the _applyingPropertyAction value.</summary>
    private Action<object, PropertyOperationData>? _applyingPropertyAction;

    /// <summary>Stores the _persistingPropertyAction value.</summary>
    private Action<object, PropertyOperationData>? _persistingPropertyAction;

    /// <summary>Stores the _appliedAction value.</summary>
    private Action<object>? _appliedAction;

    /// <summary>Stores the _persistedAction value.</summary>
    private Action<object>? _persistedAction;

    /// <summary>Initializes a new instance of the <see cref="TrackingConfiguration"/> class.</summary>
    internal TrackingConfiguration() { }

    /// <summary>Initializes a new instance of the <see cref="TrackingConfiguration"/> class.</summary>
    /// <param name="tracker">The tracker value.</param>
    /// <param name="targetType">The targetType value.</param>
    internal TrackingConfiguration(Tracker tracker, Type targetType)
    {
        TargetType = targetType;
        Tracker = tracker;
        _idFunc = target => target.GetType().Name;

        ReadAttributes();
    }

    /// <summary>Initializes a new instance of the <see cref="TrackingConfiguration"/> class.</summary>
    /// <param name="baseConfig">The baseConfig value.</param>
    /// <param name="targetType">The targetType value.</param>
    internal TrackingConfiguration(TrackingConfiguration baseConfig, Type targetType)
    {
        TargetType = targetType;
        Tracker = baseConfig.Tracker;

        _idFunc = baseConfig._idFunc;

        _appliedAction = baseConfig._appliedAction;
        _persistedAction = baseConfig._persistedAction;
        _applyingPropertyAction = baseConfig._applyingPropertyAction;
        _persistingPropertyAction = baseConfig._persistingPropertyAction;

        foreach (var kvp in baseConfig.TrackedProperties)
        {
            TrackedProperties.Add(kvp.Key, kvp.Value);
        }

        PersistTriggers.AddRange(baseConfig.PersistTriggers);

        ReadAttributes();
    }

    /// <summary>Gets the type of the target.</summary>
    /// <value>
    /// The type of the target.
    /// </value>
    public Type? TargetType { get; }

    /// <summary>Gets the StateTracker that owns this tracking configuration.</summary>
    public Tracker? Tracker { get; }

    /// <summary>Gets or sets the value.</summary>
    /// <value>
    /// The tracked properties.
    /// </value>
    public Dictionary<string, TrackedPropertyInfo> TrackedProperties { get; } = [];

    /// <summary>Gets or sets the value.</summary>
    public List<Trigger> PersistTriggers { get; } = [];

    /// <summary>Gets or sets the stop tracking trigger.</summary>
    /// <value>
    /// The stop tracking trigger.
    /// </value>
    public Trigger? StopTrackingTrigger { get; set; }

    /// <summary>Allows value conversion and cancallation when applying a stored value to a property.</summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration WhenApplyingProperty(Action<object, PropertyOperationData> action)
    {
        _applyingPropertyAction = action;
        return this;
    }

    /// <summary>Provides the WhenAppliedState member.</summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration WhenAppliedState(Action<object> action)
    {
        _appliedAction = action;
        return this;
    }

    /// <summary>Allows value conversion and cancallation when persisting a property of the target object.</summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration WhenPersistingProperty(Action<object, PropertyOperationData> action)
    {
        _persistingPropertyAction = action;
        return this;
    }

    /// <summary>Whens the persisted.</summary>
    /// <param name="action">The action.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration WhenPersisted(Action<object> action)
    {
        _persistedAction = obj => action(obj);
        return this;
    }

    /// <summary>Ases the generic.</summary>
    /// <typeparam name="T">THe type.</typeparam>
    /// <param name="request">The typed tracking request.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> AsGeneric<T>(TrackingRequest<T> request) => new(this);

    /// <summary>Gets the store identifier.</summary>
    /// <param name="target">The target.</param>
    /// <returns>A string.</returns>
    public string GetStoreId(object target) => _idFunc!(target);

    /// <summary>Identifiers the specified identifier function.</summary>
    /// <param name="idFunc">The provided function will be used to get an identifier for a target object in order to
    /// identify the data that belongs to it.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration Id(Func<object, string> idFunc) => Id(idFunc, null, true);

    /// <summary>Identifiers the specified identifier function.</summary>
    /// <param name="idFunc">The identifier function.</param>
    /// <param name="namespace">The optional namespace.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration Id(Func<object, string> idFunc, object? @namespace) => Id(idFunc, @namespace, true);

    /// <summary>Identifiers the specified identifier function.</summary>
    /// <param name="idFunc">The identifier function.</param>
    /// <param name="namespace">Serves to distinguish objects with the same ids that are used in different
    /// contexts.</param>
    /// <param name="includeType">Whether the type name is included in the identifier.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration Id(Func<object, string> idFunc, object? @namespace, bool includeType)
    {
        _idFunc = target =>
        {
            var idBuilder = new StringBuilder();
            if (includeType)
            {
                _ = idBuilder.Append($"[{target.GetType()}]");
            }

            if (@namespace is not null)
            {
                _ = idBuilder.Append($"{@namespace}.");
            }

            _ = idBuilder.Append($"{idFunc(target)}");
            return idBuilder.ToString();
        };

        return this;
    }

    /// <summary>Determines whether this instance can persist the specified can persist function.</summary>
    /// <param name="canPersistFunc">The can persist function.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration CanPersist(Func<object, bool> canPersistFunc)
    {
        _canPersistFunc = canPersistFunc;
        return this;
    }

    /// <summary>Provides the PersistOn member.</summary>
    /// <remarks>
    /// Automatically persist a target object when it fires the specified name.
    /// </remarks>
    /// <example>
    /// For a Window object, "LocationChanged" and/or "SizeChanged" would be appropriate.
    /// </example>
    /// <param name="eventNames">The names of the events that will cause the target object's data to be
    /// persisted.</param>
    /// <returns>
    /// ITrackingConfiguration.
    /// </returns>
    public ITrackingConfiguration PersistOn(params string[] eventNames)
    {
        if (eventNames is null)
        {
            throw new ArgumentNullException(nameof(eventNames));
        }

        foreach (var eventName in eventNames)
        {
            PersistTriggers.Add(new Trigger(eventName, s => s));
        }

        return this;
    }

    /// <summary>Provides the PersistOn member.</summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceObject">If not provided.</param>
    /// <returns>
    /// ITrackingConfiguration.
    /// </returns>
    public ITrackingConfiguration PersistOn(string eventName, object eventSourceObject)
    {
        _ = PersistOn(eventName, _ => eventSourceObject);
        return this;
    }

    /// <summary>Provides the PersistOn member.</summary>
    /// <param name="eventName">The name of the event that should trigger persisting stete.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>
    /// ITrackingConfiguration.
    /// </returns>
    public ITrackingConfiguration PersistOn(string eventName, Func<object, object> eventSourceGetter)
    {
        PersistTriggers.Add(new Trigger(eventName, target => eventSourceGetter(target)));
        return this;
    }

    /// <summary>Stop tracking the target when it fires the specified event.</summary>
    /// <param name="eventName">Name of the event.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration StopTrackingOn(string eventName) => StopTrackingOn(eventName, target => target);

    /// <summary>Stop tracking the target when the specified eventSource object fires the specified event.</summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSource">The event source.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration StopTrackingOn(string eventName, object eventSource) =>
        StopTrackingOn(eventName, _ => eventSource);

    /// <summary>Stop tracking the target when the specified eventSource object fires the specified event.</summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventSourceGetter">The event source getter.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration StopTrackingOn(string eventName, Func<object, object> eventSourceGetter)
    {
        StopTrackingTrigger = new(eventName, target => eventSourceGetter(target));
        return this;
    }

    /// <summary>
    /// Set up tracking for the specified property. Allows supplying a name for the property.
    /// This overload is used when the target object has a list of child objects whose properties
    /// it wishes to track. Each child object's properties can be tracked with a different name,
    /// e.g. by including the index in the name.
    /// </summary>
    /// <typeparam name="T">Type of target object.</typeparam>
    /// <typeparam name="TProperty">The tracked property type.</typeparam>
    /// <param name="propertyAccessExpression">The expression that points to the property to track. Supports accessing
    /// properties of nested objects.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration Property<T, TProperty>(Expression<Func<T, TProperty?>> propertyAccessExpression) =>
        Property(null, propertyAccessExpression, false, default);

    /// <summary>Sets up tracking for the specified property.</summary>
    /// <typeparam name="T">Type of target object.</typeparam>
    /// <typeparam name="TProperty">The tracked property type.</typeparam>
    /// <param name="propertyAccessExpression">The expression that points to the property to track.</param>
    /// <param name="name">Name to use when tracking the property's data.</param>
    /// <returns>ITracking Configuration.</returns>
    public ITrackingConfiguration Property<T, TProperty>(
        Expression<Func<T, TProperty?>> propertyAccessExpression,
        string? name)
    {
        if (propertyAccessExpression is null)
        {
            throw new ArgumentNullException(nameof(propertyAccessExpression));
        }

        return Property(name, propertyAccessExpression, false, default);
    }

    /// <summary>
    /// Set up tracking for the specified property. Allows supplying a name for the property.
    /// This overload is used when the target object has a list of child objects whose properties
    /// it wishes to track. Each child object's properties can be tracked with a different name,
    /// e.g. by including the index in the name.
    /// </summary>
    /// <typeparam name="T">Type of target object.</typeparam>
    /// <typeparam name="TProperty">The tracked property type.</typeparam>
    /// <param name="propertyAccessExpression">The expression that points to the property to track. Supports accessing
    /// properties of nested objects.</param>
    /// <param name="defaultValue">If there is no value in the store for the property, the defaultValue will be
    /// used.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration Property<T, TProperty>(
        Expression<Func<T, TProperty?>> propertyAccessExpression,
        TProperty defaultValue) => Property(propertyAccessExpression, defaultValue, null);

    /// <summary>Sets up tracking for the specified property and default value.</summary>
    /// <typeparam name="T">Type of target object.</typeparam>
    /// <typeparam name="TProperty">The tracked property type.</typeparam>
    /// <param name="propertyAccessExpression">The expression that points to the property to track.</param>
    /// <param name="defaultValue">The default property value.</param>
    /// <param name="name">Name to use when tracking the property's data.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration Property<T, TProperty>(
        Expression<Func<T, TProperty?>> propertyAccessExpression,
        TProperty defaultValue,
        string? name)
    {
        if (propertyAccessExpression is null)
        {
            throw new ArgumentNullException(nameof(propertyAccessExpression));
        }

        if (name is null && propertyAccessExpression.Body is MemberExpression me)
        {
            // If not specified, use the entire expression as the name of the property.
            // Note: we don't use just the member name because it might conflict with
            // another property that uses a different expression but the same member name
            // e.g. "firstCol.Width" and "secondCol.Width".
            name = me.ToString();
        }

        return Property(name, propertyAccessExpression, true, defaultValue);
    }

    /// <summary>Set up tracking for one or more properties. The expression should be an anonymous type projection (e.g.
    /// x => new { x.MyProp1, x.MyProp2 }).</summary>
    /// <typeparam name="T">Type of target object.</typeparam>
    /// <param name="projection">A projection of properties to track. Allows providing nested object properties.</param>
    /// <returns>ITrackingConfiguration.</returns>
    public ITrackingConfiguration Properties<T>(Expression<Func<T, object>> projection)
    {
        if (projection?.Body.NodeType != ExpressionType.New)
        {
            throw new ArgumentException(
                "Expression must project properties as an anonymous class, for example "
                    + "f => new { f.Height, f.Width }, or access one property, for example f => f.Text.");
        }

        var newExp = projection.Body as NewExpression;

        // VB.NET encapsulates the new expression in a convert-to-object expression
        if (
            newExp is null
            && projection.Body is UnaryExpression ue
            && ue.NodeType == ExpressionType.Convert
            && ue.Type == typeof(object))
        {
            newExp = ue.Operand as NewExpression;
        }

        if (newExp is not null)
        {
            var accessors = newExp.Members?.Select(
                (m, i) =>
                {
                    var right = Expression.Parameter(typeof(object));
                    var propType = (m as PropertyInfo)?.PropertyType;
                    return (
                        name: m.Name,
                        type: propType,
                        getter: Expression
                            .Lambda(
                                Expression.Convert((newExp.Arguments[i] as MemberExpression)!, typeof(object)),
                                projection.Parameters[0])
                            .Compile() as Func<T, object>,
                        setter: Expression
                            .Lambda(
                                Expression.Block(
                                    Expression.Assign(newExp.Arguments[i], Expression.Convert(right, propType!)),
                                    Expression.Empty()),
                                projection.Parameters[0],
                                right)
                            .Compile() as Action<T, object?>);
                });

            foreach (var a in accessors!)
            {
                TrackedProperties[a.name] = new(
                    x => a.getter!((T)x),
                    (x, v) => a.setter!((T)x, Convert(v, a.name, a.type)));
            }
        }
        else
        {
            throw new ArgumentException(
                "Expression must project properties as an anonymous class, for example "
                    + "f => new { f.Height, f.Width }, or access one property, for example f => f.Text.");
        }

        return this;
    }

    /// <summary>Provides the Persist member.</summary>
    /// <param name="target">The target object.</param>
    internal void Persist(object target)
    {
        if (!_canPersistFunc(target))
        {
            return;
        }

        var name = _idFunc!(target);

        IDictionary<string, object?>? originalValues = null;
        var values = new Dictionary<string, object?>();
        foreach (var propertyName in TrackedProperties.Keys)
        {
            try
            {
                var value = TrackedProperties[propertyName]?.Getter!(target);
                var shouldPersist = OnPersistingProperty(target, propertyName, ref value);
                if (shouldPersist)
                {
                    values[propertyName] = value;
                }
                else
                {
                    // keeping previously stored value in case persist cancelled
                    originalValues ??= Tracker?.Store.GetData(name);
                    values[propertyName] = originalValues?[propertyName];
                    Trace.WriteLine($"Persisting cancelled, key='{name}', property='{propertyName}'.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(
                    $"Persisting failed, property key = '{name}', property = {propertyName}, message='{ex.Message}'.");
            }
        }

        Tracker?.Store.SetData(name, values);

        OnStatePersisted(target);
    }

    /// <summary>Applies any previously stored data to the tracked properties of the target object.</summary>
    /// <param name="target">The target object.</param>
    internal void Apply(object target)
    {
        if (TrackedProperties.Count == 0)
        {
            return;
        }

        var name = _idFunc!(target);
        var data = Tracker?.Store.GetData(name);

        foreach (var propertyName in TrackedProperties.Keys)
        {
            var descriptor = TrackedProperties[propertyName];

            if (data?.ContainsKey(propertyName) == true)
            {
                try
                {
                    var value = data[propertyName];
                    var shouldApply = OnApplyingProperty(target, propertyName, ref value);
                    if (shouldApply)
                    {
                        descriptor.Setter!(target, value);
                    }
                    else
                    {
                        Trace.WriteLine($"Persisting cancelled, key='{name}', property='{propertyName}'.");
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(
                        $"TRACKING: Applying tracking to property with key='{propertyName}' failed. "
                            + $"ExceptionType:'{ex.GetType().Name}', message: '{ex.Message}'!");
                }
            }
            else if (descriptor.IsDefaultSpecified)
            {
                descriptor.Setter!(target, descriptor.DefaultValue);
            }
        }

        OnStateApplied(target);
    }

    /// <summary>Apply specified defaults to the tracked properties of the target object.</summary>
    /// <param name="target">The target object.</param>
    internal void ApplyDefaults(object target)
    {
        if (TrackedProperties.Count == 0)
        {
            return;
        }

        var name = _idFunc!(target);
        _ = Tracker?.Store.GetData(name);

        foreach (var propertyName in TrackedProperties.Keys)
        {
            var descriptor = TrackedProperties[propertyName];

            if (descriptor.IsDefaultSpecified)
            {
                descriptor.Setter!(target, descriptor.DefaultValue);
            }
        }

        OnStateApplied(target);
    }

    /// <summary>Provides the StopTracking member.</summary>
    /// <param name="target">The target object.</param>
    internal void StopTracking(object target)
    {
        // unsubscribe from all trigger events
        foreach (var trigger in PersistTriggers)
        {
            trigger.Unsubscribe(target);
        }

        // unsubscribe from stoptracking trigger too
        StopTrackingTrigger?.Unsubscribe(target);

        Tracker?.RemoveFromList(target);
    }

    /// <summary>Provides the StartTracking member.</summary>
    /// <param name="target">The target object.</param>
    internal void StartTracking(object target)
    {
        // listen for trigger events (for persisting)
        foreach (var trigger in PersistTriggers)
        {
            trigger.Subscribe(target, () => Persist(target));
        }

        // listen to stoptracking event
        StopTrackingTrigger?.Subscribe(target, () => StopTracking(target));
    }

    /// <summary>Configures tracking for a property expression.</summary>
    /// <typeparam name="T">The tracked object type.</typeparam>
    /// <typeparam name="TProperty">The TProperty type.</typeparam>
    /// <param name="name">The name value.</param>
    /// <param name="propertyAccessExpression">The propertyAccessExpression value.</param>
    /// <param name="defaultSpecified">The defaultSpecified value.</param>
    /// <param name="defaultValue">The defaultvalue.</param>
    /// <returns>The result.</returns>
    internal ITrackingConfiguration Property<T, TProperty>(
        string? name,
        Expression<Func<T, TProperty?>>? propertyAccessExpression,
        bool defaultSpecified,
        TProperty defaultValue)
    {
        if (name is null && propertyAccessExpression?.Body is MemberExpression me)
        {
            // If not specified, use the entire expression as the name of the property.
            // Note: we don't use just the member name because it might conflict with
            // another property that uses a different expression but the same member name
            // e.g. "firstCol.Width" and "secondCol.Width".
            name = me.ToString();
        }

        var membershipExpression = propertyAccessExpression?.Body;
        var getter = propertyAccessExpression?.Compile();

        var right = Expression.Parameter(typeof(object));
        _ = membershipExpression?.Type;
        var setter =
            Expression
                .Lambda(
                    Expression.Block(
                        Expression.Assign(
                            membershipExpression!,
                            Expression.Convert(right, membershipExpression?.Type!)),
                        Expression.Empty()),
                    propertyAccessExpression?.Parameters[0]!,
                    right)
                .Compile() as Action<T, object?>;
        if (defaultSpecified)
        {
            TrackedProperties[name!] = new(x => getter!((T)x), (x, v) => setter!((T)x, v), defaultValue);
        }
        else
        {
            TrackedProperties[name!] = new(x => getter!((T)x), (x, v) => setter!((T)x, v));
        }

        return this;
    }

    /// <summary>Provides the SetValue member.</summary>
    /// <param name="target">The target object.</param>
    /// <param name="pi">The pi value.</param>
    /// <param name="value">The value.</param>
    private static void SetValue(object target, PropertyInfo pi, object? value)
    {
        var valueToWrite = Convert(value, pi.Name, pi.PropertyType);
        pi.SetValue(target, valueToWrite);
    }

    /// <summary>Provides the Convert member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <param name="t">The t value.</param>
    /// <returns>The result.</returns>
    private static object? Convert(object? value, string? propertyName, Type? t)
    {
        if (value is null)
        {
            if (t?.IsValueType == true)
            {
                throw new ArgumentException($"Cannot write null into non-nullable property {propertyName}");
            }
        }
        else
        {
            var typeOfValue = value.GetType();

            // This can happen if we're trying to write an Int64 to an Int32 property (in case of overflow it will
            // throw).
            // Also can happen for enums.
            if (typeOfValue != t && t?.IsAssignableFrom(typeOfValue) == false)
            {
                var converter = TypeDescriptor.GetConverter(t);
                if (converter.CanConvertFrom(typeOfValue))
                {
                    return converter.ConvertFrom(value);
                }

                return t?.IsEnum == true ? Enum.ToObject(t, value) : System.Convert.ChangeType(value, t!);
            }
        }

        return value;
    }

    /// <summary>Provides the ReadAttributes member.</summary>
    private void ReadAttributes()
    {
        var keyProperty = TargetType
            ?.GetProperties()
            .SingleOrDefault(pi => pi.IsDefined(typeof(TrackingIdAttribute), true));
        if (keyProperty is not null)
        {
            _idFunc = (t) => keyProperty.GetValue(t, null)?.ToString()!;
        }

        foreach (var pi in TargetType?.GetProperties()!)
        {
            var propTrackableAtt = pi.GetCustomAttributes(true).OfType<TrackableAttribute>().SingleOrDefault();
            if (propTrackableAtt is not null)
            {
                var defaultAtt = pi.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultAtt is not null)
                {
                    TrackedProperties[pi.Name] = new(pi.GetValue, (x, v) => SetValue(x, pi, v), defaultAtt.Value);
                }
                else
                {
                    TrackedProperties[pi.Name] = new(pi.GetValue, (x, v) => SetValue(x, pi, v));
                }
            }
        }

        foreach (var eventInfo in TargetType.GetEvents())
        {
            var attributes = eventInfo.GetCustomAttributes(true);

            if (attributes.OfType<PersistOnAttribute>().Any())
            {
                _ = PersistOn(eventInfo.Name);
            }

            if (attributes.OfType<StopTrackingOnAttribute>().Any())
            {
                _ = StopTrackingOn(eventInfo.Name);
            }
        }
    }

    /// <summary>Provides the OnApplyingProperty member.</summary>
    /// <param name="target">The target object.</param>
    /// <param name="property">The property value.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private bool OnApplyingProperty(object target, string property, ref object? value)
    {
        var args = new PropertyOperationData(property, value);
        _applyingPropertyAction?.Invoke(target, args);
        value = args.Value;
        return !args.Cancel;
    }

    /// <summary>Provides the OnStateApplied member.</summary>
    /// <param name="target">The target object.</param>
    private void OnStateApplied(object target) => _appliedAction?.Invoke(target);

    /// <summary>Provides the OnPersistingProperty member.</summary>
    /// <param name="target">The target object.</param>
    /// <param name="property">The property value.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private bool OnPersistingProperty(object target, string property, ref object? value)
    {
        var args = new PropertyOperationData(property, value);
        _persistingPropertyAction?.Invoke(target, args);
        value = args.Value;
        return !args.Cancel;
    }

    /// <summary>Provides the OnStatePersisted member.</summary>
    /// <param name="target">The target object.</param>
    private void OnStatePersisted(object target) => _persistedAction?.Invoke(target);
}
