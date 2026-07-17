// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>
/// Represents a rich editing control that provides multi-line text editing capabilities with visual formatting.
/// Supports bold, italic, underline, and strikethrough formatting via keyboard shortcuts and context menu.
/// </summary>
/// <remarks>
/// Key features matching WPF RichTextBox:
/// <list type="bullet">
/// <item><description>Multi-line text editing with word wrap.</description></item>
/// <item><description>Visual text formatting (Bold, Italic, Underline, Strikethrough).</description></item>
/// <item><description>Keyboard shortcuts (Ctrl+B, Ctrl+I, Ctrl+U).</description></item>
/// <item><description>Context menu with formatting options.</description></item>
/// <item><description>Undo/Redo support.</description></item>
/// <item><description>Save/Load content to/from files or streams.</description></item>
/// </list>
/// </remarks>
public partial class RichTextBox : TemplatedControl
{
    /// <summary>Property for <see cref="Text"/>.</summary>
    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<
        RichTextBox,
        string?
    >(nameof(Text), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Property for <see cref="AcceptsReturn"/>.</summary>
    public static readonly StyledProperty<bool> AcceptsReturnProperty = AvaloniaProperty.Register<
        RichTextBox,
        bool
    >(nameof(AcceptsReturn), true);

    /// <summary>Property for <see cref="AcceptsTab"/>.</summary>
    public static readonly StyledProperty<bool> AcceptsTabProperty = AvaloniaProperty.Register<
        RichTextBox,
        bool
    >(nameof(AcceptsTab), true);

    /// <summary>Property for <see cref="IsReadOnly"/>.</summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<
        RichTextBox,
        bool
    >(nameof(IsReadOnly), false);

    /// <summary>Property for <see cref="IsTextSelectionEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsTextSelectionEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsTextSelectionEnabled), false);

    /// <summary>Property for <see cref="TextWrapping"/>.</summary>
    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        AvaloniaProperty.Register<RichTextBox, TextWrapping>(
            nameof(TextWrapping),
            TextWrapping.Wrap);

    /// <summary>Property for <see cref="HorizontalScrollBarVisibility"/>.</summary>
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<RichTextBox, ScrollBarVisibility>(
            nameof(HorizontalScrollBarVisibility),
            ScrollBarVisibility.Disabled);

    /// <summary>Property for <see cref="VerticalScrollBarVisibility"/>.</summary>
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<RichTextBox, ScrollBarVisibility>(
            nameof(VerticalScrollBarVisibility),
            ScrollBarVisibility.Auto);

    /// <summary>Property for <see cref="CaretBrush"/>.</summary>
    public static readonly StyledProperty<IBrush?> CaretBrushProperty = AvaloniaProperty.Register<
        RichTextBox,
        IBrush?
    >(nameof(CaretBrush), Brushes.White);

    /// <summary>Property for <see cref="SelectionBrush"/>.</summary>
    public static readonly StyledProperty<IBrush?> SelectionBrushProperty =
        AvaloniaProperty.Register<RichTextBox, IBrush?>(
            nameof(SelectionBrush),
            Brush.Parse("#400078D4"));

    /// <summary>Property for <see cref="Watermark"/>.</summary>
    public static readonly StyledProperty<string?> WatermarkProperty = AvaloniaProperty.Register<
        RichTextBox,
        string?
    >(nameof(Watermark));

    /// <summary>Property for <see cref="IsFormattingEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsFormattingEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsFormattingEnabled), true);

    /// <summary>Property for <see cref="IsDocumentEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsDocumentEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsDocumentEnabled), false);

    /// <summary>Property for <see cref="IsDragDropEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsDragDropEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsDragDropEnabled), true);

    /// <summary>Property for <see cref="EditMode"/>.</summary>
    public static readonly StyledProperty<RichTextEditMode> EditModeProperty =
        AvaloniaProperty.Register<RichTextBox, RichTextEditMode>(
            nameof(EditMode),
            RichTextEditMode.EditOnFocus);

    /// <summary>Property for <see cref="IsRichClipboardEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsRichClipboardEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsRichClipboardEnabled), true);

    /// <summary>Property for <see cref="IsImagePasteEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsImagePasteEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsImagePasteEnabled), true);

    /// <summary>Property for <see cref="IsImageDropEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsImageDropEnabledProperty =
        AvaloniaProperty.Register<RichTextBox, bool>(nameof(IsImageDropEnabled), true);

    /// <summary>Property for <see cref="MaxDroppedTextFileBytes"/>.</summary>
    public static readonly StyledProperty<long> MaxDroppedTextFileBytesProperty =
        AvaloniaProperty.Register<RichTextBox, long>(nameof(MaxDroppedTextFileBytes), 1_048_576);

    /// <summary>Property for <see cref="SelectionStart"/>.</summary>
    public static readonly StyledProperty<int> SelectionStartProperty = AvaloniaProperty.Register<
        RichTextBox,
        int
    >(nameof(SelectionStart));

    /// <summary>Property for <see cref="SelectionEnd"/>.</summary>
    public static readonly StyledProperty<int> SelectionEndProperty = AvaloniaProperty.Register<
        RichTextBox,
        int
    >(nameof(SelectionEnd));

    /// <summary>Property for <see cref="CaretIndex"/>.</summary>
    public static readonly StyledProperty<int> CaretIndexProperty = AvaloniaProperty.Register<
        RichTextBox,
        int
    >(nameof(CaretIndex));

    /// <summary>Defines the <see cref="TextChanged"/> event.</summary>
    public static readonly RoutedEvent<TextChangedEventArgs> TextChangedEvent =
        RoutedEvent.Register<RichTextBox, TextChangedEventArgs>(
            nameof(TextChanged),
            RoutingStrategies.Bubble);

    /// <summary>Defines the <see cref="SelectionChanged"/> event.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<RichTextBox, RoutedEventArgs>(
            nameof(SelectionChanged),
            RoutingStrategies.Bubble);

    /// <summary>Defines the <see cref="FormattingApplied"/> event.</summary>
    public static readonly RoutedEvent<FormattingEventArgs> FormattingAppliedEvent =
        RoutedEvent.Register<RichTextBox, FormattingEventArgs>(
            nameof(FormattingApplied),
            RoutingStrategies.Bubble);

    /// <summary>Provides the ParagraphBreakLineCount member.</summary>
    private const int ParagraphBreakLineCount = 2;

    /// <summary>Provides the DefaultImageOverlayHeight member.</summary>
    private const double DefaultImageOverlayHeight = 100D;

    /// <summary>Provides the CharacterWidthFontScale member.</summary>
    private const double CharacterWidthFontScale = 0.6D;

    /// <summary>Provides the DefaultCharacterWidth member.</summary>
    private const double DefaultCharacterWidth = 8.4D;

    /// <summary>Provides the LineHeightFontScale member.</summary>
    private const double LineHeightFontScale = 1.4D;

    /// <summary>Provides the DefaultLineHeight member.</summary>
    private const double DefaultLineHeight = 19.6D;

    /// <summary>Provides the SmallFontSize member.</summary>
    private const double SmallFontSize = 12D;

    /// <summary>Provides the MediumFontSize member.</summary>
    private const double MediumFontSize = 16D;

    /// <summary>Provides the LargeFontSize member.</summary>
    private const double LargeFontSize = 20D;

    /// <summary>Provides the WebPHeaderLength member.</summary>
    private const int WebPHeaderLength = 12;

    /// <summary>Provides the RiffSignatureLength member.</summary>
    private const int RiffSignatureLength = 4;

    /// <summary>Provides the WebPSignatureOffset member.</summary>
    private const int WebPSignatureOffset = 8;

    /// <summary>Provides the _propertyChangedHandlers member.</summary>
    private readonly Dictionary<AvaloniaProperty, Action> _propertyChangedHandlers;

    /// <summary>Provides the _undoStack member.</summary>
    private readonly Stack<RichTextHistoryEntry> _undoStack = new();

    /// <summary>Provides the _redoStack member.</summary>
    private readonly Stack<RichTextHistoryEntry> _redoStack = new();

    /// <summary>Provides the _defaultClipboardAdapter member.</summary>
    private readonly IRichTextClipboardAdapter _defaultClipboardAdapter =
        new RichTextMemoryClipboardAdapter();

    /// <summary>Provides the _editingTextBox member.</summary>
    private global::Avalonia.Controls.TextBox? _editingTextBox;

    /// <summary>Provides the _formattedPresenter member.</summary>
    private FormattedTextPresenter? _formattedPresenter;

    /// <summary>Scroll host for formatted display and image overlays.</summary>
    private global::Avalonia.Controls.ScrollViewer? _displayScrollViewer;

    /// <summary>Composed image layer used because text inlines do not render child visuals reliably.</summary>
    private Canvas? _imageOverlay;

    /// <summary>Provides the _contextMenu member.</summary>
    private ContextMenu? _contextMenu;

    /// <summary>Provides the _isUpdating member.</summary>
    private bool _isUpdating;

    /// <summary>Initializes a new instance of the <see cref="RichTextBox"/> class.</summary>
    public RichTextBox()
    {
        _propertyChangedHandlers = CreatePropertyChangedHandlers();
        Document = new();
        Selection = new(Document);
        CopyCommand = new RichTextCommand(_ => CopyCore(), _ => CanCopy);
        CutCommand = new RichTextCommand(_ => CutCore(), _ => CanCut);
        PasteCommand = new RichTextCommand(_ => PasteCore(), _ => CanPaste);
        UndoCommand = new RichTextCommand(_ => UndoCore(), _ => CanUndo);
        RedoCommand = new RichTextCommand(_ => RedoCore(), _ => CanRedo);
        SelectAllCommand = new RichTextCommand(_ => SelectAllCore(), _ => Document.Length > 0);
        ToggleBoldCommand = new RichTextCommand(
            _ => ApplyFormattingToSelection(TextFormatType.Bold),
            _ => CanApplyFormatting);
        ToggleItalicCommand = new RichTextCommand(
            _ => ApplyFormattingToSelection(TextFormatType.Italic),
            _ => CanApplyFormatting);
        ToggleUnderlineCommand = new RichTextCommand(
            _ => ApplyFormattingToSelection(TextFormatType.Underline),
            _ => CanApplyFormatting);
        ToggleStrikethroughCommand = new RichTextCommand(
            _ => ApplyFormattingToSelection(TextFormatType.Strikethrough),
            _ => CanApplyFormatting);
        ClearFormattingCommand = new RichTextCommand(
            _ => ClearFormattingCore(),
            _ => !IsReadOnlyInternal && IsFormattingEnabled && HasAnyFormatting());
        SetFontFamilyCommand = new RichTextCommand(
            parameter => SetSelectionFontFamily(parameter?.ToString() ?? string.Empty),
            parameter => CanApplyFormatting && !string.IsNullOrWhiteSpace(parameter?.ToString()));
        SetFontSizeCommand = new RichTextCommand(
            parameter =>
                SetSelectionFontSize(Convert.ToDouble(parameter, CultureInfo.InvariantCulture)),
            parameter => CanApplyFormatting && parameter is not null);
        SetForegroundCommand = new RichTextCommand(
            parameter => SetSelectionForeground((Color)parameter!),
            parameter => CanApplyFormatting && parameter is Color);
        SetHighlightCommand = new RichTextCommand(
            parameter => SetSelectionHighlight((Color)parameter!),
            parameter => CanApplyFormatting && parameter is Color);
        Focusable = true;

        AddHandler(
            DragDrop.DragOverEvent,
            OnDragOver,
            RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
            true);
        AddHandler(
            DragDrop.DropEvent,
            OnDrop,
            RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
            true);
        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel, true);
        _ = SetValue(DragDrop.AllowDropProperty, true);

        Text = string.Empty;
        Document.SetText(Text);
    }
}
