using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ZeepSDK.ZuaGenerator;

internal static class Utils
{
    public static IEnumerable<ITypeSymbol> FilterTypes(IEnumerable<ITypeSymbol> symbols)
    {
        foreach (ITypeSymbol symbol in symbols)
        {
            foreach (ITypeSymbol typeSymbol in FilterType(symbol))
            {
                yield return typeSymbol;
            }
        }
    }

    public static IEnumerable<ITypeSymbol> FilterType(ITypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.IsGenericType)
            {
                foreach (ITypeSymbol typeArgumentSymbol in namedTypeSymbol.TypeArguments)
                {
                    yield return typeArgumentSymbol;
                }
            }
            else
            {
                if (symbol.ContainingSymbol.Name == "System")
                    yield break;
                yield return symbol;
            }
        }
        else
        {
            if (symbol.ContainingSymbol.Name == "System")
                yield break;
            yield return symbol;
        }
    }
}
