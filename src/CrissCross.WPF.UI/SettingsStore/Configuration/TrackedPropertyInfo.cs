// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Configuration;

/// <summary>
/// An object that decribes the tracking information for a target object's property.
/// </summary>
/// <param name="Getter"> Gets function that gets the value of the property. </param>
/// <param name="Setter"> Gets action that sets the value of the property. </param>
/// <param name="IsDefaultSpecified"> Gets a value indicating whether indicates if a default value is provided for the property. </param>
/// <param name="DefaultValue"> Gets the value that will be applied to a tracked property if no existing persisted data is found. </param>
public record TrackedPropertyInfo(Func<object, object?>? Getter, Action<object, object?>? Setter, bool IsDefaultSpecified, object? DefaultValue)
{
    internal TrackedPropertyInfo(Func<object, object?>? getter, Action<object, object?>? setter)
        : this(getter, setter, false, null)
    {
    }

    internal TrackedPropertyInfo(Func<object, object?>? getter, Action<object, object?>? setter, object? defaultValue)
        : this(getter, setter, true, defaultValue)
    {
    }
}
