// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Drawing;
using System.Reactive.Disposables;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a text control that makes suggestions to users as they enter text using a keyboard. The app is notified when text has been changed by the user and is responsible for providing relevant suggestions for this control to display.
/// </summary>
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
public partial class AutoSuggestBox : ItemsControl, IIconControl
{
    /// <summary>
    /// Property for <see cref="OriginalItemsSource"/>.
    /// </summary>
    public static readonly DependencyProperty OriginalItemsSourceProperty = DependencyProperty.Register(
        nameof(OriginalItemsSource),
        typeof(IList),
        typeof(AutoSuggestBox),
        new PropertyMetadata(Array.Empty<object>()));

    /// <summary>
    /// Property for <see cref="IsSuggestionListOpen"/>.
    /// </summary>
    public static readonly DependencyProperty IsSuggestionListOpenProperty = DependencyProperty.Register(
        nameof(IsSuggestionListOpen),
        typeof(bool),
        typeof(AutoSuggestBox),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="Text"/>.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(AutoSuggestBox),
        new PropertyMetadata(string.Empty, TextPropertyChangedCallback));

    /// <summary>
    /// Property for <see cref="PlaceholderText"/>.
    /// </summary>
    public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
        nameof(PlaceholderText),
        typeof(string),
        typeof(AutoSuggestBox),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Property for <see cref="UpdateTextOnSelect"/>.
    /// </summary>
    public static readonly DependencyProperty UpdateTextOnSelectProperty = DependencyProperty.Register(
        nameof(UpdateTextOnSelect),
        typeof(bool),
        typeof(AutoSuggestBox),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="MaxSuggestionListHeight"/>.
    /// </summary>
    public static readonly DependencyProperty MaxSuggestionListHeightProperty = DependencyProperty.Register(
        nameof(MaxSuggestionListHeight),
        typeof(double),
        typeof(AutoSuggestBox),
        new PropertyMetadata(0d));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(AutoSuggestBox),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="FocusCommand"/>.
    /// </summary>
    public static readonly DependencyProperty FocusCommandProperty = DependencyProperty.Register(
        nameof(FocusCommand),
        typeof(ICommand),
        typeof(AutoSuggestBox),
        new PropertyMetadata(null));

    /// <summary>
    /// Routed event for <see cref="QuerySubmitted"/>.
    /// </summary>
    public static readonly RoutedEvent QuerySubmittedEvent = EventManager.RegisterRoutedEvent(
        nameof(QuerySubmitted),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs>),
        typeof(AutoSuggestBox));

    /// <summary>
    /// Routed event for <see cref="SuggestionChosen"/>.
    /// </summary>
    public static readonly RoutedEvent SuggestionChosenEvent = EventManager.RegisterRoutedEvent(
        nameof(SuggestionChosen),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs>),
        typeof(AutoSuggestBox));

    /// <summary>
    /// Routed event for <see cref="TextChanged"/>.
    /// </summary>
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>),
        typeof(AutoSuggestBox));

    /// <summary>
    /// The element text box.
    /// </summary>
    protected const string ElementTextBox = "PART_TextBox";

    /// <summary>
    /// The element suggestions popup.
    /// </summary>
    protected const string ElementSuggestionsPopup = "PART_SuggestionsPopup";

    /// <summary>
    /// The element suggestions list.
    /// </summary>
    protected const string ElementSuggestionsList = "PART_SuggestionsList";

    /// <summary>
    /// The text box.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected TextBox? TextBox;

    /// <summary>
    /// The suggestions popup.
    /// </summary>
    protected Popup SuggestionsPopup = null!;

    /// <summary>
    /// The suggestions list.
    /// </summary>
    protected ListView? SuggestionsList;
#pragma warning restore SA1401 // Fields should be private

    private readonly CompositeDisposable? _disposables = [];

    private bool _changingTextAfterSuggestionChosen;

    private bool _isChangedTextOutSideOfTextBox;

    private object? _selectedItem;

    private bool? _isHwndHookSubscribed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuggestBox"/> class.
    /// </summary>
    public AutoSuggestBox()
    {
        _disposables.Add(
            this.Events().Loaded.Subscribe(static e =>
        {
            var self = (AutoSuggestBox)e.Source;
            self.AcquireTemplateResources();
        }));

        _disposables.Add(
        this.Events().Unloaded.Subscribe(static e =>
        {
            var self = (AutoSuggestBox)e.Source;
            self.ReleaseTemplateResources();
        }));

        SetValue(FocusCommandProperty, ReactiveCommand.Create(Focus));
    }

    /// <summary>
    /// Occurs when the user submits a search query.
    /// </summary>
    public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted
    {
        add => AddHandler(QuerySubmittedEvent, value);
        remove => RemoveHandler(QuerySubmittedEvent, value);
    }

    /// <summary>
    /// Event occurs when the user selects an item from the recommended ones.
    /// </summary>
    public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen
    {
        add => AddHandler(SuggestionChosenEvent, value);
        remove => RemoveHandler(SuggestionChosenEvent, value);
    }

    /// <summary>
    /// Raised after the text content of the editable control component is updated.
    /// </summary>
    public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    /// <summary>
    /// Gets or sets your items here if you want to use the default filtering.
    /// </summary>
    public IList OriginalItemsSource
    {
        get => (IList)GetValue(OriginalItemsSourceProperty);
        set => SetValue(OriginalItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the drop-down portion of the <see cref="AutoSuggestBox"/> is open.
    /// </summary>
    public bool IsSuggestionListOpen
    {
        get => (bool)GetValue(IsSuggestionListOpenProperty);
        set => SetValue(IsSuggestionListOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the text that is shown in the control.
    /// </summary>
    /// <remarks>
    /// This property is not typically set in XAML.
    /// </remarks>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text to be displayed in the control.
    /// </summary>
    /// <remarks>
    /// The placeholder text to be displayed in the control. The default is an empty string.
    /// </remarks>
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum height for the drop-down portion of the <see cref="AutoSuggestBox"/> control.
    /// </summary>
    public double MaxSuggestionListHeight
    {
        get => (double)GetValue(MaxSuggestionListHeightProperty);
        set => SetValue(MaxSuggestionListHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether items in the view will trigger an update of the editable text part of the <see cref="AutoSuggestBox"/> when clicked.
    /// </summary>
    public bool UpdateTextOnSelect
    {
        get => (bool)GetValue(UpdateTextOnSelectProperty);
        set => SetValue(UpdateTextOnSelectProperty, value);
    }

    /// <summary>
    /// Gets or sets displayed <see cref="IconElement"/>.
    /// </summary>
    public IconElement? Icon
    {
        get => (IconElement?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets command used for focusing control.
    /// </summary>
    public ICommand FocusCommand => (ICommand)GetValue(FocusCommandProperty);

    /// <summary>
    /// Called when [apply template].
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TextBox = GetTemplateChild<TextBox>(ElementTextBox);
        SuggestionsPopup = GetTemplateChild<Popup>(ElementSuggestionsPopup);
        SuggestionsList = GetTemplateChild<ListView>(ElementSuggestionsList);
        _isHwndHookSubscribed = false;

        AcquireTemplateResources();
    }

    /// <inheritdoc cref="UIElement.Focus" />
    public new bool Focus() => TextBox?.Focus() == true;

    /// <summary>
    /// Gets the template child.
    /// </summary>
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

    /// <summary>
    /// Acquires the template resources.
    /// </summary>
    protected virtual void AcquireTemplateResources()
    {
        // Unsubscribe each handler before subscription, to prevent memory leak from double subscriptions.
        // Unsubscription is safe, even if event has never been subscribed to.
        if (TextBox != null)
        {
            TextBox.PreviewKeyDown -= TextBoxOnPreviewKeyDown;
            TextBox.PreviewKeyDown += TextBoxOnPreviewKeyDown;
            TextBox.TextChanged -= TextBoxOnTextChanged;
            TextBox.TextChanged += TextBoxOnTextChanged;
            TextBox.LostKeyboardFocus -= TextBoxOnLostKeyboardFocus;
            TextBox.LostKeyboardFocus += TextBoxOnLostKeyboardFocus;
        }

        if (SuggestionsList != null)
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

        if (_isHwndHookSubscribed == false)
        {
            var hwnd = (HwndSource)PresentationSource.FromVisual(this)!;
            hwnd.AddHook(Hook);
            _isHwndHookSubscribed = true;
        }
    }

    /// <summary>
    /// Releases the template resources.
    /// </summary>
    protected virtual void ReleaseTemplateResources()
    {
        if (TextBox != null)
        {
            TextBox.PreviewKeyDown -= TextBoxOnPreviewKeyDown;
            TextBox.TextChanged -= TextBoxOnTextChanged;
            TextBox.LostKeyboardFocus -= TextBoxOnLostKeyboardFocus;
        }

        if (SuggestionsList != null)
        {
            SuggestionsList.SelectionChanged -= SuggestionsListOnSelectionChanged;
            SuggestionsList.PreviewKeyDown -= SuggestionsListOnPreviewKeyDown;
            SuggestionsList.LostKeyboardFocus -= SuggestionsListOnLostKeyboardFocus;
            SuggestionsList.PreviewMouseLeftButtonUp -= SuggestionsListOnPreviewMouseLeftButtonUp;
        }

        if (_isHwndHookSubscribed == true && PresentationSource.FromVisual(this) is HwndSource source)
        {
            source.RemoveHook(Hook);
            _isHwndHookSubscribed = false;
        }
    }

    /// <summary>
    /// Method for <see cref="QuerySubmitted"/>.
    /// </summary>
    /// <param name="queryText">Currently submitted query text.</param>
    protected virtual void OnQuerySubmitted(string queryText)
    {
        var args = new AutoSuggestBoxQuerySubmittedEventArgs(QuerySubmittedEvent, this)
        {
            QueryText = queryText
        };

        RaiseEvent(args);
    }

    /// <summary>
    /// Method for <see cref="SuggestionChosen"/>.
    /// </summary>
    /// <param name="selectedItem">Currently selected item.</param>
    protected virtual void OnSuggestionChosen(object selectedItem)
    {
        var args = new AutoSuggestBoxSuggestionChosenEventArgs(SuggestionChosenEvent, this)
        {
            SelectedItem = selectedItem
        };

        RaiseEvent(args);

        if (UpdateTextOnSelect && !args.Handled)
        {
            UpdateTexBoxTextAfterSelection(selectedItem);
        }
    }

    /// <summary>
    /// Method for <see cref="TextChanged"/>.
    /// </summary>
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

        if (args is { Handled: false, Reason: AutoSuggestionBoxTextChangeReason.UserInput })
        {
            DefaultFiltering(text);
        }
    }

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

    private void TextBoxOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus is ListView)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);
    }

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

    private void SuggestionsListOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus is ListViewItem)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);
    }

    private void SuggestionsListOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is not Key.Enter)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);

        OnSelectedChanged(SuggestionsList!.SelectedItem);
    }

    private void SuggestionsListOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (SuggestionsList?.SelectedItem is not null)
        {
            return;
        }

        SetCurrentValue(IsSuggestionListOpenProperty, false);

        if (_selectedItem is not null)
        {
            OnSuggestionChosen(_selectedItem);
        }
    }

    private void SuggestionsListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SuggestionsList?.SelectedItem is null)
        {
            return;
        }

        OnSelectedChanged(SuggestionsList.SelectedItem);
    }

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

    private void OnSelectedChanged(object selectedObj)
    {
        OnSuggestionChosen(selectedObj);

        _selectedItem = selectedObj;
    }

    private void UpdateTexBoxTextAfterSelection(object selectedObj)
    {
        _changingTextAfterSuggestionChosen = true;

        TextBox?.SetCurrentValue(System.Windows.Controls.TextBox.TextProperty, GetStringFromObj(selectedObj));

        _changingTextAfterSuggestionChosen = false;
    }

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

            var found = splitText.All(key => itemText!.ToLower().Contains(key));

            if (found)
            {
                suitableItems?.Add(item);
            }
        }

        SetCurrentValue(ItemsSourceProperty, suitableItems);
    }

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
            text = obj as string ?? obj.ToString();
        }

        return text;
    }
}
