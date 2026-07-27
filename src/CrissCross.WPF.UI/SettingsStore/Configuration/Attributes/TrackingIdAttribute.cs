// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Configuration.Attributes;
#else
namespace CrissCross.WPF.UI.Configuration.Attributes;
#endif

/// <summary>Represents TrackingIdAttribute.</summary>
/// <param name="namespace">The namespace.</param>
/// <param name="includeType">if set to <c>true</c> [include type].</param>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="TrackingIdAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class TrackingIdAttribute(object? @namespace = null, bool includeType = false) : Attribute
{
    /// <summary>Gets a value indicating whether [include type].</summary>
    /// <value>
    ///   <c>true</c> if [include type]; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeType { get; } = includeType;

    /// <summary>Gets the namespace.</summary>
    /// <value>
    /// The namespace.
    /// </value>
    public object? Namespace { get; } = @namespace;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
