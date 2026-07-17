// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Extensions;
#else
namespace CrissCross.WPF.UI.Controls.Extensions;
#endif

/// <summary>Provides the BitArrayExtensions member.</summary>
internal static class BitArrayExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="bitArray">The bitArray value.</param>
    extension(BitArray bitArray)
    {
        /// <summary>Provides the ToInt16 member.</summary>
        /// <returns>The result.</returns>
        public short ToInt16()
        {
            short n = 0;
            for (var i = bitArray.Length - 1; i >= 0; i--)
            {
                n = (short)((n << 1) + (bitArray[i] ? 1 : 0));
            }

            return n;
        }
    }
}
