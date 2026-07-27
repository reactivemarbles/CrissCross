// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Storage;
#else
namespace CrissCross.WPF.UI.Storage;
#endif

/// <summary>An implementation of IStore that saves data to a json file.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JsonFileStore"/> class.
/// Creates a JsonFileStore that will store files in the specified folder.
/// </remarks>
/// <param name="storeFolderPath">The folder inside which the json files for tracked objects will be stored.</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class JsonFileStore(string storeFolderPath) : IStore
{
    /// <summary>Provides the SerializerOptions member.</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Converters = { new IPAddressJsonConverter(), },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in a per-user folder (%appdata%\[companyname]\[productname]).
    /// </summary>
    /// <remarks>
    /// CompanyName and ProductName are read from the entry assembly's attributes.
    /// </remarks>
    public JsonFileStore()
        : this(true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in a per-user or per-machine folder. (%appdata% or
    /// %allusersprofile% + \[companyname]\[productname]).
    /// </summary>
    /// <remarks>
    /// CompanyName and ProductName are read from the entry assembly's attributes.
    /// </remarks>
    /// <param name="perUser">Specified if a per-user or per-machine folder will be used for storing the data.</param>
    public JsonFileStore(bool perUser)
        : this(
            ConstructPath(
                perUser
                    ? Environment.SpecialFolder.ApplicationData
                    : Environment.SpecialFolder.CommonApplicationData)) { }

    /// <summary>Initializes a new instance of the <see cref="JsonFileStore"/> class.</summary>
    /// <param name="folder">The folder inside which the json files for tracked objects will be stored.</param>
    public JsonFileStore(Environment.SpecialFolder folder)
        : this(ConstructPath(folder)) { }

    /// <summary>Gets or sets the folder in which the store files will be located.</summary>
    public string FolderPath { get; set; } = storeFolderPath;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Loads values from the json file into a dictionary.</summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    /// A Dictionary of string and object.
    /// </returns>
    public IDictionary<string, object?> GetData(string id)
    {
        var filePath = GetfilePath(id);
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
                AddEntryData(element, result);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }

        return result;
    }

    /// <summary>Stores the values as a json file.</summary>
    /// <param name="id">The identifier.</param>
    /// <param name="values">The values.</param>
    public void SetData(string id, IDictionary<string, object?> values)
    {
        ThrowHelper.ThrowIfNull(id, nameof(id));

        ThrowHelper.ThrowIfNull(values, nameof(values));

        var filePath = GetfilePath(id);
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory!);
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
                    writer.WriteString(nameof(Type), kvp.Value.GetType().AssemblyQualifiedName);
                }
                else
                {
                    writer.WriteString(nameof(Type), string.Empty);
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

    /// <summary>Lists the ids.</summary>
    /// <returns>A string array.</returns>
    public IEnumerable<string> ListIds()
    {
        List<string> identifiers = [];
        foreach (var filePath in Directory.GetFiles(FolderPath, "*.json"))
        {
            identifiers.Add(Path.GetFileNameWithoutExtension(filePath));
        }

        return identifiers;
    }

    /// <summary>Clears the data.</summary>
    /// <param name="id">The identifier.</param>
    public void ClearData(string id) => File.Delete(GetfilePath(id));

    /// <summary>Clears all.</summary>
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
            JsonValueKind.Array => DeserializeArray(element),
            JsonValueKind.Object => DeserializeObject(element),
            _ => null,
        };

    /// <summary>Deserializes a JSON array.</summary>
    /// <param name="element">The array element.</param>
    /// <returns>The materialized array values.</returns>
    private static List<object?> DeserializeArray(JsonElement element)
    {
        List<object?> values = [];
        foreach (var value in element.EnumerateArray())
        {
            values.Add(DeserializeUnknown(value));
        }

        return values;
    }

    /// <summary>Deserializes a JSON object.</summary>
    /// <param name="element">The object element.</param>
    /// <returns>The materialized object properties.</returns>
    private static Dictionary<string, object?> DeserializeObject(JsonElement element)
    {
        Dictionary<string, object?> values = [];
        foreach (var property in element.EnumerateObject())
        {
            values[property.Name] = DeserializeUnknown(property.Value);
        }

        return values;
    }

    /// <summary>Deserializes a JSON number.</summary>
    /// <param name="element">The element value.</param>
    /// <returns>The deserialized number.</returns>
    private static object? DeserializeNumber(JsonElement element)
    {
        if (element.TryGetInt64(out var longValue))
        {
            return longValue;
        }

        return element.TryGetDouble(out var doubleValue) ? doubleValue : null;
    }

    /// <summary>Adds one JSON entry to the result dictionary.</summary>
    /// <param name="element">The JSON element.</param>
    /// <param name="result">The result dictionary.</param>
    private static void AddEntryData(JsonElement element, Dictionary<string, object?> result)
    {
        if (!TryGetEntryName(element, out var name))
        {
            return;
        }

        if (!element.TryGetProperty("Value", out var valueProp))
        {
            result[name] = null;
            return;
        }

        result[name] = DeserializeEntryValue(valueProp, GetEntryValueType(element));
    }

    /// <summary>Deserializes a JSON entry value.</summary>
    /// <param name="valueProp">The value property.</param>
    /// <param name="valueType">The configured value type.</param>
    /// <returns>The deserialized value.</returns>
    private static object? DeserializeEntryValue(JsonElement valueProp, Type? valueType)
    {
        try
        {
            return valueType is null
                ? DeserializeUnknown(valueProp)
                : JsonSerializer.Deserialize(valueProp.GetRawText(), valueType, SerializerOptions);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            return null;
        }
    }

    /// <summary>Gets the entry value type from the JSON element.</summary>
    /// <param name="element">The JSON element.</param>
    /// <returns>The value type, or <see langword="null"/>.</returns>
    private static Type? GetEntryValueType(JsonElement element)
    {
        if (!element.TryGetProperty(nameof(Type), out var typeProp) || typeProp.ValueKind != JsonValueKind.String)
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
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            return null;
        }
    }

    /// <summary>Gets the entry name from the JSON element.</summary>
    /// <param name="element">The JSON element.</param>
    /// <param name="name">The entry name.</param>
    /// <returns><c>true</c> if the name was found; otherwise, <c>false</c>.</returns>
    private static bool TryGetEntryName(JsonElement element, [NotNullWhen(true)] out string? name)
    {
        name = null;
        if (
            element.ValueKind != JsonValueKind.Object
            || !element.TryGetProperty("Name", out var nameProp)
            || nameProp.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        name = nameProp.GetString();
        return !string.IsNullOrEmpty(name);
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
                companyPart = $"{companyAttribute?.Company}\\";
            }

            var titleAttribute = (AssemblyTitleAttribute?)
                Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute?.Title))
            {
                appNamePart = $"{titleAttribute?.Title}\\";
            }
        }

        return Path.Combine(Environment.GetFolderPath(baseFolder), companyPart + appNamePart);
    }

    /// <summary>Provides the GetfilePath member.</summary>
    /// <param name="id">The id value.</param>
    /// <returns>The result.</returns>
    private string GetfilePath(string id) => Path.Combine(FolderPath, $"{id}.json");

    /// <summary>Provides the IPAddressJsonConverter member.</summary>
    private sealed class IPAddressJsonConverter : JsonConverter<IPAddress>
    {
        public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                return null;
            }

            var s = reader.GetString();
            return !string.IsNullOrEmpty(s) && IPAddress.TryParse(s, out var ip) ? ip : null;
        }

        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value?.ToString());
    }
}
