// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>Represents a contract with the service that provides global <see cref="Snackbar"/>.</summary>
public interface ISnackbarService
{
    /// <summary>Gets or sets a time for which the Snackbar should be visible. (By default 2 seconds).</summary>
    TimeSpan DefaultTimeOut { get; set; }

    /// <summary>Sets the <see cref="SnackbarPresenter"/>.</summary>
    /// <param name="contentPresenter">The snackbar presenter.</param>
    void SetSnackbarPresenter(SnackbarPresenter contentPresenter);

    /// <summary>Provides direct access to the <see cref="SnackbarPresenter"/>.</summary>
    /// <returns><see cref="SnackbarPresenter"/> currently in use.</returns>
    SnackbarPresenter GetSnackbarPresenter();

    /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content, and
    /// then shows it again.</summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="appearance">The appearance.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="timeout">The timeout.</param>
    void Show(string title, string message, ControlAppearance appearance, IconElement? icon, TimeSpan timeout);
}
