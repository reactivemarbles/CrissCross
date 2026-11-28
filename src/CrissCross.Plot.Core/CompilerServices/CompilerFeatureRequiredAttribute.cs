// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>Indicates that a compiler feature is required when targeting older .NET versions.</summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="CompilerFeatureRequiredAttribute"/> class.</summary>
    /// <param name="featureName">The required compiler feature name.</param>
    public CompilerFeatureRequiredAttribute(string featureName)
    {
        FeatureName = featureName;
    }

    /// <summary>Gets the required compiler feature name.</summary>
    public string FeatureName { get; }

    /// <summary>Gets or sets a value indicating whether the feature requirement is optional.</summary>
    public bool IsOptional { get; set; }
}
