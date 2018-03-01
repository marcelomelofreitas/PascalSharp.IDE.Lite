// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PascalSharp.Internal.Localization;

namespace VisualPascalABC
{

    public class Form1StringResources
    {
        public static readonly string Prefix = "VP_MF_";
        public static string Get(string key)
        {
            return StringResources.Get(Prefix + key);
        }
        public static void SetTextForAllControls(Control c)
        {
            StringResources.SetTextForAllObjects(c, Prefix);
        }
    }
    public class RuntimeExceptionsStringResources
    {
        public static readonly string Prefix = "RUNTIME_EXCEPTION";
        public static string Get(string key)
        {
            string res=StringResources.Get(Prefix + key);
            if (res != Prefix + key)
                return res;
            return key;
        }
    }



}
