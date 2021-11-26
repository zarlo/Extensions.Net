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
    public class AutotNullException: ISourceGenerator
    {

        public void Initialize(GeneratorInitializationContext context)
        {

        }
        public void Execute(GeneratorExecutionContext context)
        {
            
        }
    }
}
