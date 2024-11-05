// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace CrissCross.WPF.UI.Controls.Extensions;

internal static class BitArrayExtensions
{
    public static short ToInt16(this BitArray bitArray)
    {
        short n = 0;
        for (var i = bitArray.Length - 1; i >= 0; i--)
        {
            n = (short)((n << 1) + (bitArray[i] ? 1 : 0));
        }

        return n;
    }
}
