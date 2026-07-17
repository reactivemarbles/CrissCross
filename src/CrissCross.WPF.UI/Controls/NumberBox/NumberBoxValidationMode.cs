// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the NumberBoxValidationMode member.</summary>
public enum NumberBoxValidationMode
{
    /// <summary>Input validation is disabled.</summary>
    InvalidInputOverwritten,

    /// <summary>Invalid input is replaced by <see cref="NumberBox"/> PlaceholderText text.</summary>
    Disabled,
}
