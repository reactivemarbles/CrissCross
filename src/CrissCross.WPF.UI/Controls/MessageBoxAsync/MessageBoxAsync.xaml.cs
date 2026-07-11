// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CP.BBCode.WPF;
using ReactiveUI;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace CrissCross.WPF.UI;

/// <summary>Interaction logic for MessageBox.xaml.</summary>
public partial class MessageBoxAsync : IListenForMessages
{
    /// <summary>Identifies the Buttons dependency property.</summary>
    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(
        nameof(Buttons),
        typeof(ObservableCollection<Button>),
        typeof(MessageBoxAsync));

    /// <summary>Stores the _closeOkCommand value.</summary>
    private readonly ReactiveCommand<Unit, MessageBoxResult>? _closeOkCommand;

    /// <summary>Stores the _custom0Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom0Command;

    /// <summary>Stores the _custom1Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom1Command;

    /// <summary>Stores the _custom2Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom2Command;

    /// <summary>Stores the _custom3Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom3Command;

    /// <summary>Stores the _custom4Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom4Command;

    /// <summary>Stores the _custom5Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom5Command;

    /// <summary>Stores the _custom6Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom6Command;

    /// <summary>Stores the _custom7Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom7Command;

    /// <summary>Stores the _custom8Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom8Command;

    /// <summary>Stores the _custom9Command value.</summary>
    private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom9Command;

    /// <summary>Stores the _customMessageBoxResult value.</summary>
    private CustomMessageBoxResult _customMessageBoxResult = CustomMessageBoxResult.None;

    /// <summary>Stores the _messageBoxResult value.</summary>
    private MessageBoxResult _messageBoxResult = MessageBoxResult.None;

    /// <summary>Stores the _cancelButton value.</summary>
    private Button? _cancelButton;

    /// <summary>Stores the _closeButton value.</summary>
    private Button? _closeButton;

    /// <summary>Stores the _noButton value.</summary>
    private Button? _noButton;

    /// <summary>Stores the _okbutton value.</summary>
    private Button? _okbutton;

    /// <summary>Stores the _yesButton value.</summary>
    private Button? _yesButton;

    /// <summary>Initializes a new instance of the <see cref="MessageBoxAsync"/> class.</summary>
    public MessageBoxAsync()
    {
        InitializeComponent();
        Visibility = Visibility.Collapsed;
        _closeOkCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.OK);
        CloseTrueCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.Yes);
        CloseFalseCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.No);
        CloseCommand = ReactiveCommand.Create(() => _messageBoxResult = MessageBoxResult.Cancel);
        _custom0Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom0);
        _custom1Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom1);
        _custom2Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom2);
        _custom3Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom3);
        _custom4Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom4);
        _custom5Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom5);
        _custom6Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom6);
        _custom7Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom7);
        _custom8Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom8);
        _custom9Command = ReactiveCommand.Create(() => _customMessageBoxResult = CustomMessageBoxResult.Custom9);

        Buttons = new([CloseButton]);
        ButtonsSource.ItemsSource = Buttons;
        Loaded += (_, _) =>
        {
            // Set up magic functions
            this.ListenForMessages(message => MessageBoxShow(message.Item1, message.Item2, message.Item3));
            this.ListenForCustomMessages(
                message =>
                MessageBoxShow(
                                     message.Item1,
                                     message.Item2,
                                     message.Item3,
                                     message.Item4,
                                     message.Item5,
                                     message.Item6,
                                     message.Item7,
                                     message.Rest.Item1,
                                     message.Rest.Item2,
                                     message.Rest.Item3,
                                     message.Rest.Item4));
        };
    }

    /// <summary>Gets or sets the dialog buttons.</summary>
    public ObservableCollection<Button> Buttons
    {
        get => (ObservableCollection<Button>)GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    /// <summary>Gets the close window command that sets the dialog result to a null value.</summary>
    public ICommand CloseCommand { get; }

    /// <summary>Gets the close window command that sets the dialog result to false.</summary>
    public ICommand CloseFalseCommand { get; }

    /// <summary>Gets the close window command that sets the dialog result to True.</summary>
    public ICommand CloseTrueCommand { get; }

    /// <summary>Gets the Cancel button.</summary>
    public Button CancelButton => _cancelButton ??= CreateDialogButton("Cancel", false, true, CloseCommand);

    /// <summary>Gets the Close button.</summary>
    public Button CloseButton => _closeButton ??= CreateDialogButton("Close", true, false, CloseCommand);

    /// <summary>Gets the Yes button.</summary>
    public Button YesButton => _yesButton ??= CreateDialogButton("Yes", true, false, CloseTrueCommand);

    /// <summary>Gets the No button.</summary>
    public Button NoButton => _noButton ??= CreateDialogButton("No", false, true, CloseFalseCommand);

    /// <summary>Gets the OK button.</summary>
    public Button OkButton => _okbutton ??= CreateDialogButton("Ok", true, false, _closeOkCommand);

    /// <summary>Gets the custom button0.</summary>
    /// <value>The custom button0.</value>
    public Button? CustomButton0 { get; private set; }

    /// <summary>Gets the custom button1.</summary>
    /// <value>The custom button1.</value>
    public Button? CustomButton1 { get; private set; }

    /// <summary>Gets the custom button2.</summary>
    /// <value>The custom button2.</value>
    public Button? CustomButton2 { get; private set; }

    /// <summary>Gets the custom button3.</summary>
    /// <value>The custom button3.</value>
    public Button? CustomButton3 { get; private set; }

    /// <summary>Gets the custom button4.</summary>
    /// <value>The custom button4.</value>
    public Button? CustomButton4 { get; private set; }

    /// <summary>Gets the custom button5.</summary>
    /// <value>The custom button5.</value>
    public Button? CustomButton5 { get; private set; }

    /// <summary>Gets the custom button6.</summary>
    /// <value>The custom button6.</value>
    public Button? CustomButton6 { get; private set; }

    /// <summary>Gets the custom button7.</summary>
    /// <value>The custom button7.</value>
    public Button? CustomButton7 { get; private set; }

    /// <summary>Gets the custom button8.</summary>
    /// <value>The custom button8.</value>
    public Button? CustomButton8 { get; private set; }

    /// <summary>Gets the custom button9.</summary>
    /// <value>The custom button9.</value>
    public Button? CustomButton9 { get; private set; }

    /// <summary>Creates the dialog button.</summary>
    /// <param name="content">The content.</param>
    /// <param name="isDefault">if set to <c>true</c> [is default].</param>
    /// <param name="isCancel">if set to <c>true</c> [is cancel].</param>
    /// <param name="command">The command.</param>
    /// <returns>A Button.</returns>
    private static Button CreateDialogButton(string content, bool isDefault, bool isCancel, ICommand? command) => new()
    {
        Content = content,
        Command = command,
        IsDefault = isDefault,
        IsCancel = isCancel,
        MinHeight = 21,
        MinWidth = 65,
        Margin = new(4, 0, 0, 0)
    };

    /// <summary>Messages the box show.</summary>
    /// <param name="bbcode">The bbcode.</param>
    /// <param name="title">The title.</param>
    /// <param name="custom0">The custom0.</param>
    /// <param name="custom1">The custom1.</param>
    /// <param name="custom2">The custom2.</param>
    /// <param name="custom3">The custom3.</param>
    /// <param name="custom4">The custom4.</param>
    /// <param name="custom5">The custom5.</param>
    /// <param name="custom6">The custom6.</param>
    /// <param name="custom7">The custom7.</param>
    /// <param name="custom8">The custom8.</param>
    /// <param name="custom9">The custom9.</param>
    /// <returns>A Value.</returns>
    private async Task<CustomMessageBoxResult> MessageBoxShow(string bbcode, string title, string custom0, string? custom1 = null, string? custom2 = null, string? custom3 = null, string? custom4 = null, string? custom5 = null, string? custom6 = null, string? custom7 = null, string? custom8 = null, string? custom9 = null)
    {
        if (custom0 is null)
        {
            throw new ArgumentNullException(nameof(custom0));
        }

        // If message box is already shown wait for it to be actioned
        while (Visibility == Visibility.Visible)
        {
            await Task.Delay(100).ConfigureAwait(true);
        }

        await Dispatcher.InvokeAsync(() =>
        {
            MessageTitle.Text = title;
            MessageContent.Content = new BBCodeBlock { BBCode = bbcode, Margin = new(0, 0, 0, 8) };
            Buttons.Clear();
            foreach (var button in GetButtons(custom0, custom1, custom2, custom3, custom4, custom5, custom6, custom7, custom8, custom9))
            {
                Buttons.Add(button);
            }

            Visibility = Visibility.Visible;
        });

        // Reset the result
        _customMessageBoxResult = CustomMessageBoxResult.None;

        // Wait for response
        var customMessageBoxResult = GetCustomMessageBoxResult();
        while (customMessageBoxResult == CustomMessageBoxResult.None)
        {
            await Task.Delay(100).ConfigureAwait(true);
            customMessageBoxResult = GetCustomMessageBoxResult();
        }

        await Dispatcher.InvokeAsync(() =>

        // hide the message box and return result
        Visibility = Visibility.Collapsed);
        return customMessageBoxResult;
    }

    /// <summary>Displays a dismiss-able message-box. Click outside of the message area to dismiss.</summary>
    /// <param name="bbcode">The text. Use BBCode to style the text.</param>
    /// <param name="title">The title.</param>
    /// <param name="button">The buttons to show.</param>
    /// <returns>Task of MessageBoxResult.</returns>
    private async Task<MessageBoxResult> MessageBoxShow(string bbcode, string title = "", MessageBoxButton button = MessageBoxButton.OK)
    {
        // If message box is already shown wait for it to be actioned
        while (Visibility == Visibility.Visible)
        {
            await Task.Delay(100).ConfigureAwait(true);
        }

        await Dispatcher.InvokeAsync(() =>
        {
            MessageTitle.Text = title;
            MessageContent.Content = new BBCodeBlock { BBCode = bbcode, Margin = new(0, 0, 0, 8) };
            Buttons.Clear();
            foreach (var button in GetButtons(button))
            {
                Buttons.Add(button);
            }

            Visibility = Visibility.Visible;
        });

        // Reset the result
        _messageBoxResult = MessageBoxResult.None;

        // Wait for response
        var messageBoxResult = GetMessageBoxResult();
        while (messageBoxResult == MessageBoxResult.None)
        {
            await Task.Delay(100).ConfigureAwait(true);
            messageBoxResult = GetMessageBoxResult();
        }

        // hide the message box and return result
        await Dispatcher.InvokeAsync(() => Visibility = Visibility.Collapsed);
        return messageBoxResult;
    }

    /// <summary>Provides the GetButtons member.</summary>
    /// <param name="custom0">The custom0 value.</param>
    /// <param name="custom1">The custom1 value.</param>
    /// <param name="custom2">The custom2 value.</param>
    /// <param name="custom3">The custom3 value.</param>
    /// <param name="custom4">The custom4 value.</param>
    /// <param name="custom5">The custom5 value.</param>
    /// <param name="custom6">The custom6 value.</param>
    /// <param name="custom7">The custom7 value.</param>
    /// <param name="custom8">The custom8 value.</param>
    /// <param name="custom9">The custom9 value.</param>
    /// <returns>The result.</returns>
    private List<Button> GetButtons(string custom0, string? custom1 = null, string? custom2 = null, string? custom3 = null, string? custom4 = null, string? custom5 = null, string? custom6 = null, string? custom7 = null, string? custom8 = null, string? custom9 = null)
    {
        var result = new List<Button>();
        var buttonDefinitions = new (string? Text, ICommand? Command, Action<Button> Assign)[]
        {
            (custom0, _custom0Command, button => CustomButton0 = button),
            (custom1, _custom1Command, button => CustomButton1 = button),
            (custom2, _custom2Command, button => CustomButton2 = button),
            (custom3, _custom3Command, button => CustomButton3 = button),
            (custom4, _custom4Command, button => CustomButton4 = button),
            (custom5, _custom5Command, button => CustomButton5 = button),
            (custom6, _custom6Command, button => CustomButton6 = button),
            (custom7, _custom7Command, button => CustomButton7 = button),
            (custom8, _custom8Command, button => CustomButton8 = button),
            (custom9, _custom9Command, button => CustomButton9 = button)
        };

        foreach (var (text, command, assign) in buttonDefinitions)
        {
            if (text is null)
            {
                break;
            }

            var button = CreateDialogButton(text, result.Count == 0, false, command);
            assign(button);
            result.Add(button);
        }

        return result;
    }

    /// <summary>Gets the buttons.</summary>
    /// <param name="button">The button.</param>
    /// <returns>A IEnumerable of Buttons.</returns>
    private List<Button> GetButtons(MessageBoxButton button)
    {
        var result = new List<Button>();
        var owner = this;
        switch (button)
        {
            case MessageBoxButton.OK:
                {
                    result.Add(owner.OkButton);
                    break;
                }

            case MessageBoxButton.OKCancel:
                {
                    result.Add(owner.OkButton);
                    result.Add(owner.CancelButton);
                    break;
                }

            case MessageBoxButton.YesNo:
                {
                    result.Add(owner.YesButton);
                    result.Add(owner.NoButton);
                    break;
                }

            case MessageBoxButton.YesNoCancel:
                {
                    result.Add(owner.YesButton);
                    result.Add(owner.NoButton);
                    result.Add(owner.CancelButton);
                    break;
                }
        }

        return result;
    }

    /// <summary>Gets the current custom message box result.</summary>
    /// <returns>The current custom message box result.</returns>
    private CustomMessageBoxResult GetCustomMessageBoxResult() => _customMessageBoxResult;

    /// <summary>Gets the current message box result.</summary>
    /// <returns>The current message box result.</returns>
    private MessageBoxResult GetMessageBoxResult() => _messageBoxResult;
}
