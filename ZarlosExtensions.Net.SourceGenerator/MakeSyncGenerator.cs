// based on https://github.com/mhmd-azeez/FunWithSourceGenerators/blob/master/FunWithSourceGenerators/AsyncifyGenerator.cs
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ZarlosExtensions.Net.SourceGenerator
{
    [Generator]
    public class MakeSyncGenerator: ISourceGenerator
    {

        public void Initialize(GeneratorInitializationContext context)
        {

        }
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;
            
            Compilation compilation = context.Compilation;

            // get the newly bound attribute, and INotifyPropertyChanged
            INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName("ZarlosExtensions.Net.Attributes.MakeSyncAttribute");

            // loop over the candidate fields, and keep the ones that are actually annotated
            List<IMethodSymbol> methodSymbols = new List<IMethodSymbol>();
            foreach (var method in receiver.CandidateMethods)
            {
                SemanticModel model = compilation.GetSemanticModel(method.SyntaxTree);
                var methodSymbol = model.GetDeclaredSymbol(method);
                if (methodSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                {
                    methodSymbols.Add(methodSymbol);
                }
            }

            foreach (var group in methodSymbols.GroupBy(f => f.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList());
                context.AddSource($"{group.Key.Name}_syncify.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<IMethodSymbol> methods)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // begin building the generated source
            StringBuilder source = new StringBuilder($@"
                namespace {namespaceName}
                {{
                    public partial class {classSymbol.Name}
                    {{
                ");

            // create properties for each field 
            foreach (var methodSymbol in methods)
            {
                ProcessMethod(source, methodSymbol);
            }

            source.Append("} }");
            return source.ToString();
        }
        private void ProcessMethod(StringBuilder source, IMethodSymbol methodSymbol)
        {
            string SyncMethodName = methodSymbol.Name.Replace("Async", "");
            if(SyncMethodName == methodSymbol.Name) SyncMethodName = $@"{methodSymbol.Name}Sync";

            var staticModifier = methodSymbol.IsStatic ? "static" : string.Empty;

            var SyncReturnType = methodSymbol.ReturnType.Name == "Task" ? 
                                "Void" : 
                                    methodSymbol.ReturnType.Name
                                    .Skip(5)
                                    .Take(methodSymbol.ReturnType.Name.Length - 6).ToString();
                                

            // int number, string name
            var parameters = string.Join(",", methodSymbol.Parameters.Select(p => $"{p.Type} {p.Name}"));
            // number, name
            var arguments = string.Join(",", methodSymbol.Parameters.Select(p => p.Name));

            source.Append($@"
            public {staticModifier} {SyncReturnType} {SyncMethodName}({parameters})
            {{");
            if(SyncReturnType == "Void")
            {
                source.Append($@"
                    {methodSymbol.Name}({arguments}).Result;");

            }
            else{
                source.Append($@"
                    return {methodSymbol.Name}({arguments}).Result;");
            }
            source.Append($@"
            }}
            ");
        }

    }
    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<MethodDeclarationSyntax> CandidateMethods { get; } = new List<MethodDeclarationSyntax>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // any method with at least one attribute is a candidate for property generation
            if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax
                && methodDeclarationSyntax.AttributeLists.Count > 0)
            {
                CandidateMethods.Add(methodDeclarationSyntax);
            }
        }
    }
}
