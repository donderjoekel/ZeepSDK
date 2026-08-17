using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZeepSDK.Scripting.ZUA;

internal sealed class LuaApiDescriptor<TApi>
    where TApi : class
{
    public LuaApiDescriptor(Type type, Func<TApi> create)
    {
        Type = type;
        Create = create;
    }

    public Type Type { get; }
    public Func<TApi> Create { get; }
}

internal static class LuaApiDescriptorCache
{
    private static readonly Lazy<IReadOnlyList<LuaApiDescriptor<ILuaFunction>>> functions =
        new(() => Discover<ILuaFunction>());
    private static readonly Lazy<IReadOnlyList<LuaApiDescriptor<ILuaEvent>>> events =
        new(() => Discover<ILuaEvent>());

    public static IReadOnlyList<LuaApiDescriptor<ILuaFunction>> Functions => functions.Value;
    public static IReadOnlyList<LuaApiDescriptor<ILuaEvent>> Events => events.Value;

    private static IReadOnlyList<LuaApiDescriptor<TApi>> Discover<TApi>()
        where TApi : class
    {
        List<LuaApiDescriptor<TApi>> descriptors = new();
        foreach (Type type in GetLoadableTypes(typeof(LuaApiDescriptorCache).Assembly)
                     .Where(type => type.IsClass && !type.IsAbstract && typeof(TApi).IsAssignableFrom(type))
                     .OrderBy(type => type.FullName, StringComparer.Ordinal))
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                continue;

            NewExpression create = Expression.New(constructor);
            Func<TApi> factory = Expression.Lambda<Func<TApi>>(
                Expression.Convert(create, typeof(TApi))).Compile();
            descriptors.Add(new LuaApiDescriptor<TApi>(type, factory));
        }

        return new ReadOnlyCollection<LuaApiDescriptor<TApi>>(descriptors);
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type != null);
        }
    }
}
