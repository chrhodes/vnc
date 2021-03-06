﻿using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace VNC.CodeAnalysis.SyntaxWalkers.VB
{
    public class ClassBlock : VNCVBSyntaxWalkerBase
    {
        public override void VisitClassBlock(ClassBlockSyntax node)
        {
            if (_targetPatternRegEx.Match(node.ClassStatement.Identifier.ToString()).Success)
            {
                RecordMatchAndContext(node, BlockType.ClassBlock);
            }

            base.VisitClassBlock(node);
        }
    }
}
