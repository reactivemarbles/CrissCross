// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Defines values that specify the input validation behavior of a <see cref="NumberBox"/> when invalid input is entered.</summary>
public enum NumberBoxValidationMode
{
    /// <summary>Input validation is disabled.</summary>
    InvalidInputOverwritten,

    /// <summary>Invalid input is replaced by <see cref="NumberBox"/> PlaceholderText text.</summary>
    Disabled
}
