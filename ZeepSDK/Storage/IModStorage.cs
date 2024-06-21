using System;
using Newtonsoft.Json;

namespace ZeepSDK.Storage;

/// <summary>
/// A storage object that allows you to save and load data to a directory for your mod
/// </summary>
public interface IModStorage
{
    /// <summary>
    /// Allows you to add a custom json converter
    /// </summary>
    /// <param name="converter">The converter to add</param>
    void AddConverter(JsonConverter converter);

    /// <summary>
    /// Allows you to remove a custom json converter
    /// </summary>
    /// <param name="converter">The converter to remove</param>
    void RemoveConverter(JsonConverter converter);

    /// <summary>
    /// Saves data to a json file
    /// </summary>
    /// <param name="name">The name of the file without extension</param>
    /// <param name="data">The data to save</param>
    void SaveToJson(string name, object data);

    /// <summary>
    /// Loads data from a json file
    /// </summary>
    /// <param name="name">The name of the file without extension</param>
    /// <returns></returns>
    object LoadFromJson(string name);

    /// <summary>
    /// Loads data from a json file with a specific type
    /// </summary>
    /// <param name="name">The name of the file without extension</param>
    /// <param name="type">The type the data should be</param>
    /// <returns></returns>
    object LoadFromJson(string name, Type type);

    /// <summary>
    /// Loads data from a json file with a specific type
    /// </summary>
    /// <param name="name">The name of the file without extension</param>
    /// <typeparam name="TData">The type the data should be</typeparam>
    /// <returns></returns>
    TData LoadFromJson<TData>(string name);

    /// <summary>
    /// Deletes a file
    /// </summary>
    /// <param name="name">The name of the file to delete without extension</param>
    [Obsolete("use DeleteJsonFile or DeleteBlob instead.")]
    void DeleteFile(string name);

    /// <summary>
    /// Deletes a json file
    /// </summary>
    /// <param name="name">The name of the file to delete without extension</param>
    void DeleteJsonFile(string name);

    /// <summary>
    /// Saves data to a binary file
    /// </summary>
    /// <param name="name">The name of the file without extension</param>
    /// <param name="data">The data to save</param>
    void WriteBlob(string name, byte[] data);

    /// <summary>
    /// Loads data from a binary file
    /// </summary>
    /// <param name="name">The name of the file without extension</param>
    /// <returns></returns>
    byte[] ReadBlob(string name);

    /// <summary>
    /// Deletes a binary file
    /// </summary>
    /// <param name="name">The name of the file without extension</param>
    void DeleteBlob(string name);
}
