// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace CrissCross.WPF.UI;

/// <summary>App Bar Mixins.</summary>
public static class AppBarMixins
{
    /// <summary>Stores the _setAppBarIsStickyFunc value.</summary>
    private static readonly SingleAssign<Action<bool>> _setAppBarIsStickyFunc = new();

    /// <summary>Stores the _getAppBarIsStickyFunc value.</summary>
    private static readonly SingleAssign<Func<bool>> _getAppBarIsStickyFunc = new();

    /// <summary>Stores the _showAppBarFunc value.</summary>
    private static readonly SingleAssign<Action<bool>> _showAppBarFunc = new();

    /// <summary>Stores the _hideAppBarFunc value.</summary>
    private static readonly SingleAssign<Action> _hideAppBarFunc = new();

    /// <summary>Stores the _getAppBarLeftFunc value.</summary>
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getAppBarLeftFunc = new();

    /// <summary>Stores the _getAppBarRightFunc value.</summary>
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getAppBarRightFunc = new();

    /// <summary>Stores the _getNavBarLeftFunc value.</summary>
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getNavBarLeftFunc = new();

    /// <summary>Stores the _getNavBarFunc value.</summary>
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getNavBarFunc = new();

    /// <summary>Stores the _getMainMenuFunc value.</summary>
    private static readonly SingleAssign<Func<ObservableCollection<FrameworkElement>>> _getMainMenuFunc = new();

    /// <summary>Provides extension members.</summary>
    /// <param name="dummy">The dummy value.</param>
    extension(IControlAppBar dummy)
    {
        /// <summary>Gets the Applications Left AppBar container.</summary>
        /// <returns>
        /// ObservableCollection of FrameworkElement.
        /// </returns>
        public ObservableCollection<FrameworkElement>? AppBarLeft()
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            return _getAppBarLeftFunc.Value?.Invoke();
        }

        /// <summary>Menus the link groups.</summary>
        /// <returns>
        /// A Value.
        /// </returns>
        public ObservableCollection<FrameworkElement>? MainMenu()
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            return _getMainMenuFunc.Value?.Invoke();
        }

        /// <summary>Navs the bar left.</summary>
        /// <returns>
        /// A Value.
        /// </returns>
        public ObservableCollection<FrameworkElement>? NavBarLeft()
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            return _getNavBarLeftFunc.Value?.Invoke();
        }

        /// <summary>Gets the Applications Right AppBar container.</summary>
        /// <returns>
        /// ObservableCollection of FrameworkElement.
        /// </returns>
        public ObservableCollection<FrameworkElement>? AppBarRight()
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            return _getAppBarRightFunc.Value?.Invoke();
        }

        /// <summary>Navs the bar right.</summary>
        /// <returns>
        /// A Value.
        /// </returns>
        public ObservableCollection<FrameworkElement>? NavBar()
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            return _getNavBarFunc.Value?.Invoke();
        }

        /// <summary>Shows the application bar.</summary>
        public void ShowAppBar() => dummy.ShowAppBar(false);

        /// <summary>Shows the application bar.</summary>
        /// <param name="isSticky">if set to <c>true</c> [is sticky].</param>
        public void ShowAppBar(bool isSticky)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _showAppBarFunc.Value?.Invoke(isSticky);
        }

        /// <summary>Hides the application bar.</summary>
        public void HideAppBar()
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _hideAppBarFunc.Value?.Invoke();
        }

        /// <summary>Sets the AppBar is sticky.</summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void AppBarIsSticky(bool value)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _setAppBarIsStickyFunc.Value?.Invoke(value);
        }

        /// <summary>Gets the AppBar is sticky.</summary>
        /// <returns>
        /// bool.
        /// </returns>
        public bool? AppBarIsSticky()
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            return _getAppBarIsStickyFunc.Value?.Invoke();
        }
    }

    /// <summary>Provides extension members.</summary>
    /// <param name="dummy">The dummy value.</param>
    extension(IHaveAppBar dummy)
    {
        /// <summary>Applications the bar is sticky listener.</summary>
        /// <param name="getValue">The get value.</param>
        /// <param name="setValue">The set value.</param>
        public void AppBarIsStickyListener(Func<bool> getValue, Action<bool> setValue)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _setAppBarIsStickyFunc.Assign(setValue);
            _getAppBarIsStickyFunc.Assign(getValue);
        }

        /// <summary>Navs the bar right listener.</summary>
        /// <param name="getValue">The get value.</param>
        public void NavBarListener(Func<ObservableCollection<FrameworkElement>> getValue)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _getNavBarFunc.Assign(getValue);
        }

        /// <summary>Menus the link groups listener.</summary>
        /// <param name="getValue">The get value.</param>
        public void MainMenuListener(Func<ObservableCollection<FrameworkElement>> getValue)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _getMainMenuFunc.Assign(getValue);
        }

        /// <summary>Navs the bar left listener.</summary>
        /// <param name="getValue">The get value.</param>
        public void NavBarLeftListener(Func<ObservableCollection<FrameworkElement>> getValue)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _getNavBarLeftFunc.Assign(getValue);
        }

        /// <summary>Applications the bar left listener.</summary>
        /// <param name="getValue">The get value.</param>
        public void AppBarLeftListener(Func<ObservableCollection<FrameworkElement>> getValue)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _getAppBarLeftFunc.Assign(getValue);
        }

        /// <summary>Applications the bar right listener.</summary>
        /// <param name="getValue">The get value.</param>
        public void AppBarRightListener(Func<ObservableCollection<FrameworkElement>> getValue)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _getAppBarRightFunc.Assign(getValue);
        }

        /// <summary>Shows the application bar listener.</summary>
        /// <param name="e">The e.</param>
        public void ShowAppBarListener(Action<bool> e)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _showAppBarFunc.Assign(e);
        }

        /// <summary>Hides the application bar listener.</summary>
        /// <param name="e">The e.</param>
        public void HideAppBarListener(Action e)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            _hideAppBarFunc.Assign(e);
        }
    }
}
