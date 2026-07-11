// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Control that allows you to set an icon in it with an <see cref="Icon"/>.</summary>
public interface IIconControl
{
    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    IconElement? Icon { get; set; }
}
