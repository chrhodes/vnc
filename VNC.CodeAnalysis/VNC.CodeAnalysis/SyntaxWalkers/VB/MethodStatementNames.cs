﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace VNC.CodeAnalysis.SyntaxWalkers.VB
{
    public class MethodStatementNames : VisualBasicSyntaxWalker
    {
        public List<string> MethodNames;

        public override void VisitMethodStatement(MethodStatementSyntax node)
        {
            //MethodNames.Add(node.ToString());
            MethodNames.Add(node.Identifier.ToString());

            base.VisitMethodStatement(node);
        }
    }
}
