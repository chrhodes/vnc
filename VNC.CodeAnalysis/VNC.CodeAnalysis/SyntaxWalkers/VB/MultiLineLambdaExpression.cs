﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace VNC.CodeAnalysis.SyntaxWalkers.VB
{
    public class MultiLineLambdaExpression : VNCVBSyntaxWalkerBase
    {
        public override void VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
        {
            if (_targetPatternRegEx.Match(node.ToString()).Success)
            {
                RecordMatchAndContext(node, BlockType.None);
            }

            // Call base to visit children
            base.VisitMultiLineLambdaExpression(node);
        }
    }
}
