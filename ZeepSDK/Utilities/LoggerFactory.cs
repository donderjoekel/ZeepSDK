using System;
using System.Collections.Generic;
using BepInEx.Logging;

namespace ZeepSDK.Utilities;

/// <summary>
/// 
/// </summary>
public static class LoggerFactory
{
    private static readonly ManualLogSource _unknownLogger = Logger.CreateLogSource("Unknown");
    private static readonly Dictionary<Type, ManualLogSource> _loggers = [];

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
        if (type == null)
        {
            _unknownLogger.LogError("LoggerFactory.GetLogger requires a non-null type parameter.");
            return null;
        }

        if (_loggers.TryGetValue(type, out ManualLogSource logger))
        {
            return logger;
        }

        logger = Logger.CreateLogSource(type.Name);
        _loggers[type] = logger;
        return logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static ManualLogSource GetLogger(object obj)
    {
        return obj == null ? _unknownLogger : GetLogger(obj.GetType());
    }
}
