// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CrissCross.Tests;

/// <summary>Verifies composed form metadata and validation feedback.</summary>
public sealed class MauiReactiveFormFieldTests
{
    /// <summary>The required metadata row.</summary>
    private const int RequiredIndex = 1;

    /// <summary>The input row.</summary>
    private const int InputIndex = 2;

    /// <summary>The helper metadata row.</summary>
    private const int HelperIndex = 3;

    /// <summary>The validation status row.</summary>
    private const int StatusIndex = 4;

    /// <summary>The validation messages row.</summary>
    private const int MessagesIndex = 5;

    /// <summary>The success message position in the validation snapshot.</summary>
    private const int SuccessIndex = 3;

    /// <summary>The pending message position in the validation snapshot.</summary>
    private const int PendingIndex = 4;

    /// <summary>Verifies the default template preserves content and projects metadata.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Template_PreservesInputAndProjectsMetadata()
    {
        var input = new Entry { Text = "42" };
        var field = new ReactiveFormField { Content = input, Header = "Pressure", FieldKey = "pressure", HelperText = "Enter bar", IsRequired = true };
        var layout = (VerticalStackLayout)((IVisualTreeElement)field).GetVisualChildren()[0];
        var inputLayout = (Grid)layout.Children[InputIndex];
        var presenter = (ContentPresenter)inputLayout.Children[1];

        await Assert.That(ReferenceEquals(field.Content, input)).IsTrue();
        await Assert.That(ReferenceEquals(presenter.Content, input)).IsTrue();
        await Assert.That(((Label)((ContentView)layout.Children[0]).Content).Text).IsEqualTo("Pressure");
        await Assert.That(((Label)layout.Children[RequiredIndex]).IsVisible).IsTrue();
        await Assert.That(((Label)layout.Children[HelperIndex]).Text).IsEqualTo("Enter bar");
        await Assert.That(field.FieldKey).IsEqualTo("pressure");
        await Assert.That(SemanticProperties.GetDescription(field)).Contains("Required");

        var replacement = new Entry();
        field.Content = replacement;
        field.Header = null;
        field.HelperText = null;
        field.IsRequired = false;
        await Assert.That(ReferenceEquals(presenter.Content, replacement)).IsTrue();
        await Assert.That(((ContentView)layout.Children[0]).IsVisible).IsFalse();
        await Assert.That(((Label)layout.Children[HelperIndex]).IsVisible).IsFalse();
        await Assert.That(((Label)layout.Children[RequiredIndex]).IsVisible).IsFalse();

        var header = new Label { Text = "Custom header" };
        field.Header = header;
        await Assert.That(ReferenceEquals(((ContentView)layout.Children[0]).Content, header)).IsTrue();
    }

    /// <summary>Verifies state aliases and semantic colors remain synchronized.</summary>
    /// <param name="state">The state to project.</param>
    /// <param name="text">The visible status text.</param>
    /// <param name="resource">The semantic color key.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments(FormFieldState.Normal, "", "CrissCrossBorderColor")]
    [Arguments(FormFieldState.Focused, "Focused", "CrissCrossAccentColor")]
    [Arguments(FormFieldState.Valid, "Valid", "CrissCrossSuccessColor")]
    [Arguments(FormFieldState.Warning, "Warning", "CrissCrossCautionColor")]
    [Arguments(FormFieldState.Invalid, "Invalid", "CrissCrossDangerColor")]
    [Arguments(FormFieldState.Pending, "Validating…", "CrissCrossAccentColor")]
    public async Task State_ProjectsTextAndFollowsTheme(FormFieldState state, string text, string resource)
    {
        var field = new ReactiveFormField();
        field.Resources[resource] = Colors.Red;
        field.FieldState = state;
        var layout = (VerticalStackLayout)((IVisualTreeElement)field).GetVisualChildren()[0];
        var status = (Label)layout.Children[StatusIndex];
        var rail = (BoxView)((Grid)layout.Children[InputIndex]).Children[0];

        await Assert.That(field.State).IsEqualTo(state);
        await Assert.That(status.Text).IsEqualTo(text);
        await Assert.That(status.IsVisible).IsEqualTo(state != FormFieldState.Normal);
        await Assert.That(rail.Color).IsEqualTo(Colors.Red);
        field.Resources[resource] = Colors.Blue;
        await Assert.That(rail.Color).IsEqualTo(Colors.Blue);
        field.State = FormFieldState.Valid;
        await Assert.That(field.FieldState).IsEqualTo(FormFieldState.Valid);
        field.FieldState = null;
        await Assert.That(field.State).IsEqualTo(FormFieldState.Normal);
    }

    /// <summary>Verifies validation text and severity colors update with the message snapshot.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Messages_ProjectSeverityAndClearWhenReplaced()
    {
        var field = new ReactiveFormField();
        field.Resources["CrissCrossDangerColor"] = Colors.Red;
        field.Resources["CrissCrossCautionColor"] = Colors.Yellow;
        field.Resources["CrissCrossMutedTextColor"] = Colors.Gray;
        field.Resources["CrissCrossSuccessColor"] = Colors.Green;
        field.Resources["CrissCrossAccentColor"] = Colors.Blue;
        field.Messages =
        [
            new(null, null, "Invalid"),
            new(null, null, "Warning", ValidationSeverity.Warning),
            new(null, null, "Info", ValidationSeverity.Information),
            new(null, null, "Valid", ValidationSeverity.Success),
            new(null, null, "Checking", ValidationSeverity.Pending),
        ];
        var layout = (VerticalStackLayout)((IVisualTreeElement)field).GetVisualChildren()[0];
        var messages = (VerticalStackLayout)layout.Children[MessagesIndex];

        await Assert.That(((Label)messages.Children[0]).Text).IsEqualTo("Invalid");
        await Assert.That(((Label)messages.Children[0]).TextColor).IsEqualTo(Colors.Red);
        await Assert.That(((Label)messages.Children[1]).TextColor).IsEqualTo(Colors.Yellow);
        await Assert.That(((Label)messages.Children[2]).TextColor).IsEqualTo(Colors.Gray);
        await Assert.That(((Label)messages.Children[SuccessIndex]).TextColor).IsEqualTo(Colors.Green);
        await Assert.That(((Label)messages.Children[PendingIndex]).TextColor).IsEqualTo(Colors.Blue);
        await Assert.That(messages.IsVisible).IsTrue();
        field.Messages = null;
        await Assert.That(messages.Children.Count).IsEqualTo(0);
        await Assert.That(messages.IsVisible).IsFalse();
    }
}
