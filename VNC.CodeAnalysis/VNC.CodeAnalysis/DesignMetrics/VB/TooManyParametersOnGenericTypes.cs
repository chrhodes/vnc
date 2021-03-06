﻿using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis.VisualBasic;

namespace VNC.CodeAnalysis.DesignMetrics.VB
{
    public class TooManyParametersOnGenericTypes
    {
        public static StringBuilder Check(string sourceCode)
        {
            StringBuilder sb = new StringBuilder();

            var tree = VisualBasicSyntaxTree.ParseText(sourceCode);
            //tree.GetRoot()
            //.DescendantNodes()
            //.OfType<MethodDeclarationSyntax>()
            //.Select(mds => new {
            //    Name = mds.Identifier.ValueText,
            //    Arity = mds.Arity
            //})
            //.Where(mds => mds.Arity > 2)
            //.Dump("Generic Methods with lots of generic attribute");

            sb.AppendLine(MethodBase.GetCurrentMethod().DeclaringType
                + "." + MethodBase.GetCurrentMethod().Name
                + " Not Implemented Yet");

            return sb;
        }
    }
}
