// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides the documented member.</summary>
public sealed class TextPointer : IComparable<TextPointer>, IComparable
{
    /// <summary>Initializes a new instance of the <see cref="TextPointer"/> class.</summary>
    /// <param name="document">The document value.</param>
    /// <param name="offset">The offset value.</param>
    internal TextPointer(FlowDocument document, int offset)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        Offset = Math.Clamp(offset, 0, document.Length);
    }

    /// <summary>Gets the owning document.</summary>
    public FlowDocument Document { get; }

    /// <summary>Gets the zero-based character offset.</summary>
    public int Offset { get; }

    /// <summary>Determines whether two pointers refer to the same position.</summary>
    /// <param name="left">First pointer to compare.</param>
    /// <param name="right">Second pointer to compare.</param>
    public static bool operator ==(TextPointer? left, TextPointer? right)
    {
        return ReferenceEquals(left, right) || (left is not null && right is not null && left.Equals(right));
    }

    /// <summary>Determines whether two pointers refer to different positions.</summary>
    /// <param name="left">First pointer to compare.</param>
    /// <param name="right">Second pointer to compare.</param>
    public static bool operator !=(TextPointer? left, TextPointer? right)
    {
        return !ReferenceEquals(left, right) && (left is null || right is null || !left.Equals(right));
    }

    /// <summary>Determines whether <paramref name="left"/> precedes <paramref name="right"/>.</summary>
    /// <param name="left">First pointer to compare.</param>
    /// <param name="right">Second pointer to compare.</param>
    public static bool operator <(TextPointer left, TextPointer right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.CompareTo(right) < 0;
    }

    /// <summary>Determines whether <paramref name="left"/> follows <paramref name="right"/>.</summary>
    /// <param name="left">First pointer to compare.</param>
    /// <param name="right">Second pointer to compare.</param>
    public static bool operator >(TextPointer left, TextPointer right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.CompareTo(right) > 0;
    }

    /// <summary>Determines whether <paramref name="left"/> precedes or equals <paramref name="right"/>.</summary>
    /// <param name="left">First pointer to compare.</param>
    /// <param name="right">Second pointer to compare.</param>
    public static bool operator <=(TextPointer left, TextPointer right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.CompareTo(right) <= 0;
    }

    /// <summary>Determines whether <paramref name="left"/> follows or equals <paramref name="right"/>.</summary>
    /// <param name="left">First pointer to compare.</param>
    /// <param name="right">Second pointer to compare.</param>
    public static bool operator >=(TextPointer left, TextPointer right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.CompareTo(right) >= 0;
    }

    /// <summary>Creates a new pointer offset by the specified number of characters.</summary>
    /// <param name="offset">The relative offset.</param>
    /// <returns>A pointer constrained to the document bounds.</returns>
    public TextPointer GetPositionAtOffset(int offset) => new(Document, Offset + offset);

    /// <inheritdoc />
    public int CompareTo(TextPointer? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (!ReferenceEquals(Document, other.Document))
        {
            throw new InvalidOperationException("Cannot compare pointers belonging to different documents.");
        }

        return Offset.CompareTo(other.Offset);
    }

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is TextPointer x)
        {
            return CompareTo(x);
        }

        throw new ArgumentException(string.Empty, nameof(obj));
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is TextPointer other && ReferenceEquals(Document, other.Document) && Offset == other.Offset;

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Document, Offset);
}
