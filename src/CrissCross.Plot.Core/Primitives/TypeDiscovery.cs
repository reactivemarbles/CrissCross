// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CrissCross.Plot;

/// <summary>Discovers and creates concrete implementations of a requested interface.</summary>
internal static class TypeDiscovery
{
    /// <summary>Creates instances of concrete implementations found in an assembly.</summary>
    /// <typeparam name="T">The interface implemented by the discovered types.</typeparam>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>The created implementation instances.</returns>
    [RequiresUnreferencedCode("Discovers implementations by reflecting over assembly types.")]
    internal static T[] GetImplementations<T>(Assembly assembly)
        where T : class
    {
        ArgumentChecks.ThrowIfNull(assembly, nameof(assembly));

        List<T> instances = [];
        foreach (Type type in assembly.GetTypes())
        {
            if (!IsConcreteImplementation<T>(type) || !HasPublicParameterlessConstructor(type))
            {
                continue;
            }

            if (Activator.CreateInstance(type) is not T instance)
            {
                throw new InvalidOperationException($"Type '{type.FullName}' could not be instantiated as {typeof(T).Name}.");
            }

            instances.Add(instance);
        }

        return [.. instances];
    }

    /// <summary>Gets whether a type is a concrete implementation of the requested interface.</summary>
    /// <typeparam name="T">The interface to check.</typeparam>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> when the type is a concrete implementation.</returns>
    private static bool IsConcreteImplementation<T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type)
    {
        if (!type.IsClass || type.IsAbstract)
        {
            return false;
        }

        foreach (Type implementedInterface in type.GetInterfaces())
        {
            if (implementedInterface == typeof(T))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets whether a type has a public parameterless constructor.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> when a public parameterless constructor exists.</returns>
    private static bool HasPublicParameterlessConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
    {
        foreach (ConstructorInfo constructor in type.GetConstructors())
        {
            if (constructor.GetParameters().Length == 0)
            {
                return true;
            }
        }

        return false;
    }
}
