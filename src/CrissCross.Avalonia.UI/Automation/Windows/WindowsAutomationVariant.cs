// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Preserves native VARIANT field order and pointer-sized union storage.</summary>
/// <param name="type">The native VARIANT type.</param>
/// <param name="reserved1">The first reserved word.</param>
/// <param name="reserved2">The second reserved word.</param>
/// <param name="reserved3">The third reserved word.</param>
/// <param name="data">The union payload.</param>
/// <param name="recordInformation">The record information payload.</param>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct WindowsAutomationVariant(
    ushort type,
    ushort reserved1,
    ushort reserved2,
    ushort reserved3,
    nint data,
    nint recordInformation) : IEquatable<WindowsAutomationVariant>
{
    /// <summary>The native variant type.</summary>
    private readonly ushort _type = type;

    /// <summary>The first native reserved word.</summary>
    private readonly ushort _reserved1 = reserved1;

    /// <summary>The second native reserved word.</summary>
    private readonly ushort _reserved2 = reserved2;

    /// <summary>The third native reserved word.</summary>
    private readonly ushort _reserved3 = reserved3;

    /// <summary>The native union payload.</summary>
    private readonly nint _data = data;

    /// <summary>The native record pointer payload.</summary>
    private readonly nint _recordInformation = recordInformation;

    /// <summary>Compares two native VARIANT storage values.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns>Whether the storage is equal.</returns>
    public static bool operator ==(WindowsAutomationVariant left, WindowsAutomationVariant right) => left.Equals(right);

    /// <summary>Compares two native VARIANT storage values.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns>Whether the storage differs.</returns>
    public static bool operator !=(WindowsAutomationVariant left, WindowsAutomationVariant right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(WindowsAutomationVariant other) =>
        _type == other._type
        && _reserved1 == other._reserved1
        && _reserved2 == other._reserved2
        && _reserved3 == other._reserved3
        && _data == other._data
        && _recordInformation == other._recordInformation;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is WindowsAutomationVariant other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_type, _reserved1, _reserved2, _reserved3, _data, _recordInformation);
}
