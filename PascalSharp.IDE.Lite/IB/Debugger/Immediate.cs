﻿// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using PascalSharp.Internal.SyntaxTree;
using System.Text;
using PascalSharp.Internal.Debugger;

namespace VisualPascalABC
{
	public class ImmediateEvaluateAction : ICSharpCode.TextEditor.Actions.AbstractEditAction
	{
		public override void Execute(ICSharpCode.TextEditor.TextArea textArea)
		{
            if (WorkbenchServiceFactory.DebuggerManager.IsRunning)
			{
				int line = textArea.Caret.Line;
                string val = WorkbenchServiceFactory.DebuggerManager.ExecuteImmediate
					(textArea.Document.GetText(textArea.Document.GetLineSegment(textArea.Caret.Line)));
				if (val != null)
				{
					textArea.Document.TextContent += Environment.NewLine+val+Environment.NewLine;
					textArea.Caret.Line = line + 2;
					textArea.Caret.Column = 0;
				}
			}
			else
			{
				int line = textArea.Caret.Line;
				textArea.Document.TextContent += Environment.NewLine;
				textArea.Caret.Line = line + 1;
				textArea.Caret.Column = 0;
			}
		}
	}
}
