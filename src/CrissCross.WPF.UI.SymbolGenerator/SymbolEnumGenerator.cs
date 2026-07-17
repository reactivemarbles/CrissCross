// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CrissCross.WPF.UI.SymbolGenerator;

/// <summary>Generates the Fluent symbol enums from their partitioned catalogs.</summary>
[Generator(LanguageNames.CSharp)]
public sealed class SymbolEnumGenerator : IIncrementalGenerator
{
    /// <summary>The expected number of values in each Fluent symbol enum.</summary>
    private const int ExpectedMemberCount = 7808;

    /// <summary>Reports a malformed catalog entry.</summary>
    private static readonly DiagnosticDescriptor InvalidCatalogEntry = new(
        "CCSG001",
        "Invalid symbol catalog entry",
        "Symbol catalog '{0}' contains the invalid entry '{1}'",
        "CrissCross.Symbols",
        DiagnosticSeverity.Error,
        true);

    /// <summary>Reports a catalog whose member count has drifted.</summary>
    private static readonly DiagnosticDescriptor IncorrectMemberCount = new(
        "CCSG002",
        "Incorrect symbol catalog member count",
        "Symbol catalog '{0}' contains {1} members; expected {2}",
        "CrissCross.Symbols",
        DiagnosticSeverity.Error,
        true);

    /// <summary>Reports a duplicate member name across catalog partitions.</summary>
    private static readonly DiagnosticDescriptor DuplicateMember = new(
        "CCSG003",
        "Duplicate symbol catalog member",
        "Symbol catalog '{0}' contains the duplicate member '{1}'",
        "CrissCross.Symbols",
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var catalogs = context
            .AdditionalTextsProvider.Where(static file =>
                file.Path.EndsWith(".symbols", StringComparison.OrdinalIgnoreCase))
            .Collect();
        var reactiveShim = context.ParseOptionsProvider.Select(
            static (options, _) =>
                options is CSharpParseOptions csharp
                && csharp.PreprocessorSymbolNames.Contains("REACTIVELIST_REACTIVE", StringComparer.Ordinal));

        context.RegisterSourceOutput(
            catalogs.Combine(reactiveShim),
            static (productionContext, input) =>
            {
                var generatedNamespace =
                    input.Right ? "CrissCross.Reactive.WPF.UI.Controls" : "CrissCross.WPF.UI.Controls";
                GenerateEnum(productionContext, input.Left, "SymbolRegular", "regular", generatedNamespace);
                GenerateEnum(productionContext, input.Left, "SymbolFilled", "filled", generatedNamespace);
            });
    }

    /// <summary>Generates one Fluent symbol enum.</summary>
    /// <param name="context">The source production context.</param>
    /// <param name="files">The available symbol catalog partitions.</param>
    /// <param name="enumName">The enum name.</param>
    /// <param name="style">The human-readable icon style.</param>
    /// <param name="generatedNamespace">The namespace for the generated enum.</param>
    private static void GenerateEnum(
        SourceProductionContext context,
        ImmutableArray<AdditionalText> files,
        string enumName,
        string style,
        string generatedNamespace)
    {
        var members = ReadMembers(context, files, enumName);
        if (members is null)
        {
            return;
        }

        if (members.Count != ExpectedMemberCount)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(IncorrectMemberCount, Location.None, enumName, members.Count, ExpectedMemberCount));
            return;
        }

        var source = new StringBuilder();
        _ = source.AppendLine("// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.");
        _ = source.AppendLine("// ReactiveUI and Contributors licenses this file to you under the MIT license.");
        _ = source.AppendLine("// See the LICENSE file in the project root for full license information.");
        _ = source.AppendLine();
        _ = source.Append("namespace ").Append(generatedNamespace).AppendLine(";");
        _ = source.AppendLine();
        _ = source.AppendLine("/// <summary>");
        _ = source
            .Append("/// Represents a list of ")
            .Append(style)
            .AppendLine(" Fluent System Icons <c>v.1.1.233</c>.");
        _ = source.AppendLine(
            "/// <para>May be converted to <see langword=\"char\"/> using <c>GetGlyph()</c> "
                + "or to <see langword=\"string\"/> using <c>GetString()</c>.</para>");
        _ = source.AppendLine("/// </summary>");
        _ = source.Append("public enum ").AppendLine(enumName);
        _ = source.AppendLine("{");

        foreach (var member in members)
        {
            var summary =
                member.Name == "Empty"
                    ? "Actually, this icon is not empty, but makes it easier to navigate."
                    : $"Represents the {member.Name} icon.";
            _ = source.Append("    /// <summary>").Append(summary).AppendLine("</summary>");
            _ = source.Append("    ").Append(member.Name).Append(" = ").Append(member.Value).AppendLine(",");
        }

        _ = source.AppendLine("}");
        context.AddSource($"{enumName}.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
    }

    /// <summary>Reads, validates, and combines the partitions for one symbol enum.</summary>
    /// <param name="context">The source production context.</param>
    /// <param name="files">The available symbol catalog partitions.</param>
    /// <param name="enumName">The enum name.</param>
    /// <returns>The validated members, or <see langword="null"/> when validation fails.</returns>
    private static List<SymbolMember>? ReadMembers(
        SourceProductionContext context,
        ImmutableArray<AdditionalText> files,
        string enumName)
    {
        var catalogFiles = files
            .Where(file => Path.GetFileName(file.Path).StartsWith(enumName + ".", StringComparison.Ordinal))
            .OrderBy(static file => file.Path, StringComparer.Ordinal)
            .ToArray();
        var members = new List<SymbolMember>(ExpectedMemberCount);
        var memberNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in catalogFiles)
        {
            var text = file.GetText(context.CancellationToken);
            if (text is null)
            {
                continue;
            }

            foreach (var line in text.Lines)
            {
                var entry = line.ToString().Trim();
                if (entry.Length == 0)
                {
                    continue;
                }

                var separatorIndex = entry.IndexOf('=');
                if (separatorIndex <= 0 || separatorIndex == entry.Length - 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(InvalidCatalogEntry, Location.None, file.Path, entry));
                    return null;
                }

                var name = entry.Remove(separatorIndex).Trim();
                var value = entry.Remove(0, separatorIndex + 1).Trim();
                if (!memberNames.Add(name))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DuplicateMember, Location.None, enumName, name));
                    return null;
                }

                members.Add(new(name, value));
            }
        }

        return members;
    }

    /// <summary>Represents one named symbol and its numeric value.</summary>
    /// <param name="name">The enum member name.</param>
    /// <param name="value">The enum member numeric value.</param>
    private sealed class SymbolMember(string name, string value)
    {
        /// <summary>Gets the enum member name.</summary>
        public string Name { get; } = name;

        /// <summary>Gets the enum member numeric value.</summary>
        public string Value { get; } = value;
    }
}
