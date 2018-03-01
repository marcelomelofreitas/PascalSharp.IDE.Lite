// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Text;
using PascalSharp.Internal.Errors;

namespace VisualPascalABC
{
    class RuntimeException: LocatedError
    {
        string message;
        public RuntimeException(string Message, string FileName, int ColNumber, int LineNumber)
        {
            this.fileName = FileName;
            this.message = Message;
            sourceLocation = new SourceLocation(fileName, LineNumber, ColNumber, LineNumber, ColNumber);
        }
        public override string Message
        {
            get
            {
                return ToString();
            }
        }
        public override string ToString()
        {
            return message;
        }

    }
    class ExceptionInStartEXE : LocatedError
    {
        public ExceptionInStartEXE(string FileName)
        {
            this.fileName = FileName;
            sourceLocation = null;
        }
        public override string Message
        {
            get
            {
                return ToString();
            }
        }
        public override string ToString()
        {
            return string.Format(Form1StringResources.Get("EXCEPTION_IN_START_EXE{0}"), fileName);
        }

    }
}
