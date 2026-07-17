// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

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
    public bool HasValidationErrors => Descriptors.Any(static descriptor => descriptor.IsInvalid);

    /// <summary>Gets a value indicating whether the category contains modified descriptors.</summary>
    public bool HasModifiedDescriptors => Descriptors.Any(static descriptor => descriptor.IsModified);
}
