// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AngleSharp.Html.Parser;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Embeds bounded local images in HTML clipboard content.</summary>
internal static class RichTextClipboardImages
{
    /// <summary>Maximum local image size accepted for clipboard embedding.</summary>
    internal const long MaximumImageBytes = 10 * 1024 * 1024;

    /// <summary>Read buffer size used for local image files.</summary>
    private const int ReadBufferSize = 81_920;

    /// <summary>Embeds supported local image sources as data URIs.</summary>
    /// <param name="html">The HTML clipboard fragment.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The HTML with accessible local images embedded.</returns>
    internal static async Task<string> EmbedLocalImagesAsync(string html, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(html);
        cancellationToken.ThrowIfCancellationRequested();
        if (!html.Contains("<img", StringComparison.OrdinalIgnoreCase))
        {
            return html;
        }

        var document = new HtmlParser().ParseDocument(html);
        var images = document.QuerySelectorAll("img[src]");
        if (images.Length == 0)
        {
            return html;
        }

        var changed = false;
        foreach (var image in images)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var source = image.GetAttribute("src");
            var path = GetLocalPath(source);
            var mimeType = GetMimeType(path);
            if (path is null || mimeType is null)
            {
                continue;
            }

            var dataUri = await TryReadImageAsync(path, mimeType, cancellationToken).ConfigureAwait(false);
            if (dataUri is null)
            {
                continue;
            }

            image.SetAttribute("src", dataUri);
            changed = true;
        }

        return changed ? document.DocumentElement?.OuterHtml ?? html : html;
    }

    /// <summary>Resolves a file URI or rooted local path.</summary>
    /// <param name="source">The image source attribute.</param>
    /// <returns>A local file path, or null for non-local sources.</returns>
    private static string? GetLocalPath(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        if (Uri.TryCreate(source, UriKind.Absolute, out var uri))
        {
            return uri.IsFile ? uri.LocalPath : null;
        }

        return Path.IsPathRooted(source) ? source : null;
    }

    /// <summary>Gets the supported MIME type from the local file extension.</summary>
    /// <param name="path">The image path.</param>
    /// <returns>The MIME type for a supported format.</returns>
    private static string? GetMimeType(string? path) => Path.GetExtension(path)?.ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        ".webp" => "image/webp",
        _ => null
    };

    /// <summary>Reads a local image when its actual size remains within the configured limit.</summary>
    /// <param name="path">The image path.</param>
    /// <param name="mimeType">The supported MIME type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A data URI, or null when the file cannot be embedded.</returns>
    private static async Task<string?> TryReadImageAsync(string path, string mimeType, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, ReadBufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
            if (stream.Length is <= 0 or > MaximumImageBytes)
            {
                return null;
            }

            await using var buffer = new MemoryStream((int)stream.Length);
            var chunk = new byte[ReadBufferSize];
            int read;
            while ((read = await stream.ReadAsync(chunk, cancellationToken).ConfigureAwait(false)) > 0)
            {
                if (buffer.Length + read > MaximumImageBytes)
                {
                    return null;
                }

                await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
            }

            return $"data:{mimeType};base64,{Convert.ToBase64String(buffer.ToArray())}";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            return null;
        }
    }
}
