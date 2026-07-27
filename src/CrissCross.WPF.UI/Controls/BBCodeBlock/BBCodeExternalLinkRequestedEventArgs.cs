// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the external URI requested by a BBCode hyperlink.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class BBCodeExternalLinkRequestedEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="BBCodeExternalLinkRequestedEventArgs"/> class.</summary>
    /// <param name="uri">The allowed external URI requested by the user.</param>
    public BBCodeExternalLinkRequestedEventArgs(Uri uri) => Uri = uri ?? throw new ArgumentNullException(nameof(uri));

    /// <summary>Gets the external URI requested by the user.</summary>
    public Uri Uri { get; }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
