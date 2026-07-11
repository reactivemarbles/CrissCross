// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI;

/// <summary>Represents a contract with the service that creates <see cref="ContentDialog"/>.</summary>
/// <example>
/// <code lang="xml">
/// &lt;ContentPresenter x:Name="RootContentDialogPresenter" Grid.Row="0" /&gt;
/// </code>
/// <code lang="csharp">
/// IContentDialogService contentDialogService = new ContentDialogService();
/// contentDialogService.SetContentPresenter(RootContentDialogPresenter);
/// <summary>Represents the IContentDialogService member.</summary>
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
public interface IContentDialogService
{
    /// <summary>Sets the <see cref="ContentPresenter"/>.</summary>
    /// <param name="contentPresenter">The contentPresenter value.</param>
    void SetContentPresenter(ContentPresenter contentPresenter);

    /// <summary>Provides direct access to the <see cref="ContentPresenter"/>.</summary>
    /// <returns>Reference to the currently selected <see cref="ContentPresenter"/> which displays the <see cref="ContentDialog"/>'s.</returns>
    ContentPresenter GetContentPresenter();

    /// <summary>Asynchronously shows the specified dialog.</summary>
    /// <param name="dialog">The dialog value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the dialog result.</returns>
    Task<ContentDialogResult> ShowAsync(ContentDialog dialog, CancellationToken cancellationToken);
}
