// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Provides the HtmlClipboardUtilities member.</summary>
internal static class HtmlClipboardUtilities
{
    /// <summary>HTML line-break element used for plain-text line endings.</summary>
    private const string HtmlLineBreak = "<br />";

    /// <summary>Provides the ExtractFragment member.</summary>
    /// <param name="html">The html value.</param>
    /// <returns>The result.</returns>
    public static string ExtractFragment(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        const string startMarker = "<!--StartFragment-->";
        const string endMarker = "<!--EndFragment-->";
        var startMarkerIndex = html.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);
        if (
            startMarkerIndex >= 0
            && html.IndexOf(endMarker, startMarkerIndex + startMarker.Length, StringComparison.OrdinalIgnoreCase)
                is var endMarkerIndex
            && endMarkerIndex >= startMarkerIndex + startMarker.Length)
        {
            startMarkerIndex += startMarker.Length;
            return html[startMarkerIndex..endMarkerIndex];
        }

        var start = GetFragmentIndex(html, "StartFragment:");
        var end = GetFragmentIndex(html, "EndFragment:");
        return start >= 0 && end >= start && end <= html.Length ? html[start..end] : html;
    }

    /// <summary>Wraps an HTML fragment in the Windows HTML Clipboard Format envelope.</summary>
    /// <param name="fragment">The HTML fragment to wrap.</param>
    /// <returns>A clipboard-compatible HTML payload.</returns>
    public static string CreateClipboardHtml(string fragment)
    {
        ArgumentNullException.ThrowIfNull(fragment);

        const string headerTemplate =
            "Version:1.0\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\n";
        const string startMarker = "<!--StartFragment-->";
        const string endMarker = "<!--EndFragment-->";
        var body = $"<html><body>{startMarker}{fragment}{endMarker}</body></html>";
        var emptyHeader = string.Format(CultureInfo.InvariantCulture, headerTemplate, 0, 0, 0, 0);
        var startHtml = Encoding.UTF8.GetByteCount(emptyHeader);
        var startFragment = startHtml + Encoding.UTF8.GetByteCount("<html><body>" + startMarker);
        var endFragment = startFragment + Encoding.UTF8.GetByteCount(fragment);
        var endHtml = startHtml + Encoding.UTF8.GetByteCount(body);
        var header = string.Format(
            CultureInfo.InvariantCulture,
            headerTemplate,
            startHtml,
            endHtml,
            startFragment,
            endFragment);
        return header + body;
    }

    /// <summary>Provides the EncodePlainText member.</summary>
    /// <param name="text">The text value.</param>
    /// <returns>The result.</returns>
    public static string EncodePlainText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var encoded = WebUtility.HtmlEncode(text);
        return encoded
            .Replace("\r\n", HtmlLineBreak, StringComparison.Ordinal)
            .Replace("\n", HtmlLineBreak, StringComparison.Ordinal)
            .Replace("\r", HtmlLineBreak, StringComparison.Ordinal);
    }

    /// <summary>Provides the GetFragmentIndex member.</summary>
    /// <param name="html">The html value.</param>
    /// <param name="marker">The marker value.</param>
    /// <returns>The result.</returns>
    private static int GetFragmentIndex(string html, string marker)
    {
        var markerIndex = html.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return -1;
        }

        var indexStart = markerIndex + marker.Length;
        var indexEnd = indexStart;
        while (indexEnd < html.Length && char.IsDigit(html[indexEnd]))
        {
            indexEnd++;
        }

        return indexEnd > indexStart
            && int.TryParse(
                html[indexStart..indexEnd],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var value)
            ? value
            : -1;
    }
}
