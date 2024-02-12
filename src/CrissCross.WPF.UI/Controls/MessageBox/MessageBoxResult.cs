// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Specifies identifiers to indicate the return value of a <see cref="MessageBox"/>.
/// </summary>
public enum MessageBoxResult
{
    /// <summary>
    /// No button was tapped.
    /// </summary>
    None,

    /// <summary>
    /// The primary button was tapped by the user.
    /// </summary>
    Primary,

    /// <summary>
    /// The secondary button was tapped by the user.
    /// </summary>
    Secondary
}
