// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls.Shapes;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Presents regional busy state over arbitrary content without replacing the content layout.</summary>
public class BusyOverlay : ContentView
{
    /// <summary>Bindable property for <see cref="Operation"/>.</summary>
    public static readonly BindableProperty OperationProperty = BindableProperty.Create(
        nameof(Operation),
        typeof(BusyOperation),
        typeof(BusyOverlay),
        propertyChanged: static (bindable, _, newValue) => OnOperationChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="IsBusy"/>.</summary>
    public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(
        nameof(IsBusy),
        typeof(bool),
        typeof(BusyOverlay),
        propertyChanged: static (bindable, _, _) => ((BusyOverlay)bindable).RefreshAccessibility());

    /// <summary>Provides default spacing inside the busy card.</summary>
    private const double CardSpacing = 8;

    /// <summary>Provides the default busy card maximum width.</summary>
    private const double CardMaximumWidth = 420;

    /// <summary>Provides the default busy card padding.</summary>
    private const double CardPadding = 20;

    /// <summary>Provides the default busy card corner radius.</summary>
    private const double CardCornerRadius = 12;

    /// <summary>Provides the default overlay padding.</summary>
    private const double OverlayPadding = 16;

    /// <summary>Provides the default activity indicator size.</summary>
    private const double ActivitySize = 44;

    /// <summary>Provides the default operation title font size.</summary>
    private const double TitleFontSize = 16;

    /// <summary>Provides the default cancel button minimum width.</summary>
    private const double CancelButtonMinimumWidth = 96;

    /// <summary>Provides fallback accessibility text when no operation is active.</summary>
    private const string IdleDescription = "Idle";

    /// <summary>Provides fallback operation title text.</summary>
    private const string FallbackTitle = "Working";

    /// <summary>Provides the semantic resource key for accent elements.</summary>
    private const string AccentColorResourceKey = "CrissCrossAccentColor";

    /// <summary>Provides the semantic resource key for accent text.</summary>
    private const string AccentTextColorResourceKey = "CrissCrossAccentTextColor";

    /// <summary>Provides the semantic resource key for muted text.</summary>
    private const string MutedTextColorResourceKey = "CrissCrossMutedTextColor";

    /// <summary>Provides the semantic resource key for the busy overlay scrim.</summary>
    private const string OverlayColorResourceKey = "CrissCrossOverlayColor";

    /// <summary>Provides the semantic resource key for card surfaces.</summary>
    private const string SurfaceColorResourceKey = "CrissCrossSurfaceColor";

    /// <summary>Provides the semantic resource key for primary text.</summary>
    private const string TextColorResourceKey = "CrissCrossTextColor";

    /// <summary>Tracks the activity indicator in the applied default template.</summary>
    private ActivityIndicator? _activity;

    /// <summary>Initializes a new instance of the <see cref="BusyOverlay"/> class.</summary>
    public BusyOverlay()
    {
        ControlTemplate = new(CreateDefaultTemplate);
        RefreshAccessibility();
    }

    /// <summary>Gets or sets the busy operation displayed by the overlay.</summary>
    public BusyOperation? Operation
    {
        get => (BusyOperation?)GetValue(OperationProperty);
        set => SetValue(OperationProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the overlay is visible.</summary>
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    /// <summary>Runs the operation changed operation.</summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnOperationChanged(BindableObject bindable, object newValue)
    {
        if (bindable is not BusyOverlay overlay)
        {
            return;
        }

        overlay.SetValue(IsBusyProperty, newValue is BusyOperation { IsActive: true });
        overlay.RefreshAccessibility();
    }

    /// <summary>Creates the default template that preserves user content beneath a busy-only blocking layer.</summary>
    /// <returns>The default template root.</returns>
    private Grid CreateDefaultTemplate()
    {
        var operation = Operation;
        var isDeterminate = operation?.IsDeterminate == true;
        var isCancellable = operation?.IsCancellable == true;
        var message = operation?.Message ?? string.Empty;
        var title = string.IsNullOrWhiteSpace(operation?.Title) ? FallbackTitle : operation!.Title;
        var presenter = new ContentPresenter();
        var activity = CreateActivityIndicator(isDeterminate);
        var progress = CreateProgressBar(operation, isDeterminate);
        var titleLabel = CreateTitleLabel(title);
        var messageLabel = CreateMessageLabel(message);
        var cancelButton = CreateCancelButton(operation, isCancellable);
        var card = CreateBusyCard(activity, progress, titleLabel, messageLabel, cancelButton);
        var root = new Grid();
        root.Children.Add(presenter);
        root.Children.Add(CreateBlockingOverlay(card));
        return root;
    }

    /// <summary>Creates the indeterminate activity indicator.</summary>
    /// <param name="isDeterminate">A value indicating whether the operation has determinate progress.</param>
    /// <returns>The activity indicator.</returns>
    private ActivityIndicator CreateActivityIndicator(bool isDeterminate)
    {
        var activity = new ActivityIndicator { HeightRequest = ActivitySize };
        activity.HorizontalOptions = LayoutOptions.Center;
        activity.IsRunning = IsBusy && !isDeterminate;
        activity.IsVisible = !isDeterminate;
        activity.WidthRequest = ActivitySize;
        activity.SetDynamicResource(ActivityIndicator.ColorProperty, AccentColorResourceKey);
        activity.SetBinding(
            ActivityIndicator.IsVisibleProperty,
            new Binding(
                $"{nameof(Operation)}.{nameof(BusyOperation.IsDeterminate)}",
                converter: InvertedBooleanConverter.Instance,
                source: this));
        _activity = activity;
        return activity;
    }

    /// <summary>Creates the determinate progress indicator.</summary>
    /// <param name="operation">The current operation snapshot.</param>
    /// <param name="isDeterminate">A value indicating whether the operation has determinate progress.</param>
    /// <returns>The progress bar.</returns>
    private ProgressBar CreateProgressBar(BusyOperation? operation, bool isDeterminate)
    {
        var progress = new ProgressBar { HorizontalOptions = LayoutOptions.Fill };
        progress.IsVisible = isDeterminate;
        progress.Progress = operation?.Progress ?? 0D;
        progress.SetDynamicResource(ProgressBar.ProgressColorProperty, AccentColorResourceKey);
        progress.SetBinding(
            ProgressBar.IsVisibleProperty,
            new Binding($"{nameof(Operation)}.{nameof(BusyOperation.IsDeterminate)}", source: this));

        var progressBinding = new Binding($"{nameof(Operation)}.{nameof(BusyOperation.Progress)}", source: this);
        progressBinding.FallbackValue = 0D;
        progressBinding.TargetNullValue = 0D;
        progress.SetBinding(ProgressBar.ProgressProperty, progressBinding);
        return progress;
    }

    /// <summary>Creates the operation title label.</summary>
    /// <param name="title">The initial operation title.</param>
    /// <returns>The title label.</returns>
    private Label CreateTitleLabel(string title)
    {
        var label = new Label { FontAttributes = FontAttributes.Bold };
        label.FontSize = TitleFontSize;
        label.HorizontalTextAlignment = TextAlignment.Center;
        label.Text = title;
        label.SetDynamicResource(Label.TextColorProperty, TextColorResourceKey);

        var textBinding = new Binding($"{nameof(Operation)}.{nameof(BusyOperation.Title)}", source: this);
        textBinding.FallbackValue = FallbackTitle;
        textBinding.TargetNullValue = FallbackTitle;
        label.SetBinding(Label.TextProperty, textBinding);
        return label;
    }

    /// <summary>Creates the optional operation message label.</summary>
    /// <param name="message">The initial operation message.</param>
    /// <returns>The message label.</returns>
    private Label CreateMessageLabel(string message)
    {
        var label = new Label { HorizontalTextAlignment = TextAlignment.Center };
        label.IsVisible = !string.IsNullOrWhiteSpace(message);
        label.Text = message;
        label.SetDynamicResource(Label.TextColorProperty, MutedTextColorResourceKey);

        var messageBinding = new Binding($"{nameof(Operation)}.{nameof(BusyOperation.Message)}", source: this);
        messageBinding.FallbackValue = string.Empty;
        messageBinding.TargetNullValue = string.Empty;
        label.SetBinding(Label.TextProperty, messageBinding);
        var visibilityBinding = new Binding(
            $"{nameof(Operation)}.{nameof(BusyOperation.Message)}",
            converter: HasTextConverter.Instance,
            source: this);
        label.SetBinding(
            Label.IsVisibleProperty,
            visibilityBinding);
        return label;
    }

    /// <summary>Creates the optional operation cancel button.</summary>
    /// <param name="operation">The current operation snapshot.</param>
    /// <param name="isCancellable">A value indicating whether cancellation is available.</param>
    /// <returns>The cancel button.</returns>
    private Button CreateCancelButton(BusyOperation? operation, bool isCancellable)
    {
        var button = new Button { Command = operation?.CancelCommand };
        button.HorizontalOptions = LayoutOptions.Center;
        button.IsVisible = isCancellable;
        button.MinimumWidthRequest = CancelButtonMinimumWidth;
        button.Text = "Cancel";
        button.SetDynamicResource(Button.BackgroundColorProperty, AccentColorResourceKey);
        button.SetDynamicResource(Button.TextColorProperty, AccentTextColorResourceKey);
        button.SetBinding(
            Button.CommandProperty,
            new Binding($"{nameof(Operation)}.{nameof(BusyOperation.CancelCommand)}", source: this));
        button.SetBinding(
            Button.IsVisibleProperty,
            new Binding($"{nameof(Operation)}.{nameof(BusyOperation.IsCancellable)}", source: this));
        return button;
    }

    /// <summary>Creates the centered card that presents busy details.</summary>
    /// <param name="activity">The activity indicator.</param>
    /// <param name="progress">The progress bar.</param>
    /// <param name="title">The title label.</param>
    /// <param name="message">The message label.</param>
    /// <param name="cancelButton">The cancel button.</param>
    /// <returns>The busy card.</returns>
    private Border CreateBusyCard(
        ActivityIndicator activity,
        ProgressBar progress,
        Label title,
        Label message,
        Button cancelButton)
    {
        var content = new VerticalStackLayout { Spacing = CardSpacing };
        content.Children.Add(activity);
        content.Children.Add(progress);
        content.Children.Add(title);
        content.Children.Add(message);
        content.Children.Add(cancelButton);

        var shape = new RoundRectangle { CornerRadius = new(CardCornerRadius) };

        var card = new Border { HorizontalOptions = LayoutOptions.Center };
        card.MaximumWidthRequest = CardMaximumWidth;
        card.Padding = new(CardPadding);
        card.StrokeShape = shape;
        card.VerticalOptions = LayoutOptions.Center;
        card.Content = content;
        card.SetDynamicResource(VisualElement.BackgroundColorProperty, SurfaceColorResourceKey);
        return card;
    }

    /// <summary>Creates the overlay layer that blocks covered content while busy.</summary>
    /// <param name="card">The busy card.</param>
    /// <returns>The blocking overlay layer.</returns>
    private Grid CreateBlockingOverlay(Border card)
    {
        var overlay = new Grid { HorizontalOptions = LayoutOptions.Fill };
        overlay.InputTransparent = false;
        overlay.IsVisible = IsBusy;
        overlay.Padding = new(OverlayPadding);
        overlay.VerticalOptions = LayoutOptions.Fill;
        overlay.Children.Add(card);
        overlay.SetDynamicResource(VisualElement.BackgroundColorProperty, OverlayColorResourceKey);
        overlay.SetBinding(IsVisibleProperty, new Binding(nameof(IsBusy), source: this));
        return overlay;
    }

    /// <summary>Updates semantic description from the current busy state.</summary>
    private void RefreshAccessibility()
    {
        if (_activity is not null)
        {
            _activity.IsRunning = IsBusy && Operation?.IsDeterminate != true;
        }

        if (!IsBusy)
        {
            SemanticProperties.SetDescription(this, IdleDescription);
            return;
        }

        var operation = Operation;
        var title = string.IsNullOrWhiteSpace(operation?.Title) ? FallbackTitle : operation!.Title;
        var message = operation?.Message;
        var description = string.IsNullOrWhiteSpace(message) ? title : $"{title} {message}";
        SemanticProperties.SetDescription(this, description);
    }

    /// <summary>Converts non-empty text to visible state.</summary>
    private sealed class HasTextConverter : IValueConverter
    {
        /// <summary>Gets the shared converter instance.</summary>
        internal static readonly HasTextConverter Instance = new();

        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) =>
            !string.IsNullOrWhiteSpace(value as string);

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>Converts boolean values to their inverse.</summary>
    private sealed class InvertedBooleanConverter : IValueConverter
    {
        /// <summary>Gets the shared converter instance.</summary>
        internal static readonly InvertedBooleanConverter Instance = new();

        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) =>
            value is not true;

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
