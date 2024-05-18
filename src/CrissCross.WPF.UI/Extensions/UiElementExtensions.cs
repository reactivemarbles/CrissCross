// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

internal static class UiElementExtensions
{
    /// <summary>
    /// Do not call it outside of NCHITTEST, NCLBUTTONUP, NCLBUTTONDOWN messages.
    /// </summary>
    /// <returns><see langword="true"/> if mouse is over the element. <see langword="false"/> otherwise.</returns>
    public static bool IsMouseOverElement(this UIElement element, IntPtr lParam)
    {
        // This method will be invoked very often and must be as simple as possible.
        if (lParam == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            var mousePosScreen = new Point(Get_X_LParam(lParam), Get_Y_LParam(lParam));
            var bounds = new Rect(default, element.RenderSize);

            var mousePosRelative = element.PointFromScreen(mousePosScreen);

            return bounds.Contains(mousePosRelative);
        }
        catch
        {
            return false;
        }
    }

    private static int Get_X_LParam(IntPtr lParam) => (short)(lParam.ToInt32() & 0xFFFF);

    private static int Get_Y_LParam(IntPtr lParam) => (short)(lParam.ToInt32() >> 16);
}
