// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>Provides the UiElementExtensions member.</summary>
internal static class UiElementExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="element">The element value.</param>
    extension(UIElement element)
    {
        /// <summary>Do not call it outside of NCHITTEST, NCLBUTTONUP, NCLBUTTONDOWN messages.</summary>
        /// <param name="messageParameter">The message parameter value.</param>
        /// <returns><see langword="true"/> if mouse is over the element. <see langword="false"/> otherwise.</returns>
        public bool IsMouseOverElement(IntPtr messageParameter)
        {
            // This method will be invoked very often and must be as simple as possible.
            if (messageParameter == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                var mousePosScreen = new Point(Get_X_LParam(messageParameter), Get_Y_LParam(messageParameter));
                var bounds = new Rect(default, element.RenderSize);

                var mousePosRelative = element.PointFromScreen(mousePosScreen);

                return bounds.Contains(mousePosRelative);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>Provides the Get_X_LParam member.</summary>
    /// <param name="messageParameter">The message parameter value.</param>
    /// <returns>The result.</returns>
    private static int Get_X_LParam(IntPtr messageParameter) => (short)(messageParameter.ToInt32() & 0xFFFF);

    /// <summary>Provides the Get_Y_LParam member.</summary>
    /// <param name="messageParameter">The message parameter value.</param>
    /// <returns>The result.</returns>
    private static int Get_Y_LParam(IntPtr messageParameter) => (short)(messageParameter.ToInt32() >> 16);
}
