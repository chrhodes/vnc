﻿using System;

using Microsoft.CodeAnalysis;

namespace VNC.CodeAnalysis.SyntaxWalkers.CS
{
    public class VNCCSTypedSyntaxWalkerBase : VNCCSSyntaxWalkerBase
    {
        //public Boolean AllTypes = false;
        public Boolean HasAttributes = false;

        public VNCCSTypedSyntaxWalkerBase(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node) : base(depth)
        {

        }

        // TODO(crhodes)
        // Figure out wht CSharp equivalent to AsClause

        //internal bool FilterByType(AsClauseSyntax asClause)
        //{
        //    Boolean addField = false;

        //    if (asClause == null || _configurationOptions.AllTypes)
        //    {
        //        addField = true;
        //    }
        //    else
        //    {
        //        switch (asClause.Type().ToString())
        //        {
        //            case "Boolean":
        //                if (_configurationOptions.Boolean) addField = true;
        //                break;

        //            case "Byte":
        //                if (_configurationOptions.Byte) addField = true;
        //                break;

        //            case "DataTable":
        //                if (_configurationOptions.DataTable) addField = true;
        //                break;

        //            case "Date":
        //                if (_configurationOptions.Date) addField = true;
        //                break;

        //            case "DateTime":
        //                if (_configurationOptions.DateTime) addField = true;
        //                break;

        //            case "Int16":
        //                if (_configurationOptions.Int16) addField = true;
        //                break;

        //            case "Int32":
        //                if (_configurationOptions.Int32) addField = true;
        //                break;

        //            case "Integer":
        //                if (_configurationOptions.Integer) addField = true;
        //                break;

        //            case "Long":
        //                if (_configurationOptions.Long) addField = true;
        //                break;

        //            case "Single":
        //                if (_configurationOptions.Single) addField = true;
        //                break;

        //            case "String":
        //                if (_configurationOptions.String) addField = true;
        //                break;

        //            default:
        //                if (_configurationOptions.OtherTypes) addField = true;
        //                //if (IsOtherType && !displayStructure) addField = true;

        //                break;
        //        }
        //    }

        //    return addField;
        //}
    }
}
