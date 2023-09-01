using System;
using System.Collections.Generic;
using BepInEx.Logging;

namespace ZeepSDK.Utilities;

/// <summary>
/// 
/// </summary>
public static class LoggerFactory
{
    private static ManualLogSource unknownLogger = Logger.CreateLogSource("Unknown");
    private static readonly Dictionary<Type, ManualLogSource> loggers = new();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ManualLogSource GetLogger<T>()
    {
        return GetLogger(typeof(T));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ManualLogSource GetLogger(Type type)
    {
        if (loggers.TryGetValue(type, out ManualLogSource logger))
            return logger;

        logger = Logger.CreateLogSource(type.Name);
        loggers[type] = logger;
        return logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static ManualLogSource GetLogger(object obj)
    {
        return obj == null ? unknownLogger : GetLogger(obj.GetType());
    }
}
