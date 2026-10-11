// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Reports a documented UI Automation HRESULT through the runtime COM callable wrapper.</summary>
public sealed class WindowsAutomationException : Exception
{
    /// <summary>The native unavailable-element HRESULT.</summary>
    private const int ElementUnavailableError = unchecked((int)0x80040201);

    /// <summary>Initializes a new instance of the <see cref="WindowsAutomationException"/> class.</summary>
    public WindowsAutomationException()
        : this("The automation element is no longer available.")
    {
    }

    /// <summary>Initializes a new instance of the <see cref="WindowsAutomationException"/> class.</summary>
    /// <param name="message">The operation failure.</param>
    public WindowsAutomationException(string? message)
        : this(message, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="WindowsAutomationException"/> class.</summary>
    /// <param name="message">The operation failure.</param>
    /// <param name="innerException">The managed failure.</param>
    public WindowsAutomationException(string? message, Exception? innerException)
        : base(message, innerException) => HResult = ElementUnavailableError;

    /// <summary>Initializes a new instance of the <see cref="WindowsAutomationException"/> class.</summary>
    /// <param name="message">The native operation failure.</param>
    /// <param name="errorCode">The native HRESULT.</param>
    /// <param name="innerException">The managed failure, if any.</param>
    internal WindowsAutomationException(string message, int errorCode, Exception? innerException = null)
        : base(message, innerException) => HResult = errorCode;
}
