// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// Set of properties used when creating a new simple content dialog.
/// </summary>
public class SimpleContentDialogCreateOptions
{
    /// <summary>
    /// Gets or sets a name at the top of the content dialog.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets a message displayed in the content dialog.
    /// </summary>
    public required object Content { get; set; }

    /// <summary>
    /// Gets or sets the name of the button that closes the content dialog.
    /// </summary>
    public required string CloseButtonText { get; set; }

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
