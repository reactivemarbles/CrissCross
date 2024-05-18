// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace CrissCross.WPF.UI;

#pragma warning disable RCS1175 // Unused 'this' parameter
/// <summary>
/// App Bar Mixins.
/// </summary>
public static class AppBarMixins
{
    private static readonly SingleAssign<Action<bool>> _setAppBarIsStickyFunc = new();
    private static readonly SingleAssign<Func<bool>> _getAppBarIsStickyFunc = new();
    private static readonly SingleAssign<Action<bool>> _showAppBarFunc = new();
    private static readonly SingleAssign<Action> _hideAppBarFunc = new();
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getAppBarLeftFunc = new();
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getAppBarRightFunc = new();
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getNavBarLeftFunc = new();
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getNavBarFunc = new();
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getMainMenuFunc = new();

    /// <summary>
    /// Applications the bar is sticky listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="getValue">The get value.</param>
    /// <param name="setValue">The set value.</param>
    public static void AppBarIsStickyListener(this IHaveAppBar dummy, Func<bool> getValue, Action<bool> setValue)
    {
        _setAppBarIsStickyFunc.Assign(setValue);
        _getAppBarIsStickyFunc.Assign(getValue);
    }

    /// <summary>
    /// Navs the bar right listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="getValue">The get value.</param>
    public static void NavBarListener(this IHaveAppBar dummy, Func<ObservableCollection<FrameworkElement>> getValue) =>
        _getNavBarFunc.Assign(getValue);

    /// <summary>
    /// Menus the link groups listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="getValue">The get value.</param>
    public static void MainMenuListener(this IHaveAppBar dummy, Func<ObservableCollection<FrameworkElement>> getValue) =>
        _getMainMenuFunc.Assign(getValue);

    /// <summary>
    /// Navs the bar left listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="getValue">The get value.</param>
    public static void NavBarLeftListener(this IHaveAppBar dummy, Func<ObservableCollection<FrameworkElement>> getValue) =>
        _getNavBarLeftFunc.Assign(getValue);

    /// <summary>
    /// Applications the bar left listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="getValue">The get value.</param>
    public static void AppBarLeftListener(this IHaveAppBar dummy, Func<ObservableCollection<FrameworkElement>> getValue) =>
        _getAppBarLeftFunc.Assign(getValue);

    /// <summary>
    /// Applications the bar right listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="getValue">The get value.</param>
    public static void AppBarRightListener(this IHaveAppBar dummy, Func<ObservableCollection<FrameworkElement>> getValue) => _getAppBarRightFunc.Assign(getValue);

    /// <summary>
    /// Gets the Applications Left AppBar container.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <returns>
    /// ObservableCollection of FrameworkElement.
    /// </returns>
    public static ObservableCollection<FrameworkElement>? AppBarLeft(this IControlAppBar dummy) =>
        _getAppBarLeftFunc.Value?.Invoke();

    /// <summary>
    /// Menus the link groups.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <returns>
    /// A Value.
    /// </returns>
    public static ObservableCollection<FrameworkElement>? MainMenu(this IControlAppBar dummy) =>
        _getMainMenuFunc.Value?.Invoke();

    /// <summary>
    /// Navs the bar left.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <returns>
    /// A Value.
    /// </returns>
    public static ObservableCollection<FrameworkElement>? NavBarLeft(this IControlAppBar dummy) =>
        _getNavBarLeftFunc.Value?.Invoke();

    /// <summary>
    /// Gets the Applications Right AppBar container.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <returns>
    /// ObservableCollection of FrameworkElement.
    /// </returns>
    public static ObservableCollection<FrameworkElement>? AppBarRight(this IControlAppBar dummy) =>
        _getAppBarRightFunc.Value?.Invoke();

    /// <summary>
    /// Navs the bar right.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <returns>
    /// A Value.
    /// </returns>
    public static ObservableCollection<FrameworkElement>? NavBar(this IControlAppBar dummy) =>
        _getNavBarFunc.Value?.Invoke();

    /// <summary>
    /// Shows the application bar listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="e">The e.</param>
    public static void ShowAppBarListener(this IHaveAppBar dummy, Action<bool> e) =>
        _showAppBarFunc.Assign(e);

    /// <summary>
    /// Hides the application bar listener.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="e">The e.</param>
    public static void HideAppBarListener(this IHaveAppBar dummy, Action e) =>
        _hideAppBarFunc.Assign(e);

    /// <summary>
    /// Shows the application bar.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="isSticky">if set to <c>true</c> [is sticky].</param>
    public static void ShowAppBar(this IControlAppBar dummy, bool isSticky = false) => _showAppBarFunc.Value?.Invoke(isSticky);

    /// <summary>
    /// Hides the application bar.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    public static void HideAppBar(this IControlAppBar dummy) =>
        _hideAppBarFunc.Value?.Invoke();

    /// <summary>
    /// Sets the AppBar is sticky.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void AppBarIsSticky(this IControlAppBar dummy, bool value) =>
        _setAppBarIsStickyFunc.Value?.Invoke(value);

    /// <summary>
    /// Gets the AppBar is sticky.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <returns>
    /// bool.
    /// </returns>
    public static bool? AppBarIsSticky(this IControlAppBar dummy) =>
        _getAppBarIsStickyFunc.Value?.Invoke();
}
#pragma warning restore RCS1175 // Unused 'this' parameter

