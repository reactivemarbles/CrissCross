// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>Extensions for <see cref="Uri"/> class.</summary>
public static class UriExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="uri">The uri value.</param>
    extension(Uri uri)
    {
        /// <summary>Removes last segment of the <see cref="Uri" />.</summary>
        /// <returns>A Uri.</returns>
        public Uri TrimLastSegment()
        {
            if (uri is null)
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

        /// <summary>Determines whether the end of <see cref="Uri" /> is equal to provided value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A bool.</returns>
        public bool EndsWith(string value)
        {
            return uri?.ToString().EndsWith(value) == true;
        }

        /// <summary>Append provided segments to the <see cref="Uri" />.</summary>
        /// <param name="segments">The segments.</param>
        /// <returns>A Uri.</returns>
        public Uri Append(params string[] segments)
        {
            if (uri is null)
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

        /// <summary>Append new <see cref="Uri" /> to the <see cref="Uri" />.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A Uri.</returns>
        public Uri Append(Uri value)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (value is null)
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
}
