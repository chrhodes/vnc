﻿using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis.VisualBasic;

namespace VNC.CodeAnalysis.PerformanceMetrics.VB
{
    public class ValueTypesShouldOverrideEqualsAndGetHashCode
    {
        public static StringBuilder Check(string sourceCode)
        {
            StringBuilder sb = new StringBuilder();

            var tree = VisualBasicSyntaxTree.ParseText(sourceCode);
            //            tree.GetRoot()
            //            .DescendantNodes()
            //            .OfType<StructDeclarationSyntax>()//#2
            //            .Select(sds => new //#3
            //{
            //                StructName = sds.Identifier.ValueText,
            //    //Flag if “Equals” is overridden
            //    OverridenEquals =
            //            sds.Members
            //            .OfType<MethodDeclarationSyntax>()
            //            .FirstOrDefault(m => m.Identifier
            //            .ValueText == "Equals"
            //            && m.Modifiers.Any(mo =>
            //            mo.ValueText == "override")) != null,
            //    //Flag if “GetHashCode” is overridden
            //    OverridenGetHashCode =
            //            sds.Members
            //            .OfType<MethodDeclarationSyntax>()
            //            .FirstOrDefault(m => m.Identifier
            //            .ValueText == "GetHashCode"
            //            && m.Modifiers.Any(mo =>
            //            mo.ValueText == "override")) != null
            //            })
            //            .Where(sds => !sds.OverridenEquals
            //            || !sds.OverridenGetHashCode)//#4
            //            .Dump("Defaulter Structs");

            sb.AppendLine(MethodBase.GetCurrentMethod().DeclaringType
                + "." + MethodBase.GetCurrentMethod().Name
                + " Not Implemented Yet");

            return sb;
        }
    }
}
