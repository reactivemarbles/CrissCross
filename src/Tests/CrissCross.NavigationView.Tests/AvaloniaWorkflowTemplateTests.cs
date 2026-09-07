// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.VisualTree;
using ReactiveUI;
using Controls = CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises workflow templates and commands in an attached Avalonia window.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaWorkflowTemplateTests
{
    /// <summary>The pressure field used by the form and search examples.</summary>
    private const string PressureField = "pressure";

    /// <summary>The title typography size supplied by the CrissCross theme.</summary>
    private const double TitleFontSize = 28;

    /// <summary>The caption typography size supplied by the CrissCross theme.</summary>
    private const double CaptionFontSize = 12;

    /// <summary>Bounds an asynchronous gallery operation regression.</summary>
    private const int OperationTimeoutSeconds = 5;

    /// <summary>The rounded corner size used by the native-template interop regression.</summary>
    private const double CornerSize = 6;

    /// <summary>The disabled opacity used by standard light and dark themes.</summary>
    private const double StandardDisabledOpacity = 0.5;

    /// <summary>Verifies filter labels, removal permissions, command payloads, and state clearing.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task FilterBar_RendersTokensAndDispatchesActions()
    {
        var removable = new FilterToken("area", FilterOperator.Equals, "A", "Area A");
        var fixedToken = new FilterToken("plant", FilterOperator.Equals, "North", "North plant", false);
        FilterToken? removed = null;
        var cleared = false;
        using var remove = ReactiveCommand.Create<FilterToken>(token => removed = token);
        using var clear = ReactiveCommand.Create(() => cleared = true);
        var control = new Controls.FilterBar { QueryState = new(filters: [removable, fixedToken]), RemoveFilterCommand = remove, ClearAllCommand = clear };
        var window = new Window { Content = control };
        try
        {
            window.Show();
            window.UpdateLayout();
            await Assert.That(HasText(control, removable.DisplayText)).IsTrue();
            await Assert.That(HasText(control, fixedToken.DisplayText)).IsTrue();
            var removeButton = FindVisual<Button>(control, button => ReferenceEquals(button.CommandParameter, removable));
            var fixedButton = FindVisual<Button>(control, button => ReferenceEquals(button.CommandParameter, fixedToken));
            await Assert.That(removeButton.IsVisible).IsTrue();
            await Assert.That(fixedButton.IsVisible).IsFalse();
            removeButton.Command!.Execute(removeButton.CommandParameter);
            await Assert.That(removed).IsSameReferenceAs(removable);
            var clearButton = FindVisual<Button>(control, static button => Equals(button.Content, "Clear"));
            clearButton.Command!.Execute(clearButton.CommandParameter);
            await Assert.That(cleared).IsTrue();
            control.QueryState = null;
            window.UpdateLayout();
            await Assert.That(control.ItemsSource is null).IsTrue();
            await Assert.That(HasText(control, removable.DisplayText)).IsFalse();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies workflow and validation item templates display model labels and dispatch their commands.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task StepperAndValidation_RenderModelContentAndClearState()
    {
        var prepare = new StepDescriptor("prepare", "Prepare equipment");
        var run = new StepDescriptor("run", "Run equipment");
        var message = new ValidationMessage(PressureField, "Pressure", "Set a safe pressure");
        string? requestedStep = null;
        ValidationMessage? requestedMessage = null;
        using var stepCommand = ReactiveCommand.Create<string>(key => requestedStep = key);
        using var fieldCommand = ReactiveCommand.Create<ValidationMessage>(value => requestedMessage = value);
        var stepper = new Controls.Stepper { State = new([prepare, run], prepare.Key), StepRequestedCommand = stepCommand };
        var validation = new Controls.ValidationSummary { SummaryState = new([message]), NavigateToFieldCommand = fieldCommand };
        var window = new Window { Content = new StackPanel { Children = { stepper, validation } } };
        try
        {
            window.Show();
            window.UpdateLayout();
            await Assert.That(HasText(stepper, prepare.DisplayTitle)).IsTrue();
            await Assert.That(HasText(stepper, run.DisplayTitle)).IsTrue();
            await Assert.That(HasText(validation, message.DisplayText)).IsTrue();
            var next = FindVisual<Button>(stepper, static button => Equals(button.Content, "Next"));
            await Assert.That(next.IsEnabled).IsTrue();
            next.Command!.Execute(next.CommandParameter);
            await Assert.That(requestedStep).IsEqualTo(run.Key);
            var field = FindVisual<Button>(validation, static _ => true);
            field.Command!.Execute(field.CommandParameter);
            await Assert.That(requestedMessage).IsSameReferenceAs(message);
            stepper.State = null;
            validation.SummaryState = null;
            window.UpdateLayout();
            await Assert.That(stepper.CurrentKey).IsNull();
            await Assert.That(stepper.ItemsSource is null).IsTrue();
            await Assert.That(validation.ItemsSource is null).IsTrue();
            await Assert.That(next.IsEnabled).IsFalse();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies group commands override model commands and fall back when cleared.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task ChipGroup_UsesGroupCommandsAndModelFallback()
    {
        ChipModel? selected = null;
        ChipModel? removed = null;
        var usedGroup = false;
        using var modelSelect = ReactiveCommand.Create<ChipModel>(chip => selected = chip);
        using var modelRemove = ReactiveCommand.Create<ChipModel>(chip => removed = chip);
        using var groupSelect = ReactiveCommand.Create<ChipModel>(_ => usedGroup = true);
        var model = new ChipModel("pump", "Pump 101", new() { IsRemovable = true, SelectCommand = modelSelect, RemoveCommand = modelRemove });
        var group = new Controls.ChipGroup { GroupState = new([model]), SelectChipCommand = groupSelect };
        var window = new Window { Content = group };
        try
        {
            window.Show();
            window.UpdateLayout();
            var chip = FindVisual<Controls.Chip>(group, static _ => true);
            await Assert.That(HasText(chip, model.Text)).IsTrue();
            var select = FindVisual<Button>(chip, button => Equals(button.Content, model.Text));
            select.Command!.Execute(select.CommandParameter);
            await Assert.That(usedGroup).IsTrue();
            await Assert.That(selected).IsNull();
            group.SelectChipCommand = null;
            select.Command!.Execute(select.CommandParameter);
            await Assert.That(selected).IsSameReferenceAs(model);
            var remove = FindVisual<Button>(chip, static button => Equals(button.Content, "×"));
            remove.Command!.Execute(remove.CommandParameter);
            await Assert.That(removed).IsSameReferenceAs(model);
            chip.IsRemovable = false;
            await Assert.That(remove.IsVisible).IsFalse();
            group.GroupState = null;
            window.UpdateLayout();
            await Assert.That(group.ItemsSource is null).IsTrue();
            await Assert.That(HasVisual<Controls.Chip>(group, static _ => true)).IsFalse();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies native template reuse preserves CrissCross wrapper styling and derived button templates.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task NativeWrappers_ApplyCustomStylesAndRetainTemplates()
    {
        var title = new Controls.TextBlock { Text = "Equipment", FontTypography = Controls.FontTypography.Title };
        var suggestions = new Controls.AutoSuggestBox();
        var list = new Controls.ListBox { ItemsSource = new[] { "Pump" } };
        var command = new Controls.AsyncCommandButton { Content = "Start" };
        var window = new Controls.Window { Content = new StackPanel { Children = { title, suggestions, list, command } } };
        try
        {
            window.Show();
            window.UpdateLayout();
            await Assert.That(title.FontSize).IsEqualTo(TitleFontSize);
            title.FontTypography = Controls.FontTypography.Caption;
            await Assert.That(title.FontSize).IsEqualTo(CaptionFontSize);
            await Assert.That(suggestions.Template).IsNotNull();
            await Assert.That(suggestions.FilterMode).IsEqualTo(AutoCompleteFilterMode.Contains);
            await Assert.That(suggestions.IsTextCompletionEnabled).IsTrue();
            await Assert.That(list.Template).IsNotNull();
            await Assert.That(HasText(list, "Pump")).IsTrue();
            await Assert.That(window.Template).IsNotNull();
            await Assert.That(command.Template).IsNotNull();
            await Assert.That(HasVisual<Border>(command, static border => border.Name == "RootBorder")).IsTrue();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies the transformed reactive package renders its own model and control namespaces.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task ReactivePackage_WorkflowTemplatesRenderAndDispatchCommands()
    {
        var token = new CrissCross.Reactive.FilterToken("area", CrissCross.Reactive.FilterOperator.Equals, "South", "South plant");
        CrissCross.Reactive.FilterToken? removed = null;
        using var remove = ReactiveCommand.Create<CrissCross.Reactive.FilterToken>(value => removed = value);
        var filters = new CrissCross.Reactive.Avalonia.UI.Controls.FilterBar { QueryState = new(filters: [token]), RemoveFilterCommand = remove };
        var stepper = new CrissCross.Reactive.Avalonia.UI.Controls.Stepper { State = new([new CrissCross.Reactive.StepDescriptor("start", "Prepare reactor")]) };
        var button = new CrissCross.Reactive.Avalonia.UI.Controls.AsyncCommandButton { Content = "Import telemetry" };
        var window = new Window { Content = new StackPanel { Children = { filters, stepper, button } } };
        var styles = global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(new("avares://CrissCross.Avalonia.UI.Reactive/Themes/Index.axaml"));
        window.Styles.Add((global::Avalonia.Styling.Styles)styles);
        try
        {
            window.Show();
            window.UpdateLayout();
            await Assert.That(HasText(filters, token.DisplayText)).IsTrue();
            await Assert.That(HasText(stepper, "Prepare reactor")).IsTrue();
            await Assert.That(button.Template).IsNotNull();
            var removeButton = FindVisual<Button>(filters, candidate => ReferenceEquals(candidate.CommandParameter, token));
            removeButton.Command!.Execute(removeButton.CommandParameter);
            await Assert.That(removed).IsSameReferenceAs(token);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies high contrast keeps disabled semantic text colors fully visible.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task DisabledTextControls_HighContrastAvoidsDoubleDimming()
    {
        var radio = new Controls.RadioButton { Content = "Disabled option", IsEnabled = false };
        var check = new Controls.CheckBox { Content = "Disabled selection", IsEnabled = false };
        var button = new Controls.Button { Content = "Disabled action", IsEnabled = false };
        var window = new Window
        {
            Content = new StackPanel { Children = { radio, check, button } },
            RequestedThemeVariant = CrissCross.Avalonia.UI.Appearance.ApplicationThemeManager.HighContrastThemeVariant,
        };
        try
        {
            window.Show();
            window.UpdateLayout();
            await Assert.That(radio.Opacity).IsEqualTo(1D);
            await Assert.That(check.Opacity).IsEqualTo(1D);
            await Assert.That(button.Opacity).IsEqualTo(1D);
            window.RequestedThemeVariant = global::Avalonia.Styling.ThemeVariant.Light;
            await Assert.That(radio.Opacity).IsEqualTo(StandardDisabledOpacity);
            await Assert.That(check.Opacity).IsEqualTo(StandardDisabledOpacity);
            await Assert.That(button.Opacity).IsEqualTo(StandardDisabledOpacity);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies wrapper properties are visible to reused native rendering code and templates.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task NativeTemplateProperties_ShareTheWrapperValues()
    {
        var radius = new global::Avalonia.CornerRadius(CornerSize);
        var expander = new Controls.Expander { CornerRadius = radius };
        var icon = new TextBlock { Text = "Pump" };
        var menuItem = new Controls.MenuItem { Icon = icon };
        var image = new Controls.GifImage { StretchDirection = global::Avalonia.Media.StretchDirection.DownOnly };
        await Assert.That(((Expander)expander).CornerRadius).IsEqualTo(radius);
        await Assert.That(((MenuItem)menuItem).Icon).IsSameReferenceAs(icon);
        await Assert.That(((Image)image).StretchDirection).IsEqualTo(global::Avalonia.Media.StretchDirection.DownOnly);
    }

    /// <summary>Verifies gallery filter commands persist through subsequent query changes.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task GalleryFilters_RemainRemovedWhenSearchTextChanges()
    {
        using var model = new CrissCross.Avalonia.UI.Gallery.ViewModels.FeaturePlaygroundPageViewModel();
        var token = model.SearchState.ActiveFilters[0];
        ((System.Windows.Input.ICommand)model.RemoveFilterCommand).Execute(token);
        await Assert.That(model.SearchState.ActiveFilterCount).IsEqualTo(1);
        model.SearchText = PressureField;
        await Assert.That(model.SearchState.ActiveFilterCount).IsEqualTo(1);
        ((System.Windows.Input.ICommand)model.ClearFiltersCommand).Execute(null);
        await Assert.That(model.SearchState.ActiveFilterCount).IsEqualTo(0);
        await Assert.That(model.SearchText).IsEqualTo(PressureField);
        model.SearchText = "pump";
        await Assert.That(model.SearchState.ActiveFilterCount).IsEqualTo(0);
    }

    /// <summary>Verifies cancellation stops the import and leaves the query untouched.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task GalleryImport_CancelPreservesSearchAndClearsBusyState()
    {
        using var model = new CrissCross.Avalonia.UI.Gallery.ViewModels.FeaturePlaygroundPageViewModel();
        var originalQuery = model.SearchText;
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void OnPropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(model.CurrentOperation) || model.CurrentOperation is not null)
            {
                return;
            }

            completion.SetResult();
        }

        model.PropertyChanged += OnPropertyChanged;
        try
        {
            ((System.Windows.Input.ICommand)model.RunImportCommand).Execute(null);
            await Assert.That(model.CurrentOperation).IsNotNull();
            model.CurrentOperation!.CancelCommand!.Execute(null);
            await completion.Task.WaitAsync(TimeSpan.FromSeconds(OperationTimeoutSeconds));
            await Assert.That(model.CurrentOperation).IsNull();
            await Assert.That(model.CommandState).IsEqualTo(CommandButtonState.Cancelled);
            await Assert.That(model.SearchText).IsEqualTo(originalQuery);
        }
        finally
        {
            model.PropertyChanged -= OnPropertyChanged;
        }
    }

    /// <summary>Finds rendered text rather than an uninstantiated data template.</summary>
    /// <param name="control">The rendered control.</param>
    /// <param name="text">The expected text.</param>
    /// <returns>Whether a visible text element contains the text.</returns>
    private static bool HasText(Control control, string text) =>
        HasVisual<TextBlock>(control, block => block.Text == text && block.IsEffectivelyVisible);

    /// <summary>Finds an attached template element matching a predicate.</summary>
    /// <typeparam name="TControl">The required element type.</typeparam>
    /// <param name="control">The rendered control.</param>
    /// <param name="predicate">The element condition.</param>
    /// <returns>The matching element.</returns>
    private static TControl FindVisual<TControl>(Control control, Predicate<TControl> predicate)
        where TControl : Control
    {
        foreach (var descendant in control.GetVisualDescendants())
        {
            if (descendant is TControl candidate && predicate(candidate))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException($"The template did not render a matching {typeof(TControl).Name}.");
    }

    /// <summary>Checks whether a matching template element is present.</summary>
    /// <typeparam name="TControl">The required element type.</typeparam>
    /// <param name="control">The rendered control.</param>
    /// <param name="predicate">The element condition.</param>
    /// <returns>Whether a matching element exists.</returns>
    private static bool HasVisual<TControl>(Control control, Predicate<TControl> predicate)
        where TControl : Control
    {
        foreach (var descendant in control.GetVisualDescendants())
        {
            if (descendant is TControl candidate && predicate(candidate))
            {
                return true;
            }
        }

        return false;
    }
}
