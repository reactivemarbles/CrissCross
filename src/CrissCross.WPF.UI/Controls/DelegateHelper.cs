// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the DelegateHelper member.</summary>
internal static class DelegateHelper
{
    /// <summary>Provides the DefaultLookup member.</summary>
    private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

    /// <summary>Creates a delegate from a reflected static method.</summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="method">The method value.</param>
    /// <returns>The result.</returns>
    public static T CreateDelegate<T>(MethodInfo method)
        where T : Delegate => (T)Delegate.CreateDelegate(typeof(T), method);

    /// <summary>Creates a delegate from a reflected instance method and target.</summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="firstArgument">The firstArgument value.</param>
    /// <param name="method">The method value.</param>
    /// <returns>The result.</returns>
    public static T CreateDelegate<T>(object firstArgument, MethodInfo method)
        where T : Delegate => (T)Delegate.CreateDelegate(typeof(T), firstArgument, method);

    /// <summary>Creates a delegate from a target type and method name.</summary>
    /// <param name="bindingAttr">The bindingAttr value.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="target">The target object.</param>
    /// <param name="method">The method value.</param>
    /// <returns>The result.</returns>
    public static T CreateDelegate<T>(Type target, string method, BindingFlags bindingAttr = DefaultLookup)
        where T : Delegate
    {
        if (bindingAttr != DefaultLookup)
        {
            var methodInfo = target.GetMethod(method, bindingAttr);
            return methodInfo is not null ? CreateDelegate<T>(methodInfo) : null!;
        }

        return (T)Delegate.CreateDelegate(typeof(T), target, method);
    }

    /// <summary>Creates a delegate from a target instance and method name.</summary>
    /// <param name="bindingAttr">The bindingAttr value.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The result.</returns>
    /// <param name="target">The target object.</param>
    /// <param name="method">The method value.</param>
    public static T CreateDelegate<T>(object target, string method, BindingFlags bindingAttr = DefaultLookup)
        where T : Delegate
    {
        if (bindingAttr != DefaultLookup)
        {
            var methodInfo = target.GetType().GetMethod(method, bindingAttr);
            return methodInfo is not null ? CreateDelegate<T>(target, methodInfo) : null!;
        }

        return (T)Delegate.CreateDelegate(typeof(T), target, method);
    }

    /// <summary>Provides the CreatePropertyGetter member.</summary>
    /// <typeparam name="TProperty">The TProperty type.</typeparam>
    /// <param name="nonPublic">The nonPublic value.</param>
    /// <typeparam name="TType">The type that owns the property.</typeparam>
    /// <param name="name">The name value.</param>
    /// <returns>The result.</returns>
    /// <param name="bindingAttr">The bindingAttr value.</param>
    public static Func<TType, TProperty> CreatePropertyGetter<TType, TProperty>(
        string name,
        BindingFlags bindingAttr = DefaultLookup,
        bool nonPublic = false)
    {
        var property = typeof(TType).GetProperty(name, bindingAttr);
        if (property is not null)
        {
            var getMethod = property.GetGetMethod(nonPublic);
            if (getMethod is not null)
            {
                return CreateDelegate<Func<TType, TProperty>>(getMethod);
            }
        }

        return null!;
    }

    /// <summary>Provides the CreatePropertySetter member.</summary>
    /// <typeparam name="TProperty">The TProperty type.</typeparam>
    /// <param name="name">The name value.</param>
    /// <param name="bindingAttr">The bindingAttr value.</param>
    /// <param name="nonPublic">The nonPublic value.</param>
    /// <returns>The result.</returns>
    /// <typeparam name="TType">The type that owns the property.</typeparam>
    public static Action<TType, TProperty> CreatePropertySetter<TType, TProperty>(
        string name,
        BindingFlags bindingAttr = DefaultLookup,
        bool nonPublic = false)
    {
        var property = typeof(TType).GetProperty(name, bindingAttr);
        if (property is not null)
        {
            var setMethod = property.GetSetMethod(nonPublic);
            if (setMethod is not null)
            {
                return CreateDelegate<Action<TType, TProperty>>(setMethod);
            }
        }

        return null!;
    }
}
