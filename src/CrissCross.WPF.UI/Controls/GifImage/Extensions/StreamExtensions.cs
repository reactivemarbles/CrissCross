// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Extensions;

/// <summary>Provides the StreamExtensions member.</summary>
internal static class StreamExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="stream">The stream value.</param>
    extension(Stream stream)
    {
        /// <summary>Provides the ReadAllAsync member.</summary>
        /// <param name="buffer">The buffer value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="count">The count value.</param>
        /// <param name="cancellationToken">The cancellationToken value.</param>
        /// <returns>The result.</returns>
        public async Task ReadAllAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken = default)
        {
            var totalRead = 0;
            while (totalRead < count)
            {
                var n = await stream.ReadBufferAsync(buffer, offset + totalRead, count - totalRead, cancellationToken);
                if (n == 0)
                {
                    throw new EndOfStreamException();
                }

                totalRead += n;
            }
        }

        /// <summary>Provides the ReadAll member.</summary>
        /// <param name="buffer">The buffer value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="count">The count value.</param>
        public void ReadAll(byte[] buffer, int offset, int count)
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

        /// <summary>Provides the ReadByteAsync member.</summary>
        /// <param name="cancellationToken">The cancellationToken value.</param>
        /// <returns>The result.</returns>
        public async Task<int> ReadByteAsync(CancellationToken cancellationToken = default)
        {
            var buffer = new byte[1];
            var n = await stream.ReadBufferAsync(buffer, 0, 1, cancellationToken);
            return n switch
            {
                0 => -1,
                _ => buffer[0],
            };
        }

        /// <summary>Provides the AsBuffered member.</summary>
        /// <returns>The result.</returns>
        public Stream AsBuffered()
        {
            return stream is BufferedStream bs ? bs : new BufferedStream(stream);
        }

        /// <summary>Provides the CopyToAsync member.</summary>
        /// <param name="destination">The destination value.</param>
        /// <param name="progress">The progress value.</param>
        /// <param name="bufferSize">The bufferSize value.</param>
        /// <param name="cancellationToken">The cancellationToken value.</param>
        /// <returns>The result.</returns>
        public async Task CopyToAsync(
            Stream destination,
            IProgress<long> progress,
            int bufferSize = 81_920,
            CancellationToken cancellationToken = default)
        {
            var buffer = new byte[bufferSize];
            int bytesRead;
            long bytesCopied = 0;
            while (
                (
                    bytesRead = await stream
                        .ReadBufferAsync(buffer, 0, buffer.Length, cancellationToken)
                        .ConfigureAwait(false)) != 0)
            {
                await destination.WriteBufferAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                bytesCopied += bytesRead;
                progress?.Report(bytesCopied);
            }
        }

        /// <summary>Provides the ReadBufferAsync member.</summary>
        /// <param name="buffer">The buffer value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="count">The count value.</param>
        /// <param name="cancellationToken">The cancellationToken value.</param>
        /// <returns>The result.</returns>
        public Task<int> ReadBufferAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken = default)
        {
#if NET472 || NET48 || NET481
            return stream.ReadAsync(buffer, offset, count, cancellationToken);
#else
            return stream.ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
#endif
        }

        /// <summary>Provides the WriteBufferAsync member.</summary>
        /// <param name="buffer">The buffer value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="count">The count value.</param>
        /// <param name="cancellationToken">The cancellationToken value.</param>
        /// <returns>The result.</returns>
        public Task WriteBufferAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken = default)
        {
#if NET472 || NET48 || NET481
            return stream.WriteAsync(buffer, offset, count, cancellationToken);
#else
            return stream.WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
#endif
        }
    }
}
