// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Imaging;

namespace CrissCross.WPF.UI.Controls.Extensions;

internal static class WritableBitmapExtensions
{
    public static IDisposable LockInScope(this WriteableBitmap bitmap) => new WriteableBitmapLock(bitmap);

    private class WriteableBitmapLock : IDisposable
    {
        private readonly WriteableBitmap _bitmap;

        public WriteableBitmapLock(WriteableBitmap bitmap)
        {
            _bitmap = bitmap;
            _bitmap.Lock();
        }

        public void Dispose()
        {
            _bitmap.Unlock();
        }
    }
}
