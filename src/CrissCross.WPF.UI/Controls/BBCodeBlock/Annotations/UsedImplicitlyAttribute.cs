// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode.Annotations;
#else
namespace CrissCross.WPF.UI.Controls.BBCode.Annotations;
#endif

/// <summary>Marks a symbol as used through reflection, XAML, or another implicit mechanism.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.All, Inherited = false)]
internal sealed class UsedImplicitlyAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.</summary>
    internal UsedImplicitlyAttribute()
        : this(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Itself) { }

    /// <summary>Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.</summary>
    /// <param name="useKindFlags">The implicit use kind.</param>
    /// <param name="targetFlags">The target scope.</param>
    internal UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    /// <summary>Gets the implicit use kind.</summary>
    internal ImplicitUseKindFlags UseKindFlags { get; }

    /// <summary>Gets the target scope.</summary>
    internal ImplicitUseTargetFlags TargetFlags { get; }
}
