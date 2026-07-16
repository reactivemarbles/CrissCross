// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>Extensions for <see cref="IContentDialogService"/>.</summary>
public static class ContentDialogServiceExtensions
{
    /// <summary>Provides extension members for <see cref="IContentDialogService"/>.</summary>
    /// <param name="contentDialogService">The content dialog service.</param>
    extension(IContentDialogService contentDialogService)
    {
        /// <summary>Shows the content dialog asynchronously.</summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the dialog
        /// result.</returns>
        public Task<ContentDialogResult> ShowAsync(ContentDialog dialog)
        {
            ArgumentNullException.ThrowIfNull(contentDialogService);
            return contentDialogService.ShowAsync(dialog, CancellationToken.None);
        }
    }
}
