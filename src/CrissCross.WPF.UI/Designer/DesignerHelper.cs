// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Designer;

/// <summary>Helper class for Visual Studio designer.</summary>
public static class DesignerHelper
{
    /// <summary>Stores the _isValueAlreadyValidated value.</summary>
    private static bool _isValueAlreadyValidated;

    /// <summary>Stores the _isInDesignMode value.</summary>
    private static bool _isInDesignMode;

    /// <summary>Gets a value indicating whether the project is currently in design mode.</summary>
    public static bool IsInDesignMode => IsCurrentAppInDebugMode();

    /// <summary>Gets a value indicating whether the project is currently debugged.</summary>
    public static bool IsDebugging => Debugger.IsAttached;

    /// <summary>Provides the IsCurrentAppInDebugMode member.</summary>
    /// <returns>The result.</returns>
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
