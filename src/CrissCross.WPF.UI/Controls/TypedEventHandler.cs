// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on Windows UI Library
//// Copyright(c) Microsoft Corporation.All rights reserved.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a method that handles general events.
/// </summary>
/// <typeparam name="TSender">The type of the sender.</typeparam>
/// <typeparam name="TArgs">The type of the arguments.</typeparam>
/// <param name="sender">The sender.</param>
/// <param name="args">The arguments.</param>
public delegate void TypedEventHandler<in TSender, in TArgs>(TSender sender, TArgs args)
    where TSender : DependencyObject
    where TArgs : RoutedEventArgs;
