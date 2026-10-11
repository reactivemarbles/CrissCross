// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Converts the supported rich document interchange formats.</summary>
internal static class RtfDocumentConverter
{
    /// <summary>Reads RTF as HTML, retaining document content and basic formatting.</summary>
    /// <param name="rtf">The RTF document.</param>
    /// <returns>The HTML document fragment.</returns>
    internal static string ToHtml(string rtf)
    {
        ArgumentNullException.ThrowIfNull(rtf);
        return new RtfContentParser(rtf).Read();
    }

    /// <summary>Writes HTML as an RTF document.</summary>
    /// <param name="html">The HTML document.</param>
    /// <returns>The RTF document.</returns>
    internal static string ToRtf(string html)
    {
        ArgumentNullException.ThrowIfNull(html);
        return new RtfContentWriter().Write(html);
    }

    /// <summary>Attempts to export RTF while allowing richer HTML clipboard content to remain available.</summary>
    /// <param name="html">The HTML document.</param>
    /// <returns>The RTF document, or null when the document uses unsupported RTF features.</returns>
    internal static string? TryToRtf(string html)
    {
        ArgumentNullException.ThrowIfNull(html);
        try
        {
            return ToRtf(html);
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }
}
