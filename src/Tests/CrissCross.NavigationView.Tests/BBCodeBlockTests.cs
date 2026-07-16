// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using CrissCross.WPF.UI.Controls;
using WpfBorder = System.Windows.Controls.Border;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests the WPF BBCodeBlock parser, renderer, navigation, and theme integration.</summary>
public class BBCodeBlockTests
{
    /// <summary>The expected number of safe hyperlinks.</summary>
    private const int ExpectedLinkCount = 2;

    /// <summary>The minimum number of structural UI containers.</summary>
    private const int MinimumContainerCount = 11;

    /// <summary>The minimum number of themed borders.</summary>
    private const int MinimumBorderCount = 4;

    /// <summary>Verifies nested markup, case-insensitive tags, formatting, and literal unknown tags.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenNestedAndMixedCase_RendersFormattingAndPreservesUnknownTags()
    {
        var result = RunOnStaThread(() =>
        {
            BBCodeBlock block = new()
            {
                BBCode = "[B]bold [i]italic[/I][/b] [unknown=x]literal[/unknown]",
            };
            var inlines = EnumerateInlines(block.Inlines).ToList();
            return new FormattingResult(
                string.Concat(inlines.OfType<Run>().Select(run => run.Text)),
                inlines.OfType<Span>().Any(span => span.FontWeight == FontWeights.Bold),
                inlines.OfType<Span>().Any(span => span.FontStyle == FontStyles.Italic));
        });

        await Assert.That(result.Text).IsEqualTo("bold italic [unknown=x]literal[/unknown]");
        await Assert.That(result.HasBold).IsTrue();
        await Assert.That(result.HasItalic).IsTrue();
    }

    /// <summary>Verifies that only explicitly supported navigation schemes become hyperlinks.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenLinksAreRendered_AllowsOnlySafeSchemes()
    {
        var result = RunOnStaThread(() =>
        {
            BBCodeBlock block = new()
            {
                BBCode = "[url=https://example.com]web[/url] [email]user@example.com[/email] " +
                    "[url=javascript:alert(1)]blocked[/url]",
            };
            var inlines = EnumerateInlines(block.Inlines).ToList();
            var schemes = inlines
                .OfType<Hyperlink>()
                .Select(link => link.NavigateUri.Scheme)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToList();
            return new LinkResult(
                schemes,
                string.Concat(inlines.OfType<Run>().Select(run => run.Text)));
        });

        await Assert.That(result.Schemes.Count).IsEqualTo(ExpectedLinkCount);
        await Assert.That(result.Schemes[0]).IsEqualTo("https");
        await Assert.That(result.Schemes[1]).IsEqualTo("mailto");
        await Assert.That(result.Text).Contains("blocked");
    }

    /// <summary>Verifies that command links execute through ICommand and retain the complete payload.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CommandLink_WhenInvoked_ExecutesCompletePayload()
    {
        var result = RunOnStaThread(() =>
        {
            RecordingCommand command = new();
            BBCodeBlock block = new()
            {
                BBCode = "[url=cmd:refresh:all]Refresh[/url]",
                Command = command,
            };
            var hyperlink = EnumerateInlines(block.Inlines).OfType<Hyperlink>().Single();
            RequestNavigateEventArgs eventArgs = new(hyperlink.NavigateUri, null)
            {
                RoutedEvent = Hyperlink.RequestNavigateEvent,
            };
            hyperlink.RaiseEvent(eventArgs);
            return new CommandResult(command.Parameter as string, block.CommandParameter as string, eventArgs.Handled);
        });

        await Assert.That(result.ExecutedParameter).IsEqualTo("refresh:all");
        await Assert.That(result.ControlParameter).IsEqualTo("refresh:all");
        await Assert.That(result.Handled).IsTrue();
    }

    /// <summary>Verifies that the reference document structures render as WPF-hosted content.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BBCode_WhenReferenceStructuresAreCombined_RendersEveryStructure()
    {
        var result = RunOnStaThread(() =>
        {
            const string Markup =
                "[h1]Heading[/h1][center]Centered[/center][quote=Author]Quote[/quote]" +
                "[spoiler=Details]Hidden[/spoiler][code=csharp]var value = 1;[/code]" +
                "[list=1][*]One[*]Two[/list][table][tr][th]Name[/th][td]Value[/td][/tr][/table]" +
                "[pipes]|One|Two|\n|Three|Four|[/pipes][rating=4 max=5][/rating]" +
                "[youtube]dQw4w9WgXcQ[/youtube][hr]";
            BBCodeBlock block = new() { BBCode = Markup };
            var inlines = EnumerateInlines(block.Inlines).ToList();
            var containers = inlines.OfType<InlineUIContainer>().ToList();
            var borders = containers
                .SelectMany(container => EnumerateLogicalTree(container.Child))
                .OfType<WpfBorder>()
                .ToList();
            return new StructureResult(
                containers.Count,
                borders.Count,
                borders.Any(border =>
                    DependencyPropertyHelper.GetValueSource(border, WpfBorder.BackgroundProperty).IsExpression));
        });

        await Assert.That(result.ContainerCount).IsGreaterThanOrEqualTo(MinimumContainerCount);
        await Assert.That(result.BorderCount).IsGreaterThanOrEqualTo(MinimumBorderCount);
        await Assert.That(result.HasDynamicThemeResource).IsTrue();
    }

    /// <summary>Enumerates an inline tree in document order.</summary>
    /// <param name="inlines">The root inline collection.</param>
    /// <returns>The inline sequence.</returns>
    private static IEnumerable<Inline> EnumerateInlines(InlineCollection inlines)
    {
        foreach (Inline inline in inlines)
        {
            yield return inline;
            if (inline is Span span)
            {
                foreach (var child in EnumerateInlines(span.Inlines))
                {
                    yield return child;
                }
            }
        }
    }

    /// <summary>Enumerates a logical WPF tree.</summary>
    /// <param name="root">The logical root.</param>
    /// <returns>The logical object sequence.</returns>
    private static IEnumerable<DependencyObject> EnumerateLogicalTree(DependencyObject root)
    {
        yield return root;
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
        {
            foreach (var descendant in EnumerateLogicalTree(child))
            {
                yield return descendant;
            }
        }
    }

    /// <summary>Executes a function on a dedicated STA thread.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The operation to execute.</param>
    /// <returns>The operation result.</returns>
    private static T RunOnStaThread<T>(Func<T> action)
    {
        T? result = default;
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                result = action();
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (exception is not null)
        {
            throw exception;
        }

        return result!;
    }

    /// <summary>Records command execution for command-link verification.</summary>
    private sealed class RecordingCommand : ICommand
    {
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <summary>Gets the most recently executed parameter.</summary>
        public object? Parameter { get; private set; }

        /// <inheritdoc />
        public bool CanExecute(object? parameter) => true;

        /// <inheritdoc />
        public void Execute(object? parameter) => Parameter = parameter;
    }

    /// <summary>Captures formatting results outside the STA test thread.</summary>
    /// <param name="Text">The rendered text.</param>
    /// <param name="HasBold">Whether bold formatting was present.</param>
    /// <param name="HasItalic">Whether italic formatting was present.</param>
    private sealed record FormattingResult(string Text, bool HasBold, bool HasItalic);

    /// <summary>Captures safe-link results outside the STA test thread.</summary>
    /// <param name="Schemes">The rendered hyperlink schemes.</param>
    /// <param name="Text">The rendered text.</param>
    private sealed record LinkResult(IReadOnlyList<string> Schemes, string Text);

    /// <summary>Captures command-link results outside the STA test thread.</summary>
    /// <param name="ExecutedParameter">The command execution parameter.</param>
    /// <param name="ControlParameter">The command parameter dependency property value.</param>
    /// <param name="Handled">Whether navigation was handled.</param>
    private sealed record CommandResult(string? ExecutedParameter, string? ControlParameter, bool Handled);

    /// <summary>Captures structural rendering results outside the STA test thread.</summary>
    /// <param name="ContainerCount">The number of inline UI containers.</param>
    /// <param name="BorderCount">The number of themed borders.</param>
    /// <param name="HasDynamicThemeResource">Whether generated content uses a dynamic theme resource.</param>
    private sealed record StructureResult(int ContainerCount, int BorderCount, bool HasDynamicThemeResource);
}
