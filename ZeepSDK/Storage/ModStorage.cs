using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Newtonsoft.Json;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Camera;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Geometry;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Hashing;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Math;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.NativeArray;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Random;
using ZeepSDK.External.Newtonsoft.Json.UnityConverters.Scripting;

namespace ZeepSDK.Storage;

internal class ModStorage : IModStorage
{
    private readonly BaseUnityPlugin plugin;
    private readonly JsonSerializerSettings settings;
    private readonly string directoryPath;

    public ModStorage(BaseUnityPlugin plugin)
    {
        this.plugin = plugin;
        settings = new JsonSerializerSettings
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

        directoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Zeepkist",
            "Mods",
            plugin.Info.Metadata.GUID);
    }

    private string CreatePath(string name, string extension)
    {
        string filePath = Path.Combine(directoryPath, name) + extension;

        string directoryName = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        return filePath;
    }

    public void AddConverter(JsonConverter converter)
    {
        settings.Converters.Add(converter);
    }

    public void RemoveConverter(JsonConverter converter)
    {
        settings.Converters.Remove(converter);
    }

    public void SaveToJson(string name, object data)
    {
        string json = JsonConvert.SerializeObject(data, settings);
        File.WriteAllText(CreatePath(name, ".json"), json);
    }

    public object LoadFromJson(string name)
    {
        string json = File.ReadAllText(CreatePath(name, ".json"));
        return JsonConvert.DeserializeObject(json, settings);
    }

    public object LoadFromJson(string name, Type type)
    {
        string json = File.ReadAllText(CreatePath(name, ".json"));
        return JsonConvert.DeserializeObject(json, type, settings);
    }

    public TData LoadFromJson<TData>(string name)
    {
        string json = File.ReadAllText(CreatePath(name, ".json"));
        return JsonConvert.DeserializeObject<TData>(json, settings);
    }

    public void DeleteFile(string name)
    {
        string path = CreatePath(name, ".json");
        if (File.Exists(path))
            File.Delete(path);
    }

    public void DeleteJsonFile(string name)
    {
        string path = CreatePath(name, ".json");
        if (File.Exists(path))
            File.Delete(path);
    }

    public void WriteBlob(string name, byte[] data)
    {
        File.WriteAllBytes(CreatePath(name, ".blob"), data);
    }

    public byte[] ReadBlob(string name)
    {
        return File.ReadAllBytes(CreatePath(name, ".blob"));
    }

    public void DeleteBlob(string name)
    {
        string path = CreatePath(name, ".blob");
        if (File.Exists(path))
            File.Delete(path);
    }
}
