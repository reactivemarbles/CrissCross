// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using CrissCross.WPF.UI.Input;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Customized window for notifications.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(MessageBox), "MessageBox.bmp")]
public class MessageBox : Window
{
    /// <summary>
    /// Property for <see cref="ShowTitle"/>.
    /// </summary>
    public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
        nameof(ShowTitle),
        typeof(bool),
        typeof(MessageBox),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="PrimaryButtonText"/>.
    /// </summary>
    public static readonly DependencyProperty PrimaryButtonTextProperty = DependencyProperty.Register(
        nameof(PrimaryButtonText),
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Property for <see cref="SecondaryButtonText"/>.
    /// </summary>
    public static readonly DependencyProperty SecondaryButtonTextProperty = DependencyProperty.Register(
        nameof(SecondaryButtonText),
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Property for <see cref="CloseButtonText"/>.
    /// </summary>
    public static readonly DependencyProperty CloseButtonTextProperty = DependencyProperty.Register(
        nameof(CloseButtonText),
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata("Close"));

    /// <summary>
    /// Property for <see cref="PrimaryButtonIcon"/>.
    /// </summary>
    public static readonly DependencyProperty PrimaryButtonIconProperty = DependencyProperty.Register(
        nameof(PrimaryButtonIcon),
        typeof(SymbolRegular),
        typeof(MessageBox),
        new PropertyMetadata(SymbolRegular.Empty));

    /// <summary>
    /// Property for <see cref="SecondaryButtonIcon"/>.
    /// </summary>
    public static readonly DependencyProperty SecondaryButtonIconProperty = DependencyProperty.Register(
        nameof(SecondaryButtonIcon),
        typeof(SymbolRegular),
        typeof(MessageBox),
        new PropertyMetadata(SymbolRegular.Empty));

    /// <summary>
    /// Property for <see cref="CloseButtonIcon"/>.
    /// </summary>
    public static readonly DependencyProperty CloseButtonIconProperty = DependencyProperty.Register(
        nameof(CloseButtonIcon),
        typeof(SymbolRegular),
        typeof(MessageBox),
        new PropertyMetadata(SymbolRegular.Empty));

    /// <summary>
    /// Property for <see cref="PrimaryButtonAppearance"/>.
    /// </summary>
    public static readonly DependencyProperty PrimaryButtonAppearanceProperty = DependencyProperty.Register(
        nameof(PrimaryButtonAppearance),
        typeof(ControlAppearance),
        typeof(MessageBox),
        new PropertyMetadata(ControlAppearance.Primary));

    /// <summary>
    /// Property for <see cref="SecondaryButtonAppearance"/>.
    /// </summary>
    public static readonly DependencyProperty SecondaryButtonAppearanceProperty = DependencyProperty.Register(
        nameof(SecondaryButtonAppearance),
        typeof(ControlAppearance),
        typeof(MessageBox),
        new PropertyMetadata(ControlAppearance.Secondary));

    /// <summary>
    /// Property for <see cref="CloseButtonAppearance"/>.
    /// </summary>
    public static readonly DependencyProperty CloseButtonAppearanceProperty = DependencyProperty.Register(
        nameof(CloseButtonAppearance),
        typeof(ControlAppearance),
        typeof(MessageBox),
        new PropertyMetadata(ControlAppearance.Secondary));

    /// <summary>
    /// Property for <see cref="IsPrimaryButtonEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsPrimaryButtonEnabled),
        typeof(bool),
        typeof(MessageBox),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="IsSecondaryButtonEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsSecondaryButtonEnabled),
        typeof(bool),
        typeof(MessageBox),
        new PropertyMetadata(true));

    /// <summary>
    /// Property for <see cref="TemplateButtonCommand"/>.
    /// </summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty = DependencyProperty.Register(
        nameof(TemplateButtonCommand),
        typeof(IRelayCommand),
        typeof(MessageBox),
        new PropertyMetadata(null));

    /// <summary>
    /// The TCS.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected TaskCompletionSource<MessageBoxResult>? Tcs;
#pragma warning restore SA1401 // Fields should be private

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBox"/> class.
    /// </summary>
    public MessageBox()
    {
        Topmost = true;
        SetValue(TemplateButtonCommandProperty, new RelayCommand<MessageBoxButton>(OnButtonClick));

        PreviewMouseDoubleClick += static (_, args) => args.Handled = true;

        Loaded += static (sender, _) =>
        {
            var self = (MessageBox)sender;
            self.OnLoaded();
        };
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value that determines whether to show the Title in <see cref="TitleBar"/>.
    /// </summary>
    public bool ShowTitle
    {
        get => (bool)GetValue(ShowTitleProperty);
        set => SetValue(ShowTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the primary button.
    /// </summary>
    public string PrimaryButtonText
    {
        get => (string)GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to be displayed on the secondary button.
    /// </summary>
    public string SecondaryButtonText
    {
        get => (string)GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the close button.
    /// </summary>
    public string CloseButtonText
    {
        get => (string)GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="SymbolRegular"/> on the primary button.
    /// </summary>
    public SymbolRegular PrimaryButtonIcon
    {
        get => (SymbolRegular)GetValue(PrimaryButtonIconProperty);
        set => SetValue(PrimaryButtonIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="SymbolRegular"/> on the secondary button.
    /// </summary>
    public SymbolRegular SecondaryButtonIcon
    {
        get => (SymbolRegular)GetValue(SecondaryButtonIconProperty);
        set => SetValue(SecondaryButtonIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="SymbolRegular"/> on the close button.
    /// </summary>
    public SymbolRegular CloseButtonIcon
    {
        get => (SymbolRegular)GetValue(CloseButtonIconProperty);
        set => SetValue(CloseButtonIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="ControlAppearance"/> on the primary button.
    /// </summary>
    public ControlAppearance PrimaryButtonAppearance
    {
        get => (ControlAppearance)GetValue(PrimaryButtonAppearanceProperty);
        set => SetValue(PrimaryButtonAppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="ControlAppearance"/> on the secondary button.
    /// </summary>
    public ControlAppearance SecondaryButtonAppearance
    {
        get => (ControlAppearance)GetValue(SecondaryButtonAppearanceProperty);
        set => SetValue(SecondaryButtonAppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="ControlAppearance"/> on the close button.
    /// </summary>
    public ControlAppearance CloseButtonAppearance
    {
        get => (ControlAppearance)GetValue(CloseButtonAppearanceProperty);
        set => SetValue(CloseButtonAppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the <see cref="MessageBox"/> primary button is enabled.
    /// </summary>
    public bool IsSecondaryButtonEnabled
    {
        get => (bool)GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the <see cref="MessageBox"/> secondary button is enabled.
    /// </summary>
    public bool IsPrimaryButtonEnabled
    {
        get => (bool)GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets command triggered after clicking the button on the Footer.
    /// </summary>
    public IRelayCommand TemplateButtonCommand => (IRelayCommand)GetValue(TemplateButtonCommandProperty);

    /// <summary>
    /// Shows this instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">$"Use {nameof(ShowDialogAsync)} instead.</exception>
    [Obsolete($"Use {nameof(ShowDialogAsync)} instead")]
    public new void Show() =>
        throw new InvalidOperationException($"Use {nameof(ShowDialogAsync)} instead");

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <returns>A bool.</returns>
    /// <exception cref="InvalidOperationException">$"Use {nameof(ShowDialogAsync)} instead.</exception>
    [Obsolete($"Use {nameof(ShowDialogAsync)} instead")]
    public new bool? ShowDialog() =>
        throw new InvalidOperationException($"Use {nameof(ShowDialogAsync)} instead");

    /// <summary>
    /// Closes this instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">$"Use {nameof(Close)} with MessageBoxResult instead.</exception>
    [Obsolete($"Use {nameof(Close)} with MessageBoxResult instead")]
    public new void Close() =>
        throw new InvalidOperationException($"Use {nameof(Close)} with MessageBoxResult instead");

    /// <summary>
    /// Displays a message box.
    /// </summary>
    /// <param name="showAsDialog">if set to <c>true</c> [show as dialog].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <see cref="MessageBoxResult" />.
    /// </returns>
    public async Task<MessageBoxResult> ShowDialogAsync(
        bool showAsDialog = true,
        CancellationToken cancellationToken = default)
    {
        Tcs = new TaskCompletionSource<MessageBoxResult>();
        var tokenRegistration = cancellationToken.Register(
            o => Tcs.TrySetCanceled((CancellationToken)o!),
            cancellationToken);

        try
        {
            RemoveTitleBarAndApplyMica();

            if (showAsDialog)
            {
                base.ShowDialog();
            }
            else
            {
                base.Show();
            }

            return await Tcs.Task;
        }
        finally
        {
#if NET6_0_OR_GREATER
            await tokenRegistration.DisposeAsync();
#else
            tokenRegistration.Dispose();
#endif
        }
    }

    /// <summary>
    /// Occurs after Loading event.
    /// </summary>
    protected virtual void OnLoaded()
    {
        var rootElement = (UIElement)GetVisualChild(0)!;

        ResizeToContentSize(rootElement);
        CenterWindowOnScreen();
    }

    /// <summary>
    /// Sets Width and Height.
    /// </summary>
    /// <param name="rootElement">The root element.</param>
    protected virtual void ResizeToContentSize(UIElement rootElement)
    {
        if (rootElement == null)
        {
            return;
        }

        var desiredSize = rootElement.DesiredSize;

        // left and right margin
        const double margin = 12.0 * 2;

        Width = desiredSize.Width + margin;
        Height = desiredSize.Height;

        ResizeWidth(rootElement);
        ResizeHeight(rootElement);
    }

    /// <summary>
    /// Raises the <see cref="E:Closing" /> event.
    /// </summary>
    /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (e?.Cancel == true)
        {
            return;
        }

        Tcs?.TrySetResult(MessageBoxResult.None);
    }

    /// <summary>
    /// Centers the window on screen.
    /// </summary>
    protected virtual void CenterWindowOnScreen()
    {
        // TODO MessageBox should be displayed on the window on which the application
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;

        Left = (screenWidth / 2) - (Width / 2);
        Top = (screenHeight / 2) - (Height / 2);
    }

    /// <summary>
    /// Occurs after the <see cref="MessageBoxButton" /> is clicked.
    /// </summary>
    /// <param name="button">The button.</param>
    protected virtual void OnButtonClick(MessageBoxButton button)
    {
        var result = button switch
        {
            MessageBoxButton.Primary => MessageBoxResult.Primary,
            MessageBoxButton.Secondary => MessageBoxResult.Secondary,
            _ => MessageBoxResult.None
        };

        Tcs?.TrySetResult(result);
        base.Close();
    }

    private void RemoveTitleBarAndApplyMica()
    {
        UnsafeNativeMethods.RemoveWindowTitlebarContents(this);
        WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
    }

    private void ResizeWidth(UIElement element)
    {
        if (Width <= MaxWidth)
        {
            return;
        }

        Width = MaxWidth;
        element.UpdateLayout();

        Height = element.DesiredSize.Height;

        if (Height > MaxHeight)
        {
            MaxHeight = Height;
        }
    }

    private void ResizeHeight(UIElement element)
    {
        if (Height <= MaxHeight)
        {
            return;
        }

        Height = MaxHeight;
        element.UpdateLayout();

        Width = element.DesiredSize.Width;

        if (Width > MaxWidth)
        {
            MaxWidth = Width;
        }
    }
}
