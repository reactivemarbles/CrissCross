// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides information when an external BBCode link cannot be opened.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class BBCodeNavigationFailedEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="BBCodeNavigationFailedEventArgs"/> class.</summary>
    /// <param name="uri">The link that could not be opened.</param>
    /// <param name="exception">The navigation failure.</param>
    public BBCodeNavigationFailedEventArgs(Uri uri, Exception exception)
    {
        Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    /// <summary>Gets the navigation failure.</summary>
    public Exception Exception { get; }

    /// <summary>Gets the link that could not be opened.</summary>
    public Uri Uri { get; }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
