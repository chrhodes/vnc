﻿using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VNC.CodeAnalysis.DesignMetrics.CS
{
    public class DataClasses
    {
        public static StringBuilder Check(string sourceCode)
        {
            StringBuilder sb = new StringBuilder();

            var tree = CSharpSyntaxTree.ParseText(sourceCode);

            var results = tree
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()//#1
            .Select(cds => new //#2
            {
                //Name of the class
                Name = cds.Identifier.ValueText,
                //Number of members of the class
                MemberCount = cds.Members.Count,
                //Number of public properties
                PublicPropertyCount = cds.Members
            .OfType<PropertyDeclarationSyntax>()
            .Count(pds => pds.Modifiers
            .Any(m => m.ValueText == "public"))
            })
            .Where(cds => cds.MemberCount == cds.PublicPropertyCount); //#3
            //            .Dump("Data Classes");

            foreach (var item in results)
            {
                sb.AppendLine($"  {item.Name} {item.MemberCount}");
            }

            return sb;
        }
    }
}
