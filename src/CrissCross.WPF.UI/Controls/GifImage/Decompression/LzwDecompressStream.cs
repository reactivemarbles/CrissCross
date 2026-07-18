// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Buffer = System.Buffer;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decompression;
#else
namespace CrissCross.WPF.UI.Controls.Decompression;
#endif

/// <summary>Provides the LzwDecompressStream member.</summary>
/// <param name="compressedBuffer">The compressedBuffer value.</param>
/// <param name="minimumCodeLength">The minimumCodeLength value.</param>
internal sealed class LzwDecompressStream(byte[] compressedBuffer, int minimumCodeLength) : Stream
{
    /// <summary>Stores the _reader value.</summary>
    private readonly BitReader _reader = new(compressedBuffer);

    /// <summary>Stores the _codeTable value.</summary>
    private readonly CodeTable _codeTable = new(minimumCodeLength);

    /// <summary>Stores the _prevCode value.</summary>
    private int _prevCode = -1;

    /// <summary>Stores the _remainingBytes value.</summary>
    private byte[]? _remainingBytes;

    /// <summary>Stores the _endOfStream value.</summary>
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

    public override void Flush() { }

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

    /// <summary>Provides the CopySequenceToBuffer member.</summary>
    /// <param name="sequence">The sequence value.</param>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="offset">The offset value.</param>
    /// <param name="count">The count value.</param>
    /// <param name="read">The read value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the ValidateReadArgs member.</summary>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="offset">The offset value.</param>
    /// <param name="count">The count value.</param>
    [Conditional("DISABLED")]
    private static void ValidateReadArgs(byte[] buffer, int offset, int count)
    {
        if (buffer is null)
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

        if (offset + count <= buffer.Length)
        {
            return;
        }

        throw new ArgumentException("Buffer is to small to receive the requested data");
    }

    /// <summary>Provides the InitCodeTable member.</summary>
    private void InitCodeTable()
    {
        _codeTable.Reset();
        _prevCode = -1;
    }

    /// <summary>Provides the FlushRemainingBytes member.</summary>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="offset">The offset value.</param>
    /// <param name="count">The count value.</param>
    /// <param name="read">The read value.</param>
    private void FlushRemainingBytes(byte[] buffer, int offset, int count, ref int read)
    {
        // If we read too many bytes last time, copy them first.
        if (_remainingBytes is null)
        {
            return;
        }

        _remainingBytes = CopySequenceToBuffer(_remainingBytes, buffer, offset, count, ref read);
    }

    /// <summary>Provides the ProcessCode member.</summary>
    /// <param name="code">The code value.</param>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="offset">The offset value.</param>
    /// <param name="count">The count value.</param>
    /// <param name="read">The read value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the Sequence member.</summary>
    private readonly struct Sequence
    {
        /// <summary>Initializes a new instance of the <see cref="Sequence"/> struct.</summary>
        /// <param name="bytes">The bytes value.</param>
        public Sequence(byte[] bytes)
            : this() => Bytes = bytes;

        /// <summary>Initializes a new instance of the <see cref="Sequence"/> struct.</summary>
        /// <param name="isClearCode">The isClearCode value.</param>
        /// <param name="isStopCode">The isStopCode value.</param>
        private Sequence(bool isClearCode, bool isStopCode)
            : this()
        {
            IsClearCode = isClearCode;
            IsStopCode = isStopCode;
        }

        /// <summary>Gets the ClearCode value.</summary>
        public static Sequence ClearCode { get; } = new Sequence(true, false);

        /// <summary>Gets the StopCode value.</summary>
        public static Sequence StopCode { get; } = new Sequence(false, true);

        /// <summary>Gets the Bytes value.</summary>
        public byte[]? Bytes { get; }

        /// <summary>Gets the IsClearCode value.</summary>
        public bool IsClearCode { get; }

        /// <summary>Gets the IsStopCode value.</summary>
        public bool IsStopCode { get; }

        /// <summary>Provides the Append member.</summary>
        /// <param name="b">The b value.</param>
        /// <returns>The result.</returns>
        public Sequence Append(byte b)
        {
            var bytes = new byte[Bytes!.Length + 1];
            Bytes.CopyTo(bytes, 0);
            bytes[Bytes.Length] = b;
            return new Sequence(bytes);
        }
    }

    /// <summary>Provides the CodeTable member.</summary>
    private sealed class CodeTable
    {
        /// <summary>Provides the MaxCodeLength member.</summary>
        private const int MaxCodeLength = 12;

        /// <summary>The number of reserved clear and stop codes in the GIF LZW table.</summary>
        private const int ReservedCodeCount = 2;

        /// <summary>Stores the _minimumCodeLength value.</summary>
        private readonly int _minimumCodeLength;

        /// <summary>Stores the _table value.</summary>
        private readonly Sequence[] _table;

        /// <summary>Stores the _count value.</summary>
        private int _count;

        /// <summary>Stores the _codeLength value.</summary>
        private int _codeLength;

        /// <summary>Initializes a new instance of the <see cref="CodeTable"/> class.</summary>
        /// <param name="minimumCodeLength">The minimumCodeLength value.</param>
        public CodeTable(int minimumCodeLength)
        {
            _minimumCodeLength = minimumCodeLength;
            _codeLength = _minimumCodeLength + 1;
            var initialEntries = 1 << minimumCodeLength;
            _table = new Sequence[1 << MaxCodeLength];
            for (var i = 0; i < initialEntries; i++)
            {
                _table[_count] = new([(byte)i]);
                _count++;
            }

            Add(Sequence.ClearCode);
            Add(Sequence.StopCode);
        }

        /// <summary>Gets Count.</summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _count;
        }

        /// <summary>Gets CodeLength.</summary>
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

        /// <summary>Provides the Reset member.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _count = (1 << _minimumCodeLength) + ReservedCodeCount;
            _codeLength = _minimumCodeLength + 1;
        }

        /// <summary>Provides the Add member.</summary>
        /// <param name="sequence">The sequence value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Sequence sequence)
        {
            // Code table is full, stop adding new codes
            if (_count >= _table.Length)
            {
                return;
            }

            _table[_count] = sequence;
            _count++;
            if ((_count & (_count - 1)) != 0 || _codeLength >= MaxCodeLength)
            {
                return;
            }

            _codeLength++;
        }
    }
}
