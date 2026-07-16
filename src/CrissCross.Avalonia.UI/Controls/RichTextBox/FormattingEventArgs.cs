// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Event arguments for formatting events.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FormattingEventArgs"/> class.
/// </remarks>
/// <param name="routedEvent">The routed event.</param>
/// <param name="source">The source.</param>
/// <param name="formatType">The type of formatting applied.</param>
/// <param name="affectedText">The text that was formatted.</param>
public class FormattingEventArgs(RoutedEvent routedEvent, object source, TextFormatType formatType, string affectedText)
    : RoutedEventArgs(routedEvent, source)
{
    /// <summary>Gets the type of formatting that was applied.</summary>
    public TextFormatType FormatType { get; } = formatType;

    /// <summary>Gets the text that was formatted.</summary>
    public string AffectedText { get; } = affectedText;
}
