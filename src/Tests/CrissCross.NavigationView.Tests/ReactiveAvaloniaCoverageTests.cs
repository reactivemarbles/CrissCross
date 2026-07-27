// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using Avalonia;
using CrissCross.Reactive.Avalonia.UI.Converters;
using ReactiveAppBar = CrissCross.Reactive.Avalonia.UI.Controls.AppBar;
using ReactiveBBCodeBlock = CrissCross.Reactive.Avalonia.UI.Controls.BBCodeBlock;
using ReactiveNavigationUserControl = CrissCross.Reactive.Avalonia.NavigationUserControl;
using ReactiveRichTextBox = CrissCross.Reactive.Avalonia.UI.Controls.RichTextBox;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises the reactive Avalonia UI assembly's constructible public surface.</summary>
public sealed class ReactiveAvaloniaCoverageTests
{
    /// <summary>The offset at which the word selected for editing begins.</summary>
    private const int WorldStartOffset = 6;

    /// <summary>The length of the word selected for editing.</summary>
    private const int WorldLength = 5;

    /// <summary>Verifies every public parameterless reactive styled element can be constructed.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactivePublicParameterlessStyledElements_WhenConstructed_ProduceStyledElements()
    {
        var assembly = typeof(ReactiveAppBar).Assembly;
        var constructedCount = 0;
        var roundTrippedPropertyCount = 0;

        foreach (var type in assembly.GetExportedTypes())
        {
            if (type.IsAbstract || !typeof(StyledElement).IsAssignableFrom(type) || type.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            var element = (StyledElement)Activator.CreateInstance(type)!;
            await Assert.That(element is StyledElement).IsTrue();
            constructedCount++;

            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (!IsRoundTrippableCrissCrossProperty(property))
                {
                    continue;
                }

                var value = property.GetValue(element);
                property.SetValue(element, value);
                roundTrippedPropertyCount++;
            }
        }

        await Assert.That(constructedCount).IsGreaterThan(0);
        await Assert.That(roundTrippedPropertyCount).IsGreaterThan(0);
    }

    /// <summary>Verifies the reactive converter variant keeps the same boolean semantics.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveBoolToInvertedConverter_WhenGivenBooleanValues_InvertsThem()
    {
        var converter = new BoolToInvertedBoolConverter();
        var culture = CultureInfo.InvariantCulture;

        await Assert.That(RequireBoolean(converter.Convert(true, typeof(bool), null, culture))).IsFalse();
        await Assert.That(RequireBoolean(converter.Convert(false, typeof(bool), null, culture))).IsTrue();
    }

    /// <summary>Verifies the reactive Avalonia navigation host can be constructed independently.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveNavigationUserControl_WhenConstructed_ExposesEmptyName()
    {
        var control = new ReactiveNavigationUserControl();

        await Assert.That(control.Name).IsNull();
    }

    /// <summary>Verifies the reactive BBCode implementation renders formatting and executes command links.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveBBCodeBlock_WhenFormattingAndCommandLinksAreUsed_RendersAndExecutes()
    {
        var command = new ReactiveRecordingCommand();
        ReactiveBBCodeBlock block = new() { BBCode = "[b]bold[/b] [color=#FF112233]colour[/color] [url=cmd:refresh]refresh[/url]", Command = command };

        Uri commandUri = new("cmd:refresh");
        block.Navigate(commandUri);

        await Assert.That(block.Inlines is { Count: > 0 }).IsTrue();
        await Assert.That(command.Parameter).IsEqualTo("refresh");
    }

    /// <summary>Verifies reactive RichText parsing, editing, and document projection preserve plain text.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveRichText_WhenHtmlIsEdited_ProjectsExpectedPlainText()
    {
        var box = new ReactiveRichTextBox();
        box.SetHtml("<strong>Hello</strong> world");

        box.Select(WorldStartOffset, WorldLength);
        box.ToggleItalic();

        await Assert.That(box.PlainText).IsEqualTo("Hello world");
        await Assert.That(box.Html).Contains("<em>world</em>");
    }

    /// <summary>Determines whether a property can safely be round-tripped by the construction catalog.</summary>
    /// <param name="property">The candidate property.</param>
    /// <returns><see langword="true" /> when the property belongs to CrissCross and can be read and written.</returns>
    private static bool IsRoundTrippableCrissCrossProperty(System.Reflection.PropertyInfo property) =>
        property is { CanRead: true, CanWrite: true }
        && property.GetIndexParameters().Length == 0
        && property.DeclaringType?.Namespace?.StartsWith("CrissCross.", StringComparison.Ordinal) == true;

    /// <summary>Returns a converter result as a required boolean value.</summary>
    /// <param name="value">The converter result.</param>
    /// <returns>The boolean result.</returns>
    private static bool RequireBoolean(object? value) => value as bool? ?? throw new InvalidOperationException();

    /// <summary>Records the reactive BBCode command payload.</summary>
    private sealed class ReactiveRecordingCommand : System.Windows.Input.ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <summary>Gets the most recent command payload.</summary>
        public string? Parameter { get; private set; }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => true;

        /// <inheritdoc/>
        public void Execute(object? parameter) => Parameter = parameter as string;
    }
}
