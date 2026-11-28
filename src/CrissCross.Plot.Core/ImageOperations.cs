// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides helpers for embedding images in HTML.</summary>
public static class ImageOperations
{
    /// <summary>Creates an HTML image element with the supplied image data.</summary>
    /// <param name="bytes">The image data.</param>
    /// <param name="imageType">The image format, such as png.</param>
    /// <param name="classContent">The CSS classes applied to the image.</param>
    /// <param name="styleContent">The inline CSS applied to the image.</param>
    /// <returns>An HTML image element containing the image data.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Optional defaults are retained for source compatibility.")]
    public static string GetImageHtml(byte[] bytes, string imageType = "png", string classContent = "", string styleContent = "")
    {
        string b64 = Convert.ToBase64String(bytes);
        return $"<img class='{classContent}' style='{styleContent}' src=\"data:image/{imageType};base64,{b64}\"></img>";
    }
}
