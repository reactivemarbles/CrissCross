// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides information when an external BBCode link cannot be opened.</summary>
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
}
