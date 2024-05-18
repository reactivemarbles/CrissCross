// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

internal static class DelegateHelper
{
    private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

    public static T CreateDelegate<T>(MethodInfo method)
        where T : Delegate => (T)Delegate.CreateDelegate(typeof(T), method);

    public static T CreateDelegate<T>(object firstArgument, MethodInfo method)
        where T : Delegate => (T)Delegate.CreateDelegate(typeof(T), firstArgument, method);

    public static T CreateDelegate<T>(Type target, string method, BindingFlags bindingAttr = DefaultLookup)
        where T : Delegate
    {
        if (bindingAttr != DefaultLookup)
        {
            var methodInfo = target.GetMethod(method, bindingAttr);
            if (methodInfo != null)
            {
                return CreateDelegate<T>(methodInfo);
            }

            return null!;
        }

        return (T)Delegate.CreateDelegate(typeof(T), target, method);
    }

    public static T CreateDelegate<T>(object target, string method, BindingFlags bindingAttr = DefaultLookup)
        where T : Delegate
    {
        if (bindingAttr != DefaultLookup)
        {
            var methodInfo = target.GetType().GetMethod(method, bindingAttr);
            if (methodInfo != null)
            {
                return CreateDelegate<T>(target, methodInfo);
            }

            return null!;
        }

        return (T)Delegate.CreateDelegate(typeof(T), target, method);
    }

    public static Func<TType, TProperty> CreatePropertyGetter<TType, TProperty>(string name, BindingFlags bindingAttr = DefaultLookup, bool nonPublic = false)
    {
        var property = typeof(TType).GetProperty(name, bindingAttr);
        if (property != null)
        {
            var getMethod = property.GetGetMethod(nonPublic);
            if (getMethod != null)
            {
                return CreateDelegate<Func<TType, TProperty>>(getMethod);
            }
        }

        return null!;
    }

    public static Action<TType, TProperty> CreatePropertySetter<TType, TProperty>(string name, BindingFlags bindingAttr = DefaultLookup, bool nonPublic = false)
    {
        var property = typeof(TType).GetProperty(name, bindingAttr);
        if (property != null)
        {
            var setMethod = property.GetSetMethod(nonPublic);
            if (setMethod != null)
            {
                return CreateDelegate<Action<TType, TProperty>>(setMethod);
            }
        }

        return null!;
    }
}
