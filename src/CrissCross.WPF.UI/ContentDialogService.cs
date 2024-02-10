// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Windows.Controls;

namespace CrissCross.WPF.UI;

/// <summary>
/// Represents a contract with the service that creates <see cref="ContentDialog"/>.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ContentPresenter x:Name="RootContentDialogPresenter" Grid.Row="0" /&gt;
/// </code>
/// <code lang="csharp">
/// IContentDialogService contentDialogService = new ContentDialogService();
/// contentDialogService.SetContentPresenter(RootContentDialogPresenter);
///
/// await _contentDialogService.ShowAsync(
///     new ContentDialog(){
///         Title = "The cake?",
///         Content = "IS A LIE!",
///         PrimaryButtonText = "Save",
///         SecondaryButtonText = "Don't Save",
///         CloseButtonText = "Cancel"
///     }
/// );
/// </code>
/// </example>
public class ContentDialogService : IContentDialogService
{
    private ContentPresenter? _contentPresenter;

    /// <inheritdoc/>
    public void SetContentPresenter(ContentPresenter contentPresenter) => _contentPresenter = contentPresenter;

    /// <inheritdoc/>
    public ContentPresenter GetContentPresenter()
    {
        if (_contentPresenter is null)
        {
            throw new ArgumentNullException("The ContentPresenter didn't set previously.");
        }

        return _contentPresenter;
    }

    /// <inheritdoc/>
    public Task<ContentDialogResult> ShowAsync(ContentDialog dialog, CancellationToken cancellationToken)
    {
        if (_contentPresenter is null)
        {
            throw new ArgumentNullException("The ContentPresenter didn't set previously.");
        }

        if (dialog is null)
        {
            throw new ArgumentNullException(nameof(dialog));
        }

        dialog.ContentPresenter ??= _contentPresenter;

        if (dialog.ContentPresenter != _contentPresenter)
        {
            throw new InvalidOperationException("The ContentPresenter is not the same as the previously set.");
        }

        return dialog.ShowAsync(cancellationToken);
    }
}
