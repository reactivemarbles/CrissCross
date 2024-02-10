// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright(C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Designer;

/// <summary>
/// Helper class for Visual Studio designer.
/// </summary>
public static class DesignerHelper
{
    private static bool _isValueAlreadyValidated;
    private static bool _isInDesignMode;

    /// <summary>
    /// Gets a value indicating whether the project is currently in design mode.
    /// </summary>
    public static bool IsInDesignMode => IsCurrentAppInDebugMode();

    /// <summary>
    /// Gets a value indicating whether the project is currently debugged.
    /// </summary>
    public static bool IsDebugging => System.Diagnostics.Debugger.IsAttached;

    private static bool IsCurrentAppInDebugMode()
    {
        if (_isValueAlreadyValidated)
        {
            return _isInDesignMode;
        }

        _isInDesignMode = (bool)(
            DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject))?.DefaultValue
            ?? false);

        _isValueAlreadyValidated = true;

        return _isInDesignMode;
    }
}
