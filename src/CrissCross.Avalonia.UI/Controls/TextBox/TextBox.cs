// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Input;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a text input control that supports placeholder text, an optional icon, and a clear button, with
/// additional customization options for user interaction.
/// </summary>
/// <remarks>The TextBox control extends the standard Avalonia TextBox by providing features such as configurable
/// placeholder text, an optional icon with customizable placement, and a clear button that can be enabled or disabled.
/// The clear button appears when the control is focused and contains text, allowing users to quickly clear the input.
/// The control also exposes commands for template button interactions and supports enabling or disabling text
/// selection. These features make it suitable for scenarios where enhanced user experience and input customization are
/// required.</remarks>
public class TextBox : global::Avalonia.Controls.TextBox
{
    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<TextBox, object?>(
        nameof(Icon));

    /// <summary>Property for <see cref="IconPlacement"/>.</summary>
    public static readonly StyledProperty<ElementPlacement> IconPlacementProperty = AvaloniaProperty.Register<TextBox, ElementPlacement>(
        nameof(IconPlacement),
        ElementPlacement.Left);

    /// <summary>Property for <see cref="PlaceholderText"/>.</summary>
    public static new readonly StyledProperty<string> PlaceholderTextProperty = AvaloniaProperty.Register<TextBox, string>(
        nameof(PlaceholderText),
        string.Empty);

    /// <summary>Property for <see cref="PlaceholderEnabled"/>.</summary>
    public static readonly StyledProperty<bool> PlaceholderEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(PlaceholderEnabled),
        true);

    /// <summary>Property for <see cref="CurrentPlaceholderEnabled"/>.</summary>
    public static readonly StyledProperty<bool> CurrentPlaceholderEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(CurrentPlaceholderEnabled),
        true);

    /// <summary>Property for <see cref="ClearButtonEnabled"/>.</summary>
    public static readonly StyledProperty<bool> ClearButtonEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(ClearButtonEnabled),
        true);

    /// <summary>Property for <see cref="ShowClearButton"/>.</summary>
    public static readonly StyledProperty<bool> ShowClearButtonProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(ShowClearButton),
        false);

    /// <summary>Property for <see cref="IsTextSelectionEnabled"/>.</summary>
    public static readonly StyledProperty<bool> IsTextSelectionEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(IsTextSelectionEnabled),
        false);

    /// <summary>Property for <see cref="TemplateButtonCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> TemplateButtonCommandProperty = AvaloniaProperty.Register<TextBox, ICommand?>(
        nameof(TemplateButtonCommand));

    /// <summary>Initializes a new instance of the <see cref="TextBox"/> class.</summary>
    public TextBox()
    {
        TemplateButtonCommand = ReactiveCommand.Create<string?>(OnTemplateButtonClick);
        CurrentPlaceholderEnabled = PlaceholderEnabled;
    }

    /// <summary>Gets or sets displayed icon.</summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets which side the icon should be placed on.</summary>
    public ElementPlacement IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    /// <summary>Gets or sets the value.</summary>
    public new string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>Gets or sets the PlaceholderEnabled value.</summary>
    public bool PlaceholderEnabled
    {
        get => GetValue(PlaceholderEnabledProperty);
        set => SetValue(PlaceholderEnabledProperty, value);
    }

    /// <summary>Gets the CurrentPlaceholderEnabled value.</summary>
    public bool CurrentPlaceholderEnabled
    {
        get => GetValue(CurrentPlaceholderEnabledProperty);
        protected set => SetValue(CurrentPlaceholderEnabledProperty, value);
    }

    /// <summary>Gets or sets the ClearButtonEnabled value.</summary>
    public bool ClearButtonEnabled
    {
        get => GetValue(ClearButtonEnabledProperty);
        set => SetValue(ClearButtonEnabledProperty, value);
    }

    /// <summary>Gets the ShowClearButton value.</summary>
    public bool ShowClearButton
    {
        get => GetValue(ShowClearButtonProperty);
        protected set => SetValue(ShowClearButtonProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether text selection is enabled.</summary>
    public bool IsTextSelectionEnabled
    {
        get => GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    /// <summary>Gets the command triggered when clicking the button.</summary>
    public ICommand? TemplateButtonCommand
    {
        get => GetValue(TemplateButtonCommandProperty);
        private set => SetValue(TemplateButtonCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null)
        {
            return;
        }

        if (change.Property == TextProperty)
        {
            OnTextChanged();
        }
        else if (change.Property == PlaceholderEnabledProperty)
        {
            OnPlaceholderEnabledChanged();
        }
    }

    /// <summary>Called when text changes.</summary>
    protected virtual void OnTextChanged()
    {
        SetPlaceholderTextVisibility();
        RevealClearButton();
    }

    /// <summary>Sets the placeholder text visibility.</summary>
    protected void SetPlaceholderTextVisibility()
    {
        var text = Text ?? string.Empty;

        if (PlaceholderEnabled)
        {
            if (CurrentPlaceholderEnabled && text.Length > 0)
            {
                _ = SetValue(CurrentPlaceholderEnabledProperty, false);
            }

            if (!CurrentPlaceholderEnabled && text.Length < 1)
            {
                _ = SetValue(CurrentPlaceholderEnabledProperty, true);
            }
        }
        else if (CurrentPlaceholderEnabled)
        {
            _ = SetValue(CurrentPlaceholderEnabledProperty, false);
        }
    }

    /// <inheritdoc />
    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);

        CaretIndex = (Text ?? string.Empty).Length;
        RevealClearButton();
    }

    /// <inheritdoc />
    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        HideClearButton();
    }

    /// <summary>Reveals the clear button by <see cref="ShowClearButton"/> property.</summary>
    protected void RevealClearButton()
    {
        if (!ClearButtonEnabled || !IsFocused)
        {
            return;
        }

        _ = SetValue(ShowClearButtonProperty, (Text ?? string.Empty).Length > 0);
    }

    /// <summary>Hides the clear button by <see cref="ShowClearButton"/> property.</summary>
    protected void HideClearButton()
    {
        if (!ClearButtonEnabled || IsFocused || !ShowClearButton)
        {
            return;
        }

        _ = SetValue(ShowClearButtonProperty, false);
    }

    /// <summary>Triggered when the user clicks the clear text button.</summary>
    protected virtual void OnClearButtonClick()
    {
        if ((Text ?? string.Empty).Length == 0)
        {
            return;
        }

        Text = string.Empty;
    }

    /// <summary>Triggered by clicking a button in the control template.</summary>
    /// <param name="parameter">The parameter.</param>
    protected virtual void OnTemplateButtonClick(string? parameter)
    {
        Debug.WriteLine($"INFO: {typeof(TextBox)} button clicked", "CrissCross.Avalonia.UI.TextBox");
        OnClearButtonClick();
    }

    /// <summary>Called when placeholder enabled property changes.</summary>
    protected virtual void OnPlaceholderEnabledChanged() => SetPlaceholderTextVisibility();
}
