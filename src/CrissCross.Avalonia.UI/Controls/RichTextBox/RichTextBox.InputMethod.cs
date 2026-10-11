// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.TextInput;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Connects paper editing to the platform input method.</summary>
public partial class RichTextBox
{
    /// <summary>The input method client for the paper surface.</summary>
    private PagedInputMethodClient? _pagedInputMethodClient;

    /// <summary>Identifies properties that change platform text input availability.</summary>
    /// <param name="property">The changed property.</param>
    /// <returns>Whether the client must be requeried.</returns>
    private static bool IsInputMethodStateProperty(AvaloniaProperty property) => property == IsPageLayoutEnabledProperty
        || property == IsReadOnlyProperty || property == EditModeProperty || property == IsTextSelectionEnabledProperty
        || property == IsEnabledProperty || property == IsEffectivelyEnabledProperty;

    /// <summary>Installs input method lifecycle notifications.</summary>
    private void InitializeInputMethod()
    {
        _pagedInputMethodClient = new(this);
        AddHandler(TextInputMethodClientRequestedEvent, OnInputMethodClientRequested);
        LostFocus += (_, _) => _pagedInputMethodClient.Cancel();
        DetachedFromVisualTree += (_, _) => _pagedInputMethodClient.Cancel();
        LayoutUpdated += (_, _) => _pagedInputMethodClient.NotifyGeometry();
        PropertyChanged += OnInputMethodPropertyChanged;
        Document.TextChanged += (_, _) => _pagedInputMethodClient.NotifyText();
    }

    /// <summary>Supplies the client only while paper editing is available.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The client request.</param>
    private void OnInputMethodClientRequested(object? sender, TextInputMethodClientRequestedEventArgs e)
    {
        if (!CanUsePagedInputMethod())
        {
            return;
        }

        e.Client = _pagedInputMethodClient;
        e.Handled = true;
    }

    /// <summary>Updates or resets the platform composition after editor state changes.</summary>
    /// <param name="sender">The editor.</param>
    /// <param name="e">The changed property.</param>
    private void OnInputMethodPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == SelectionStartProperty || e.Property == SelectionEndProperty || e.Property == CaretIndexProperty)
        {
            _pagedInputMethodClient?.NotifySelection();
        }
        else if (IsInputMethodStateProperty(e.Property))
        {
            _pagedInputMethodClient?.Cancel();
            RaiseEvent(new(InputMethod.TextInputMethodClientRequeryRequestedEvent));
        }
    }

    /// <summary>Determines whether the paper surface can accept platform input.</summary>
    /// <returns>Whether platform input is available.</returns>
    private bool CanUsePagedInputMethod() => IsPageLayoutEnabled && !IsReadOnlyInternal && IsEffectivelyEnabled;

    /// <summary>Encodes preview text without advancing the pending insertion styles.</summary>
    /// <param name="text">The transient input.</param>
    /// <returns>The preview fragment.</returns>
    private string EncodePreeditText(string text)
    {
        var encoded = HtmlClipboardUtilities.EncodePlainText(text);
        if (_typingStyles.Count == 0 || _typingStylePosition != Math.Min(SelectionStart, SelectionEnd))
        {
            return encoded;
        }

        var style = new StringBuilder();
        foreach (var declaration in _typingStyles)
        {
            _ = style.Append(declaration.Key).Append(':').Append(declaration.Value).Append(';');
        }

        return $"<span style=\"{WebUtility.HtmlEncode(style.ToString())}\">{encoded}</span>";
    }

    /// <summary>Provides the platform IME contract without changing committed document content.</summary>
    /// <param name="owner">The paper editor.</param>
    private sealed class PagedInputMethodClient(RichTextBox owner) : TextInputMethodClient
    {
        /// <summary>The composition selection anchor.</summary>
        private int _compositionStart;

        /// <summary>The composition selection endpoint.</summary>
        private int _compositionEnd;

        /// <summary>Whether an uncommitted composition is active.</summary>
        private bool _composing;

        /// <inheritdoc/>
        public override Visual TextViewVisual => owner;

        /// <inheritdoc/>
        public override bool SupportsPreedit => true;

        /// <inheritdoc/>
        public override bool SupportsSurroundingText => true;

        /// <inheritdoc/>
        public override string SurroundingText => owner.Document.PlainText;

        /// <inheritdoc/>
        public override Rect CursorRectangle => owner._pagedPresenter is { } presenter && presenter.TransformToVisual(owner) is { } transform
            ? presenter.GetInputMethodCursorRectangle().TransformToAABB(transform)
            : default;

        /// <inheritdoc/>
        public override global::Avalonia.Input.TextInput.TextSelection Selection
        {
            get => new(owner.SelectionStart, owner.SelectionEnd);
            set => owner.SetPageSelection(Math.Clamp(value.Start, 0, owner.Document.Length), Math.Clamp(value.End, 0, owner.Document.Length));
        }

        /// <inheritdoc/>
        public override void ExecuteContextMenuAction(ContextMenuAction action)
        {
            var command = action switch
            {
                ContextMenuAction.Copy => owner.CopyCommand,
                ContextMenuAction.Cut => owner.CutCommand,
                ContextMenuAction.Paste => owner.PasteCommand,
                ContextMenuAction.SelectAll => owner.SelectAllCommand,
                _ => null,
            };
            if (command?.CanExecute(null) != true)
            {
                return;
            }

            command.Execute(null);
        }

        /// <inheritdoc/>
        public override void SetPreeditText(string? preeditText) => SetPreeditText(preeditText, null);

        /// <inheritdoc/>
        public override void SetPreeditText(string? preeditText, int? cursorPos)
        {
            if (!owner.CanUsePagedInputMethod())
            {
                PrepareCommit();
                return;
            }

            if (!_composing && !string.IsNullOrEmpty(preeditText))
            {
                _compositionStart = owner.SelectionStart;
                _compositionEnd = owner.SelectionEnd;
                _composing = true;
            }

            owner._pagedPresenter?.SetPreedit(
                preeditText,
                cursorPos,
                Math.Min(_compositionStart, _compositionEnd),
                Math.Abs(_compositionEnd - _compositionStart),
                owner.EncodePreeditText(preeditText ?? string.Empty));
            if (string.IsNullOrEmpty(preeditText))
            {
                _composing = false;
            }

            RaiseCursorRectangleChanged();
            owner._pagedPresenter?.UpdateLayout();
            owner._pagedPresenter?.BringIntoView(owner._pagedPresenter.GetInputMethodCursorRectangle());
        }

        /// <summary>Clears preedit before the document commits its single undoable replacement.</summary>
        public void PrepareCommit()
        {
            owner._pagedPresenter?.SetPreedit(null, null, 0);
            _composing = false;
        }

        /// <summary>Cancels transient input and requests a platform reset.</summary>
        public void Cancel()
        {
            PrepareCommit();
            RequestReset();
            RaiseCursorRectangleChanged();
        }

        /// <summary>Notifies the platform after the document changes.</summary>
        public void NotifyText()
        {
            if (_composing)
            {
                Cancel();
            }

            RaiseSurroundingTextChanged();
            RaiseSelectionChanged();
        }

        /// <summary>Notifies the platform after the selection changes.</summary>
        public void NotifySelection()
        {
            if (_composing && (owner.SelectionStart != _compositionStart || owner.SelectionEnd != _compositionEnd))
            {
                Cancel();
            }

            RaiseSelectionChanged();
            RaiseCursorRectangleChanged();
        }

        /// <summary>Notifies the platform after scrolling or page layout changes.</summary>
        public void NotifyGeometry() => RaiseCursorRectangleChanged();
    }
}
