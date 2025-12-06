// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls.Presenters;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI;

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
///         Title = "CrissCross Dialog?",
///         Content = "Async Dialog!",
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
    public ContentPresenter GetContentPresenter() => _contentPresenter switch
    {
        null => throw new ArgumentNullException("The ContentPresenter didn't set previously."),
        _ => _contentPresenter
    };

    /// <inheritdoc/>
    public Task<ContentDialogResult> ShowAsync(ContentDialog dialog, CancellationToken cancellationToken)
    {
        if (_contentPresenter is null)
        {
            throw new ArgumentNullException("The ContentPresenter didn't set previously.");
        }

        ArgumentNullException.ThrowIfNull(dialog);

        dialog.ContentPresenter ??= _contentPresenter;

        if (dialog.ContentPresenter != _contentPresenter)
        {
            throw new InvalidOperationException("The ContentPresenter is not the same as the previously set.");
        }

        return dialog.ShowAsync(cancellationToken);
    }
}
