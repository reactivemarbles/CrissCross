// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.BBCode.Annotations;

/// <summary>Describes how a symbol is used implicitly.</summary>
[Flags]
internal enum ImplicitUseKindFlags
{
    /// <summary>No implicit use.</summary>
    None = 0,

    /// <summary>The symbol is accessed.</summary>
    Access = 1,

    /// <summary>The symbol is assigned.</summary>
    Assign = 2,

    /// <summary>The symbol is instantiated.</summary>
    InstantiatedWithFixedConstructorSignature = 4,

    /// <summary>The symbol is instantiated with any constructor signature.</summary>
    InstantiatedNoFixedConstructorSignature = 8,
}
