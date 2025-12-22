// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace CrissCross.Avalonia.UI.Controls;

internal static class HtmlClipboardUtilities
{
    public static string ExtractFragment(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var start = GetFragmentIndex(html, "StartFragment:");
        var end = GetFragmentIndex(html, "EndFragment:");
        if (start >= 0 && end > start && end <= html.Length)
        {
            return html[start..end];
        }

        const string startMarker = "<!--StartFragment-->";
        const string endMarker = "<!--EndFragment-->";
        var startMarkerIndex = html.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);
        if (startMarkerIndex >= 0)
        {
            startMarkerIndex += startMarker.Length;
            var endMarkerIndex = html.IndexOf(endMarker, startMarkerIndex, StringComparison.OrdinalIgnoreCase);
            if (endMarkerIndex > startMarkerIndex)
            {
                return html[startMarkerIndex..endMarkerIndex];
            }
        }

        return html;
    }

    public static string EncodePlainText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var encoded = WebUtility.HtmlEncode(text);
        return encoded.Replace("\r\n", "<br />", StringComparison.Ordinal)
                      .Replace("\n", "<br />", StringComparison.Ordinal)
                      .Replace("\r", "<br />", StringComparison.Ordinal);
    }

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

        if (indexEnd > indexStart &&
            int.TryParse(html[indexStart..indexEnd], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        return -1;
    }
}
