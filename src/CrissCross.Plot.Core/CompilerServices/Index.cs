// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if !NETSTANDARD2_1_OR_GREATER && !NET6_0_OR_GREATER

using System.Runtime.CompilerServices;

namespace System;

/// <summary>Represents an index into a collection from the start or the end.</summary>
/// <remarks>This compatibility type supports the C# index-from-end syntax on older target frameworks.</remarks>
internal readonly struct Index : IEquatable<Index>
{
    /// <summary>The encoded zero-based index value.</summary>
    private readonly int _value;

    /// <summary>Initializes a new instance of the <see cref="Index"/> class.</summary>
    /// <param name="value">The non-negative index value.</param>
    /// <param name="fromEnd">Whether the index is measured from the end.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Index(int value, bool fromEnd = false)
    {
        if (value < 0)
        {
            ThrowValueArgumentOutOfRange_NeedNonNegNumException();
        }

        _value = fromEnd ? ~value : value;
    }

    /// <summary>Initializes a new instance of the <see cref="Index"/> class from its encoded value without validation.</summary>
    /// <param name="value">The encoded index value.</param>
    private Index(int value)
    {
        _value = value;
    }

    /// <summary>Gets an index pointing to the first element.</summary>
    internal static Index Start => new(0);

    /// <summary>Gets an index pointing beyond the last element.</summary>
    internal static Index End => new(~0);

    /// <summary>Gets the non-negative index value.</summary>
    internal int Value => _value < 0 ? ~_value : _value;

    /// <summary>Gets whether the index is measured from the end.</summary>
    internal bool IsFromEnd => _value < 0;

    /// <summary>Converts an integer to an index measured from the start.</summary>
    /// <param name="value">The non-negative index value.</param>
    /// <returns>The corresponding index.</returns>
    public static implicit operator Index(int value) => FromStart(value);

    /// <summary>Determines whether this index equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal index.</returns>
    public override bool Equals(object? obj) => obj is Index other && Equals(other);

    /// <summary>Determines whether this index equals another index.</summary>
    /// <param name="other">The index to compare.</param>
    /// <returns><see langword="true"/> when both encoded values match.</returns>
    public bool Equals(Index other) => _value == other._value;

    /// <summary>Gets the hash code for this index.</summary>
    /// <returns>The encoded index value.</returns>
    public override int GetHashCode() => _value;

    /// <summary>Converts this index to its string representation.</summary>
    /// <returns>The index value, prefixed with a caret when measured from the end.</returns>
    public override string ToString() => IsFromEnd ? ToStringFromEnd() : ((uint)Value).ToString();

    /// <summary>Creates an index measured from the start.</summary>
    /// <param name="value">The non-negative index value.</param>
    /// <returns>The index from the start.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Index FromStart(int value)
    {
        if (value < 0)
        {
            ThrowValueArgumentOutOfRange_NeedNonNegNumException();
        }

        return new(value);
    }

    /// <summary>Creates an index measured from the end.</summary>
    /// <param name="value">The non-negative index value.</param>
    /// <returns>The index from the end.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Index FromEnd(int value)
    {
        if (value < 0)
        {
            ThrowValueArgumentOutOfRange_NeedNonNegNumException();
        }

        return new(~value);
    }

    /// <summary>Calculates the offset from the start for a collection of the specified length.</summary>
    /// <param name="length">The collection length.</param>
    /// <returns>The calculated offset.</returns>
    /// <remarks>The input length and resulting offset are not validated for performance.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetOffset(int length)
    {
        int offset = _value;
        if (IsFromEnd)
        {
            offset += length + 1;
        }

        return offset;
    }

    /// <summary>Throws the standard range exception for a negative index value.</summary>
    private static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
    {
#if SYSTEM_PRIVATE_CORELIB
        throw new ArgumentOutOfRangeException("value", SR.ArgumentOutOfRange_NeedNonNegNum);
#else
        throw new ArgumentOutOfRangeException("value", "value must be non-negative");
#endif
    }

    /// <summary>Formats this index using the from-end notation.</summary>
    /// <returns>The caret-prefixed index value.</returns>
    private string ToStringFromEnd() => '^' + Value.ToString();
}

#endif
