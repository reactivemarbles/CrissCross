// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO.Packaging;
using System.Net.Http;
using System.Security.Cryptography;
using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides the UriLoader member.</summary>
internal static class UriLoader
{
    /// <summary>The completed download percentage.</summary>
    private const int CompletedDownloadPercentage = 100;

    /// <summary>Gets or sets DownloadCacheLocation.</summary>
    public static string DownloadCacheLocation { get; set; } = Path.GetTempPath();

    /// <summary>Provides the GetStreamFromUriAsync member.</summary>
    /// <param name="uri">The uri value.</param>
    /// <param name="progress">The progress value.</param>
    /// <returns>The result.</returns>
    public static Task<Stream> GetStreamFromUriAsync(Uri uri, IProgress<int>? progress)
    {
        return uri.IsAbsoluteUri && (uri.Scheme == "http" || uri.Scheme == "https")
            ? GetNetworkStreamAsync(uri, progress)!
            : GetStreamFromUriCoreAsync(uri);
    }

    /// <summary>Provides the GetNetworkStreamAsync member.</summary>
    /// <param name="uri">The uri value.</param>
    /// <param name="progress">The progress value.</param>
    /// <returns>The result.</returns>
    private static async Task<Stream?> GetNetworkStreamAsync(Uri uri, IProgress<int>? progress)
    {
        var cacheFileName = GetCacheFileName(uri);
        var cacheStream = await OpenTempFileStreamAsync(cacheFileName);
        if (cacheStream is null)
        {
            await DownloadToCacheFileAsync(uri, cacheFileName, progress);
            cacheStream = await OpenTempFileStreamAsync(cacheFileName);
        }

        progress?.Report(CompletedDownloadPercentage);
        return cacheStream;
    }

    /// <summary>Provides the DownloadToCacheFileAsync member.</summary>
    /// <param name="uri">The uri value.</param>
    /// <param name="fileName">The fileName value.</param>
    /// <param name="progress">The progress value.</param>
    /// <returns>The result.</returns>
    private static async Task DownloadToCacheFileAsync(Uri uri, string fileName, IProgress<int>? progress)
    {
        try
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await client.SendAsync(request);
            _ = response.EnsureSuccessStatusCode();
            var length = response.Content.Headers.ContentLength ?? 0;
#if NET8_0_OR_GREATER
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = await CreateTempFileStreamAsync(fileName);
#else
            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = await CreateTempFileStreamAsync(fileName);
#endif
            IProgress<long> absoluteProgress = default!;
            if (progress is not null)
            {
                absoluteProgress = new Progress<long>(bytesCopied =>
                {
                    if (length > 0)
                    {
                        progress.Report((int)(CompletedDownloadPercentage * bytesCopied / length));
                    }
                    else
                    {
                        progress.Report(-1);
                    }
                });
            }

            await responseStream.CopyToAsync(fileStream, absoluteProgress);
        }
        catch
        {
            DeleteTempFile(fileName);
            throw;
        }
    }

    /// <summary>Provides the GetStreamFromUriCoreAsync member.</summary>
    /// <param name="uri">The uri value.</param>
    /// <returns>The result.</returns>
    private static Task<Stream> GetStreamFromUriCoreAsync(Uri uri)
    {
        if (uri.Scheme == PackUriHelper.UriSchemePack)
        {
            var sri =
                uri.Authority == "siteoforigin:,,,"
                    ? Application.GetRemoteStream(uri)
                    : Application.GetResourceStream(uri);

            if (sri is not null)
            {
                return Task.FromResult(sri.Stream);
            }

            throw new FileNotFoundException("Cannot find file with the specified URI");
        }

        if (uri.Scheme == Uri.UriSchemeFile)
        {
            return Task.FromResult<Stream>(File.OpenRead(uri.LocalPath));
        }

        throw new NotSupportedException("Only pack:, file:, http: and https: URIs are supported");
    }

    /// <summary>Provides the OpenTempFileStreamAsync member.</summary>
    /// <param name="fileName">The fileName value.</param>
    /// <returns>The result.</returns>
    private static Task<Stream?> OpenTempFileStreamAsync(string fileName)
    {
        if (!Directory.Exists(DownloadCacheLocation))
        {
            _ = Directory.CreateDirectory(DownloadCacheLocation);
        }

        var path = Path.Combine(DownloadCacheLocation, fileName);
        Stream? stream = default;
        try
        {
            stream = File.OpenRead(path);
        }
        catch (FileNotFoundException) { }

        return Task.FromResult(stream);
    }

    /// <summary>Provides the CreateTempFileStreamAsync member.</summary>
    /// <param name="fileName">The fileName value.</param>
    /// <returns>The result.</returns>
    private static Task<Stream> CreateTempFileStreamAsync(string fileName)
    {
        var path = Path.Combine(DownloadCacheLocation, fileName);
        Stream stream = File.OpenWrite(path);
        stream.SetLength(0);
        return Task.FromResult(stream);
    }

    /// <summary>Provides the DeleteTempFile member.</summary>
    /// <param name="fileName">The fileName value.</param>
    private static void DeleteTempFile(string fileName)
    {
        var path = Path.Combine(DownloadCacheLocation, fileName);
        if (!File.Exists(path))
        {
            return;
        }

        File.Delete(path);
    }

    /// <summary>Provides the GetCacheFileName member.</summary>
    /// <param name="uri">The uri value.</param>
    /// <returns>The result.</returns>
    private static string GetCacheFileName(Uri uri)
    {
        // Use SHA256 instead of SHA1 to address CA5350
        var bytes = Encoding.UTF8.GetBytes(uri.AbsoluteUri);
#if NET5_0_OR_GREATER
        var hash = SHA256.HashData(bytes);
#else
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(bytes);
#endif
        return ToHex(hash);
    }

    /// <summary>Provides the ToHex member.</summary>
    /// <param name="bytes">The bytes value.</param>
    /// <returns>The result.</returns>
    private static string ToHex(byte[] bytes) =>
        bytes.Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("X2")), sb => sb.ToString());
}
