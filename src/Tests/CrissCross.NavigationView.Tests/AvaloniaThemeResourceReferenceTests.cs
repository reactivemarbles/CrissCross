// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies Avalonia theme resource references resolve to project or verified Fluent keys.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed partial class AvaloniaThemeResourceReferenceTests
{
    /// <summary>Provides the Avalonia UI project directory name.</summary>
    private const string AvaloniaUiProject = "CrissCross.Avalonia.UI";

    /// <summary>Provides the Avalonia UI gallery project directory name.</summary>
    private const string AvaloniaGalleryProject = "CrissCross.Avalonia.UI.Gallery";

    /// <summary>Provides the markup file search pattern.</summary>
    private const string MarkupFilePattern = "*.axaml";

    /// <summary>Provides the x namespace used by Avalonia resource dictionaries.</summary>
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>Provides resource keys that are verified in Avalonia.Themes.Fluent.</summary>
    private static readonly HashSet<string> VerifiedFluentResourceKeys = new(StringComparer.Ordinal) { "SystemAccentColorLight1", "SystemAccentColorLight2" };

    /// <summary>Provides the source root used to locate Avalonia markup files.</summary>
    private static readonly string SourceRoot = LocateSourceRoot();

    /// <summary>Verifies all Avalonia UI and gallery resource references use defined resource keys.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaMarkupResourceReferences_ResolveToProjectOrVerifiedFluentResources()
    {
        var markupFiles = GetMarkupFiles();
        var definedKeys = ReadDefinedResourceKeys(markupFiles);
        var unresolvedReferences = GetUnresolvedResourceReferences(markupFiles, definedKeys);

        await Assert.That(unresolvedReferences).IsEmpty();
    }

    /// <summary>Reads all Avalonia UI and gallery markup files.</summary>
    /// <returns>The discovered markup files.</returns>
    private static string[] GetMarkupFiles()
    {
        var files = new List<string>();
        foreach (var projectName in new[] { AvaloniaUiProject, AvaloniaGalleryProject })
        {
            files.AddRange(Directory.GetFiles(
                Path.Combine(SourceRoot, projectName),
                MarkupFilePattern,
                SearchOption.AllDirectories));
        }

        files.Sort(StringComparer.Ordinal);
        return files.ToArray();
    }

    /// <summary>Reads every keyed project resource from the markup files.</summary>
    /// <param name="markupFiles">The markup files to inspect.</param>
    /// <returns>The defined resource keys.</returns>
    private static HashSet<string> ReadDefinedResourceKeys(IEnumerable<string> markupFiles)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var markupFile in markupFiles)
        {
            foreach (var element in XDocument.Load(markupFile).Descendants())
            {
                AddAttributeValue(keys, element.Attribute(XamlNamespace + "Key"));
                AddAttributeValue(keys, element.Attribute("Key"));
            }
        }

        return keys;
    }

    /// <summary>Gets resource references that are not resolved by project or verified Fluent resource keys.</summary>
    /// <param name="markupFiles">The markup files to inspect.</param>
    /// <param name="definedKeys">The project-defined resource keys.</param>
    /// <returns>The unresolved references formatted for assertion output.</returns>
    private static string[] GetUnresolvedResourceReferences(IEnumerable<string> markupFiles, HashSet<string> definedKeys)
    {
        var unresolvedReferences = new List<string>();
        foreach (var markupFile in markupFiles)
        {
            var text = File.ReadAllText(markupFile);
            var matches = ResourceReferenceExpression().Matches(text);
            for (var matchIndex = 0; matchIndex < matches.Count; matchIndex++)
            {
                var match = (Match)matches[matchIndex];
                var key = match.Groups[1].Value;
                if (definedKeys.Contains(key) || VerifiedFluentResourceKeys.Contains(key) || IsNativeControlTheme(key))
                {
                    continue;
                }

                unresolvedReferences.Add($"{Path.GetRelativePath(SourceRoot, markupFile)}:{GetLineNumber(text, match.Index)} uses undefined resource '{key}'.");
            }
        }

        unresolvedReferences.Sort(StringComparer.Ordinal);
        return unresolvedReferences.ToArray();
    }

    /// <summary>Resolves a typed native control-theme key against the running application's resources.</summary>
    /// <param name="key">The complete resource key expression.</param>
    /// <returns>Whether the referenced native theme exists.</returns>
    private static bool IsNativeControlTheme(string key)
    {
        const string typePrefix = "{x:Type ";
        const string primitivesPrefix = "primitives:";
        if (!key.StartsWith(typePrefix, StringComparison.Ordinal) || !key.EndsWith('}'))
        {
            return false;
        }

        var typeName = key[typePrefix.Length..^1].Trim();
        var fullName = typeName.StartsWith(primitivesPrefix, StringComparison.Ordinal)
            ? $"Avalonia.Controls.Primitives.{typeName[primitivesPrefix.Length..]}"
            : $"Avalonia.Controls.{typeName}";
        var type = typeof(Control).Assembly.GetType(fullName);
        return type is not null
            && Application.Current!.TryGetResource(type, ThemeVariant.Default, out var resource)
            && resource is ControlTheme theme
            && theme.TargetType == type;
    }

    /// <summary>Adds an attribute value to a resource key set.</summary>
    /// <param name="keys">The key set.</param>
    /// <param name="attribute">The attribute to read.</param>
    private static void AddAttributeValue(HashSet<string> keys, XAttribute? attribute)
    {
        if (string.IsNullOrWhiteSpace(attribute?.Value))
        {
            return;
        }

        _ = keys.Add(attribute.Value);
    }

    /// <summary>Gets the one-based line number for a character index.</summary>
    /// <param name="text">The complete file text.</param>
    /// <param name="index">The character index.</param>
    /// <returns>The one-based line number.</returns>
    private static int GetLineNumber(string text, int index)
    {
        var line = 1;
        for (var i = 0; i < index; i++)
        {
            if (text[i] == '\n')
            {
                line++;
            }
        }

        return line;
    }

    /// <summary>Locates the source root while supporting MTP's test working directory.</summary>
    /// <returns>The source root path.</returns>
    private static string LocateSourceRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "CrissCross.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate CrissCross.slnx from the current test working directory.");
    }

    /// <summary>Gets the resource reference parser.</summary>
    /// <returns>The resource reference parser.</returns>
    [GeneratedRegex(@"\{(?:DynamicResource|StaticResource)\s+(\{x:Type\s+[^\}]+\}|[^\},\s]+)", RegexOptions.CultureInvariant)]
    private static partial Regex ResourceReferenceExpression();
}
