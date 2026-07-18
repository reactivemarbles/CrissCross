// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents a contract with the service that creates <see cref="ContentDialog"/>.</summary>
/// <example>
/// <code lang="xml">
/// &lt;ContentPresenter x:Name="RootContentDialogPresenter" Grid.Row="0" /&gt;
/// </code>
/// <code lang="csharp">
/// IContentDialogService contentDialogService = new ContentDialogService();
/// contentDialogService.SetContentPresenter(RootContentDialogPresenter);
/// <summary>Represents the ContentDialogService member.</summary>
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
    /// <summary>Stores the _contentPresenter value.</summary>
    private ContentPresenter? _contentPresenter;

    /// <inheritdoc/>
    public void SetContentPresenter(ContentPresenter contentPresenter) => _contentPresenter = contentPresenter;

    /// <inheritdoc/>
    public ContentPresenter GetContentPresenter() =>
        _contentPresenter switch
        {
            null => throw new ArgumentNullException("The ContentPresenter didn't set previously."),
            _ => _contentPresenter,
        };

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

        if (!ReferenceEquals(dialog.ContentPresenter, _contentPresenter))
        {
            throw new InvalidOperationException("The ContentPresenter is not the same as the previously set.");
        }

        return dialog.ShowAsync(cancellationToken);
    }
}
