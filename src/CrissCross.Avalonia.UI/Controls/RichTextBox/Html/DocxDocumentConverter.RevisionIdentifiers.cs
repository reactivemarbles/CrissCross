// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Xml.Linq;
using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Preserves logical revision identities while assigning valid numeric Word identifiers.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The inert custom XML part containing editor revision identities.</summary>
    private const string RevisionIdentifiersPart = "customXml/crisscross-revision-identifiers.xml";

    /// <summary>The native identifier metadata attribute.</summary>
    private const string RevisionNativeAttribute = "native";

    /// <summary>The logical identifier metadata attribute.</summary>
    private const string RevisionLogicalAttribute = "logical";

    /// <summary>The revision identity metadata namespace.</summary>
    private static readonly XNamespace RevisionIdentifiersNamespace = "urn:crisscross:revision-identifiers:1";

    /// <summary>Gets an editor identity from a native Word revision number.</summary>
    /// <param name="parts">The Word package.</param>
    /// <param name="native">The native revision number.</param>
    /// <returns>The logical identity, or the native number for ordinary Word documents.</returns>
    private static string? LogicalRevisionIdentifier(Dictionary<string, byte[]> parts, string? native)
    {
        if (native is null || !parts.TryGetValue(RevisionIdentifiersPart, out var bytes))
        {
            return native;
        }

        var root = ReadXml(bytes).Root;
        foreach (var identifier in root?.Elements(RevisionIdentifiersNamespace + "revision") ?? [])
        {
            if ((string?)identifier.Attribute(RevisionNativeAttribute) == native)
            {
                return (string?)identifier.Attribute(RevisionLogicalAttribute) ?? native;
            }
        }

        return native;
    }

    /// <summary>Assigns numeric Word IDs independently of user-supplied revision names.</summary>
    private sealed partial class PackageWriter
    {
        /// <summary>The native revision numbers already written by this story.</summary>
        private readonly HashSet<string> _writtenRevisionIdentifiers = [with(StringComparer.Ordinal)];

        /// <summary>The logical identities of emitted native revisions.</summary>
        private readonly Dictionary<string, string> _revisionIdentityMap = [with(StringComparer.Ordinal)];

        /// <summary>Assigns one unique numeric Word identifier to an emitted revision container.</summary>
        /// <param name="element">The editor revision element.</param>
        /// <param name="logical">The user-supplied revision name.</param>
        /// <returns>The native Word identifier.</returns>
        private string NativeRevisionIdentifier(IElement element, string logical)
        {
            var preferred = element.GetAttribute("data-docx-native-revision-id") ?? logical;
            var native = int.TryParse(preferred, NumberStyles.None, CultureInfo.InvariantCulture, out var numeric)
                ? numeric.ToString(CultureInfo.InvariantCulture)
                : NextIdentifier();
            while (!_writtenRevisionIdentifiers.Add(native))
            {
                native = NextIdentifier();
            }

            if (native != logical)
            {
                _revisionIdentityMap[native] = logical;
            }

            return native;
        }

        /// <summary>Writes inert revision identity metadata for future editor round trips.</summary>
        private void SaveRevisionIdentifiers()
        {
            if (_revisionIdentityMap.Count == 0 && !_parts.ContainsKey(RevisionIdentifiersPart))
            {
                return;
            }

            var root = new XElement(RevisionIdentifiersNamespace + "revisions");
            foreach (var identifier in _revisionIdentityMap)
            {
                root.Add(new XElement(
                    RevisionIdentifiersNamespace + "revision",
                    new XAttribute(RevisionNativeAttribute, identifier.Key),
                    new XAttribute(RevisionLogicalAttribute, identifier.Value)));
            }

            _parts[RevisionIdentifiersPart] = XmlBytes(root);
            AddContentType(RevisionIdentifiersPart, "application/xml");
            EnsureRevisionMetadataRelationship();
        }

        /// <summary>Attaches revision identity metadata as an ordinary custom XML part.</summary>
        private void EnsureRevisionMetadataRelationship()
        {
            foreach (var relationship in _rels.Values)
            {
                if (!relationship.External && relationship.Type.EndsWith("/customXml", StringComparison.Ordinal)
                    && ResolvePart(_path, relationship.Target) == RevisionIdentifiersPart)
                {
                    return;
                }
            }

            var owner = new Uri($"https://package.invalid/{_path}", UriKind.Absolute);
            var metadata = new Uri($"https://package.invalid/{RevisionIdentifiersPart}", UriKind.Absolute);
            _ = AddRelationship("customXml", owner.MakeRelativeUri(metadata).ToString(), false);
        }
    }
}
