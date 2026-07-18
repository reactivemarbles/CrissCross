// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the Helpers members for <see cref="RichTextBox"/>.</summary>
public partial class RichTextBox
{
    /// <summary>Provides the RichTextHistoryEntry member.</summary>
    /// <param name="Html">The Html value.</param>
    /// <param name="SelectionStart">The SelectionStart value.</param>
    /// <param name="SelectionEnd">The SelectionEnd value.</param>
    /// <param name="CaretIndex">The CaretIndex value.</param>
    private readonly record struct RichTextHistoryEntry(
        string Html,
        int SelectionStart,
        int SelectionEnd,
        int CaretIndex);

    /// <summary>Provides the RichTextHelpers member.</summary>
    private static class RichTextHelpers
    {
        /// <summary>Provides the supported text file extensions.</summary>
        private static readonly string[] SupportedTextFileExtensions =
        [
            ".txt",
            ".md",
            ".markdown",
            ".csv",
            ".log",
            ".json",
            ".xml",
            ".html",
            ".htm",
            ".rtf",];

        /// <summary>Sets drag and drop availability on a control when it exists.</summary>
        /// <param name="control">The target control.</param>
        /// <param name="allowDrop">Whether drag and drop is allowed.</param>
        public static void SetAllowDrop(Control? control, bool allowDrop)
        {
            if (control is null)
            {
                return;
            }

            DragDrop.SetAllowDrop(control, allowDrop);
        }

        /// <summary>Determines whether a context menu header controls formatting.</summary>
        /// <param name="header">The menu header.</param>
        /// <returns><see langword="true"/> when the header represents a formatting command.</returns>
        public static bool IsFormattingMenuHeader(string? header) =>
            header
                is "Bold"
                    or "Italic"
                    or "Underline"
                    or "Strikethrough"
                    or "Font"
                    or "Font Size"
                    or "Foreground"
                    or "Highlight";

        /// <summary>Appends selected text from one segment as HTML.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <param name="start">The selected range start.</param>
        /// <param name="end">The selected range end.</param>
        /// <param name="builder">The destination builder.</param>
        public static void AppendSelectedSegmentHtml(
            TextSegment segment,
            int start,
            int end,
            StringBuilder builder)
        {
            if (
                !segment.HasRenderableText
                || segment.EndIndex <= start
                || segment.StartIndex >= end)
            {
                return;
            }

            var segmentStart = Math.Max(start, segment.StartIndex);
            var segmentEnd = Math.Min(end, segment.EndIndex);
            var segmentLength = segmentEnd - segmentStart;
            if (segmentLength <= 0)
            {
                return;
            }

            var localStart = segmentStart - segment.StartIndex;
            var selectedText = segment.Text.Substring(localStart, segmentLength);
            _ = builder.Append(FormatSegmentAsHtml(segment, selectedText));
        }

        /// <summary>Determines whether one segment contains formatting.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <returns><see langword="true"/> when formatting is present.</returns>
        public static bool HasFormatting(TextSegment segment) =>
            HasCharacterFormatting(segment)
            || HasObjectFormatting(segment)
            || segment.ParagraphAlignment.HasValue;

        /// <summary>Determines whether one segment contains character formatting.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <returns><see langword="true"/> when character formatting is present.</returns>
        public static bool HasCharacterFormatting(TextSegment segment) =>
            segment.IsBold
            || segment.IsItalic
            || segment.IsUnderline
            || segment.IsStrikethrough
            || segment.Foreground is not null
            || segment.Background is not null
            || segment.FontSize.HasValue
            || segment.FontFamily is not null;

        /// <summary>Determines whether one segment contains object formatting.</summary>
        /// <param name="segment">The segment to inspect.</param>
        /// <returns><see langword="true"/> when object formatting is present.</returns>
        public static bool HasObjectFormatting(TextSegment segment) => segment.IsImage;

        /// <summary>Provides the GetStorageItemPath member.</summary>
        /// <param name="file">The storage item.</param>
        /// <returns>The local storage item path when available.</returns>
        public static string? GetStorageItemPath(IStorageItem file)
        {
            var localPath = file.TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(localPath))
            {
                return localPath;
            }

            return file.Path.IsAbsoluteUri ? file.Path.AbsolutePath : file.Path.ToString();
        }

        /// <summary>Provides the TryCreateImageSourceAsync member.</summary>
        /// <param name="file">The file value.</param>
        /// <returns>The result.</returns>
        public static async Task<string?> TryCreateImageSourceAsync(IStorageFile file)
        {
            var path = GetStorageItemPath(file);
            if (!IsSupportedImagePath(path))
            {
                return null;
            }

            var dataUri = await TryCreateImageDataUriAsync(file, path).ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(dataUri))
            {
                var localPath = file.TryGetLocalPath();
                return string.IsNullOrWhiteSpace(localPath)
                    ? dataUri
                    : new Uri(localPath, UriKind.Absolute).AbsoluteUri;
            }

            return file.Path.Scheme is "http" or "https" ? file.Path.AbsoluteUri : null;
        }

        /// <summary>Provides the FormatSegmentAsHtml member.</summary>
        /// <param name="segment">The segment value.</param>
        /// <param name="text">The text value.</param>
        /// <returns>The result.</returns>
        public static string FormatSegmentAsHtml(TextSegment segment, string text)
        {
            var result = HtmlClipboardUtilities.EncodePlainText(text);
            var styles = new List<string>();
            if (segment.FontFamily is not null)
            {
                styles.Add($"font-family:{segment.FontFamily.Name}");
            }

            if (segment.FontSize.HasValue)
            {
                styles.Add(FormattableString.Invariant($"font-size:{segment.FontSize.Value}px"));
            }

            if (segment.Foreground is SolidColorBrush foreground)
            {
                styles.Add($"color:{foreground.Color}");
            }

            if (segment.Background is SolidColorBrush background)
            {
                styles.Add($"background-color:{background.Color}");
            }

            if (styles.Count > 0)
            {
                result = $"<span style=\"{string.Join(';', styles)}\">{result}</span>";
            }

            if (segment.IsStrikethrough)
            {
                result = $"<s>{result}</s>";
            }

            if (segment.IsUnderline)
            {
                result = $"<u>{result}</u>";
            }

            if (segment.IsItalic)
            {
                result = $"<em>{result}</em>";
            }

            if (segment.IsBold)
            {
                result = $"<strong>{result}</strong>";
            }

            return result;
        }

        /// <summary>Provides the LooksLikeHtml member.</summary>
        /// <param name="text">The text value.</param>
        /// <returns>The result.</returns>
        public static bool LooksLikeHtml(string text) =>
            text.Contains('<', StringComparison.Ordinal)
            && text.Contains('>', StringComparison.Ordinal);

        /// <summary>Provides the NormalizeClipboardText member.</summary>
        /// <param name="textPayload">The textPayload value.</param>
        /// <returns>The result.</returns>
        public static string NormalizeClipboardText(string? textPayload)
        {
            if (string.IsNullOrWhiteSpace(textPayload))
            {
                return string.Empty;
            }

            var fragment = HtmlClipboardUtilities.ExtractFragment(textPayload);
            return string.IsNullOrWhiteSpace(fragment) ? textPayload : fragment;
        }

        /// <summary>Provides the IsSupportedImageSource member.</summary>
        /// <param name="source">The source value.</param>
        /// <returns>The result.</returns>
        public static bool IsSupportedImageSource(string? source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            var value = source.Trim();
            if (value.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            {
                return value.Contains(";base64,", StringComparison.OrdinalIgnoreCase)
                    || value.Contains(',', StringComparison.Ordinal);
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return IsSupportedImagePath(uri.IsFile ? uri.LocalPath : uri.AbsolutePath);
            }

            return IsSupportedImagePath(value);
        }

        /// <summary>Provides the IsSupportedImagePath member.</summary>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
        public static bool IsSupportedImagePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var extension = Path.GetExtension(path) ?? string.Empty;
            return extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".gif", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".webp", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Provides the IsSupportedTextFilePath member.</summary>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
        public static bool IsSupportedTextFilePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var extension = Path.GetExtension(path) ?? string.Empty;
            return SupportedTextFileExtensions.Contains(
                extension,
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Provides the GetImageMimeType member.</summary>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
        public static string GetImageMimeType(string? path)
        {
            var extension = Path.GetExtension(path) ?? string.Empty;
            if (
                extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return "image/jpeg";
            }

            if (extension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
            {
                return "image/gif";
            }

            if (extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                return "image/bmp";
            }

            return extension.Equals(".webp", StringComparison.OrdinalIgnoreCase)
                ? "image/webp"
                : "image/png";
        }

        /// <summary>Provides the CreateImageHtml member.</summary>
        /// <param name="imageSource">The imageSource value.</param>
        /// <returns>The result.</returns>
        public static string CreateImageHtml(string imageSource) =>
            $"<img src=\"{imageSource.Replace("\"", "%22", StringComparison.Ordinal)}\" />";

        /// <summary>Provides the TryCreateImageDataUriAsync member.</summary>
        /// <param name="file">The file value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>The result.</returns>
        private static async Task<string?> TryCreateImageDataUriAsync(
            IStorageFile file,
            string? path)
        {
            try
            {
                await using var stream = await file.OpenReadAsync().ConfigureAwait(true);
                await using var buffer = new MemoryStream();
                await stream.CopyToAsync(buffer).ConfigureAwait(true);
                var bytes = buffer.ToArray();
                if (!HasExpectedImageSignature(bytes, path))
                {
                    return null;
                }

                var mimeType = GetImageMimeType(path);
                return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>Checks that file content starts with the signature expected for its image extension.</summary>
        /// <param name="bytes">The image file bytes.</param>
        /// <param name="path">The image file path.</param>
        /// <returns>The result.</returns>
        private static bool HasExpectedImageSignature(ReadOnlySpan<byte> bytes, string? path)
        {
            var extension = Path.GetExtension(path) ?? string.Empty;
            return extension.ToUpperInvariant() switch
            {
                ".PNG" => bytes.StartsWith(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                ".JPG" or ".JPEG" => bytes.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }),
                ".GIF" => bytes.StartsWith("GIF87a"u8) || bytes.StartsWith("GIF89a"u8),
                ".BMP" => bytes.StartsWith("BM"u8),
                ".WEBP" => HasWebPImageSignature(bytes),
                _ => false,
            };
        }

        /// <summary>Checks for the RIFF and WebP markers in a WebP image header.</summary>
        /// <param name="bytes">The image file bytes.</param>
        /// <returns>The result.</returns>
        private static bool HasWebPImageSignature(ReadOnlySpan<byte> bytes) =>
            bytes.Length >= WebPHeaderLength
            && bytes[..RiffSignatureLength].SequenceEqual("RIFF"u8)
            && bytes[WebPSignatureOffset..WebPHeaderLength].SequenceEqual("WEBP"u8);
    }
}
