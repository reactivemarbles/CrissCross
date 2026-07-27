// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using CrissCross.WPF.UI.SymbolGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Exercises the Fluent symbol incremental generator.</summary>
public class SymbolEnumGeneratorTests
{
    /// <summary>The required number of members per symbol style.</summary>
    private const int ExpectedMemberCount = 7_808;

    /// <summary>The expected number of generated symbol enum sources.</summary>
    private const int ExpectedGeneratedSourceCount = 2;

    /// <summary>The virtual regular-symbol catalog path.</summary>
    private const string RegularCatalogPath = "SymbolRegular.0.symbols";

    /// <summary>The virtual filled-symbol catalog path.</summary>
    private const string FilledCatalogPath = "SymbolFilled.0.symbols";

    /// <summary>The reactive compilation preprocessor symbol.</summary>
    private const string ReactivePreprocessorSymbol = "REACTIVELIST_REACTIVE";

    /// <summary>The standard generated namespace.</summary>
    private const string StandardNamespace = "namespace CrissCross.WPF.UI.Controls;";

    /// <summary>The reactive generated namespace.</summary>
    private const string ReactiveNamespace = "namespace CrissCross.Reactive.WPF.UI.Controls;";

    /// <summary>The regular catalog documentation marker.</summary>
    private const string RegularCatalogDocumentation = "Represents a list of regular";

    /// <summary>The malformed-catalog diagnostic identifier.</summary>
    private const string MalformedCatalogDiagnosticId = "CCSG001";

    /// <summary>The missing-catalog-text diagnostic identifier.</summary>
    private const string MissingCatalogTextDiagnosticId = "CCSG002";

    /// <summary>The duplicate-catalog-entry diagnostic identifier.</summary>
    private const string DuplicateCatalogEntryDiagnosticId = "CCSG003";

    /// <summary>The malformed catalog content.</summary>
    private const string MalformedCatalogContent = "MissingSeparator";

    /// <summary>The duplicate catalog content.</summary>
    private const string DuplicateCatalogContent = "Same=1\nSame=2";

    /// <summary>Verifies complete catalogs generate both normal and reactive namespace variants.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CompleteCatalogs_WhenGenerated_EmitBothSymbolEnumsAndNamespaceVariants()
    {
        ImmutableArray<AdditionalText> catalogs = CreateCompleteCatalogs();
        CSharpParseOptions standardParseOptions = new(LanguageVersion.Latest);
        CSharpParseOptions reactiveParseOptions = new(
            LanguageVersion.Latest,
            preprocessorSymbols: [ReactivePreprocessorSymbol]);
        GeneratorRunResult standard = RunGenerator(catalogs, standardParseOptions);
        GeneratorRunResult reactive = RunGenerator(
            catalogs,
            reactiveParseOptions);

        await Assert.That(standard.Diagnostics).IsEmpty();
        await Assert.That(standard.GeneratedSources).Count().IsEqualTo(ExpectedGeneratedSourceCount);
        await Assert.That(ContainsGeneratedSource(standard, StandardNamespace)).IsTrue();
        await Assert.That(ContainsGeneratedSource(standard, RegularCatalogDocumentation)).IsTrue();
        await Assert.That(reactive.Diagnostics).IsEmpty();
        await Assert.That(reactive.GeneratedSources).Count().IsEqualTo(ExpectedGeneratedSourceCount);
        await Assert.That(ContainsGeneratedSource(reactive, ReactiveNamespace)).IsTrue();
    }

    /// <summary>Verifies malformed, duplicate, and incomplete catalogs report their documented diagnostics.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task InvalidCatalogs_WhenGenerated_ReportSpecificDiagnostics()
    {
        CSharpParseOptions parseOptions = new(LanguageVersion.Latest);
        GeneratorRunResult malformed = RunGenerator(
            [new InMemoryAdditionalText(RegularCatalogPath, MalformedCatalogContent)],
            parseOptions);
        GeneratorRunResult duplicate = RunGenerator(
            [new InMemoryAdditionalText(RegularCatalogPath, DuplicateCatalogContent)],
            parseOptions);
        GeneratorRunResult missingText = RunGenerator(
            [new InMemoryAdditionalText(RegularCatalogPath, null)],
            parseOptions);

        await Assert.That(ContainsDiagnostic(malformed, MalformedCatalogDiagnosticId)).IsTrue();
        await Assert.That(ContainsDiagnostic(duplicate, DuplicateCatalogEntryDiagnosticId)).IsTrue();
        await Assert.That(ContainsDiagnostic(missingText, MissingCatalogTextDiagnosticId)).IsTrue();
    }

    /// <summary>Runs the generator with the supplied catalogs and parse options.</summary>
    /// <param name="catalogs">The additional catalog files.</param>
    /// <param name="parseOptions">The compilation parse options.</param>
    /// <returns>The generator result.</returns>
    private static GeneratorRunResult RunGenerator(
        ImmutableArray<AdditionalText> catalogs,
        CSharpParseOptions parseOptions)
    {
        CSharpCompilation compilation = CSharpCompilation.Create(
            "GeneratorTests",
            syntaxTrees: [CSharpSyntaxTree.ParseText("internal sealed class Marker;", parseOptions)]);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new SymbolEnumGenerator().AsSourceGenerator()],
            catalogs,
            parseOptions);
        driver = driver.RunGenerators(compilation);
        return driver.GetRunResult().Results.Single();
    }

    /// <summary>Determines whether a generator result contains a diagnostic with the required identifier.</summary>
    /// <param name="result">The generator result.</param>
    /// <param name="diagnosticId">The expected diagnostic identifier.</param>
    /// <returns><c>true</c> when the result contains the diagnostic.</returns>
    private static bool ContainsDiagnostic(in GeneratorRunResult result, string diagnosticId)
    {
        foreach (Diagnostic diagnostic in result.Diagnostics)
        {
            if (string.Equals(diagnostic.Id, diagnosticId, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Determines whether a generated source contains the required text.</summary>
    /// <param name="result">The generator result.</param>
    /// <param name="expectedText">The required generated text.</param>
    /// <returns><c>true</c> when a generated source contains the text.</returns>
    private static bool ContainsGeneratedSource(in GeneratorRunResult result, string expectedText)
    {
        foreach (GeneratedSourceResult source in result.GeneratedSources)
        {
            if (source.SourceText.ToString().Contains(expectedText, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Creates complete regular and filled catalogs.</summary>
    /// <returns>The complete in-memory catalogs.</returns>
    private static ImmutableArray<AdditionalText> CreateCompleteCatalogs()
    {
        List<string> memberDefinitions = new(ExpectedMemberCount);
        for (int index = 0; index < ExpectedMemberCount; index++)
        {
            memberDefinitions.Add($"Icon{index}={index}");
        }

        string members = string.Join("\n", memberDefinitions);
        return
        [
            new InMemoryAdditionalText(RegularCatalogPath, members),
            new InMemoryAdditionalText(FilledCatalogPath, members),
        ];
    }

    /// <summary>Provides an in-memory Roslyn additional file.</summary>
    /// <param name="path">The virtual file path.</param>
    /// <param name="content">The optional source content.</param>
    private sealed class InMemoryAdditionalText(string path, string? content) : AdditionalText
    {
        /// <inheritdoc/>
        public override string Path { get; } = path;

        /// <inheritdoc/>
        public override SourceText? GetText(CancellationToken cancellationToken = default) =>
            content is null ? null : SourceText.From(content);
    }
}
