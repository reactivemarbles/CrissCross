// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls.Presenters;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.UI.Controls;
#else
using CrissCross.Avalonia.UI.Controls;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI;
#else
namespace CrissCross.Avalonia.UI;
#endif

/// <summary>Represents a contract with the service that creates <see cref="ContentDialog"/>.</summary>
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
    /// <summary>Provides the _contentPresenter member.</summary>
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

        if (!object.Equals(dialog.ContentPresenter, _contentPresenter))
        {
            throw new InvalidOperationException("The ContentPresenter is not the same as the previously set.");
        }

        return dialog.ShowAsync(cancellationToken);
    }
}
