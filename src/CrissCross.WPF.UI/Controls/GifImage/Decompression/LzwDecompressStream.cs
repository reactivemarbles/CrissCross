// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Buffer = System.Buffer;

namespace CrissCross.WPF.UI.Controls.Decompression;

internal class LzwDecompressStream(byte[] compressedBuffer, int minimumCodeLength) : Stream
{
    private const int MaxCodeLength = 12;
    private readonly BitReader _reader = new(compressedBuffer);
    private readonly CodeTable _codeTable = new(minimumCodeLength);
    private int _prevCode = -1;
    private byte[]? _remainingBytes;
    private bool _endOfStream;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateReadArgs(buffer, offset, count);

        if (_endOfStream)
        {
            return 0;
        }

        var read = 0;

        FlushRemainingBytes(buffer, offset, count, ref read);

        while (read < count)
        {
            var code = _reader.ReadBits(_codeTable.CodeLength);

            if (!ProcessCode(code, buffer, offset, count, ref read))
            {
                _endOfStream = true;
                break;
            }
        }

        return read;
    }

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    private static byte[]? CopySequenceToBuffer(byte[]? sequence, byte[] buffer, int offset, int count, ref int read)
    {
        var bytesToRead = Math.Min(sequence!.Length, count - read);
        Buffer.BlockCopy(sequence, 0, buffer, offset + read, bytesToRead);
        read += bytesToRead;
        byte[]? remainingBytes = null;
        if (bytesToRead < sequence.Length)
        {
            var remainingBytesCount = sequence.Length - bytesToRead;
            remainingBytes = new byte[remainingBytesCount];
            Buffer.BlockCopy(sequence, bytesToRead, remainingBytes, 0, remainingBytesCount);
        }

        return remainingBytes;
    }

    [Conditional("DISABLED")]
    private static void ValidateReadArgs(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset can't be negative");
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count can't be negative");
        }

        if (offset + count > buffer.Length)
        {
            throw new ArgumentException("Buffer is to small to receive the requested data");
        }
    }

    private void InitCodeTable()
    {
        _codeTable.Reset();
        _prevCode = -1;
    }

    private void FlushRemainingBytes(byte[] buffer, int offset, int count, ref int read)
    {
        // If we read too many bytes last time, copy them first;
        if (_remainingBytes != null)
        {
            _remainingBytes = CopySequenceToBuffer(_remainingBytes, buffer, offset, count, ref read);
        }
    }

    private bool ProcessCode(int code, byte[] buffer, int offset, int count, ref int read)
    {
        if (code < _codeTable.Count)
        {
            var sequence = _codeTable[code];
            if (sequence.IsStopCode)
            {
                return false;
            }

            if (sequence.IsClearCode)
            {
                InitCodeTable();
                return true;
            }

            _remainingBytes = CopySequenceToBuffer(sequence.Bytes, buffer, offset, count, ref read);
            if (_prevCode >= 0)
            {
                var prev = _codeTable[_prevCode];
                var newSequence = prev.Append(sequence.Bytes![0]);
                _codeTable.Add(newSequence);
            }
        }
        else
        {
            var prev = _codeTable[_prevCode];
            var newSequence = prev.Append(prev.Bytes![0]);
            _codeTable.Add(newSequence);
            _remainingBytes = CopySequenceToBuffer(newSequence.Bytes, buffer, offset, count, ref read);
        }

        _prevCode = code;
        return true;
    }

    private struct Sequence
    {
        public Sequence(byte[] bytes)
            : this() => Bytes = bytes;

        private Sequence(bool isClearCode, bool isStopCode)
            : this()
        {
            IsClearCode = isClearCode;
            IsStopCode = isStopCode;
        }

        public static Sequence ClearCode { get; } = new Sequence(true, false);

        public static Sequence StopCode { get; } = new Sequence(false, true);

        public byte[]? Bytes { get; }

        public bool IsClearCode { get; }

        public bool IsStopCode { get; }

        public Sequence Append(byte b)
        {
            var bytes = new byte[Bytes!.Length + 1];
            Bytes.CopyTo(bytes, 0);
            bytes[Bytes.Length] = b;
            return new Sequence(bytes);
        }
    }

    private class CodeTable
    {
        private readonly int _minimumCodeLength;
        private readonly Sequence[] _table;
        private int _count;
        private int _codeLength;

        public CodeTable(int minimumCodeLength)
        {
            _minimumCodeLength = minimumCodeLength;
            _codeLength = _minimumCodeLength + 1;
            var initialEntries = 1 << minimumCodeLength;
            _table = new Sequence[1 << MaxCodeLength];
            for (var i = 0; i < initialEntries; i++)
            {
                _table[_count++] = new Sequence(new[] { (byte)i });
            }

            Add(Sequence.ClearCode);
            Add(Sequence.StopCode);
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
        }

        public int CodeLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _codeLength;
        }

        public Sequence this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _table[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _count = (1 << _minimumCodeLength) + 2;
            _codeLength = _minimumCodeLength + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Sequence sequence)
        {
            // Code table is full, stop adding new codes
            if (_count >= _table.Length)
            {
                return;
            }

            _table[_count++] = sequence;
            if ((_count & (_count - 1)) == 0 && _codeLength < MaxCodeLength)
            {
                _codeLength++;
            }
        }
    }
}
