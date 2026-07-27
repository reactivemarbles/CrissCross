// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides information about a selected external BBCode link.</summary>
public sealed class BBCodeLinkRequestedEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="BBCodeLinkRequestedEventArgs"/> class.</summary>
    /// <param name="uri">The selected URI.</param>
    public BBCodeLinkRequestedEventArgs(Uri uri) => Uri = uri ?? throw new ArgumentNullException(nameof(uri));

    /// <summary>Gets the selected URI.</summary>
    public Uri Uri { get; }
}
