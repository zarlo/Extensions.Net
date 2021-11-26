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

using Newtonsoft.Json;
using System.Security.Cryptography;


using Scriban;

namespace ZarlosExtensions.Net.SourceGenerator
{
    public static partial class GeneratorEx
    {

        public static void FromTemplateFile(this GeneratorExecutionContext context, object model, string templatePath)
        {
            context.FromTemplateString(model, System.IO.File.ReadAllText(templatePath));
        }

        public static void FromTemplateString(this GeneratorExecutionContext context, object model, string templateString)
        {

            var template = Template.Parse(templateString);

            if(template.HasErrors)
            {
                var errors = string.Join(" | ", template.Messages.Select(x => x.Message));
                throw new InvalidOperationException($"Template parse error: {template.Messages}");
            }

            var result = template.Render(model);

            byte[] hash = null;
            byte[] responsebytes = UTF8Encoding.UTF8.GetBytes(result);

            using SHA1 sha1 = SHA1Managed.Create();
            hash = sha1.ComputeHash(responsebytes);


            StringBuilder formatted = new StringBuilder(2 * hash.Length);
            foreach (byte b in hash)
            {
                formatted.AppendFormat("{0:X2}", b);
            }

            context.AddSource($@"{formatted}.cs", SourceText.From(result, Encoding.UTF8));
            

        }
    }
}
