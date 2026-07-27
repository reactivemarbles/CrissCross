// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Groups descriptor-driven property inspector fields by category.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PropertyDescriptorGroup"/> class.
/// </remarks>
/// <param name="name">The category name.</param>
/// <param name="descriptors">The descriptors in the category.</param>
public sealed class PropertyDescriptorGroup(string name, IReadOnlyList<PropertyDescriptorModel>? descriptors = null)
{
    /// <summary>Gets the category name.</summary>
    public string Name { get; } = string.IsNullOrWhiteSpace(name) ? "General" : name.Trim();

    /// <summary>Gets the descriptors in the category.</summary>
    public IReadOnlyList<PropertyDescriptorModel> Descriptors { get; } = descriptors ?? [];

    /// <summary>Gets the number of descriptors in the category.</summary>
    public int Count => Descriptors.Count;

    /// <summary>Gets a value indicating whether the category contains invalid descriptors.</summary>
    public bool HasValidationErrors => HasDescriptor(static descriptor => descriptor.IsInvalid);

    /// <summary>Gets a value indicating whether the category contains modified descriptors.</summary>
    public bool HasModifiedDescriptors => HasDescriptor(static descriptor => descriptor.IsModified);

    /// <summary>Determines whether a descriptor meets the supplied condition.</summary>
    /// <param name="predicate">The condition to evaluate.</param>
    /// <returns><c>true</c> when a descriptor meets the condition; otherwise, <c>false</c>.</returns>
    private bool HasDescriptor(System.Func<PropertyDescriptorModel, bool> predicate)
    {
        foreach (var descriptor in Descriptors)
        {
            if (predicate(descriptor))
            {
                return true;
            }
        }

        return false;
    }
}
