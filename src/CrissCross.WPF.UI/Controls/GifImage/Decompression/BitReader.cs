// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decompression;

/// <summary>Provides the BitReader member.</summary>
/// <param name="buffer">The buffer value.</param>
internal sealed class BitReader(byte[] buffer)
{
    /// <summary>Stores the _bytePosition value.</summary>
    private int _bytePosition = -1;

    /// <summary>Stores the _bitPosition value.</summary>
    private int _bitPosition;

    /// <summary>Stores the _currentvalue.</summary>
    private int _currentValue = -1;

    /// <summary>Provides the ReadBits member.</summary>
    /// <param name="bitCount">The bitCount value.</param>
    /// <returns>The result.</returns>
    public int ReadBits(int bitCount)
    {
        // The following code assumes it's running on a little-endian architecture.
        // It's probably safe to assume it will always be the case, because:
        // - Windows only supports little-endian architectures: x86/x64 and ARM (which supports
        //   both endiannesses, but Windows on ARM is always in little-endian mode)
        // - No platforms other than Windows support XAML applications
        // If the situation changes, this code will have to be updated.
        if (_bytePosition == -1)
        {
            _bytePosition = 0;
            _bitPosition = 0;
            _currentValue = ReadInt32();
        }
        else if (bitCount > 32 - _bitPosition)
        {
            var n = _bitPosition >> 3;
            _bytePosition += n;
            _bitPosition &= 0x07;
            _currentValue = ReadInt32() >> _bitPosition;
        }

        var mask = (1 << bitCount) - 1;
        var value = _currentValue & mask;
        _currentValue >>= bitCount;
        _bitPosition += bitCount;
        return value;
    }

    /// <summary>Provides the ReadInt32 member.</summary>
    /// <returns>The result.</returns>
    private int ReadInt32()
    {
        var value = 0;
        for (var i = 0; i < 4; i++)
        {
            if (_bytePosition + i >= buffer.Length)
            {
                break;
            }

            value |= buffer[_bytePosition + i] << (i << 3);
        }

        return value;
    }
}
