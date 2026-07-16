// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.BBCode.Annotations;

/// <summary>Marks a symbol as used through reflection, XAML, or another implicit mechanism.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.All, Inherited = false)]
internal sealed class UsedImplicitlyAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.</summary>
    public UsedImplicitlyAttribute()
        : this(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Itself)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.</summary>
    /// <param name="useKindFlags">The implicit use kind.</param>
    /// <param name="targetFlags">The target scope.</param>
    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    /// <summary>Gets the implicit use kind.</summary>
    public ImplicitUseKindFlags UseKindFlags { get; }

    /// <summary>Gets the target scope.</summary>
    public ImplicitUseTargetFlags TargetFlags { get; }
}
