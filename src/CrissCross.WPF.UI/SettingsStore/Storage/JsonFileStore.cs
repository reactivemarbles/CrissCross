// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
        List<StoreItem>? storeItems = null;
        if (File.Exists(filePath))
        {
            try
            {
                var fileContents = File.ReadAllText(filePath);
                storeItems = JsonConvert.DeserializeObject<List<StoreItem>>(fileContents, new StoreItemConverter(), new IPAddressConverter());
            }
            catch
            {
            }
        }

        storeItems ??= [];

        return storeItems.ToDictionary(item => item.Name!, item => item.Value);
    }

    /// <summary>
    /// Stores the values as a json file.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="values">The values.</param>
    public void SetData(string id, IDictionary<string, object?> values)
    {
        var filePath = GetfilePath(id);
        var list = values.Select(kvp => new StoreItem() { Name = kvp.Key, Value = kvp.Value, Type = kvp.Value?.GetType() });
        var serialized = JsonConvert.SerializeObject(list, new JsonSerializerSettings() { Formatting = Formatting.Indented, TypeNameHandling = TypeNameHandling.None, Converters = new JsonConverter[] { new IPAddressConverter() } });

        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        File.WriteAllText(filePath, serialized);
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

    private class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IPAddress);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;
            if (value != null)
            {
                return IPAddress.Parse((string?)reader.Value!);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => writer.WriteValue(value?.ToString());
    }

    private class StoreItemConverter : JsonConverter
    {
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType == typeof(StoreItem);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            reader.Read();
            reader.Read();
            var t = serializer.Deserialize<Type>(reader);
            _ = reader.Read();
            var name = reader.ReadAsString();

            reader.Read();
            reader.Read();
            var res = serializer.Deserialize(reader, t);

            reader.Read();

            return new StoreItem() { Name = name, Type = t, Value = res };
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var converters = serializer.Converters.ToArray();
            var jObject = JObject.FromObject(value!);
            jObject.WriteTo(writer, converters);
        }
    }

    private class StoreItem
    {
        [JsonProperty(Order = 1)]
        public Type? Type { get; set; }

        [JsonProperty(Order = 2)]
        public string? Name { get; set; }

        [JsonProperty(Order = 3)]
        public object? Value { get; set; }
    }
}
