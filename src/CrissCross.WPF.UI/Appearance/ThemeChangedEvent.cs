// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Appearance;

/// <summary>
/// Event triggered when application theme is updated.
/// </summary>
/// <param name="currentApplicationTheme">Current application <see cref="ApplicationTheme"/>.</param>
/// <param name="systemAccent">Current base system accent <see cref="Color"/>.</param>
public delegate void ThemeChangedEvent(ApplicationTheme currentApplicationTheme, Color systemAccent);
