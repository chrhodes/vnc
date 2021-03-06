﻿using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis.VisualBasic;

namespace VNC.CodeAnalysis.QualityMetrics.VB
{
    public class LotsOfLocalVariables
    {
        public static StringBuilder Check(string sourceCode)
        {
            StringBuilder sb = new StringBuilder();

            var tree = VisualBasicSyntaxTree.ParseText(sourceCode);
            //        List<MethodDeclarationSyntax> methods =
            //        tree.GetRoot()
            //        .DescendantNodes()
            //        .Where(d => d.Kind() == SyntaxKind.MethodDeclaration)
            //        .Cast<MethodDeclarationSyntax>()
            //        .ToList();//#1
            //        methods
            //        .Select(z =>
            //        new {
            //            MethodName = z.Identifier.ValueText,//#2
            //NBLocal = z.Body.Statements
            //        //#3
            //        .Count(x => x.Kind() == SyntaxKind.LocalDeclarationStatement)
            //        })
            //        .OrderByDescending(x => x.NBLocal)
            //        .ToList()
            //        .ForEach(x =>
            //        Console.WriteLine(x.MethodName + " " + x.NBLocal));

            sb.AppendLine(MethodBase.GetCurrentMethod().DeclaringType
                + "." + MethodBase.GetCurrentMethod().Name
                + " Not Implemented Yet");

            return sb;
        }
    }
}
