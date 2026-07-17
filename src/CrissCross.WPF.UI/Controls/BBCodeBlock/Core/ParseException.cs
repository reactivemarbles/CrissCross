// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.BBCode;
#else
namespace CrissCross.WPF.UI.Controls.BBCode;
#endif

/// <summary>Represents an error encountered while parsing BBCode.</summary>
public class ParseException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="ParseException"/> class.</summary>
    public ParseException() { }

    /// <summary>Initializes a new instance of the <see cref="ParseException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public ParseException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="ParseException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying exception.</param>
    public ParseException(string message, Exception innerException)
        : base(message, innerException) { }
}
