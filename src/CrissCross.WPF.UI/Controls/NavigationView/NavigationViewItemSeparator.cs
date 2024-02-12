// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on Windows UI Library
//// Copyright(c) Microsoft Corporation.All rights reserved.

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

// https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.navigationviewitemseparator?view=winrt-22621

/// <summary>
/// Represents a line that separates menu items in a NavigationMenu.
/// </summary>
[ToolboxItem(true)]
[System.Drawing.ToolboxBitmap(typeof(NavigationViewItemSeparator), "NavigationViewItemSeparator.bmp")]
public class NavigationViewItemSeparator : System.Windows.Controls.Separator;
