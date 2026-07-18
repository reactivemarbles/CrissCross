// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents a contract with a service that provides tools for manipulating the theme.</summary>
public interface IThemeService
{
    /// <summary>Gets current application theme.</summary>
    /// <returns>Currently set application theme.</returns>
    ApplicationTheme GetTheme();

    /// <summary>Gets current system theme.</summary>
    /// <returns>Currently set Windows theme.</returns>
    ApplicationTheme GetSystemTheme();

    /// <summary>Gets current system theme.</summary>
    /// <returns>Currently set Windows theme using system enumeration.</returns>
    SystemTheme GetNativeSystemTheme();

    /// <summary>Sets current application theme.</summary>
    /// <param name="applicationTheme">The applicationTheme value.</param>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetTheme(ApplicationTheme applicationTheme);

    /// <summary>Sets currently used Windows OS accent.</summary>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool SetSystemAccent();

    /// <summary>Sets current application accent.</summary>
    /// <param name="accentColor">The accentColor value.</param>
    /// <returns>
    ///   <see langword="true" /> if the operation succeeds. <see langword="false" /> otherwise.
    /// </returns>
    bool SetAccent(Color accentColor);

    /// <summary>Sets current application accent.</summary>
    /// <param name="accentSolidBrush">The accentSolidBrush value.</param>
    /// <returns>
    ///   <see langword="true" /> if the operation succeeds. <see langword="false" /> otherwise.
    /// </returns>
    bool SetAccent(SolidColorBrush accentSolidBrush);
}
