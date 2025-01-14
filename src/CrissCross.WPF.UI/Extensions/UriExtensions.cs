// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Extensions for <see cref="Uri"/> class.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Removes last segment of the <see cref="Uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>A Uri.</returns>
    public static Uri TrimLastSegment(this Uri uri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (uri.Segments.Length < 2)
        {
            return uri;
        }

#if NET5_0_OR_GREATER
        var uriLastSegmentLength = uri.Segments[^1].Length;
#else
        var uriLastSegmentLength = uri.Segments[uri.Segments.Length - 1].Length;
#endif
        var uriOriginalString = uri.ToString();

        return new Uri(
            uriOriginalString!.Substring(0, uriOriginalString.Length - uriLastSegmentLength),
            UriKind.RelativeOrAbsolute);
    }

    /// <summary>
    /// Determines whether the end of <see cref="Uri" /> is equal to provided value.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="value">The value.</param>
    /// <returns>A bool.</returns>
    public static bool EndsWith(this Uri uri, string value)
    {
        if (uri == null)
        {
            return false;
        }

        return uri.ToString().EndsWith(value);
    }

    /// <summary>
    /// Append provided segments to the <see cref="Uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="segments">The segments.</param>
    /// <returns>A Uri.</returns>
    public static Uri Append(this Uri uri, params string[] segments)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        return new Uri(
            segments.Aggregate(
                uri.AbsoluteUri,
                (current, path) =>
                    string.Format(
                        "{0}/{1}",
                        current.TrimEnd('/').TrimEnd('\\'),
                        path.TrimStart('/').TrimStart('\\'))));
    }

    /// <summary>
    /// Append new <see cref="Uri" /> to the <see cref="Uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="value">The value.</param>
    /// <returns>A Uri.</returns>
    public static Uri Append(this Uri uri, Uri value)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new Uri(
            string.Format(
                "{0}/{1}",
                uri.ToString().TrimEnd('/').TrimEnd('\\'),
                value.ToString().TrimStart('/').TrimStart('\\')),
            UriKind.RelativeOrAbsolute);
    }
}
