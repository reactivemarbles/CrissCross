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
- `Save(stream, RichTextDataFormat.Rtf)` and `Load(stream, RichTextDataFormat.Rtf)` support typography, Unicode, links, embedded PNG/JPEG, merged and nested tables, table widths/borders/shading, and explicit page breaks. RTF readers preserve byte-valued binary payloads with the default Latin-1 decoding.
- `Save(stream, RichTextDataFormat.Docx)` and `Load(stream, RichTextDataFormat.Docx)` read and write native Word ZIP/XML packages. Character encodings apply to text formats; DOCX streams are binary. The caller retains ownership of DOCX streams.
- Character commands at a collapsed selection set insertion formatting. Subsequent typing and IME commits retain those styles; moving the insertion position resets explicit pending styles.
- Ordinary tables paginate at measured row boundaries. Vertically merged row groups remain together.

## Document structures

Use `CreateSection(id, size, padding)` to apply paper geometry to selected content. `SetPageRegion(html, isFooter, sectionId, variant)` configures repeated page regions; variants are `default`, `first`, `even`, and `odd`. Page regions use separate text stories and do not consume body selection positions. Fields such as `<span data-page-field="PAGE">1</span>` and `NUMPAGES` resolve after pagination.

`InsertNote`, `SetNoteText`, `AddBookmark`, `NavigateToBookmark`, `AddComment`, and `RemoveComment` expose document annotations. Footnotes reserve measured space on their reference sheet. `TrackChanges` and `RevisionAuthor` record editor replacements; `AcceptRevision` and `RejectRevision` resolve individual changes. These mutations participate in editor history.

The paged input method client supplies surrounding text, selection and transformed cursor geometry. Composition uses a transient rich document which replaces the selected range visually and wraps across sheets; committing produces one undoable replacement.

`RichTextBoxAutomationPeer` exposes Avalonia's native editable value provider and semantic text, link, image and table children. Its application range APIs provide document text, selection, point lookup, scrolling and measured range rectangles. These application APIs are distinct from the platform's native TextPattern contract.

Measured headers, footers and footnotes expose separate read-only value providers and semantic children with page-specific geometry. These stories do not expose a separate native TextPattern. Endnotes remain part of the body text range.

Windows applications can call `RichTextWindowsAutomation.Enable(window)` after the window has a Win32 handle and retain the returned registration until the window closes. The registration supplies native TextPattern/TextRange and table/grid providers through a window automation tree over public Avalonia peers. Standard control patterns remain available. This opt-in adapter uses runtime COM callable wrappers and requires an untrimmed Windows runtime; it explicitly rejects Native AOT and native control islands. The editor's cross-platform rendering and input method implementation do not require this adapter.

If native control content is inserted later, the registration restores Avalonia's original provider and raises `RichTextWindowsAutomation.RegistrationUnavailable` with the affected window and reason.

Hosts that disable built-in COM support must set `<BuiltInComInteropSupport>true</BuiltInComInteropSupport>` when using this adapter. The Gallery enables this runtime capability.

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

`Fixtures/WordNativeSample.docx` was saved by Microsoft Word from the rich fixture. The gallery's **Open Word Document** and **Save Word Document** actions exercise native DOCX interchange. The test project independently validates exports using Microsoft's Open XML SDK; the production editor uses BCL ZIP/XML and its existing HTML dependencies.

Run the Gallery with `--native-richtext-automation` to exercise the native Windows accessibility adapter.

## Compatibility boundaries

This is not complete WPF RichTextBox or Microsoft Word fidelity. The custom document API does not expose WPF's complete FlowDocument object model.

- Oversized indivisible images, merged row groups and footnotes expand sheets. Each sheet currently retains a presenter for the full document.
- Embedded OLE/ActiveX objects and macros are unsupported. Passive unsupported DOCX XML is retained for interchange without becoming an editable visual element. This does not imply full Word layout or style fidelity.
- Nested sections use their outer section's geometry. Page chrome clips to configured margins. Comments store annotations without a separate review pane.
- RTF rejects unsupported constructs rather than silently discarding them. Clipboard export omits the optional RTF representation when it cannot encode a construct, retaining the HTML representation.
- The Windows accessibility adapter requires runtime COM support and an untrimmed host without native control islands. Other platforms use Avalonia's editable value and semantic child contracts.
