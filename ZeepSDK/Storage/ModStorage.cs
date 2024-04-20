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
    private readonly JsonSerializerSettings _settings;
    private readonly string _directoryPath;

    public ModStorage(BaseUnityPlugin plugin)
    {
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

        _directoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Zeepkist",
            "Mods",
            plugin.Info.Metadata.GUID);
    }

    private string CreatePath(string name, string extension = ".json")
    {
        string filePath = Path.Combine(_directoryPath, name) + extension;

        string directoryName = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            _ = Directory.CreateDirectory(directoryName);
        }

        return filePath;
    }

    public void AddConverter(JsonConverter converter)
    {
        _settings.Converters.Add(converter);
    }

    public void RemoveConverter(JsonConverter converter)
    {
        _ = _settings.Converters.Remove(converter);
    }

    public void SaveToJson(string name, object data)
    {
        string json = JsonConvert.SerializeObject(data, _settings);
        File.WriteAllText(CreatePath(name), json);
    }

    public object LoadFromJson(string name)
    {
        string json = File.ReadAllText(CreatePath(name));
        return JsonConvert.DeserializeObject(json, _settings);
    }

    public object LoadFromJson(string name, Type type)
    {
        string json = File.ReadAllText(CreatePath(name));
        return JsonConvert.DeserializeObject(json, type, _settings);
    }

    public TData LoadFromJson<TData>(string name)
    {
        string json = File.ReadAllText(CreatePath(name));
        return JsonConvert.DeserializeObject<TData>(json, _settings);
    }

    public void DeleteFile(string name)
    {
        string path = CreatePath(name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
