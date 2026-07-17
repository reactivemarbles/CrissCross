// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrissCross.Avalonia.UI.Storage;

/// <summary>An implementation of IStore that saves data to a JSON file.</summary>
[System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
    "Arbitrary runtime value types require dynamic JSON serialization metadata.")]
[System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
    "Arbitrary runtime value types and assembly metadata may be removed by trimming.")]
public class JsonFileStore : IStore
{
    /// <summary>Name of the serialized type metadata property.</summary>
    private const string TypePropertyName = nameof(Type);

    /// <summary>Provides the SerializerOptions member.</summary>
    private static readonly JsonSerializerOptions SerializerOptions = CreateOptions();

    /// <summary>Initializes a new instance of the <see cref="JsonFileStore"/> class.</summary>
    public JsonFileStore()
        : this(true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in a per-user or per-machine folder.
    /// </summary>
    /// <param name="perUser">Specified if a per-user or per-machine folder will be used for storing the data.</param>
    public JsonFileStore(bool perUser)
        : this(
            ConstructPath(
                perUser
                    ? Environment.SpecialFolder.ApplicationData
                    : Environment.SpecialFolder.CommonApplicationData)) { }

    /// <summary>Initializes a new instance of the <see cref="JsonFileStore"/> class.</summary>
    /// <param name="storeFolderPath">The folder inside which the json files for tracked objects will be stored.</param>
    public JsonFileStore(string storeFolderPath)
    {
        FolderPath = storeFolderPath;
    }

    /// <summary>Gets or sets the folder in which the store files will be located.</summary>
    public string FolderPath { get; set; }

    /// <inheritdoc/>
    public IDictionary<string, object?> GetData(string id)
    {
        var filePath = GetFilePath(id);
        Dictionary<string, object?> result = new(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(filePath))
        {
            return result;
        }

        try
        {
            using var fs = File.OpenRead(filePath);
            using var doc = JsonDocument.Parse(fs);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return result;
            }

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                if (TryReadEntry(element, out var name, out var value))
                {
                    result[name] = value;
                }
            }
        }
        catch (IOException)
        {
            // swallow errors to keep previous behaviour (ignore corrupted file)
        }
        catch (JsonException)
        {
            // swallow errors to keep previous behaviour (ignore corrupted file)
        }
        catch (UnauthorizedAccessException)
        {
            // swallow errors to keep previous behaviour (ignore corrupted file)
        }

        return result;
    }

    /// <inheritdoc/>
    public void SetData(string id, IDictionary<string, object?> values)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(values);

        var filePath = GetFilePath(id);
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartArray();
            foreach (var kvp in values)
            {
                writer.WriteStartObject();
                if (kvp.Value is not null)
                {
                    writer.WriteString(TypePropertyName, kvp.Value.GetType().AssemblyQualifiedName);
                }
                else
                {
                    writer.WriteString(TypePropertyName, string.Empty);
                }

                writer.WriteString("Name", kvp.Key);
                writer.WritePropertyName("Value");
                if (kvp.Value is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, kvp.Value, kvp.Value.GetType(), SerializerOptions);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        File.WriteAllBytes(filePath, ms.ToArray());
    }

    /// <inheritdoc/>
    public IEnumerable<string> ListIds()
    {
        return !Directory.Exists(FolderPath)
            ? []
            : Directory.GetFiles(FolderPath, "*.json").Select(Path.GetFileNameWithoutExtension).OfType<string>();
    }

    /// <inheritdoc/>
    public void ClearData(string id)
    {
        var filePath = GetFilePath(id);
        if (!File.Exists(filePath))
        {
            return;
        }

        File.Delete(filePath);
    }

    /// <inheritdoc/>
    public void ClearAll()
    {
        foreach (var id in ListIds())
        {
            ClearData(id);
        }
    }

    /// <summary>Provides the DeserializeUnknown member.</summary>
    /// <param name="element">The element value.</param>
    /// <returns>The result.</returns>
    private static object? DeserializeUnknown(JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => DeserializeNumber(element),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => element.EnumerateArray().Select(DeserializeUnknown).ToList(),
            JsonValueKind.Object => element
                .EnumerateObject()
                .ToDictionary(p => p.Name, p => DeserializeUnknown(p.Value)),
            _ => null,
        };

    /// <summary>Tries to read one stored entry from a JSON element.</summary>
    /// <param name="element">The source JSON element.</param>
    /// <param name="name">The entry name.</param>
    /// <param name="value">The entry value.</param>
    /// <returns><see langword="true"/> when an entry name was found.</returns>
    private static bool TryReadEntry(JsonElement element, out string name, out object? value)
    {
        name = string.Empty;
        value = null;

        if (element.ValueKind != JsonValueKind.Object || !TryReadName(element, out name))
        {
            return false;
        }

        var valueType = ReadValueType(element);
        if (!element.TryGetProperty("Value", out var valueProp))
        {
            return true;
        }

        value = DeserializeValue(valueProp, valueType);
        return true;
    }

    /// <summary>Tries to read an entry name.</summary>
    /// <param name="element">The source JSON element.</param>
    /// <param name="name">The entry name.</param>
    /// <returns><see langword="true"/> when a non-empty name was found.</returns>
    private static bool TryReadName(JsonElement element, out string name)
    {
        name = string.Empty;
        if (!element.TryGetProperty("Name", out var nameProp) || nameProp.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        name = nameProp.GetString() ?? string.Empty;
        return name.Length != 0;
    }

    /// <summary>Reads the stored value type when it can be resolved.</summary>
    /// <param name="element">The source JSON element.</param>
    /// <returns>The resolved type, or <see langword="null"/>.</returns>
    private static Type? ReadValueType(JsonElement element)
    {
        if (!element.TryGetProperty(TypePropertyName, out var typeProp) || typeProp.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        var typeName = typeProp.GetString();
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        try
        {
            return Type.GetType(typeName, throwOnError: false, ignoreCase: false);
        }
        catch (TypeLoadException)
        {
            return null;
        }
    }

    /// <summary>Deserializes one stored value.</summary>
    /// <param name="valueProp">The JSON value element.</param>
    /// <param name="valueType">The resolved value type.</param>
    /// <returns>The deserialized value.</returns>
    private static object? DeserializeValue(JsonElement valueProp, Type? valueType)
    {
        try
        {
            return valueType is null
                ? DeserializeUnknown(valueProp)
                : JsonSerializer.Deserialize(valueProp.GetRawText(), valueType, SerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Provides the CreateOptions member.</summary>
    /// <returns>The result.</returns>
    private static JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };
    }

    /// <summary>Provides the DeserializeNumber member.</summary>
    /// <param name="element">The element value.</param>
    /// <returns>The result.</returns>
    private static object? DeserializeNumber(JsonElement element)
    {
        if (element.TryGetInt64(out var longValue))
        {
            return longValue;
        }

        return element.TryGetDouble(out var doubleValue) ? doubleValue : null;
    }

    /// <summary>Provides the ConstructPath member.</summary>
    /// <param name="baseFolder">The baseFolder value.</param>
    /// <returns>The result.</returns>
    private static string ConstructPath(Environment.SpecialFolder baseFolder)
    {
        var companyPart = string.Empty;
        var appNamePart = string.Empty;

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is not null)
        {
            var companyAttribute = (AssemblyCompanyAttribute?)
                Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCompanyAttribute));
            if (!string.IsNullOrEmpty(companyAttribute?.Company))
            {
                companyPart = $"{companyAttribute.Company}{Path.DirectorySeparatorChar}";
            }

            var titleAttribute = (AssemblyTitleAttribute?)
                Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute?.Title))
            {
                appNamePart = $"{titleAttribute.Title}{Path.DirectorySeparatorChar}";
            }
        }

        return Path.Combine(Environment.GetFolderPath(baseFolder), companyPart + appNamePart);
    }

    /// <summary>Provides the GetFilePath member.</summary>
    /// <param name="id">The id value.</param>
    /// <returns>The result.</returns>
    private string GetFilePath(string id) => Path.Combine(FolderPath, $"{id}.json");
}
