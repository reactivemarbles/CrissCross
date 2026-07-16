// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>Provides complete BBCodeBlock syntax examples.</summary>
public partial class BBCodeBlockViewModel : RxObject
{
    /// <summary>The deterministic local image used by image examples.</summary>
    private const string LocalImage =
        "/CrissCross.WPF.UI.Gallery;component/Assets/ControlImages/RichTextBlock.png";

    /// <summary>The latest command-link payload.</summary>
    [Reactive]
    private string _lastCommandParameter = "No command link has been selected.";

    /// <summary>Initializes a new instance of the <see cref="BBCodeBlockViewModel"/> class.</summary>
    public BBCodeBlockViewModel()
    {
        ExampleCommand = ReactiveCommand.Create<string>(ExecuteExampleCommand);
        ExampleGroups = CreateExampleGroups();
    }

    /// <summary>Gets the grouped reference and extension examples.</summary>
    public IReadOnlyList<BBCodeExampleGroup> ExampleGroups { get; }

    /// <summary>Gets the command used by <c>cmd:</c> links.</summary>
    public ReactiveCommand<string, Unit> ExampleCommand { get; }

    /// <summary>Creates the complete gallery example matrix.</summary>
    /// <returns>The example groups.</returns>
    private static IReadOnlyList<BBCodeExampleGroup> CreateExampleGroups() =>
    [
        CreateFormattingGroup(),
        CreateStylesGroup(),
        CreateAlignmentGroup(),
        CreateQuotesGroup(),
        CreateLinksGroup(),
        CreateImagesGroup(),
        CreateListsGroup(),
        CreateCodeGroup(),
        CreateTablesGroup(),
        CreateMediaGroup(),
        CreateRobustnessGroup(),
    ];

    /// <summary>Creates the formatting examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateFormattingGroup() =>
        new(
            "Formatting",
            [
            new("Bold", "Bold and strong aliases.", "[b]Bold[/b] and [strong]strong[/strong]"),
            new("Italic", "Italic and emphasis aliases.", "[i]Italic[/i] and [em]emphasis[/em]"),
            new("Underline", "Underline and inserted aliases.", "[u]Underline[/u] and [ins]inserted[/ins]"),
            new(
                "Strike",
                "Strike, strike-through, and delete aliases.",
                "[s]Strike[/s], [strike]strike alias[/strike], [del]deleted[/del]"),
            new(
                "Nested",
                "Nested formatting restores its parent style.",
                "[b]Bold [i]bold italic [u]and underlined[/u][/i] bold again[/b]"),
            new("Case insensitive", "Tag names are case-insensitive.", "[B]Upper-case bold[/B] [I]and italic[/I]"),
        ]);

    /// <summary>Creates the styles examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateStylesGroup() =>
        new(
            "Size, font, color, and style",
            [
            new(
                "Size",
                "Named, numeric, and percentage sizes.",
                "[size=small]Small[/size] [size=22]22px[/size] [size=150%]150 percent[/size]"),
            new("Font", "Author-selected font family.", "[font=Consolas]Consolas text[/font]"),
            new(
                "Color",
                "Named, RGB, and ARGB color values.",
                "[color=Orange]Orange[/color] [color=#0078D4]Blue[/color] [color=#FF107C10]Green[/color]"),
            new(
                "Style attributes",
                "bbcode.org complex style attributes.",
                "[style size=18 color=#0078D4 font=Consolas]Combined style[/style]"),
        ]);

    /// <summary>Creates the alignment examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateAlignmentGroup() =>
        new(
            "Alignment and headings",
            [
            new(
                "Alignment tags",
                "Left, center, right, and justify blocks.",
                "[left]Left[/left][center]Center[/center][right]Right[/right][justify]Justified text[/justify]"),
            new(
                "Align parameter",
                "Parameterized alignment aliases.",
                "[align=center]Centered[/align][align=right]Right aligned[/align]"),
            new(
                "Headings",
                "Heading levels one through six.",
                "[h1]Heading 1[/h1][h2]Heading 2[/h2][h3]Heading 3[/h3]" +
                "[h4]Heading 4[/h4][h5]Heading 5[/h5][h6]Heading 6[/h6]"),
            new(
                "Heading aliases",
                "Named heading and paragraph extensions.",
                "[header]Header alias[/header][heading=3]Parameterized heading[/heading]" +
                "[p]A paragraph with themed text.[/p]"),
        ]);

    /// <summary>Creates the quotes examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateQuotesGroup() =>
        new(
            "Quotes, spoilers, and effects",
            [
            new(
                "Quote",
                "Unnamed, named, and q alias quotations.",
                "[quote]A quotation[/quote][quote=Grace Hopper]" +
                "The most dangerous phrase is: we've always done it this way.[/quote]" +
                "[q]A compact quote alias[/q]"),
            new(
                "Spoiler",
                "Keyboard-accessible collapsed spoilers and aliases.",
                "[spoiler]Secret[/spoiler][spoiler=Release notes]Named spoiler[/spoiler]" +
                "[spoil]Alias[/spoil][hide=Details]Hidden details[/hide]"),
            new(
                "Blur",
                "Blurred text with an optional author color.",
                "[blur]Blurred text[/blur] [blur=Red]Red blurred text[/blur]"),
            new(
                "Super and subscript",
                "Scientific and mathematical typography.",
                "E=mc[sup]2[/sup] and H[sub]2[/sub]O"),
        ]);

    /// <summary>Creates the links examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateLinksGroup() =>
        new(
            "Links, email, and commands",
            [
            new(
                "URL",
                "Plain, named, and link alias URLs.",
                "[url]https://www.bbcode.org/[/url] [url=https://www.bbcode.org/]BBCode.org[/url]" +
                " [link=https://www.bbcode.org/reference.php]Reference[/link]"),
            new(
                "Email",
                "Plain, named, and mail alias addresses.",
                "[email]gallery@example.com[/email] [email=gallery@example.com]Email the gallery[/email]" +
                " [mail]alias@example.com[/mail]"),
            new(
                "Command",
                "A cmd link invokes the view-model command without shell navigation.",
                "[url=cmd:refresh:all]Run the reactive gallery command[/url]"),
            new(
                "Unsafe URI",
                "Unsupported URI schemes render as non-clickable text.",
                "[url=javascript:alert(1)]Unsafe schemes are blocked[/url]"),
        ]);

    /// <summary>Creates the images examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateImagesGroup() =>
        new(
            "Images",
            [
            new("Standard image", "The standard inner-URL form uses a local resource.", $"[img]{LocalImage}[/img]"),
            new("Resized shorthand", "Width by height shorthand.", $"[img=64x64]{LocalImage}[/img]"),
            new(
                "Complex image",
                "Named width, height, alt, and title attributes.",
                $"[img width=72 height=72 alt=Local-gallery-image title=Rich-text-icon]{LocalImage}[/img]"),
            new(
                "Legacy image",
                "Compatibility with the migrated comma-delimited form.",
                $"[img={LocalImage},width=60,height=60]Legacy caption[/img]"),
            new("Image alias", "The image alias uses standard syntax.", $"[image]{LocalImage}[/image]"),
        ]);

    /// <summary>Creates the lists examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateListsGroup() =>
        new(
            "Lists",
            [
            new(
                "Unordered",
                "ul/li and list/* forms.",
                "[ul][li]One[/li][li]Two[/li][/ul][list][*]Alpha[*]Beta[/list]"),
            new(
                "Ordered",
                "ol/li and numeric list forms.",
                "[ol][li]First[/li][li]Second[/li][/ol][list=1][*]One[*]Two[/list]"),
            new(
                "Markers",
                "Circle, disc, square, alphabetic, and Roman marker options.",
                "[list=circle][*]Circle[/list][list=disc][*]Disc[/list][list=square][*]Square[/list]" +
                "[list=a][*]lower alpha[/list][list=A][*]upper alpha[/list]" +
                "[list=i][*]lower Roman[/list][list=I][*]upper Roman[/list]"),
            new(
                "Nested list",
                "Lists can be nested inside list items.",
                "[ul][li]Parent[ol][li]First child[/li][li]Second child[/li][/ol][/li][/ul]"),
        ]);

    /// <summary>Creates the code examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateCodeGroup() =>
        new(
            "Code and preformatted text",
            [
            new("Code", "Code contents remain literal.", "[code]var text = \"[b]literal[/b]\";[/code]"),
            new(
                "Language code",
                "Language metadata is retained as a tooltip.",
                "[code=csharp]public string Name { get; set; }[/code]"),
            new(
                "Preformatted",
                "Pre and NFO preserve spaces and new lines.",
                "[pre]first line\n    indented line[/pre][nfo]+-----+\n| NFO |\n+-----+[/nfo]"),
            new(
                "Literal aliases",
                "Inline-code and noparse extensions.",
                "[c]inline [b]literal[/b][/c] [noparse][i]not italic[/i][/noparse]"),
        ]);

    /// <summary>Creates the tables examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateTablesGroup() =>
        new(
            "Tables and structure",
            [
            new(
                "Table",
                "Canonical table, row, heading, and data-cell tags.",
                "[table][tr][th]Name[/th][th]Role[/th][/tr]" +
                "[tr][td][b]Ada[/b][/td][td]Mathematician[/td][/tr]" +
                "[tr][td]Grace[/td][td]Computer scientist[/td][/tr][/table]"),
            new(
                "Table aliases",
                "row/cell aliases compose into the same grid.",
                "[table][row][cell]A[/cell][cell]B[/cell][/row][/table]"),
            new(
                "Pipe table",
                "Pipe-delimited table extension.",
                "[pipes]| Name | Value |\n| Alpha | 1 |\n| Beta | 2 |[/pipes]"),
            new(
                "Breaks and rules",
                "Line breaks, self-closing breaks, line, and hr.",
                "First[br]Second[br /]Third[line]After line[hr]After rule"),
        ]);

    /// <summary>Creates the media examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateMediaGroup() =>
        new(
            "Media and ratings",
            [
            new(
                "YouTube",
                "YouTube IDs render a safe open affordance without auto-play.",
                "[youtube]E7d-3-uXlZM[/youtube]"),
            new(
                "Video",
                "Video, bbvideo, and gvideo links never auto-launch while parsing.",
                "[video]https://example.com/video.mp4[/video]" +
                "[bbvideo]https://example.com/video.mp4[/bbvideo][gvideo]123456[/gvideo]"),
            new(
                "Rating",
                "Rating, maximum, and rate aliases.",
                "[rate=3.5] [rating=4] [rating=7 max=10]"),
        ]);

    /// <summary>Creates the robustness examples.</summary>
    /// <returns>The example group.</returns>
    private static BBCodeExampleGroup CreateRobustnessGroup() =>
        new(
            "Robustness",
            [
            new(
                "Unknown",
                "Unknown tags remain visible instead of silently losing content.",
                "Before [custom option=value]unknown content[/custom] after"),
            new(
                "Malformed",
                "Unclosed tags retain their parsed content without hanging.",
                "[b]Unclosed bold and a literal closing [/missing] tag"),
        ]);

    /// <summary>Records a command-link payload.</summary>
    /// <param name="parameter">The command payload.</param>
    private void ExecuteExampleCommand(string parameter) => LastCommandParameter = "Command received: " + parameter;
}
