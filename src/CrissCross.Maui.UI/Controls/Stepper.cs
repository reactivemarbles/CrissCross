// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays progress through a sequence of steps.</summary>
public class Stepper : ContentView
{
    /// <summary>Bindable property for <see cref="StepperState"/>.</summary>
    public static readonly BindableProperty StepperStateProperty = BindableProperty.Create(
        nameof(StepperState),
        typeof(StepperState),
        typeof(Stepper),
        propertyChanged: static (view, _, _) => ((Stepper)view).Refresh());

    /// <summary>Bindable property for <see cref="StepCommand"/>.</summary>
    public static readonly BindableProperty StepCommandProperty = BindableProperty.Create(
        nameof(StepCommand),
        typeof(ICommand),
        typeof(Stepper),
        propertyChanged: static (view, _, _) => ((Stepper)view).Refresh());

    /// <summary>Provides default spacing between rendered steps.</summary>
    private const double StepSpacing = 8;

    /// <summary>Provides default spacing between command buttons.</summary>
    private const double ActionSpacing = 8;

    /// <summary>Provides the default width for step status glyphs.</summary>
    private const double GlyphWidth = 24;

    /// <summary>Provides the progress label font size.</summary>
    private const double ProgressFontSize = 13;

    /// <summary>Provides vertical default content padding.</summary>
    private const double VerticalPadding = 4;

    /// <summary>Provides the previous navigation offset.</summary>
    private const int PreviousStepOffset = -1;

    /// <summary>Provides the next navigation offset.</summary>
    private const int NextStepOffset = 1;

    /// <summary>Provides the primary text semantic resource key.</summary>
    private const string TextColorResourceKey = "CrissCrossTextColor";

    /// <summary>Provides the muted text semantic resource key.</summary>
    private const string MutedTextColorResourceKey = "CrissCrossMutedTextColor";

    /// <summary>Provides the accent fill semantic resource key.</summary>
    private const string AccentColorResourceKey = "CrissCrossAccentColor";

    /// <summary>Provides the accent text semantic resource key.</summary>
    private const string AccentTextColorResourceKey = "CrissCrossAccentTextColor";

    /// <summary>Provides the neutral surface semantic resource key.</summary>
    private const string NeutralSurfaceColorResourceKey = "CrissCrossNeutralSurfaceColor";

    /// <summary>Provides the success semantic resource key.</summary>
    private const string SuccessColorResourceKey = "CrissCrossSuccessColor";

    /// <summary>Provides the caution semantic resource key.</summary>
    private const string CautionColorResourceKey = "CrissCrossCautionColor";

    /// <summary>Provides the danger semantic resource key.</summary>
    private const string DangerColorResourceKey = "CrissCrossDangerColor";

    /// <summary>Displays compact progress text.</summary>
    private readonly Label _progress = new() { FontSize = ProgressFontSize };

    /// <summary>Hosts rendered step rows.</summary>
    private readonly VerticalStackLayout _steps = new() { Spacing = StepSpacing };

    /// <summary>Moves to the previous allowed step.</summary>
    private readonly Button _previous = new() { Text = "Previous" };

    /// <summary>Moves to the next allowed step.</summary>
    private readonly Button _next = new() { Text = "Next" };

    /// <summary>Emits a finish request when the state allows completion.</summary>
    private readonly Button _finish = new() { Text = "Finish" };

    /// <summary>Command that moves to the previous allowed step.</summary>
    private readonly Command _previousCommand;

    /// <summary>Command that moves to the next allowed step.</summary>
    private readonly Command _nextCommand;

    /// <summary>Command that emits a finish request when allowed.</summary>
    private readonly Command _finishCommand;

    /// <summary>Initializes a new instance of the <see cref="Stepper"/> class.</summary>
    public Stepper()
    {
        _previousCommand = new(() => MoveBy(PreviousStepOffset), () => StepperState?.CanGoPrevious == true);
        _nextCommand = new(() => MoveBy(NextStepOffset), () => StepperState?.CanGoNext == true);
        _finishCommand = new(Finish, () => StepperState?.CanFinish == true);
        _progress.SetDynamicResource(Label.TextColorProperty, MutedTextColorResourceKey);
        _previous.SetDynamicResource(Button.BackgroundColorProperty, NeutralSurfaceColorResourceKey);
        _previous.SetDynamicResource(Button.TextColorProperty, TextColorResourceKey);
        _next.SetDynamicResource(Button.BackgroundColorProperty, AccentColorResourceKey);
        _next.SetDynamicResource(Button.TextColorProperty, AccentTextColorResourceKey);
        _finish.SetDynamicResource(Button.BackgroundColorProperty, AccentColorResourceKey);
        _finish.SetDynamicResource(Button.TextColorProperty, AccentTextColorResourceKey);
        _previous.Command = _previousCommand;
        _next.Command = _nextCommand;
        _finish.Command = _finishCommand;

        HorizontalStackLayout actions = new() { Spacing = ActionSpacing, Children = { _previous, _next, _finish } };
        Content = new VerticalStackLayout { Padding = new(0, VerticalPadding), Spacing = StepSpacing, Children = { _progress, _steps, actions } };
        Refresh();
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public StepperState? StepperState
    {
        get => (StepperState?)GetValue(StepperStateProperty);
        set => SetValue(StepperStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? StepCommand
    {
        get => (ICommand?)GetValue(StepCommandProperty);
        set => SetValue(StepCommandProperty, value);
    }

    /// <summary>Creates a label for one step descriptor.</summary>
    /// <param name="step">The step descriptor.</param>
    /// <param name="isCurrent">A value indicating whether the row represents the current step.</param>
    /// <returns>The configured row.</returns>
    private static HorizontalStackLayout CreateStepRow(StepDescriptor step, bool isCurrent)
    {
        Label glyph = new() { FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, Text = GetStatusGlyph(step.Status, isCurrent), WidthRequest = GlyphWidth };
        Label title = new() { FontAttributes = isCurrent ? FontAttributes.Bold : FontAttributes.None, Text = step.DisplayTitle };
        glyph.SetDynamicResource(Label.TextColorProperty, GetStatusResource(step.Status, isCurrent));
        title.SetDynamicResource(Label.TextColorProperty, step.IsAvailable ? TextColorResourceKey : MutedTextColorResourceKey);
        return new() { Spacing = StepSpacing, Children = { glyph, title } };
    }

    /// <summary>Gets the glyph for a step status.</summary>
    /// <param name="status">The step status.</param>
    /// <param name="isCurrent">A value indicating whether the step is current.</param>
    /// <returns>The visible glyph.</returns>
    private static string GetStatusGlyph(StepStatus status, bool isCurrent) => status switch
    {
        StepStatus.Completed => "✓",
        StepStatus.Skipped => "−",
        StepStatus.Warning => "!",
        StepStatus.Error => "×",
        _ => isCurrent ? "●" : "○",
    };

    /// <summary>Gets the semantic resource key for a step status.</summary>
    /// <param name="status">The step status.</param>
    /// <param name="isCurrent">A value indicating whether the step is current.</param>
    /// <returns>The dynamic resource key.</returns>
    private static string GetStatusResource(StepStatus status, bool isCurrent) => status switch
    {
        StepStatus.Completed => SuccessColorResourceKey,
        StepStatus.Warning => CautionColorResourceKey,
        StepStatus.Error => DangerColorResourceKey,
        _ => isCurrent ? AccentColorResourceKey : MutedTextColorResourceKey,
    };

    /// <summary>Gets a value indicating whether a relative move is allowed by current state.</summary>
    /// <param name="state">The current stepper state.</param>
    /// <param name="offset">The requested movement offset.</param>
    /// <returns><c>true</c> when movement is allowed.</returns>
    private static bool CanMove(StepperState state, int offset) => offset switch
    {
        PreviousStepOffset => state.CanGoPrevious,
        NextStepOffset => state.CanGoNext,
        _ => false,
    };

    /// <summary>Emits a finish request through the external command.</summary>
    private void Finish()
    {
        var state = StepperState;
        if (state?.CanFinish != true || state.CurrentStep is null)
        {
            return;
        }

        EmitStep(state.CurrentStep);
    }

    /// <summary>Moves relative to the current step when that movement is allowed.</summary>
    /// <param name="offset">The relative step offset.</param>
    private void MoveBy(int offset)
    {
        var state = StepperState;
        if (state is null)
        {
            return;
        }

        var requestedIndex = state.CurrentIndex + offset;
        if (!CanMove(state, offset) || requestedIndex < 0 || requestedIndex >= state.Steps.Count)
        {
            return;
        }

        EmitAvailableStep(state.Steps[requestedIndex]);
    }

    /// <summary>Emits a step only when it is available.</summary>
    /// <param name="step">The requested step.</param>
    private void EmitAvailableStep(StepDescriptor step)
    {
        if (!step.IsAvailable)
        {
            return;
        }

        EmitStep(step);
    }

    /// <summary>Emits an allowed step through per-step and aggregate commands.</summary>
    /// <param name="step">The emitted step.</param>
    private void EmitStep(StepDescriptor step)
    {
        var current = StepperState?.CurrentStep;
        if (current?.LeaveCommand?.CanExecute(current) == true)
        {
            current.LeaveCommand.Execute(current);
        }

        if (step.EnterCommand?.CanExecute(step) == true)
        {
            step.EnterCommand.Execute(step);
        }

        if (StepCommand?.CanExecute(step) != true)
        {
            return;
        }

        StepCommand.Execute(step);
    }

    /// <summary>Updates the composed native content from the current stepper snapshot.</summary>
    private void Refresh()
    {
        var state = StepperState;
        _progress.Text = state?.ProgressText ?? "No steps";
        _steps.Children.Clear();

        if (state is null)
        {
            RefreshCommandAvailability();
            SemanticProperties.SetDescription(this, _progress.Text ?? string.Empty);
            return;
        }

        foreach (var step in state.Steps)
        {
            _steps.Children.Add(CreateStepRow(step, ReferenceEquals(step, state.CurrentStep)));
        }

        RefreshCommandAvailability();
        SemanticProperties.SetDescription(this, _progress.Text ?? string.Empty);
    }

    /// <summary>Updates command availability and button enabled states.</summary>
    private void RefreshCommandAvailability()
    {
        _previousCommand.ChangeCanExecute();
        _nextCommand.ChangeCanExecute();
        _finishCommand.ChangeCanExecute();
        _previous.IsEnabled = _previousCommand.CanExecute(null);
        _next.IsEnabled = _nextCommand.CanExecute(null);
        _finish.IsEnabled = _finishCommand.CanExecute(null);
    }
}
