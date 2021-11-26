#if SOURCEGENERATOR
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
using ZarlosExtensions.Net.Attributes;

namespace ZarlosExtensions.Net.CompilationEx
{
    [AutotNullException]
    public static class GetAttribute
    {

        public static IEnumerable<MethodDeclarationSyntax> GetMethodsWithAttribute<T>(this Compilation compilation)
        {
            return compilation
                .SyntaxTrees
                .SelectMany(s => s.GetRoot().DescendantNodes())
                .Where(d => d.IsKind(SyntaxKind.MethodDeclaration))
                .OfType<MethodDeclarationSyntax>()
                .Where(attr => attr.GetType() == typeof(T))
                .ToImmutableArray();

        }
        public static IEnumerable<T> GetAttributes<T>(this Compilation compilation)
        {
            return compilation.GetMethodsWithAttribute<T>()
            .Cast<T>()
            .Where(item => item != null)
            .ToImmutableArray();

        }

    }
}
#endif