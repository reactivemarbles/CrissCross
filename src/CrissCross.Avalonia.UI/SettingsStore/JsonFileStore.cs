// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrissCross.Avalonia.UI.Storage;

/// <summary>
/// An implementation of IStore that saves data to a JSON file.
/// </summary>
public class JsonFileStore : IStore
{
    private static readonly JsonSerializerOptions SerializerOptions = CreateOptions();

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in a per-user folder.
    /// </summary>
    public JsonFileStore()
        : this(true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in a per-user or per-machine folder.
    /// </summary>
    /// <param name="perUser">Specified if a per-user or per-machine folder will be used for storing the data.</param>
    public JsonFileStore(bool perUser)
        : this(ConstructPath(perUser ? Environment.SpecialFolder.ApplicationData : Environment.SpecialFolder.CommonApplicationData))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileStore"/> class.
    /// Creates a JsonFileStore that will store files in the specified folder.
    /// </summary>
    /// <param name="storeFolderPath">The folder inside which the json files for tracked objects will be stored.</param>
    public JsonFileStore(string storeFolderPath)
    {
        FolderPath = storeFolderPath;
    }

    /// <summary>
    /// Gets or sets the folder in which the store files will be located.
    /// </summary>
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

    /// <inheritdoc/>
    public void SetData(string id, IDictionary<string, object?> values)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(values);

        var filePath = GetFilePath(id);
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
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

    /// <inheritdoc/>
    public IEnumerable<string> ListIds()
    {
        if (!Directory.Exists(FolderPath))
        {
            return [];
        }

        return Directory.GetFiles(FolderPath, "*.json").Select(Path.GetFileNameWithoutExtension)!;
    }

    /// <inheritdoc/>
    public void ClearData(string id)
    {
        var filePath = GetFilePath(id);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <inheritdoc/>
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
                companyPart = $"{companyAttribute.Company}{Path.DirectorySeparatorChar}";
            }

            var titleAttribute = (AssemblyTitleAttribute?)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute?.Title))
            {
                appNamePart = $"{titleAttribute.Title}{Path.DirectorySeparatorChar}";
            }
        }

        return Path.Combine(Environment.GetFolderPath(baseFolder), companyPart + appNamePart);
    }

    private string GetFilePath(string id) => Path.Combine(FolderPath, $"{id}.json");
}
