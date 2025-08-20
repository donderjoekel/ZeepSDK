using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using MoonSharp.Interpreter;
using ZeepSDK.Utilities;

namespace ZeepSDK.Scripting.ZUA;

/// <summary>
/// A wrapper around a Lua script
/// </summary>
public class Zua
{
    internal static ManualLogSource Logger { get; } = LoggerFactory.GetLogger(typeof(Zua));

    private Script script;
    private bool loaded;
    private readonly List<ILuaEvent> registeredEvents = [];
    private readonly List<ILuaEvent> subscribedEvents = [];

    private Zua()
    {
    }

    private void Initialize(string luaContent)
    {
        script = new Script(CoreModules.Preset_SoftSandbox);
        RegisterFunction(new ListenToLuaFunction(this));
        RegisterAllFunctionsInCurrentAssembly();
        RegisterAllEventsInCurrentAssembly();
        ZuaFunctionCache.RegisterCachedFunctions(this);
        ZuaEventCache.RegisterCachedEvents(this);
        script.DoString(luaContent);
        loaded = true;
        CallFunction("OnLoad");
    }

    /// <summary>
    /// Unloads the current Lua script, unsubscribes from events, and resets the loaded state.
    /// </summary>
    public void Unload()
    {
        CallFunction("OnUnload");
        Unsubscribe();
        loaded = false;
        ScriptingApi.UnloadZua(this);
    }

    /// <summary>
    /// Listens to an event by name, subscribing to it if found.
    /// </summary>
    /// <param name="eventName">The name of the event to listen to.</param>
    public void ListenTo(string eventName)
    {
        ILuaEvent luaEvent = registeredEvents.FirstOrDefault(e => e.Name == eventName);
        if (luaEvent == null)
        {
            Logger.LogWarning($"Event '{eventName}' not found.");
            return;
        }

        if (subscribedEvents.Any(e => e.Name == luaEvent.Name))
        {
            Logger.LogInfo($"Event '{eventName}' is already being listened to.");
            return;
        }

        try
        {
            luaEvent.Subscribe();
            subscribedEvents.Add(luaEvent);
            Logger.LogInfo($"Started listening to event '{eventName}'.");
        }
        catch (Exception e)
        {
            Logger.LogError($"Error subscribing to event '{eventName}': {e.Message}");
            Logger.LogError(e);
        }
    }

    /// <summary>
    /// Calls a Lua function by name with the specified arguments.
    /// </summary>
    /// <param name="name">The name of the Lua function to call.</param>
    /// <param name="args">The arguments to pass to the Lua function.</param>
    public void CallFunction(string name, params object[] args)
    {
        try
        {
            DynValue function = script.Globals.Get(name);
            if (function.Type != DataType.Function)
            {
                Logger.LogWarning($"Lua function '{name}' is not implemented. Skipping.");
                return;
            }

            DynValue[] dynArgs = args.Select(arg => DynValue.FromObject(script, arg)).ToArray();
            script.Call(function, dynArgs);
        }
        catch (ScriptRuntimeException ex)
        {
            Logger.LogError($"Error calling Lua function '{name}. {ex.DecoratedMessage}");
            Logger.LogError(ex);
        }
        catch (Exception e)
        {
            Logger.LogError($"Error calling Lua function '{name}': {e.Message}");
            Logger.LogError(e);
        }
    }

    /// <summary>
    /// Registers a Lua function of the specified type.
    /// </summary>
    /// <typeparam name="TFunction">The type of the Lua function to register.</typeparam>
    public void RegisterFunction<TFunction>()
        where TFunction : ILuaFunction, new()
    {
        RegisterFunction(typeof(TFunction));
    }

    /// <summary>
    /// Registers a Lua event of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the Lua event to register.</typeparam>
    public void RegisterEvent<TEvent>()
        where TEvent : ILuaEvent, new()
    {
        TEvent luaEvent = new TEvent();
        if (registeredEvents.Any(e => e.Name == luaEvent.Name)) return;
        registeredEvents.Add(luaEvent);
    }

    private void RegisterAllFunctionsInCurrentAssembly()
    {
        List<Type> types = typeof(Zua).Assembly.GetTypes()
            .Where(x => !x.IsAbstract && x.IsClass)
            .Where(x => typeof(ILuaFunction).IsAssignableFrom(x))
            .Where(x => x.GetConstructors().Any(x => x.GetParameters().Length == 0))
            .ToList();

        foreach (Type type in types)
        {
            RegisterFunction(type);
        }
    }

    internal void RegisterFunction(Type functionType)
    {
        if (functionType == null)
            throw new ArgumentNullException(nameof(functionType));

        if (!typeof(ILuaFunction).IsAssignableFrom(functionType))
            throw new ArgumentException($"{functionType.FullName} is not a valid function type");

        ILuaFunction function = Activator.CreateInstance(functionType) as ILuaFunction;
        RegisterFunction(function);
    }

    private void RegisterFunction(ILuaFunction function)
    {
        Table namespaceTable = script.Globals.Get(function.Namespace).Table;
        if (namespaceTable == null)
        {
            namespaceTable = new Table(script);
            script.Globals[function.Namespace] = namespaceTable;
        }

        DynValue existingFunction = namespaceTable.Get(function.Name);
        if (Equals(existingFunction, DynValue.Nil))
        {
            namespaceTable[function.Name] = function.CreateFunction();
            Logger.LogInfo($"Registered: {function.Namespace}.{function.Name}");
        }
        else
        {
            Logger.LogWarning($"Skipped: {function.Namespace}.{function.Name} (already exists)");
        }
    }

    internal void RegisterEvent(Type eventType)
    {
        if (eventType == null)
            throw new ArgumentNullException(nameof(eventType));
        
        if (!typeof(ILuaEvent).IsAssignableFrom(eventType))
            throw new ArgumentException($"{eventType.FullName} is not a valid event type");
        
        ILuaEvent luaEvent = Activator.CreateInstance(eventType) as ILuaEvent;
        if (luaEvent == null)
        {
            // TODO: Log
            return;
        }

        if (registeredEvents.Any(x => x.Name == luaEvent.Name))
        {
            Logger.LogWarning($"Skipped event '{eventType.FullName}' (already exists)");
        }
        else
        {
            registeredEvents.Add(luaEvent);
            Logger.LogInfo($"Registered event '{eventType.FullName}'.");
        }
    }

    private void RegisterAllEventsInCurrentAssembly()
    {
        IEnumerable<Type> eventTypes = typeof(Zua).Assembly.GetTypes()
            .Where(t => typeof(ILuaEvent).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (Type type in eventTypes)
        {
            RegisterEvent(type);
            // ILuaEvent luaEvent = Activator.CreateInstance(type) as ILuaEvent;
            // if (luaEvent != null && registeredEvents.All(e => e.Name != luaEvent.Name))
            // {
            //     registeredEvents.Add(luaEvent);
            // }
        }
    }

    private void Unsubscribe()
    {
        foreach (ILuaEvent luaEvent in subscribedEvents)
        {
            try
            {
                luaEvent.Unsubscribe();
            }
            catch (Exception e)
            {
                Logger.LogError($"Error unsubscribing from event '{luaEvent.Name}': {e.Message}");
                Logger.LogError(e);
            }
        }

        subscribedEvents.Clear();
    }

    internal static Zua LoadLuaByPath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Lua file not found: {filePath}");
        }

        string luaContent = File.ReadAllText(filePath);

        Zua zua = new();
        zua.Initialize(luaContent);
        return zua;
    }
}