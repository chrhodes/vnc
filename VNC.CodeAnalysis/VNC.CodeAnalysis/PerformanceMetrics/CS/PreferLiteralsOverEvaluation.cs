﻿using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VNC.CodeAnalysis.PerformanceMetrics.CS
{
    public class PreferLiteralsOverEvaluation
    {
        public static StringBuilder Check(string sourceCode)
        {
            StringBuilder sb = new StringBuilder();

            var tree = CSharpSyntaxTree.ParseText(sourceCode); // 1

            //Finding all the literals in the code.

            var literals = tree.GetRoot()
            .DescendantNodes()
            .OfType<LiteralExpressionSyntax>() // 2
            .Select(les => les.ToFullString())
            .Distinct();

            var results = tree.GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(ies => new // 3
            {
                MethodName = ies.Ancestors()
                .OfType<MethodDeclarationSyntax>()
                ?.First()?.Identifier.ValueText,
                Expression = ies.Expression.ToFullString(),
                CallTokens = ies.Expression.ChildNodes()
            .Select(e => e.ToFullString())
            })
            // 4
            .Where(ies => ies.CallTokens.Any(ct => literals.Contains(ct)))
            // 5
            .Select(ies =>
            new
            {
                MethodName = ies.MethodName,
                Expression = ies.Expression
            });
            //            .Dump("Methods using ToUpper or ToLower on string literals");

            foreach (var item in results)
            {
                sb.AppendLine($"  {item.MethodName} {item.Expression}");
            }

            return sb;
        }
    }
}
