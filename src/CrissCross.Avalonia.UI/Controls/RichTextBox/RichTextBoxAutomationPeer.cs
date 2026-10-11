// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Exposes the rich editor through Avalonia's supported value and semantic-element contracts.</summary>
public sealed class RichTextBoxAutomationPeer : ControlAutomationPeer, IValueProvider
{
    /// <summary>The associated rich editor.</summary>
    private readonly RichTextBox _editor;

    /// <summary>The last rendered value announced to accessibility clients.</summary>
    private string _value;

    /// <summary>The last effective read-only state.</summary>
    private bool _isReadOnly;

    /// <summary>Initializes a new instance of the <see cref="RichTextBoxAutomationPeer"/> class.</summary>
    /// <param name="owner">The associated editor.</param>
    public RichTextBoxAutomationPeer(RichTextBox owner)
        : base(owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _editor = owner;
        _value = owner.Document.PlainText;
        _isReadOnly = owner.IsAutomationReadOnly;
        owner.Document.TextChanged += OnTextChanged;
        owner.SelectionChanged += OnSelectionChanged;
        owner.PropertyChanged += OnEditorPropertyChanged;
    }

    /// <summary>Occurs after the document selection or caret changes.</summary>
    public event EventHandler? TextSelectionChanged;

    /// <summary>Gets the effective read-only state, including display modes.</summary>
    public bool IsReadOnly => _editor.IsAutomationReadOnly;

    /// <summary>Gets rendered text rather than HTML source.</summary>
    public string Value => _editor.Document.PlainText;

    /// <summary>Gets the current whole-document range.</summary>
    public RichTextAutomationRange DocumentRange => new(_editor, 0, _editor.Document.Length);

    /// <summary>Gets the current selection, including a collapsed caret range.</summary>
    public RichTextAutomationRange SelectionRange => new(_editor, _editor.SelectionStart, _editor.SelectionEnd);

    /// <summary>Gets the associated document editor.</summary>
    internal RichTextBox Editor => _editor;

    /// <summary>Replaces the entire document with literal text.</summary>
    /// <param name="value">The literal replacement text.</param>
    public void SetValue(string? value)
    {
        EnsureEnabled();
        if (IsReadOnly)
        {
            throw new InvalidOperationException("The rich text document is read-only.");
        }

        _editor.Select(0, _editor.Document.Length);
        _editor.ReplaceSelection(value ?? string.Empty);
    }

    /// <summary>Gets a validated document-position range.</summary>
    /// <param name="start">The first insertion position.</param>
    /// <param name="length">The number of positions.</param>
    /// <returns>A range associated with this document.</returns>
    public RichTextAutomationRange GetRange(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, _editor.Document.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, _editor.Document.Length - start);
        return new(_editor, start, start + length);
    }

    /// <summary>Gets the insertion range nearest a top-level point.</summary>
    /// <param name="point">The top-level device-independent point.</param>
    /// <returns>A collapsed insertion range.</returns>
    public RichTextAutomationRange RangeFromPoint(Point point)
    {
        var offset = _editor.GetAutomationDocumentPosition(point);
        return new(_editor, offset, offset);
    }

    /// <inheritdoc/>
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Edit;

    /// <inheritdoc/>
    protected override string GetClassNameCore() => nameof(RichTextBox);

    /// <inheritdoc/>
    protected override string? GetPlaceholderTextCore() => _editor.Watermark;

    /// <inheritdoc/>
    protected override IReadOnlyList<AutomationPeer> GetChildrenCore()
    {
        var children = new List<AutomationPeer>(RichTextSemanticAutomationPeer.CreateChildren(_editor));
        children.AddRange(_editor.GetAutomationStories());
        return children;
    }

    /// <summary>Announces rendered content and semantic-tree changes.</summary>
    /// <param name="sender">The editor.</param>
    /// <param name="args">The text event.</param>
    private void OnTextChanged(object? sender, EventArgs args)
    {
        var value = Value;
        if (_value != value)
        {
            RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, _value, value);
            _value = value;
        }

        InvalidateChildren();
    }

    /// <summary>Announces a change to the effective editing mode.</summary>
    /// <param name="sender">The editor.</param>
    /// <param name="args">The property event.</param>
    private void OnEditorPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == RichTextBox.PageCountProperty || args.Property == RichTextBox.IsPageLayoutEnabledProperty)
        {
            InvalidateChildren();
        }

        var readOnly = IsReadOnly;
        if (_isReadOnly == readOnly)
        {
            return;
        }

        RaisePropertyChangedEvent(ValuePatternIdentifiers.IsReadOnlyProperty, _isReadOnly, readOnly);
        _isReadOnly = readOnly;
    }

    /// <summary>Announces document selection changes to application accessibility integrations.</summary>
    /// <param name="sender">The editor.</param>
    /// <param name="args">The routed event.</param>
    private void OnSelectionChanged(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args) => TextSelectionChanged?.Invoke(this, EventArgs.Empty);
}
