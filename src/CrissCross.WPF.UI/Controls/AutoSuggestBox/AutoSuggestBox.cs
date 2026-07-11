// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents a text control that makes suggestions to users as they enter text using a keyboard. The app is notified when text has been changed by the user and is responsible for providing relevant suggestions for this control to display.</summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:AutoSuggestBox x:Name="AutoSuggestBox" PlaceholderText="Search"&gt;
///     &lt;ui:AutoSuggestBox.Icon&gt;
///         &lt;ui:IconSourceElement&gt;
///             &lt;ui:SymbolIconSource Symbol="Search24" /&gt;
///         &lt;/ui:IconSourceElement&gt;
///     &lt;/ui:AutoSuggestBox.Icon&gt;
/// &lt;/ui:AutoSuggestBox&gt;
/// </code>
/// </example>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(AutoSuggestBox), "AutoSuggestBox.bmp")]
[TemplatePart(Name = ElementTextBox, Type = typeof(TextBox))]
[TemplatePart(Name = ElementSuggestionsPopup, Type = typeof(Popup))]
[TemplatePart(Name = ElementSuggestionsList, Type = typeof(ListView))]
public class AutoSuggestBox : ItemsControl, IIconControl
{
    /// <summary>Property for <see cref="OriginalItemsSource"/>.</summary>
    public static readonly DependencyProperty OriginalItemsSourceProperty = DependencyProperty.Register(
        nameof(OriginalItemsSource),
        typeof(IList),
        typeof(AutoSuggestBox),
        new PropertyMetadata(Array.Empty<object>()));

    /// <summary>Property for <see cref="IsSuggestionListOpen"/>.</summary>
    public static readonly DependencyProperty IsSuggestionListOpenProperty = DependencyProperty.Register(
        nameof(IsSuggestionListOpen),
        typeof(bool),
        typeof(AutoSuggestBox),
        new PropertyMetadata(false));

    /// <summary>Property for <see cref="Text"/>.</summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(AutoSuggestBox),
        new PropertyMetadata(string.Empty, TextPropertyChangedCallback));

    /// <summary>Property for <see cref="PlaceholderText"/>.</summary>
    public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
        nameof(PlaceholderText),
        typeof(string),
        typeof(AutoSuggestBox),
        new PropertyMetadata(string.Empty));

    /// <summary>Property for <see cref="UpdateTextOnSelect"/>.</summary>
    public static readonly DependencyProperty UpdateTextOnSelectProperty = DependencyProperty.Register(
        nameof(UpdateTextOnSelect),
        typeof(bool),
        typeof(AutoSuggestBox),
        new PropertyMetadata(true));

    /// <summary>Property for <see cref="MaxSuggestionListHeight"/>.</summary>
    public static readonly DependencyProperty MaxSuggestionListHeightProperty = DependencyProperty.Register(
        nameof(MaxSuggestionListHeight),
        typeof(double),
        typeof(AutoSuggestBox),
        new PropertyMetadata(0d));

    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(AutoSuggestBox),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="FocusCommand"/>.</summary>
    public static readonly DependencyProperty FocusCommandProperty = DependencyProperty.Register(
        nameof(FocusCommand),
        typeof(ICommand),
        typeof(AutoSuggestBox),
        new PropertyMetadata(null));

    /// <summary>Routed event for <see cref="QuerySubmitted"/>.</summary>
    public static readonly RoutedEvent QuerySubmittedEvent = EventManager.RegisterRoutedEvent(
        nameof(QuerySubmitted),
        RoutingStrategy.Bubble,
        typeof(EventHandler<AutoSuggestBoxQuerySubmittedEventArgs>),
        typeof(AutoSuggestBox));

    /// <summary>Routed event for <see cref="SuggestionChosen"/>.</summary>
    public static readonly RoutedEvent SuggestionChosenEvent = EventManager.RegisterRoutedEvent(
        nameof(SuggestionChosen),
        RoutingStrategy.Bubble,
        typeof(EventHandler<AutoSuggestBoxSuggestionChosenEventArgs>),
        typeof(AutoSuggestBox));

    /// <summary>Routed event for <see cref="TextChanged"/>.</summary>
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(EventHandler<AutoSuggestBoxTextChangedEventArgs>),
        typeof(AutoSuggestBox));

    /// <summary>The element text box.</summary>
    protected const string ElementTextBox = "PART_TextBox";

    /// <summary>The element suggestions popup.</summary>
    protected const string ElementSuggestionsPopup = "PART_SuggestionsPopup";

    /// <summary>The element suggestions list.</summary>
    protected const string ElementSuggestionsList = "PART_SuggestionsList";

    /// <summary>Stores the _changingTextAfterSuggestionChosen value.</summary>
    private bool _changingTextAfterSuggestionChosen;

    /// <summary>Stores the _isChangedTextOutSideOfTextBox value.</summary>
    private bool _isChangedTextOutSideOfTextBox;

    /// <summary>Stores the _selectedItem value.</summary>
    private object? _selectedItem;

    /// <summary>Stores the _isHwndHookSubscribed value.</summary>
    private bool? _isHwndHookSubscribed;

    /// <summary>Initializes a new instance of the <see cref="AutoSuggestBox"/> class.</summary>
    public AutoSuggestBox()
    {
        Loaded += static (sender, _) =>
        {
            var self = (AutoSuggestBox)sender;
            self.AcquireTemplateResources();
        };

        Unloaded += static (sender, _) =>
        {
            var self = (AutoSuggestBox)sender;
            self.ReleaseTemplateResources();
        };

        SetValue(FocusCommandProperty, ReactiveCommand.Create(Focus));
    }

    /// <summary>Occurs when the user submits a search query.</summary>
    public event EventHandler<AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted
    {
        add => AddHandler(QuerySubmittedEvent, value);
        remove => RemoveHandler(QuerySubmittedEvent, value);
    }

    /// <summary>Event occurs when the user selects an item from the recommended ones.</summary>
    public event EventHandler<AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen
    {
        add => AddHandler(SuggestionChosenEvent, value);
        remove => RemoveHandler(SuggestionChosenEvent, value);
    }

    /// <summary>Raised after the text content of the editable control component is updated.</summary>
    public event EventHandler<AutoSuggestBoxTextChangedEventArgs> TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    /// <summary>Gets or sets your items here if you want to use the default filtering.</summary>
    public IList OriginalItemsSource
    {
        get => (IList)GetValue(OriginalItemsSourceProperty);
        set => SetValue(OriginalItemsSourceProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the drop-down portion of the <see cref="AutoSuggestBox"/> is open.</summary>
    public bool IsSuggestionListOpen
    {
        get => (bool)GetValue(IsSuggestionListOpenProperty);
        set => SetValue(IsSuggestionListOpenProperty, value);
    }

    /// <summary>Gets or sets the text that is shown in the control.</summary>
    /// <remarks>
    /// This property is not typically set in XAML.
    /// </remarks>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets or sets the placeholder text to be displayed in the control.</summary>
    /// <remarks>
    /// The placeholder text to be displayed in the control. The default is an empty string.
    /// </remarks>
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>Gets or sets the maximum height for the drop-down portion of the <see cref="AutoSuggestBox"/> control.</summary>
    public double MaxSuggestionListHeight
    {
        get => (double)GetValue(MaxSuggestionListHeightProperty);
        set => SetValue(MaxSuggestionListHeightProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether items in the view will trigger an update of the editable text part of the <see cref="AutoSuggestBox"/> when clicked.</summary>
    public bool UpdateTextOnSelect
    {
        get => (bool)GetValue(UpdateTextOnSelectProperty);
        set => SetValue(UpdateTextOnSelectProperty, value);
    }

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    public IconElement? Icon
    {
        get => (IconElement?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets command used for focusing control.</summary>
    public ICommand FocusCommand => (ICommand)GetValue(FocusCommandProperty);

    /// <summary>Gets or sets the text box.</summary>
    private TextBox? TextBox { get; set; }

    /// <summary>Gets or sets the suggestions list.</summary>
    private ListView? SuggestionsList { get; set; }

    /// <summary>Called when [apply template].</summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TextBox = GetTemplateChild<TextBox>(ElementTextBox);
        _ = GetTemplateChild<Popup>(ElementSuggestionsPopup);
        SuggestionsList = GetTemplateChild<ListView>(ElementSuggestionsList);
        _isHwndHookSubscribed = false;

        AcquireTemplateResources();
    }

    /// <summary>Sets focus to the editable text box inside the control.</summary>
    /// <returns><c>true</c> when focus is applied to the inner text box; otherwise, <c>false</c>.</returns>
    public new bool Focus() => TextBox?.Focus() == true;

    /// <summary>Gets the template child.</summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="name">The name.</param>
    /// <returns>The child.</returns>
    /// <exception cref="ArgumentNullException">name.</exception>
    protected T GetTemplateChild<T>(string name)
        where T : DependencyObject
    {
        if (GetTemplateChild(name) is not T dependencyObject)
        {
            throw new ArgumentNullException(name);
        }

        return dependencyObject;
    }

    /// <summary>Acquires the template resources.</summary>
    protected virtual void AcquireTemplateResources()
    {
        // Unsubscribe each handler before subscription, to prevent memory leak from double subscriptions.
        // Unsubscription is safe, even if event has never been subscribed to.
        if (TextBox is not null)
        {
            TextBox.PreviewKeyDown -= TextBoxOnPreviewKeyDown;
            TextBox.PreviewKeyDown += TextBoxOnPreviewKeyDown;
            TextBox.TextChanged -= TextBoxOnTextChanged;
            TextBox.TextChanged += TextBoxOnTextChanged;
            TextBox.LostKeyboardFocus -= TextBoxOnLostKeyboardFocus;
            TextBox.LostKeyboardFocus += TextBoxOnLostKeyboardFocus;
        }

        if (SuggestionsList is not null)
        {
            SuggestionsList.SelectionChanged -= SuggestionsListOnSelectionChanged;
            SuggestionsList.SelectionChanged += SuggestionsListOnSelectionChanged;
            SuggestionsList.PreviewKeyDown -= SuggestionsListOnPreviewKeyDown;
            SuggestionsList.PreviewKeyDown += SuggestionsListOnPreviewKeyDown;
            SuggestionsList.LostKeyboardFocus -= SuggestionsListOnLostKeyboardFocus;
            SuggestionsList.LostKeyboardFocus += SuggestionsListOnLostKeyboardFocus;
            SuggestionsList.PreviewMouseLeftButtonUp -= SuggestionsListOnPreviewMouseLeftButtonUp;
            SuggestionsList.PreviewMouseLeftButtonUp += SuggestionsListOnPreviewMouseLeftButtonUp;
        }

        if (_isHwndHookSubscribed != false)
        {
            return;
        }

        var hwnd = (HwndSource)PresentationSource.FromVisual(this)!;
        hwnd.AddHook(Hook);
        _isHwndHookSubscribed = true;
    }

    /// <summary>Releases the template resources.</summary>
    protected virtual void ReleaseTemplateResources()
    {
        if (TextBox is not null)
        {
            TextBox.PreviewKeyDown -= TextBoxOnPreviewKeyDown;
            TextBox.TextChanged -= TextBoxOnTextChanged;
            TextBox.LostKeyboardFocus -= TextBoxOnLostKeyboardFocus;
        }

        if (SuggestionsList is not null)
        {
            SuggestionsList.SelectionChanged -= SuggestionsListOnSelectionChanged;
            SuggestionsList.PreviewKeyDown -= SuggestionsListOnPreviewKeyDown;
            SuggestionsList.LostKeyboardFocus -= SuggestionsListOnLostKeyboardFocus;
            SuggestionsList.PreviewMouseLeftButtonUp -= SuggestionsListOnPreviewMouseLeftButtonUp;
        }

        if (_isHwndHookSubscribed != true || PresentationSource.FromVisual(this) is not HwndSource source)
        {
            return;
        }

        source.RemoveHook(Hook);
        _isHwndHookSubscribed = false;
    }

    /// <summary>Method for <see cref="QuerySubmitted"/>.</summary>
    /// <param name="queryText">Currently submitted query text.</param>
    protected virtual void OnQuerySubmitted(string queryText)
    {
        var args = new AutoSuggestBoxQuerySubmittedEventArgs(QuerySubmittedEvent, this)
        {
            QueryText = queryText
        };

        RaiseEvent(args);
    }

    /// <summary>Method for <see cref="SuggestionChosen"/>.</summary>
    /// <param name="selectedItem">Currently selected item.</param>
    protected virtual void OnSuggestionChosen(object selectedItem)
    {
        var args = new AutoSuggestBoxSuggestionChosenEventArgs(SuggestionChosenEvent, this)
        {
            SelectedItem = selectedItem
        };

        RaiseEvent(args);

        if (!UpdateTextOnSelect || args.Handled)
        {
            return;
        }

        UpdateTexBoxTextAfterSelection(selectedItem);
    }

    /// <summary>Method for <see cref="TextChanged"/>.</summary>
    /// <param name="reason">Data for the text changed event.</param>
    /// <param name="text">Changed text.</param>
    protected virtual void OnTextChanged(AutoSuggestionBoxTextChangeReason reason, string text)
    {
        var args = new AutoSuggestBoxTextChangedEventArgs(TextChangedEvent, this)
        {
            Reason = reason,
            Text = text
        };

        RaiseEvent(args);

        if (args.Handled || args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
        {
            return;
        }

        DefaultFiltering(text);
    }

    /// <summary>Provides the TextPropertyChangedCallback member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void TextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (AutoSuggestBox)d;
        var newText = (string)e.NewValue;

        if (self.TextBox is null)
        {
            return;
        }

        if (self.TextBox.Text == newText)
        {
            return;
        }

        self._isChangedTextOutSideOfTextBox = true;

        self.TextBox.SetCurrentValue(System.Windows.Controls.TextBox.TextProperty, newText);

        self._isChangedTextOutSideOfTextBox = false;
    }

    /// <summary>Provides the TextBoxOnPreviewKeyDown member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void TextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape)
        {
            SetCurrentValue(IsSuggestionListOpenProperty, false);

            return;
        }

        if (e.Key is Key.Enter)
        {
            SetCurrentValue(IsSuggestionListOpenProperty, false);

            OnQuerySubmitted(TextBox!.Text);

            return;
        }

        if (e.Key is not Key.Down || !IsSuggestionListOpen)
        {
            return;
        }

        _ = SuggestionsList?.Focus();
    }

    /// <summary>Provides the TextBoxOnLostKeyboardFocus member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void TextBoxOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus is ListView)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);
    }

    /// <summary>Provides the TextBoxOnTextChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
    {
        var changeReason = AutoSuggestionBoxTextChangeReason.UserInput;

        if (_changingTextAfterSuggestionChosen)
        {
            changeReason = AutoSuggestionBoxTextChangeReason.SuggestionChosen;
        }

        if (_isChangedTextOutSideOfTextBox)
        {
            changeReason = AutoSuggestionBoxTextChangeReason.ProgrammaticChange;
        }

        OnTextChanged(changeReason, TextBox!.Text);

        SuggestionsList?.SetCurrentValue(Selector.SelectedItemProperty, null);

        if (changeReason is not AutoSuggestionBoxTextChangeReason.UserInput)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, true);
    }

    /// <summary>Provides the SuggestionsListOnLostKeyboardFocus member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void SuggestionsListOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus is ListViewItem)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);
    }

    /// <summary>Provides the SuggestionsListOnPreviewKeyDown member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void SuggestionsListOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is not Key.Enter)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);

        OnSelectedChanged(SuggestionsList!.SelectedItem);
    }

    /// <summary>Provides the SuggestionsListOnPreviewMouseLeftButtonUp member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void SuggestionsListOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (SuggestionsList?.SelectedItem is not null)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);

        if (_selectedItem is null)
        {
            return;
        }

        OnSuggestionChosen(_selectedItem);
    }

    /// <summary>Provides the SuggestionsListOnSelectionChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void SuggestionsListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SuggestionsList?.SelectedItem is null)
        {
            return;
        }

        OnSelectedChanged(SuggestionsList.SelectedItem);
    }

    /// <summary>Provides the Hook member.</summary>
    /// <param name="hwnd">The hwnd value.</param>
    /// <param name="msg">The msg value.</param>
    /// <param name="wparam">The wparam value.</param>
    /// <param name="lparam">The lparam value.</param>
    /// <param name="handled">The handled value.</param>
    /// <returns>The result.</returns>
    private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        if (!IsSuggestionListOpen)
        {
            return IntPtr.Zero;
        }

        var message = (User32.WM)msg;

        if (message is User32.WM.NCACTIVATE or User32.WM.WINDOWPOSCHANGED)
        {
            SetCurrentValue(IsSuggestionListOpenProperty, false);
        }

        return IntPtr.Zero;
    }

    /// <summary>Provides the OnSelectedChanged member.</summary>
    /// <param name="selectedObj">The selectedObj value.</param>
    private void OnSelectedChanged(object selectedObj)
    {
        OnSuggestionChosen(selectedObj);

        _selectedItem = selectedObj;
    }

    /// <summary>Provides the UpdateTexBoxTextAfterSelection member.</summary>
    /// <param name="selectedObj">The selectedObj value.</param>
    private void UpdateTexBoxTextAfterSelection(object selectedObj)
    {
        _changingTextAfterSuggestionChosen = true;

        TextBox?.SetCurrentValue(System.Windows.Controls.TextBox.TextProperty, GetStringFromObj(selectedObj));

        _changingTextAfterSuggestionChosen = false;
    }

    /// <summary>Provides the DefaultFiltering member.</summary>
    /// <param name="text">The text value.</param>
    private void DefaultFiltering(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            SetCurrentValue(ItemsSourceProperty, OriginalItemsSource);

            return;
        }

        var suitableItems = new List<object?>();
        var splitText = text.ToLower().Split(' ');

        for (var i = 0; i < OriginalItemsSource.Count; i++)
        {
            var item = OriginalItemsSource[i];
            var itemText = GetStringFromObj(item!);

            var found = splitText.All(key => itemText!.Contains(key, StringComparison.OrdinalIgnoreCase));

            if (found)
            {
                suitableItems?.Add(item);
            }
        }

        SetCurrentValue(ItemsSourceProperty, suitableItems);
    }

    /// <summary>Provides the GetStringFromObj member.</summary>
    /// <param name="obj">The obj value.</param>
    /// <returns>The result.</returns>
    private string? GetStringFromObj(object obj)
    {
        var text = string.Empty;

        if (!string.IsNullOrEmpty(DisplayMemberPath))
        {
            // Maybe it needs some optimization?
            if (obj.GetType().GetProperty(DisplayMemberPath)?.GetValue(obj) is string value)
            {
                text = value;
            }
        }

        if (string.IsNullOrEmpty(text))
        {
            text = (obj as string) ?? obj.ToString();
        }

        return text;
    }
}
