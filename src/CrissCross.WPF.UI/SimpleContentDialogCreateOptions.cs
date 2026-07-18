// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Set of properties used when creating a new simple content dialog.</summary>
/// <param name="Title"> Gets or sets a name at the top of the content dialog. </param>
/// <param name="Content"> Gets or sets a message displayed in the content dialog. </param>
/// <param name="CloseButtonText"> Gets or sets the name of the button that closes the content dialog. </param>
public sealed record SimpleContentDialogCreateOptions(string Title, object Content, string CloseButtonText)
{
    /// <summary>
    /// Gets or sets the default text of the primary button at the bottom of the content dialog.
    /// <para>If not added, or <see cref="string.Empty"/>, it will not be displayed.</para>
    /// </summary>
    public string PrimaryButtonText { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the default text of the secondary button at the bottom of the content dialog.
    /// <para>If not added, or <see cref="string.Empty"/>, it will not be displayed.</para>
    /// </summary>
    public string SecondaryButtonText { get; init; } = string.Empty;
}
