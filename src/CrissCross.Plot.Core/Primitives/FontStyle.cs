// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides font family, weight, size, and rendering settings for styled text.</summary>
public class FontStyle
{
    /// <summary>Gets the typeface matching this font style.</summary>
    public SKTypeface Typeface => Fonts.GetTypeface(Name, Weight, Slant, Width);

    /// <summary>Gets or sets the font family name.</summary>
    public string Name { get; set; } = Fonts.Default;

    /// <summary>Gets or sets the font weight.</summary>
    public FontWeight Weight { get; set; } = Fonts.DefaultWeight ?? FontWeight.Normal;

    /// <summary>Gets or sets the font slant.</summary>
    public FontSlant Slant { get; set; } = Fonts.DefaultSlant ?? FontSlant.Upright;

    /// <summary>Gets or sets the font width class.</summary>
    public FontSpacing Width { get; set; } = Fonts.DefaultWidth ?? FontSpacing.Normal;

    /// <summary>Gets or sets whether the font is bold.</summary>
    public bool Bold
    {
        get => Weight == FontWeight.Bold;
        set => Weight = value ? FontWeight.Bold : FontWeight.Normal;
    }

    /// <summary>Gets or sets whether the font is italic.</summary>
    public bool Italic
    {
        get => Slant == FontSlant.Italic;
        set => Slant = value ? FontSlant.Italic : FontSlant.Upright;
    }

    /// <summary>Gets or sets the text color.</summary>
    public Color Color { get; set; } = Colors.Black;

    /// <summary>Gets or sets the font size.</summary>
    public float Size { get; set; } = 12;

    /// <summary>Gets or sets whether antialiasing is enabled.</summary>
    public bool AntiAlias { get; set; } = true;

    /// <summary>Formats this font style as text.</summary>
    /// <returns>A textual representation of the font style.</returns>
    public override string ToString() => $"{Name}, Size {Size}, {Color}";

    /// <summary>Uses the specified text to select an installed font likely to support its characters.</summary>
    /// <param name="text">The text used to select a suitable font.</param>
    public void SetBestFont(string text) => Name = Fonts.Detect(text);

    /// <summary>Creates a copy of this font style.</summary>
    /// <returns>A copy with the same font and rendering settings.</returns>
    public FontStyle Clone() => new() { Name = Name, Bold = Bold, Italic = Italic, Color = Color, Size = Size, AntiAlias = AntiAlias, Weight = Weight, Slant = Slant, Width = Width, };

    /// <summary>Applies this font style to a paint.</summary>
    /// <param name="paint">The paint to configure.</param>
    public void ApplyToPaint(Paint paint)
    {
        paint.SKShader = null;
        paint.IsStroke = false;
        paint.SKTypeface = Typeface;
        paint.TextSize = Size;
        paint.Color = Color;
        paint.IsAntialias = AntiAlias;
        paint.Bold = Bold;
    }
}
