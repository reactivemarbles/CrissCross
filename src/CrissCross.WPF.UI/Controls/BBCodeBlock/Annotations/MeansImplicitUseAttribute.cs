// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode.Annotations;
#else
namespace CrissCross.WPF.UI.Controls.BBCode.Annotations;
#endif

/// <summary>Marks another annotation as implying use of its target.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter)]
internal sealed class MeansImplicitUseAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.</summary>
    internal MeansImplicitUseAttribute()
        : this(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Itself) { }

    /// <summary>Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.</summary>
    /// <param name="useKindFlags">The implicit use kind.</param>
    /// <param name="targetFlags">The target scope.</param>
    internal MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    /// <summary>Gets the implicit use kind.</summary>
    internal ImplicitUseKindFlags UseKindFlags { get; }

    /// <summary>Gets the target scope.</summary>
    internal ImplicitUseTargetFlags TargetFlags { get; }
}
