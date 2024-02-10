// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using CrissCross.WPF.UI.Configuration.Attributes;
using Expression = System.Linq.Expressions.Expression;

namespace CrissCross.WPF.UI.Configuration
{
    /// <summary>
    /// A TrackingConfiguration is an object that determines how a target object will be tracked.
    /// </summary>
    public class TrackingConfiguration : ITrackingConfiguration
    {
        private Func<object, string>? _idFunc;
        private Func<object, bool> _canPersistFunc = _ => true;
        private Action<object, PropertyOperationData>? _applyingPropertyAction;
        private Action<object, PropertyOperationData>? _persistingPropertyAction;
        private Action<object>? _appliedAction;
        private Action<object>? _persistedAction;

        internal TrackingConfiguration()
        {
        }

        internal TrackingConfiguration(
            Tracker tracker,
            Type targetType)
        {
            TargetType = targetType;
            Tracker = tracker;
            _idFunc = target => target.GetType().Name;

            ReadAttributes();
        }

        internal TrackingConfiguration(
            TrackingConfiguration baseConfig,
            Type targetType)
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

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public Type? TargetType { get; }

        /// <summary>
        /// Gets the StateTracker that owns this tracking configuration.
        /// </summary>
        public virtual Tracker? Tracker { get; }

        /// <summary>
        /// Gets a dictionary containing the tracked properties.
        /// </summary>
        /// <value>
        /// The tracked properties.
        /// </value>
        public Dictionary<string, TrackedPropertyInfo> TrackedProperties { get; } = [];

        /// <summary>
        /// Gets list containing the events that will trigger persisting.
        /// </summary>
        public List<Trigger> PersistTriggers { get; } = [];

        /// <summary>
        /// Gets or sets the stop tracking trigger.
        /// </summary>
        /// <value>
        /// The stop tracking trigger.
        /// </value>
        public Trigger? StopTrackingTrigger { get; set; }

        /// <summary>
        /// Allows value conversion and cancallation when applying a stored value to a property.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>ITracking Configuration.</returns>
        public ITrackingConfiguration WhenApplyingProperty(Action<object, PropertyOperationData> action)
        {
            _applyingPropertyAction = action;
            return this;
        }

        /// <summary>
        /// Allows supplying a callback that will be called when all saved state is applied to a target object.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>ITracking Configuration.</returns>
        public ITrackingConfiguration WhenAppliedState(Action<object> action)
        {
            _appliedAction = action;
            return this;
        }

        /// <summary>
        /// Allows value conversion and cancallation when persisting a property of the target object.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>ITracking Configuration.</returns>
        public ITrackingConfiguration WhenPersistingProperty(Action<object, PropertyOperationData> action)
        {
            _persistingPropertyAction = action;
            return this;
        }

        /// <summary>
        /// Whens the persisted.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>ITracking Configuration.</returns>
        public ITrackingConfiguration WhenPersisted(Action<object> action)
        {
            _persistedAction = obj => action(obj);
            return this;
        }

        /// <summary>
        /// Ases the generic.
        /// </summary>
        /// <typeparam name="T">THe type.</typeparam>
        /// <returns>Tracking Configuration.</returns>
        public TrackingConfiguration<T> AsGeneric<T>()
            => new(this);

        /// <summary>
        /// Gets the store identifier.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>A string.</returns>
        public string GetStoreId(object target) => _idFunc!(target);

        /// <summary>
        /// Identifiers the specified identifier function.
        /// </summary>
        /// <param name="idFunc">The provided function will be used to get an identifier for a target object in order to identify the data that belongs to it.</param>
        /// <param name="namespace">Serves to distinguish objects with the same ids that are used in different contexts.</param>
        /// <param name="includeType">If true, the name of the type will be included in the id. This prevents id clashes with different types.</param>
        /// <returns>ITrackingConfiguration.</returns>
        public ITrackingConfiguration Id(Func<object, string> idFunc, object? @namespace = null, bool includeType = true)
        {
            _idFunc = target =>
            {
                var idBuilder = new StringBuilder();
                if (includeType)
                {
                    idBuilder.Append($"[{target.GetType()}]");
                }

                if (@namespace != null)
                {
                    idBuilder.Append($"{@namespace}.");
                }

                idBuilder.Append($"{idFunc(target)}");
                return idBuilder.ToString();
            };

            return this;
        }

        /// <summary>
        /// Determines whether this instance can persist the specified can persist function.
        /// </summary>
        /// <param name="canPersistFunc">The can persist function.</param>
        /// <returns>ITracking Configuration.</returns>
        public ITrackingConfiguration CanPersist(Func<object, bool> canPersistFunc)
        {
            _canPersistFunc = canPersistFunc;
            return this;
        }

        /// <summary>
        /// Registers the specified event of the target object as a trigger that will cause the target's data to be persisted.
        /// </summary>
        /// <param name="eventNames">The names of the events that will cause the target object's data to be persisted.</param>
        /// <returns>
        /// ITrackingConfiguration.
        /// </returns>
        /// <remarks>
        /// Automatically persist a target object when it fires the specified name.
        /// </remarks>
        /// <example>
        /// For a Window object, "LocationChanged" and/or "SizeChanged" would be appropriate.
        /// </example>
        public ITrackingConfiguration PersistOn(params string[] eventNames)
        {
            if (eventNames == null)
            {
                throw new ArgumentNullException(nameof(eventNames));
            }

            foreach (var eventName in eventNames)
            {
                PersistTriggers.Add(new Trigger(eventName, s => s));
            }

            return this;
        }

        /// <summary>
        /// Automatically persist a target object when the specified eventSourceObject fires the specified event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventSourceObject">If not provided.</param>
        /// <returns>
        /// ITrackingConfiguration.
        /// </returns>
        public ITrackingConfiguration PersistOn(string eventName, object eventSourceObject)
        {
            PersistOn(eventName, _ => eventSourceObject);
            return this;
        }

        /// <summary>
        /// Automatically persist a target object when the specified eventSourceObject fires the specified event.
        /// </summary>
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

        /// <summary>
        /// Stop tracking the target when it fires the specified event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>ITrackingConfiguration.</returns>
        public ITrackingConfiguration StopTrackingOn(string eventName) => StopTrackingOn(eventName, target => target);

        /// <summary>
        /// Stop tracking the target when the specified eventSource object fires the specified event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventSource">The event source.</param>
        /// <returns>ITrackingConfiguration.</returns>
        public ITrackingConfiguration StopTrackingOn(string eventName, object eventSource) => StopTrackingOn(eventName, _ => eventSource);

        /// <summary>
        /// Stop tracking the target when the specified eventSource object fires the specified event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventSourceGetter">The event source getter.</param>
        /// <returns>ITracking Configuration.</returns>
        public ITrackingConfiguration StopTrackingOn(string eventName, Func<object, object> eventSourceGetter)
        {
            StopTrackingTrigger = new Trigger(eventName, target => eventSourceGetter(target));
            return this;
        }

        /// <summary>
        /// Set up tracking for the specified property. Allows supplying a name for the property.
        /// This overload is used when the target object has a list of child objects whose properties
        /// it wishes to track. Each child object's properties can be tracked with a different name,
        /// e.g. by including the index in the name.
        /// </summary>
        /// <typeparam name="T">Type of target object.</typeparam>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="propertyAccessExpression">The expression that points to the property to track. Supports accessing properties of nested objects.</param>
        /// <param name="name">Name to use when tracking the property's data.</param>
        /// <returns>
        /// ITracking Configuration.
        /// </returns>
        public ITrackingConfiguration Property<T, TProperty>(Expression<Func<T, TProperty?>> propertyAccessExpression, string? name = null)
        {
            if (propertyAccessExpression == null)
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
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="propertyAccessExpression">The expression that points to the property to track. Supports accessing properties of nested objects.</param>
        /// <param name="defaultValue">If there is no value in the store for the property, the defaultValue will be used.</param>
        /// <param name="name">Name to use when tracking the property's data.</param>
        /// <returns>ITrackingConfiguration.</returns>
        public ITrackingConfiguration Property<T, TProperty>(Expression<Func<T, TProperty?>> propertyAccessExpression, TProperty defaultValue, string? name = null)
        {
            if (propertyAccessExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyAccessExpression));
            }

            if (name == null && propertyAccessExpression?.Body is MemberExpression me)
            {
                // If not specified, use the entire expression as the name of the property.
                // Note: we don't use just the member name because it might conflict with
                // another property that uses a different expression but the same member name
                // e.g. "firstCol.Width" and "secondCol.Width".
                name = me.ToString();
            }

            return Property(name, propertyAccessExpression, true, defaultValue);
        }

        /// <summary>
        /// Set up tracking for one or more properties. The expression should be an anonymous type projection (e.g. x => new { x.MyProp1, x.MyProp2 }).
        /// </summary>
        /// <typeparam name="T">Type of target object.</typeparam>
        /// <param name="projection">A projection of properties to track. Allows providing nested object properties.</param>
        /// <returns>ITrackingConfiguration.</returns>
        public ITrackingConfiguration Properties<T>(Expression<Func<T, object>> projection)
        {
            if (projection?.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException("Expression must project properties as an anonymous class e.g. f => new { f.Height, f.Width } or access a single property e.g. f => f.Text.");
            }

            var newExp = projection.Body as NewExpression;

            // VB.NET encapsulates the new expression in a convert-to-object expression
            if (newExp == null && projection.Body is UnaryExpression ue && ue.NodeType == ExpressionType.Convert && ue.Type == typeof(object))
            {
                newExp = ue.Operand as NewExpression;
            }

            if (newExp != null)
            {
                var accessors = newExp.Members?.Select((m, i) =>
                {
                    var right = Expression.Parameter(typeof(object));
                    var propType = (m as PropertyInfo)?.PropertyType;
                    return new
                    {
                        name = m.Name,
                        type = propType,
                        getter = Expression.Lambda(Expression.Convert((newExp.Arguments[i] as MemberExpression)!, typeof(object)), projection.Parameters[0]).Compile() as Func<T, object>,

                        // todo: call the Convert method instead of using Expression.Convert which will not work for enums
                        setter = Expression.Lambda(Expression.Block(Expression.Assign(newExp.Arguments[i], Expression.Convert(right, propType!)), Expression.Empty()), projection.Parameters[0], right).Compile() as Action<T, object?>
                    };
                });

                foreach (var a in accessors!)
                {
                    TrackedProperties[a.name] = new TrackedPropertyInfo(x => a.getter!((T)x), (x, v) => a.setter!((T)x, Convert(v, a.name, a.type)));
                }
            }
            else
            {
                throw new ArgumentException("Expression must project properties as an anonymous class e.g. f => new { f.Height, f.Width } or access a single property e.g. f => f.Text.");
            }

            return this;
        }

        /// <summary>
        /// Reads the data from the tracked properties and saves it to the data store for the tracked object.
        /// </summary>
        internal void Persist(object target)
        {
            if (_canPersistFunc(target))
            {
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
                        Trace.WriteLine($"Persisting failed, property key = '{name}', property = {propertyName}, message='{ex.Message}'.");
                    }
                }

                Tracker?.Store.SetData(name, values);

                OnStatePersisted(target);
            }
        }

        /// <summary>
        /// Applies any previously stored data to the tracked properties of the target object.
        /// </summary>
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
                        Trace.WriteLine($"TRACKING: Applying tracking to property with key='{propertyName}' failed. ExceptionType:'{ex.GetType().Name}', message: '{ex.Message}'!");
                    }
                }
                else if (descriptor.IsDefaultSpecified)
                {
                    descriptor.Setter!(target, descriptor.DefaultValue);
                }
            }

            OnStateApplied(target);
        }

        /// <summary>
        /// Apply specified defaults to the tracked properties of the target object.
        /// </summary>
        internal void ApplyDefaults(object target)
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

                if (descriptor.IsDefaultSpecified)
                {
                    descriptor.Setter!(target, descriptor.DefaultValue);
                }
            }

            OnStateApplied(target);
        }

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

        internal ITrackingConfiguration Property<T, TProperty>(string? name, Expression<Func<T, TProperty?>>? propertyAccessExpression, bool defaultSpecified, TProperty defaultValue)
        {
            if (name == null && propertyAccessExpression?.Body is MemberExpression me)
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
            var propType = membershipExpression?.Type;
            var setter = Expression.Lambda(Expression.Block(Expression.Assign(membershipExpression!, Expression.Convert(right, membershipExpression?.Type!)), Expression.Empty()), propertyAccessExpression?.Parameters[0]!, right).Compile() as Action<T, object?>;
            if (defaultSpecified)
            {
                TrackedProperties[name!] = new TrackedPropertyInfo(x => getter!((T)x), (x, v) => setter!((T)x, v), defaultValue);
            }
            else
            {
                TrackedProperties[name!] = new TrackedPropertyInfo(x => getter!((T)x), (x, v) => setter!((T)x, v));
            }

            return this;
        }

        private static void SetValue(object target, PropertyInfo pi, object? value)
        {
            var valueToWrite = Convert(value, pi.Name, pi.PropertyType);
            pi.SetValue(target, valueToWrite);
        }

        private static object? Convert(object? value, string? propertyName, Type? t)
        {
            if (value == null)
            {
                if (t?.IsValueType == true)
                {
                    throw new ArgumentException($"Cannot write null into non-nullable property {propertyName}");
                }
            }
            else
            {
                var typeOfValue = value.GetType();

                // This can happen if we're trying to write an Int64 to an Int32 property (in case of overflow it will throw).
                // Also can happen for enums.
                if (typeOfValue != t && t?.IsAssignableFrom(typeOfValue) == false)
                {
                    var converter = TypeDescriptor.GetConverter(t);
                    if (converter.CanConvertFrom(typeOfValue))
                    {
                        return converter.ConvertFrom(value);
                    }
                    else if (t?.IsEnum == true)
                    {
                        return Enum.ToObject(t, value);
                    }
                    else
                    {
                        return System.Convert.ChangeType(value, t!);
                    }
                }
            }

            return value;
        }

        private void ReadAttributes()
        {
            var keyProperty = TargetType?.GetProperties().SingleOrDefault(pi => pi.IsDefined(typeof(TrackingIdAttribute), true));
            if (keyProperty != null)
            {
                _idFunc = (t) => keyProperty.GetValue(t, null)?.ToString()!;
            }

            foreach (var pi in TargetType?.GetProperties()!)
            {
                var propTrackableAtt = pi.GetCustomAttributes(true).OfType<TrackableAttribute>().SingleOrDefault();
                if (propTrackableAtt != null)
                {
                    var defaultAtt = pi.GetCustomAttribute<DefaultValueAttribute>();
                    if (defaultAtt != null)
                    {
                        TrackedProperties[pi.Name] = new TrackedPropertyInfo(x => pi.GetValue(x), (x, v) => SetValue(x, pi, v), defaultAtt.Value);
                    }
                    else
                    {
                        TrackedProperties[pi.Name] = new TrackedPropertyInfo(x => pi.GetValue(x), (x, v) => SetValue(x, pi, v));
                    }
                }
            }

            foreach (var eventInfo in TargetType.GetEvents())
            {
                var attributes = eventInfo.GetCustomAttributes(true);

                if (attributes.OfType<PersistOnAttribute>().Any())
                {
                    PersistOn(eventInfo.Name);
                }

                if (attributes.OfType<StopTrackingOnAttribute>().Any())
                {
                    StopTrackingOn(eventInfo.Name);
                }
            }
        }

        private bool OnApplyingProperty(object target, string property, ref object? value)
        {
            var args = new PropertyOperationData(property, value);
            _applyingPropertyAction?.Invoke(target, args);
            value = args.Value;
            return !args.Cancel;
        }

        private void OnStateApplied(object target) => _appliedAction?.Invoke(target);

        private bool OnPersistingProperty(object target, string property, ref object? value)
        {
            var args = new PropertyOperationData(property, value);
            _persistingPropertyAction?.Invoke(target, args);
            value = args.Value;
            return !args.Cancel;
        }

        private void OnStatePersisted(object target) => _persistedAction?.Invoke(target);
    }
}
