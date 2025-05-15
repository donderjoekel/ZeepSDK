using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using BugsnagUnity;
using Newtonsoft.Json;
using ZeepSDK.Crashlytics;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Camera;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Geometry;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Hashing;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Math;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.NativeArray;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Random;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Scripting;
using ZeepSDK.Utilities;

namespace ZeepSDK.Storage;

internal class ModStorage : IModStorage
{
    private static ManualLogSource _logger = LoggerFactory.GetLogger<ModStorage>();
    private readonly List<char> _invalidCharacters;
    private readonly BaseUnityPlugin _plugin;
    private readonly JsonSerializerSettings _settings;
    private readonly string _directoryPath;

    public ModStorage(BaseUnityPlugin plugin)
    {
        _plugin = plugin;
        _settings = new JsonSerializerSettings
        {
            ContractResolver = new UnityTypeContractResolver(),
            Converters = new List<JsonConverter>()
            {
                new BoundsConverter(),
                new BoundsIntConverter(),
                new Color32Converter(),
                new ColorConverter(),
                new CullingGroupEventConverter(),
                new Hash128Converter(),
                new LayerMaskConverter(),
                new Matrix4x4Converter(),
                new NativeArrayConverter(),
                new PlaneConverter(),
                new QuaternionConverter(),
                new RandomStateConverter(),
                new RangeIntConverter(),
                new RectConverter(),
                new RectIntConverter(),
                new RectOffsetConverter(),
                new SphericalHarmonicsL2Converter(),
                new Vector2Converter(),
                new Vector2IntConverter(),
                new Vector3Converter(),
                new Vector3IntConverter(),
                new Vector4Converter(),
            }
        };
        
        _invalidCharacters = [];
        _invalidCharacters.AddRange(Path.GetInvalidFileNameChars());
        _invalidCharacters.AddRange(Path.GetInvalidPathChars());

        _directoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Zeepkist",
            "Mods",
            plugin.Info.Metadata.GUID);
    }

    private string CreatePath(string name, string extension)
    {
        string filePath = Path.Combine(_directoryPath, SanitizePath(name)) + extension;

        string directoryName = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        return filePath;
    }

    private string SanitizePath(string input)
    {
        StringBuilder builder = new();

        foreach (char c in input)
        {
            if (_invalidCharacters.Contains(c))
                builder.Append("_");
            else
                builder.Append(c);
        }
        
        return builder.ToString();
    }

    public void AddConverter(JsonConverter converter)
    {
        _settings.Converters.Add(converter);
    }

    public void RemoveConverter(JsonConverter converter)
    {
        _settings.Converters.Remove(converter);
    }

    public bool JsonFileExists(string name)
    {
        try
        {
            return File.Exists(CreatePath(name, ".json"));
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to check if json file exists");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
            return false;
        }
    }

    public void SaveToJson(string name, object data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, _settings);
            File.WriteAllText(CreatePath(name, ".json"), json);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to save to json");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
        }
    }

    public object LoadFromJson(string name)
    {
        try
        {
            string json = File.ReadAllText(CreatePath(name, ".json"));
            return JsonConvert.DeserializeObject(json, _settings);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to load to json");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
            return null;
        }
    }

    public object LoadFromJson(string name, Type type)
    {
        try
        {
            string json = File.ReadAllText(CreatePath(name, ".json"));
            return JsonConvert.DeserializeObject(json, type, _settings);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to load to json");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
            return null;
        }
    }

    public TData LoadFromJson<TData>(string name)
    {
        try
        {
            string json = File.ReadAllText(CreatePath(name, ".json"));
            return JsonConvert.DeserializeObject<TData>(json, _settings);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to load to json");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
            return default;
        }
    }

    public void DeleteFile(string name)
    {
        try
        {
            string path = CreatePath(name, ".json");
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to delete file");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
        }
    }

    public void DeleteJsonFile(string name)
    {
        try
        {
            string path = CreatePath(name, ".json");
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to delete file");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
        }
    }

    public bool BlobFileExists(string name)
    {
        try
        {
            return File.Exists(CreatePath(name, ".blob"));
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to check if blob file exists");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
            return false;
        }
    }

    public void WriteBlob(string name, byte[] data)
    {
        try
        {
            File.WriteAllBytes(CreatePath(name, ".blob"), data);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to write blob");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
        }
    }

    public byte[] ReadBlob(string name)
    {
        try
        {
            return File.ReadAllBytes(CreatePath(name, ".blob"));
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to read blob");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
            return null;
        }
    }

    public void DeleteBlob(string name)
    {
        try
        {
            string path = CreatePath(name, ".blob");
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to delete blob");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
        }
    }

    public void SaveToFile(string filename, byte[] data)
    {
        try
        {
            string path = CreatePath(filename, string.Empty);
            File.WriteAllBytes(path, data);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to save file");
            _logger.LogError(e);
            CrashlyticsApi.Notify(e);
        }
    }
}
