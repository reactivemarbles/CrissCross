// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// Set of properties used when creating a new simple content dialog.
/// </summary>
/// <param name="Title"> Gets or sets a name at the top of the content dialog. </param>
/// <param name="Content"> Gets or sets a message displayed in the content dialog. </param>
/// <param name="CloseButtonText"> Gets or sets the name of the button that closes the content dialog. </param>
public record SimpleContentDialogCreateOptions(string Title, object Content, string CloseButtonText)
{
    /// <summary>
    /// Gets or sets the default text of the primary button at the bottom of the content dialog.
    /// <para>If not added, or <see cref="string.Empty"/>, it will not be displayed.</para>
    /// </summary>
    public string PrimaryButtonText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default text of the secondary button at the bottom of the content dialog.
    /// <para>If not added, or <see cref="string.Empty"/>, it will not be displayed.</para>
    /// </summary>
    public string SecondaryButtonText { get; set; } = string.Empty;
}
