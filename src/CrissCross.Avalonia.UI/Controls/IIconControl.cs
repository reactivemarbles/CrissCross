// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Control that allows you to set an icon in it with an <see cref="Icon"/>.</summary>
public interface IIconControl
{
    /// <summary>Gets or sets displayed icon.</summary>
    object? Icon { get; set; }
}
