// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Control changing its properties or appearance depending on the theme.
/// </summary>
public interface IThemeControl
{
    /// <summary>
    /// Gets the theme is currently set.
    /// </summary>
    public ApplicationTheme ApplicationTheme { get; }
}
