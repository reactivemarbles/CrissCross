// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Expression = System.Linq.Expressions.Expression;

namespace CrissCross.WPF.UI.Configuration;

/// <summary>
/// Trigger.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Trigger"/> class.
/// </remarks>
/// <param name="eventName">Name of the event.</param>
/// <param name="sourceGetter">The source getter.</param>
public class Trigger(string eventName, Func<object, object> sourceGetter)
{
    private readonly ConditionalWeakTable<object, Delegate> _handlers = new();

    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    /// <value>
    /// The name of the event.
    /// </value>
    public string EventName { get; } = eventName;

    /// <summary>
    /// Gets the source getter.
    /// </summary>
    /// <value>
    /// The source getter.
    /// </value>
    public Func<object, object> SourceGetter { get; } = sourceGetter;

    /// <summary>
    /// Subscribes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="action">The action.</param>
    /// <exception cref="ArgumentException">Event '{EventName}' not found on target of type '{source.GetType().Name}'. Check the tracking configuration for this type.</exception>
    public void Subscribe(object target, Action action)
    {
        // clear a possible previous subscription for the same target/event
        Unsubscribe(target);

        var source = SourceGetter(target);

        var eventInfo = source.GetType().GetEvent(EventName) ?? throw new ArgumentException($"Event '{EventName}' not found on target of type '{source.GetType().Name}'. Check the tracking configuration for this type.");
        var parameters = eventInfo.EventHandlerType?
            .GetMethod("Invoke")?
            .GetParameters()
            .Select(parameter => Expression.Parameter(parameter.ParameterType))
            .ToArray();

        var handler = Expression.Lambda(
                eventInfo.EventHandlerType!,
                Expression.Call(Expression.Constant(action), "Invoke", Type.EmptyTypes),
                parameters)
          .Compile();

        eventInfo.AddEventHandler(source, handler);

        _handlers.Add(target, handler);
    }

    /// <summary>
    /// Unsubscribes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    public void Unsubscribe(object target)
    {
        if (_handlers.TryGetValue(target, out var handler))
        {
            var source = SourceGetter(target);
            var eventInfo = source.GetType().GetEvent(EventName);
            eventInfo?.RemoveEventHandler(source, handler);
            _handlers.Remove(target);
        }
    }

    internal void Subscribe<T>(T target, object p) => throw new NotImplementedException();
}
