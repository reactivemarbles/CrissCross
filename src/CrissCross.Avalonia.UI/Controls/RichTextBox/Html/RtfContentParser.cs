// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Globalization;
using System.Net;
using System.Text;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reads scoped RTF groups into the editor's HTML representation.</summary>
/// <param name = "source">The source input.</param>
internal sealed class RtfContentParser(string source)
{
    /// <summary>Stores the MaximumGroupDepth conversion state.</summary>
    private const int MaximumGroupDepth = 128;

    /// <summary>Stores the HexByteLength conversion state.</summary>
    private const int HexByteLength = 2;

    /// <summary>Stores the WindowsCodePage conversion state.</summary>
    private const int WindowsCodePage = 1252;

    /// <summary>Stores the AnsiExtensionStart conversion state.</summary>
    private const int AnsiExtensionStart = 128;

    /// <summary>Stores the AnsiExtensionEnd conversion state.</summary>
    private const int AnsiExtensionEnd = 160;

    /// <summary>Stores the DefaultFontSize conversion state.</summary>
    private const int DefaultFontSize = 24;

    /// <summary>Stores the HalfPointsPerPoint conversion state.</summary>
    private const double HalfPointsPerPoint = 2D;

    /// <summary>Stores the TwipsPerPixel conversion state.</summary>
    private const double TwipsPerPixel = 15D;

    /// <summary>Stores the HyperlinkInstruction conversion state.</summary>
    private const string HyperlinkInstruction = "HYPERLINK ";

    /// <summary>Stores the MetadataDestinations conversion state.</summary>
    private static readonly HashSet<string> MetadataDestinations = ["fonttbl", "colortbl", "stylesheet", "info", "listtable", "listoverridetable", "generator", "nonshppict"];

    /// <summary>Stores the UnsupportedDestinations conversion state.</summary>
    private static readonly HashSet<string> UnsupportedDestinations = ["object", "shp", "footnote", "header", "footer", "headerl", "headerr", "footerl", "footerr", "annotation"];

    /// <summary>Stores the SpecialCharacters conversion state.</summary>
    private static readonly Dictionary<string, string> SpecialCharacters = new(StringComparer.Ordinal)
    {
        ["tab"] = "\t",
        ["~"] = "\u00a0",
        ["_"] = "\u2011",
        ["-"] = "\u00ad",
        ["emdash"] = "\u2014",
        ["endash"] = "\u2013",
        ["bullet"] = "\u2022",
        ["lquote"] = "\u2018",
        ["rquote"] = "\u2019",
        ["ldblquote"] = "\u201c",
        ["rdblquote"] = "\u201d",
    };

    /// <summary>Stores the fonts conversion state.</summary>
    private readonly Dictionary<int, string> _fonts = [];

    /// <summary>Stores the colors conversion state.</summary>
    private readonly List<string> _colors = [];

    /// <summary>Stores the output conversion state.</summary>
    private readonly StringBuilder _output = new();

    /// <summary>Stores the paragraph conversion state.</summary>
    private readonly StringBuilder _paragraph = new();

    /// <summary>Buffers native table structures and cell definitions.</summary>
    private readonly RtfTableReader _tables = new();

    /// <summary>Stores the offset conversion state.</summary>
    private int _offset;

    /// <summary>Stores the fallback conversion state.</summary>
    private int _fallback;

    /// <summary>Stores the defaultFont conversion state.</summary>
    private int _defaultFont;

    /// <summary>Tracks Word's trailing nested table definition destination.</summary>
    private bool _nestedProperties;

    /// <summary>Tracks whether the current paragraph belongs to a table.</summary>
    private bool _tableParagraph;

    /// <summary>The nesting level inherited by the current paragraph.</summary>
    private int _paragraphDepth = 1;

    /// <summary>Stores the alignment conversion state.</summary>
    private string _alignment = "left";

    /// <summary>The pending high surrogate from a Unicode control.</summary>
    private char? _highSurrogate;

    /// <summary>The formatting attached to a pending surrogate pair.</summary>
    private Formatting _surrogateFormatting = new();

    /// <summary>Gets the current cell or document output buffer.</summary>
    private StringBuilder Output => _tables.Output(_output);

    /// <summary>Returns HTML after validating and decoding the complete RTF document.</summary>
    /// <returns>The conversion result.</returns>
    internal string Read()
    {
        if (!source.StartsWith("{\\rtf", StringComparison.Ordinal))
        {
            throw new FormatException("The document is not an RTF document.");
        }

        var root = ReadGroup(0);
        if (_offset != source.Length && !string.IsNullOrWhiteSpace(source[_offset..]))
        {
            throw new FormatException("Unexpected content follows the RTF document.");
        }

        ReadTables(root);
        _tables.Colors.AddRange(_colors);
        Render(root, new Formatting { Font = _defaultFont });
        if (_highSurrogate is not null)
        {
            throw new FormatException("The RTF document ends with an incomplete Unicode surrogate pair.");
        }

        FlushFinalContent();
        CloseTable();
        return _output.ToString();
    }

    /// <summary>Updates character decoration without mutating the parent group.</summary>
    /// <param name = "token">The token input.</param>
    /// <param name = "formatting">The formatting input.</param>
    /// <returns>The conversion result.</returns>
    private static Formatting ApplyDecorations(Token token, Formatting formatting)
    {
        var enabled = token.Number != 0;
        return token.Word switch
        {
            "b" => formatting with
            {
                Bold = enabled
            },
            "i" => formatting with
            {
                Italic = enabled
            },
            "ul" => formatting with
            {
                Underline = enabled
            },
            "ulnone" => formatting with
            {
                Underline = false
            },
            "strike" => formatting with
            {
                Strike = enabled
            },
            "super" or "sub" => formatting with
            {
                Baseline = token.Word
            },
            "nosupersub" => formatting with
            {
                Baseline = string.Empty
            },
            _ => formatting,
        };
    }

    /// <summary>Maps a Windows-1252 byte to its Unicode character.</summary>
    /// <param name = "value">The value input.</param>
    /// <returns>The conversion result.</returns>
    private static char DecodeAnsi(byte value)
    {
        const string Windows1252 = "\u20ac\u0081\u201a\u0192\u201e\u2026\u2020\u2021\u02c6\u2030\u0160\u2039\u0152\u008d\u017d\u008f"
            + "\u0090\u2018\u2019\u201c\u201d\u2022\u2013\u2014\u02dc\u2122\u0161\u203a\u0153\u009d\u017e\u0178";
        return value is >= AnsiExtensionStart and < AnsiExtensionEnd ? Windows1252[value - AnsiExtensionStart] : (char)value;
    }

    /// <summary>Returns the first destination control, skipping the ignorable marker.</summary>
    /// <param name = "tokens">The tokens input.</param>
    /// <returns>The conversion result.</returns>
    private static string Destination(List<Token> tokens) => tokens.Find(static token => token.Word is not (null or "*"))?.Word ?? string.Empty;

    /// <summary>Decodes Unicode and fallback characters in destination text.</summary>
    /// <param name = "tokens">The tokens input.</param>
    /// <returns>The conversion result.</returns>
    private static string ReadDestinationText(List<Token> tokens)
    {
        var text = new StringBuilder();
        var fallback = 0;
        var uc = 1;
        foreach (var token in tokens)
        {
            if (token.Children is { } children)
            {
                _ = text.Append(ReadNestedDestinationText(children));
                continue;
            }

            if (token.Word == "uc")
            {
                uc = token.Number ?? 1;
            }
            else if (token.Word == "u")
            {
                _ = text.Append((char)(token.Number ?? 0));
                fallback = uc;
            }
            else if (token.Text is { } part)
            {
                var skip = Math.Min(fallback, part.Length);
                fallback -= skip;
                _ = text.Append(part.AsSpan(skip));
            }
        }

        return text.ToString();
    }

    /// <summary>Reads visible nested destination text while excluding ignorable metadata.</summary>
    /// <param name="tokens">The nested destination tokens.</param>
    /// <returns>The decoded text or an empty ignored destination.</returns>
    private static string ReadNestedDestinationText(List<Token> tokens) =>
        tokens.Count > 0 && tokens[0].Word == "*" ? string.Empty : ReadDestinationText(tokens);

    /// <summary>Adds inline character decoration to the generated CSS.</summary>
    /// <param name = "style">The style input.</param>
    /// <param name = "formatting">The formatting input.</param>
    private static void AppendDecorations(StringBuilder style, Formatting formatting)
    {
        if (formatting.Bold)
        {
            _ = style.Append("font-weight:bold;");
        }

        if (formatting.Italic)
        {
            _ = style.Append("font-style:italic;");
        }

        if (formatting.Underline || formatting.Strike)
        {
            _ = style.Append("text-decoration:").Append(formatting.Underline ? "underline " : string.Empty).Append(formatting.Strike ? "line-through" : string.Empty).Append(';');
        }

        if (formatting.Baseline.Length == 0)
        {
            return;
        }

        _ = style.Append("vertical-align:").Append(formatting.Baseline).Append(';');
    }

    /// <summary>Combines binary or hexadecimal picture bytes.</summary>
    /// <param name = "tokens">The tokens input.</param>
    /// <returns>The conversion result.</returns>
    private static List<byte> ReadPictureData(List<Token> tokens)
    {
        var data = new List<byte>();
        var hex = new StringBuilder();
        foreach (var token in tokens)
        {
            AppendPictureData(token, data, hex);
        }

        if (hex.Length == 0)
        {
            return data;
        }

        try
        {
            data.AddRange(Convert.FromHexString(hex.ToString()));
        }
        catch (FormatException exception)
        {
            throw new FormatException("The RTF picture hexadecimal payload is invalid.", exception);
        }

        return data;
    }

    /// <summary>Collects picture bytes and non-whitespace hexadecimal digits.</summary>
    /// <param name = "token">The token input.</param>
    /// <param name = "data">The data input.</param>
    /// <param name = "hex">The hex input.</param>
    private static void AppendPictureData(Token token, List<byte> data, StringBuilder hex)
    {
        if (token.Text is { } text)
        {
            foreach (var c in text)
            {
                if (!char.IsWhiteSpace(c))
                    {
                    _ = hex.Append(c);
                }
            }
        }

        if (token.Binary is not { } binary)
        {
            return;
        }

        foreach (var c in binary)
        {
            data.Add((byte)c);
        }
    }

    /// <summary>Reads one balanced group, rejecting truncated or deeply nested input.</summary>
    /// <param name = "depth">The depth input.</param>
    /// <returns>The conversion result.</returns>
    private List<Token> ReadGroup(int depth)
    {
        if (depth > MaximumGroupDepth)
        {
            throw new FormatException("RTF group nesting exceeds the supported depth.");
        }

        if (_offset >= source.Length || source[_offset] != '{')
        {
            throw new FormatException("An RTF group was expected.");
        }

        _offset++;
        var tokens = new List<Token>();
        while (_offset < source.Length)
        {
            var c = source[_offset];
            if (c == '}')
            {
                _offset++;
                return tokens;
            }

            tokens.Add(ReadToken(depth));
        }

        throw new FormatException("An RTF group is not closed.");
    }

    /// <summary>Reads the next group, control, escape, or text run.</summary>
    /// <param name = "depth">The depth input.</param>
    /// <returns>The conversion result.</returns>
    private Token ReadToken(int depth)
    {
        var c = source[_offset];
        if (c == '{')
        {
            return new Token { Children = ReadGroup(depth + 1) };
        }

        if (c != '\\')
        {
            return ReadText();
        }

        _offset++;
        if (_offset == source.Length)
        {
            throw new FormatException("An RTF escape is incomplete.");
        }

        c = source[_offset];
        _offset++;
        return char.IsAsciiLetter(c) ? ReadControlWord() : ReadSymbol(c);
    }

    /// <summary>Reads literal text while ignoring RTF source line endings.</summary>
    /// <returns>The conversion result.</returns>
    private Token ReadText()
    {
        var start = _offset;
        _offset++;
        while (_offset < source.Length && source[_offset] is not ('{' or '}' or '\\'))
        {
            _offset++;
        }

        var text = source[start.._offset].Replace("\r", string.Empty, StringComparison.Ordinal).Replace("\n", string.Empty, StringComparison.Ordinal);
        return new Token { Text = text };
    }

    /// <summary>Decodes an escaped literal, hexadecimal byte, or control symbol.</summary>
    /// <param name = "c">The c input.</param>
    /// <returns>The conversion result.</returns>
    private Token ReadSymbol(char c)
    {
        if (c != '\'')
        {
            return c is '\\' or '{' or '}'
                ? new Token { Text = c.ToString() }
                : new Token { Word = c.ToString() };
        }

        if (_offset + HexByteLength > source.Length || !byte.TryParse(source.AsSpan(_offset, HexByteLength), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
        {
            throw new FormatException("An RTF hexadecimal escape is invalid.");
        }

        _offset += HexByteLength;
        return new Token { Text = DecodeAnsi(value).ToString() };
    }

    /// <summary>Reads a control parameter or an exact-length binary payload.</summary>
    /// <returns>The conversion result.</returns>
    private Token ReadControlWord()
    {
        var start = _offset - 1;
        while (_offset < source.Length && char.IsAsciiLetter(source[_offset]))
        {
            _offset++;
        }

        var word = source[start.._offset];
        var number = ReadNumber();
        if (_offset < source.Length && source[_offset] == ' ')
        {
            _offset++;
        }

        if (word != "bin")
        {
            return new Token { Word = word, Number = number };
        }

        var length = number ?? -1;
        if (length < 0 || length > source.Length - _offset)
        {
            throw new FormatException("An RTF binary payload is truncated.");
        }

        var binary = source.Substring(_offset, length);
        _offset += length;
        return new Token { Binary = binary };
    }

    /// <summary>Reads an optional signed control parameter.</summary>
    /// <returns>The conversion result.</returns>
    private int? ReadNumber()
    {
        var start = _offset;
        if (_offset < source.Length && source[_offset] == '-')
        {
            _offset++;
        }

        while (_offset < source.Length && char.IsAsciiDigit(source[_offset]))
        {
            _offset++;
        }

        if (_offset == start)
        {
            return null;
        }

        if (!int.TryParse(source.AsSpan(start, _offset - start), NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
        {
            throw new FormatException("An RTF numeric parameter is invalid.");
        }

        return number;
    }

    /// <summary>Loads document defaults, font names, and color definitions.</summary>
    /// <param name = "tokens">The tokens input.</param>
    private void ReadTables(List<Token> tokens)
    {
        foreach (var token in tokens)
        {
            CheckHeader(token);
            if (token.Children is not { } children)
            {
                continue;
            }

            var destination = Destination(children);
            if (destination == "fonttbl")
            {
                ReadFonts(children);
            }

            if (destination == "colortbl")
            {
                ReadColors(children);
            }
        }
    }

    /// <summary>Reads the default font and rejects unsupported byte encodings.</summary>
    /// <param name = "token">The token input.</param>
    private void CheckHeader(Token token)
    {
        if (token.Word == "deff")
        {
            _defaultFont = token.Number ?? 0;
        }

        if (!(token.Word == "ansicpg" && token.Number is not (null or WindowsCodePage)))
        {
            return;
        }

        throw new NotSupportedException("Only Windows-1252 RTF byte escapes are supported; use Unicode RTF for other scripts.");
    }

    /// <summary>Indexes font names from the RTF font table.</summary>
    /// <param name = "tokens">The tokens input.</param>
    private void ReadFonts(List<Token> tokens)
    {
        foreach (var token in tokens)
        {
            if (token.Children is not { } fontTokens)
            {
                continue;
            }

            var id = fontTokens.Find(static t => t.Word == "f")?.Number ?? 0;
            _fonts[id] = ReadDestinationText(fontTokens).Trim().TrimEnd(';');
        }
    }

    /// <summary>Reads automatic and explicit colors in table order.</summary>
    /// <param name = "tokens">The tokens input.</param>
    private void ReadColors(List<Token> tokens)
    {
        var color = new ColorComponents();
        foreach (var token in tokens)
        {
            color.Set(token);
            if (token.Text is not { } text)
            {
                continue;
            }

            foreach (var c in text)
            {
                if (c != ';')
                    {
                    continue;
                }

                _colors.Add(color.ToHtml());
                color = new();
            }
        }
    }

    /// <summary>Renders a scoped group while keeping parent character formatting intact.</summary>
    /// <param name = "tokens">The tokens input.</param>
    /// <param name = "formatting">The formatting input.</param>
    private void Render(List<Token> tokens, Formatting formatting)
    {
        if (RenderDestination(tokens, formatting))
        {
            return;
        }

        foreach (var token in tokens)
        {
            if (token.Children is { } children)
            {
                var paragraphDepth = _paragraphDepth;
                var tableParagraph = _tableParagraph;
                Render(children, formatting);
                _paragraphDepth = paragraphDepth;
                _tableParagraph = tableParagraph;
                continue;
            }

            if (token.Text is { } text)
            {
                var skip = Math.Min(_fallback, text.Length);
                _fallback -= skip;
                AppendText(text[skip..], formatting);
                continue;
            }

            if (_fallback > 0 && token.Word is "~" or "_" or "-")
            {
                _fallback--;
                continue;
            }

            formatting = ApplyFormatting(token, formatting);
            RenderControl(token, formatting);
        }
    }

    /// <summary>Handles special destinations and skips ignorable metadata.</summary>
    /// <param name = "tokens">The tokens input.</param>
    /// <param name = "formatting">The formatting input.</param>
    /// <returns>The conversion result.</returns>
    private bool RenderDestination(List<Token> tokens, Formatting formatting)
    {
        var destination = Destination(tokens);
        if (destination == "nonesttables")
        {
            return true;
        }

        if (destination == "nesttableprops")
        {
            _nestedProperties = true;
        }

        if (MetadataDestinations.Contains(destination))
        {
            return true;
        }

        if (UnsupportedDestinations.Contains(destination))
        {
            throw new NotSupportedException($"RTF destination '{destination}' cannot be represented by this editor.");
        }

        if (destination == "pict")
        {
            AppendPicture(tokens);
            return true;
        }

        if (destination == "field")
        {
            AppendField(tokens, formatting);
            return true;
        }

        return tokens.Count > 0 && tokens[0].Word == "*" && destination is not ("shppict" or "nesttableprops");
    }

    /// <summary>Preserves a hyperlink target and its formatted display text.</summary>
    /// <param name = "tokens">The tokens input.</param>
    /// <param name = "formatting">The formatting input.</param>
    private void AppendField(List<Token> tokens, Formatting formatting)
    {
        var instruction = tokens.Find(static t => t.Children is { } c && Destination(c) == "fldinst")?.Children;
        var result = tokens.Find(static t => t.Children is { } c && Destination(c) == "fldrslt")?.Children;
        var field = instruction is null ? string.Empty : ReadDestinationText(instruction).Trim();
        if (!field.StartsWith(HyperlinkInstruction, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Only HYPERLINK RTF fields are supported.");
        }

        var target = field[HyperlinkInstruction.Length..].Trim();
        if (target.StartsWith('"'))
        {
            var end = target.IndexOf('"', 1);
            target = end > 0 ? target[1..end] : target.Trim('"');
        }

        _ = _paragraph.Append("<a href=\"").Append(WebUtility.HtmlEncode(target)).Append("\">");
        if (result is not null)
        {
            Render(result, formatting);
        }

        _ = _paragraph.Append("</a>");
    }

    /// <summary>Updates font, size, color, and Unicode fallback state.</summary>
    /// <param name = "token">The token input.</param>
    /// <param name = "formatting">The formatting input.</param>
    /// <returns>The conversion result.</returns>
    private Formatting ApplyFormatting(Token token, Formatting formatting) => token.Word switch
    {
        "uc" => formatting with
        {
            UnicodeSkip = Math.Max(0, token.Number ?? 1)
        },
        "f" => formatting with
        {
            Font = token.Number ?? _defaultFont
        },
        "fs" => formatting with
        {
            Size = token.Number ?? DefaultFontSize
        },
        "cf" => formatting with
        {
            Color = token.Number ?? 0
        },
        "highlight" or "cb" => formatting with
        {
            Background = token.Number ?? 0
        },
        "plain" => new Formatting { Font = _defaultFont, UnicodeSkip = formatting.UnicodeSkip },
        _ => ApplyDecorations(token, formatting),
    };

    /// <summary>Appends visible controls or applies document structure.</summary>
    /// <param name = "token">The token input.</param>
    /// <param name = "formatting">The formatting input.</param>
    private void RenderControl(Token token, Formatting formatting)
    {
        if (SpecialCharacters.TryGetValue(token.Word ?? string.Empty, out var character))
        {
            AppendText(character, formatting);
            return;
        }

        if (token.Word == "u")
        {
            AppendUnicode(token, formatting);
            return;
        }

        if (token.Word == "par")
        {
            FlushParagraph(true);
            return;
        }

        if (token.Word == "line")
        {
            _ = _paragraph.Append("<br>");
            return;
        }

        if (token.Word == "page")
        {
            AppendPageBreak();
            return;
        }

        RenderStructuralControl(token);
    }

    /// <summary>Appends a decoded Unicode character and starts fallback skipping.</summary>
    /// <param name = "token">The Unicode token.</param>
    /// <param name = "formatting">The character formatting.</param>
    private void AppendUnicode(Token token, Formatting formatting)
    {
        var character = (char)(token.Number ?? 0);
        if (char.IsHighSurrogate(character))
        {
            if (_highSurrogate is not null)
            {
                throw new FormatException("The RTF document contains consecutive Unicode high surrogates.");
            }

            _highSurrogate = character;
            _surrogateFormatting = formatting;
        }
        else
        {
            AppendUnicodeCharacter(character, formatting);
        }

        _fallback = formatting.UnicodeSkip;
    }

    /// <summary>Combines surrogate controls before encoding a complete Unicode scalar.</summary>
    /// <param name="character">The decoded UTF-16 character.</param>
    /// <param name="formatting">The character formatting.</param>
    private void AppendUnicodeCharacter(char character, Formatting formatting)
    {
        if (_highSurrogate is { } high)
        {
            if (!char.IsLowSurrogate(character))
            {
                throw new FormatException("The RTF document contains an incomplete Unicode surrogate pair.");
            }

            _highSurrogate = null;
            AppendText(new([high, character]), _surrogateFormatting);
            return;
        }

        if (char.IsLowSurrogate(character))
        {
            throw new FormatException("The RTF document contains an unpaired Unicode low surrogate.");
        }

        AppendText(character.ToString(), formatting);
    }

    /// <summary>Ends the current block and appends an explicit page break.</summary>
    private void AppendPageBreak()
    {
        FlushParagraph();
        CloseTable();
        _ = Output.Append("<br style=\"break-before:page\" />");
    }

    /// <summary>Applies non-character document controls.</summary>
    /// <param name = "token">The structural control.</param>
    private void RenderStructuralControl(Token token)
    {
        RenderTableControl(token);
        ApplyParagraphFormatting(token);
    }

    /// <summary>Finishes a table cell.</summary>
    /// <returns>A recognized cell boundary result.</returns>
    private bool EndCell()
    {
        EnsureCell();
        FlushParagraph();
        _tables.EndCell();
        return true;
    }

    /// <summary>Finishes a table row.</summary>
    /// <returns>A recognized row boundary result.</returns>
    private bool EndRow()
    {
        FlushParagraph();
        _tables.EndRow();
        _nestedProperties = false;
        _tableParagraph = false;
        return true;
    }

    /// <summary>Reads native row, cell, merge, and nesting controls.</summary>
    /// <param name="token">The table control.</param>
    private void RenderTableControl(Token token)
    {
        if (token.Word == "trowd")
        {
            FlushParagraph();
            if (!_nestedProperties)
            {
                _tables.SetDepth(_paragraphDepth, _output);
            }

            _tables.BeginDefinition();
            _tableParagraph = true;
            return;
        }

        if (_tables.ApplyProperty(token.Word, token.Number))
        {
            return;
        }

        ApplyTableBoundary(token);
    }

    /// <summary>Changes table depth or terminates physical row and cell contents.</summary>
    /// <param name="token">The table boundary control.</param>
    private void ApplyTableBoundary(Token token) => _ = token.Word switch
    {
        "itap" => ChangeTableDepth(token.Number ?? 1),
        "intbl" => _tableParagraph = true,
        "cell" or "nestcell" => EndCell(),
        "row" or "nestrow" => EndRow(),
        "pard" => ResetTableParagraph(),
        _ => false,
    };

    /// <summary>Changes the active paragraph's table nesting level.</summary>
    /// <param name="depth">The requested table depth.</param>
    /// <returns>A recognized nesting control result.</returns>
    private bool ChangeTableDepth(int depth)
    {
        FlushParagraph();
        _paragraphDepth = depth;
        _tables.SetDepth(_paragraphDepth, _output);
        return true;
    }

    /// <summary>Restores the default paragraph state before subsequent table controls.</summary>
    /// <returns>A recognized paragraph reset result.</returns>
    private bool ResetTableParagraph()
    {
        _tableParagraph = false;
        _paragraphDepth = 1;
        return true;
    }

    /// <summary>Updates paragraph alignment from an RTF control.</summary>
    /// <param name = "token">The token input.</param>
    private void ApplyParagraphFormatting(Token token) => _alignment = token.Word switch
    {
        "ql" or "pard" => "left",
        "qr" => "right",
        "qc" => "center",
        "qj" => "justify",
        _ => _alignment,
    };

    /// <summary>Appends escaped text with its character formatting.</summary>
    /// <param name = "text">The text input.</param>
    /// <param name = "formatting">The formatting input.</param>
    private void AppendText(string text, Formatting formatting)
    {
        if (text.Length == 0)
        {
            return;
        }

        if (_highSurrogate is not null)
        {
            throw new FormatException("Text interrupts an RTF Unicode surrogate pair.");
        }

        if (_tables.HasTable && !_tables.InRow && !_tableParagraph)
        {
            CloseTable();
        }

        EnsureCell();
        var style = GetTextStyle(formatting);
        _ = _paragraph.Append("<span style=\"").Append(WebUtility.HtmlEncode(style.ToString())).Append("\">").Append(WebUtility.HtmlEncode(text)).Append("</span>");
    }

    /// <summary>Builds the CSS for one character formatting scope.</summary>
    /// <param name = "formatting">The formatting input.</param>
    /// <returns>The conversion result.</returns>
    private StringBuilder GetTextStyle(Formatting formatting)
    {
        var style = new StringBuilder();
        if (_fonts.TryGetValue(formatting.Font, out var font))
        {
            _ = style.Append("font-family:'").Append(font.Replace("'", "\\'", StringComparison.Ordinal)).Append("';");
        }

        _ = style.Append("font-size:").Append((formatting.Size / HalfPointsPerPoint).ToString(CultureInfo.InvariantCulture)).Append("pt;");
        AppendDecorations(style, formatting);
        AppendColor(style, "color", formatting.Color);
        AppendColor(style, "background-color", formatting.Background);
        return style;
    }

    /// <summary>Adds a non-automatic table color to the generated CSS.</summary>
    /// <param name = "style">The style input.</param>
    /// <param name = "property">The property input.</param>
    /// <param name = "index">The index input.</param>
    private void AppendColor(StringBuilder style, string property, int index)
    {
        if (!(index >= 0 && index < _colors.Count && _colors[index].Length > 0))
        {
            return;
        }

        _ = style.Append(property).Append(':').Append(_colors[index]).Append(';');
    }

    /// <summary>Embeds PNG or JPEG bytes as a data URI.</summary>
    /// <param name = "tokens">The tokens input.</param>
    private void AppendPicture(List<Token> tokens)
    {
        var png = tokens.Exists(static t => t.Word == "pngblip");
        if (!png && !tokens.Exists(static t => t.Word == "jpegblip"))
        {
            throw new NotSupportedException("Only PNG and JPEG RTF pictures are supported.");
        }

        var data = ReadPictureData(tokens);
        EnsureCell();
        _ = _paragraph.Append("<img src=\"data:image/").Append(png ? "png" : "jpeg").Append(";base64,").Append(Convert.ToBase64String(data.ToArray())).Append('"');
        AppendPictureSize(tokens, "picwgoal", "width");
        AppendPictureSize(tokens, "pichgoal", "height");
        _ = _paragraph.Append('>');
    }

    /// <summary>Converts a picture dimension from twips to CSS pixels.</summary>
    /// <param name = "tokens">The tokens input.</param>
    /// <param name = "word">The word input.</param>
    /// <param name = "attribute">The attribute input.</param>
    private void AppendPictureSize(List<Token> tokens, string word, string attribute)
    {
        var size = tokens.Find(t => t.Word == word)?.Number;
        if (size is not > 0)
        {
            return;
        }

        _ = _paragraph.Append(' ').Append(attribute).Append("=\"").Append((size.Value / TwipsPerPixel).ToString(CultureInfo.InvariantCulture)).Append('"');
    }

    /// <summary>Ensures the active physical cell exists before appending content.</summary>
    private void EnsureCell()
    {
        if (_tableParagraph)
        {
            _tables.SetDepth(_paragraphDepth, _output);
            _tables.EnsureRow();
        }

        if (!_tables.InRow)
        {
            return;
        }

        _tables.EnsureContent();
    }

    /// <summary>Emits the accumulated paragraph, optionally retaining an empty paragraph.</summary>
    /// <param name = "force">The force input.</param>
    private void FlushParagraph(bool force = false)
    {
        if (!(_paragraph.Length > 0 || force))
        {
            return;
        }

        if (_tables.HasTable && !_tables.InRow && !_tableParagraph)
        {
            CloseTable();
        }

        EnsureCell();
        _ = Output.Append("<p style=\"text-align:").Append(_alignment).Append(";white-space:pre-wrap\">").Append(_paragraph).Append("</p>");
        _ = _paragraph.Clear();
    }

    /// <summary>Emits unterminated final text without inventing a paragraph break.</summary>
    private void FlushFinalContent()
    {
        if (_paragraph.Length == 0)
        {
            return;
        }

        if (_tables.InRow)
        {
            FlushParagraph();
            return;
        }

        CloseTable();
        _ = Output.Append("<span style=\"text-align:").Append(_alignment)
            .Append(";white-space:pre-wrap\">").Append(_paragraph).Append("</span>");
        _ = _paragraph.Clear();
    }

    /// <summary>Completes buffered table boundaries.</summary>
    private void CloseTable() => _tables.Close(_output);

    /// <summary>Tracks RGB components and automatic color entries.</summary>
    private sealed class ColorComponents
    {
        /// <summary>The maximum RGB component value.</summary>
        private const int MaximumColorComponent = 255;

        /// <summary>Stores the red conversion state.</summary>
        private int _red;

        /// <summary>Stores the green conversion state.</summary>
        private int _green;

        /// <summary>Stores the blue conversion state.</summary>
        private int _blue;

        /// <summary>Stores the automatic conversion state.</summary>
        private bool _automatic = true;

        /// <summary>Reads a color component and marks the entry as explicit.</summary>
        /// <param name = "token">The token input.</param>
        internal void Set(Token token)
        {
            if (token.Word is not ("red" or "green" or "blue"))
            {
                return;
            }

            _automatic = false;
            var number = Math.Clamp(token.Number ?? 0, 0, MaximumColorComponent);
            if (token.Word == "red")
            {
                _red = number;
            }

            if (token.Word == "green")
            {
                _green = number;
            }

            if (token.Word != "blue")
            {
                return;
            }

            _blue = number;
        }

        /// <summary>Returns a CSS color or an empty automatic color.</summary>
        /// <returns>The conversion result.</returns>
        internal string ToHtml() => _automatic ? string.Empty : $"#{_red:X2}{_green:X2}{_blue:X2}";
    }

    /// <summary>Stores one RTF control, scoped group, text run, or binary payload.</summary>
    private sealed class Token
    {
        /// <summary>Gets the Word formatting value.</summary>
        internal string? Word { get; init; }

        /// <summary>Gets the Number formatting value.</summary>
        internal int? Number { get; init; }

        /// <summary>Gets the Text formatting value.</summary>
        internal string? Text { get; init; }

        /// <summary>Gets the Binary formatting value.</summary>
        internal string? Binary { get; init; }

        /// <summary>Gets the Children formatting value.</summary>
        internal List<Token>? Children { get; init; }
    }

    /// <summary>Captures character formatting inherited by nested RTF groups.</summary>
    private sealed record Formatting
    {
        /// <summary>Gets the Font formatting value.</summary>
        internal int Font { get; init; }

        /// <summary>Gets the Size formatting value.</summary>
        internal int Size { get; init; } = DefaultFontSize;

        /// <summary>Gets the Color formatting value.</summary>
        internal int Color { get; init; }

        /// <summary>Gets the Background formatting value.</summary>
        internal int Background { get; init; }

        /// <summary>Gets the UnicodeSkip formatting value.</summary>
        internal int UnicodeSkip { get; init; } = 1;

        /// <summary>Gets the Bold formatting value.</summary>
        internal bool Bold { get; init; }

        /// <summary>Gets the Italic formatting value.</summary>
        internal bool Italic { get; init; }

        /// <summary>Gets the Underline formatting value.</summary>
        internal bool Underline { get; init; }

        /// <summary>Gets the Strike formatting value.</summary>
        internal bool Strike { get; init; }

        /// <summary>Gets the Baseline formatting value.</summary>
        internal string Baseline { get; init; } = string.Empty;
    }
}
