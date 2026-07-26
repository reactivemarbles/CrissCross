// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Imaging;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Extensions;
#else
namespace CrissCross.WPF.UI.Controls.Extensions;
#endif

/// <summary>Provides the WritableBitmapExtensions member.</summary>
internal static class WritableBitmapExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="bitmap">The bitmap value.</param>
    extension(WriteableBitmap bitmap)
    {
        /// <summary>Provides the LockInScope member.</summary>
        /// <returns>The result.</returns>
        internal IDisposable LockInScope() => new WriteableBitmapLock(bitmap);
    }

    /// <summary>Provides the WriteableBitmapLock member.</summary>
    private sealed class WriteableBitmapLock : IDisposable
    {
        /// <summary>Stores the _bitmap value.</summary>
        private readonly WriteableBitmap _bitmap;

        /// <summary>Initializes a new instance of the <see cref="WriteableBitmapLock"/> class.</summary>
        /// <param name="bitmap">The bitmap value.</param>
        internal WriteableBitmapLock(WriteableBitmap bitmap)
        {
            _bitmap = bitmap;
            _bitmap.Lock();
        }

        /// <summary>Provides the Dispose member.</summary>
        void IDisposable.Dispose() => _bitmap.Unlock();
    }
}
