// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Describes activation of a document hyperlink.</summary>
/// <param name="navigationUri">The preserved HTML navigation address.</param>
/// <param name="target">The preserved HTML target.</param>
public sealed class RichTextHyperlinkEventArgs(string navigationUri, string? target) : EventArgs
{
    /// <summary>Gets the preserved navigation address.</summary>
    public string NavigationUri { get; } = navigationUri;

    /// <summary>Gets the preserved HTML target.</summary>
    public string? Target { get; } = target;
}
