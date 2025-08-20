using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using ZeepSDK.ChatCommands;
using ZeepSDK.Scripting.ZUA;
using ZeepSDK.Scripting.ZUA.Commands;
using ZeepSDK.Utilities;

namespace ZeepSDK.Scripting;

/// <summary>
/// 
/// </summary>
[PublicAPI]
public static class ScriptingApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(Zua));
    private static readonly Dictionary<string, Zua> loadedScripts = new();

    internal static void Initialize()
    {
        ChatCommandApi.RegisterLocalChatCommand<ZuaLoadCommand>();
        ChatCommandApi.RegisterLocalChatCommand<ZuaUnloadCommand>();
        ChatCommandApi.RegisterLocalChatCommand<ZuaReloadCommand>();
        
        Script.WarmUp();
        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<ulong>(
            (_, value) => DynValue.NewString(value.ToString()));
        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.String, typeof(ulong),
            value =>
            {
                if (ulong.TryParse(value.String, out ulong result))
                    return result;
                throw new InvalidCastException($"Cannot convert string to ulong; {value.String}");
            });
    }

    /// <summary>
    /// Calls a Lua function by name with the specified arguments on all loaded scripts
    /// </summary>
    /// <param name="name">The name of the function to call</param>
    /// <param name="args">The arguments to pass to the lua function</param>
    public static void CallFunction(string name, params object[] args)
    {
        foreach (KeyValuePair<string, Zua> kvp in loadedScripts)
        {
            kvp.Value.CallFunction(name, args);
        }
    }

    /// <summary>
    /// Registers a function into the cache for new scripts and injects it into all loaded scripts
    /// </summary>
    public static void RegisterFunction<TFunction>()
        where TFunction : ILuaFunction, new()
    {
        ZuaFunctionCache.CacheFunction<TFunction>();
        
        foreach ((string _, Zua zua) in loadedScripts)
        {
            zua.RegisterFunction<TFunction>();
        }
    }

    /// <summary>
    /// Registers a type into the lua userdata system
    /// </summary>
    public static void RegisterType<TType>()
    {
        if (UserData.IsTypeRegistered(typeof(TType)))
        {
            logger.LogInfo($"Type '{typeof(TType).FullName}' is already registered. Skipping.");
            return;
        }

        try
        {
            UserData.RegisterType<TType>();
            logger.LogInfo($"Successfully registered type '{typeof(TType).FullName}'.");
        }
        catch (Exception e)
        {
            logger.LogError($"Error registering type '{typeof(TType).FullName}': {e.Message}");
            logger.LogError(e);
        }
    }

    /// <summary>
    /// Registers an event into the cache for new scripts and injects it into all loaded scripts
    /// </summary>
    public static void RegisterEvent<TEvent>()
        where TEvent : ILuaEvent, new()
    {
        ZuaEventCache.CacheEvent<TEvent>();
        
        foreach ((string _, Zua zua) in loadedScripts)
        {
            zua.RegisterEvent<TEvent>();
        }
    }

    /// <summary>
    /// Loads a lua script by absolute path
    /// </summary>
    /// <param name="filePath">The path to the lua script</param>
    /// <returns>Zua instance if successful, null if either already loaded, or an error occured</returns>
    public static Zua LoadLuaByPath(string filePath)
    {
        if (loadedScripts.ContainsKey(filePath))
        {
            logger.LogError($"This script has already been loaded '{filePath}'");
            return null;
        }

        try
        {
            Zua zua = Zua.LoadLuaByPath(filePath);
            loadedScripts.Add(filePath, zua);
            return zua;
        }
        catch (SyntaxErrorException ex)
        {
            logger.LogError($"An error occured trying to load a lua script. {ex.DecoratedMessage}");
            logger.LogError(ex);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured trying to load a lua script.");
            logger.LogError(ex);
            return null;
        }
    }

    /// <summary>
    /// Loads a lua script by name, searching for a matching file in the plugins folder
    /// </summary>
    /// <param name="name">The name of the file to search for, excluding the extension</param>
    /// <returns>Zua instance if successful, null if either already loaded, or an error occured</returns>
    public static Zua LoadLuaByName(string name)
    {
        string[] files = Directory.GetFiles(Paths.PluginPath, $"{name}.lua", SearchOption.AllDirectories);
        if (files.Length == 1)
            return LoadLuaByPath(files[0]);
        if (files.Length == 0)
        {
            // TODO: Log
            return null;
        }

        if (files.Length > 1)
        {
            // TODO: Log
            return null;
        }

        // This will be caught by if-statements above
        return null;
    }

    /// <summary>
    /// Unloads a lua script by absolute path
    /// </summary>
    /// <param name="path">The absolute path to the lua script</param>
    public static bool UnloadLuaByPath(string path)
    {
        if (loadedScripts.TryGetValue(path, out Zua zua))
        {
            zua.Unload();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Unloads a lua script by name, searching for a matching file in the plugins folder
    /// </summary>
    /// <param name="name">The name of the file to search for, excluding the extension</param>
    public static bool UnloadLuaByName(string name)
    {
        string[] files = Directory.GetFiles(Paths.PluginPath, $"{name}.lua", SearchOption.AllDirectories);
        return files.Length == 1 && UnloadLuaByPath(files[0]);
    }

    internal static void UnloadZua(Zua zua)
    {
        (string path, Zua existingZua) = loadedScripts.FirstOrDefault(x => x.Value == zua);
        if (existingZua != null && existingZua == zua)
        {
            loadedScripts.Remove(path);
        }
    }
}
