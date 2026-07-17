// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Describes the content and buttons of a custom message box.</summary>
public sealed class CustomMessageBoxRequest
{
    /// <summary>Gets or sets the BBCode message content.</summary>
    public string BBCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the message title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the custom button labels.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = Array.Empty<string>();
}
