// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode.Annotations;
#else
namespace CrissCross.WPF.UI.Controls.BBCode.Annotations;
#endif

/// <summary>Describes the target of an implicit use annotation.</summary>
[Flags]
internal enum ImplicitUseTargetFlags
{
    /// <summary>No implicit target.</summary>
    None = 0,

    /// <summary>The annotated symbol itself.</summary>
    Itself = 1,

    /// <summary>Members of the annotated symbol.</summary>
    Members = 2,

    /// <summary>Symbols with the annotated symbol as an ancestor.</summary>
    WithInheritors = 4,

    /// <summary>The annotated symbol and its members.</summary>
    WithMembers = Itself | Members,
}
