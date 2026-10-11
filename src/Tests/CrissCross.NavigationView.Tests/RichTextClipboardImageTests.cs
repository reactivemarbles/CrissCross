// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests local image handling for rich text clipboard HTML.</summary>
public sealed class RichTextClipboardImageTests
{
    /// <summary>A valid one-pixel PNG used by the local image fixture.</summary>
    private const string TinyPngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+j5XcAAAAASUVORK5CYII=";

    /// <summary>Embeds file URI images and preserves other image attributes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EmbedLocalImagesAsync_EmbedsFileUriAndPreservesAttributes()
    {
        var path = Path.Combine(Path.GetTempPath(), $"clipboard-{Guid.NewGuid():N}.png");
        await File.WriteAllBytesAsync(path, Convert.FromBase64String(TinyPngBase64));
        try
        {
            var source = new Uri(path).AbsoluteUri;
            var html = $"<p style=\"color:red\"><img src=\"{source}\" alt=\"tiny\" width=\"1\"></p>";
            var result = await RichTextClipboardImages.EmbedLocalImagesAsync(html, CancellationToken.None);

            await Assert.That(result).Contains("src=\"data:image/png;base64,");
            await Assert.That(result).Contains("alt=\"tiny\"");
            await Assert.That(result).Contains("width=\"1\"");
            await Assert.That(result).Contains("style=\"color:red\"");
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    /// <summary>Returns the exact original HTML when it contains no local image candidate.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EmbedLocalImagesAsync_RemoteImageUsesFastPath()
    {
        const string html = "<p><img src=\"https://example.test/image.png\"></p>";

        var result = await RichTextClipboardImages.EmbedLocalImagesAsync(html, CancellationToken.None);

        await Assert.That(result).IsSameReferenceAs(html);
    }

    /// <summary>Observes cancellation before parsing clipboard HTML.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EmbedLocalImagesAsync_ObservesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.That(async () => await RichTextClipboardImages.EmbedLocalImagesAsync("<p>text</p>", cancellation.Token))
            .Throws<OperationCanceledException>();
    }
}
