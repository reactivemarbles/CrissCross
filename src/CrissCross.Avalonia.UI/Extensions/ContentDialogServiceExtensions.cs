// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>
/// Extensions for <see cref="IContentDialogService"/>.
/// </summary>
public static class ContentDialogServiceExtensions
{
    /// <summary>
    /// Shows the content dialog asynchronously.
    /// </summary>
    /// <param name="contentDialogService">The content dialog service.</param>
    /// <param name="dialog">The dialog to show.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the dialog result.</returns>
    public static Task<ContentDialogResult> ShowAsync(
        this IContentDialogService contentDialogService,
        ContentDialog dialog)
    {
        ArgumentNullException.ThrowIfNull(contentDialogService);
        return contentDialogService.ShowAsync(dialog, CancellationToken.None);
    }
}
