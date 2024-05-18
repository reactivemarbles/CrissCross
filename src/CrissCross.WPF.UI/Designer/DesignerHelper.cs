// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
