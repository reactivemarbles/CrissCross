// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Expression = System.Linq.Expressions.Expression;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Configuration;
#else
namespace CrissCross.WPF.UI.Configuration;
#endif

/// <summary>Represents Trigger.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Trigger"/> class.
/// </remarks>
/// <param name="eventName">Name of the event.</param>
/// <param name="sourceGetter">The source getter.</param>
public class Trigger(string eventName, Func<object, object> sourceGetter)
{
    /// <summary>Stores the _handlers value.</summary>
    private readonly ConditionalWeakTable<object, Delegate> _handlers = new();

    /// <summary>Gets the name of the event.</summary>
    /// <value>
    /// The name of the event.
    /// </value>
    public string EventName { get; } = eventName;

    /// <summary>Gets the source getter.</summary>
    /// <value>
    /// The source getter.
    /// </value>
    public Func<object, object> SourceGetter { get; } = sourceGetter;

    /// <summary>Subscribes the specified target.</summary>
    /// <exception cref="ArgumentException">Event '{EventName}' not found on target of type '{source.GetType().Name}'.
    /// Check the tracking configuration for this type.</exception>
    /// <param name="target">The target.</param>
    /// <param name="action">The action.</param>
    public void Subscribe(object target, Action action) => _ = SubscribeDisposable(target, action);

    /// <summary>Subscribes the specified target and returns an <see cref="IDisposable"/> to unsubscribe.</summary>
    /// <param name="target">The target.</param>
    /// <param name="action">The action.</param>
    /// <returns>An <see cref="IDisposable"/> which when disposed will unsubscribe.</returns>
    public IDisposable SubscribeDisposable(object target, Action action)
    {
        // clear a possible previous subscription for the same target/event
        Unsubscribe(target);

        var source = SourceGetter(target);

        var eventInfo =
            source.GetType().GetEvent(EventName)
            ?? throw new ArgumentException(
                $"Event '{EventName}' not found on target of type '{source.GetType().Name}'. "
                    + "Check the tracking configuration for this type.");
        var parameters = eventInfo
            .EventHandlerType?.GetMethod("Invoke")
            ?.GetParameters()
            .Select(parameter => Expression.Parameter(parameter.ParameterType))
            .ToArray();

        var handler = Expression
            .Lambda(
                eventInfo.EventHandlerType!,
                Expression.Call(Expression.Constant(action), nameof(Action.Invoke), Type.EmptyTypes),
                parameters)
            .Compile();

        eventInfo.AddEventHandler(source, handler);

        _handlers.Add(target, handler);
        return new ActionDisposable(() => Unsubscribe(target));
    }

    /// <summary>Unsubscribes the specified target.</summary>
    /// <param name="target">The target.</param>
    public void Unsubscribe(object target)
    {
        if (!_handlers.TryGetValue(target, out var handler))
        {
            return;
        }

        var source = SourceGetter(target);
        var eventInfo = source.GetType().GetEvent(EventName);
        eventInfo?.RemoveEventHandler(source, handler);
        _ = _handlers.Remove(target);
    }
}
