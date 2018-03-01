// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PascalABCCompiler.Parsers;
using PascalABCCompiler.SyntaxTree;
using PascalSharp.Internal.Localization;

namespace VisualPascalABC
{
    public class CodeCompletionImagesProvider
    {
        ImageList images = new ImageList();
        public int IconNumberMethod = -1;
        public int IconNumberField = -1;
        public int IconNumberProperty = -1;
        public int IconNumberEvent = -1;
        public int IconNumberClass = -1;
        public int IconNumberNamespace = -1;
        public int IconNumberInterface = -1;
        public int IconNumberStruct = -1;
        public int IconNumberDelegate = -1;
        public int IconNumberEnum = -1;
        public int IconNumberLocal = -1;
        public int IconNumberConstant = -1;
        public int IconNumberPrivateField = -1;
        public int IconNumberPrivateMethod = -1;
        public int IconNumberPrivateProperty = -1;
        public int IconNumberProtectedField = -1;
        public int IconNumberProtectedMethod = -1;
        public int IconNumberProtectedProperty = -1;
        public int IconNumberPrivateEvent = -1;
        public int IconNumberProtectedEvent = -1;
        public int IconNumberPrivateDelegate = -1;
        public int IconNumberProtectedDelegate = -1;
        public int IconNumberUnitNamespace = -1;
        public int IconNumberInternalField = -1;
        public int IconNumberInternalMethod = -1;
        public int IconNumberInternalProperty = -1;
        public int IconNumberInternalEvent = -1;
        public int IconNumberInternalDelegate = -1;
        public int IconNumberGotoText = -1;
        public int IconNumberPrivateConstant = -1;
        public int IconNumberProtectedConstant = -1;
        public int IconNumberInternalConstant = -1;
        public int IconNumberKeyword = -1;
        public int IconNumberEvalError = -1;
        public int IconNumberExtensionMethod = -1;

        int AddImageFromManifestResource(string ResName)
        {
            var obj = VisualPascalABC.Resources.ResourceManager.GetObject(ResName, VisualPascalABC.Resources.Culture) ?? throw new ArgumentNullException($@"ResourceManager.GetObject(""{ResName}"")");
            switch (obj)
            {
                case Image img: {
                    images.Images.Add(img);
                    break;
                }
                case Icon ico: {
                    images.Images.Add(ico);
                    break;
                }
                default:
                {
                    throw new ArgumentException($@"ResourceManager.GetObject(""{ResName}"")");
                }
            }

            return images.Images.Count - 1;
        }

        public ImageList ImageList
        {
            get
            {
                return images;
            }
        }

        public CodeCompletionImagesProvider()
        {
            images.ColorDepth = ColorDepth.Depth32Bit;
            IconNumberMethod = AddImageFromManifestResource("Icons_16x16_Method");
            IconNumberField = AddImageFromManifestResource("Icons_16x16_Field");
            IconNumberProperty = AddImageFromManifestResource("Icons_16x16_Property");
            IconNumberEvent = AddImageFromManifestResource("Icons_16x16_Event");
            IconNumberClass = AddImageFromManifestResource("Icons_16x16_Class");
            IconNumberNamespace = AddImageFromManifestResource("Icons_16x16_NameSpace");
            IconNumberInterface = AddImageFromManifestResource("Icons_16x16_Interface");
            IconNumberStruct = AddImageFromManifestResource("Icons_16x16_Struct");
            IconNumberDelegate = AddImageFromManifestResource("Icons_16x16_Delegate");
            IconNumberEnum = AddImageFromManifestResource("Icons_16x16_Enum");
            IconNumberLocal = AddImageFromManifestResource("Icons_16x16_Local");
            IconNumberConstant = AddImageFromManifestResource("Icons_16x16_Literal");
            IconNumberPrivateField = AddImageFromManifestResource("Icons_16x16_PrivateField");
            IconNumberPrivateMethod = AddImageFromManifestResource("Icons_16x16_PrivateMethod");
            IconNumberPrivateProperty = AddImageFromManifestResource("Icons_16x16_PrivateProperty");
            IconNumberProtectedField = AddImageFromManifestResource("Icons_16x16_ProtectedField");
            IconNumberProtectedMethod = AddImageFromManifestResource("Icons_16x16_ProtectedMethod");
            IconNumberProtectedProperty = AddImageFromManifestResource("Icons_16x16_ProtectedProperty");
            IconNumberPrivateEvent = AddImageFromManifestResource("Icons_16x16_PrivateEvent");
            IconNumberProtectedEvent = AddImageFromManifestResource("Icons_16x16_ProtectedEvent");
            IconNumberPrivateDelegate = AddImageFromManifestResource("Icons_16x16_PrivateDelegate");
            IconNumberProtectedDelegate = AddImageFromManifestResource("Icons_16x16_ProtectedDelegate");
            IconNumberUnitNamespace = AddImageFromManifestResource("Icons_16x16_UnitNamespace");
            IconNumberInternalField = AddImageFromManifestResource("Icons_16x16_InternalField");
            IconNumberInternalMethod = AddImageFromManifestResource("Icons_16x16_InternalMethod");
            IconNumberInternalProperty = AddImageFromManifestResource("Icons_16x16_InternalProperty");
            IconNumberInternalEvent = AddImageFromManifestResource("Icons_16x16_InternalEvent");
            IconNumberInternalDelegate = AddImageFromManifestResource("Icons_16x16_InternalDelegate");
            IconNumberGotoText = AddImageFromManifestResource("Icons_16x16_GotoText");
            IconNumberInternalConstant = AddImageFromManifestResource("Icons_16x16_InternalConstant");
            IconNumberPrivateConstant = AddImageFromManifestResource("Icons_16x16_PrivateConstant");
            IconNumberProtectedConstant = AddImageFromManifestResource("Icons_16x16_ProtectedConstant");
            IconNumberKeyword = AddImageFromManifestResource("Icons_16x16_Keyword");
            IconNumberEvalError = AddImageFromManifestResource("Icons_16x16_ErrorInWatch");
            IconNumberExtensionMethod = AddImageFromManifestResource("Icons_16x16_ExtensionMethod");
        }

        public int GetPictureNum(SymInfo si)
        {
            switch (si.kind)
            {

                case SymbolKind.Field:
                    switch (si.acc_mod)
                    {
                        case access_modifer.private_modifer:
                            return IconNumberPrivateField;
                        case access_modifer.protected_modifer:
                            return IconNumberProtectedField;
                        case access_modifer.internal_modifer:
                            return IconNumberInternalField;
                        default:
                            return IconNumberField;
                    }
                case SymbolKind.Method:
                    switch (si.acc_mod)
                    {
                        case access_modifer.private_modifer:
                            return IconNumberPrivateMethod;
                        case access_modifer.protected_modifer:
                            return IconNumberProtectedMethod;
                        case access_modifer.internal_modifer:
                            return IconNumberInternalMethod;
                        default:
                            if (si.description != null && si.description.Contains("(" + StringResources.Get("CODE_COMPLETION_EXTENSION")))
                                return IconNumberExtensionMethod;
                            return IconNumberMethod;
                    }
                case SymbolKind.Property:
                    switch (si.acc_mod)
                    {
                        case access_modifer.private_modifer:
                            return IconNumberPrivateProperty;
                        case access_modifer.protected_modifer:
                            return IconNumberProtectedProperty;
                        case access_modifer.internal_modifer:
                            return IconNumberInternalProperty;
                        default:
                            return IconNumberProperty;
                    }
                case SymbolKind.Event:
                    switch (si.acc_mod)
                    {
                        case access_modifer.private_modifer:
                            return IconNumberPrivateEvent;
                        case access_modifer.protected_modifer:
                            return IconNumberProtectedEvent;
                        case access_modifer.internal_modifer:
                            return IconNumberInternalEvent;
                        default:
                            return IconNumberEvent;
                    }
                case SymbolKind.Delegate:
                    switch (si.acc_mod)
                    {
                        case access_modifer.private_modifer:
                            return IconNumberPrivateDelegate;
                        case access_modifer.protected_modifer:
                            return IconNumberProtectedDelegate;
                        case access_modifer.internal_modifer:
                            return IconNumberInternalDelegate;
                        default:
                            return IconNumberDelegate;
                    }
                case SymbolKind.Variable:
                case SymbolKind.Parameter:
                    return IconNumberLocal;
                case SymbolKind.Type:
                case SymbolKind.Class:
                    return IconNumberClass;
                case SymbolKind.Namespace:
                    if (si.IsUnitNamespace)
                        return IconNumberUnitNamespace;
                    else
                        return IconNumberNamespace;
                case SymbolKind.Interface:
                    return IconNumberInterface;
                case SymbolKind.Struct:
                    return IconNumberStruct;
                case SymbolKind.Enum:
                    return IconNumberEnum;
                case SymbolKind.Constant:
                    switch (si.acc_mod)
                    {
                        case access_modifer.private_modifer:
                            return IconNumberPrivateConstant;
                        case access_modifer.protected_modifer:
                            return IconNumberProtectedConstant;
                        case access_modifer.internal_modifer:
                            return IconNumberInternalConstant;
                        default:
                            return IconNumberConstant;
                    }
            }
            return IconNumberMethod;
        }


    }
}
