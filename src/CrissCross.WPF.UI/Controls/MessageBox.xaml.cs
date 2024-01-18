// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive;
using System.Windows;
using System.Windows.Input;
using CP.BBCode.WPF;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Wpf.Ui.Controls;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace CrissCross.WPF.UI
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml.
    /// </summary>
    public partial class MessageBox : IListenForMessages, ICanShowMessages
    {
        /// <summary>
        /// Identifies the Buttons dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(nameof(Buttons), typeof(IEnumerable<Button>), typeof(ModernWindow));

        private readonly ReactiveCommand<Unit, MessageBoxResult>? _closeOkCommand;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom0Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom1Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom2Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom3Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom4Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom5Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom6Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom7Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom8Command;
        private readonly ReactiveCommand<Unit, CustomMessageBoxResult>? _custom9Command;
        private CustomMessageBoxResult _customMessageBoxResult = CustomMessageBoxResult.None;
        private MessageBoxResult _messageBoxResult = MessageBoxResult.None;
        private Button? _cancelButton;
        private Button? _closeButton;
        private Button? _noButton;
        private Button? _okbutton;
        private Button? _yesButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        public MessageBox()
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

            Buttons = new[] { CloseButton };
            ButtonsSource.ItemsSource = Buttons;
            this.Events().Loaded.Subscribe(_ =>
            {
                // Set up magic functions
                this.ListenForMessages(message => MessageBoxShow(message.Item1, message.Item2, message.Item3));
                ////this.ListenForCustomMessages(
                ////    async message =>
                ////    await MessageBoxShow(
                ////                         message.Item1,
                ////                         message.Item2,
                ////                         message.Item3,
                ////                         message.Item4,
                ////                         message.Item5,
                ////                         message.Item6,
                ////                         message.Item7,
                ////                         message.Rest.Item1,
                ////                         message.Rest.Item2,
                ////                         message.Rest.Item3,
                ////                         message.Rest.Item4).ConfigureAwait(false));
            });
        }

        /// <summary>
        /// Gets or sets the dialog buttons.
        /// </summary>
        public IEnumerable<Button> Buttons
        {
            get => (IEnumerable<Button>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        /// <summary>
        /// Gets the close window command that sets the dialog result to a null value.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Gets the close window command that sets the dialog result to false.
        /// </summary>
        public ICommand CloseFalseCommand { get; }

        /// <summary>
        /// Gets the close window command that sets the dialog result to True.
        /// </summary>
        public ICommand CloseTrueCommand { get; }

        /// <summary>
        /// Gets the Cancel button.
        /// </summary>
        public Button CancelButton => _cancelButton ??= CreateDialogButton("Cancel", false, true, CloseCommand);

        /// <summary>
        /// Gets the Close button.
        /// </summary>
        public Button CloseButton => _closeButton ??= CreateDialogButton("Close", true, false, CloseCommand);

        /// <summary>
        /// Gets the Yes button.
        /// </summary>
        public Button YesButton => _yesButton ??= CreateDialogButton("Yes", true, false, CloseTrueCommand);

        /// <summary>
        /// Gets the No button.
        /// </summary>
        public Button NoButton => _noButton ??= CreateDialogButton("No", false, true, CloseFalseCommand);

        /// <summary>
        /// Gets the OK button.
        /// </summary>
        public Button OkButton => _okbutton ??= CreateDialogButton("Ok", true, false, _closeOkCommand);

        /// <summary>
        /// Gets the custom button0.
        /// </summary>
        /// <value>The custom button0.</value>
        public Button? CustomButton0 { get; private set; }

        /// <summary>
        /// Gets the custom button1.
        /// </summary>
        /// <value>The custom button1.</value>
        public Button? CustomButton1 { get; private set; }

        /// <summary>
        /// Gets the custom button2.
        /// </summary>
        /// <value>The custom button2.</value>
        public Button? CustomButton2 { get; private set; }

        /// <summary>
        /// Gets the custom button3.
        /// </summary>
        /// <value>The custom button3.</value>
        public Button? CustomButton3 { get; private set; }

        /// <summary>
        /// Gets the custom button4.
        /// </summary>
        /// <value>The custom button4.</value>
        public Button? CustomButton4 { get; private set; }

        /// <summary>
        /// Gets the custom button5.
        /// </summary>
        /// <value>The custom button5.</value>
        public Button? CustomButton5 { get; private set; }

        /// <summary>
        /// Gets the custom button6.
        /// </summary>
        /// <value>The custom button6.</value>
        public Button? CustomButton6 { get; private set; }

        /// <summary>
        /// Gets the custom button7.
        /// </summary>
        /// <value>The custom button7.</value>
        public Button? CustomButton7 { get; private set; }

        /// <summary>
        /// Gets the custom button8.
        /// </summary>
        /// <value>The custom button8.</value>
        public Button? CustomButton8 { get; private set; }

        /// <summary>
        /// Gets the custom button9.
        /// </summary>
        /// <value>The custom button9.</value>
        public Button? CustomButton9 { get; private set; }

        /// <summary>
        /// Creates the dialog button.
        /// </summary>
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
            Margin = new Thickness(4, 0, 0, 0)
        };

        /// <summary>
        /// Messages the box show.
        /// </summary>
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
            if (custom0 == null)
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
                MessageContent.Content = new BBCodeBlock { BBCode = bbcode, Margin = new Thickness(0, 0, 0, 8) };
                Buttons = GetButtons(custom0, custom1, custom2, custom3, custom4, custom5, custom6, custom7, custom8, custom9);
                Visibility = Visibility.Visible;
            });

            // Reset the result
            _customMessageBoxResult = CustomMessageBoxResult.None;

            // Wait for response
            while (_customMessageBoxResult == CustomMessageBoxResult.None)
            {
                await Task.Delay(100).ConfigureAwait(true);
            }

            await Dispatcher.InvokeAsync(() =>

            // hide the message box and return result
            Visibility = Visibility.Collapsed);
            return _customMessageBoxResult;
        }

        /// <summary>
        /// Displays a dismiss-able message-box. Click outside of the message area to dismiss.
        /// </summary>
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
                MessageContent.Content = new BBCodeBlock { BBCode = bbcode, Margin = new Thickness(0, 0, 0, 8) };
                Buttons = GetButtons(button);
                Visibility = Visibility.Visible;
            });

            // Reset the result
            _messageBoxResult = MessageBoxResult.None;

            // Wait for response
            while (_messageBoxResult == MessageBoxResult.None)
            {
                await Task.Delay(100).ConfigureAwait(true);
            }

            // hide the message box and return result
            await Dispatcher.InvokeAsync(() => Visibility = Visibility.Collapsed);
            return _messageBoxResult;
        }

        private IEnumerable<Button> GetButtons(string custom0, string? custom1 = null, string? custom2 = null, string? custom3 = null, string? custom4 = null, string? custom5 = null, string? custom6 = null, string? custom7 = null, string? custom8 = null, string? custom9 = null)
        {
            var owner = this;
            owner.CustomButton0 = CreateDialogButton(custom0, true, false, _custom0Command);
            yield return owner.CustomButton0;
            if (custom1 != null)
            {
                owner.CustomButton1 = CreateDialogButton(custom1, false, false, _custom1Command);
                yield return owner.CustomButton1;
                if (custom2 != null)
                {
                    owner.CustomButton2 = CreateDialogButton(custom2, false, false, _custom2Command);
                    yield return owner.CustomButton2;
                    if (custom3 != null)
                    {
                        owner.CustomButton3 = CreateDialogButton(custom3, false, false, _custom3Command);
                        yield return owner.CustomButton3;
                        if (custom4 != null)
                        {
                            owner.CustomButton4 = CreateDialogButton(custom4, false, false, _custom4Command);
                            yield return owner.CustomButton4;
                            if (custom5 != null)
                            {
                                owner.CustomButton5 = CreateDialogButton(custom5, false, false, _custom5Command);
                                yield return owner.CustomButton5;
                                if (custom6 != null)
                                {
                                    owner.CustomButton6 = CreateDialogButton(custom6, false, false, _custom6Command);
                                    yield return owner.CustomButton6;
                                    if (custom7 != null)
                                    {
                                        owner.CustomButton7 = CreateDialogButton(custom7, false, false, _custom7Command);
                                        yield return owner.CustomButton7;
                                        if (custom8 != null)
                                        {
                                            owner.CustomButton8 = CreateDialogButton(custom8, false, false, _custom8Command);
                                            yield return owner.CustomButton8;
                                            if (custom9 != null)
                                            {
                                                owner.CustomButton9 = CreateDialogButton(custom9, false, false, _custom9Command);
                                                yield return owner.CustomButton9;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>A IEnumerable of Buttons.</returns>
        private IEnumerable<Button> GetButtons(MessageBoxButton button)
        {
            var owner = this;
            switch (button)
            {
                case MessageBoxButton.OK:
                    yield return owner.OkButton;
                    break;

                case MessageBoxButton.OKCancel:
                    yield return owner.OkButton;
                    yield return owner.CancelButton;
                    break;

                case MessageBoxButton.YesNo:
                    yield return owner.YesButton;
                    yield return owner.NoButton;
                    break;

                case MessageBoxButton.YesNoCancel:
                    yield return owner.YesButton;
                    yield return owner.NoButton;
                    yield return owner.CancelButton;
                    break;
            }
        }
    }
}
