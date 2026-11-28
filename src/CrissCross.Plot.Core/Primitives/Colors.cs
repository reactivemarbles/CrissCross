// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using CrissCross.Plot.Palettes;

namespace CrissCross.Plot;

/// <summary>Provides named colors and palette helpers for plots.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1005", Justification = "Empty marker struct; equality members would change the public API.")]
public readonly struct Colors
{
    /// <summary>Contains the Category10 colors; array elements are mutable.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1499", Justification = "Array mutability is part of the existing public API.")]
    public static readonly Color[] Category10 = new Category10().Colors;

    /// <summary>Contains the Category20 colors; array elements are mutable.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1499", Justification = "Array mutability is part of the existing public API.")]
    public static readonly Color[] Category20 = new Category20().Colors;

    /// <summary>Contains the colorblind-friendly colors; array elements are mutable.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1499", Justification = "Array mutability is part of the existing public API.")]
    public static readonly Color[] ColorblindFriendly = new ColorblindFriendly().Colors;

    /// <summary>Contains the dark colorblind-friendly colors; array elements are mutable.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1499", Justification = "Array mutability is part of the existing public API.")]
    public static readonly Color[] ColorblindFriendlyDark = new ColorblindFriendlyDark().Colors;

    /// <summary>Gets the AliceBlue color (RGB 240, 248, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color AliceBlue => new(240, 248, 255);

    /// <summary>Gets the AntiqueWhite color (RGB 250, 235, 215).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color AntiqueWhite => new(250, 235, 215);

    /// <summary>Gets the Aqua color (RGB 0, 255, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Aqua => new(0, 255, 255);

    /// <summary>Gets the Aquamarine color (RGB 127, 255, 212).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Aquamarine => new(127, 255, 212);

    /// <summary>Gets the Azure color (RGB 240, 255, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Azure => new(240, 255, 255);

    /// <summary>Gets the Beige color (RGB 245, 245, 220).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Beige => new(245, 245, 220);

    /// <summary>Gets the Bisque color (RGB 255, 228, 196).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Bisque => new(255, 228, 196);

    /// <summary>Gets the Black color (RGB 0, 0, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Black => new(0, 0, 0);

    /// <summary>Gets the BlanchedAlmond color (RGB 255, 235, 205).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color BlanchedAlmond => new(255, 235, 205);

    /// <summary>Gets the Blue color (RGB 0, 0, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Blue => new(0, 0, 255);

    /// <summary>Gets the BlueViolet color (RGB 138, 43, 226).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color BlueViolet => new(138, 43, 226);

    /// <summary>Gets the Brown color (RGB 165, 42, 42).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Brown => new(165, 42, 42);

    /// <summary>Gets the BurlyWood color (RGB 222, 184, 135).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color BurlyWood => new(222, 184, 135);

    /// <summary>Gets the CadetBlue color (RGB 95, 158, 160).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color CadetBlue => new(95, 158, 160);

    /// <summary>Gets the Chartreuse color (RGB 127, 255, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Chartreuse => new(127, 255, 0);

    /// <summary>Gets the Chocolate color (RGB 210, 105, 30).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Chocolate => new(210, 105, 30);

    /// <summary>Gets the Coral color (RGB 255, 127, 80).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Coral => new(255, 127, 80);

    /// <summary>Gets the CornflowerBlue color (RGB 100, 149, 237).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color CornflowerBlue => new(100, 149, 237);

    /// <summary>Gets the Cornsilk color (RGB 255, 248, 220).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Cornsilk => new(255, 248, 220);

    /// <summary>Gets the Crimson color (RGB 220, 20, 60).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Crimson => new(220, 20, 60);

    /// <summary>Gets the Cyan color (RGB 0, 255, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Cyan => new(0, 255, 255);

    /// <summary>Gets the DarkBlue color (RGB 0, 0, 139).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkBlue => new(0, 0, 139);

    /// <summary>Gets the DarkCyan color (RGB 0, 139, 139).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkCyan => new(0, 139, 139);

    /// <summary>Gets the DarkGoldenRod color (RGB 184, 134, 11).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkGoldenRod => new(184, 134, 11);

    /// <summary>Gets the DarkGray color (RGB 169, 169, 169).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkGray => new(169, 169, 169);

    /// <summary>Gets the DarkGrey color (RGB 169, 169, 169).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkGrey => new(169, 169, 169);

    /// <summary>Gets the DarkGreen color (RGB 0, 100, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkGreen => new(0, 100, 0);

    /// <summary>Gets the DarkKhaki color (RGB 189, 183, 107).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkKhaki => new(189, 183, 107);

    /// <summary>Gets the DarkMagenta color (RGB 139, 0, 139).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkMagenta => new(139, 0, 139);

    /// <summary>Gets the DarkOliveGreen color (RGB 85, 107, 47).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkOliveGreen => new(85, 107, 47);

    /// <summary>Gets the DarkOrange color (RGB 255, 140, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkOrange => new(255, 140, 0);

    /// <summary>Gets the DarkOrchid color (RGB 153, 50, 204).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkOrchid => new(153, 50, 204);

    /// <summary>Gets the DarkRed color (RGB 139, 0, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkRed => new(139, 0, 0);

    /// <summary>Gets the DarkSalmon color (RGB 233, 150, 122).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkSalmon => new(233, 150, 122);

    /// <summary>Gets the DarkSeaGreen color (RGB 143, 188, 143).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkSeaGreen => new(143, 188, 143);

    /// <summary>Gets the DarkSlateBlue color (RGB 72, 61, 139).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkSlateBlue => new(72, 61, 139);

    /// <summary>Gets the DarkSlateGray color (RGB 47, 79, 79).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkSlateGray => new(47, 79, 79);

    /// <summary>Gets the DarkSlateGrey color (RGB 47, 79, 79).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkSlateGrey => new(47, 79, 79);

    /// <summary>Gets the DarkTurquoise color (RGB 0, 206, 209).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkTurquoise => new(0, 206, 209);

    /// <summary>Gets the DarkViolet color (RGB 148, 0, 211).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DarkViolet => new(148, 0, 211);

    /// <summary>Gets the DeepPink color (RGB 255, 20, 147).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DeepPink => new(255, 20, 147);

    /// <summary>Gets the DeepSkyBlue color (RGB 0, 191, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DeepSkyBlue => new(0, 191, 255);

    /// <summary>Gets the DimGray color (RGB 105, 105, 105).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DimGray => new(105, 105, 105);

    /// <summary>Gets the DimGrey color (RGB 105, 105, 105).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DimGrey => new(105, 105, 105);

    /// <summary>Gets the DodgerBlue color (RGB 30, 144, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color DodgerBlue => new(30, 144, 255);

    /// <summary>Gets the FireBrick color (RGB 178, 34, 34).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color FireBrick => new(178, 34, 34);

    /// <summary>Gets the FloralWhite color (RGB 255, 250, 240).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color FloralWhite => new(255, 250, 240);

    /// <summary>Gets the ForestGreen color (RGB 34, 139, 34).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color ForestGreen => new(34, 139, 34);

    /// <summary>Gets the Fuchsia color (RGB 255, 0, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Fuchsia => new(255, 0, 255);

    /// <summary>Gets the Gainsboro color (RGB 220, 220, 220).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Gainsboro => new(220, 220, 220);

    /// <summary>Gets the GhostWhite color (RGB 248, 248, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color GhostWhite => new(248, 248, 255);

    /// <summary>Gets the Gold color (RGB 255, 215, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Gold => new(255, 215, 0);

    /// <summary>Gets the GoldenRod color (RGB 218, 165, 32).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color GoldenRod => new(218, 165, 32);

    /// <summary>Gets the Gray color (RGB 128, 128, 128).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Gray => new(128, 128, 128);

    /// <summary>Gets the Grey color (RGB 128, 128, 128).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Grey => new(128, 128, 128);

    /// <summary>Gets the Green color (RGB 0, 128, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Green => new(0, 128, 0);

    /// <summary>Gets the GreenYellow color (RGB 173, 255, 47).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color GreenYellow => new(173, 255, 47);

    /// <summary>Gets the HoneyDew color (RGB 240, 255, 240).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color HoneyDew => new(240, 255, 240);

    /// <summary>Gets the HotPink color (RGB 255, 105, 180).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color HotPink => new(255, 105, 180);

    /// <summary>Gets the IndianRed color (RGB 205, 92, 92).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color IndianRed => new(205, 92, 92);

    /// <summary>Gets the Indigo color (RGB 75, 0, 130).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Indigo => new(75, 0, 130);

    /// <summary>Gets the Ivory color (RGB 255, 255, 240).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Ivory => new(255, 255, 240);

    /// <summary>Gets the Khaki color (RGB 240, 230, 140).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Khaki => new(240, 230, 140);

    /// <summary>Gets the Lavender color (RGB 230, 230, 250).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Lavender => new(230, 230, 250);

    /// <summary>Gets the LavenderBlush color (RGB 255, 240, 245).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LavenderBlush => new(255, 240, 245);

    /// <summary>Gets the LawnGreen color (RGB 124, 252, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LawnGreen => new(124, 252, 0);

    /// <summary>Gets the LemonChiffon color (RGB 255, 250, 205).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LemonChiffon => new(255, 250, 205);

    /// <summary>Gets the LightBlue color (RGB 173, 216, 230).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightBlue => new(173, 216, 230);

    /// <summary>Gets the LightCoral color (RGB 240, 128, 128).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightCoral => new(240, 128, 128);

    /// <summary>Gets the LightCyan color (RGB 224, 255, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightCyan => new(224, 255, 255);

    /// <summary>Gets the LightGoldenRodYellow color (RGB 250, 250, 210).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightGoldenRodYellow => new(250, 250, 210);

    /// <summary>Gets the LightGray color (RGB 211, 211, 211).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightGray => new(211, 211, 211);

    /// <summary>Gets the LightGrey color (RGB 211, 211, 211).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightGrey => new(211, 211, 211);

    /// <summary>Gets the LightGreen color (RGB 144, 238, 144).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightGreen => new(144, 238, 144);

    /// <summary>Gets the LightPink color (RGB 255, 182, 193).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightPink => new(255, 182, 193);

    /// <summary>Gets the LightSalmon color (RGB 255, 160, 122).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightSalmon => new(255, 160, 122);

    /// <summary>Gets the LightSeaGreen color (RGB 32, 178, 170).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightSeaGreen => new(32, 178, 170);

    /// <summary>Gets the LightSkyBlue color (RGB 135, 206, 250).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightSkyBlue => new(135, 206, 250);

    /// <summary>Gets the LightSlateGray color (RGB 119, 136, 153).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightSlateGray => new(119, 136, 153);

    /// <summary>Gets the LightSlateGrey color (RGB 119, 136, 153).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightSlateGrey => new(119, 136, 153);

    /// <summary>Gets the LightSteelBlue color (RGB 176, 196, 222).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightSteelBlue => new(176, 196, 222);

    /// <summary>Gets the LightYellow color (RGB 255, 255, 224).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LightYellow => new(255, 255, 224);

    /// <summary>Gets the Lime color (RGB 0, 255, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Lime => new(0, 255, 0);

    /// <summary>Gets the LimeGreen color (RGB 50, 205, 50).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color LimeGreen => new(50, 205, 50);

    /// <summary>Gets the Linen color (RGB 250, 240, 230).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Linen => new(250, 240, 230);

    /// <summary>Gets the Magenta color (RGB 255, 0, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Magenta => new(255, 0, 255);

    /// <summary>Gets the Maroon color (RGB 128, 0, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Maroon => new(128, 0, 0);

    /// <summary>Gets the MediumAquaMarine color (RGB 102, 205, 170).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumAquaMarine => new(102, 205, 170);

    /// <summary>Gets the MediumBlue color (RGB 0, 0, 205).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumBlue => new(0, 0, 205);

    /// <summary>Gets the MediumOrchid color (RGB 186, 85, 211).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumOrchid => new(186, 85, 211);

    /// <summary>Gets the MediumPurple color (RGB 147, 112, 219).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumPurple => new(147, 112, 219);

    /// <summary>Gets the MediumSeaGreen color (RGB 60, 179, 113).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumSeaGreen => new(60, 179, 113);

    /// <summary>Gets the MediumSlateBlue color (RGB 123, 104, 238).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumSlateBlue => new(123, 104, 238);

    /// <summary>Gets the MediumSpringGreen color (RGB 0, 250, 154).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumSpringGreen => new(0, 250, 154);

    /// <summary>Gets the MediumTurquoise color (RGB 72, 209, 204).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumTurquoise => new(72, 209, 204);

    /// <summary>Gets the MediumVioletRed color (RGB 199, 21, 133).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MediumVioletRed => new(199, 21, 133);

    /// <summary>Gets the MidnightBlue color (RGB 25, 25, 112).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MidnightBlue => new(25, 25, 112);

    /// <summary>Gets the MintCream color (RGB 245, 255, 250).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MintCream => new(245, 255, 250);

    /// <summary>Gets the MistyRose color (RGB 255, 228, 225).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color MistyRose => new(255, 228, 225);

    /// <summary>Gets the Moccasin color (RGB 255, 228, 181).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Moccasin => new(255, 228, 181);

    /// <summary>Gets the NavajoWhite color (RGB 255, 222, 173).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color NavajoWhite => new(255, 222, 173);

    /// <summary>Gets the Navy color (RGB 0, 0, 128).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Navy => new(0, 0, 128);

    /// <summary>Gets the OldLace color (RGB 253, 245, 230).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color OldLace => new(253, 245, 230);

    /// <summary>Gets the Olive color (RGB 128, 128, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Olive => new(128, 128, 0);

    /// <summary>Gets the OliveDrab color (RGB 107, 142, 35).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color OliveDrab => new(107, 142, 35);

    /// <summary>Gets the Orange color (RGB 255, 165, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Orange => new(255, 165, 0);

    /// <summary>Gets the OrangeRed color (RGB 255, 69, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color OrangeRed => new(255, 69, 0);

    /// <summary>Gets the Orchid color (RGB 218, 112, 214).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Orchid => new(218, 112, 214);

    /// <summary>Gets the PaleGoldenRod color (RGB 238, 232, 170).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color PaleGoldenRod => new(238, 232, 170);

    /// <summary>Gets the PaleGreen color (RGB 152, 251, 152).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color PaleGreen => new(152, 251, 152);

    /// <summary>Gets the PaleTurquoise color (RGB 175, 238, 238).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color PaleTurquoise => new(175, 238, 238);

    /// <summary>Gets the PaleVioletRed color (RGB 219, 112, 147).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color PaleVioletRed => new(219, 112, 147);

    /// <summary>Gets the PapayaWhip color (RGB 255, 239, 213).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color PapayaWhip => new(255, 239, 213);

    /// <summary>Gets the PeachPuff color (RGB 255, 218, 185).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color PeachPuff => new(255, 218, 185);

    /// <summary>Gets the Peru color (RGB 205, 133, 63).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Peru => new(205, 133, 63);

    /// <summary>Gets the Pink color (RGB 255, 192, 203).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Pink => new(255, 192, 203);

    /// <summary>Gets the Plum color (RGB 221, 160, 221).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Plum => new(221, 160, 221);

    /// <summary>Gets the PowderBlue color (RGB 176, 224, 230).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color PowderBlue => new(176, 224, 230);

    /// <summary>Gets the Purple color (RGB 128, 0, 128).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Purple => new(128, 0, 128);

    /// <summary>Gets the RebeccaPurple color (RGB 102, 51, 153).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color RebeccaPurple => new(102, 51, 153);

    /// <summary>Gets the Red color (RGB 255, 0, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Red => new(255, 0, 0);

    /// <summary>Gets the RosyBrown color (RGB 188, 143, 143).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color RosyBrown => new(188, 143, 143);

    /// <summary>Gets the RoyalBlue color (RGB 65, 105, 225).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color RoyalBlue => new(65, 105, 225);

    /// <summary>Gets the SaddleBrown color (RGB 139, 69, 19).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SaddleBrown => new(139, 69, 19);

    /// <summary>Gets the Salmon color (RGB 250, 128, 114).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Salmon => new(250, 128, 114);

    /// <summary>Gets the SandyBrown color (RGB 244, 164, 96).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SandyBrown => new(244, 164, 96);

    /// <summary>Gets the SeaGreen color (RGB 46, 139, 87).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SeaGreen => new(46, 139, 87);

    /// <summary>Gets the SeaShell color (RGB 255, 245, 238).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SeaShell => new(255, 245, 238);

    /// <summary>Gets the Sienna color (RGB 160, 82, 45).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Sienna => new(160, 82, 45);

    /// <summary>Gets the Silver color (RGB 192, 192, 192).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Silver => new(192, 192, 192);

    /// <summary>Gets the SkyBlue color (RGB 135, 206, 235).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SkyBlue => new(135, 206, 235);

    /// <summary>Gets the SlateBlue color (RGB 106, 90, 205).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SlateBlue => new(106, 90, 205);

    /// <summary>Gets the SlateGray color (RGB 112, 128, 144).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SlateGray => new(112, 128, 144);

    /// <summary>Gets the SlateGrey color (RGB 112, 128, 144).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SlateGrey => new(112, 128, 144);

    /// <summary>Gets the Snow color (RGB 255, 250, 250).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Snow => new(255, 250, 250);

    /// <summary>Gets the SpringGreen color (RGB 0, 255, 127).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SpringGreen => new(0, 255, 127);

    /// <summary>Gets the SteelBlue color (RGB 70, 130, 180).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color SteelBlue => new(70, 130, 180);

    /// <summary>Gets the Tan color (RGB 210, 180, 140).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Tan => new(210, 180, 140);

    /// <summary>Gets the Teal color (RGB 0, 128, 128).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Teal => new(0, 128, 128);

    /// <summary>Gets the Thistle color (RGB 216, 191, 216).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Thistle => new(216, 191, 216);

    /// <summary>Gets the Tomato color (RGB 255, 99, 71).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Tomato => new(255, 99, 71);

    /// <summary>Gets black with zero alpha.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Transparent => Black.WithAlpha(0);

    /// <summary>Gets the Turquoise color (RGB 64, 224, 208).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Turquoise => new(64, 224, 208);

    /// <summary>Gets the Violet color (RGB 238, 130, 238).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Violet => new(238, 130, 238);

    /// <summary>Gets the Wheat color (RGB 245, 222, 179).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Wheat => new(245, 222, 179);

    /// <summary>Gets the White color (RGB 255, 255, 255).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color White => new(255, 255, 255);

    /// <summary>Gets the WhiteSmoke color (RGB 245, 245, 245).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color WhiteSmoke => new(245, 245, 245);

    /// <summary>Gets the Yellow color (RGB 255, 255, 0).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color Yellow => new(255, 255, 0);

    /// <summary>Gets the YellowGreen color (RGB 154, 205, 50).</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST1471", Justification = "Literal RGB values preserve the authoritative public color data.")]
    public static Color YellowGreen => new(154, 205, 50);

    /// <summary>Gets the Category10 color at index 0 (#1f77b4).</summary>
    public static Color C0 => Color.FromHex("#1f77b4");

    /// <summary>Gets the Category10 color at index 1 (#ff7f0e).</summary>
    public static Color C1 => Color.FromHex("#ff7f0e");

    /// <summary>Gets the Category10 color at index 2 (#2ca02c).</summary>
    public static Color C2 => Color.FromHex("#2ca02c");

    /// <summary>Gets the Category10 color at index 3 (#d62728).</summary>
    public static Color C3 => Color.FromHex("#d62728");

    /// <summary>Gets the Category10 color at index 4 (#9467bd).</summary>
    public static Color C4 => Color.FromHex("#9467bd");

    /// <summary>Gets the Category10 color at index 5 (#8c564b).</summary>
    public static Color C5 => Color.FromHex("#8c564b");

    /// <summary>Gets the Category10 color at index 6 (#e377c2).</summary>
    public static Color C6 => Color.FromHex("#e377c2");

    /// <summary>Gets the Category10 color at index 7 (#7f7f7f).</summary>
    public static Color C7 => Color.FromHex("#7f7f7f");

    /// <summary>Gets the Category10 color at index 8 (#bcbd22).</summary>
    public static Color C8 => Color.FromHex("#bcbd22");

    /// <summary>Gets the Category10 color at index 9 (#17becf).</summary>
    public static Color C9 => Color.FromHex("#17becf");

    /// <summary>Gets the CrissCross.Plot purple color (#67217a).</summary>
    public static Color ScottPlotPurple => Color.FromHex("#67217a");

    /// <summary>Gets the CrissCross.Plot pink color (#9a4993).</summary>
    public static Color ScottPlotPink => Color.FromHex("#9a4993");

    /// <summary>Returns colors with independently generated hues; near-matches may occur.</summary>
    /// <param name="count">Number of colors to generate; must be non-negative.</param>
    /// <returns>An empty array when <paramref name="count"/> is zero.</returns>
    public static Color[] RandomHue(int count)
    {
        Color[] colors = new Color[count];

        for (int i = 0; i < count; i++)
        {
            colors[i] = Color.RandomHue();
        }

        return colors;
    }

    /// <summary>Returns maximum-saturation colors with evenly spaced hues.</summary>
    /// <param name="count">Number of colors to generate; must be non-negative.</param>
    /// <returns>An empty sequence when <paramref name="count"/> is zero.</returns>
    public static IEnumerable<Color> Rainbow(int count)
    {
        Color[] colors = new Color[count];

        for (int i = 0; i < count; i++)
        {
            float hue = (float)i / count;
            const float saturation = 1;
            const float luminosity = 0.5F;
            colors[i] = Color.FromHSL(hue, saturation, luminosity);
        }

        return colors;
    }

    /// <summary>Returns the named colors exposed by this type and its standard and xkcd collections.</summary>
    /// <returns>A new list of fully qualified color names and their values.</returns>
    public static List<(string Name, Color Value)> GetNamedColors()
    {
        List<(string Name, Color Value)> colors = [];
        AddNamedColors(colors, typeof(Colors).GetProperties(), "Colors.");
        AddNamedColors(colors, typeof(NamedColors.WindowsStandardColors).GetProperties(), "Colors.Windows.");
        AddNamedColors(colors, typeof(NamedColors.XkcdColors).GetProperties(), "Colors.Xkcd.");
        return colors;
    }

    /// <summary>Adds named colors from a public color type.</summary>
    /// <param name="colors">The list to populate.</param>
    /// <param name="properties">The public properties to examine.</param>
    /// <param name="prefix">The name prefix to apply.</param>
    private static void AddNamedColors(
        List<(string Name, Color Value)> colors,
        PropertyInfo[] properties,
        string prefix)
    {
        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(Color))
            {
                colors.Add(($"{prefix}{property.Name}", (Color)property.GetValue(null)!));
            }
        }
    }

    /// <summary>Default Windows Colors.</summary>
    public class Windows : NamedColors.WindowsStandardColors;

    /// <summary>Colors from the xkcd color name survey https://xkcd.com/color/rgb/.</summary>
    public class Xkcd : NamedColors.XkcdColors;
}
