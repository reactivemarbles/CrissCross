// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.NamedColors;

/// <summary>Windows Vista system colors.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Style",
    "SST1471",
    Justification = "RGB channel values are the authoritative public color data; named constants would obscure this color table and risk changing exact values.")]
public class WindowsVistaColors : NamedColorsBase
{
    /// <summary>Gets the ActiveBorder color.</summary>
    public static Color ActiveBorder => new(180, 180, 180);

    /// <summary>Gets the ActiveCaption color.</summary>
    public static Color ActiveCaption => new(153, 180, 209);

    /// <summary>Gets the AppWorkspace color.</summary>
    public static Color AppWorkspace => new(171, 171, 171);

    /// <summary>Gets the Background color.</summary>
    public static Color Background => new(000, 000, 000);

    /// <summary>Gets the ButtonFace color.</summary>
    public static Color ButtonFace => new(240, 240, 240);

    /// <summary>Gets the ButtonHighlight color.</summary>
    public static Color ButtonHighlight => new(255, 255, 255);

    /// <summary>Gets the ButtonShadow color.</summary>
    public static Color ButtonShadow => new(160, 160, 160);

    /// <summary>Gets the ButtonText color.</summary>
    public static Color ButtonText => new(000, 000, 000);

    /// <summary>Gets the CaptionText color.</summary>
    public static Color CaptionText => new(000, 000, 000);

    /// <summary>Gets the GrayText color.</summary>
    public static Color GrayText => new(128, 128, 128);

    /// <summary>Gets the Highlight color.</summary>
    public static Color Highlight => new(51, 153, 255);

    /// <summary>Gets the HighlightText color.</summary>
    public static Color HighlightText => new(255, 255, 255);

    /// <summary>Gets the InactiveBorder color.</summary>
    public static Color InactiveBorder => new(244, 247, 252);

    /// <summary>Gets the InactiveCaption color.</summary>
    public static Color InactiveCaption => new(191, 205, 219);

    /// <summary>Gets the InactiveCaptionText color.</summary>
    public static Color InactiveCaptionText => new(67, 78, 84);

    /// <summary>Gets the InfoBackground color.</summary>
    public static Color InfoBackground => new(255, 255, 225);

    /// <summary>Gets the InfoText color.</summary>
    public static Color InfoText => new(000, 000, 000);

    /// <summary>Gets the Menu color.</summary>
    public static Color Menu => new(240, 240, 240);

    /// <summary>Gets the MenuText color.</summary>
    public static Color MenuText => new(000, 000, 000);

    /// <summary>Gets the Scrollbar color.</summary>
    public static Color Scrollbar => new(200, 200, 200);

    /// <summary>Gets the ThreeDDarkShadow color.</summary>
    public static Color ThreeDDarkShadow => new(105, 105, 105);

    /// <summary>Gets the ThreeDFace color.</summary>
    public static Color ThreeDFace => new(240, 240, 240);

    /// <summary>Gets the ThreeDHighlight color.</summary>
    public static Color ThreeDHighlight => new(255, 255, 255);

    /// <summary>Gets the ThreeDLightShadow color.</summary>
    public static Color ThreeDLightShadow => new(227, 227, 227);

    /// <summary>Gets the ThreeDShadow color.</summary>
    public static Color ThreeDShadow => new(160, 160, 160);

    /// <summary>Gets the Window color.</summary>
    public static Color Window => new(255, 255, 255);

    /// <summary>Gets the WindowFrame color.</summary>
    public static Color WindowFrame => new(100, 100, 100);

    /// <summary>Gets the WindowText color.</summary>
    public static Color WindowText => new(000, 000, 000);
}
