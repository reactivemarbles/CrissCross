// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// <auto-generated>

#nullable enable

using System.Runtime.InteropServices;

namespace CrissCross.WPF.UI.Interop;

/// <summary>
/// Common Windows API result;
/// </summary>
internal struct HRESULT
{
    ///<summary>
    /// Operation successful.
    ///</summary>
    public const int S_OK = unchecked((int)0x00000000);

    ///<summary>
    /// Operation successful.
    ///</summary>
    public const int NO_ERROR = unchecked((int)0x00000000);

    ///<summary>
    /// Operation successful.
    ///</summary>
    public const int NOERROR = unchecked((int)0x00000000);

    ///<summary>
    /// Unspecified failure.
    ///</summary>
    public const int S_FALSE = unchecked((int)0x00000001);

    public static void Check(int hr)
    {
        if (hr >= S_OK)
            return;

        Marshal.ThrowExceptionForHR(hr, (IntPtr)(-1));
    }
}
