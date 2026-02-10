// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Extensions;

internal static class StreamExtensions
{
    public static async Task ReadAllAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
    {
        var totalRead = 0;
        while (totalRead < count)
        {
            var n = await stream.ReadAsync(buffer.AsMemory(offset + totalRead, count - totalRead), cancellationToken);
            if (n == 0)
            {
                throw new EndOfStreamException();
            }

            totalRead += n;
        }
    }

    public static void ReadAll(this Stream stream, byte[] buffer, int offset, int count)
    {
        var totalRead = 0;
        while (totalRead < count)
        {
            var n = stream.Read(buffer, offset + totalRead, count - totalRead);
            if (n == 0)
            {
                throw new EndOfStreamException();
            }

            totalRead += n;
        }
    }

    public static async Task<int> ReadByteAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[1];
        var n = await stream.ReadAsync(buffer.AsMemory(0, 1), cancellationToken);
        return n switch
        {
            0 => -1,
            _ => buffer[0]
        };
    }

    public static Stream AsBuffered(this Stream stream)
    {
        if (stream is BufferedStream bs)
        {
            return bs;
        }

        return new BufferedStream(stream);
    }

    public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress, int bufferSize = 81920, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[bufferSize];
        int bytesRead;
        long bytesCopied = 0;
        while ((bytesRead = await source.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false)) != 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
            bytesCopied += bytesRead;
            progress?.Report(bytesCopied);
        }
    }
}
