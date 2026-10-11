# Avalonia rich document editing

CrissCross provides a self-contained rich editor.

```xml
<StackPanel xmlns="https://github.com/avaloniaui"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:cc="clr-namespace:CrissCross.Avalonia.UI.Controls;assembly=CrissCross.Avalonia.UI">
    <cc:RichTextEditorRibbon Editor="{Binding ElementName=Editor}" />
    <cc:RichTextBox x:Name="Editor" Height="650"
                    IsPageLayoutEnabled="True"
                    PageSize="794,1123" PagePadding="48" PageSpacing="20" />
</StackPanel>
```

Include the existing CrissCross control styles in the application. The reactive package uses the corresponding `CrissCross.Reactive.Avalonia.UI.Controls` namespace.

## Editing and interchange

- Rich HTML preserves fonts, colors, highlights, hyperlinks, inline images, and tables with row/column spans.
- The ribbon composes existing editor commands with font, paragraph alignment, lists, image, hyperlink, table, and page-break actions.
- Paged layout partitions measured text lines and embedded objects. Paragraphs retain independent alignment, including wrapped paragraphs spanning sheets. Pointer selection and caret navigation use document positions. Unicode movement/deletion preserves text elements.
- Explicit `break-before:page`, `page-break-before:always`, and inserted page breaks start new sheets. Page dimensions and margins are set on the editor.
- Clipboard operations prefer HTML, then RTF, then an image or plain text. Native HTML and RTF use byte formats with their respective encodings. Windows CF_HTML offsets count UTF-8 bytes. Class-based Office stylesheet declarations are inlined, and source wrapping whitespace is normalized when extracting a fragment.
- Local clipboard image files are embedded as data URIs for portability, with a 10 MiB limit per image. Unavailable or unsupported sources remain in the document. Remote loading requires the presenter's existing opt-in loader.
- `Save(stream, RichTextDataFormat.Rtf)` and `Load(stream, RichTextDataFormat.Rtf)` support basic RTF typography, Unicode, links, embedded PNG/JPEG, simple tables and explicit page breaks. RTF readers preserve byte-valued binary payloads with the default Latin-1 decoding.

Use the asynchronous clipboard APIs when completion matters:

```csharp
editor.SelectAll();
await editor.CopyToClipboardAsync();
await destination.PasteFromClipboardAsync();
```

Handle `HyperlinkActivated` in the host to decide how to open a target. The editor does not launch arbitrary addresses itself. `IsReadOnly` and display mode prevent editing commands while retaining selection/copy.

## Gallery verification

The Input page contains a paged document, a reusable ribbon, and a second editor for clipboard transfer. Both use the platform clipboard. Deterministic sample buttons temporarily use an in-memory adapter.

`src/CrissCross.Avalonia.UI.Gallery/Fixtures/WordInteropSample.rtf` contains three explicit pages, different fonts/colors, Unicode text, a hyperlink, an embedded PNG, and a table. Copy the fixture to a temporary location before opening it in Microsoft Word so Word's file lock does not interfere with builds. Select all in Word, copy, focus the gallery editor, select all, and paste. Repeat in the opposite direction using a new unsaved Word document.

Choose Word's **Keep Source Formatting** paste option when verifying exported fonts, colors and highlighting. Word's **Merge Formatting** option deliberately applies destination formatting.

## Compatibility boundaries

This is not complete WPF RichTextBox or Microsoft Word fidelity. The custom document API does not expose WPF's complete FlowDocument object model.

- An oversized indivisible table or image expands its sheet instead of splitting across pages. Each sheet currently retains a presenter for the full document.
- Native DOCX import/export, section page setup, headers/footers, footnotes, revisions, embedded OLE objects, and full Word style/list semantics are not implemented.
- Character formatting commands apply to selected content; a separate pending typing-format state is not implemented.
- RTF rejects unsupported constructs rather than silently discarding them. Clipboard export omits the optional RTF representation when it cannot encode a construct, retaining the HTML representation.
- RTF table export does not reproduce merged/nested tables or complete table width/border styling. HTML retains these structures.
- Paged editing currently routes committed text and keyboard gestures directly; it does not provide the complete TextBox IME/accessibility editing contract.
