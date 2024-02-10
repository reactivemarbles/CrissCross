// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Content Dialog Service Extensions.
/// </summary>
public static class ContentDialogServiceExtensions
{
    /// <summary>
    /// Shows the simple alert-like dialog.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="closeButtonText">The close button text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// Result of the life cycle of the <see cref="ContentDialog" />.
    /// </returns>
    public static Task<ContentDialogResult> ShowAlertAsync(
        this IContentDialogService dialogService,
        string title,
        string message,
        string closeButtonText,
        CancellationToken cancellationToken = default)
    {
        if (dialogService is null)
        {
            throw new ArgumentNullException(nameof(dialogService));
        }

        var dialog = new ContentDialog();

        dialog.SetCurrentValue(ContentDialog.TitleProperty, title);
        dialog.SetCurrentValue(ContentControl.ContentProperty, message);
        dialog.SetCurrentValue(ContentDialog.CloseButtonTextProperty, closeButtonText);

        return dialogService.ShowAsync(dialog, cancellationToken);
    }

    /// <summary>
    /// Shows simple dialog.
    /// </summary>
    /// <param name="dialogService">The <see cref="IContentDialogService"/>.</param>
    /// <param name="options">Set of parameters of the basic dialog box.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Result of the life cycle of the <see cref="ContentDialog"/>.</returns>
    public static Task<ContentDialogResult> ShowSimpleDialogAsync(
        this IContentDialogService dialogService,
        SimpleContentDialogCreateOptions options,
        CancellationToken cancellationToken = default)
    {
        if (dialogService is null)
        {
            throw new ArgumentNullException(nameof(dialogService));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var dialog = new ContentDialog()
        {
            Title = options.Title,
            Content = options.Content,
            CloseButtonText = options.CloseButtonText,
            PrimaryButtonText = options.PrimaryButtonText,
            SecondaryButtonText = options.SecondaryButtonText
        };

        return dialogService.ShowAsync(dialog, cancellationToken);
    }
}
