// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Configuration;
#else
namespace CrissCross.WPF.UI.Configuration;
#endif

/// <summary>An object that decribes the tracking information for a target object's property.</summary>
/// <param name="Getter"> Gets function that gets the value of the property. </param>
/// <param name="Setter"> Gets action that sets the value of the property. </param>
/// <param name="IsDefaultSpecified"> Gets a value indicating whether indicates if a default value is provided for the
/// property. </param>
/// <param name="DefaultValue"> Gets the value that will be applied to a tracked property if no existing persisted data
/// is found. </param>
public sealed record TrackedPropertyInfo(
    Func<object, object?>? Getter,
    Action<object, object?>? Setter,
    bool IsDefaultSpecified,
    object? DefaultValue)
{
    /// <summary>Initializes a new instance of the <see cref="TrackedPropertyInfo"/> class.</summary>
    /// <param name="getter">The getter value.</param>
    /// <param name="setter">The setter value.</param>
    internal TrackedPropertyInfo(Func<object, object?>? getter, Action<object, object?>? setter)
        : this(getter, setter, false, null) { }

    /// <summary>Initializes a new instance of the <see cref="TrackedPropertyInfo"/> class.</summary>
    /// <param name="getter">The getter value.</param>
    /// <param name="setter">The setter value.</param>
    /// <param name="defaultValue">The defaultvalue.</param>
    internal TrackedPropertyInfo(Func<object, object?>? getter, Action<object, object?>? setter, object? defaultValue)
        : this(getter, setter, true, defaultValue) { }
}
