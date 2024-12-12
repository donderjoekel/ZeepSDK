﻿using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ZeepSDK.ZuaGenerator;

[Generator]
public class FunctionGenerator : IIncrementalGenerator
{
    private const string AttributeSource = @"// <auto-generated/>

namespace ZeepSDK.Scripting.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class GenerateFunctionAttribute : System.Attribute
    {
    }
}";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            ctx => ctx.AddSource(
                "GenerateFunctionAttribute.g.cs",
                SourceText.From(AttributeSource, Encoding.UTF8)));

        IncrementalValuesProvider<MethodDeclarationSyntax> provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (node, ct) => node is MethodDeclarationSyntax,
                (ctx, ct) => GetMethodDeclarationForSourceGen(ctx))
            .Where(x => x.reportAttributeFound)
            .Select((x, _) => x.syntax);

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateFunctionCode(ctx, t.Left, t.Right));
    }

    private static (MethodDeclarationSyntax syntax, bool reportAttributeFound) GetMethodDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        MethodDeclarationSyntax nodeSyntax = (MethodDeclarationSyntax)context.Node;

        foreach (AttributeListSyntax attributeListSyntax in nodeSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                string attributeName = attributeSymbol.ContainingType.ToDisplayString();

                if (attributeName == "ZeepSDK.Scripting.Attributes.GenerateFunctionAttribute")
                    return (nodeSyntax, true);
            }
        }

        return (nodeSyntax, false);
    }

    private void GenerateFunctionCode(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<MethodDeclarationSyntax> methods)
    {
        foreach (MethodDeclarationSyntax syntax in methods)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(syntax) is not IMethodSymbol symbol)
                continue;

            string @namespace = symbol.ContainingType.Name;
            string functionName = symbol.Name;
            string className = symbol.ContainingType.Name + "_" + functionName + "_Function";
            string api = symbol.ContainingType.ToDisplayString();
            string returnType = symbol.ReturnType.ToDisplayString();
            string typedParameters = string.Join(", ", symbol.Parameters.Select(x => $"{x.Type.ToDisplayString()} {x.Name}"));
            string parameters = string.Join(", ", symbol.Parameters.Select(x => x.Name));
            string returnString = symbol.ReturnsVoid ? "" : "return ";
            string typeRegistrations = string.Join(
                "\r\n        ",
                Utils.FilterType(symbol.ReturnType)
                    .Concat(Utils.FilterTypes(symbol.Parameters.Select(x => x.Type)))
                    .Distinct(SymbolEqualityComparer.Default)
                    .Select(x => $"ScriptingApi.RegisterType<{x.ToDisplayString()}>();"));
            
            string generatedCode = $@"// <auto-generated/>
using JetBrains.Annotations;

namespace ZeepSDK.Scripting.Functions;

[UsedImplicitly]
internal class {className} : ZeepSDK.Scripting.ZUA.ILuaFunction
{{
    public string Namespace => ""{@namespace}"";
    public string Name => ""{functionName}"";

    public {className}()
    {{
        {typeRegistrations}
    }}

    public System.Delegate CreateFunction()
    {{
        return Implementation;
    }}

    private static {returnType} Implementation({typedParameters})
    {{
        {returnString}{api}.{functionName}({parameters});
    }}
}}";
            
            context.AddSource($"{@namespace}.{functionName}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
        }
    }
}