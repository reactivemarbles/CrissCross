// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI;

/// <summary>Represents a contract with the service that provides global <see cref="Snackbar"/>.</summary>
public interface ISnackbarService
{
    /// <summary>Gets or sets a time for which the <see cref="Snackbar"/> should be visible. (By default 2 seconds).</summary>
    TimeSpan DefaultTimeOut { get; set; }

    /// <summary>Sets the <see cref="SnackbarPresenter"/>.</summary>
    /// <param name="contentPresenter">The contentPresenter value.</param>
    void SetSnackbarPresenter(SnackbarPresenter contentPresenter);

    /// <summary>Provides direct access to the <see cref="ContentPresenter"/>.</summary>
    /// <returns><see cref="Snackbar"/> currently in use.</returns>
    SnackbarPresenter GetSnackbarPresenter();

    /// <summary>Shows the snackbar. If it is already visible, firstly hides it for a moment, changes its content, and then shows it again.</summary>
    /// <param name="title">The title value.</param>
    /// <param name="message">The message value.</param>
    /// <param name="appearance">The appearance value.</param>
    /// <param name="icon">The icon value.</param>
    /// <param name="timeout">The timeout value.</param>
    void Show(
        string title,
        string message,
        ControlAppearance appearance,
        IconElement? icon,
        TimeSpan timeout);
}
