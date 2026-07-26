// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
        var result = RunOnStaThread(static () =>
        {
            BBCodeBlock block = new() { BBCode = "[B]bold [i]italic[/I][/b] [unknown=x]literal[/unknown]", };
            var inlines = MaterializeInlines(block.Inlines);
            return new FormattingResult(
                GetRunText(inlines),
                HasSpan(inlines, static span => span.FontWeight == FontWeights.Bold),
                HasSpan(inlines, static span => span.FontStyle == FontStyles.Italic));
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
        var result = RunOnStaThread(static () =>
        {
            BBCodeBlock block = new() { BBCode = "[url=https://example.com]web[/url] [email]user@example.com[/email] " + "[url=javascript:alert(1)]blocked[/url]", };
            var inlines = MaterializeInlines(block.Inlines);
            var schemes = GetHyperlinkSchemes(inlines);
            return new LinkResult(
                schemes,
                GetRunText(inlines));
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
        var result = RunOnStaThread(static () =>
        {
            RecordingCommand command = new();
            BBCodeBlock block = new() { BBCode = "[url=cmd:refresh:all]Refresh[/url]", Command = command, };
            Hyperlink? hyperlink = null;
            foreach (var inline in EnumerateInlines(block.Inlines))
            {
                if (inline is Hyperlink candidate)
                {
                    hyperlink = candidate;
                    break;
                }
            }

            ArgumentNullException.ThrowIfNull(hyperlink);
            RequestNavigateEventArgs eventArgs = new(hyperlink.NavigateUri, null) { RoutedEvent = Hyperlink.RequestNavigateEvent, };
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
        var result = RunOnStaThread(static () =>
        {
            const string Markup =
                "[h1]Heading[/h1][center]Centered[/center][quote=Author]Quote[/quote]"
                + "[spoiler=Details]Hidden[/spoiler][code=csharp]var value = 1;[/code]"
                + "[list=1][*]One[*]Two[/list][table][tr][th]Name[/th][td]Value[/td][/tr][/table]"
                + "[pipes]|One|Two|\n|Three|Four|[/pipes][rating=4 max=5][/rating]"
                + "[youtube]dQw4w9WgXcQ[/youtube][hr]";
            BBCodeBlock block = new() { BBCode = Markup };
            var inlines = MaterializeInlines(block.Inlines);
            var containers = new List<InlineUIContainer>();
            var borders = new List<WpfBorder>();
            foreach (var inline in inlines)
            {
                if (inline is not InlineUIContainer container)
                {
                    continue;
                }

                containers.Add(container);
                foreach (var element in EnumerateLogicalTree(container.Child))
                {
                    if (element is WpfBorder border)
                    {
                        borders.Add(border);
                    }
                }
            }

            var hasDynamicThemeResource = false;
            foreach (var border in borders)
            {
                if (DependencyPropertyHelper.GetValueSource(border, WpfBorder.BackgroundProperty).IsExpression)
                {
                    hasDynamicThemeResource = true;
                    break;
                }
            }

            return new StructureResult(
                containers.Count,
                borders.Count,
                hasDynamicThemeResource);
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
        foreach (var child in LogicalTreeHelper.GetChildren(root))
        {
            if (child is not DependencyObject dependencyObject)
            {
                continue;
            }

            foreach (var descendant in EnumerateLogicalTree(dependencyObject))
            {
                yield return descendant;
            }
        }
    }

    /// <summary>Materializes inline content.</summary>
    /// <param name="inlines">The source collection.</param>
    /// <returns>The materialized inlines.</returns>
    private static List<Inline> MaterializeInlines(InlineCollection inlines) => [.. EnumerateInlines(inlines)];

    /// <summary>Gets the concatenated run text.</summary>
    /// <param name="inlines">The source inlines.</param>
    /// <returns>The rendered text.</returns>
    private static string GetRunText(IEnumerable<Inline> inlines)
    {
        var text = new System.Text.StringBuilder();
        foreach (var inline in inlines)
        {
            if (inline is Run run)
            {
                _ = text.Append(run.Text);
            }
        }

        return text.ToString();
    }

    /// <summary>Determines whether a span matches a predicate.</summary>
    /// <param name="inlines">The source inlines.</param>
    /// <param name="predicate">The match predicate.</param>
    /// <returns>Whether a matching span exists.</returns>
    private static bool HasSpan(IEnumerable<Inline> inlines, Func<Span, bool> predicate)
    {
        foreach (var inline in inlines)
        {
            if (inline is Span span && predicate(span))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets hyperlink schemes in ordinal order.</summary>
    /// <param name="inlines">The source inlines.</param>
    /// <returns>The ordered schemes.</returns>
    private static List<string> GetHyperlinkSchemes(IEnumerable<Inline> inlines)
    {
        var schemes = new List<string>();
        foreach (var inline in inlines)
        {
            if (inline is Hyperlink { NavigateUri: not null } hyperlink)
            {
                schemes.Add(hyperlink.NavigateUri.Scheme);
            }
        }

        schemes.Sort(StringComparer.Ordinal);
        return schemes;
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
