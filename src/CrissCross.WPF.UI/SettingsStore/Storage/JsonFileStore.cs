// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrissCross.WPF.UI.Storage;

/// <summary>
/// An implementation of IStore that saves data to a json file.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JsonFileStore"/> class.
/// Creates a JsonFileStore that will store files in the specified folder.
/// </remarks>
/// <param name="storeFolderPath">The folder inside which the json files for tracked objects will be stored.</param>
public class JsonFileStore(string storeFolderPath) : IStore
{
    private static readonly JsonSerializerOptions SerializerOptions = CreateOptions();

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in a per-user folder (%appdata%\[companyname]\[productname]).
    /// </summary>
    /// <remarks>
    /// CompanyName and ProductName are read from the entry assembly's attributes.
    /// </remarks>
    public JsonFileStore()
        : this(true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in a per-user or per-machine folder. (%appdata% or %allusersprofile%  + \[companyname]\[productname]).
    /// </summary>
    /// <param name="perUser">Specified if a per-user or per-machine folder will be used for storing the data.</param>
    /// <remarks>
    /// CompanyName and ProductName are read from the entry assembly's attributes.
    /// </remarks>
    public JsonFileStore(bool perUser)
        : this(ConstructPath(perUser ? Environment.SpecialFolder.ApplicationData : Environment.SpecialFolder.CommonApplicationData))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in the specified folder.
    /// </summary>
    /// <param name="folder">The folder inside which the json files for tracked objects will be stored.</param>
    public JsonFileStore(Environment.SpecialFolder folder)
        : this(ConstructPath(folder))
    {
    }

    /// <summary>
    /// Gets or sets the folder in which the store files will be located.
    /// </summary>
    public string FolderPath { get; set; } = storeFolderPath;

    /// <summary>
    /// Loads values from the json file into a dictionary.
    /// </summary>
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
                if (element.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                if (!element.TryGetProperty("Name", out var nameProp) || nameProp.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                var name = nameProp.GetString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                Type? valueType = null;
                if (element.TryGetProperty("Type", out var typeProp) && typeProp.ValueKind == JsonValueKind.String)
                {
                    var typeName = typeProp.GetString();
                    if (!string.IsNullOrWhiteSpace(typeName))
                    {
                        try
                        {
                            valueType = Type.GetType(typeName!, throwOnError: false, ignoreCase: false);
                        }
                        catch
                        {
                            // ignore resolution errors
                        }
                    }
                }

                object? value = null;
                if (element.TryGetProperty("Value", out var valueProp))
                {
                    try
                    {
                        if (valueType == null)
                        {
                            // Fallback: try primitive kinds
                            value = DeserializeUnknown(valueProp);
                        }
                        else
                        {
                            var raw = valueProp.GetRawText();
                            value = JsonSerializer.Deserialize(raw, valueType, SerializerOptions);
                        }
                    }
                    catch
                    {
                        value = null;
                    }
                }

                result[name!] = value;
            }
        }
        catch
        {
            // swallow errors to keep previous behaviour (ignore corrupted file)
        }

        return result;
    }

    /// <summary>
    /// Stores the values as a json file.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="values">The values.</param>
    public void SetData(string id, IDictionary<string, object?> values)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        var filePath = GetfilePath(id);
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartArray();
            foreach (var kvp in values)
            {
                writer.WriteStartObject();
                if (kvp.Value != null)
                {
                    writer.WriteString("Type", kvp.Value.GetType().AssemblyQualifiedName);
                }
                else
                {
                    writer.WriteString("Type", string.Empty);
                }

                writer.WriteString("Name", kvp.Key);
                writer.WritePropertyName("Value");
                if (kvp.Value == null)
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

    /// <summary>
    /// Lists the ids.
    /// </summary>
    /// <returns>A string array.</returns>
    public IEnumerable<string> ListIds() => Directory.GetFiles(FolderPath, "*.json").Select(Path.GetFileNameWithoutExtension)!;

    /// <summary>
    /// Clears the data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void ClearData(string id) => File.Delete(GetfilePath(id));

    /// <summary>
    /// Clears all.
    /// </summary>
    public void ClearAll()
    {
        foreach (var id in ListIds())
        {
            ClearData(id);
        }
    }

    private static object? DeserializeUnknown(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.TryGetDouble(out var d) ? d : null,
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        JsonValueKind.Array => element.EnumerateArray().Select(DeserializeUnknown).ToList(),
        JsonValueKind.Object => element.EnumerateObject().ToDictionary(p => p.Name, p => DeserializeUnknown(p.Value)),
        _ => null
    };

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };
        options.Converters.Add(new IPAddressJsonConverter());
        return options;
    }

    private static string ConstructPath(Environment.SpecialFolder baseFolder)
    {
        var companyPart = string.Empty;
        var appNamePart = string.Empty;

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            var companyAttribute = (AssemblyCompanyAttribute?)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCompanyAttribute));
            if (!string.IsNullOrEmpty(companyAttribute?.Company))
            {
                companyPart = $"{companyAttribute?.Company}\\";
            }

            var titleAttribute = (AssemblyTitleAttribute?)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute?.Title))
            {
                appNamePart = $"{titleAttribute?.Title}\\";
            }
        }

        return Path.Combine(Environment.GetFolderPath(baseFolder), companyPart + appNamePart);
    }

    private string GetfilePath(string id) => Path.Combine(FolderPath, $"{id}.json");

    private sealed class IPAddressJsonConverter : JsonConverter<IPAddress>
    {
        public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (!string.IsNullOrEmpty(s) && IPAddress.TryParse(s, out var ip))
                {
                    return ip;
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value?.ToString());
    }
}
